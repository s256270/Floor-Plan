using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateOneDwellingMbps : MonoBehaviour
{
    [SerializeField] PlanReader pr;
    [SerializeField] CommonFunctions cf;
    [SerializeField] Parts pa;

    //階段室の座標
    Vector3[] stairsCoordinates;
    //住戸の座標のリスト
    List<Vector3[]> dwellingCoordinates;

    /// <summary>
    /// 1住戸のMBPSを配置
    /// </summary>
    /// <param name="allPattern">全ての配置結果</param>
    /// <returns>1住戸のMBPSを配置した結果</returns>
    public List<Dictionary<string, Dictionary<string, Vector3[]>>> placeOneDwellingMbps(List<Dictionary<string, Dictionary<string, Vector3[]>>> allPattern) {
        //配置結果のリスト
        var result = new List<Dictionary<string, Dictionary<string, Vector3[]>>>();

        //必要な座標の準備
        //階段室の座標
        stairsCoordinates = pr.getStairsCoordinates();
        //住戸座標のリスト
        dwellingCoordinates = pr.getDwellingCoordinatesList();
        
        //全パターンに対して配置
        for (int i = 0; i < allPattern.Count; i++) {
            
            //1つのパターンについて配置した結果のリスト
            var currentPatternResult = new List<Dictionary<string, Dictionary<string, Vector3[]>>>();
            currentPatternResult.Add(allPattern[i]);
            
            //住戸・階段室のループで
            foreach (KeyValuePair<string, Dictionary<string, Vector3[]>> planParts in allPattern[i]) {
                //住戸について
                if (planParts.Key.Contains("Dwelling")) {
                    //2住戸にまたがるMBPSが配置されているとき
                    if (planParts.Value.ContainsKey("Mbps")) {
                        //スキップ
                        continue;
                    }

                    //住戸のインデックスを算出
                    int dwellingIndex = int.Parse(planParts.Key.Replace("Dwelling", "")) - 1;
                    
                    //MBPSが配置されていない住戸に配置する
                    currentPatternResult = placeOneDwellingMbpsInOneDwelling(currentPatternResult, dwellingIndex);
                }
            }

            result.AddRange(currentPatternResult);
        }

        return result;
    }

    /// <summary>
    /// 1住戸のMBPSを配置するループ
    /// </summary>
    /// <param name="currentPlacement">現在の配置結果</param>
    /// <param name="dwellingIndex">1住戸のMBPSを配置する住戸のインデックス</param>
    /// <returns>dwellingIndexで指定された住戸にMBPSを配置した結果</returns>
    public List<Dictionary<string, Dictionary<string, Vector3[]>>> placeOneDwellingMbpsInOneDwelling(List<Dictionary<string, Dictionary<string, Vector3[]>>> currentPlacement, int dwellingIndex) {
        //配置結果のリスト
        var result = new List<Dictionary<string, Dictionary<string, Vector3[]>>>();

        //配置する住戸の座標
        Vector3[] currentDwellingCoordinates = dwellingCoordinates[dwellingIndex];

        //MBPSをくっつける階段室の辺を求める
        List<Vector3[]> contactStairsCoodinatesList = cf.ContactCoordinates(stairsCoordinates, currentDwellingCoordinates);
        
        //currentPlacementの各パターンについて配置を考える
        for (int i = 0; i < currentPlacement.Count; i++) {
            //配置できる全ての辺について
            for (int j = 0; j < contactStairsCoodinatesList.Count; j++) {
                //配置する辺
                Vector3[] placementSide = contactStairsCoodinatesList[j];
                
                //配置する辺のどちら側に配置するかのループ
                for (int k = 0; k < placementSide.Length; k++) {
                    //MBPSの形状についてのループ
                    for (int l = 0; l < pa.oneDwellingsMbpsCoordinatesList.Count; l++) {
                        result.Add(placeOneDwellingMbpsInOneDwellingActually(currentPlacement[i], placementSide, k, pa.oneDwellingsMbpsCoordinatesList[l], dwellingIndex));
                    }
                }
            }
        }

        return result;
    }

    /// <summary>
    /// 1住戸のMBPSを実際に配置
    /// </summary>
    /// <param name="currentPlacement">現在の配置結果</param>
    /// <param name="placementSide">配置する辺</param>
    /// <param name="placementSideIndex">配置する辺のインデックス</param>
    /// <param name="mbpsCoordinates">MBPSの種類のインデックス</param>
    /// <param name="dwellingIndex">配置する住戸のインデックス</param>
    /// <returns>1住戸に1つのMBPSを配置した結果</returns>
    public Dictionary<string, Dictionary<string, Vector3[]>> placeOneDwellingMbpsInOneDwellingActually(Dictionary<string, Dictionary<string, Vector3[]>> currentPlacement, Vector3[] placementSide, int placementSideIndex, Vector3[] mbpsCoodinates, int dwellingIndex) {
        //配置結果
        var result = cf.DuplicateDictionary(currentPlacement);

        //移動させたMBPSの座標
        Vector3[] correctMbpsCoordinates = mbpsCoodinates;

        //配置する住戸の座標
        Vector3[] currentDwellingCoordinates = dwellingCoordinates[dwellingIndex];

        //配置する辺のどちら側にMBPSが入るか調べる
        int stairsShiftDirection = cf.ShiftJudge(currentDwellingCoordinates, placementSide);

        //階段室に接する辺がy軸平行のとき
        if (cf.Slope(placementSide) == Mathf.Infinity) {         
            //MBPSを回転させる角度
            int rotationAngle = 0;

            //MBPSが左側に入るとき
            if (stairsShiftDirection < 0) {
                //どれだけ回転させるかを決める
                rotationAngle = 270;
            }
            //MBPSが右側に入るとき
            else if (stairsShiftDirection > 0) {
                //どれだけ回転させるかを決める
                rotationAngle = 90;
            }

            //MBPSを回転させる
            correctMbpsCoordinates = cf.Rotation(mbpsCoodinates, rotationAngle);

            //MBPSの幅・高さを求める
            float width = cf.MakeRectangle(correctMbpsCoordinates)[1].x - cf.MakeRectangle(correctMbpsCoordinates)[0].x;
            float height = cf.MakeRectangle(correctMbpsCoordinates)[0].y - cf.MakeRectangle(correctMbpsCoordinates)[3].y;

            //MBPSを移動させる
            //x軸方向に移動
            if (stairsShiftDirection < 0) {
                correctMbpsCoordinates = cf.CorrectCoordinates(correctMbpsCoordinates, new Vector3(placementSide[0].x - width / 2, 0, 0));
            }
            else if (stairsShiftDirection > 0) {
                correctMbpsCoordinates = cf.CorrectCoordinates(correctMbpsCoordinates, new Vector3(placementSide[0].x + width / 2, 0, 0));
            }

            //y軸方向に移動
            if (placementSideIndex == 1) { //配置する辺の上端に配置
                correctMbpsCoordinates = cf.CorrectCoordinates(correctMbpsCoordinates, new Vector3(0, placementSide[placementSideIndex].y - height / 2, 0));
            }
            else if (placementSideIndex == 0) { //配置する辺の下端に配置
                correctMbpsCoordinates = cf.CorrectCoordinates(correctMbpsCoordinates, new Vector3(0, placementSide[placementSideIndex].y + height / 2, 0));
            }
        }
        //階段室に接する辺がy軸平行でないとき
        else {
            //MBPSを回転させる角度
            int rotationAngle = 0;

            //MBPSが上側に入るとき
            if (stairsShiftDirection > 0) {
                //どれだけ回転させるかを決める
                rotationAngle = 180;
            }

            //MBPSを回転させる
            correctMbpsCoordinates = cf.Rotation(mbpsCoodinates, rotationAngle);

            //MBPSの幅・高さを求める
            float width = cf.MakeRectangle(correctMbpsCoordinates)[1].x - cf.MakeRectangle(correctMbpsCoordinates)[0].x;
            float height = cf.MakeRectangle(correctMbpsCoordinates)[0].y - cf.MakeRectangle(correctMbpsCoordinates)[3].y;

            //MBPSを移動させる
            //x軸方向に移動
            if (placementSideIndex == 0) { //配置する辺の左端に配置
                correctMbpsCoordinates = cf.CorrectCoordinates(correctMbpsCoordinates, new Vector3(placementSide[placementSideIndex].x + width / 2, 0, 0));
            }
            else if (placementSideIndex == 1) { //配置する辺の右端に配置
                correctMbpsCoordinates = cf.CorrectCoordinates(correctMbpsCoordinates, new Vector3(placementSide[placementSideIndex].x - width / 2, 0, 0));
            }

            //y軸方向に移動
            if (stairsShiftDirection > 0) {
                correctMbpsCoordinates = cf.CorrectCoordinates(correctMbpsCoordinates, new Vector3(0, placementSide[0].y + height / 2, 0));
            }
            else if (stairsShiftDirection < 0) {
                correctMbpsCoordinates = cf.CorrectCoordinates(correctMbpsCoordinates, new Vector3(0, placementSide[0].y - height / 2, 0));
            }
        }

        //配置結果に追加
        result["Dwelling" + (dwellingIndex + 1)].Add("Mbps", correctMbpsCoordinates);

        return result;
    }
}
