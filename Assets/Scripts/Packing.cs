using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Packing : CreateRoom
{
    [SerializeField] Parts pa;
    public Vector3[] range;
    Vector3[] entrance;
    List<Dictionary<string, Vector3[]>> allPattern = new List<Dictionary<string, Vector3[]>>();
    int count = 0;
    int limit = 0;

    void Start()
    {
        range = new Vector3[]{new Vector3(-2050, 1850, 0), new Vector3(2050, 1850, 0), new Vector3(2050, -50, 0), new Vector3(1050, -50, 0), new Vector3(1050, -1850, 0), new Vector3(-2050, -1850, 0)};
        range = new Vector3[]{new Vector3(-2050, 1850, 0), new Vector3(1050, 1850, 0), new Vector3(1050, 350, 0), new Vector3(2050, 350, 0), new Vector3(2050, -1550, 0), new Vector3(1050, -1550, 0), new Vector3(1050, -1850, 0), new Vector3(-2050, -1850, 0)};
        //range = new Vector3[]{new Vector3(-2050, 1850, 0), new Vector3(2050, 1850, 0), new Vector3(2050, 750, 0), new Vector3(1050, 750, 0), new Vector3(1050, -750, 0), new Vector3(2050, -750, 0), new Vector3(2050, -1550, 0), new Vector3(1050, -1550, 0), new Vector3(1050, -1850, 0), new Vector3(-2050, -1850, 0)};
        createRoom("range", range);

        entrance = new Vector3[]{new Vector3(1050, -50, 0), new Vector3(2050, -50, 0), new Vector3(2050, -1550, 0), new Vector3(1050, -1550, 0)};
        entrance = CorrectCoordinates(entrance, new Vector3(0, 1900, 0));
        //entrance = CorrectCoordinates(entrance, new Vector3(0, -1100, 0));

        createRoom("entrance", entrance);
        
        for (int i = 0; i < 16; i++) {
            for (int j = 0; j < 24; j++) {
                allPattern.Add(placement(j, i));
            }
        }

        limit = allPattern.Count;

        /*
        Dictionary<string, Vector3[]> wetAreaRooms = placement(0, 0);
        foreach (string roomName in wetAreaRooms.Keys) {
            createRoom(roomName, wetAreaRooms[roomName]);
        }
        */
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

    //部屋パーツ配置
    public Dictionary<string, Vector3[]> placement(int wetAreasPatternIndex, int rotationPatternIndex) {
        var result = new Dictionary<string, Vector3[]>();

        //玄関の廊下に続く辺を決める
        List<Vector3[]> entrance_side = new List<Vector3[]>();
        Vector3[] zero = new Vector3[] {Vector3.zero, Vector3.zero};
        for (int i = 0; i < entrance.Length; i++) {
            Vector3[] entrance_contact = contact(new Vector3[]{entrance[i], entrance[(i+1)%entrance.Length]}, range);

            if ((entrance_contact[0] != zero[0]) && (entrance_contact[1] != zero[1])) {
                entrance_side.Add(entrance_contact);
            }
        }

        //洋室の廊下に続く辺を決める
        Vector3[] western_side = new Vector3[]{new Vector3(-2050, -1850, 0), new Vector3(-2050, 1850, 0)};

        //玄関から洋室へつながる辺の決定
        List<Vector3[]> hallway_side = new List<Vector3[]>();

        int entranceSideIndex = 1;

        if (!CrossJudge(entrance_side[entranceSideIndex][0], western_side[0], entrance_side[entranceSideIndex][1], western_side[1])) {
            hallway_side.Add(new Vector3[]{entrance_side[entranceSideIndex][0], western_side[0]});
            hallway_side.Add(new Vector3[]{entrance_side[entranceSideIndex][1], western_side[1]});
        }
        else {
            hallway_side.Add(new Vector3[]{entrance_side[entranceSideIndex][0], western_side[1]});
            hallway_side.Add(new Vector3[]{entrance_side[entranceSideIndex][1], western_side[0]});
        }

        //辺に従って領域を切り取り
        List<Vector3[]> wetAreas = FrameSlice(range, hallway_side[0], hallway_side[1]);

        //面積の大きい方を求める
        if (areaCalculation(wetAreas[0]) < areaCalculation(wetAreas[1])) {
            wetAreas.Reverse();
            hallway_side.Reverse();
        }
        
        //水回りパーツの組み合わせ
        int[] wetAreasIndex = new int[]{0, 1, 2, 3};
        List<int[]> wetAreasAllPermutation = AllPermutation(wetAreasIndex);

        //これから配置するパターン
        List<int> wetAreasPermutation = wetAreasAllPermutation[wetAreasPatternIndex].ToList();

        //長い辺から順に並べていく
        int[] longIndex1 = LongIndex(wetAreas[0]);
        for (int i = 0; i < longIndex1.Length; i++) {
            if (result.ContainsKey("UB") && result.ContainsKey("Washroom") && result.ContainsKey("Toilet") && result.ContainsKey("Kitchen")) {
                break;
            }

            Vector3[] current_side = new Vector3[] {wetAreas[0][longIndex1[i]], wetAreas[0][(longIndex1[i]+1)%longIndex1.Length]};
            int[] rotationPattern = flagPatternList(wetAreasIndex.Length)[rotationPatternIndex];

            if ((current_side[0] == hallway_side[0][0] && current_side[1] == hallway_side[0][1]) || (current_side[0] == hallway_side[0][1] && current_side[1] == hallway_side[0][0])) {
                if (longIndex1.Length > 2) {
                    continue;
                }
            }

            for (int j = 0; j < wetAreasPermutation.Count; j++) {
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
                            if (JudgeInscribed(range, CorrectCoordinates(current_room, new Vector3(ContactGap(current_room, current_side)[k], gap_y, 0)))) {
                                gap_x = ContactGap(current_room, current_side)[k];
                                break_flag = false;
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
                                break_flag = false;
                            }
                        }
                    }
                }

                if (break_flag) {
                    break;
                }

                CreateCoordinates = CorrectCoordinates(current_room, new Vector3(gap_x, gap_y, 0));
                result.Add(current_room_name, CreateCoordinates);

                Vector3[] contact_coordinates = contact(current_side, CreateCoordinates);

                if ((contact_coordinates[0] != zero[0]) && (contact_coordinates[1] != zero[1])) {
                    current_side = SideSubstraction(current_side, contact_coordinates);
                }

            }

            break;
        }

        //長い辺から順に並べていく
        int[] longIndex2 = LongIndex(wetAreas[1]);
        for (int i = 0; i < longIndex2.Length; i++) {
            if (result.ContainsKey("UB") && result.ContainsKey("Washroom") && result.ContainsKey("Toilet") && result.ContainsKey("Kitchen")) {
                break;
            }

            Vector3[] current_side = new Vector3[] {wetAreas[1][longIndex2[i]], wetAreas[1][(longIndex2[i]+1)%longIndex2.Length]};
            int[] rotationPattern = flagPatternList(wetAreasIndex.Length)[rotationPatternIndex];

            if ((current_side[0] == hallway_side[1][0] && current_side[1] == hallway_side[1][1]) || (current_side[0] == hallway_side[1][1] && current_side[1] == hallway_side[1][0])) {
                if (longIndex2.Length > 2) {
                    continue;
                }
            }

            for (int j = 0; j < wetAreasPermutation.Count; j++) {
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

                if (current_side[0].x == current_side[1].x) {
                    float max = current_room[0].y;
                    for (int k = 1; k < current_room.Length; k++) {
                        if (max < current_room[k].y) {
                            max = current_room[k].y;
                        }
                    }

                    gap_y = Mathf.Max(current_side[0].y, current_side[1].y) - max;

                    for (int k = 0; k < ContactGap(current_room, current_side).Length; k++) {
                        if (JudgeInscribed(range, CorrectCoordinates(current_room, new Vector3(ContactGap(current_room, current_side)[k], gap_y, 0)))) {
                            gap_x = ContactGap(current_room, current_side)[k];
                            break_flag = false;
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
                            break_flag = false;
                        }
                    }
                }

                if (break_flag) {
                    break;
                }

                CreateCoordinates = CorrectCoordinates(current_room, new Vector3(gap_x, gap_y, 0));
                result.Add(current_room_name, CreateCoordinates);

                Vector3[] contact_coordinates = contact(current_side, CreateCoordinates);

                if ((contact_coordinates[0] != zero[0]) && (contact_coordinates[1] != zero[1])) {
                    current_side = SideSubstraction(current_side, contact_coordinates);
                }
            }

            //break;
        }

        return result;
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
        List<Vector3> newOuter = new List<Vector3>();
        Vector3 start_coordinates = new Vector3();
        Vector3 end_coordinates = new Vector3();

        //List<Vector3[]> contact_coordinates_list = new List<Vector3[]>();
        //外側の部屋の必要な座標      
        for (int i = 0; i < outer.Length; i++) {
            Vector3[] contact_coordinates = contact(new Vector3[]{outer[i], outer[(i+1)%outer.Length]}, inside);
            Vector3[] zero = new Vector3[] {Vector3.zero, Vector3.zero};

            if ((contact_coordinates[0] != zero[0]) && (contact_coordinates[1] != zero[1])) {
                if ((outer[i].x.CompareTo(outer[(i+1)%outer.Length].x) == contact_coordinates[0].x.CompareTo(contact_coordinates[1].x) && outer[i].x.CompareTo(outer[(i+1)%outer.Length].x) != 0) || (outer[i].y.CompareTo(outer[(i+1)%outer.Length].y) == contact_coordinates[0].y.CompareTo(contact_coordinates[1].y) && outer[i].y.CompareTo(outer[(i+1)%outer.Length].y) != 0)) {
                    if (outer[i] != contact_coordinates[0]) {
                        if (!newOuter.Contains(outer[i])) {
                            newOuter.Add(outer[i]);
                        }

                        newOuter.Add(contact_coordinates[0]);
                        start_coordinates = contact_coordinates[0];
                    }
                    if (outer[(i+1)%outer.Length] != contact_coordinates[1]) {
                        newOuter.Add(contact_coordinates[1]);
                        end_coordinates = contact_coordinates[1];

                        if (!newOuter.Contains(outer[(i+1)%outer.Length])) {
                            newOuter.Add(outer[(i+1)%outer.Length]);
                        }
                    }
                } else {
                    if (outer[i] != contact_coordinates[1]) {
                        if (!newOuter.Contains(outer[i])) {
                            newOuter.Add(outer[i]);
                        }

                        newOuter.Add(contact_coordinates[1]);
                        start_coordinates = contact_coordinates[1];
                    }
                    if (outer[(i+1)%outer.Length] != contact_coordinates[0]) {
                        newOuter.Add(contact_coordinates[0]);
                        end_coordinates = contact_coordinates[0];

                        if (!newOuter.Contains(outer[(i+1)%outer.Length])) {
                            newOuter.Add(outer[(i+1)%outer.Length]);
                        }
                    }
                }
            } else {
                if (!newOuter.Contains(outer[i])) {
                    newOuter.Add(outer[i]);
                }
                if (!newOuter.Contains(outer[(i+1)%outer.Length])) {
                    newOuter.Add(outer[(i+1)%outer.Length]);
                }
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
        needInside.Reverse();

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

        List<float> length = new List<float>();
        for (int i = 0; i < room.Length; i++) {
            length.Add(Vector3.Distance(room[i], room[(i+1)%room.Length]));
        }

        for (int i = 0; i < result.Length; i++) {
            result[i] = length.IndexOf(length.Max());
            length.RemoveAt(length.IndexOf(length.Max()));
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
    図形の内接判定
    ***/
    public bool JudgeInscribed(Vector3[] outer, Vector3[] inner) {
        bool flag = false;

        int count1 = 0;
        for (int i = 0; i < inner.Length; i++) {
            if (CheckPoint(outer, new Vector3(inner[i].x + 1, inner[i].y + 1, 0))) {
                count1++;
            }
        }

        int count2 = 0;
        for (int i = 0; i < inner.Length; i++) {
            if (CheckPoint(outer, new Vector3(inner[i].x + 1, inner[i].y - 1, 0))) {
                count2++;
            }
        }

        int count3 = 0;
        for (int i = 0; i < inner.Length; i++) {
            if (CheckPoint(outer, new Vector3(inner[i].x - 1, inner[i].y + 1, 0))) {
                count3++;
            }
        }

        int count4 = 0;
        for (int i = 0; i < inner.Length; i++) {
            if (CheckPoint(outer, new Vector3(inner[i].x - 1, inner[i].y - 1, 0))) {
                count4++;
            }
        }

        if ((count1 == inner.Length) || (count2 == inner.Length) || (count3 == inner.Length) || (count4 == inner.Length)) {
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
