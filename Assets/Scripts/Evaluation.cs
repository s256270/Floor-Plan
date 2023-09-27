using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Evaluation : CreateRoom
{
    /// <summary>
    /// 水回りと洋室を配置した住戸に対して評価指標を用いて評価
    /// </summary> 
    /// <param name="allPattern">全パターンのリスト</param>
    /// <returns>評価の高いもののリストを返す</returns>
    public List<Dictionary<string, Dictionary<string, Vector3[]>>> EvaluateFloorPlan(List<Dictionary<string, Dictionary<string, Vector3[]>>> allPattern) {
        //評価指標によって絞ったパターンのリスト
        var selectedPattern = new List<Dictionary<string, Dictionary<string, Vector3[]>>>();

        //評価指標のリスト
        var westernSizeRatioList = new Dictionary<int, float>(); //全パターンのリストと対応付けるためのインデックスと洋室の大きさの割合の辞書
        var westernShapeRatioList = new Dictionary<int, float>(); //全パターンのリストと対応付けるためのインデックスと洋室の形状の割合の辞書

        /* 評価指標のリストを作成 */
        //全パターンについてひとつずつ評価
        for (int i = 0; i < allPattern.Count; i++) {
            //Debug.Log("i: " + i);

            foreach (KeyValuePair<string, Dictionary<string, Vector3[]>> space in allPattern[i]) {
                //住戸ごとに評価
                if (space.Key.Contains("Dwelling1")) {
                    //住戸の座標
                    Vector3[] dwellingCoordinates = allPattern[i][space.Key]["1K"];
                    //洋室の座標
                    Vector3[] westernCoordinates = allPattern[i][space.Key]["Western"];

                    /* 洋室の面積の割合を算出 */
                    //洋室の面積
                    float westernSize = areaCalculation(westernCoordinates);
                    //住戸の面積
                    float dwellingSize = areaCalculation(dwellingCoordinates);
                    //洋室の面積の割合
                    float westernSizeRatio = westernSize / dwellingSize;
                    //洋室の面積の割合のリストに追加
                    westernSizeRatioList.Add(i, westernSizeRatio);

                    /* 洋室の形状を割合として算出 */
                    //洋室の座標のx座標の最大値・最小値
                    float westernMaxX = westernCoordinates[0].x;
                    float westernMinX = westernCoordinates[0].x;

                    //洋室の座標のy座標の最大値・最小値
                    float westernMaxY = westernCoordinates[0].y;
                    float westernMinY = westernCoordinates[0].y;

                    //洋室の座標のx,y座標の最大値・最小値を求める
                    for (int j = 0; j < westernCoordinates.Length; j++) {
                        if (westernMaxX < westernCoordinates[j].x) {
                            westernMaxX = westernCoordinates[j].x;
                        }

                        if (westernCoordinates[j].x < westernMinX) {
                            westernMinX = westernCoordinates[j].x;
                        }

                        if (westernMaxY < westernCoordinates[j].y) {
                            westernMaxY = westernCoordinates[j].y;
                        }

                        if (westernCoordinates[j].y < westernMinY) {
                            westernMinY = westernCoordinates[j].y;
                        }
                    }

                    //洋室の形状の割合
                    float westernShapeRatio =  westernSize / ((westernMaxX - westernMinX) * (westernMaxY - westernMinY));
                    //洋室の形状の割合のリストに追加
                    westernShapeRatioList.Add(i, westernShapeRatio);
                }
            }
        }

        /* 得点の算出 */
        //洋室の面積の割合に洋室の形状の割合を掛ける
        var scoreList = new Dictionary<int, float>();
        foreach (KeyValuePair<int, float> kvp in westernSizeRatioList) {
            scoreList.Add(kvp.Key, kvp.Value * westernShapeRatioList[kvp.Key]);
        }

        //評価指標の辞書を評価指標の降順にソート
        var scoreListSort = scoreList.OrderByDescending((x) => x.Value);
        

        // ↓↓↓ここから先の手法は要検討

        //評価指標の辞書のインデックスに合わせて全パターンのリストをソート
        var allPatternSort = new List<Dictionary<string, Dictionary<string, Vector3[]>>>();
        foreach (KeyValuePair<int, float> kvp in scoreListSort) {
            allPatternSort.Add(allPattern[kvp.Key]);
        }
        

        /* 評価指標のリストをもとにパターンを絞る */
        //全パターンのリストのうち，前から20個を選択
        for (int i = 0; i < 20/*allPatternSort.Count*/; i++) {
            selectedPattern.Add(allPatternSort[i]);
        }

        return selectedPattern;
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

        //内側の部屋の必要な座標を追加
        List<Vector3> needInside = new List<Vector3>();

        /* 内側の部屋の必要な座標を追加 */
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

    /// <summary>
    /// ある点がある線分上にあるかどうかの判定
    /// </summary> 
    /// <param name="side">線分の端点の座標配列</param>
    /// <param name="point">判定する点の座標</param>
    /// <returns>点が線分上にある場合True，ない場合Flase</returns>
    public bool OnLineSegment(Vector3[] side, Vector3 point) {
        if ((side[0].x <= point.x && point.x <= side[1].x) || (side[1].x <= point.x && point.x <= side[0].x)) {
            if ((side[0].y <= point.y && point.y <= side[1].y) || (side[1].y <= point.y && point.y <= side[0].y)) {
                if ((point.y * (side[0].x - side[1].x)) + (side[0].y * (side[1].x - point.x)) + (side[1].y * (point.x - side[0].x)) == 0) {
                    return true;
                }
            }
        }

        return false;
    }

    public Vector3[] NumberClean (Vector3[] coodinates) {
        //返すリスト
        Vector3[] cleanCoodinates = new Vector3[coodinates.Length];

        for (int i = 0; i < coodinates.Length; i++) {
            cleanCoodinates[i].x = (float) Math.Truncate(coodinates[i].x);
            cleanCoodinates[i].y = (float) Math.Truncate(coodinates[i].y);
        }

        return cleanCoodinates;
    }
}
