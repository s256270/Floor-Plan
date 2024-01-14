using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateTwoDwellingMbps : MonoBehaviour
{
    [SerializeField] MakeTwoDwellingsMbpsList mtdml;
    [SerializeField] PlanReader pr;
    [SerializeField] CommonFunctions cf;
    [SerializeField] Parts pa;

    //階段室の座標
    Vector3[] stairsCoordinates;
    //住戸の座標のリスト
    List<Vector3[]> dwellingCoordinates;

    /// <summary>
    /// 2住戸にまたがるMBPSを配置
    /// </summary>
    /// <param name="allPattern">全ての配置結果</param>
    /// <returns>2住戸のMBPSを全て配置した結果</returns>
    public List<Dictionary<string, Dictionary<string, Vector3[]>>> PlaceTwoDwellingsMbps(List<Dictionary<string, Dictionary<string, Vector3[]>>> allPattern) {
        //配置結果のリスト
        var result = new List<Dictionary<string, Dictionary<string, Vector3[]>>>();

        //必要な座標の準備
        //階段室の座標
        stairsCoordinates = pr.getStairsCoordinates();
        //住戸座標のリスト
        dwellingCoordinates = pr.getDwellingCoordinatesList();

        //2住戸にまたがるMBPSの全配置パターンを作成
        List<List<List<int>>> twoDwellingsMbpsAllPattern = mtdml.makeTwoDwellingsMbpsAllPatternList();

        //2住戸にまたがるMBPSの全配置パターンで配置
        for (int i = 0; i < twoDwellingsMbpsAllPattern.Count; i++) {

            //現在の配置結果
            var currentPatternResult = new List<Dictionary<string, Dictionary<string, Vector3[]>>>();
            currentPatternResult.Add(pr.plan);

            //MBPSを配置する2住戸について
            for (int j = 0; j < twoDwellingsMbpsAllPattern[i].Count; j++) {
                //配置結果を空に更新
                result = new List<Dictionary<string, Dictionary<string, Vector3[]>>>();

                //現在の配置結果について
                for (int k = 0; k < currentPatternResult.Count; k++) {
                    //MBPSパターンについて
                    for (int l = 0; l < pa.twoDwellingsMbpsCoordinatesList.Count; l++) {
                        result.Add(PlaceTwoDwellingsMbps(currentPatternResult[k], twoDwellingsMbpsAllPattern[i][j], pa.twoDwellingsMbpsCoordinatesList[l]));
                    }
                }

                //配置結果のリストを更新
                currentPatternResult = cf.DuplicateList(result);
            }
        }

        //TODO: はみ出しているパターンを削除

        return result;
    }

    /// <summary>
    /// 2住戸にまたがるMBPSを配置
    /// </summary>
    /// <param name="currentPlacement">現在の配置結果</param>
    /// <param name="dwellingIndexSet">2住戸にまたがるMBPSを配置する住戸のインデックスの組合わせ</param>
    /// <returns>dwellingIndexで指定された住戸にMBPSを配置した結果</returns>
    public Dictionary<string, Dictionary<string, Vector3[]>> PlaceTwoDwellingsMbps(Dictionary<string, Dictionary<string, Vector3[]>> currentPlacement, List<int> dwellingIndexSet, List<Vector3[]> mbpsCoordinatesSet) {
        //配置結果
        var result = cf.DuplicateDictionary(currentPlacement);

        //MBPSが接する階段室の辺を求める
        int stairsIndex = StairsSideContactMbps(dwellingIndexSet);
        
        //MBPSを配置する2つの住戸のループ
        for (int i = 0; i < dwellingIndexSet.Count; i++) {
            //MBPSを配置する住戸の座標
            Vector3[] currentDwellingCoordinates = dwellingCoordinates[dwellingIndexSet[i]];

            //住戸と階段室の接する辺の座標を求める
            Vector3[] contactStairsCoordinates = cf.LineContactCoordinates(new Vector3[]{stairsCoordinates[stairsIndex], stairsCoordinates[(stairsIndex+1)%stairsCoordinates.Length]}, currentDwellingCoordinates);

            //2つの住戸が接する辺のうち，階段室の辺に触れている辺の座標
            Vector3[] contactDwellingCoordinates = new Vector3[2];

            //MBPSを配置する住戸ともう一方の住戸が接している辺
            List<Vector3[]> contactDwellingCoordinatesList = cf.ContactCoordinates(currentDwellingCoordinates, dwellingCoordinates[dwellingIndexSet[(i+1)%dwellingIndexSet.Count]]);
            
            //住戸同士が接している辺について
            for (int j = 0; j < contactDwellingCoordinatesList.Count; j++) {
                //住戸同士が接している辺が階段室の辺に触れているとき
                if (cf.OnLineSegment(contactStairsCoordinates, contactDwellingCoordinatesList[j][0]) || cf.OnLineSegment(contactStairsCoordinates, contactDwellingCoordinatesList[j][1])) {
                    //欲しい辺の座標を確定
                    contactDwellingCoordinates = contactDwellingCoordinatesList[j];
                }
            }

            //階段室と接する辺のどちら側にMBPSが入るか調べる
            int stairsShiftDirection = cf.ShiftJudge(currentDwellingCoordinates, contactStairsCoordinates);
            //2つの住戸が接する辺のどちら側にMBPSが入るか調べる
            int dwellingShiftDirection = cf.ShiftJudge(currentDwellingCoordinates, contactDwellingCoordinates);

            //配置するMBPSの座標
            Vector3[] mbpsCoordinates = mbpsCoordinatesSet[0];

            //階段室に接する辺がy軸平行のとき
            if (cf.Slope(contactStairsCoordinates) == Mathf.Infinity) {
                //左側の住戸に配置するMBPSについて
                if (stairsShiftDirection * dwellingShiftDirection < 0) {
                    //配置するMBPSの形状を決める
                    mbpsCoordinates = mbpsCoordinatesSet[0];

                    //回転させる角度
                    int rotationAngle = 0;
                    
                    //どれだけ回転させるかを決める
                    if (dwellingShiftDirection > 0) {
                        rotationAngle = 270;
                    }
                    else if (dwellingShiftDirection < 0) {
                        rotationAngle = 90;
                    }

                    //回転させる
                    mbpsCoordinates = cf.Rotation(mbpsCoordinates, rotationAngle);
                }
                //右側の住戸に配置するMBPSについて
                else if (stairsShiftDirection * dwellingShiftDirection > 0) {
                    //配置するMBPSの形状を決める
                    mbpsCoordinates = mbpsCoordinatesSet[1];
                    
                    //回転させる角度
                    int rotationAngle = 0;

                    //どれだけ回転させるかを決める
                    if (dwellingShiftDirection < 0) {
                        rotationAngle = 270;
                    }
                    else if (dwellingShiftDirection > 0) {
                        rotationAngle = 90;
                    }

                    //回転させる
                    mbpsCoordinates = cf.Rotation(mbpsCoordinates, rotationAngle);
                }

                //MBPSの幅・高さを求める
                float width = cf.CalculateRectangleWidth(mbpsCoordinates);
                float height = cf.CalculateRectangleHeight(mbpsCoordinates);

                //MBPSをずらして配置
                //x軸方向にずらす
                if (stairsShiftDirection < 0) {
                    mbpsCoordinates = cf.CorrectCoordinates(mbpsCoordinates, new Vector3(contactStairsCoordinates[0].x - width / 2, 0, 0));
                }
                else if (stairsShiftDirection > 0) {
                    mbpsCoordinates = cf.CorrectCoordinates(mbpsCoordinates, new Vector3(contactStairsCoordinates[0].x + width / 2, 0, 0));
                }

                //y軸方向にずらす
                if (dwellingShiftDirection < 0) {
                    mbpsCoordinates = cf.CorrectCoordinates(mbpsCoordinates, new Vector3(0, contactDwellingCoordinates[0].y - height / 2, 0));
                }
                else if (dwellingShiftDirection > 0) {
                    mbpsCoordinates = cf.CorrectCoordinates(mbpsCoordinates, new Vector3(0, contactDwellingCoordinates[0].y + height / 2, 0));
                }
            }
            //階段室に接する辺がy軸平行でないとき
            else {
                //上側の住戸に配置するMBPSについて
                if (stairsShiftDirection * dwellingShiftDirection > 0) {
                    //配置するMBPSの形状を決める
                    mbpsCoordinates = mbpsCoordinatesSet[0];

                    //回転させる角度
                    int rotationAngle = 0;
                    
                    //どれだけ回転させるかを決める
                    if (dwellingShiftDirection > 0) {
                        rotationAngle = 180;

                        //回転させる
                        mbpsCoordinates = cf.Rotation(mbpsCoordinates, rotationAngle);
                    }
                }
                //下側の住戸に配置するMBPSについて
                else if (stairsShiftDirection * dwellingShiftDirection < 0) {
                    //配置するMBPSの形状を決める
                    mbpsCoordinates = mbpsCoordinatesSet[1];
                    
                    //回転させる角度
                    int rotationAngle = 0;

                    //どれだけ回転させるかを決める
                    if (dwellingShiftDirection < 0) {
                        rotationAngle = 180;

                        //回転させる
                        mbpsCoordinates = cf.Rotation(mbpsCoordinates, rotationAngle);
                    }
                }

                //MBPSの幅・高さを求める
                float width = cf.CalculateRectangleWidth(mbpsCoordinates);
                float height = cf.CalculateRectangleHeight(mbpsCoordinates);

                //MBPSをずらして配置
                //x軸方向にずらす
                if (dwellingShiftDirection < 0) {
                    mbpsCoordinates = cf.CorrectCoordinates(mbpsCoordinates, new Vector3(contactDwellingCoordinates[0].x - width / 2, 0, 0));
                }
                else if (dwellingShiftDirection > 0) {
                    mbpsCoordinates = cf.CorrectCoordinates(mbpsCoordinates, new Vector3(contactDwellingCoordinates[0].x + width / 2, 0, 0));
                }

                //y軸方向にずらす
                if (stairsShiftDirection < 0) {
                    mbpsCoordinates = cf.CorrectCoordinates(mbpsCoordinates, new Vector3(0, contactStairsCoordinates[0].y - height / 2, 0));
                }
                else if (stairsShiftDirection > 0) {
                    mbpsCoordinates = cf.CorrectCoordinates(mbpsCoordinates, new Vector3(0, contactStairsCoordinates[0].y + height / 2, 0));
                }
            }

            //配置結果を格納
            result["Dwelling" + (dwellingIndexSet[i] + 1)].Add("Mbps", mbpsCoordinates);
            //cf.CreateRoom("Mbps", mbpsCoordinates);
        }

        return result;
    }

    /// <summary>
    /// MBPSが接する階段室の辺を求める
    /// </summary> 
    /// <param name="dwellingIndexSet">2住戸にまたがるMBPSを配置する住戸のインデックスの組み合わせ</param>
    /// <returns>MBPSが接する階段室の辺のインデックス</returns>
    public int StairsSideContactMbps(List<int> dwellingIndexSet) {
        int correctStairsIndex = 0;

        //片方の住戸が接する階段室の辺のインデックスの候補
        List<int> stairsIndexCandidates1 = new List<int>();
        //もう片方の住戸が接する階段室の辺のインデックスの候補
        List<int> stairsIndexCandidates2 = new List<int>();

        //それぞれの住戸について
        for (int i = 0; i < dwellingIndexSet.Count; i++) {
            //階段室1辺について
            for (int j = 0; j < stairsCoordinates.Length; j++) {
                //住戸が接しているとき
                if (cf.ContactJudge(new Vector3[]{stairsCoordinates[j], stairsCoordinates[(j+1)%stairsCoordinates.Length]}, dwellingCoordinates[dwellingIndexSet[i]])) {
                    //それぞれの住戸に対応する階段室の辺のインデックスの候補を追加
                    if (i == 0) {
                        stairsIndexCandidates1.Add(j);
                    } else if (i == 1) {
                        stairsIndexCandidates2.Add(j);
                    }
                }
            }
        }

        //2つの住戸が接する階段室の辺のインデックスを求める
        correctStairsIndex = stairsIndexCandidates1.FindAll(n => stairsIndexCandidates2.Contains(n))[0];

        return correctStairsIndex;
    }

    /// <summary>
    /// 多角形と線分の平行な線の組み合わせの切片の差を求める
    /// </summary> 
    /// <param name="polygon">多角形の座標</param>
    /// <param name="line">線分の座標</param>
    /// <returns>多角形と線分の平行な線の組み合わせの切片の差の配列</returns>
    // public float[] ContactGap(Vector3[] polygon, Vector3[] line) {
    //     //返す配列
    //     List<float> result = new List<float>();

    //     for (int i = 0; i < polygon.Length; i++) {
    //         if ((positionalRelation(new Vector3[]{polygon[i], polygon[(i+1)%polygon.Length]}, line)[2] == 1.00f) || (positionalRelation(new Vector3[]{polygon[i], polygon[(i+1)%polygon.Length]}, line)[2] == 2.00f)) {
    //             result.Add(positionalRelation(new Vector3[]{polygon[i], polygon[(i+1)%polygon.Length]}, line)[1] - positionalRelation(new Vector3[]{polygon[i], polygon[(i+1)%polygon.Length]}, line)[0]);
    //         }
    //     }

    //     return result.ToArray();
    // }

    // /// <summary>
    // /// 線分と線分が平行かどうかを確認し，平行な線分のそれぞれの切片を求める
    // /// </summary> 
    // /// <param name="lineA">線分Aの座標</param>
    // /// <param name="lineB">線分Bの座標</param>
    // /// <returns>線分のそれぞれの切片と，平行でないとき0, y軸平行のとき1, それ以外で平行のとき2の配列</returns>
    // public float[] positionalRelation(Vector3[] lineA, Vector3[] lineB) {
    //     float m1, m2, n1, n2;

    //     //y軸平行の時で直線と部屋が平行
    //     if (lineA[0].x == lineA[1].x) {
    //         n1 = lineA[0].x;
    //         if (lineB[0].x == lineB[1].x) {
    //             n2 = lineB[0].x;

    //             return new float[] {n1, n2, 1.00f};
    //         }
    //     }
    //     //それ以外で直線と部屋が平行
    //     else {
    //         m1 = (lineA[1].y - lineA[0].y) / (lineA[1].x - lineA[0].x);
    //         n1 = (lineA[1].x * lineA[0].y - lineA[0].x * lineA[1].y) / (lineA[1].x - lineA[0].x);
    //         if (lineB[0].x != lineB[1].x) {
    //             m2 = (lineB[1].y - lineB[0].y) / (lineB[1].x - lineB[0].x);
    //             n2 = (lineB[1].x * lineB[0].y - lineB[0].x * lineB[1].y) / (lineB[1].x - lineB[0].x);
    //             if (m1 == m2) {

    //                 return new float[] {n1, n2, 2.00f};
    //             }
    //         }     
    //     }

    //     //直線と部屋が平行でない
    //     return new float[] {0.00f, 0.00f, 0.00f};
    // }
}