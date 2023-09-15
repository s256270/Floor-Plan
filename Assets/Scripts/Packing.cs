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
        
        
        //リストの作成
        allPattern = PlacementListCreate(new int[]{0, 1, 2, 3});

        //初めの方だけリストの作成
        //allPattern = PlacementListCreate(new int[]{0, 1, 2, 3}, 0, 0);
        
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
            //回転のパターン
            List<int[]> rotationAllPattern = flagPatternList(wetAreasAllPermutation[i].Length);
            for (int j = 0; j < rotationAllPattern.Count; j++) {
                result.AddRange(placement(wetAreasAllPermutation[i], rotationAllPattern[j]));
            }
        }

        return result;
    }

    //リストの作成(水回りパーツの組み合わせ, 回転のパターンをひとつ選ぶ)
    public List<Dictionary<string, Vector3[]>> PlacementListCreate(int[] wetAreasKinds, int wetAreasAllPermutationIndex, int rotationAllPatternIndex) {
        var result = new List<Dictionary<string, Vector3[]>>();

        List<int[]> wetAreasAllPermutation = AllPermutation(wetAreasKinds);
        List<int[]> rotationAllPattern = flagPatternList(wetAreasAllPermutation[wetAreasAllPermutationIndex].Length);
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

        if (!CrossJudge(entrance_side[entranceSideIndex][0], western_side[westernSideIndex][0], entrance_side[entranceSideIndex][1], western_side[westernSideIndex][1])) {
            hallway_side.Add(new Vector3[]{entrance_side[entranceSideIndex][0], western_side[westernSideIndex][0]});
            hallway_side.Add(new Vector3[]{entrance_side[entranceSideIndex][1], western_side[westernSideIndex][1]});
        }
        else {
            hallway_side.Add(new Vector3[]{entrance_side[entranceSideIndex][0], western_side[westernSideIndex][1]});
            hallway_side.Add(new Vector3[]{entrance_side[entranceSideIndex][1], western_side[westernSideIndex][0]});
        }

        //辺に従って領域を切り取り
        List<Vector3[]> wetAreas = FrameSlice(range, hallway_side[0], hallway_side[1]);

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
                        if (JudgeInscribed(range, CorrectCoordinates(current_room, new Vector3(ContactGap(current_room, current_side)[k], gap_y, 0)))) {
                            gap_x = ContactGap(current_room, current_side)[k];

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
                else if (current_side[0].y == current_side[0].y) {
                    float max = current_room[0].x;
                    for (int k = 1; k < current_room.Length; k++) {
                        if (max < current_room[k].x) {
                            max = current_room[k].x;
                        }
                    }

                    gap_x = Mathf.Max(current_side[0].x, current_side[1].x) - max;

                    for (int k = 0; k < ContactGap(current_room, current_side).Length; k++) {
                        if (JudgeInscribed(range, CorrectCoordinates(current_room, new Vector3(gap_x, ContactGap(current_room, current_side)[k], 0)))) {
                            gap_y = ContactGap(current_room, current_side)[k];

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

    /***

    多角形を辺上の2点を通るように切り取った座標×2

    ***/
    public List<Vector3[]> FrameSlice(Vector3[] room, Vector3[] pointsA, Vector3[] pointsB) {
        //返すリスト
        List<Vector3[]> sliceRooms = new List<Vector3[]>();

        //全ての点を加えた配列を作成
        Vector3[] pointAddRoom = room;
        pointAddRoom = PointAdd(pointAddRoom, pointsA[0]);
        pointAddRoom = PointAdd(pointAddRoom, pointsA[1]);

        pointAddRoom = PointAdd(pointAddRoom, pointsB[0]);
        pointAddRoom = PointAdd(pointAddRoom, pointsB[1]);

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

    /***

    多角形の辺上の点を座標配列に追加

    ***/
    public Vector3[] PointAdd(Vector3[] room, Vector3 point) {
        //返すリスト
        List<Vector3> addedRoom = room.ToList();

        if (room.Contains(point)) {
            return addedRoom.ToArray();
        }

        for (int i = 0; i < room.Length; i++) {
            if ((Mathf.Min(room[i].x, room[(i+1)%room.Length].x) <= point.x) && (point.x <= Mathf.Max(room[i].x, room[(i+1)%room.Length].x))) {
                if ((Mathf.Min(room[i].y, room[(i+1)%room.Length].y) <= point.y) && (point.y <= Mathf.Max(room[i].y, room[(i+1)%room.Length].y))) {
                    if (Vector3.Distance(room[i], point) + Vector3.Distance(point, room[(i+1)%room.Length]) == Vector3.Distance(room[i], room[(i+1)%room.Length])) {
                        addedRoom.Insert(i+1, point);
    
                        return addedRoom.ToArray();
                    }
                }
            }
        }

        return addedRoom.ToArray();
    }

    /***

    外側の部屋の座標から接している内側の部屋を抜き取った座標

    ***/
    public Vector3[] FrameChange(Vector3[] outer, Vector3[] inside) {
        List<Vector3> newOuter = new List<Vector3>(); //返すリスト
        Vector3 start_coordinates = new Vector3();
        bool start_flag = true;
        Vector3 end_coordinates = new Vector3();
        bool end_flag = true;

        //外側の部屋の必要な座標を追加
        //外側の部屋の辺をひとつずつ確認
        for (int i = 0; i < outer.Length; i++) {
            Vector3[] contact_coordinates = contact(new Vector3[]{outer[i], outer[(i+1)%outer.Length]}, inside);

            //内側の部屋と接している場合
            if (!ZeroJudge(contact_coordinates)) {
                //外側の辺と内側の辺の向き(リスト内の座標の順番)が逆の場合
                if (Vector3.Dot(outer[(i+1)%outer.Length] - outer[i], contact_coordinates[1] - contact_coordinates[0]) < 0) {
                    //内側の辺の向きを逆にする
                    Array.Reverse(contact_coordinates);
                }
                //外側と内側の辺のリスト上で先に来る座標についての処理
                //外側の辺と内側の辺の頂点が同じでない場合
                if (outer[i] != contact_coordinates[0]) {
                    //外側の辺の頂点を追加
                    newOuter.Add(outer[i]);

                    //内側の辺の頂点を追加
                    newOuter.Add(contact_coordinates[0]);
                    //切れ目の始点に設定
                    if (start_flag) {
                        start_coordinates = contact_coordinates[0];
                        start_flag = false;
                    }
                }
                //外側の辺と内側の辺の頂点が同じ場合
                else {
                    if (start_flag) {
                        //外側の辺の頂点を追加
                        newOuter.Add(outer[i]);

                        //切れ目の始点に設定
                        start_coordinates = outer[i];
                        start_flag = false;
                    }
                }

                //外側と内側の辺のリスト上で後に来る座標についての処理
                //外側の辺と内側の辺の頂点が同じでない場合
                if (outer[(i+1)%outer.Length] != contact_coordinates[1]) {
                    //内側の辺の頂点を追加
                    newOuter.Add(contact_coordinates[1]);
                    //切れ目の終点に設定
                    if (end_flag) {
                        end_coordinates = contact_coordinates[1];
                        end_flag = false;
                    }
                }
                //外側の辺と内側の辺の頂点が同じ場合
                else {
                    //切れ目の終点に設定
                    if (end_flag) {
                        end_coordinates = outer[(i+1)%outer.Length];
                    }
                }
            }
            //内側の部屋と接していない場合
            else {
                newOuter.Add(outer[i]);
            }
        }

        //内側の部屋の必要な座標を追加
        List<Vector3> needInside = inside.ToList();
        
        int inside_start_index = needInside.IndexOf(start_coordinates);
        int inside_end_index = needInside.IndexOf(end_coordinates);

        if (inside_end_index.CompareTo(inside_start_index) == 1) {
            needInside.RemoveRange(inside_start_index, inside_end_index - inside_start_index + 1);
        } 
        else {
            needInside.RemoveRange(inside_start_index, needInside.Count - inside_start_index);
            needInside.RemoveRange(0, inside_end_index + 1);
        }
        if (!((needInside.Count == 2) && ((Array.IndexOf(inside, needInside[0]) == 0) && (Array.IndexOf(inside, needInside[1]) == inside.Length - 1)))) {
            needInside.Reverse();
        }

        int outer_end_index = newOuter.IndexOf(end_coordinates);
        newOuter.InsertRange(outer_end_index, needInside);

        return newOuter.ToArray();
    }

    /***

    2本の線分の交差判定

    ***/
    public bool CrossJudge(Vector3 pointA, Vector3 pointB, Vector3 pointC, Vector3 pointD)
    {
        double s, t;

        s = (pointA.x - pointB.x) * (pointC.y - pointA.y) - (pointA.y - pointB.y) * (pointC.x - pointA.x);
        t = (pointA.x - pointB.x) * (pointD.y - pointA.y) - (pointA.y - pointB.y) * (pointD.x - pointA.x);
        if (s * t > 0) {
            return false;
        }

        s = (pointC.x - pointD.x) * (pointA.y - pointC.y) - (pointC.y - pointD.y) * (pointA.x - pointC.x);
        t = (pointC.x - pointD.x) * (pointB.y - pointC.y) - (pointC.y - pointD.y) * (pointB.x - pointC.x);
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

    多角形が別の多角形の内部（辺上も可）にあるかどうか

    ***/
    public bool JudgeInscribed(Vector3[] outer, Vector3[] inner) {
        bool flag = false;
        int trueCounter = 0;

        for (int i = 0; i < inner.Length; i++) {
            if (CheckPoint(outer, new Vector3(inner[i].x, inner[i].y, 0))) {
                trueCounter++;
            }
        }

        if (trueCounter == inner.Length) {
            flag = true;
        }

        return flag;
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
    /// 図形に対する点の内外判定
    /// </summary> 
    /// <param name="points">図形の座標配列</param>
    /// <param name="target">判定する点の座標</param>
    /// <returns>内分の場合True，外分の場合Flase</returns>
    public bool CheckPoint(Vector3[] points, Vector3 target) {
        //まず，点が辺上にあるかどうかを判定する
        for (int i = 0; i < points.Length; i++) {
            if (Vector3.Distance(points[i], target) + Vector3.Distance(target, points[(i+1)%points.Length]) == Vector3.Distance(points[i], points[(i+1)%points.Length])) {
                return true;
            }
        }

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
