using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateTwoDwellingMbps : MonoBehaviour
{
    public List<List<Dictionary<string, Vector3[]>>> placeTwoDwellingsMbps(List<List<Dictionary<string, Vector3[]>>> allPattern) {
        //配置結果のリスト
        var result = new List<List<Dictionary<string, Vector3[]>>>();

        //プラン図の状態を保持
        //var plan = DuplicateDictionary(allPattern[0]);

        //2住戸にまたがるMBPSの全配置パターンを作成
        makeTwoDwellingsMbpsAllPatternList();

        //2住戸にまたがるMBPSの全配置パターンで配置
        for (int i = 0; i < twoDwellingsMbpsAllPattern.Count; i++) {

            //i番目の配置パターンの結果
            var thisPatternResult = new List<Dictionary<string, Vector3[]>>();

            for (int j = 0; j < twoDwellingsMbpsAllPattern[i].Count; j++) {
                thisPatternResult = placeTwoDwellingsMbps(thisPatternResult, twoDwellingsMbpsAllPattern[i][j]);
            }

            //配置結果をリストに追加
            result.Add(thisPatternResult);
        }

        return result;
    }
}
