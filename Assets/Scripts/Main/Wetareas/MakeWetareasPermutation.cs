using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeWetareasPermutation : MonoBehaviour
{
    /// <summary>
    /// 水回りの部屋の全並び順を作成
    /// </summary>
    /// <param name="wetareasKinds">水回りの部屋の種類を指定する配列</param>
    /// <returns>水回りの部屋を全て配置した結果</returns>
    public List<int[]> MakeWetareasKindsPermutation(params int[] array)
    {
        var a = new List<int>(array).ToArray();
        var result = new List<int[]>();
        result.Add(new List<int>(a).ToArray());
        var n = a.Length;
        var next = true;

        while (next)
        {
            next = false;

            //隣り合う要素が昇順(a[i] < a[i+1])になっている一番大きい i を見つける
            int i;
            for (i = n - 2; i >= 0; i--)
            {
                //a[i] < a[i+1]のとき
                if (a[i].CompareTo(a[i + 1]) < 0) { 
                    break;
                }
            }

            //i が見つからないとき、全体が降順になっているので処理終了
            if (i < 0) {
                break;
            }

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
                result.Add(new List<int>(a).ToArray());
                next = true;
            }
        }

        //結果を返す
        return result;
    }

    /// <summary>
    /// 2^n通りの0,1の組み合わせを作成
    /// </summary>
    /// <param name="n">要素数</param>
    /// <returns>2^n通りの0,1の組み合わせ</returns>
    public List<int[]> MakeWetareasRotationPermutation(int n) {
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
