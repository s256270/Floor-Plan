using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeTwoDwellingsMbpsList : MonoBehaviour
{
    [SerializeField] PlanReader pr;
    [SerializeField] CommonFunctions cf;

    //階段室の座標
    Vector3[] stairsCoordinates;
    //住戸の座標のリスト
    List<Vector3[]> dwellingCoordinates;

    //階段室のある1辺に接している住戸（dwellingCoordinatesのインデックスで管理）の組のリスト
    List<List<int>> contactStairs = new List<List<int>>();

    //2住戸にまたがるMBPSの全配置パターン
    List<List<List<int>>> twoDwellingsMbpsAllPattern = new List<List<List<int>>>();

    /// <summary>
    /// 2住戸にまたがるMBPSの全配置パターンを作成する
    /// </summary> 
    public List<List<List<int>>> makeTwoDwellingsMbpsAllPatternList() {
        // 階段室のある1辺に接している住戸（dwellingCoordinatesのインデックスで管理）の組のリスト
        // List<List<int>> contactStairs = new List<List<int>>();

        //必要な座標の準備
        //階段室の座標のみを抜き出し
        stairsCoordinates = pr.getStairsCoordinates();
        //住戸座標のリストを作成
        dwellingCoordinates = pr.getDwellingCoordinatesList();

        //リストを作成
        //階段室の1辺の座標
        for (int i = 0; i < stairsCoordinates.Length; i++) {  
            List<int> contactDwellingsIndex = new List<int>();       
            for (int j = 0; j < dwellingCoordinates.Count; j++) {
                //階段室と住戸が接しているとき
                if (cf.ContactJudge(new Vector3[]{stairsCoordinates[i], stairsCoordinates[(i+1)%stairsCoordinates.Length]}, dwellingCoordinates[j])) {
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

        //contactStairsに含まれる住戸の種類数
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
        
        //contactStairsに3住戸以上の組が含まれるとき，2住戸ずつの組み合わせに分ける
        //Coming Soon...

        //2住戸にまたがるMBPSの全配置パターンを作成
        //組み合わせ決定用の配列
        int[] pattern = new int[contactStairs.Count];

        //2住戸にまたがるMBPSの全配置パターンを作成
        combination(pattern, contactNumber / 2, 0);

        return twoDwellingsMbpsAllPattern;
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

        //num_decided個目の要素を"選ぶ"場合のパターンを作成
        pattern[num_decided] = 1;
        combination(pattern, r, num_decided + 1);

        //num_decided個目の要素を"選ばない"場合のパターンを作成
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
}
