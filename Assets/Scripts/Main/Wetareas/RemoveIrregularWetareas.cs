using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveIrregularWetareas : MonoBehaviour
{
    /// <summary>
    /// 問題のある水回りパターンを削除
    /// </summary> 
    /// <param name="allPattern">全ての配置結果</param>
    /// <returns>問題のある水回りパターンを削除した結果</returns>
    public List<Dictionary<string, Vector3[]>> RemoveIrregularWetareasPattern(List<Dictionary<string, Vector3[]>> allPattern) {
        //結果のリスト
        var result = new List<Dictionary<string, Vector3[]>>(allPattern);

        //必要な部屋がすべて配置されていないものを除く
        result = RemoveNotEnaughRoom(result);

        

        return result;
    }

    /// <summary>
    /// 必要な部屋がすべて配置されていないパターンを削除
    /// </summary>
    /// <param name="allPattern">全ての配置結果</param>
    /// <returns>必要な部屋がすべて配置されていないパターンを削除した結果</returns>
    public List<Dictionary<string, Vector3[]>> RemoveNotEnaughRoom(List<Dictionary<string, Vector3[]>> allPattern) {
        //結果のリスト
        var result = new List<Dictionary<string, Vector3[]>>(allPattern);

        //必要な部屋がすべて配置されていないものを除く
        for (int i = 0; i < result.Count; i++) {
            //必要な部屋がすべて配置されていないとき
            if (!(result[i].ContainsKey("UB") && result[i].ContainsKey("Washroom") && result[i].ContainsKey("Toilet") && result[i].ContainsKey("Kitchen"))) {
                //その配置結果を除く
                result.RemoveAt(i);
                i--;
            }
        }

        return result;
    }
}
