using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CreateWetareas : FloorPlanner
{
    Vector3[] dwelling;
    Vector3[] balcony;
    Vector3[] range;
    Vector3[] entrance;
    Vector3[] mbps;

    public List<Dictionary<string, Dictionary<string, Vector3[]>>> PlaceWetareas(List<Dictionary<string, Dictionary<string, Vector3[]>>> allPattern) {
        //配置結果のリスト
        var result = new List<Dictionary<string, Dictionary<string, Vector3[]>>>();

        /* 全パターンについて配置 */
        for (int i = 0; i < 1/*allPattern.Count*/; i++) {
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

                    //水回りを配置したリストを作成
                    List<Dictionary<string, Vector3[]>> placeWetareasResult = MakeWetareasAddList(new int[]{0, 1, 2, 3});

                    //作成したリストを配置結果のリストに追加
                    for (int j = 0; j < placeWetareasResult.Count; j++) {
                        //配置したパターン
                        Dictionary<string, Dictionary<string, Vector3[]>> patternToPlace = DuplicateDictionary(allPattern[i]);

                        foreach (KeyValuePair<string, Vector3[]> wetAreaRoom in placeWetareasResult[j]) {
                            patternToPlace[space.Key].Add(wetAreaRoom.Key, wetAreaRoom.Value);
                        }

                        result.Add(patternToPlace);
                    }
                }
            }
        }

        return result; 
    }

    //リストの作成
    public List<Dictionary<string, Vector3[]>> MakeWetareasAddList(int[] wetAreasKinds) {
        var result = new List<Dictionary<string, Vector3[]>>();

        //水回りパーツの組み合わせ
        List<int[]> wetAreasAllPermutation = AllPermutation(wetAreasKinds);   
        for (int i = 0; i < wetAreasAllPermutation.Count; i++) {
            //Debug.Log("i: " + i);
            //回転のパターンの組み合わせ
            List<int[]> rotationAllPattern = flagPatternList(wetAreasAllPermutation[i].Length);
            for (int j = 0; j < rotationAllPattern.Count; j++) {
                //Debug.Log("j: " + j);
                //洗面室とキッチンの回転は除く
                if (rotationAllPattern[j][3] == 1 || rotationAllPattern[j][1] == 1) {
                    continue;
                }

                /* 洋室あり */
                /*
                List<Dictionary<string, Vector3[]>> wetAreasResult = PlaceWetareasOnePattern(wetAreasAllPermutation[i], rotationAllPattern[j]);
                var westernResult = new List<Dictionary<string, Vector3[]>>();
                for (int k = 0; k < wetAreasResult.Count; k++) {
                    //Debug.Log("k: " + k);

                    westernResult.AddRange(CreateWestern(wetAreasResult[k]));
                }

                result.AddRange(westernResult);
                */
                
                /* 洋室なし */
                result.AddRange(PlaceWetareasOnePattern(wetAreasAllPermutation[i], rotationAllPattern[j]));
            }
        }

        return result;
    }

    //部屋パーツ配置
    public List<Dictionary<string, Vector3[]>> PlaceWetareasOnePattern(int[] wetAreasPermutation, int[] rotationPattern) {
        var result = new List<Dictionary<string, Vector3[]>>();

        //玄関の廊下に続く辺を決める
        List<Vector3[]> entrance_side = new List<Vector3[]>();
        for (int i = 0; i < entrance.Length; i++) {
            Vector3[] entrance_contact = contact(new Vector3[]{entrance[i], entrance[(i+1)%entrance.Length]}, range);

            if (!ZeroJudge(entrance_contact)) {
                entrance_side.Add(entrance_contact);
            }
        }

        //洋室の廊下に続く辺を決める
        List<Vector3[]> western_side = new List<Vector3[]>();
        for (int i = 0; i < balcony.Length; i++) {
            Vector3[] balcony_contact = contact(new Vector3[]{balcony[i], balcony[(i+1)%balcony.Length]}, range);
            if (!ZeroJudge(balcony_contact)) {
                western_side.Add(balcony_contact);
            }
        }
        //western_side = new Vector3[]{new Vector3(-3400, -1900, 0), new Vector3(-3400, 1900, 0)};

        //玄関から洋室へつながる辺の決定
        List<Vector3[]> hallway_side = new List<Vector3[]>();

        int entranceSideIndex = 1;
        int westernSideIndex = 0;

        if (!CrossJudge(new Vector3[]{entrance_side[entranceSideIndex][0], western_side[westernSideIndex][0]}, new Vector3[]{entrance_side[entranceSideIndex][1], western_side[westernSideIndex][1]})) {
            hallway_side.Add(new Vector3[]{entrance_side[entranceSideIndex][0], western_side[westernSideIndex][0]});
            hallway_side.Add(new Vector3[]{entrance_side[entranceSideIndex][1], western_side[westernSideIndex][1]});
        }
        else {
            hallway_side.Add(new Vector3[]{entrance_side[entranceSideIndex][0], western_side[westernSideIndex][1]});
            hallway_side.Add(new Vector3[]{entrance_side[entranceSideIndex][1], western_side[westernSideIndex][0]});
        }

        /* 玄関から洋室へつながる辺に従って領域を切る */
        //水回りを配置する領域
        List<Vector3[]> wetAreas = new List<Vector3[]>();

        //辺0を通るように切る
        List<Vector3[]> wetAreas0Candidates = Slice(range, hallway_side[0]);
        //辺1が含まれている方の領域を除く
        for (int i = 0; i < wetAreas0Candidates.Count; i++) {
            if (PolyogonContainsPoints(wetAreas0Candidates[i], hallway_side[1])) {
                wetAreas0Candidates.RemoveAt(i);
            }
        }
        //水回りを配置する領域のリストに追加
        wetAreas.AddRange(wetAreas0Candidates);

        //辺1を通るように切る
        List<Vector3[]> wetAreas1Candidates = Slice(range, hallway_side[1]);
        //辺0が含まれている方の領域を除く
        for (int i = 0; i < wetAreas1Candidates.Count; i++) {
            if (PolyogonContainsPoints(wetAreas1Candidates[i], hallway_side[0])) {
                wetAreas1Candidates.RemoveAt(i);
            }
        }
        //水回りを配置する領域のリストに追加
        wetAreas.AddRange(wetAreas1Candidates);


        //面積の大きい方を求める
        if (areaCalculation(wetAreas[0]) < areaCalculation(wetAreas[1])) {
            wetAreas.Reverse();
            hallway_side.Reverse();
        }

        //長い辺から順に並べていく
        int[] longIndex1 = LongIndex(wetAreas[0]);
        for (int i = 0; i < longIndex1.Length; i++) {
            Vector3[] current_side = new Vector3[] {wetAreas[0][longIndex1[i]], wetAreas[0][(longIndex1[i]+1)%longIndex1.Length]};

            if ((current_side[0] == hallway_side[0][0] && current_side[1] == hallway_side[0][1]) || (current_side[0] == hallway_side[0][1] && current_side[1] == hallway_side[0][0])) {
                if (longIndex1.Length > 2) {
                    continue;
                }
            }
        
            int currentPatternTotal = result.Count;
            if (currentPatternTotal == 0) {
                currentPatternTotal = 1;
            }

            //パターンのリストをひとつひとつ渡す
            for (int j = 0; j < currentPatternTotal; j++) {
                //これから配置する部屋の状況
                Dictionary<string, Vector3[]> currentPattern = new Dictionary<string, Vector3[]>();
                if (result.Count != 0) {
                    currentPattern = result[j];
                    
                    current_side = new Vector3[] {wetAreas[0][longIndex1[i]], wetAreas[0][(longIndex1[i]+1)%longIndex1.Length]};
                    for (int k = 0; k < currentPattern.Count; k++) {
                        Vector3[] contact_coordinates = contact(current_side, currentPattern.Values.ElementAt(k));
                        if (!ZeroJudge(contact_coordinates)) {
                            current_side = SideSubstraction(current_side, contact_coordinates)[0];
                        }
                    }
                }

                //全ての部屋が配置されている場合
                if (currentPattern.ContainsKey("UB") && currentPattern.ContainsKey("Washroom") && currentPattern.ContainsKey("Toilet") && currentPattern.ContainsKey("Kitchen")) {
                    continue;
                }

                //配置できるだけ配置する
                Dictionary<string, Vector3[]> placementResult = CenterOfPlacement(currentPattern, wetAreasPermutation, rotationPattern, current_side);

                if (result.Count == 0) {
                    result.Add(placementResult);
                }

                //重複がある場合は追加しない
                for (int l = 0; l < result.Count; l++) {
                    if (DictionaryEquals(result[l], placementResult)) {
                        break;
                    }
                    if (l == result.Count - 1) {
                        result.Add(placementResult);
                    }
                }

                string[] differentKeys = KeysDifference(placementResult, currentPattern, wetAreasPermutation);

                for (int k = 1; k < differentKeys.Length + 1; k++) {
                    string[] keysToRemove = differentKeys[^k..];
                    placementResult = RemoveDictionaryElement(placementResult, keysToRemove);
                    //重複がある場合は追加しない
                    for (int l = 0; l < result.Count; l++) {
                        if (DictionaryEquals(result[l], placementResult)) {
                            break;
                        }
                        if (l == result.Count - 1) {
                            result.Add(placementResult);
                        }
                    }
                }
            }

            //break;
        }

        //長い辺から順に並べていく
        int[] longIndex2 = LongIndex(wetAreas[1]);
        for (int i = 0; i < longIndex2.Length; i++) {
            Vector3[] current_side = new Vector3[] {wetAreas[1][longIndex2[i]], wetAreas[1][(longIndex2[i]+1)%longIndex2.Length]};

            if ((current_side[0] == hallway_side[1][0] && current_side[1] == hallway_side[1][1]) || (current_side[0] == hallway_side[1][1] && current_side[1] == hallway_side[1][0])) {
                if (longIndex2.Length > 2) {
                    continue;
                }
            }

            int currentPatternTotal = result.Count;
            if (currentPatternTotal == 0) {
                currentPatternTotal = 1;
            }

            //パターンのリストをひとつひとつ渡す
            for (int j = 0; j < currentPatternTotal; j++) {
                //これから配置する部屋の状況
                Dictionary<string, Vector3[]> currentPattern = new Dictionary<string, Vector3[]>();
                if (result.Count != 0) {
                    currentPattern = result[j];
                    
                    current_side = new Vector3[] {wetAreas[1][longIndex2[i]], wetAreas[1][(longIndex2[i]+1)%longIndex2.Length]};
                    for (int k = 0; k < currentPattern.Count; k++) {
                        Vector3[] contact_coordinates = contact(current_side, currentPattern.Values.ElementAt(k));
                        if (!ZeroJudge(contact_coordinates)) {
                            current_side = SideSubstraction(current_side, contact_coordinates)[0];
                        }
                    }
                }

                //全ての部屋が配置されている場合
                if (currentPattern.ContainsKey("UB") && currentPattern.ContainsKey("Washroom") && currentPattern.ContainsKey("Toilet") && currentPattern.ContainsKey("Kitchen")) {
                    continue;
                }

                //配置できるだけ配置する
                Dictionary<string, Vector3[]> placementResult = CenterOfPlacement(currentPattern, wetAreasPermutation, rotationPattern, current_side);

                //重複がある場合は追加しない
                for (int l = 0; l < result.Count; l++) {
                    if (DictionaryEquals(result[l], placementResult)) {
                        break;
                    }
                    if (l == result.Count - 1) {
                        result.Add(placementResult);
                    }
                }

                string[] differentKeys = KeysDifference(placementResult, currentPattern, wetAreasPermutation);

                for (int k = 1; k < differentKeys.Length + 1; k++) {
                    string[] keysToRemove = differentKeys[^k..];
                    placementResult = RemoveDictionaryElement(placementResult, keysToRemove);
                    //重複がある場合は追加しない
                    for (int l = 0; l < result.Count; l++) {
                        if (DictionaryEquals(result[l], placementResult)) {
                            break;
                        }
                        if (l == result.Count - 1) {
                            result.Add(placementResult);
                        }
                    }
                }
            }

            //break;
        }

        //必要な部屋がすべて配置されていないものを除く
        for (int i = 0; i < result.Count; i++) {
            if (!(result[i].ContainsKey("UB") && result[i].ContainsKey("Washroom") && result[i].ContainsKey("Toilet") && result[i].ContainsKey("Kitchen"))) {
                result.RemoveAt(i);
                i--;
            }
        }

        //玄関が封鎖されているものを除く
        for (int i = 0; i < result.Count; i++) {
            foreach (Vector3[] value in result[i].Values) {
                Vector3[] contact_entrance = contact(entrance_side[entranceSideIndex], value);
                if (!ZeroJudge(contact_entrance)) {
                    if (Vector3.Distance(contact_entrance[0], contact_entrance[1]) > 600f) {
                        result.RemoveAt(i);
                        i--;
                    }
                }
            }
        } 

        
        //洗面室とユニットバスが隣り合っていないものを除く
        for (int i = 0; i < result.Count; i++) {
            bool ajacent_flag = true;
            for (int j = 0; j < result[i]["UB"].Length; j++) {
                Vector3[] contact_ub_wash = contact(new Vector3[]{result[i]["UB"][j], result[i]["UB"][(j+1)%balcony.Length]}, result[i]["Washroom"]);
                if (!ZeroJudge(contact_ub_wash)) {
                    ajacent_flag = false;
                }
            }

            if (ajacent_flag) {
                result.RemoveAt(i);
                i--;
            }
        }
        
        return result;
    }

    /***

    配置の大事なとこ？

    ***/
    public Dictionary<string, Vector3[]> CenterOfPlacement(Dictionary<string, Vector3[]> placementPattern, int[] wetAreasPermutation, int[] rotationPattern, Vector3[] current_side) {
        var result = new Dictionary<string, Vector3[]>(placementPattern);

        for (int j = 0; j < wetAreasPermutation.Length; j++) {
            bool break_flag = true;
            Vector3[] current_room = pa.ub_coordinates;
            string current_room_name = "None";

            if (wetAreasPermutation[j] == 0) {
                if (result.ContainsKey("UB")) {
                    continue;
                }

                if (rotationPattern[wetAreasPermutation[j]] == 0) {
                    current_room = pa.ub_coordinates;
                } else {
                    current_room = Rotation(pa.ub_coordinates);
                }
                current_room_name = "UB";

            } 
            else if (wetAreasPermutation[j] == 1) {
                if (result.ContainsKey("Washroom")) {
                    continue;
                }

                if (rotationPattern[wetAreasPermutation[j]] == 0) {
                    current_room = pa.washroom_coordinates;
                } else {
                    current_room = Rotation(pa.washroom_coordinates);
                }
                current_room_name = "Washroom";
                
            }
            else if (wetAreasPermutation[j] == 2) {
                if (result.ContainsKey("Toilet")) {
                    continue;
                }
                
                if (rotationPattern[wetAreasPermutation[j]] == 0) {
                    current_room = pa.toilet_coordinates;
                } else {
                    current_room = Rotation(pa.toilet_coordinates);
                }
                current_room_name = "Toilet";
                
            }
            else if (wetAreasPermutation[j] == 3) {
                if (result.ContainsKey("Kitchen")) {
                    continue;
                }
                
                if (rotationPattern[wetAreasPermutation[j]] == 0) {
                    current_room = pa.kitchen_coordinates;
                } else {
                    current_room = Rotation(pa.kitchen_coordinates);
                }              
                current_room_name = "Kitchen";
                
            }

            Vector3[] CreateCoordinates = new Vector3[current_room.Length];

            float gap_x = 0;
            float gap_y = 0;

            if (current_side[0] != current_side[1]) {
                if (current_side[0].x == current_side[1].x) {
                    float max = current_room[0].y;
                    for (int k = 1; k < current_room.Length; k++) {
                        if (max < current_room[k].y) {
                            max = current_room[k].y;
                        }
                    }

                    gap_y = Mathf.Max(current_side[0].y, current_side[1].y) - max;

                    for (int k = 0; k < ContactGap(current_room, current_side).Length; k++) {
                        //部屋が水回り範囲内に配置できるかの判定
                        if (JudgeInside(range, CorrectCoordinates(current_room, new Vector3(ContactGap(current_room, current_side)[k], gap_y, 0)))) {
                            gap_x = ContactGap(current_room, current_side)[k];

                            //部屋が他の部屋の外側にあるかの判定
                            bool outside_flag = true;
                            foreach (Vector3[] value in result.Values) {
                                if (!JudgeOutside(value, CorrectCoordinates(current_room, new Vector3(gap_x, gap_y, 0)))) {
                                    outside_flag = false;
                                }
                                if (!JudgeOutside(CorrectCoordinates(current_room, new Vector3(gap_x, gap_y, 0)), value)) {
                                    outside_flag = false;
                                }
                            }

                            if (outside_flag) {
                                //部屋が辺上におさまっているかの判定
                                int contactIndex = 0;
                                Vector3[] tempCoordinates = CorrectCoordinates(current_room, new Vector3(gap_x, gap_y, 0));
                                for (int l = 0; l < tempCoordinates.Length; l++) {
                                    Vector3[] current_contact_coordinates = contact(current_side, new Vector3[] {tempCoordinates[l], tempCoordinates[(l+1)%tempCoordinates.Length]});

                                    if (!ZeroJudge(current_contact_coordinates)) {
                                        contactIndex = l;
                                    }
                                }

                                if (ContainsSide(current_side, new Vector3[] {tempCoordinates[contactIndex], tempCoordinates[(contactIndex+1)%tempCoordinates.Length]})) {
                                    break_flag = false;
                                }
                            }
                        }
                    }
                }
                else if (current_side[0].y == current_side[1].y) {
                    float max = current_room[0].x;
                    for (int k = 1; k < current_room.Length; k++) {
                        if (max < current_room[k].x) {
                            max = current_room[k].x;
                        }
                    }

                    gap_x = Mathf.Max(current_side[0].x, current_side[1].x) - max;

                    for (int k = 0; k < ContactGap(current_room, current_side).Length; k++) {
                        if (JudgeInside(range, CorrectCoordinates(current_room, new Vector3(gap_x, ContactGap(current_room, current_side)[k], 0)))) {
                            gap_y = ContactGap(current_room, current_side)[k];

                            //部屋が他の部屋の外側にあるかの判定
                            bool outside_flag = true;
                            foreach (Vector3[] value in result.Values) {
                                if (!JudgeOutside(value, CorrectCoordinates(current_room, new Vector3(gap_x, gap_y, 0)))) {
                                    outside_flag = false;
                                }
                                if (!JudgeOutside(CorrectCoordinates(current_room, new Vector3(gap_x, gap_y, 0)), value)) {
                                    outside_flag = false;
                                }
                            }

                            if (outside_flag) {
                                //部屋が辺上におさまっているかの判定
                                int contactIndex = 0;
                                Vector3[] tempCoordinates = CorrectCoordinates(current_room, new Vector3(gap_x, gap_y, 0));

                                for (int l = 0; l < tempCoordinates.Length; l++) {
                                    Vector3[] current_contact_coordinates = contact(current_side, new Vector3[] {tempCoordinates[l], tempCoordinates[(l+1)%tempCoordinates.Length]});

                                    if (!ZeroJudge(current_contact_coordinates)) {
                                        contactIndex = l;
                                    }
                                }

                                if (ContainsSide(current_side, new Vector3[] {tempCoordinates[contactIndex], tempCoordinates[(contactIndex+1)%tempCoordinates.Length]})) {
                                    break_flag = false;
                                }
                            }
                        }
                    }
                }
            }

            //部屋が配置できなければ，リストに追加せずに次のパターンへ
            if (break_flag) {
                break;
            }

            CreateCoordinates = CorrectCoordinates(current_room, new Vector3(gap_x, gap_y, 0));
            result.Add(current_room_name, CreateCoordinates);
            
            //追加した部屋の長さ分，current_sideを更新
            Vector3[] contact_coordinates = contact(current_side, CreateCoordinates);

            if (!ZeroJudge(contact_coordinates)) {
                current_side = SideSubstraction(current_side, contact_coordinates)[0];
            }
        }

        return result;
    }
    
    /***

    辞書のキーの差分を返す

    ***/
    public string[] KeysDifference(Dictionary<string, Vector3[]> addDictionary, Dictionary<string, Vector3[]> originalDictionary, int[] wetAreasPermutation) {
        List<string> result = new List<string>();

        for (int i = 0; i < wetAreasPermutation.Length; i++) {
            string key = "";

            if (wetAreasPermutation[i] == 0) {
                key = "UB";
            } 
            else if (wetAreasPermutation[i] == 1) {
                key = "Washroom";
            }
            else if (wetAreasPermutation[i] == 2) {
                key = "Toilet";
            }
            else if (wetAreasPermutation[i] == 3) {
                key = "Kitchen";
            }

            if (addDictionary.ContainsKey(key) && !originalDictionary.ContainsKey(key)) {
                result.Add(key);
            }
        }

        return result.ToArray();
    }

    /***

    辞書からキーを除いた別の辞書を返す

    ***/
    public Dictionary<string, Vector3[]> RemoveDictionaryElement(Dictionary<string, Vector3[]> dictionary, string[] keys) {
        Dictionary<string, Vector3[]> result = new Dictionary<string, Vector3[]>(dictionary);

        for (int i = 0; i < keys.Length; i++) {
            result.Remove(keys[i]);
        }

        return result;
    }
    
    /// <summary>
    /// 水回りと洋室を配置した住戸に対して評価指標を用いて評価
    /// </summary> 
    /// <param name="allPattern">全パターンのリスト</param>
    /// <returns>評価の高いもののリストを返す</returns>
    public List<Dictionary<string, Vector3[]>> Evaluation(List<Dictionary<string, Vector3[]>> allPattern) {
        //評価指標によって絞ったパターンのリスト
        var selectedPattern = new List<Dictionary<string, Vector3[]>>();

        //評価指標のリスト
        var westernSizeRatioList = new Dictionary<int, float>(); //全パターンのリストと対応付けるためのインデックスと洋室の大きさの割合の辞書
        var westernShapeRatioList = new Dictionary<int, float>(); //全パターンのリストと対応付けるためのインデックスと洋室の形状の割合の辞書


        Vector3[] hallway = range;

        /* 評価指標のリストを作成 */
        //全パターンについてひとつずつ評価
        for (int i = 0; i < allPattern.Count; i++) {
            //Debug.Log("i: " + i);

            //廊下の座標
            hallway = dwelling;

            //リストを整える
            foreach (string roomName in allPattern[i].Keys) {
                hallway = FrameChange(NumberClean(hallway), NumberClean(allPattern[i][roomName]));
            }

            /*
            //廊下の幅の最小値を算出
            float hallwayLengthMin = Vector3.Distance(hallway[0], hallway[1]);
            for (int j = 0; j < hallway.Length - 1; j++) {
                for (int k = j + 1; k < hallway.Length; k++) {
                    if (Vector3.Distance(hallway[j], hallway[k]) == 0) {
                        Debug.Log("                                    0");
                    }
                    if (Vector3.Distance(hallway[j], hallway[k]) < hallwayLengthMin) {
                        hallwayLengthMin = Vector3.Distance(hallway[j], hallway[k]);
                    }
                }
            }

            //廊下の幅の最小値が700より小さい場合
            if (hallwayLengthMin < 700.0f) {
                //allPatternから削除
                allPattern.RemoveAt(i);
                i--;
                continue;
            }
            */
        }

        for (int i = 0; i < allPattern.Count; i++) {
            //洋室の座標
            Vector3[] westernCoordinates = allPattern[i]["Western"];

            /* 洋室の面積の割合を算出 */
            //洋室の面積
            float westernSize = areaCalculation(westernCoordinates);
            //住戸の面積
            float dwellingSize = areaCalculation(dwelling);
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

        Debug.Log("allPattern.Count: " + allPattern.Count);

        /* 得点の算出 */
        //洋室の面積の割合に洋室の形状の割合を掛ける
        var westernSizeShapeRatioList = new Dictionary<int, float>();
        foreach (KeyValuePair<int, float> kvp in westernSizeRatioList) {
            westernSizeShapeRatioList.Add(kvp.Key, kvp.Value * westernShapeRatioList[kvp.Key]);
        }

        //評価指標の辞書を評価指標の降順にソート
        var westernSizeRatioListSort = westernSizeRatioList.OrderByDescending((x) => x.Value);
        

        // ↓↓↓ここから先の手法は要検討

        //評価指標の辞書のインデックスに合わせて全パターンのリストをソート
        var allPatternSort = new List<Dictionary<string, Vector3[]>>();
        foreach (KeyValuePair<int, float> kvp in westernSizeRatioListSort) {
            allPatternSort.Add(allPattern[kvp.Key]);
        }
        

        /* 評価指標のリストをもとにパターンを絞る */
        //全パターンのリストのうち，前から20個を選択
        for (int i = 0; i < 20/*allPatternSort.Count*/; i++) {
            selectedPattern.Add(allPatternSort[i]);
        }

        return selectedPattern;
    }

    /// <summary>
    /// 2つの辞書が等しいかどうかを判定
    /// </summary> 
    /// <param name="dictionaryA">辞書A</param>
    /// <param name="dictionaryB">辞書B</param>
    /// <returns>2つの辞書が等しいときtrue, そうでないときfalse</returns>
    public bool DictionaryEquals(Dictionary<string, Vector3[]> dictionaryA, Dictionary<string, Vector3[]> dictionaryB) {
        //判定結果
        bool flag = false;

        //辞書の要素数が異なる場合
        if (dictionaryA.Count != dictionaryB.Count) {
            return flag;
        }

        //辞書のキーが異なる場合
        foreach (string key in dictionaryA.Keys) {
            if (!dictionaryB.ContainsKey(key)) {
                return flag;
            }
        }

        //辞書の値の長さ異なる場合
        foreach (string key in dictionaryA.Keys) {
            if (dictionaryA[key].Length != dictionaryB[key].Length) {
                return flag;
            }
        }

        //辞書の値が異なる場合
        foreach (string key in dictionaryA.Keys) {
            for (int i = 0; i < dictionaryA[key].Length; i++) {
                if (dictionaryA[key][i].x != dictionaryB[key][i].x) {
                    return flag;
                }

                if (dictionaryA[key][i].y != dictionaryB[key][i].y) {
                    return flag;
                }
            }
        }

        flag = true;

        return flag;
    }

    /***

    線分の内部に線分が含まれるかどうか

    ***/
    public bool ContainsSide(Vector3[] outerSide, Vector3[] innerSide) {
        bool flag = false;
        
        //入力が線分でない場合
        if (!(outerSide.Length == 2 && innerSide.Length == 2)) {
            return flag;
        }

        //含まれるかどうかの判定
        if (Vector3.Distance(outerSide[0], innerSide[0]) + Vector3.Distance(innerSide[0], outerSide[1]) == Vector3.Distance(outerSide[0], outerSide[1])) {
            if (Vector3.Distance(outerSide[0], innerSide[1]) + Vector3.Distance(innerSide[1], outerSide[1]) == Vector3.Distance(outerSide[0], outerSide[1])) {
                flag = true;
            }
        }

        return flag;
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

        return slicePolygons;
    }

    /// <summary>
    /// 多角形の辺上(頂点含む)に点が含まれるかどうか
    /// </summary> 
    /// <param name="polygon">多角形の座標配列</param>
    /// <param name="points">含まれるか調べる点の座標の配列</param>
    /// <returns>内分の場合true，外分の場合flase</returns>
    public bool PolyogonContainsPoints(Vector3[] polygon, Vector3[] points) {
        //返す判定
        bool flag = false;
        //多角形の辺上に含まれる点の数
        int trueCounter = 0;

        //全ての点について調べる
        for (int i = 0; i < points.Length; i++) {
            //多角形の辺に含まれるかどうか
            for (int j = 0; j < polygon.Length; j++) {
                //多角形の辺
                Vector3[] side = new Vector3[]{polygon[j], polygon[(j+1)%polygon.Length]};

                //多角形の辺上に含まれる場合
                if (OnLineSegment(side, points[i])) {
                    //カウントを増やし，次の点に移る
                    trueCounter++;
                    break;
                }
            }
        }

        //全ての点が多角形の辺上に含まれる場合
        if (trueCounter >= points.Length) {
            flag = true;
        }

        return flag;
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

    public Vector3[] NumberClean (Vector3[] coodinates) {
        //返すリスト
        Vector3[] cleanCoodinates = new Vector3[coodinates.Length];

        for (int i = 0; i < coodinates.Length; i++) {
            cleanCoodinates[i].x = (float) Math.Truncate(coodinates[i].x);
            cleanCoodinates[i].y = (float) Math.Truncate(coodinates[i].y);
        }

        return cleanCoodinates;
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


    /***

    辺が長い順のインデックスの配列を返す

    ***/
    public int[] LongIndex(Vector3[] room) {
        int[] result = new int[room.Length];

        var length = new Dictionary<int, float>();
        for (int i = 0; i < room.Length; i++) {
            length.Add(i, Vector3.Distance(room[i], room[(i+1)%room.Length]));
        }

        for (int i = 0; i < result.Length; i++) {
            result[i] = length.FirstOrDefault(x => x.Value == length.Values.Max()).Key;
            length.Remove(result[i]);
        }

        return result;
    }

    /***

    順列の全組み合わせを返す

    ***/
    public List<int[]> AllPermutation(params int[] array)
    {
        var a = new List<int>(array).ToArray();
        var res = new List<int[]>();
        res.Add(new List<int>(a).ToArray());
        var n = a.Length;
        var next = true;
        while (next)
        {
            next = false;

            //隣り合う要素が昇順(a[i] < a[i+1])になっている一番大きい i を見つける
            int i;
            for (i = n - 2; i >= 0; i--)
            {
                if (a[i].CompareTo(a[i + 1]) < 0) break;
            }
            //i が見つからないとき、全体が降順になっているので処理終了
            if (i < 0) break;

            //末尾から順番に見て、a[i] より大きい要素のインデックス j を見つける
            var j = n;
            do
            {
                j--;
            } while (a[i].CompareTo(a[j]) > 0);

            if (a[i].CompareTo(a[j]) < 0)
            {
                //a[i] と a[j] を入れ替え、i+1以降の要素を反転する
                var tmp = a[i];
                a[i] = a[j];
                a[j] = tmp;
                Array.Reverse(a, i + 1, n - i - 1);
                res.Add(new List<int>(a).ToArray());
                next = true;
            }
        }
        return res;
    }

    /***

    2^n通りの0,1の組み合わせを生成

    ***/
    public List<int[]> flagPatternList(int n) {
        List<int[]> flagPatternList = new List<int[]>();

        //0~2^nについて
        for (int i = 0; i < Mathf.Pow(2, n); i++) {
            //iを2進数の文字列に変換
            string binaryString = Convert.ToString (i, 2);

            //n桁の2進数の文字列に直す
            string nDigitsBinaryString;
            if (binaryString.Length != n) {
                nDigitsBinaryString = new string('0', n - binaryString.Length) + binaryString;
            } else {
                nDigitsBinaryString = binaryString;
            }

            //1文字ずつintに変換し，配列にする
            int[] flagPattern = new int[n];
            for (int j = 0; j < n; j++) {
                flagPattern[j] = int.Parse(nDigitsBinaryString.Substring(j, 1));
            }
            
            //配列をリストに追加
            flagPatternList.Add(flagPattern);
        }

        return flagPatternList;
    }
}
