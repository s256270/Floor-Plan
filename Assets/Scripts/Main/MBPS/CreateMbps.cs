using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMbps : MonoBehaviour
{
    [SerializeField] PlanReader pr;
    [SerializeField] CommonFunctions cf;
    [SerializeField] CreateTwoDwellingMbps ctdm;
    [SerializeField] CreateOneDwellingMbps codm;

    /// <summary>
    /// MBPSを配置
    /// </summary> 
    /// <param name="allPattern">全ての配置結果</param>
    /// <returns>MBPSを配置した結果</returns>
    public List<Dictionary<string, Dictionary<string, Vector3[]>>> PlaceMbps(List<Dictionary<string, Dictionary<string, Vector3[]>>> allPattern) {
        //配置結果のリスト
        var result = new List<Dictionary<string, Dictionary<string, Vector3[]>>>();

        //2部屋にまたがるMBPSの配置開始
        result = ctdm.PlaceTwoDwellingsMbps(result);

        //1住戸MBPSテスト
        // result.Add(pr.plan);
        // result[0]["Dwelling3"].Add("Mbps", new Vector3[]{new Vector3(-700, -2550, 0), new Vector3(-350, -2550, 0), new Vector3(-350, -3550, 0), new Vector3(-700, -3550, 0)});
        // result[0]["Dwelling4"].Add("Mbps", new Vector3[]{new Vector3(-350, -2550, 0), new Vector3(0, -2550, 0), new Vector3(0, -3550, 0), new Vector3(-350, -3550, 0)});

        //1部屋のみのMBPSの配置開始
        result = codm.PlaceOneDwellingMbps(result);

        //問題のあるパターンを削除
        result = cf.RemoveIrregularPattern(result, "Mbps");

        return result;
    }
}
