using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMbps : FloorPlanner
{
    //階段室の座標
    Vector3[] stairsCoordinates;
    //住戸の座標のリスト
    List<Vector3[]> dwellingCoordinates;

    //階段室のある1辺に接している住戸（dwellingCoordinatesのインデックスで管理）の組のリスト
    List<List<int>> contactStairs = new List<List<int>>();

    //2住戸にまたがるMBPSの全配置パターン
    List<List<List<int>>> twoDwellingsMbpsAllPattern = new List<List<List<int>>>();

    /// <summary>
    /// プランを入力すると部屋の配置を行い，間取図を作成する
    /// </summary> 
    /// <param name="plan">プラン図</param>
    /// <returns>間取図（それぞれの部屋名と座標がセットのリスト）</returns>
    public List<Dictionary<string, Dictionary<string, Vector3[]>>> PlaceMbps(List<Dictionary<string, Dictionary<string, Vector3[]>>> allPattern) {
        //配置結果のリスト
        var result = new List<Dictionary<string, Dictionary<string, Vector3[]>>>();

        /* 必要な座標の準備 */
        //階段室の座標のみを抜き出し
        stairsCoordinates = allPattern[0]["Stairs"]["Stairs"];

        //住戸座標のリストを作成
        dwellingCoordinates = makeDwellingCoordinatesList(allPattern[0]);

        /* 2部屋にまたがるMBPSの配置開始 */
        result = placeTwoDwellingsMbps(allPattern);

        /* 1部屋のみのMBPSの配置開始 */
        result = placeOneDwellingMbps(result);

        return result;
    }

    public List<Dictionary<string, Dictionary<string, Vector3[]>>> placeTwoDwellingsMbps(List<Dictionary<string, Dictionary<string, Vector3[]>>> allPattern) {
        //配置結果のリスト
        var result = new List<Dictionary<string, Dictionary<string, Vector3[]>>>();

        //プラン図の状態を保持
        var plan = DuplicateDictionary(allPattern[0]);

        //2住戸にまたがるMBPSの全配置パターンを作成
        maketwoDwellingsMbpsAllPatternList();

        //2住戸にまたがるMBPSの全配置パターンで配置
        for (int i = 0; i < twoDwellingsMbpsAllPattern.Count; i++) {

            //i番目の配置パターンの結果
            var thisPatternResult = new Dictionary<string, Dictionary<string, Vector3[]>>(plan);

            for (int j = 0; j < twoDwellingsMbpsAllPattern[i].Count; j++) {
                thisPatternResult = placeTwoDwellingsMbps(thisPatternResult, twoDwellingsMbpsAllPattern[i][j]);
            }

            //配置結果をリストに追加
            result.Add(thisPatternResult);
        }

        return result;
    }

    public Dictionary<string, Dictionary<string, Vector3[]>> placeTwoDwellingsMbps(Dictionary<string, Dictionary<string, Vector3[]>> planPattern, List<int> placePattern) {
        /* MBPSが接する階段室の辺を求める */
        int stairsIndex = 0;

        List<int> stairsIndexCandidates1 = new List<int>();
        List<int> stairsIndexCandidates2 = new List<int>();

        //階段室のある1辺について
        for (int i = 0; i < stairsCoordinates.Length; i++) {
            for (int j = 0; j < dwellingCoordinates[placePattern[0]].Length; j++) {
                Vector3[] contactCoodinates = contact(new Vector3[]{stairsCoordinates[i], stairsCoordinates[(i+1)%stairsCoordinates.Length]}, dwellingCoordinates[placePattern[0]]);

                //階段室と住戸が接しているとき
                if (!ZeroJudge(contactCoodinates)) {
                    stairsIndexCandidates1.Add(i);
                }
            }

            for (int j = 0; j < dwellingCoordinates[placePattern[1]].Length; j++) {
                Vector3[] contactCoodinates = contact(new Vector3[]{stairsCoordinates[i], stairsCoordinates[(i+1)%stairsCoordinates.Length]}, dwellingCoordinates[placePattern[1]]);

                //階段室と住戸が接しているとき
                if (!ZeroJudge(contactCoodinates)) {
                    stairsIndexCandidates2.Add(i);
                }
            }
            
        }

        stairsIndex = stairsIndexCandidates1.FindAll(n => stairsIndexCandidates2.Contains(n))[0];
        
        /* 2つの住戸にMBPSを配置 */
        for (int i = 0; i < placePattern.Count; i++) {
            //MBPSを配置する住戸の座標
            Vector3[] currentDwellingCoordinates = dwellingCoordinates[placePattern[i]];

            /* 住戸と階段室の接する辺の座標を求める */
            Vector3[] contactStairsCoodinates = new Vector3[2];
            Vector3[] contactCoodinates = contact(new Vector3[]{stairsCoordinates[stairsIndex], stairsCoordinates[(stairsIndex+1)%stairsCoordinates.Length]}, currentDwellingCoordinates);
            
            if (!ZeroJudge(contactCoodinates)) {
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
                        Vector3[] contactDwellingCoodinates = contact(new Vector3[]{currentDwellingCoordinates[k], currentDwellingCoordinates[(k+1)%currentDwellingCoordinates.Length]}, dwellingCoordinates[placePattern[(i+1)%placePattern.Count]]);
                        
                        //2つの住戸が接するとき
                        if (!ZeroJudge(contactDwellingCoodinates)) {
                            //2つの住戸が接する辺が階段室の辺に触れているとき
                            if (OnLineSegment(contactStairsCoodinates, contactDwellingCoodinates[0]) || OnLineSegment(contactStairsCoodinates, contactDwellingCoodinates[1])) {
                                //x座標をContactGapの結果に，y座標を2つの住戸が接する辺の上に接するようにずらしてみる
                                if (JudgeInside(currentDwellingCoordinates, CorrectCoordinates(mbpsCoordinates, new Vector3(ContactGap(mbpsCoordinates, contactStairsCoodinates)[j], contactDwellingCoodinates[0].y + Mathf.Abs(mbpsCoordinates[0].y), 0)))) {
                                    gapX = ContactGap(mbpsCoordinates, contactStairsCoodinates)[j];
                                    gapY = contactDwellingCoodinates[0].y + Mathf.Abs(mbpsCoordinates[0].y);

                                    planPattern["Dwelling" + (placePattern[i] + 1)].Add("Mbps", CorrectCoordinates(mbpsCoordinates, new Vector3(gapX, gapY, 0)));

                                    break;
                                }
                                //x座標をContactGapの結果に，y座標を2つの住戸が接する辺の下に接するようにずらしてみる
                                else if (JudgeInside(currentDwellingCoordinates, CorrectCoordinates(mbpsCoordinates, new Vector3(ContactGap(mbpsCoordinates, contactStairsCoodinates)[j], contactDwellingCoodinates[0].y - Mathf.Abs(mbpsCoordinates[0].y), 0)))) {
                                    gapX = ContactGap(mbpsCoordinates, contactStairsCoodinates)[j];
                                    gapY = contactDwellingCoodinates[0].y - Mathf.Abs(mbpsCoordinates[0].y);

                                    planPattern["Dwelling" + (placePattern[i] + 1)].Add("Mbps", CorrectCoordinates(mbpsCoordinates, new Vector3(gapX, gapY, 0)));

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
                        Vector3[] contactDwellingCoodinates = contact(new Vector3[]{currentDwellingCoordinates[k], currentDwellingCoordinates[(k+1)%currentDwellingCoordinates.Length]}, dwellingCoordinates[placePattern[(i+1)%placePattern.Count]]);
                        
                        //2つの住戸が接するとき
                        if (!ZeroJudge(contactDwellingCoodinates)) {
                            //2つの住戸が接する辺が階段室の辺に触れているとき
                            if (OnLineSegment(contactStairsCoodinates, contactDwellingCoodinates[0]) || OnLineSegment(contactStairsCoodinates, contactDwellingCoodinates[1])) {
                                //y座標をContactGapの結果に，x座標を2つの住戸が接する辺の上に接するようにずらしてみる
                                if (JudgeInside(currentDwellingCoordinates, CorrectCoordinates(mbpsCoordinates, new Vector3(contactDwellingCoodinates[0].x + Mathf.Abs(mbpsCoordinates[0].x), ContactGap(mbpsCoordinates, contactStairsCoodinates)[j], 0)))) {
                                    gapX = contactDwellingCoodinates[0].x + Mathf.Abs(mbpsCoordinates[0].x);
                                    gapY = ContactGap(mbpsCoordinates, contactStairsCoodinates)[j];

                                    planPattern["Dwelling" + (placePattern[i] + 1)].Add("Mbps", CorrectCoordinates(mbpsCoordinates, new Vector3(gapX, gapY, 0)));

                                    break;
                                }
                                //y座標をContactGapの結果に，x座標を2つの住戸が接する辺の下に接するようにずらしてみる
                                else if (JudgeInside(currentDwellingCoordinates, CorrectCoordinates(mbpsCoordinates, new Vector3(contactDwellingCoodinates[0].x - Mathf.Abs(mbpsCoordinates[0].x), ContactGap(mbpsCoordinates, contactStairsCoodinates)[j], 0)))) {
                                    gapX = contactDwellingCoodinates[0].x - Mathf.Abs(mbpsCoordinates[0].x);
                                    gapY = ContactGap(mbpsCoordinates, contactStairsCoodinates)[j];

                                    planPattern["Dwelling" + (placePattern[i] + 1)].Add("Mbps", CorrectCoordinates(mbpsCoordinates, new Vector3(gapX, gapY, 0)));

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

    public List<Dictionary<string, Dictionary<string, Vector3[]>>> placeOneDwellingMbps(List<Dictionary<string, Dictionary<string, Vector3[]>>> allPattern) {
        //配置結果のリスト
        var result = new List<Dictionary<string, Dictionary<string, Vector3[]>>>();
        
        //全パターンに対して配置
        for (int i = 0; i < allPattern.Count; i++) {
            
            //1つのパターンについて配置した結果のリスト
            var currentPatternResult = new List<Dictionary<string, Dictionary<string, Vector3[]>>>();
            currentPatternResult.Add(allPattern[i]);
            
            foreach (KeyValuePair<string, Dictionary<string, Vector3[]>> space in allPattern[i]) {
                if (space.Key.Contains("Dwelling")) {

                    //2住戸にまたがるMBPSが配置されているとき
                    if (space.Value.ContainsKey("Mbps")) {
                        //スキップ
                        continue;
                    }
                    
                    currentPatternResult = placeOneDwellingMbpsInOneDwellig(space, currentPatternResult);
                }
            }

            result.AddRange(currentPatternResult);
        }

        return result;
    }

    public List<Dictionary<string, Dictionary<string, Vector3[]>>> placeOneDwellingMbpsInOneDwellig(KeyValuePair<string, Dictionary<string, Vector3[]>> space, List<Dictionary<string, Dictionary<string, Vector3[]>>> placedPattern) {
        //配置結果のリスト
        var result = new List<Dictionary<string, Dictionary<string, Vector3[]>>>();
        
        //placedPatternの各パターンについて配置を考える
        for (int i = 0; i < placedPattern.Count; i++) {
        
            //配置する住戸の座標
            Vector3[] currentDwellingCoordinates = space.Value["1K"];

            /* MBPSをくっつける階段室の辺を求める */
            List<Vector3[]> contactStairsCoodinates = new List<Vector3[]>();

            for (int j = 0; j < stairsCoordinates.Length; j++) {
                Vector3[] contactCoodinates = contact(new Vector3[]{stairsCoordinates[j], stairsCoordinates[(j+1)%stairsCoordinates.Length]}, currentDwellingCoordinates);
                
                if (!ZeroJudge(contactCoodinates)) {
                    contactStairsCoodinates.Add(contactCoodinates);
                }
            }

            for (int j = 0; j < contactStairsCoodinates.Count; j++) {
                //配置する辺
                Vector3[] placementSide = contactStairsCoodinates[j];

                /* MBPSの座標を決める */
                float gapX = 0; //x座標のずれ
                float gapY = 0; //y座標のずれ

                //住戸と階段室の接する辺がy軸平行のとき
                if (placementSide[0].x == placementSide[1].x) {
                    //配置するMBPSの座標
                    Vector3[] mbpsCoordinates = pa.oneDwellingMbpsCoordinates;

                    //MBPSが住戸と階段室の接する辺に接するようなx座標を探す
                    for (int k = 0; k < ContactGap(mbpsCoordinates, placementSide).Length; k++) {

                        //MBPSが住戸の端になるようなy座標を探す
                        for (int l = 0; l < placementSide.Length; l++) {

                            //MBPSを配置した結果の座標
                            var placedPatternResult = DuplicateDictionary(placedPattern[i]);

                            //x座標をContactGapの結果に，y座標を住戸の端になるようにずらしてみる
                            if (JudgeInside(currentDwellingCoordinates, CorrectCoordinates(mbpsCoordinates, new Vector3(ContactGap(mbpsCoordinates, placementSide)[k], placementSide[l].y + Mathf.Abs(mbpsCoordinates[0].y), 0)))) {
                                //辺上にあるかどうかの判定
                                Vector3[] tempCoordinates = contact(placementSide, CorrectCoordinates(mbpsCoordinates, new Vector3(ContactGap(mbpsCoordinates, placementSide)[k], placementSide[l].y + Mathf.Abs(mbpsCoordinates[0].y), 0)));
                                if (!ZeroJudge(tempCoordinates)) {
                                    gapX = ContactGap(mbpsCoordinates, placementSide)[k];
                                    gapY = placementSide[l].y + Mathf.Abs(mbpsCoordinates[0].y);

                                    placedPatternResult[space.Key].Add("Mbps", CorrectCoordinates(mbpsCoordinates, new Vector3(gapX, gapY, 0)));
                                    result.Add(placedPatternResult);
                                }
                            }
                            //x座標をContactGapの結果に，y座標を住戸の端になるようにずらしてみる
                            else if (JudgeInside(currentDwellingCoordinates, CorrectCoordinates(mbpsCoordinates, new Vector3(ContactGap(mbpsCoordinates, placementSide)[k], placementSide[l].y - Mathf.Abs(mbpsCoordinates[0].y), 0)))) {
                                //辺上にあるかどうかの判定
                                Vector3[] tempCoordinates = contact(placementSide, CorrectCoordinates(mbpsCoordinates, new Vector3(ContactGap(mbpsCoordinates, placementSide)[k], placementSide[l].y - Mathf.Abs(mbpsCoordinates[0].y), 0)));
                                if (!ZeroJudge(tempCoordinates)) {
                                    gapX = ContactGap(mbpsCoordinates, placementSide)[k];
                                    gapY = placementSide[l].y - Mathf.Abs(mbpsCoordinates[0].y);

                                    placedPatternResult[space.Key].Add("Mbps", CorrectCoordinates(mbpsCoordinates, new Vector3(gapX, gapY, 0)));
                                    result.Add(placedPatternResult);
                                }
                            }
                        }
                    }
                }
                //住戸と階段室の接する辺がx軸平行のとき
                else if (placementSide[0].y == placementSide[1].y) {
                    //配置するMBPSの座標
                    Vector3[] mbpsCoordinates = pa.oneDwellingMbpsCoordinates;

                    //MBPSが住戸と階段室の接する辺に接するようなy座標を探す
                    for (int k = 0; k < ContactGap(mbpsCoordinates, placementSide).Length; k++) {
                        //MBPSが住戸の端になるようなx座標を探す
                        for (int l = 0; l < placementSide.Length; l++) {

                            //MBPSを配置した結果の座標
                            var placedPatternResult = DuplicateDictionary(placedPattern[i]);

                            //y座標をContactGapの結果に，x座標を住戸の端になるようにずらしてみる
                            if (JudgeInside(currentDwellingCoordinates, CorrectCoordinates(mbpsCoordinates, new Vector3(placementSide[l].x + Mathf.Abs(mbpsCoordinates[0].x), ContactGap(mbpsCoordinates, placementSide)[k], 0)))) {
                                //辺上にあるかどうかの判定
                                Vector3[] tempCoordinates = contact(placementSide, CorrectCoordinates(mbpsCoordinates, new Vector3(placementSide[l].x + Mathf.Abs(mbpsCoordinates[0].x), ContactGap(mbpsCoordinates, placementSide)[k], 0)));
                                if (!ZeroJudge(tempCoordinates)) {
                                    gapX = placementSide[l].x + Mathf.Abs(mbpsCoordinates[0].x);
                                    gapY = ContactGap(mbpsCoordinates, placementSide)[k];

                                    placedPatternResult[space.Key].Add("Mbps", CorrectCoordinates(mbpsCoordinates, new Vector3(gapX, gapY, 0)));
                                    result.Add(placedPatternResult);
                                }
                            }
                            //y座標をContactGapの結果に，x座標を住戸の端になるようにずらしてみる
                            else if (JudgeInside(currentDwellingCoordinates, CorrectCoordinates(mbpsCoordinates, new Vector3(placementSide[l].x - Mathf.Abs(mbpsCoordinates[0].x), ContactGap(mbpsCoordinates, placementSide)[k], 0)))) {
                                //辺上にあるかどうかの判定
                                Vector3[] tempCoordinates = contact(placementSide, CorrectCoordinates(mbpsCoordinates, new Vector3(placementSide[l].x - Mathf.Abs(mbpsCoordinates[0].x), ContactGap(mbpsCoordinates, placementSide)[k], 0)));
                                if (!ZeroJudge(tempCoordinates)) {
                                    gapX = placementSide[l].x - Mathf.Abs(mbpsCoordinates[0].x);
                                    gapY = ContactGap(mbpsCoordinates, placementSide)[k];

                                    placedPatternResult[space.Key].Add("Mbps", CorrectCoordinates(mbpsCoordinates, new Vector3(gapX, gapY, 0)));
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

    /// <summary>
    /// 2住戸にまたがるMBPSの全配置パターンを作成する
    /// </summary> 
    public void maketwoDwellingsMbpsAllPatternList() {
        //階段室のある1辺に接している住戸（dwellingCoordinatesのインデックスで管理）の組のリスト
        //List<List<int>> contactStairs = new List<List<int>>();

        /* リストを作成 */
        //階段室の1辺の座標
        for (int i = 0; i < stairsCoordinates.Length; i++) {  
            List<int> contactDwellingsIndex = new List<int>();         
            for (int j = 0; j < dwellingCoordinates.Count; j++) {
                Vector3[] contactCoodinates = contact(new Vector3[]{stairsCoordinates[i], stairsCoordinates[(i+1)%stairsCoordinates.Length]}, dwellingCoordinates[j]);
                //階段室と住戸が接しているとき
                if (!ZeroJudge(contactCoodinates)) {
                    //住戸のリスト番号を記録
                    contactDwellingsIndex.Add(j);
                }
            }

            //階段室の1辺に2つ以上の住戸が接しているとき
            if (contactDwellingsIndex.Count >= 2) {
                //リストに追加
                contactStairs.Add(contactDwellingsIndex);
            }
        }

        /* contactStairsに含まれる住戸の種類数 */
        //カウントしたかどうかを判別するためだけのリスト
        List<int> countList = new List<int>();
        //接している住戸の種類数
        int contactNumber = 0;
        for (int i = 0; i < contactStairs.Count; i++) {
            for (int j = 0; j < contactStairs[i].Count; j++) {
                if (!countList.Contains(contactStairs[i][j])) {
                    countList.Add(contactStairs[i][j]);
                    contactNumber++;
                }
            }
        }
        
        /* contactStairsに3住戸以上の組が含まれるとき，2住戸ずつの組み合わせに分ける */
        //Coming Soon...

        /* 2住戸にまたがるMBPSの全配置パターンを作成 */
        //組み合わせ決定用の配列
        int[] pattern = new int[contactStairs.Count];

        //2住戸にまたがるMBPSの全配置パターンを作成
        combination(pattern, contactNumber / 2, 0);
    }

    /// <summary>
    /// elemsの要素からr個を選ぶ組み合わせのパターンを0, 1で作成する
    /// </summary> 
    /// <param name="pattern">組み合わせ決定用の配列</param>
    /// <param name="elems">組み合わせを求めたい配列</param>
    /// <param name="r">何個ずつ選ぶかを決める変数</param>
    /// <param name="num_decided">選ぶかどうかが決まっている要素の数</param>
    void combination(int[] pattern, int r, int num_decided) {
        //選ぶかどうかが決まっている要素の数
        int num_selected = getNumSelected(pattern, num_decided);

        //全ての要素に対して"選ぶ"or"選ばない"が決定ずみ
        if (num_decided == pattern.Length) {
            //r個だけ選ばれているとき
            if (num_selected == r) {
                //0, 1を使って要素の組み合わせを作成
                makeCombinationList(pattern);
            }

            //r個だけ選ばれているもの以外は無視
            return;
        }

        /* num_decided個目の要素を"選ぶ"場合のパターンを作成 */
        pattern[num_decided] = 1;
        combination(pattern, r, num_decided + 1);

        /* num_decided個目の要素を"選ばない"場合のパターンを作成 */
        pattern[num_decided] = 0;
        combination(pattern, r, num_decided + 1);
    }

    /// <summary>
    /// "選ぶ"と決定された要素の数（patternの1の数）を計算
    /// </summary> 
    /// <param name="pattern">組み合わせ決定用の配列</param>
    /// <param name="num_decided">選ぶかどうかが決まっている要素の数</param>
    /// <returns>前からnum_decided個のpatternの内，いくつ1があるかを返す</returns>
    int getNumSelected(int[] pattern, int num_decided) {
        //選ぶかどうかが決まっている要素の数
        int num_selected = 0;

        //前からnum_decided個のpatternの内，いくつ1があるかを計算
        for (int i = 0; i < num_decided; i++) {
            num_selected += pattern[i];
        }

        return num_selected;
    }
    
    /// <summary>
    /// 0, 1の組み合わせパターンに従って，2住戸にまたがるMBPSの配置パターンを作成
    /// </summary> 
    /// <param name="pattern">組み合わせ決定用の配列</param>
    void makeCombinationList(int[] pattern) {
        //2住戸にまたがるMBPSの配置パターンのひとつ
        List<List<int>> twoDwellingsMbpsPattern = new List<List<int>>();

        //0, 1のパターンに従って2住戸にまたがるMBPSの配置パターンのひとつを作成
        for (int i = 0; i < pattern.Length; i++) {
            if (pattern[i] == 1) {
                twoDwellingsMbpsPattern.Add(contactStairs[i]);
            }
        }

        //配置パターンの要素に重複がないかを確認
        for (int i = 0; i < twoDwellingsMbpsPattern.Count - 1; i++) {
            for (int j = i + 1; j < twoDwellingsMbpsPattern.Count; j++) {
                if ((twoDwellingsMbpsPattern[i][0] == twoDwellingsMbpsPattern[j][0]) || (twoDwellingsMbpsPattern[i][0] == twoDwellingsMbpsPattern[j][1])) {
                    //一つでも重複があればここで終了
                    return;
                }

                if ((twoDwellingsMbpsPattern[i][1] == twoDwellingsMbpsPattern[j][0]) || (twoDwellingsMbpsPattern[i][1] == twoDwellingsMbpsPattern[j][1])) {
                    //一つでも重複があればここで終了
                    return;
                }
            }
        }

        //配置パターンを全配置パターンのリストに追加
        twoDwellingsMbpsAllPattern.Add(twoDwellingsMbpsPattern);
    }

    /// <summary>
    /// 住戸の座標のリストを作成
    /// </summary> 
    public List<Vector3[]> makeDwellingCoordinatesList(Dictionary<string, Dictionary<string, Vector3[]>> plan) {
        //作成結果
        var result = new List<Vector3[]>();

        foreach (KeyValuePair<string, Dictionary<string, Vector3[]>> space in plan) {
            if (space.Key.Contains("Dwelling")) {
                foreach (KeyValuePair<string, Vector3[]> spaceElements in space.Value) {
                    if (spaceElements.Key.Contains("1K")) {
                        result.Add(spaceElements.Value);
                    }
                }
            }
        }

        return result;
    }
}
