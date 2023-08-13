using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Packing : CreateRoom
{
    void Start()
    {
        Vector3[] range = new Vector3[]{new Vector3(-2050, 1850, 0), new Vector3(2050, 1850, 0), new Vector3(2050, -50, 0), new Vector3(1050, -50, 0), new Vector3(1050, -1850, 0), new Vector3(-2050, -1850, 0)};
        createRoom("range", range);

        Vector3[] entrance = new Vector3[]{new Vector3(1050, -50, 0), new Vector3(2050, -50, 0), new Vector3(2050, -1550, 0), new Vector3(1050, -1550, 0)};
        createRoom("entrance", entrance);

        Vector3[] mbps = new Vector3[]{new Vector3(1050, -1550, 0), new Vector3(2050, -1550, 0), new Vector3(2050, -1850, 0), new Vector3(1050, -1850, 0)};
        createRoom("mbps", mbps);

        int[] roomsIndex = new int[]{1, 2, 3, 4, 5};

        Debug.Log("パターン数:" + AllPermutation(roomsIndex).Count);
        for (int i = 0; i < AllPermutation(roomsIndex).Count; i++) {
            Debug.Log("---");
            for (int j = 0; j < AllPermutation(roomsIndex)[i].Length; j++) {
                Debug.Log(AllPermutation(roomsIndex)[i][j]);
            }
        }
        Debug.Log("---");
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
