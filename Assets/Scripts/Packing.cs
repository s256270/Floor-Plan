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

        /*
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

        //placement();

        Vector3[] test = new Vector3[]{new Vector3(-2050, 1850, 0), new Vector3(-500, 1850, 0), new Vector3(-500, 1350, 0), new Vector3(-1000, 1350, 0), new Vector3(-1000, 1000, 0), new Vector3(-2050, 1000, 0)};
        createRoom("test", test);
        for (int i = 0; i < FrameChanger(range, test).Length; i++) {
            Debug.Log(FrameChanger(range, test)[i]);
        }
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

    外側の部屋の座標から接している内側の部屋を抜き取った座標

    ***/
    public Vector3[] FrameChanger(Vector3[] outer, Vector3[] inside) {
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
            needInside.RemoveRange(0, inside_end_index + 1);
            inside_start_index = needInside.IndexOf(start_coordinates);
            needInside.RemoveRange(inside_start_index, needInside.Count - inside_start_index);
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
