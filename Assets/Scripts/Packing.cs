using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Packing : CreateRoom
{
    [SerializeField] Parts pa;
    Vector3[] range;
    Vector3[] dwelling;
    Vector3[] balcony;
    Vector3[] entrance;
    Vector3[] mbps;
    List<Dictionary<string, Vector3[]>> allPattern = new List<Dictionary<string, Vector3[]>>();
    //List<Dictionary<string, Vector3[]>> results = new List<Dictionary<string, Vector3[]>>();
    int count = 0;
    int limit = 0;

    void Start()
    {   
        //住戸作成
        dwelling = new Vector3[]{new Vector3(-3400, 1900, 0), new Vector3(4400, 1900, 0), new Vector3(4400, -1900, 0), new Vector3(-3400, -1900, 0)};
        createRoom("dwelling", dwelling);

        //バルコニー作成
        balcony = new Vector3[]{new Vector3(-4400, 1100, 0), new Vector3(-3400, 1100, 0), new Vector3(-3400, -1900, 0), new Vector3(-4400, -1900, 0)};
        createRoom("balcony", balcony);
        
        //玄関作成
        entrance = new Vector3[]{new Vector3(3400, -50, 0), new Vector3(4400, -50, 0), new Vector3(4400, -1550, 0), new Vector3(3400, -1550, 0)};
        createRoom("entrance", entrance);
        //MBPS作成
        mbps = new Vector3[]{new Vector3(3400, -1550, 0), new Vector3(4400, -1550, 0), new Vector3(4400, -1900, 0), new Vector3(3400, -1900, 0)};
        createRoom("mbps", mbps);

        //水回り範囲の決定
        //住戸から玄関を除いた範囲
        range = FrameChange(dwelling, entrance);
        //さらにMBPSを除いた範囲
        range = FrameChange(range, mbps);
        
        
        if (true) {
            //リストの作成
            allPattern = PlacementListCreate(new int[]{0, 1, 2, 3});
        } else {
            //初めの方だけリストの作成
            allPattern = PlacementListCreate(new int[]{0, 1, 2, 3}, 0, 0);
        }

        
        limit = allPattern.Count;
        Debug.Log("総パターン数：" + limit);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) {
            if (count < limit) {
                Debug.Log((count+1) + "パターン目");

                if (GameObject.Find("UB")) {
                    Destroy(GameObject.Find("UB"));
                }
                if (GameObject.Find("Washroom")) {
                    Destroy(GameObject.Find("Washroom"));
                }
                if (GameObject.Find("Toilet")) {
                    Destroy(GameObject.Find("Toilet"));
                }
                if (GameObject.Find("Kitchen")) {
                    Destroy(GameObject.Find("Kitchen"));
                }
                if (GameObject.Find("Western")) {
                    Destroy(GameObject.Find("Western"));
                }

                Dictionary<string, Vector3[]> currentPattern = allPattern[count];

                foreach (string roomName in currentPattern.Keys) {
                    if (roomName == "UB") {
                        createRoom(roomName, currentPattern[roomName], Color.cyan);
                    }
                    if (roomName == "Washroom") {
                        createRoom(roomName, currentPattern[roomName], Color.magenta);
                    }
                    if (roomName == "Toilet") {
                        createRoom(roomName, currentPattern[roomName], Color.green);
                    }
                    if (roomName == "Kitchen") {
                        createRoom(roomName, currentPattern[roomName], Color.gray);
                    }
                    if (roomName == "Western") {
                        createRoom(roomName, currentPattern[roomName], Color.yellow);
                    }
                }
                count++;
            } else {
                Debug.Log("終了");
            }
        }
    }

    //リストの作成
    public List<Dictionary<string, Vector3[]>> PlacementListCreate(int[] wetAreasKinds) {
        var result = new List<Dictionary<string, Vector3[]>>();

        //水回りパーツの組み合わせ
        List<int[]> wetAreasAllPermutation = AllPermutation(wetAreasKinds);   
        for (int i = 0; i < wetAreasAllPermutation.Count; i++) {
            //Debug.Log("i: " + i);
            //回転のパターン
            List<int[]> rotationAllPattern = flagPatternList(wetAreasAllPermutation[i].Length);
            for (int j = 0; j < rotationAllPattern.Count; j++) {
                //Debug.Log("j: " + j);

                /* 洋室あり */ 
                List<Dictionary<string, Vector3[]>> wetAreasResult = placement(wetAreasAllPermutation[i], rotationAllPattern[j]);
                var westernResult = new List<Dictionary<string, Vector3[]>>();
                for (int k = 0; k < wetAreasResult.Count; k++) {
                    //Debug.Log("k: " + k);

                    westernResult.AddRange(CreateWestern(wetAreasResult[k]));
                }

                result.AddRange(westernResult);
                
                /* 洋室なし */
                //result.AddRange(placement(wetAreasAllPermutation[i], rotationAllPattern[j]));
            }
        }

        return result;
    }

    //リストの作成(水回りパーツの組み合わせ, 回転のパターンをひとつ選ぶ)
    public List<Dictionary<string, Vector3[]>> PlacementListCreate(int[] wetAreasKinds, int wetAreasAllPermutationIndex, int rotationAllPatternIndex) {
        var result = new List<Dictionary<string, Vector3[]>>();

        List<int[]> wetAreasAllPermutation = AllPermutation(wetAreasKinds);
        List<int[]> rotationAllPattern = flagPatternList(wetAreasAllPermutation[wetAreasAllPermutationIndex].Length);
        
        /* 洋室あり */
        /*
        List<Dictionary<string, Vector3[]>> wetAreasResult = placement(wetAreasAllPermutation[wetAreasAllPermutationIndex], rotationAllPattern[rotationAllPatternIndex]);
        var westernResult = new List<Dictionary<string, Vector3[]>>();
        for (int i = 0; i < wetAreasResult.Count; i++) {
            //Debug.Log("i: " + i);
            westernResult.AddRange(CreateWestern(wetAreasResult[i]));
        }

        result.AddRange(westernResult);
        */
        
        /* 洋室なし */
        result.AddRange(placement(wetAreasAllPermutation[wetAreasAllPermutationIndex], rotationAllPattern[rotationAllPatternIndex]));

        return result;
    }

    //部屋パーツ配置
    public List<Dictionary<string, Vector3[]>> placement(int[] wetAreasPermutation, int[] rotationPattern) {
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
                            current_side = SideSubstraction(current_side, contact_coordinates);
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
                    if (result[l].SequenceEqual(placementResult)) {
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
                    for (int l = 0; l < result.Count; l++) {
                        if (result[l].SequenceEqual(placementResult)) {
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
                            current_side = SideSubstraction(current_side, contact_coordinates);
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
                    if (result[l].SequenceEqual(placementResult)) {
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
                        if (result[l].SequenceEqual(placementResult)) {
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
                                    if (!JudgeOutside(CorrectCoordinates(current_room, new Vector3(gap_x, gap_y, 0)), value)) {
                                        outside_flag = false;
                                    }
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
                else if (current_side[0].y == current_side[0].y) {
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
                                    if (!JudgeOutside(CorrectCoordinates(current_room, new Vector3(gap_x, gap_y, 0)), value)) {
                                        outside_flag = false;
                                    }
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
                current_side = SideSubstraction(current_side, contact_coordinates);
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

    /***

    洋室の形を決定

    ***/
    public  List<Dictionary<string, Vector3[]>> CreateWestern(Dictionary<string, Vector3[]> dictionary) {
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

                    /* 洋室の面積が妥当なものを追加 */
                    //洋室の面積
                    float westernArea = areaCalculation(rangeAddedIntersection);

                    //洋室の面積が一定より大きくなったら終了
                    if (LivingRoomRange(areaCalculation(dwelling), "1K")[1] < westernArea) {
                        break;
                    }

                    //洋室の面積が妥当なものを追加
                    if (LivingRoomRange(areaCalculation(dwelling), "1K")[0] <= westernArea && westernArea <= LivingRoomRange(areaCalculation(dwelling), "1K")[1]) {
                        western.Add(rangeAddedIntersection);
                    }
                }
            }
            else {
                limit = min;
            }

        }
        //基準となる辺がx軸平行の場合
        else if (westernStandardside[0][0].y == westernStandardside[0][1].y) {

        }

        //洋室を配置したリストを作成
        for (int i = 0; i < western.Count; i++) {
            var currentWetAreasPattern = new Dictionary<string, Vector3[]>(dictionary);
            currentWetAreasPattern.Add("Western", western[i]);
            result.Add(currentWetAreasPattern);
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
        var hallwayLengthRatioList = new Dictionary<int, float>(); //全パターンのリストと対応付けるためのインデックスと廊下の長さの割合の辞書

        /* 評価指標のリストを作成 */
        //全パターンについてひとつずつ評価
        for (int i = 0; i < allPattern.Count; i++) {

            //廊下の座標
            Vector3[] hallway = range;

            foreach (string roomName in allPattern[i].Keys) {
                /* 洋室の大きさの割合を算出 */
                if (roomName == "Western") {
                    //洋室の面積
                    float westernSize = areaCalculation(allPattern[i][roomName]);
                    //洋室の面積の割合
                    float westernSizeRatio = westernSize / areaCalculation(dwelling);
                    //洋室の面積の割合のリストに追加
                    westernSizeRatioList.Add(i, westernSizeRatio);
                }

                //廊下の座標を作成
                hallway = FrameChange(hallway, allPattern[i][roomName]);
            }

            /* 廊下の長さの割合を算出 */
            //廊下の長さ
            float hallwayLength = 0.0f;
            //住戸の長さ
            float dwellingLength = 0.0f;

            //バルコニーと住戸が接する辺を求める
            bool parallelX = false;
            bool parallelY = false;
            for (int j = 0; j < balcony.Length; j++) {
                var balconyWesternContact = contact(new Vector3[]{balcony[j], balcony[(j+1)%balcony.Length]}, range);
                    if (!ZeroJudge(balconyWesternContact)) {
                        //接する辺がx軸平行の場合
                        if (balconyWesternContact[0].y == balconyWesternContact[1].y) {
                            parallelX = true;
                        }
                        //接する辺がy軸平行の場合
                        else if (balconyWesternContact[0].x == balconyWesternContact[1].x) {
                            parallelY = true;
                        }
                    }
            }

            //x軸平行の場合
            if (parallelX) {
                //住戸の座標の内，x座標が最大・最小のものを求める
                float dwellingMax = dwelling[0].x;
                float dwellingMin = dwelling[0].x;
                for (int j = 1; j < dwelling.Length; j++) {
                    if (dwellingMax < dwelling[j].x) {
                        dwellingMax = dwelling[j].x;
                    }

                    if (dwelling[j].x < dwellingMin) {
                        dwellingMin = dwelling[j].x;
                    }
                }

                //住戸の長さを算出
                dwellingLength = dwellingMax - dwellingMin;

                //廊下の座標の内，x座標が最大・最小のものを求める
                float hallwayMax = hallway[0].x;
                float hallwayMin = hallway[0].x;
                for (int j = 1; j < hallway.Length; j++) {
                    if (hallwayMax < hallway[j].x) {
                        hallwayMax = hallway[j].x;
                    }

                    if (hallway[j].x < hallwayMin) {
                        hallwayMin = hallway[j].x;
                    }
                }

                //廊下の長さを算出
                hallwayLength = hallwayMax - hallwayMin;
            }
            else if (parallelY) {
                //住戸の座標の内，y座標が最大・最小のものを求める
                float dwellingMax = dwelling[0].y;
                float dwellingMin = dwelling[0].y;
                for (int j = 1; j < dwelling.Length; j++) {
                    if (dwellingMax < dwelling[j].y) {
                        dwellingMax = dwelling[j].y;
                    }

                    if (dwelling[j].y < dwellingMin) {
                        dwellingMin = dwelling[j].y;
                    }
                }

                //住戸の長さを算出
                dwellingLength = dwellingMax - dwellingMin;

                //廊下の座標の内，y座標が最大・最小のものを求める
                float hallwayMax = hallway[0].y;
                float hallwayMin = hallway[0].y;
                for (int j = 1; j < hallway.Length; j++) {
                    if (hallwayMax < hallway[j].y) {
                        hallwayMax = hallway[j].y;
                    }

                    if (hallway[j].y < hallwayMin) {
                        hallwayMin = hallway[j].y;
                    }
                }

                //廊下の長さを算出
                hallwayLength = hallwayMax - hallwayMin;

            }

            //廊下の長さの割合
            float hallwayLengthRatio = hallwayLength / dwellingLength;
            //廊下の長さの割合のリストに追加
            hallwayLengthRatioList.Add(i, hallwayLengthRatio);
        }

        //評価指標の辞書を評価指標の降順にソート
        var westernSizeRatioListSort = westernSizeRatioList.OrderBy((x) => x.Value);
        var hallwayLengthRatioListSort = hallwayLengthRatioList.OrderBy((x) => x.Value);

        // ↓↓↓ここから先の手法は要検討

        //評価指標の辞書のインデックスに合わせて全パターンのリストをソート
        var allPatternSort = new List<Dictionary<string, Vector3[]>>();
        foreach (KeyValuePair<int, float> kvp in westernSizeRatioListSort) {
            allPatternSort.Add(allPattern[kvp.Key]);
        }
        

        /* 評価指標のリストをもとにパターンを絞る */
        //全パターンのリストのうち，前から30個を選択
        for (int i = 0; i < 30; i++) {
            selectedPattern.Add(allPatternSort[i]);
        }

        return selectedPattern;
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
    /// 住戸の面積・タイプに対して，洋室の面積の範囲を返す
    /// </summary> 
    /// <param name="area">住戸面積</param>
    /// <param name="type">住戸タイプ</param>
    /// <returns>最小値と最大値が入った配列を返す</returns>
    public float[] LivingRoomRange(float area, string type) {
        float[] result = new float[2];

        if (type == "1K") {
            if ((27.00f <= area) && (area < 28.00f)) {
                result[0] = 11.00f;
                result[1] = 12.00f;
            }
            else if ((28.00f <= area) && (area < 29.00f)) {
                result[0] = 12.00f;
                result[1] = 14.00f;
            }
            else if ((29.00f <= area) && (area < 30.00f)) {
                result[0] = 14.00f;
                result[1] = 15.50f;
            }
            else if ((30.00f <= area) && (area < 31.00f)) {
                result[0] = 15.00f;
                result[1] = 17.00f;
            }
        }

        return result;
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
        int outside_end_index = newOuter.IndexOf(end_coordinates);
        newOuter.InsertRange(outside_end_index, needInside);

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

    /***

    座標を全て同じだけずらす

    ***/
    public Vector3[] CorrectCoordinates(Vector3[] room, Vector3 correctValue) {
        Vector3[] correctedCoordinates = new Vector3[room.Length];

        for (int i = 0; i < room.Length; i++) {
            correctedCoordinates[i] = new Vector3(room[i].x + correctValue.x, room[i].y + correctValue.y, 0);
        }

        return correctedCoordinates;
    }

    /***

    部屋と辺の距離の配列

    ***/
    public float[] ContactGap(Vector3[] room, Vector3[] side) {
        List<float> result = new List<float>();

        for (int i = 0; i < room.Length; i++) {
            if ((positionalRelation(new Vector3[]{room[i], room[(i+1)%room.Length]}, side)[2] == 1.00f) || (positionalRelation(new Vector3[]{room[i], room[(i+1)%room.Length]}, side)[2] == 2.00f)) {
                result.Add(positionalRelation(new Vector3[]{room[i], room[(i+1)%room.Length]}, side)[1] - positionalRelation(new Vector3[]{room[i], room[(i+1)%room.Length]}, side)[0]);
            }
        }

        return result.ToArray();
    }

    /***

    直線と直線が平行かどうかを確認し，
    平行な直線どうしの切片を返すメソッド

    ***/
    public float[] positionalRelation(Vector3[] sideA, Vector3[] sideB) {
        float m1, m2, n1, n2;

        //y軸平行の時で直線と部屋が平行
        if (sideA[0].x == sideA[1].x) {
            n1 = sideA[0].x;
            if (sideB[0].x == sideB[1].x) {
                n2 = sideB[0].x;

                return new float[] {n1, n2, 1.00f};
            }
        }
        //それ以外で直線と部屋が平行
        else {
            m1 = (sideA[1].y - sideA[0].y) / (sideA[1].x - sideA[0].x);
            n1 = (sideA[1].x * sideA[0].y - sideA[0].x * sideA[1].y) / (sideA[1].x - sideA[0].x);
            if (sideB[0].x != sideB[1].x) {
                m2 = (sideB[1].y - sideB[0].y) / (sideB[1].x - sideB[0].x);
                n2 = (sideB[1].x * sideB[0].y - sideB[0].x * sideB[1].y) / (sideB[1].x - sideB[0].x);
                if (m1 == m2) {

                    return new float[] {n1, n2, 2.00f};
                }
            }     
        }

        //直線と部屋が平行でない
        return new float[] {0.00f, 0.00f, 0.00f};
    }

    /***

    重なった2本の線分から重なっていない座標を求める

    ***/
    public Vector3[] SideSubstraction(Vector3[] sideA, Vector3[] sideB) {
        Vector3[] result = sideA;

        if (sideA[0] == sideB[0]) {
            result = new Vector3[] {sideA[1], sideB[1]};
        }
        else if (sideA[0] == sideB[1]) {
            result = new Vector3[] {sideA[1], sideB[0]};
        }
        else if (sideA[1] == sideB[0]) {
            result = new Vector3[] {sideA[0], sideB[1]};
        }
        else if (sideA[1] == sideB[1]) {
            result = new Vector3[] {sideA[0], sideB[0]};
        }

        return result;
    }

    /***

    座標を原点周りに90°回転させて返す

    ***/
    public Vector3[] Rotation(Vector3[] room) {
        Vector3[] rotatedCoordinates = new Vector3[room.Length];
        for (int i = 0; i < room.Length; i++) {
            rotatedCoordinates[i].x = - room[(i+1)%room.Length].y;
            rotatedCoordinates[i].y = room[(i+1)%room.Length].x;
        }

        return rotatedCoordinates;
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
    /// 多角形が別の多角形の外部（辺上も可）にあるかどうかの判定
    /// </summary> 
    /// <param name="polygonA">多角形Aの座標配列</param>
    /// <param name="polygonB">多角形Bの座標配列</param>
    /// <returns>多角形が別の多角形の外部にある場合True，ない場合Flase</returns>
    public bool JudgeOutside(Vector3[] polygonA, Vector3[] polygonB) {
        bool flag = false;
        //点がいくつ外側にあるかを数える
        int outsideCounter = 0;

        //多角形Bの頂点を全て調べる
        for (int i = 0; i < polygonB.Length; i++) {
            //まず辺上にあるかどうかを調べる
            bool onLineFlag = false;
            for (int j = 0; j < polygonA.Length; j++) {
                if (OnLineSegment(new Vector3[]{polygonA[j], polygonA[(j+1)%polygonA.Length]}, new Vector3(polygonB[i].x, polygonB[i].y, 0))) {
                    outsideCounter++;
                    onLineFlag = true;
                    break;
                }
            }

            //辺上にある場合は次の頂点へ
            if (onLineFlag) {
                continue;
            }

            //次に，外部にあるかどうかを調べる
            if (!CheckPoint(polygonA, new Vector3(polygonB[i].x, polygonB[i].y, 0))) {
                outsideCounter++;
            }
        }
        
        //外側にある点の数が多角形Bの頂点の数と同じ場合，外側にあると判定
        if (outsideCounter == polygonB.Length) {
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

    /***

    多角形を辺上の2点を通るように切り取った座標×2

    ***/
    /*
    public List<Vector3[]> FrameSlice(Vector3[] room, Vector3[] pointsA, Vector3[] pointsB) {
        //返すリスト
        List<Vector3[]> sliceRooms = new List<Vector3[]>();

        //全ての点を加えた配列を作成
        Vector3[] pointAddRoom = room;
        pointAddRoom = AddPoint(pointAddRoom, pointsA[0]);
        pointAddRoom = AddPoint(pointAddRoom, pointsA[1]);

        pointAddRoom = AddPoint(pointAddRoom, pointsB[0]);
        pointAddRoom = AddPoint(pointAddRoom, pointsB[1]);

        //PointsAで切り取る
        List<Vector3> sliceRoomA = pointAddRoom.ToList();

        int A0_index =  sliceRoomA.IndexOf(pointsA[0]);
        int A1_index =  sliceRoomA.IndexOf(pointsA[1]);

        if (A0_index < A1_index) {
            bool insideFlag = false;
            for (int i = A0_index + 1; i < A1_index; i++) {
                if ((sliceRoomA[i] == pointsB[0]) || (sliceRoomA[i] == pointsB[0])) {
                    insideFlag = true;
                }
            }

            //想定している入力じゃない時にエラー起きそう
            if (insideFlag) {
                sliceRoomA.RemoveRange(A0_index + 1, A1_index - A0_index - 1);
            }
            else {
                sliceRoomA.RemoveRange(A1_index + 1, sliceRoomA.Count - A1_index - 1);
                sliceRoomA.RemoveRange(0, A0_index);
            }
        } 
        else {
            bool insideFlag = false;
            for (int i = A1_index + 1; i < A0_index; i++) {
                if ((sliceRoomA[i] == pointsB[0]) || (sliceRoomA[i] == pointsB[0])) {
                    insideFlag = true;
                }
            }

            //想定している入力じゃない時にエラー起きそう
            if (insideFlag) {
                sliceRoomA.RemoveRange(A1_index + 1, A0_index - A1_index - 1);
            }
            else {
                sliceRoomA.RemoveRange(A0_index + 1, sliceRoomA.Count - A0_index - 1);
                sliceRoomA.RemoveRange(0, A1_index);
            }
        }

        //PointsBで切り取る
        List<Vector3> sliceRoomB = pointAddRoom.ToList();

        int B0_index =  sliceRoomB.IndexOf(pointsB[0]);
        int B1_index =  sliceRoomB.IndexOf(pointsB[1]);

        if (B0_index < B1_index) {
            bool insideFlag = false;
            for (int i = B0_index + 1; i < B1_index; i++) {
                if ((sliceRoomB[i] == pointsA[0]) || (sliceRoomB[i] == pointsA[0])) {
                    insideFlag = true;
                }
            }

            //想定している入力じゃない時にエラー起きそう
            if (insideFlag) {
                sliceRoomB.RemoveRange(B0_index + 1, B1_index - B0_index - 1);
            }
            else {
                sliceRoomB.RemoveRange(B1_index + 1, sliceRoomB.Count - B1_index - 1);
                sliceRoomB.RemoveRange(0, B0_index);
            }
        } 
        else {
            bool insideFlag = false;
            for (int i = B1_index + 1; i < B0_index; i++) {
                if ((sliceRoomB[i] == pointsA[0]) || (sliceRoomB[i] == pointsA[0])) {
                    insideFlag = true;
                }
            }

            //想定している入力じゃない時にエラー起きそう
            if (insideFlag) {
                sliceRoomB.RemoveRange(B1_index + 1, B0_index - B1_index - 1);
            }
            else {
                sliceRoomB.RemoveRange(B0_index + 1, sliceRoomB.Count - B0_index - 1);
                sliceRoomB.RemoveRange(0, B1_index);
            }
        }

        //戻り値を作成
        sliceRooms.Add(sliceRoomA.ToArray());
        sliceRooms.Add(sliceRoomB.ToArray());

        return sliceRooms;
    }
    */
}