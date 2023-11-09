using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateEntrance : FloorPlanner
{
    //階段室の座標
    Vector3[] stairsCoordinates;

    /// <summary>
    /// プランを入力すると部屋の配置を行い，間取図を作成する
    /// </summary> 
    /// <param name="plan">プラン図</param>
    /// <returns>間取図（それぞれの部屋名と座標がセットのリスト）</returns>
    public List<Dictionary<string, Dictionary<string, Vector3[]>>> PlaceEntrance(List<Dictionary<string, Dictionary<string, Vector3[]>>> allPattern) {
        //配置結果のリスト
        var result = new List<Dictionary<string, Dictionary<string, Vector3[]>>>();

        stairsCoordinates = allPattern[0]["Stairs"]["Stairs"];

        //現在の全パターンの配置結果について
        for (int i = 0; i < allPattern.Count; i++) {
            //現在のパターンの配置結果
            List<Dictionary<string, Dictionary<string, Vector3[]>>> thisPatternResult = new List<Dictionary<string, Dictionary<string, Vector3[]>>>();
            thisPatternResult.Add(allPattern[i]);

            foreach (KeyValuePair<string, Dictionary<string, Vector3[]>> space in allPattern[i]) {
                //住戸オブジェクトに配置していく
                if (space.Key.Contains("Dwelling")) {
                    thisPatternResult = PlaceEntranceInOneDwelling(space, thisPatternResult);
                }
            }

            result.AddRange(thisPatternResult);
        }

        return result;
    }

    public List<Dictionary<string, Dictionary<string, Vector3[]>>> PlaceEntranceInOneDwelling(KeyValuePair<string, Dictionary<string, Vector3[]>> space, List<Dictionary<string, Dictionary<string, Vector3[]>>> placedPattern) {
        //配置結果のリスト
        var result = new List<Dictionary<string, Dictionary<string, Vector3[]>>>();
        
        //placedPatternの各パターンについて配置を考える
        for (int i = 0; i < placedPattern.Count; i++) {

            /* 玄関を配置する辺を求めるための準備 */
            //住戸の座標を取得
            Vector3[] currentDwellingCoordinates = space.Value["1K"];

            //MBPSの座標を取得
            Vector3[] currentMbpsCoordinates = space.Value["Mbps"];

            //階段室と住戸が接している辺
            List<Vector3[]> contactStairsCoordinates = new List<Vector3[]>();
            for (int j = 0; j < stairsCoordinates.Length; j++) {
                Vector3[] contactCoodinates = contact(new Vector3[]{stairsCoordinates[j], stairsCoordinates[(j+1)%stairsCoordinates.Length]}, currentDwellingCoordinates);
                
                if (!ZeroJudge(contactCoodinates)) {
                    contactStairsCoordinates.Add(contactCoodinates);
                }
            }
            
            //階段室とMBPSが接している辺
            Vector3[] contactMbpsCoordinates = new Vector3[2];
            for (int j = 0; j < stairsCoordinates.Length; j++) {
                Vector3[] contactCoodinates = contact(new Vector3[]{stairsCoordinates[j], stairsCoordinates[(j+1)%stairsCoordinates.Length]}, currentMbpsCoordinates);
                
                if (!ZeroJudge(contactCoodinates)) {
                    contactMbpsCoordinates = contactCoodinates;
                }
            }


            /* 階段室と住戸が接している辺の数だけパターンを作成 */
            for (int j = 0; j < contactStairsCoordinates.Count; j++) {
                //配置する辺
                Vector3[] placementSide = SideSubstraction(contactStairsCoordinates[j], contactMbpsCoordinates)[0];

                /* MBPSの座標を決める */
                float gapX = 0; //x座標のずれ
                float gapY = 0; //y座標のずれ

                //住戸と階段室の接する辺がy軸平行のとき
                if (placementSide[0].x == placementSide[1].x) {

                    //配置する玄関の座標
                    Vector3[] entranceCoordinates = Rotation(pa.entranceCoordinates);

                    //玄関が配置する辺に接するようなx座標を探す
                    for (int k = 0; k < ContactGap(entranceCoordinates, placementSide).Length; k++) {
                        //玄関が住戸の端になるようなy座標を探す
                        for (int l = 0; l < placementSide.Length; l++) {

                            //玄関を配置した結果の座標
                            var placedPatternResult = DuplicateDictionary(placedPattern[i]);

                            //x座標をContactGapの結果に，y座標を端になるようにずらしてみる
                            if (JudgeInside(currentDwellingCoordinates, CorrectCoordinates(entranceCoordinates, new Vector3(ContactGap(entranceCoordinates, placementSide)[k], placementSide[l].y + Mathf.Abs(entranceCoordinates[0].y), 0)))) {
                                //辺上にあるかどうかの判定
                                Vector3[] tempCoordinates = contact(placementSide, CorrectCoordinates(entranceCoordinates, new Vector3(ContactGap(entranceCoordinates, placementSide)[k], placementSide[l].y + Mathf.Abs(entranceCoordinates[0].y), 0)));
                                if (!ZeroJudge(tempCoordinates)) {
                                    gapX = ContactGap(entranceCoordinates, placementSide)[k];
                                    gapY = placementSide[l].y + Mathf.Abs(entranceCoordinates[0].y);

                                    //リストに追加
                                    placedPatternResult[space.Key].Add("Entrance", CorrectCoordinates(entranceCoordinates, new Vector3(gapX, gapY, 0)));

                                    result.Add(placedPatternResult);
                                }
                            }
                            //x座標をContactGapの結果に，y座標を端になるようにずらしてみる
                            else if (JudgeInside(currentDwellingCoordinates, CorrectCoordinates(entranceCoordinates, new Vector3(ContactGap(entranceCoordinates, placementSide)[k], placementSide[l].y - Mathf.Abs(entranceCoordinates[0].y), 0)))) {
                                //辺上にあるかどうかの判定
                                Vector3[] tempCoordinates = contact(placementSide, CorrectCoordinates(entranceCoordinates, new Vector3(ContactGap(entranceCoordinates, placementSide)[k], placementSide[l].y - Mathf.Abs(entranceCoordinates[0].y), 0)));
                                if (!ZeroJudge(tempCoordinates)) {
                                    gapX = ContactGap(entranceCoordinates, placementSide)[k];
                                    gapY = placementSide[l].y - Mathf.Abs(entranceCoordinates[0].y);
                                    
                                    //リストに追加
                                    placedPatternResult[space.Key].Add("Entrance", CorrectCoordinates(entranceCoordinates, new Vector3(gapX, gapY, 0)));

                                    result.Add(placedPatternResult);
                                }
                            }
                        }
                    }
                }
                //住戸と階段室の接する辺がx軸平行のとき
                else if (placementSide[0].y == placementSide[1].y) {
                    //配置する玄関の座標
                    Vector3[] entranceCoordinates = pa.entranceCoordinates;

                    //玄関が配置する辺に接するようなy座標を探す
                    for (int k = 0; k < ContactGap(entranceCoordinates, placementSide).Length; k++) {
                        //MBPSが住戸の端になるようなy座標を探す
                        for (int l = 0; l < placementSide.Length; l++) {

                            //玄関を配置した結果の座標
                            var placedPatternResult = DuplicateDictionary(placedPattern[i]);

                            //y座標をContactGapの結果に，x座標を端になるようにずらしてみる
                            if (JudgeInside(currentDwellingCoordinates, CorrectCoordinates(entranceCoordinates, new Vector3(placementSide[l].x + Mathf.Abs(entranceCoordinates[0].x), ContactGap(entranceCoordinates, placementSide)[k], 0)))) {
                                //辺上にあるかどうかの判定
                                Vector3[] tempCoordinates = contact(placementSide, CorrectCoordinates(entranceCoordinates, new Vector3(placementSide[l].x + Mathf.Abs(entranceCoordinates[0].x), ContactGap(entranceCoordinates, placementSide)[k], 0)));
                                if (!ZeroJudge(tempCoordinates)) {
                                    gapX = placementSide[l].x + Mathf.Abs(entranceCoordinates[0].x);
                                    gapY = ContactGap(entranceCoordinates, placementSide)[k];
                                    
                                    //リストに追加
                                    placedPatternResult[space.Key].Add("Entrance", CorrectCoordinates(entranceCoordinates, new Vector3(gapX, gapY, 0)));

                                    result.Add(placedPatternResult);
                                }
                            }
                            //y座標をContactGapの結果に，x座標を端になるようにずらしてみる
                            else if (JudgeInside(currentDwellingCoordinates, CorrectCoordinates(entranceCoordinates, new Vector3(placementSide[l].x - Mathf.Abs(entranceCoordinates[0].x), ContactGap(entranceCoordinates, placementSide)[k], 0)))) {
                                //辺上にあるかどうかの判定
                                Vector3[] tempCoordinates = contact(placementSide, CorrectCoordinates(entranceCoordinates, new Vector3(placementSide[l].x - Mathf.Abs(entranceCoordinates[0].x), ContactGap(entranceCoordinates, placementSide)[k], 0)));
                                if (!ZeroJudge(tempCoordinates)) {
                                    gapX = placementSide[l].x - Mathf.Abs(entranceCoordinates[0].x);
                                    gapY = ContactGap(entranceCoordinates, placementSide)[k];
                                    
                                    //リストに追加
                                    placedPatternResult[space.Key].Add("Entrance", CorrectCoordinates(entranceCoordinates, new Vector3(gapX, gapY, 0)));

                                    result.Add(placedPatternResult);
                                }
                            }
                        }
                    }
                }
            }
        }

        return result;
    }
}
