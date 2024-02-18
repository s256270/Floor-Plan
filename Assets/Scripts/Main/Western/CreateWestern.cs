using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CreateWestern : MonoBehaviour
{
    [SerializeField] CommonFunctions cf;
    
    //住戸の座標
    Vector3[] dwelling;
    //バルコニーの座標
    Vector3[] balcony;
    //配置範囲の座標
    Vector3[] range;
    //玄関の座標
    Vector3[] entrance;
    //MBPSの座標
    Vector3[] mbps;

    /// <summary>
    /// 洋室を配置したリストを返す
    /// </summary> 
    /// <param name="allPattern">配置前の全パターン</param>
    /// <returns>洋室を配置したリスト</returns>
    public List<Dictionary<string, Dictionary<string, Vector3[]>>> PlaceWestern(List<Dictionary<string, Dictionary<string, Vector3[]>>> allPattern) {
        //配置結果のリスト
        var result = new List<Dictionary<string, Dictionary<string, Vector3[]>>>();

        //各パターンについて配置
        for (int i = 0; i < allPattern.Count; i++) {
            //Debug.Log("i: " + i);
            foreach (KeyValuePair<string, Dictionary<string, Vector3[]>> planParts in allPattern[i]) {
                //住戸オブジェクトに配置していく
                if (planParts.Key.Contains("Dwelling1")) {
                    //必要な座標の準備
                    //住戸の座標を取得
                    dwelling = planParts.Value["1K"];

                    //バルコニーの座標を取得
                    balcony = planParts.Value["Balcony"];

                    //玄関の座標を取得
                    entrance = planParts.Value["Entrance"];

                    //MBPSの座標を取得
                    mbps = planParts.Value["Mbps"];

                    //配置範囲の座標を作成
                    range = FrameChange(dwelling, mbps);
                    range = FrameChange(range, entrance);

                    //cf.CreateRoom("range", range);

                    //水回りの部屋のみの辞書を作成
                    //水回りの部屋のみの辞書
                    var wetareasPattern = new Dictionary<string, Vector3[]>();
                    foreach (KeyValuePair<string, Vector3[]> planPartsElements in planParts.Value) {
                        //水回りの部屋パーツでないとき
                        if (planPartsElements.Key == "1K" || planPartsElements.Key == "Balcony" || planPartsElements.Key == "Entrance" || planPartsElements.Key == "Mbps") {
                            //スキップ
                            continue;
                        }

                        //辞書に追加
                        wetareasPattern.Add(planPartsElements.Key, planPartsElements.Value);
                        //cf.CreateRoom(planPartsElements.Key, planPartsElements.Value);
                    }

                    //洋室の座標を決定
                    Vector3[] westernCoordinates = PlaceWesternOnePattern(wetareasPattern);

                    //洋室が空でないとき
                    if (westernCoordinates.Length > 0) {
                        //洋室を追加した辞書を作成
                        //洋室を追加した辞書
                        var roomPattern = new Dictionary<string, Vector3[]>(wetareasPattern);
                        roomPattern.Add("Western", westernCoordinates);

                        //部屋の入り口前に十分なスペースがあるかを確認
                        if (SecureWidth(roomPattern)) {
                            //配置したパターン
                            Dictionary<string, Dictionary<string, Vector3[]>> patternToPlace = cf.DuplicateDictionary(allPattern[i]);

                            //洋室の座標を追加
                            patternToPlace[planParts.Key].Add("Western", westernCoordinates);

                            //配置結果に追加
                            result.Add(patternToPlace);
                        }
                    }
                }
            }
        }

        return result;
    }

    /// <summary>
    /// 洋室の座標を決定する
    /// </summary> 
    /// <param name="wetareasPattern">配置する水回りのパターン</param>
    /// <returns>洋室の座標</returns>
    public Vector3[] PlaceWesternOnePattern(Dictionary<string, Vector3[]> wetareasPattern) {
        //洋室の座標結果
        var result = new Vector3[0];

        //洋室を配置できる範囲を求める
        Vector3[] westernRange = range;
        
        foreach (KeyValuePair<string, Vector3[]> wetareasRoom in wetareasPattern) {
            //水回りの範囲から水回りの部屋を除く
            westernRange = FrameChange(westernRange, wetareasRoom.Value);
            //cf.CreateRoom("westernRange", westernRange);
        }

        //緊急措置
        //westernRangeに斜めの辺が含まれるとき洋室を配置しない
        for (int i = 0; i < westernRange.Length; i++) {
            //配置範囲の辺
            Vector3[] westernRangeSide = new Vector3[]{westernRange[i], westernRange[(i+1)%westernRange.Length]};
            //配置範囲の辺が斜めのとき
            if (cf.Slope(westernRangeSide) != 0.0f && cf.Slope(westernRangeSide) != Mathf.Infinity) {
                //Debug.Log("斜めの辺が含まれるため洋室を配置しません");
                //終了
                return result;
            }
        }

        //配置する範囲とバルコニーが接する辺を求める
        List<Vector3[]> westernStandardSideList = cf.ContactCoordinates(westernRange, balcony);

        //最も長い辺を洋室を生成する基準の辺とする
        //基準となる辺
        Vector3[] westernStandardSide = new Vector3[2];

        //最も長い辺を求める
        float maxLength = 0.0f;
        for (int i = 0; i < westernStandardSideList.Count; i++) {
            if (maxLength < Vector3.Distance(westernStandardSideList[i][0], westernStandardSideList[i][1])) {
                //最も長い辺を更新
                maxLength = Vector3.Distance(westernStandardSideList[i][0], westernStandardSideList[i][1]);
                //最も長い辺を基準となる辺とする
                westernStandardSide = westernStandardSideList[i];
            }
        }

        //洋室の座標の候補
        //List<Vector3[]> westernCoordinatesList = new List<Vector3[]>();

        //洋室の形を決める

        //洋室の限界
        float limit = 0.0f;
        int conditionalExpressionContoroler = 1;
        //洋室の基準
        float nearestWetareasCoordinates = 0.0f;
        //洋室の移動単位
        float unitLength = 10.0f;

        //基準となる辺がy軸平行の場合
        if (cf.Slope(westernStandardSide) == Mathf.Infinity) {
            //配置範囲のx座標の最大値と最小値を求める
            float max = cf.GetMaxOrMin(westernRange, "x", "max");
            float min = cf.GetMaxOrMin(westernRange, "x", "min");

            //基準となる辺が左側にあるとき
            if ((max - westernStandardSide[0].x) > (westernStandardSide[0].x - min)) {
                limit = max;

                //最も洋室と近くなる水回りの部屋を求める
                //座標の初期化
                nearestWetareasCoordinates = cf.GetMaxOrMin(wetareasPattern["UB"], "x", "min");
                //最も洋室と近くなる水回りの部屋の座標を求める
                foreach (KeyValuePair<string, Vector3[]> wetareasRoom in wetareasPattern) {
                    if (nearestWetareasCoordinates > cf.GetMaxOrMin(wetareasRoom.Value, "x", "min")) {
                        nearestWetareasCoordinates = cf.GetMaxOrMin(wetareasRoom.Value, "x", "min");
                    }
                }

                unitLength = 10.0f;
            }
            //基準となる辺が右側にあるとき
            else {
                limit = min;
                conditionalExpressionContoroler = -1;

                //最も洋室と近くなる水回りの部屋を求める
                //座標の初期化
                nearestWetareasCoordinates = cf.GetMaxOrMin(wetareasPattern["UB"], "x", "max");
                //最も洋室と近くなる水回りの部屋の座標を求める
                foreach (KeyValuePair<string, Vector3[]> wetareasRoom in wetareasPattern) {
                    if (nearestWetareasCoordinates < cf.GetMaxOrMin(wetareasRoom.Value, "x", "max")) {
                        nearestWetareasCoordinates = cf.GetMaxOrMin(wetareasRoom.Value, "x", "max");
                    }
                }

                unitLength = - 10.0f;
                
            }

            //基準となる辺が元の基準となる辺に近すぎるとき
            // if (nearestWetareasCoordinates - westernStandardSide[0].x < 2000.0f) {
            //     //終了
            //     return result;
            // }

            //基準となる辺からlimit方向へ動かして洋室範囲を決定
            for (float x = nearestWetareasCoordinates; conditionalExpressionContoroler * x <= conditionalExpressionContoroler * limit; x += unitLength) {
                //Debug.Log("x: " + x);
                //配置範囲との交点を求める
                //切り取るための交点の候補
                List<Vector3> intersectionCandidate = new List<Vector3>();
                for (int j = 0; j < westernRange.Length; j++) {
                    //配置範囲の交わっているか調べる辺
                    Vector3[] checkRangeSide = new Vector3[]{westernRange[j], westernRange[(j+1)%westernRange.Length]};
                    //交わっているか調べるための動かす辺を拡張した線分
                    Vector3[] checkExpantionSide = new Vector3[]{new Vector3(x, cf.GetMaxOrMin(dwelling, "y", "min"), 0), new Vector3(x, cf.GetMaxOrMin(dwelling, "y", "max"), 0)};
                    
                    //上の2本の線分の傾きが異なるとき
                    if (cf.Slope(checkRangeSide) != cf.Slope(checkExpantionSide)) {
                        //上の2本の線分が交わっている場合
                        if (CrossJudge(checkRangeSide, checkExpantionSide)) {
                            //交点を追加
                            intersectionCandidate.Add(Intersection(checkRangeSide, checkExpantionSide));
                        }
                    }
                }

                //洋室の範囲を決める辺になるように交点を仕分ける
                //仕分けた交点のリスト
                List<Vector3[]> intersection = new List<Vector3[]>();

                //交点をy座標でソート
                intersectionCandidate.Sort((a, b) => (int)(b.y - a.y));
                
                //交点を2つずつ確認し，中点が配置範囲に含まれるならば交点として追加
                for (int j = 0; j < intersectionCandidate.Count - 1; j++) {
                    Vector3[] currentIntersection = new Vector3[]{intersectionCandidate[j], intersectionCandidate[j+1]};

                    //交点による辺が配置範囲に接しているとき
                    if (cf.ContactJudge(westernRange, currentIntersection)) {
                        //スキップ
                        continue;
                    }

                    Vector3 middlePoint = new Vector3((currentIntersection[0].x + currentIntersection[1].x) / 2, (currentIntersection[0].y + currentIntersection[1].y) / 2, 0);

                    if (cf.JudgeInside(westernRange, new Vector3[]{middlePoint})) {
                        intersection.Add(currentIntersection);
                    }
                }


                //配置範囲を交点で切り取って洋室を作成
                //洋室の座標
                Vector3[] westernCoordinates = westernRange;
                //交点の組ごとに処理を行う
                for (int j = 0; j < intersection.Count; j++) {
                    List<Vector3[]> westernCoordinatesCandidate = Slice(westernCoordinates, intersection[j]);

                    //交点の座標が含まれる方を洋室の座標とする
                    for (int k = 0; k < westernCoordinatesCandidate.Count; k++) {
                        //if (cf.OnPolyogon(westernCoordinatesCandidate[k], intersection[j][0]) && cf.OnPolyogon(westernCoordinatesCandidate[k], intersection[j][1])) {nearestWetareasCoordinates
                        if (cf.OnPolyogon(westernCoordinatesCandidate[k], westernStandardSide)) {
                            westernCoordinates = westernCoordinatesCandidate[k];
                            break;
                        }
                    }
                }

                //洋室の面積が最大のものに更新
                bool westernDicisionFlag = false;
                //洋室を加えたパターンを作成
                var currentPattern = new Dictionary<string, Vector3[]>(wetareasPattern);
                currentPattern.Add("Western", westernCoordinates);
                //洋室が他の部屋の入口を封鎖しないかを確認
                foreach (KeyValuePair<string, Vector3[]> currentRoom in currentPattern) {
                    //調べる部屋がユニットバスと洋室の場合
                    if (currentRoom.Key.Contains("UB") || currentRoom.Key.Contains("Western")) {
                        //スキップ
                        continue;
                    }

                    //調べる部屋が洋室と接していないとき
                    if (!cf.ContactJudge(currentRoom.Value, currentPattern["Western"])) {
                        //スキップ
                        continue;
                    }

                    //部屋の入口を封鎖するとき
                    if (!SecureNecessarySide(currentRoom.Key, currentPattern)) {
                        //resultが空でないとき
                        if (result.Length > 0) {
                            //洋室が小さすぎないかを確認
                            if (cf.GetMaxOrMin(result, "x", "max") - cf.GetMaxOrMin(result, "x", "min") < 2700.0f) {
                                result = new Vector3[0];
                            }
                        }

                        //終了
                        westernDicisionFlag = true;
                        break;
                    }
                }

                //これ以上は大きくならないので終了
                if (westernDicisionFlag) {
                    break;
                }

                //洋室の座標を更新
                result = westernCoordinates;
            }
        }
        //基準となる辺がx軸平行の場合
        else if (cf.Slope(westernStandardSide) == 0.0f) {
            //配置範囲のy座標の最大値と最小値を求める
            float max = cf.GetMaxOrMin(westernRange, "y", "max");
            float min = cf.GetMaxOrMin(westernRange, "y", "min");

            //基準となる辺が下側にあるとき
            if ((max - westernStandardSide[0].y) > (westernStandardSide[0].y - min)) {
                limit = max;

                //最も洋室と近くなる水回りの部屋を求める
                //座標の初期化
                nearestWetareasCoordinates = cf.GetMaxOrMin(wetareasPattern["UB"], "y", "min");
                //最も洋室と近くなる水回りの部屋の座標を求める
                foreach (KeyValuePair<string, Vector3[]> wetareasRoom in wetareasPattern) {
                    if (nearestWetareasCoordinates > cf.GetMaxOrMin(wetareasRoom.Value, "y", "min")) {
                        nearestWetareasCoordinates = cf.GetMaxOrMin(wetareasRoom.Value, "y", "min");
                    }
                }

                unitLength = 10.0f;
            }
            //基準となる辺が上側にあるとき
            else {
                limit = min;

                //最も洋室と近くなる水回りの部屋を求める
                //座標の初期化
                nearestWetareasCoordinates = cf.GetMaxOrMin(wetareasPattern["UB"], "y", "max");
                //最も洋室と近くなる水回りの部屋の座標を求める
                foreach (KeyValuePair<string, Vector3[]> wetareasRoom in wetareasPattern) {
                    if (nearestWetareasCoordinates > cf.GetMaxOrMin(wetareasRoom.Value, "y", "max")) {
                        nearestWetareasCoordinates = cf.GetMaxOrMin(wetareasRoom.Value, "y", "max");
                    }
                }

                unitLength = - 10.0f;
                
            }

            //基準となる辺が元の基準となる辺に近すぎるとき
            if (nearestWetareasCoordinates - westernStandardSide[0].y < 2000.0f) {
                //終了
                return result;
            }

            //基準となる辺からlimit方向へ動かして洋室範囲を決定
            for (float y = nearestWetareasCoordinates; y <= limit; y += unitLength) {
                //配置範囲との交点を求める
                //切り取るための交点の候補
                List<Vector3> intersectionCandidate = new List<Vector3>();
                for (int j = 0; j < westernRange.Length; j++) {
                    //配置範囲の交わっているか調べる辺
                    Vector3[] checkRangeSide = new Vector3[]{westernRange[j], westernRange[(j+1)%westernRange.Length]};
                    //交わっているか調べるための動かす辺を拡張した線分
                    Vector3[] checkExpantionSide = new Vector3[]{new Vector3(cf.GetMaxOrMin(dwelling, "x", "min"), y, 0), new Vector3(cf.GetMaxOrMin(dwelling, "x", "max"), y, 0)};
                    
                    //上の2本の線分の傾きが異なるとき
                    if (cf.Slope(checkRangeSide) != cf.Slope(checkExpantionSide)) {
                        //上の2本の線分が交わっている場合
                        if (CrossJudge(checkRangeSide, checkExpantionSide)) {
                            //交点を追加
                            intersectionCandidate.Add(Intersection(checkRangeSide, checkExpantionSide));
                        }
                    }
                }

                //洋室の範囲を決める辺になるように交点を仕分ける
                //仕分けた交点のリスト
                List<Vector3[]> intersection = new List<Vector3[]>();

                //交点をx座標でソート
                intersectionCandidate.Sort((a, b) => (int)(b.x - a.x));
                
                //交点を2つずつ確認し，中点が配置範囲に含まれるならば交点として追加
                for (int j = 0; j < intersectionCandidate.Count - 1; j++) {
                    Vector3[] currentIntersection = new Vector3[]{intersectionCandidate[j], intersectionCandidate[j+1]};

                    //交点による辺が配置範囲に接しているとき
                    if (cf.ContactJudge(westernRange, currentIntersection)) {
                        //スキップ
                        continue;
                    }

                    Vector3 middlePoint = new Vector3((currentIntersection[0].x + currentIntersection[1].x) / 2, (currentIntersection[0].y + currentIntersection[1].y) / 2, 0);

                    if (cf.JudgeInside(westernRange, new Vector3[]{middlePoint})) {
                        intersection.Add(currentIntersection);
                    }
                }


                //配置範囲を交点で切り取って洋室を作成
                //洋室の座標
                Vector3[] westernCoordinates = westernRange;
                //交点の組ごとに処理を行う
                for (int j = 0; j < intersection.Count; j++) {
                    List<Vector3[]> westernCoordinatesCandidate = Slice(westernCoordinates, intersection[j]);

                    //交点の座標が含まれる方を洋室の座標とする
                    for (int k = 0; k < westernCoordinatesCandidate.Count; k++) {
                        //if (cf.OnPolyogon(westernCoordinatesCandidate[k], intersection[j][0]) && cf.OnPolyogon(westernCoordinatesCandidate[k], intersection[j][1])) {
                        if (cf.OnPolyogon(westernCoordinatesCandidate[k], westernStandardSide)) {
                            westernCoordinates = westernCoordinatesCandidate[k];
                            break;
                        }
                    }
                }

                //洋室の面積が最大のものに更新
                bool westernDicisionFlag = false;
                //洋室を加えたパターンを作成
                var currentPattern = new Dictionary<string, Vector3[]>(wetareasPattern);
                currentPattern.Add("Western", westernCoordinates);
                //洋室が他の部屋の入口を封鎖しないかを確認
                foreach (KeyValuePair<string, Vector3[]> currentRoom in currentPattern) {
                    //調べる部屋がユニットバスと洋室の場合
                    if (currentRoom.Key.Contains("UB") || currentRoom.Key.Contains("Western")) {
                        //スキップ
                        continue;
                    }

                    //調べる部屋が洋室と接していないとき
                    if (!cf.ContactJudge(currentRoom.Value, currentPattern["Western"])) {
                        //スキップ
                        continue;
                    }

                    //部屋の入口を封鎖するとき
                    if (!SecureNecessarySide(currentRoom.Key, currentPattern)) {
                        //resultが空でないとき
                        if (result.Length > 0) {
                            //洋室が小さすぎないかを確認
                            if (cf.GetMaxOrMin(result, "y", "max") - cf.GetMaxOrMin(result, "y", "min") < 2700.0f) {
                                result = new Vector3[0];
                            }
                        }

                        //終了
                        westernDicisionFlag = true;
                        break;
                    }
                }

                //これ以上は大きくならないので終了
                if (westernDicisionFlag) {
                    break;
                }

                //洋室の座標を更新
                result = westernCoordinates;
            }
        }

        return result;
    }

    /// <summary>
    /// 部屋の開けておなかなければならない辺が空いているかどうかを判定
    /// </summary> 
    /// <param name="checkRoom">調べる部屋</param>
    /// <param name="surroundingRooms">周りの部屋</param>
    /// <returns>部屋の開けておなかなければならない辺が空いているかどうか</returns>
    public bool SecureNecessarySide(string checkRoomName, Dictionary<string, Vector3[]> surroundingRooms) {
        //判定結果
        bool flag = true;

        //調べる部屋を1辺ずつに分割
        List<Vector3[]> checkRoomSides = new List<Vector3[]>(); //調べる部屋の辺のリスト
        Vector3[] checkRoomCoordinates = surroundingRooms[checkRoomName]; //調べる部屋の座標
        //調べる部屋の辺のリストを作成
        for (int i = 0; i < checkRoomCoordinates.Length; i++) {
            checkRoomSides.Add(new Vector3[]{checkRoomCoordinates[i], checkRoomCoordinates[(i+1)%checkRoomCoordinates.Length]});
        }

        //水回り範囲との共通部分を調べる
        //水回り範囲の各辺について
        for (int i = 0; i < range.Length; i++) {
            Vector3[] checkRangeSide = new Vector3[]{range[i], range[(i+1)%range.Length]};

            //調べる部屋の各辺について
            for (int j = 0; j < checkRoomSides.Count; j++) {
                Vector3[] checkRoomSide = checkRoomSides[j];

                //2辺が接しているとき
                if (cf.ContactJudge(checkRoomSide, checkRangeSide)) {
                    //水回り範囲との共通部分を除いた辺が存在しないとき
                    if (cf.GetLength(cf.SideSubstraction(checkRoomSide, checkRangeSide)[0]) == 0.0f) {
                        //その辺を削除
                        checkRoomSides.RemoveAt(j);
                        j--;
                    }
                    else {
                        //共通部分を除いた辺に更新
                        checkRoomSides[j] = cf.SideSubstraction(checkRoomSide, checkRangeSide)[0];
                    }
                }
            }
        }

        //周りの部屋について調べる
        foreach (KeyValuePair<string, Vector3[]> surroundingRoom in surroundingRooms) {
            //調べる部屋と周りの部屋が同じ場合
            if (surroundingRoom.Key == checkRoomName) {
                //スキップ
                continue;
            }

            //調べる部屋と周りの部屋が接しているとき
            if (cf.ContactJudge(surroundingRoom.Value, checkRoomCoordinates)) {
                //周りの部屋との共通部分につい調べる
                //周りの部屋の各辺について
                for (int i = 0; i < surroundingRoom.Value.Length; i++) {
                    Vector3[] surroundingRoomSide = new Vector3[]{surroundingRoom.Value[i], surroundingRoom.Value[(i+1)%surroundingRoom.Value.Length]};

                    //調べる部屋の各辺について
                    for (int j = 0; j < checkRoomSides.Count; j++) {
                        Vector3[] checkRoomSide = checkRoomSides[j];

                        //2辺が接しているとき
                        if (cf.ContactJudge(checkRoomSide, surroundingRoomSide)) {
                            //周りの部屋との共通部分を除いた辺が存在しないとき
                            if (cf.GetLength(cf.SideSubstraction(checkRoomSide, surroundingRoomSide)[0]) == 0.0f) {
                                //その辺を削除
                                checkRoomSides.RemoveAt(j);
                                j--;
                            }
                            else {
                                //共通部分を除いた辺に更新
                                checkRoomSides[j] = cf.SideSubstraction(checkRoomSide, surroundingRoomSide)[0];
                            }
                        }
                    }
                }
            }
        }

        //残った辺が必要な長さがあるかを調べる
        //残った辺が存在するとき
        if (checkRoomSides.Count > 0) {     
            //調べる部屋の辺の一番長いものを求める
            float checkRoomSideMax = Vector3.Distance(checkRoomSides[0][0], checkRoomSides[0][1]);
            for (int i = 0; i < checkRoomSides.Count; i++) {
                if (checkRoomSideMax < Vector3.Distance(checkRoomSides[i][0], checkRoomSides[i][1])) {
                    checkRoomSideMax = Vector3.Distance(checkRoomSides[i][0], checkRoomSides[i][1]);
                }
            }

            if (checkRoomName == "Washroom") {
                //洗面室の場合
                if (checkRoomSideMax < 900.0f) {
                    flag = false;
                }
            }
            else if (checkRoomName == "Toilet") {
                //トイレの場合
                if (checkRoomSideMax < 800.0f) {
                    flag = false;
                }
            }
            else if (checkRoomName == "Kitchen") {
                //キッチンの場合
                if (checkRoomSideMax < 2400.0f) {
                    flag = false;
                }
            }
        }
        //残った辺が存在しないとき
        else {
            if (checkRoomName == "Washroom" || checkRoomName == "Toilet" || checkRoomName == "Kitchen") {
                flag = false;
            }
        }

        return flag;
    }

    /// <summary>
    /// 部屋を利用するための幅が取れていないものを除く
    /// </summary> 
    /// <param name="roomPattern">部屋の配置パターン（座標）</param>
    /// <returns>幅が取れている場合True, そうでない場合False</returns>
    public bool SecureWidth(Dictionary<string, Vector3[]> roomPattern) {
        //判定結果
        bool flag = true;

        //調べる部屋を決める
        foreach (KeyValuePair<string, Vector3[]> checkRoom in roomPattern) {
            //調べる部屋が洗面室，トイレ，キッチン以外の場合
            if (!(checkRoom.Key == "Washroom" || checkRoom.Key == "Toilet" || checkRoom.Key == "Kitchen")) {
                //スキップ
                continue;
            }

            //調べる部屋を1辺ずつに分割
            List<Vector3[]> checkRoomSides = new List<Vector3[]>(); //調べる部屋の辺のリスト
            Vector3[] checkRoomCoordinates = roomPattern[checkRoom.Key]; //調べる部屋の座標
            //調べる部屋の辺のリストを作成
            for (int i = 0; i < checkRoomCoordinates.Length; i++) {
                checkRoomSides.Add(new Vector3[]{checkRoomCoordinates[i], checkRoomCoordinates[(i+1)%checkRoomCoordinates.Length]});
            }

            //水回り範囲との共通部分を調べる
            //水回り範囲の各辺について
            for (int i = 0; i < range.Length; i++) {
                Vector3[] checkRangeSide = new Vector3[]{range[i], range[(i+1)%range.Length]};

                //調べる部屋の各辺について
                for (int j = 0; j < checkRoomSides.Count; j++) {
                    Vector3[] checkRoomSide = checkRoomSides[j];

                    //2辺が接しているとき
                    if (cf.ContactJudge(checkRoomSide, checkRangeSide)) {
                        //水回り範囲との共通部分を除いた辺が存在しないとき
                        if (cf.GetLength(cf.SideSubstraction(checkRoomSide, checkRangeSide)[0]) == 0.0f) {
                            //その辺を削除
                            checkRoomSides.RemoveAt(j);
                            j--;
                        }
                        else {
                            //共通部分を除いた辺に更新
                            checkRoomSides[j] = cf.SideSubstraction(checkRoomSide, checkRangeSide)[0];
                        }
                    }
                }
            }

            //周りの部屋について調べる
            foreach (KeyValuePair<string, Vector3[]> surroundingRoom in roomPattern) {
                //調べる部屋と周りの部屋が同じ場合
                if (surroundingRoom.Key == checkRoom.Key) {
                    //スキップ
                    continue;
                }

                //調べる部屋と周りの部屋が接しているとき
                if (cf.ContactJudge(surroundingRoom.Value, checkRoomCoordinates)) {
                    //周りの部屋との共通部分につい調べる
                    //周りの部屋の各辺について
                    for (int i = 0; i < surroundingRoom.Value.Length; i++) {
                        Vector3[] surroundingRoomSide = new Vector3[]{surroundingRoom.Value[i], surroundingRoom.Value[(i+1)%surroundingRoom.Value.Length]};

                        //調べる部屋の各辺について
                        for (int j = 0; j < checkRoomSides.Count; j++) {
                            Vector3[] checkRoomSide = checkRoomSides[j];

                            //2辺が接しているとき
                            if (cf.ContactJudge(checkRoomSide, surroundingRoomSide)) {
                                //周りの部屋との共通部分を除いた辺が存在しないとき
                                if (cf.GetLength(cf.SideSubstraction(checkRoomSide, surroundingRoomSide)[0]) == 0.0f) {
                                    //その辺を削除
                                    checkRoomSides.RemoveAt(j);
                                    j--;
                                }
                                else {
                                    //共通部分を除いた辺に更新
                                    checkRoomSides[j] = cf.SideSubstraction(checkRoomSide, surroundingRoomSide)[0];
                                }
                            }
                        }
                    }
                }
            }

            //残った辺が必要な長さがあるかを調べる
            //残った辺が存在するとき
            if (checkRoomSides.Count > 0) {     
                //調べる部屋の辺の一番長いものを求める
                Vector3[] checkRoomSide = checkRoomSides[0];
                float checkRoomSideLength = cf.GetLength(checkRoomSides[0]);
                for (int i = 0; i < checkRoomSides.Count; i++) {
                    if (checkRoomSideLength < cf.GetLength(checkRoomSides[i])) {
                        checkRoomSide = checkRoomSides[i];
                        checkRoomSideLength = cf.GetLength(checkRoomSides[i]);
                    }
                }

                //最大辺の前のスペースに幅が取れているか調べる
                //スペースが取れているか調べるための長方形の座標
                Vector3[] judgeSquare = new Vector3[4];

                //辺がy軸平行の場合
                if (cf.Slope(checkRoomSide) == Mathf.Infinity) {
                    judgeSquare = new Vector3[]{new Vector3(-400, checkRoomSideLength/2, 0), new Vector3(400, checkRoomSideLength/2, 0), new Vector3(400, -checkRoomSideLength/2, 0), new Vector3(-400, -checkRoomSideLength/2, 0)};

                    //動かす方向
                    int wetareasShiftDirection = cf.ShiftJudge(checkRoom.Value, checkRoomSide);

                    //x座標・y座標の移動
                    //調べるための長方形を辺の左側にするとき
                    if (wetareasShiftDirection > 0) {
                        judgeSquare = cf.CorrectCoordinates(judgeSquare, new Vector3(checkRoomSide[0].x - 400, (checkRoomSide[0].y + checkRoomSide[1].y)/2, 0));
                    }
                    //部屋を辺の右側にするとき
                    else if (wetareasShiftDirection < 0) {
                        judgeSquare = cf.CorrectCoordinates(judgeSquare, new Vector3(checkRoomSide[0].x + 450, (checkRoomSide[0].y + checkRoomSide[1].y)/2, 0));
                    }
                }
                //辺がy軸平行の場合
                else if (cf.Slope(checkRoomSide) == 0.0f) {
                    judgeSquare = new Vector3[]{new Vector3(checkRoomSideLength/2, -400, 0), new Vector3(checkRoomSideLength/2, 400, 0), new Vector3(-checkRoomSideLength/2, 400, 0), new Vector3(-checkRoomSideLength/2, -400, 0)};

                    //動かす方向
                    int wetareasShiftDirection = cf.ShiftJudge(checkRoom.Value, checkRoomSide);

                    //x座標・y座標の移動
                    //調べるための長方形を辺の下側にするとき
                    if (wetareasShiftDirection > 0) {
                        judgeSquare = cf.CorrectCoordinates(judgeSquare, new Vector3((checkRoomSide[0].x + checkRoomSide[1].x)/2, checkRoomSide[0].y - 400, 0));
                    }
                    //部屋を辺の上側にするとき
                    else if (wetareasShiftDirection < 0) {
                        judgeSquare = cf.CorrectCoordinates(judgeSquare, new Vector3((checkRoomSide[0].x + checkRoomSide[1].x)/2, checkRoomSide[0].y + 400, 0));
                    }
                }

                //調べる長方形が他の部屋の外側にあるかを調べる
                foreach (KeyValuePair<string, Vector3[]> surroundingRoom in roomPattern) {
                    //調べる部屋と他の部屋が同じ場合
                    if (surroundingRoom.Key == checkRoom.Key) {
                        //スキップ
                        continue;
                    }

                    //調べる長方形が水回り範囲の内側にないとき
                    if (!cf.JudgeInside(range, judgeSquare)) {
                        flag = false;
                        return flag;
                    }
                    //調べる長方形が他の部屋の外側にないとき
                    if (!cf.JudgeOutside(surroundingRoom.Value, judgeSquare)) {
                        flag = false;
                        return flag;
                    }
                    //他の部屋が調べる長方形外側にないとき
                    if (!cf.JudgeOutside(judgeSquare, surroundingRoom.Value)) {
                        flag = false;
                        return flag;
                    }
                }
            }
            //残った辺が存在しないとき
            else {
                if (checkRoom.Key == "Washroom" || checkRoom.Key == "Toilet" || checkRoom.Key == "Kitchen") {
                    flag = false;
                    return flag;
                }
            }
        }
        
        return flag;
    }

    /***

    2本の線分の交差判定

    ***/
    public bool CrossJudge(Vector3[] lineSegmentA, Vector3[] lineSegmentB) {
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

    /// <summary>
    /// 多角形の辺上の2点を通るように切る
    /// </summary> 
    /// <param name="polygon">多角形の座標配列</param>
    /// <param name="slicePoints">切る2点の座標配列</param>
    /// <returns>切った2つの多角形の座標配列のリスト</returns>
    public List<Vector3[]> Slice(Vector3[] polygon, Vector3[] slicePoints) {
        //返すリスト
        List<Vector3[]> slicePolygons = new List<Vector3[]>();

        //多角形に切りたい点を追加
        Vector3[] polygonAddedPoints = polygon;
        polygonAddedPoints = AddPoint(polygonAddedPoints, slicePoints[0]);
        polygonAddedPoints = AddPoint(polygonAddedPoints, slicePoints[1]);

        /* 切る */
        //切る処理のためにリストに変換(removeを使うため複数用意)
        List<Vector3> polygonAddedPointsListA = polygonAddedPoints.ToList();
        List<Vector3> polygonAddedPointsListB = polygonAddedPoints.ToList();

        //切る点が含まれるインデックスを求める
        int[] slicePointsIndex = new int[] {polygonAddedPointsListA.IndexOf(slicePoints[0]), polygonAddedPointsListA.IndexOf(slicePoints[1])};
        //昇順にソート
        Array.Sort(slicePointsIndex);

        //切る点のインデックスの間を削除
        polygonAddedPointsListA.RemoveRange(slicePointsIndex[0] + 1, slicePointsIndex[1] - slicePointsIndex[0] - 1);
        //切る点のインデックスの外側を削除
        polygonAddedPointsListB.RemoveRange(slicePointsIndex[1] + 1, polygonAddedPointsListB.Count - slicePointsIndex[1] - 1); //まず後ろ側を削除(インデックスが変わってしまうため)
        polygonAddedPointsListB.RemoveRange(0, slicePointsIndex[0]); //前側を削除

        //返すリストを作成
        slicePolygons.Add(polygonAddedPointsListA.ToArray());
        slicePolygons.Add(polygonAddedPointsListB.ToArray());

        //要らない座標（頂点を含まない辺上にある座標）を除く
        for (int i = 0; i < slicePolygons.Count; i++) {
            //要らない座標を除く
            slicePolygons[i] = cf.RemoveExtraPoint(slicePolygons[i]);
        }

        return slicePolygons;
    }

    /// <summary>
    /// 多角形の辺上の点を座標配列に追加
    /// </summary> 
    /// <param name="polygon">多角形の座標配列</param>
    /// <param name="point">追加したい点</param>
    /// <returns>点を追加した座標配列</returns>
    public Vector3[] AddPoint(Vector3[] polygon, Vector3 point) {
        //返すリスト
        List<Vector3> polygonAddedPoint = polygon.ToList();

        if (polygon.Contains(point)) {
            return polygonAddedPoint.ToArray();
        }

        for (int i = 0; i < polygon.Length; i++) {
            if (cf.OnLineSegment(new Vector3[]{polygon[i], polygon[(i+1)%polygon.Length]}, point)) {
                polygonAddedPoint.Insert(i+1, point);

                return polygonAddedPoint.ToArray();
            }
        }

        return polygonAddedPoint.ToArray();
    }

    /// <summary>
    /// 外側の部屋の座標から接している内側の部屋を抜き取る
    /// </summary> 
    /// <param name="outside">外側の部屋の座標</param>
    /// <param name="inside">内側の辺の座標</param>
    /// <returns>外側の部屋の座標から接している内側の部屋を抜き取った座標配列</returns>
    public Vector3[] FrameChange(Vector3[] outside, Vector3[] inside) {
        List<Vector3> newOuter = new List<Vector3>(); //返すリスト
        Vector3 startCoordinates = new Vector3();
        bool startFlag = true;
        Vector3 endCoordinates = new Vector3();
        bool endFlag = true;

        //外側の部屋の必要な座標を追加
        //外側の部屋の辺をひとつずつ確認
        for (int i = 0; i < outside.Length; i++) {

            //内側の部屋と接していない場合
            if (!cf.ContactJudge(new Vector3[]{outside[i], outside[(i+1)%outside.Length]}, inside)) {
                newOuter.Add(outside[i]);
            }
            //内側の部屋と接している場合
            else {
                //接している辺の組み合わせを探す
                for (int j = 0; j < inside.Length; j++) {

                    //接していない辺の組み合わせの場合
                    if (!cf.ContactJudge(new Vector3[]{outside[i], outside[(i+1)%outside.Length]}, new Vector3[]{inside[j], inside[(j+1)%inside.Length]})) {
                        continue;
                    }

                    //外側と内側の辺のリスト上で先に来る座標についての処理
                    //内側の辺の頂点が外側の辺上にあり，外側の辺と内側の辺の頂点が同じでない場合
                    if (cf.OnLineSegment(new Vector3[]{outside[i], outside[(i+1)%outside.Length]}, inside[j]) && (outside[i] != inside[j])) {
                        //外側の辺の頂点を追加
                        newOuter.Add(outside[i]);

                        //内側の辺の頂点を追加
                        newOuter.Add(inside[j]);
                        //切れ目の始点に設定
                        if (startFlag) {
                            startCoordinates = inside[j];
                            startFlag = false;
                        }
                    }
                    //内側の辺の頂点が外側の辺辺上にない場合
                    else if (!cf.OnLineSegment(new Vector3[]{outside[i], outside[(i+1)%outside.Length]}, inside[j])) {
                        //外側の辺の頂点を追加
                        newOuter.Add(outside[i]);

                        //切れ目の始点に設定
                        if (startFlag) {
                            startCoordinates = outside[i];
                            startFlag = false;
                        }
                    }

                    //外側と内側の辺のリスト上で後に来る座標についての処理
                    //内側の辺の頂点が外側の辺上にあり，外側の辺と内側の辺の頂点が同じでない場合
                    if (cf.OnLineSegment(new Vector3[]{outside[i], outside[(i+1)%outside.Length]}, inside[(j+1)%inside.Length]) && (outside[(i+1)%outside.Length] != inside[(j+1)%inside.Length])) {
                        //内側の辺の頂点を追加
                        newOuter.Add(inside[(j+1)%inside.Length]);
                        //切れ目の終点に設定
                        if (endFlag) {
                            endCoordinates = inside[(j+1)%inside.Length];
                            endFlag = false;
                        }
                    }
                    //外側の辺と内側の辺の頂点が同じ場合
                    else if (!cf.OnLineSegment(new Vector3[]{outside[i], outside[(i+1)%outside.Length]}, inside[(j+1)%inside.Length]) || (outside[(i+1)%outside.Length] == inside[(j+1)%inside.Length])) {
                        //切れ目の終点に設定
                        if (endFlag) {
                            endCoordinates = outside[(i+1)%outside.Length];
                        }
                    }

                    break;
                }
            }
        }

        //内側の部屋の必要な座標を追加
        List<Vector3> needInside = new List<Vector3>();

        //内側の部屋の必要な座標を追加
        //内側の部屋の辺をひとつずつ確認
        for (int i = 0; i < inside.Length; i++) {
            //Vector3[] contactCoordinates = cf.ContactCoordinates(new Vector3[]{inside[i], inside[(i+1)%inside.Length]}, outside)[0];

            //外側の部屋と接していない場合
            if (!cf.ContactJudge(new Vector3[]{inside[i], inside[(i+1)%inside.Length]}, outside)) {
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
                    //contactCoordinates = cf.ContactCoordinates(new Vector3[]{inside[i], inside[(i+1)%inside.Length]}, new Vector3[]{outside[j], outside[(j+1)%outside.Length]})[0];

                    //接していない辺の組み合わせの場合
                    if (!cf.ContactJudge(new Vector3[]{inside[i], inside[(i+1)%inside.Length]}, new Vector3[]{outside[j], outside[(j+1)%outside.Length]})) {
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

        //外側の頂点と内側の頂点をくっつける
        if (needInside.Count != 0) {
            //endCoodinatesのミスを無理やり修正
            //newOuterに斜めの辺が含まれるとき
            for (int i = 0; i < newOuter.Count; i++) {
                //newOuterの辺
                Vector3[] newOuterSide = new Vector3[]{newOuter[i], newOuter[(i+1)%newOuter.Count]};
                if (cf.Slope(newOuterSide) != 0.0f && cf.Slope(newOuterSide) != Mathf.Infinity) {
                    if (newOuter[i] == startCoordinates) {
                        endCoordinates = newOuter[(i+1)%newOuter.Count];
                    }
                    else if (newOuter[(i+1)%newOuter.Count] == startCoordinates) {
                        endCoordinates = newOuter[i];
                    }
                    break;
                }
            }

            int outside_end_index = newOuter.IndexOf(endCoordinates);
            newOuter.InsertRange(outside_end_index, needInside);
        }

        //要らない座標（頂点を含まない辺上にある座標）を除く
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
}
