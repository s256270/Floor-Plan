using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Evaluation : CreateRoom
{
    [SerializeField] CommonFunctions cf;

    /// <summary>
    /// 水回りと洋室を配置した住戸に対して評価指標を用いて評価
    /// </summary> 
    /// <param name="allPattern">全パターンのリスト</param>
    /// <returns>評価の高いもののリストを返す</returns>
    public List<Dictionary<string, Dictionary<string, Vector3[]>>> EvaluateFloorPlan(List<Dictionary<string, Dictionary<string, Vector3[]>>> allPattern) {
        //評価指標によって絞ったパターンのリスト
        var result = new List<Dictionary<string, Dictionary<string, Vector3[]>>>();

        // ↓↓↓ここから先の手法は要検討↓↓↓

        //評価指標のリスト
        var westernSizeList = new List<float>(); //洋室の大きさのリスト
        var westernShapeRatioList = new List<float>(); //洋室の形状の割合のリスト
        var mbpsToiletDistanceList = new List<int>(); //MBPSとトイレの距離のリスト
        var kitchenWallRelationList = new List<int>(); //キッチンと外壁面の関係のリスト

        //評価指標のリストを作成
        //全パターンについてひとつずつ評価
        for (int i = 0; i < allPattern.Count; i++) {
            //Debug.Log("i: " + i);

            foreach (KeyValuePair<string, Dictionary<string, Vector3[]>> planParts in allPattern[i]) {
                //住戸ごとに評価
                if (planParts.Key.Contains("Dwelling4")) {
                    //必要な座標の準備
                    //住戸の座標を取得
                    Vector3[] dwellingCoordinates = planParts.Value["1K"];
                    //洋室の座標を取得
                    Vector3[] westernCoordinates = planParts.Value["Western"];
                    //MBPSの座標を取得
                    Vector3[] mbps = planParts.Value["Mbps"];
                    //トイレの座標を取得
                    Vector3[] toilet = planParts.Value["Toilet"];
                    //キッチンの座標を取得
                    Vector3[] kitchen = planParts.Value["Kitchen"];

                    //洋室の面積を算出
                    //洋室の面積
                    float westernSize = areaCalculation(westernCoordinates);
                    //洋室の面積のリストに追加
                    westernSizeList.Add(westernSize);

                    //洋室の形状の割合を算出
                    //洋室の形状の割合
                    float westernShapeRatio =  westernSize / (cf.CalculateRectangleHeight(westernCoordinates) * cf.CalculateRectangleWidth(westernCoordinates));
                    //洋室の形状の割合のリストに追加
                    westernShapeRatioList.Add(westernShapeRatio);

                    //MBPSとトイレの距離を算出
                    //MBPSの中心点
                    Vector3 mbpsCenter = new Vector3(cf.CalculateRectangleWidth(mbps) / 2, cf.CalculateRectangleHeight(mbps) / 2, 0);
                    //トイレの中心点
                    Vector3 toiletCenter = new Vector3(cf.CalculateRectangleWidth(toilet) / 2, cf.CalculateRectangleHeight(toilet) / 2, 0);
                    //MBPSとトイレの距離
                    float mbpsToiletDistance = Mathf.Sqrt(Mathf.Pow(mbpsCenter.x - toiletCenter.x, 2) + Mathf.Pow(mbpsCenter.y - toiletCenter.y, 2));
                    //MBPSとトイレの距離のリストに0, 1の値を追加
                    if (mbpsToiletDistance < 2000) {
                        mbpsToiletDistanceList.Add(1);
                    } else {
                        mbpsToiletDistanceList.Add(0);
                    }

                    //キッチンと外壁面の関係
                    //住戸の各辺について
                    for (int j = 0; j < dwellingCoordinates.Length; j++) {
                        //住戸の辺
                        Vector3[] dwellingSide = new Vector3[] {dwellingCoordinates[j], dwellingCoordinates[(j + 1) % dwellingCoordinates.Length]};

                        //キッチンと接しているとき
                        if (cf.ContactJudge(kitchen, dwellingSide)) {
                            //それが外壁面のとき
                            if (i == 0) {
                                //キッチンと外壁面の関係のリストに1を追加
                                kitchenWallRelationList.Add(1);
                            } else {
                                //キッチンと外壁面の関係のリストに0を追加
                                kitchenWallRelationList.Add(0);
                            }
                        } else {
                            //キッチンと外壁面の関係のリストに0を追加
                            kitchenWallRelationList.Add(0);
                        }
                    }
                }
            }
        }

        //得点の算出
        //間取図のインデックスと得点のリスト
        var scoreList = new Dictionary<int, float>();
        
        for (int i = 0; i < westernSizeList.Count; i++) {
            //得点
            float score = 0.0f;

            //洋室の面積に形状の割合を掛ける
            score += westernSizeList[i] * westernShapeRatioList[i];

            //MBPSとトイレの距離の結果を乗算
            if (mbpsToiletDistanceList[i] == 1) {
                score *= 1.05f;
            }

            //キッチンと外壁面の関係の結果を乗算
            if (kitchenWallRelationList[i] == 1) {
                score *= 1.05f;
            }
            
            //得点のリストにインデックスとともに追加
            scoreList.Add(i, score);
        }

        //評価指標の辞書を評価指標の降順にソート
        var sortedScoreList = scoreList.OrderByDescending((x) => x.Value);

        //評価指標の辞書のインデックスに合わせて全パターンのリストをソート
        var sortedAllPattern = new List<Dictionary<string, Dictionary<string, Vector3[]>>>();
        foreach (KeyValuePair<int, float> score in sortedScoreList) {
            sortedAllPattern.Add(allPattern[score.Key]);
        }  

        //評価指標のリストをもとにパターンを絞る
        //全パターンのリストのうち，前からselectedNum個を選択
        int selectedNum = 20;
        if (selectedNum > sortedAllPattern.Count) {
            selectedNum = sortedAllPattern.Count;
        }
        //selectedNum = sortedAllPattern.Count; //全パターンを選択
        for (int i = 0; i < selectedNum; i++) {
            result.Add(sortedAllPattern[i]);
        }

        return result;
    }
}
