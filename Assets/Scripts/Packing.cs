using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Packing : CreateRoom
{
    [SerializeField] Parts pa;
    Vector3[] range;
    Vector3[] entrance;

    void Start()
    {
        range = new Vector3[]{new Vector3(-2050, 1850, 0), new Vector3(2050, 1850, 0), new Vector3(2050, -50, 0), new Vector3(1050, -50, 0), new Vector3(1050, -1850, 0), new Vector3(-2050, -1850, 0)};
        createRoom("range", range);

        //entrance = new Vector3[]{new Vector3(1050, -50, 0), new Vector3(2050, -50, 0), new Vector3(2050, -1550, 0), new Vector3(1050, -1550, 0)};
        //createRoom("entrance", entrance);

        //Vector3[] mbps = new Vector3[]{new Vector3(1050, -1550, 0), new Vector3(2050, -1550, 0), new Vector3(2050, -1850, 0), new Vector3(1050, -1850, 0)};
        //createRoom("mbps", mbps);

        //placement();
        
        /*
        //FrameSliceテスト
        List<Vector3[]> test = FrameSlice(range, new Vector3[]{new Vector3(-2050, 1850, 0), new Vector3(1050, -50, 0)}, new Vector3[]{new Vector3(-2050, -1850, 0), new Vector3(1050, -1550, 0)});
        
        Debug.Log("A");
        for (int i = 0; i < test[0].Length; i++) {
            Debug.Log(test[0][i]);
        }
        Debug.Log("B");
        for (int i = 0; i < test[1].Length; i++) {
            Debug.Log(test[1][i]);
        }
        */
        
        /*
        //PointAddテスト
        Vector3 testPoint = new Vector3(1050, -1550, 0);
        for (int i = 0; i < PointAdd(range, testPoint).Length; i++) {
            Debug.Log(PointAdd(range, testPoint)[i]);
        }
        */
        
        /*
        //FrameChangeテスト
        Vector3[] test = new Vector3[]{new Vector3(-2050, 1850, 0), new Vector3(-500, 1850, 0), new Vector3(-500, 1350, 0), new Vector3(-1000, 1350, 0), new Vector3(-1000, 1000, 0), new Vector3(-2050, 1000, 0)};
        createRoom("test", test);
        for (int i = 0; i < FrameChange(range, test).Length; i++) {
            Debug.Log(FrameChange(range, test)[i]);
        }
        */

        /*
        //AllPermutationテスト
        int[] roomsIndex = new int[]{1, 2, 3, 4, 5};

        Debug.Log("パターン数:" + AllPermutation(roomsIndex).Count);
        for (int i = 0; i < AllPermutation(roomsIndex).Count; i++) {
            Debug.Log("---");
            for (int j = 0; j < AllPermutation(roomsIndex)[i].Length; j++) {
                Debug.Log(AllPermutation(roomsIndex)[i][j]);
            }
        }
        Debug.Log("---");
        */
    }

    //部屋パーツ配置
    public void placement() {
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

        for (int i = 0; i < pointAddRoom.Length; i++) {
            Debug.Log(pointAddRoom[i]);
        }

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
        //Debug.Log(B0_index);
        Debug.Log(B1_index);

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
        Vector3[] zero = new Vector3[] {Vector3.zero, Vector3.zero};
        for (int i = 0; i < outer.Length; i++) {
            Vector3[] contact_coordinates = contact(new Vector3[]{outer[i], outer[(i+1)%outer.Length]}, inside);

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
}
