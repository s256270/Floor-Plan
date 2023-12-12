using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateTwoDwellingMbps : MonoBehaviour
{
    [SerializeField] MakeTwoDwellingsMbpsList mtdml;
    [SerializeField] PlanReader pr;
    [SerializeField] CreateRoom cr;
    [SerializeField] CommonFunctions cf;
    [SerializeField] Parts pa;

    //階段室の座標
    Vector3[] stairsCoordinates;
    //住戸の座標のリスト
    List<Vector3[]> dwellingCoordinates;

    public List<List<Dictionary<string, Vector3[]>>> placeTwoDwellingsMbps(List<List<Dictionary<string, Vector3[]>>> allPattern) {
        //配置結果のリスト
        var result = new List<List<Dictionary<string, Vector3[]>>>();

        /* 必要な座標の準備 */
        //階段室の座標のみを抜き出し
        stairsCoordinates = pr.stairsCoordinates;
        //住戸座標のリストを作成
        dwellingCoordinates = pr.dwellingCoordinates;

        //プラン図の状態を保持
        //var plan = DuplicateDictionary(allPattern[0]);

        //2住戸にまたがるMBPSの全配置パターンを作成
        List<List<List<int>>> twoDwellingsMbpsAllPattern = mtdml.makeTwoDwellingsMbpsAllPatternList();
        //パターンの確認
        // for (int i = 0; i < twoDwellingsMbpsAllPattern.Count; i++) {
        //     Debug.Log(i + "パターン目");
        //     for (int j = 0; j < twoDwellingsMbpsAllPattern[i].Count; j++) {
        //         Debug.Log("(" + twoDwellingsMbpsAllPattern[i][j][0] + ", " + twoDwellingsMbpsAllPattern[i][j][1] + ")");
        //     }
        // }

        //2住戸にまたがるMBPSの全配置パターンで配置
        for (int i = 0; i < twoDwellingsMbpsAllPattern.Count; i++) {

            //i番目の配置パターンの結果
            var thisPatternResult = new List<Dictionary<string, Vector3[]>>();

            for (int j = 0; j < twoDwellingsMbpsAllPattern[i].Count; j++) {
                thisPatternResult = placeTwoDwellingsMbps(thisPatternResult, twoDwellingsMbpsAllPattern[i][j]);
            }

            //配置結果をリストに追加
            result.Add(thisPatternResult);
        }

        return result;
    }

    /// <summary>
    /// 2住戸にまたがるMBPSを配置
    /// </summary>
    /// <param name="currentPlacement">現在の配置結果</param>
    /// <param name="dwellingIndexSet">2住戸にまたがるMBPSを配置する住戸のインデックスの組合わせ</param>
    /// <returns>dwellingIndexで指定された住戸にMBPSを配置した結果</returns>
    public List<Dictionary<string, Vector3[]>> placeTwoDwellingsMbps(List<Dictionary<string, Vector3[]>> currentPlacement, List<int> dwellingIndexSet) {
        //MBPSが接する階段室の辺を求める
        int stairsIndex = StairsSideContactMbps(placePattern);
        
        /* 2つの住戸にMBPSを配置 */
        for (int i = 0; i < placePattern.Count; i++) {
            //MBPSを配置する住戸の座標
            Vector3[] currentDwellingCoordinates = dwellingCoordinates[placePattern[i]];

            /* 住戸と階段室の接する辺の座標を求める */
            Vector3[] contactStairsCoodinates = new Vector3[2];
            Vector3[] contactCoodinates = cr.contact(new Vector3[]{stairsCoordinates[stairsIndex], stairsCoordinates[(stairsIndex+1)%stairsCoordinates.Length]}, currentDwellingCoordinates);
            
            if (!cr.ZeroJudge(contactCoodinates)) {
                contactStairsCoodinates = contactCoodinates;
            }
            
            /* MBPSの座標を決める */
            float gapX = 0; //x座標のずれ
            float gapY = 0; //y座標のずれ

            //住戸と階段室の接する辺がy軸平行のとき
            if (contactStairsCoodinates[0].x == contactStairsCoodinates[1].x) {
                //配置するMBPSの座標
                Vector3[] mbpsCoordinates = Rotation(pa.twoDwellingMbpsCoordinates);

                //MBPSが住戸と階段室の接する辺に接するようなx座標を探す
                for (int j = 0; j < ContactGap(mbpsCoordinates, contactStairsCoodinates).Length; j++) {
                    //MBPSが2つの住戸が接する辺に接するようなy座標を探す
                    for (int k = 0; k < currentDwellingCoordinates.Length; k++) {
                        Vector3[] contactDwellingCoodinates = cr.contact(new Vector3[]{currentDwellingCoordinates[k], currentDwellingCoordinates[(k+1)%currentDwellingCoordinates.Length]}, dwellingCoordinates[placePattern[(i+1)%placePattern.Count]]);
                        
                        //2つの住戸が接するとき
                        if (!cr.ZeroJudge(contactDwellingCoodinates)) {
                            //2つの住戸が接する辺が階段室の辺に触れているとき
                            if (OnLineSegment(contactStairsCoodinates, contactDwellingCoodinates[0]) || OnLineSegment(contactStairsCoodinates, contactDwellingCoodinates[1])) {
                                //x座標をContactGapの結果に，y座標を2つの住戸が接する辺の上に接するようにずらしてみる
                                if (JudgeInside(currentDwellingCoordinates, CorrectCoordinates(mbpsCoordinates, new Vector3(ContactGap(mbpsCoordinates, contactStairsCoodinates)[j], contactDwellingCoodinates[0].y + Mathf.Abs(mbpsCoordinates[0].y), 0)))) {
                                    gapX = ContactGap(mbpsCoordinates, contactStairsCoodinates)[j];
                                    gapY = contactDwellingCoodinates[0].y + Mathf.Abs(mbpsCoordinates[0].y);

                                    //planPattern["Dwelling" + (placePattern[i] + 1)].Add("Mbps", CorrectCoordinates(mbpsCoordinates, new Vector3(gapX, gapY, 0)));

                                    break;
                                }
                                //x座標をContactGapの結果に，y座標を2つの住戸が接する辺の下に接するようにずらしてみる
                                else if (JudgeInside(currentDwellingCoordinates, CorrectCoordinates(mbpsCoordinates, new Vector3(ContactGap(mbpsCoordinates, contactStairsCoodinates)[j], contactDwellingCoodinates[0].y - Mathf.Abs(mbpsCoordinates[0].y), 0)))) {
                                    gapX = ContactGap(mbpsCoordinates, contactStairsCoodinates)[j];
                                    gapY = contactDwellingCoodinates[0].y - Mathf.Abs(mbpsCoordinates[0].y);

                                    //planPattern["Dwelling" + (placePattern[i] + 1)].Add("Mbps", CorrectCoordinates(mbpsCoordinates, new Vector3(gapX, gapY, 0)));

                                    break;
                                }
                            }
                        }
                    }
                }
            }
            //住戸と階段室の接する辺がx軸平行のとき
            else if (contactStairsCoodinates[0].y == contactStairsCoodinates[1].y) {
                //配置するMBPSの座標
                Vector3[] mbpsCoordinates = pa.twoDwellingMbpsCoordinates;

                //MBPSが住戸と階段室の接する辺に接するようなy座標を探す
                for (int j = 0; j < ContactGap(mbpsCoordinates, contactStairsCoodinates).Length; j++) {
                    //MBPSが2つの住戸が接する辺に接するようなx座標を探す
                    for (int k = 0; k < currentDwellingCoordinates.Length; k++) {
                        Vector3[] contactDwellingCoodinates = cr.contact(new Vector3[]{currentDwellingCoordinates[k], currentDwellingCoordinates[(k+1)%currentDwellingCoordinates.Length]}, dwellingCoordinates[placePattern[(i+1)%placePattern.Count]]);
                        
                        //2つの住戸が接するとき
                        if (!cr.ZeroJudge(contactDwellingCoodinates)) {
                            //2つの住戸が接する辺が階段室の辺に触れているとき
                            if (OnLineSegment(contactStairsCoodinates, contactDwellingCoodinates[0]) || OnLineSegment(contactStairsCoodinates, contactDwellingCoodinates[1])) {
                                //y座標をContactGapの結果に，x座標を2つの住戸が接する辺の上に接するようにずらしてみる
                                if (JudgeInside(currentDwellingCoordinates, CorrectCoordinates(mbpsCoordinates, new Vector3(contactDwellingCoodinates[0].x + Mathf.Abs(mbpsCoordinates[0].x), ContactGap(mbpsCoordinates, contactStairsCoodinates)[j], 0)))) {
                                    gapX = contactDwellingCoodinates[0].x + Mathf.Abs(mbpsCoordinates[0].x);
                                    gapY = ContactGap(mbpsCoordinates, contactStairsCoodinates)[j];

                                    //planPattern["Dwelling" + (placePattern[i] + 1)].Add("Mbps", CorrectCoordinates(mbpsCoordinates, new Vector3(gapX, gapY, 0)));

                                    break;
                                }
                                //y座標をContactGapの結果に，x座標を2つの住戸が接する辺の下に接するようにずらしてみる
                                else if (JudgeInside(currentDwellingCoordinates, CorrectCoordinates(mbpsCoordinates, new Vector3(contactDwellingCoodinates[0].x - Mathf.Abs(mbpsCoordinates[0].x), ContactGap(mbpsCoordinates, contactStairsCoodinates)[j], 0)))) {
                                    gapX = contactDwellingCoodinates[0].x - Mathf.Abs(mbpsCoordinates[0].x);
                                    gapY = ContactGap(mbpsCoordinates, contactStairsCoodinates)[j];

                                    //planPattern["Dwelling" + (placePattern[i] + 1)].Add("Mbps", CorrectCoordinates(mbpsCoordinates, new Vector3(gapX, gapY, 0)));

                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        return planPattern;
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
                if (cf.ContactJudge(new Vector3[]{stairsCoordinates[j], stairsCoordinates[(j+1)%stairsCoordinates.Length]}, dwellingCoordinates[dwellingIndexSet[i]][k])) {
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
    public float[] ContactGap(Vector3[] polygon, Vector3[] line) {
        //返す配列
        List<float> result = new List<float>();

        for (int i = 0; i < polygon.Length; i++) {
            if ((positionalRelation(new Vector3[]{polygon[i], polygon[(i+1)%polygon.Length]}, line)[2] == 1.00f) || (positionalRelation(new Vector3[]{polygon[i], polygon[(i+1)%polygon.Length]}, line)[2] == 2.00f)) {
                result.Add(positionalRelation(new Vector3[]{polygon[i], polygon[(i+1)%polygon.Length]}, line)[1] - positionalRelation(new Vector3[]{polygon[i], polygon[(i+1)%polygon.Length]}, line)[0]);
            }
        }

        return result.ToArray();
    }

    /// <summary>
    /// 線分と線分が平行かどうかを確認し，平行な線分のそれぞれの切片を求める
    /// </summary> 
    /// <param name="lineA">線分Aの座標</param>
    /// <param name="lineB">線分Bの座標</param>
    /// <returns>線分のそれぞれの切片と，平行でないとき0, y軸平行のとき1, それ以外で平行のとき2の配列</returns>
    public float[] positionalRelation(Vector3[] lineA, Vector3[] lineB) {
        float m1, m2, n1, n2;

        //y軸平行の時で直線と部屋が平行
        if (lineA[0].x == lineA[1].x) {
            n1 = lineA[0].x;
            if (lineB[0].x == lineB[1].x) {
                n2 = lineB[0].x;

                return new float[] {n1, n2, 1.00f};
            }
        }
        //それ以外で直線と部屋が平行
        else {
            m1 = (lineA[1].y - lineA[0].y) / (lineA[1].x - lineA[0].x);
            n1 = (lineA[1].x * lineA[0].y - lineA[0].x * lineA[1].y) / (lineA[1].x - lineA[0].x);
            if (lineB[0].x != lineB[1].x) {
                m2 = (lineB[1].y - lineB[0].y) / (lineB[1].x - lineB[0].x);
                n2 = (lineB[1].x * lineB[0].y - lineB[0].x * lineB[1].y) / (lineB[1].x - lineB[0].x);
                if (m1 == m2) {

                    return new float[] {n1, n2, 2.00f};
                }
            }     
        }

        //直線と部屋が平行でない
        return new float[] {0.00f, 0.00f, 0.00f};
    }

    /// <summary>
    /// 全ての座標を原点周りに90度回転させる
    /// </summary> 
    /// <param name="shapes">回転させたい座標配列</param>
    /// <returns>回転させた後の座標配列</returns>
    public Vector3[] Rotation(Vector3[] shapes) {
        //回転させた後の座標を格納する配列
        Vector3[] rotatedCoordinates = new Vector3[shapes.Length];

        //回転させる
        for (int i = 0; i < shapes.Length; i++) {
            rotatedCoordinates[i].x = - shapes[(i+1)%shapes.Length].y;
            rotatedCoordinates[i].y = shapes[(i+1)%shapes.Length].x;
        }

        return rotatedCoordinates;
    }

    /// <summary>
    /// 全ての座標を同じだけ移動させる
    /// </summary> 
    /// <param name="shapes">動かしたい座標配列</param>
    /// <param name="correctValue">移動させる距離</param>
    /// <returns>移動させた後の座標配列</returns>
    public Vector3[] CorrectCoordinates(Vector3[] shapes, Vector3 correctValue) {
        Vector3[] correctedCoordinates = new Vector3[shapes.Length];

        for (int i = 0; i < shapes.Length; i++) {
            correctedCoordinates[i] = new Vector3(shapes[i].x + correctValue.x, shapes[i].y + correctValue.y, 0);
        }

        return correctedCoordinates;
    }

    /// <summary>
    /// 多角形が別の多角形の内部（辺上も可）にあるかどうかの判定
    /// </summary> 
    /// <param name="outer">外側にあってほしい多角形の座標配列</param>
    /// <param name="inner">内側にあってほしい多角形の座標配列</param>
    /// <returns>多角形が別の多角形の内部にある場合True，ない場合Flase</returns>
    public bool JudgeInside(Vector3[] outer, Vector3[] inner) {
        bool flag = false;
        //点がいくつ内側にあるかを数える
        int insideCounter = 0;

        //内側にあってほしい多角形の頂点を全て調べる
        for (int i = 0; i < inner.Length; i++) {
            //まず辺上にあるかどうかを調べる
            bool onLineFlag = false;
            for (int j = 0; j < outer.Length; j++) {
                if (OnLineSegment(new Vector3[]{outer[j], outer[(j+1)%outer.Length]}, new Vector3(inner[i].x, inner[i].y, 0))) {
                    insideCounter++;
                    onLineFlag = true;
                    break;
                }
            }
            
            //辺上にある場合は次の頂点へ
            if (onLineFlag) {
                continue;
            }

            //次に，内部にあるかどうかを調べる
            if (CheckPoint(outer, new Vector3(inner[i].x, inner[i].y, 0))) {
                insideCounter++;
            }
        }

        //内側にある点の数が内側の多角形の頂点の数と同じ場合，内側にあると判定
        if (insideCounter == inner.Length) {
            flag = true;
        }

        return flag;
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

    /// <summary>
    /// 図形に対する点の内外判定
    /// </summary> 
    /// <param name="points">図形の座標配列</param>
    /// <param name="target">判定する点の座標</param>
    /// <returns>内分の場合True，外分の場合Flase</returns>
    public bool CheckPoint(Vector3[] points, Vector3 target) {
        Vector3 normal = new Vector3(1f, 0f, 0f);
        //Vector3 normal = Vector3.up;//(0, 1, 0)
        // XY平面上に写像した状態で計算を行う
        Quaternion rot = Quaternion.FromToRotation(normal, -Vector3.forward);

        Vector3[] rotPoints = new Vector3[points.Length];

        for (int i = 0; i < rotPoints.Length; i++) {
            rotPoints[i] = rot * points[i];
        }

        target = rot * target;

        int wn = 0;
        float vt = 0;

        for (int i = 0; i < rotPoints.Length; i++) {
            // 上向きの辺、下向きの辺によって処理を分ける

            int cur = i;
            int next = (i + 1) % rotPoints.Length;

            // 上向きの辺。点PがY軸方向について、始点と終点の間にある。（ただし、終点は含まない）
            if ((rotPoints[cur].y <= target.y) && (rotPoints[next].y > target.y)) {
                // 辺は点Pよりも右側にある。ただし重ならない
                // 辺が点Pと同じ高さになる位置を特定し、その時のXの値と点PのXの値を比較する
                vt = (target.y - rotPoints[cur].y) / (rotPoints[next].y - rotPoints[cur].y);

                if (target.x < (rotPoints[cur].x + (vt * (rotPoints[next].x - rotPoints[cur].x)))) {
                    // 上向きの辺と交差した場合は+1
                    wn++;
                }
            }
            else if ((rotPoints[cur].y > target.y) && (rotPoints[next].y <= target.y)) {
                // 辺は点Pよりも右側にある。ただし重ならない
                // 辺が点Pと同じ高さになる位置を特定し、その時のXの値と点PのXの値を比較する
                vt = (target.y - rotPoints[cur].y) / (rotPoints[next].y - rotPoints[cur].y);

                if (target.x < (rotPoints[cur].x + (vt * (rotPoints[next].x - rotPoints[cur].x)))) {
                    // 下向きの辺と交差した場合は-1
                    wn--;
                }
            }
        }

        return wn != 0;
    }
}