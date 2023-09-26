using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CreateWestern : FloorPlanner
{
    Vector3[] dwelling;
    Vector3[] balcony;
    Vector3[] range;
    Vector3[] entrance;
    Vector3[] mbps;

    /// <summary>
    /// 洋室を配置したリストを返す
    /// </summary> 
    /// <param name="allPattern">配置前の全パターン</param>
    /// <returns>洋室を配置したリスト</returns>
    public List<Dictionary<string, Dictionary<string, Vector3[]>>> PlaceWestern(List<Dictionary<string, Dictionary<string, Vector3[]>>> allPattern) {
        //配置結果のリスト
        var result = new List<Dictionary<string, Dictionary<string, Vector3[]>>>();

        /* 全パターンについて配置 */
        for (int i = 0; i < allPattern.Count; i++) {
            foreach (KeyValuePair<string, Dictionary<string, Vector3[]>> space in allPattern[i]) {
                //住戸オブジェクトに配置していく
                if (space.Key.Contains("Dwelling1")) {
                    /* 必要な座標の抜き出し */
                    //住戸の座標を取得
                    dwelling = allPattern[i][space.Key]["1K"];

                    //バルコニーの座標を取得
                    balcony = allPattern[i][space.Key]["Balcony"];

                    //玄関の座標を取得
                    entrance = allPattern[i][space.Key]["Entrance"];

                    //MBPSの座標を取得
                    mbps = allPattern[i][space.Key]["Mbps"];

                    //配置範囲の座標を作成
                    range = FrameChange(dwelling, entrance);
                    range = FrameChange(range, mbps);

                    /* 水回りの部屋のみの辞書を作成 */
                    //水回りの部屋のみの辞書
                    var wetAreasPattern = new Dictionary<string, Vector3[]>();
                    foreach (KeyValuePair<string, Vector3[]> spaceElements in space.Value) {
                        //水回りの部屋パーツでないとき
                        if (spaceElements.Key == "1K" || spaceElements.Key == "Balcony" || spaceElements.Key == "Entrance" || spaceElements.Key == "Mbps") {
                            //スキップ
                            continue;
                        }

                        //辞書に追加
                        wetAreasPattern.Add(spaceElements.Key, spaceElements.Value);
                    }

                    /* 洋室を配置 */
                    List<Dictionary<string, Vector3[]>> placeWesternResult = PlaceWesternOnePattern(wetAreasPattern);

                    for (int j = 0; j < placeWesternResult.Count; j++) {
                        //配置したパターン
                        Dictionary<string, Dictionary<string, Vector3[]>> patternToPlace = DuplicateDictionary(allPattern[i]);

                        foreach (KeyValuePair<string, Vector3[]> western in placeWesternResult[j]) {
                            if (western.Key == "Western") {
                                patternToPlace[space.Key].Add(western.Key, western.Value);
                            }
                        }

                        result.Add(patternToPlace);
                    }
                }
            }
        }

        return result;
    }

    /***

    洋室の形を決定

    ***/
    public List<Dictionary<string, Vector3[]>> PlaceWesternOnePattern(Dictionary<string, Vector3[]> dictionary) {
        var result = new List<Dictionary<string, Vector3[]>>();

        //洋室を配置できる範囲を求める
        Vector3[] western_range = range;
        
        foreach (Vector3[] value in dictionary.Values) {
            //水回りの範囲から水回りの部屋を除く
            western_range = FrameChange(western_range, value);
        }

        //配置する範囲とバルコニーが接する辺を求める
        List<Vector3[]> westernStandardside = new List<Vector3[]>();

        for (int i = 0; i < western_range.Length; i++) {
            Vector3[] balcony_contact = contact(new Vector3[]{western_range[i], western_range[(i+1)%western_range.Length]}, balcony);
            if (!ZeroJudge(balcony_contact)) {
                westernStandardside.Add(new Vector3[]{western_range[i], western_range[(i+1)%western_range.Length]});
            }
        }

        //洋室の座標
        List<Vector3[]> western = new List<Vector3[]>();

        //洋室の形を決める
        //基準となる辺がy軸平行の場合
        if (westernStandardside[0][0].x == westernStandardside[0][1].x) {
            //配置範囲のx座標の最大値と最小値を求める
            float max = western_range[0].x;
            float min = western_range[0].x;
            for (int i = 1; i < western_range.Length; i++) {
                if (max < western_range[i].x) {
                    max = western_range[i].x;
                }

                if (western_range[i].x < min) {
                    min = western_range[i].x;
                }
            }

            //動かす辺の上限
            float limit = 0.0f;
            if ((max - westernStandardside[0][0].x) > (westernStandardside[0][0].x - min)) {
                limit = max;

                //基準となる辺からlimit方向へ動かして洋室範囲を決定
                for (float x = westernStandardside[0][0].x + 10.0f; x <= limit; x += 10.0f) {
                    //Debug.Log("x: " + x);
                    /* 配置範囲との交点を求める */
                    //切り取るための交点の候補
                    List<Vector3> intersectionCandidate = new List<Vector3>();
                    for (int j = 0; j < western_range.Length; j++) {
                        //配置範囲の交わっているか調べる辺
                        Vector3[] checkRangeSide = new Vector3[]{western_range[j], western_range[(j+1)%western_range.Length]};
                        //交わっているか調べるための動かす辺を拡張した線分
                        Vector3[] checkExpantionSide = new Vector3[]{new Vector3(x, 20000f, 0), new Vector3(x, -20000f, 0)};
                        
                        //上の4点が一直線上にない場合
                        if (!OnLineSegment(checkExpantionSide, checkRangeSide[0]) || !OnLineSegment(checkExpantionSide, checkRangeSide[1])) {
                            //上の2本の線分が交わっている場合
                            if (CrossJudge(checkRangeSide, checkExpantionSide)) {
                                //交点を追加
                                intersectionCandidate.Add(Intersection(checkRangeSide, checkExpantionSide));
                            }
                        }
                    }

                    /* 洋室の範囲を決める辺になるように交点を仕分ける */
                    //仕分けた交点のリスト
                    List<Vector3[]> intersection = new List<Vector3[]>();

                    //交点をy座標でソート
                    intersectionCandidate.Sort((a, b) => (int)(b.y - a.y));
                    
                    //交点を2つずつ確認し，中点が配置範囲に含まれるならば交点として追加
                    for (int j = 0; j < intersectionCandidate.Count - 1; j++) {
                        Vector3[] currentIntersection = new Vector3[]{intersectionCandidate[j], intersectionCandidate[j+1]};
                        Vector3 middlePoint = new Vector3((currentIntersection[0].x + currentIntersection[1].x) / 2, (currentIntersection[0].y + currentIntersection[1].y) / 2, 0);

                        if (JudgeInside(western_range, new Vector3[]{middlePoint})) {
                            intersection.Add(currentIntersection);
                        }
                    }

                    /* 配置範囲を交点で切り取る */
                    Vector3[] rangeAddedIntersection = western_range;

                    //交点の組ごとに処理を行う
                    for (int j = 0; j < intersection.Count; j++) {
                        //配置範囲に交点を追加
                        rangeAddedIntersection = AddPoint(rangeAddedIntersection, intersection[j][0]);
                        rangeAddedIntersection = AddPoint(rangeAddedIntersection, intersection[j][1]);

                        List<Vector3> rangeAddedIntersectionList = new List<Vector3>(rangeAddedIntersection.ToList());

                        //追加した交点のインデックス
                        int smallerIntersectionIndex =  rangeAddedIntersectionList.IndexOf(intersection[j][0]);
                        int biggerIntersectionIndex =  rangeAddedIntersectionList.IndexOf(intersection[j][1]);
                        
                        if (smallerIntersectionIndex > biggerIntersectionIndex) {
                            int temp = smallerIntersectionIndex;
                            smallerIntersectionIndex = biggerIntersectionIndex;
                            biggerIntersectionIndex = temp;
                        }

                        //基準となる辺のインデックス
                        int westernStandardsideIndexA =  rangeAddedIntersectionList.IndexOf(westernStandardside[0][0]);
                        int westernStandardsideIndexB =  rangeAddedIntersectionList.IndexOf(westernStandardside[0][1]);

                        //基準となる辺のインデックスが交点の小さい方のインデックスと大きい方のインデックスの間にある場合
                        if ((smallerIntersectionIndex < westernStandardsideIndexA) && (westernStandardsideIndexA < biggerIntersectionIndex)) {
                            if ((smallerIntersectionIndex < westernStandardsideIndexB) && (westernStandardsideIndexB < biggerIntersectionIndex)) {
                                //間以外を削除
                                rangeAddedIntersectionList.RemoveRange(biggerIntersectionIndex + 1, rangeAddedIntersectionList.Count - biggerIntersectionIndex - 1);
                                rangeAddedIntersectionList.RemoveRange(0, smallerIntersectionIndex);
                            }
                        }
                        //基準となる辺のインデックスが交点の小さい方のインデックスと大きい方のインデックスの間にない場合
                        else if ((westernStandardsideIndexA < smallerIntersectionIndex) || (biggerIntersectionIndex < westernStandardsideIndexA)) {
                            if ((westernStandardsideIndexB < smallerIntersectionIndex) || (biggerIntersectionIndex < westernStandardsideIndexB)) {
                                //間を削除
                                rangeAddedIntersectionList.RemoveRange(smallerIntersectionIndex + 1, biggerIntersectionIndex - smallerIntersectionIndex - 1);
                            }
                        }

                        rangeAddedIntersection = rangeAddedIntersectionList.ToArray();
                    }

                    /* 洋室の面積が最大のものを追加 */

                    //洋室を現在のパターンに追加
                    var currentWetAreasPattern = new Dictionary<string, Vector3[]>(dictionary);
                    currentWetAreasPattern.Add("Western", rangeAddedIntersection);
                    
                    bool westernDicisionFlag = false;
                    //洋室の面積が他の部屋の入口を封鎖しないもので，最大のものを追加
                    foreach (string key in currentWetAreasPattern.Keys) {
                        //調べる部屋がユニットバスの場合
                        if (key == "UB" || key == "Western") {
                            //スキップ
                            continue;
                        }

                        //部屋の入口を封鎖しないか調べる
                        if (!SecureNecessarySide(key, currentWetAreasPattern)) {
                            //洋室の面積が最大のもの以外を削除する
                            if (result.Count > 0) {
                                result.RemoveRange(0, result.Count - 1);
                            }

                            westernDicisionFlag = true;

                            break;
                        }
                    }

                    //洋室の面積が最大のものが見つかったら終了
                    if (westernDicisionFlag) {
                        break;
                    }

                    //洋室を追加したものをリストに追加
                    result.Add(currentWetAreasPattern);
                }
            }
            else {
                limit = min;
            }

        }
        //基準となる辺がx軸平行の場合
        else if (westernStandardside[0][0].y == westernStandardside[0][1].y) {

        }

        return result;
    }

    /// <summary>
    /// 部屋の開けておなかなければならない辺が空いているかどうかを判定
    /// </summary> 
    /// <param name="checkRoom">調べる部屋</param>
    /// <param name="surroundingRooms">周りの部屋</param>
    /// <returns>評価の高いもののリストを返す</returns>
    public bool SecureNecessarySide(string checkRoom, Dictionary<string, Vector3[]> surroundingRooms) {
        //判定結果
        bool flag = true;

        /* 調べる部屋を1辺ずつに分割 */
        List<Vector3[]> checkRoomSides = new List<Vector3[]>(); //調べる部屋の辺のリスト
        Vector3[] checkRoomCoordinates = surroundingRooms[checkRoom]; //調べる部屋の座標
        //調べる部屋の辺のリストを作成
        for (int i = 0; i < checkRoomCoordinates.Length; i++) {
            checkRoomSides.Add(new Vector3[]{checkRoomCoordinates[i], checkRoomCoordinates[(i+1)%checkRoomCoordinates.Length]});
        }

        //水回り範囲との共通部分を調べる
        for (int i = 0; i < range.Length; i++) {
            int checkRoomSidesLength = checkRoomSides.Count;
            for (int j = 0; j < checkRoomSidesLength; j++) {
                //水回り範囲との共通部分を除く
                checkRoomSides[j] = SideSubstraction(checkRoomSides[j], new Vector3[]{range[i], range[(i+1)%range.Length]})[0];
                if (SideSubstraction(checkRoomSides[j], new Vector3[]{range[i], range[(i+1)%range.Length]}).Count == 2) {
                    checkRoomSides.Add(SideSubstraction(checkRoomSides[j], new Vector3[]{range[i], range[(i+1)%range.Length]})[1]);
                }
            }
        }

        //周りの部屋について調べる
        foreach (string key in surroundingRooms.Keys) {
            //調べる部屋と周りの部屋が同じ場合
            if (key == checkRoom) {
                //スキップ
                continue;
            }

            //周りの部屋との共通部分につい調べる
            for (int i = 0; i < surroundingRooms[key].Length; i++) {
                int checkRoomSidesLength = checkRoomSides.Count;
                for (int j = 0; j < checkRoomSidesLength; j++) {
                    //水回り範囲との共通部分を除く
                    checkRoomSides[j] = SideSubstraction(checkRoomSides[j], new Vector3[]{surroundingRooms[key][i], surroundingRooms[key][(i+1)%surroundingRooms[key].Length]})[0];
                    if (SideSubstraction(checkRoomSides[j], new Vector3[]{surroundingRooms[key][i], surroundingRooms[key][(i+1)%surroundingRooms[key].Length]}).Count == 2) {
                        checkRoomSides.Add(SideSubstraction(checkRoomSides[j], new Vector3[]{surroundingRooms[key][i], surroundingRooms[key][(i+1)%surroundingRooms[key].Length]})[1]);
                    }
                }
            }
        }

        /* 残った辺が必要な長さがあるかを調べる */
        //調べる部屋の辺の一番長いものを求める
        float checkRoomSideMax = Vector3.Distance(checkRoomSides[0][0], checkRoomSides[0][1]);
        for (int i = 0; i < checkRoomSides.Count; i++) {
            if (checkRoomSideMax < Vector3.Distance(checkRoomSides[i][0], checkRoomSides[i][1])) {
                checkRoomSideMax = Vector3.Distance(checkRoomSides[i][0], checkRoomSides[i][1]);
            }
        }

        if (checkRoom == "Washroom") {
            //洗面室の場合
            if (checkRoomSideMax < 900.0f) {
                flag = false;
            }
        }
        else if (checkRoom == "Toilet") {
            //トイレの場合
            if (checkRoomSideMax < 800.0f) {
                flag = false;
            }
        }
        else if (checkRoom == "Kitchen") {
            //キッチンの場合
            if (checkRoomSideMax < 2400.0f) {
                flag = false;
            }
        }

        return flag;
    }

    /***

    2本の線分の交点を求める

    ***/
    public Vector3 Intersection(Vector3[] lineSegmentA, Vector3[] lineSegmentB) {
        //線分Aの座標
        Vector3 p1 = lineSegmentA[0];
        Vector3 p2 = lineSegmentA[1];
        
        //線分Bの座標
        Vector3 p3 = lineSegmentB[0];
        Vector3 p4 = lineSegmentB[1];

        //端点で交わる場合
        if (Vector3.Distance(p1, p3) + Vector3.Distance(p3, p2) == Vector3.Distance(p1, p2)) {
            return p3;
        }
        else if (Vector3.Distance(p1, p4) + Vector3.Distance(p4, p2) == Vector3.Distance(p1, p2)) {
            return p4;
        }
        else if (Vector3.Distance(p3, p1) + Vector3.Distance(p1, p4) == Vector3.Distance(p3, p4)) {
            return p1;
        }
        else if (Vector3.Distance(p3, p2) + Vector3.Distance(p2, p4) == Vector3.Distance(p3, p4)) {
            return p2;
        }

        //交点を求めるための計算
        float det = (p1.x - p2.x) * (p4.y - p3.y) - (p4.x - p3.x) * (p1.y - p2.y); //行列式
        float t = ((p4.y - p3.y) * (p4.x - p2.x) + (p3.x - p4.x) * (p4.y - p2.y)) / det;
        //交点の座標
        float x = t * p1.x + (1.0f - t) * p2.x;
        float y = t * p1.y + (1.0f - t) * p2.y;
        
        return new Vector3(x, y, 0);
    }

    /***

    多角形の辺上の点を座標配列に追加

    ***/
    public Vector3[] AddPoint(Vector3[] polygon, Vector3 point) {
        //返すリスト
        List<Vector3> polygonAddedPoint = polygon.ToList();

        if (polygon.Contains(point)) {
            return polygonAddedPoint.ToArray();
        }

        for (int i = 0; i < polygon.Length; i++) {
            if (OnLineSegment(new Vector3[]{polygon[i], polygon[(i+1)%polygon.Length]}, point)) {
                polygonAddedPoint.Insert(i+1, point);

                return polygonAddedPoint.ToArray();
            }
        }

        return polygonAddedPoint.ToArray();
    }

    /***

    外側の部屋の座標から接している内側の部屋を抜き取った座標

    ***/
    public Vector3[] FrameChange(Vector3[] outside, Vector3[] inside) {
        List<Vector3> newOuter = new List<Vector3>(); //返すリスト
        Vector3 start_coordinates = new Vector3();
        bool start_flag = true;
        Vector3 end_coordinates = new Vector3();
        bool end_flag = true;

        /* 外側の部屋の必要な座標を追加 */
        //外側の部屋の辺をひとつずつ確認
        for (int i = 0; i < outside.Length; i++) {
            Vector3[] contact_coordinates = contact(new Vector3[]{outside[i], outside[(i+1)%outside.Length]}, inside);

            //内側の部屋と接していない場合
            if (ZeroJudge(contact_coordinates)) {
                newOuter.Add(outside[i]);
            }
            //内側の部屋と接している場合
            else {
                //接している辺の組み合わせを探す
                for (int j = 0; j < inside.Length; j++) {
                    contact_coordinates = ContactSide(new Vector3[]{outside[i], outside[(i+1)%outside.Length]}, new Vector3[]{inside[j], inside[(j+1)%inside.Length]});

                    //接していない辺の組み合わせの場合
                    if (ZeroJudge(contact_coordinates)) {
                        continue;
                    }

                    //外側と内側の辺のリスト上で先に来る座標についての処理
                    //内側の辺の頂点が外側の辺上にあり，外側の辺と内側の辺の頂点が同じでない場合
                    if (OnLineSegment(new Vector3[]{outside[i], outside[(i+1)%outside.Length]}, inside[j]) && (outside[i] != inside[j])) {
                        //外側の辺の頂点を追加
                        newOuter.Add(outside[i]);

                        //内側の辺の頂点を追加
                        newOuter.Add(inside[j]);
                        //切れ目の始点に設定
                        if (start_flag) {
                            start_coordinates = inside[j];
                            start_flag = false;
                        }
                    }
                    //内側の辺の頂点が外側の辺辺上にない場合
                    else if (!OnLineSegment(new Vector3[]{outside[i], outside[(i+1)%outside.Length]}, inside[j])) {
                        //外側の辺の頂点を追加
                        newOuter.Add(outside[i]);

                        //切れ目の始点に設定
                        if (start_flag) {
                            start_coordinates = outside[i];
                            start_flag = false;
                        }
                    }

                    //外側と内側の辺のリスト上で後に来る座標についての処理
                    //内側の辺の頂点が外側の辺上にあり，外側の辺と内側の辺の頂点が同じでない場合
                    if (OnLineSegment(new Vector3[]{outside[i], outside[(i+1)%outside.Length]}, inside[(j+1)%inside.Length]) && (outside[(i+1)%outside.Length] != inside[(j+1)%inside.Length])) {
                        //内側の辺の頂点を追加
                        newOuter.Add(inside[(j+1)%inside.Length]);
                        //切れ目の終点に設定
                        if (end_flag) {
                            end_coordinates = inside[(j+1)%inside.Length];
                            end_flag = false;
                        }
                    }
                    //外側の辺と内側の辺の頂点が同じ場合
                    else if (!OnLineSegment(new Vector3[]{outside[i], outside[(i+1)%outside.Length]}, inside[(j+1)%inside.Length]) || (outside[(i+1)%outside.Length] == inside[(j+1)%inside.Length])) {
                        //切れ目の終点に設定
                        if (end_flag) {
                            end_coordinates = outside[(i+1)%outside.Length];
                        }
                    }

                    break;
                }
            }
        }

        /* 内側の部屋の必要な座標を追加 */
        //内側の部屋の必要な座標
        List<Vector3> needInside = new List<Vector3>();

        //内側の部屋の辺をひとつずつ確認
        for (int i = 0; i < inside.Length; i++) {
            Vector3[] contact_coordinates = contact(new Vector3[]{inside[i], inside[(i+1)%inside.Length]}, outside);

            //外側の部屋と接していない場合
            if (ZeroJudge(contact_coordinates)) {
                //リストになければ内側の辺の頂点を追加
                if (!newOuter.Contains(inside[i])) {
                    if (!needInside.Contains(inside[i])) {
                        needInside.Add(inside[i]);
                    }
                }
            }
            //外側の部屋と接している場合
            else {
                //接している辺の組み合わせを探す
                for (int j = 0; j < outside.Length; j++) {
                    contact_coordinates = ContactSide(new Vector3[]{inside[i], inside[(i+1)%inside.Length]}, new Vector3[]{outside[j], outside[(j+1)%outside.Length]});

                    //接していない辺の組み合わせの場合
                    if (ZeroJudge(contact_coordinates)) {
                        continue;
                    }
                    
                    //内側の辺の先に来る端点が外側の辺の範囲外にある場合
                    if (Vector3.Distance(outside[j], inside[i]) + Vector3.Distance(inside[i], outside[(j+1)%outside.Length]) > Vector3.Distance(outside[j], outside[(j+1)%outside.Length])) {
                        //その端点を追加
                        needInside.Add(inside[i]);
                    }

                    //内側の辺の後に来る端点が外側の辺の範囲外にある場合
                    if (Vector3.Distance(outside[j], inside[(i+1)%inside.Length]) + Vector3.Distance(inside[(i+1)%inside.Length], outside[(j+1)%outside.Length]) > Vector3.Distance(outside[j], outside[(j+1)%outside.Length])) {
                        //その端点を追加
                        needInside.Add(inside[(i+1)%inside.Length]);
                    }

                    break;
                }
            }
        }

        //内側の必要な頂点が2つで，元の内側の部屋の始点と終点の場合のみ順番を入れ替えない
        if (!((needInside.Count == 2) && ((Array.IndexOf(inside, needInside[0]) == 0) && (Array.IndexOf(inside, needInside[1]) == inside.Length - 1)))) {
            needInside.Reverse();
        }

        /* 外側の頂点と内側の頂点をくっつける */
        if (needInside.Count != 0) {
            int outside_end_index = newOuter.IndexOf(end_coordinates);
            newOuter.InsertRange(outside_end_index, needInside);
        }

        /* 要らない座標（頂点を含まない辺上にある座標）を除く */
        for (int i = 0; i < newOuter.Count; i++) {
            //調べる辺
            Vector3[] sideA = new Vector3[]{newOuter[(i+1)%newOuter.Count], newOuter[i]};
            Vector3[] sideB = new Vector3[]{newOuter[(i+1)%newOuter.Count], newOuter[(i+2)%newOuter.Count]};

            //座標配列で連続する2辺の内積を計算
            if (Vector3.Dot(sideA[1] - sideA[0], sideB[1] - sideB[0]) == -Vector3.Distance(sideA[1], sideA[0]) * Vector3.Distance(sideB[1], sideB[0])) {
                //座標配列から削除
                newOuter.Remove(sideA[0]);
                i = -1;
            }
        }


        return newOuter.ToArray();
    }

    /***

    2本の線分の交差判定

    ***/
    public bool CrossJudge(Vector3[] lineSegmentA, Vector3[] lineSegmentB)
    {
        double s, t;

        s = (lineSegmentA[0].x - lineSegmentA[1].x) * (lineSegmentB[0].y - lineSegmentA[0].y) - (lineSegmentA[0].y - lineSegmentA[1].y) * (lineSegmentB[0].x - lineSegmentA[0].x);
        t = (lineSegmentA[0].x - lineSegmentA[1].x) * (lineSegmentB[1].y - lineSegmentA[0].y) - (lineSegmentA[0].y - lineSegmentA[1].y) * (lineSegmentB[1].x - lineSegmentA[0].x);
        if (s * t > 0) {
            return false;
        }

        s = (lineSegmentB[0].x - lineSegmentB[1].x) * (lineSegmentA[0].y - lineSegmentB[0].y) - (lineSegmentB[0].y - lineSegmentB[1].y) * (lineSegmentA[0].x - lineSegmentB[0].x);
        t = (lineSegmentB[0].x - lineSegmentB[1].x) * (lineSegmentA[1].y - lineSegmentB[0].y) - (lineSegmentB[0].y - lineSegmentB[1].y) * (lineSegmentA[1].x - lineSegmentB[0].x);
        if (s * t > 0) {
            return false;
        }

        return true;
    }
}
