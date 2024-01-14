using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateEntrance : MonoBehaviour
{
    [SerializeField] PlanReader pr;
    [SerializeField] CommonFunctions cf;
    [SerializeField] Parts pa;

    //階段室の座標
    Vector3[] stairsCoordinates;
    //住戸の座標のリスト
    List<Vector3[]> dwellingCoordinates;

    /// <summary>
    /// 玄関を配置
    /// </summary> 
    /// <param name="allPattern">全ての配置結果</param>
    /// <returns>玄関を配置した結果</returns>
    public List<Dictionary<string, Dictionary<string, Vector3[]>>> PlaceEntrance(List<Dictionary<string, Dictionary<string, Vector3[]>>> allPattern) {
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
                    //住戸のインデックスを算出
                    int dwellingIndex = int.Parse(planParts.Key.Replace("Dwelling", "")) - 1;

                    //玄関を配置
                    currentPatternResult = PlaceEntranceInOneDwelling(currentPatternResult, dwellingIndex);
                }
            }

            result.AddRange(currentPatternResult);
        }

        //問題のあるパターンを削除
        result = cf.RemoveIrregularPattern(result, "Entrance");

        return result;
    }

    /// <summary>
    /// 玄関を様々なパターンで配置
    /// </summary> 
    /// <param name="currentPlacement">現在の配置結果</param>
    /// <param name="dwellingIndex">玄関を配置する住戸のインデックス</param>
    /// <returns>dwellingIndexで指定された住戸に玄関を配置した結果</returns>
    public List<Dictionary<string, Dictionary<string, Vector3[]>>> PlaceEntranceInOneDwelling(List<Dictionary<string, Dictionary<string, Vector3[]>>> currentPlacement, int dwellingIndex) {
        //配置結果のリスト
        var result = new List<Dictionary<string, Dictionary<string, Vector3[]>>>();

        //配置する住戸の座標
        Vector3[] currentDwellingCoordinates = dwellingCoordinates[dwellingIndex];

        //配置されているMBPSの座標
        Vector3[] currentMbpsCoordinates = currentPlacement[0]["Dwelling" + (dwellingIndex + 1)]["Mbps"];

        //玄関をくっつける階段室の辺を求める
        List<Vector3[]> contactStairsCoordinatesList = cf.ContactCoordinates(stairsCoordinates, currentDwellingCoordinates);
        
        //currentPlacementの各パターンについて配置を考える
        for (int i = 0; i < currentPlacement.Count; i++) {
            //配置できる全ての辺について
            for (int j = 0; j < contactStairsCoordinatesList.Count; j++) {
                //配置する辺
                Vector3[] placementSide = contactStairsCoordinatesList[j];

                //配置する辺からMBPSがある部分を除く
                if (cf.ContactJudge(placementSide, currentMbpsCoordinates)) {
                    placementSide = cf.SideSubstraction(placementSide, cf.ContactCoordinates(placementSide, currentMbpsCoordinates)[0])[0];
                }
                
                //配置する辺のどちら側に配置するかのループ
                for (int k = 0; k < placementSide.Length; k++) {
                    //玄関の形状についてのループ
                    for (int l = 0; l < pa.entranceCoordinatesList.Count; l++) {
                        result.Add(PlaceEntranceInOneDwellingActually(currentPlacement[i], placementSide, k, pa.entranceCoordinatesList[l], dwellingIndex));
                    }
                }
            }
        }

        return result;
    }

    /// <summary>
    /// 玄関を実際に配置
    /// </summary>
    /// <param name="currentPlacement">現在の配置結果</param>
    /// <param name="placementSide">配置する辺</param>
    /// <param name="placementSideIndex">配置する辺のインデックス</param>
    /// <param name="entranceCoordinates">MBPSの種類のインデックス</param>
    /// <param name="dwellingIndex">配置する住戸のインデックス</param>
    /// <returns>1住戸に1つのMBPSを配置した結果</returns>
    public Dictionary<string, Dictionary<string, Vector3[]>> PlaceEntranceInOneDwellingActually(Dictionary<string, Dictionary<string, Vector3[]>> currentPlacement, Vector3[] placementSide, int placementSideIndex, Vector3[] entranceCoordinates, int dwellingIndex) {
        //配置結果
        var result = cf.DuplicateDictionary(currentPlacement);

        //移動させた玄関の座標
        Vector3[] correctEntranceCoordinates = entranceCoordinates;

        //配置する住戸の座標
        Vector3[] currentDwellingCoordinates = dwellingCoordinates[dwellingIndex];

        //配置する辺のどちら側に玄関が入るか調べる
        int stairsShiftDirection = cf.ShiftJudge(currentDwellingCoordinates, placementSide);

        //階段室に接する辺がy軸平行のとき
        if (cf.Slope(placementSide) == Mathf.Infinity) {         
            //玄関を回転させる角度
            int rotationAngle = 0;

            //玄関が左側に入るとき
            if (stairsShiftDirection < 0) {
                //どれだけ回転させるかを決める
                rotationAngle = 270;
            }
            //玄関が右側に入るとき
            else if (stairsShiftDirection > 0) {
                //どれだけ回転させるかを決める
                rotationAngle = 90;
            }

            //玄関を回転させる
            correctEntranceCoordinates = cf.Rotation(entranceCoordinates, rotationAngle);

            //玄関の幅・高さを求める
            float width = cf.CalculateRectangleWidth(correctEntranceCoordinates);
            float height = cf.CalculateRectangleHeight(correctEntranceCoordinates);

            //玄関を移動させる
            //x軸方向に移動
            if (stairsShiftDirection < 0) {
                correctEntranceCoordinates = cf.CorrectCoordinates(correctEntranceCoordinates, new Vector3(placementSide[0].x - width / 2, 0, 0));
            }
            else if (stairsShiftDirection > 0) {
                correctEntranceCoordinates = cf.CorrectCoordinates(correctEntranceCoordinates, new Vector3(placementSide[0].x + width / 2, 0, 0));
            }

            //y軸方向に移動
            if (placementSideIndex == 1) { //配置する辺の上端に配置
                correctEntranceCoordinates = cf.CorrectCoordinates(correctEntranceCoordinates, new Vector3(0, placementSide[placementSideIndex].y - height / 2, 0));
            }
            else if (placementSideIndex == 0) { //配置する辺の下端に配置
                correctEntranceCoordinates = cf.CorrectCoordinates(correctEntranceCoordinates, new Vector3(0, placementSide[placementSideIndex].y + height / 2, 0));
            }
        }
        //階段室に接する辺がy軸平行でないとき
        else {
            //玄関を回転させる角度
            int rotationAngle = 0;

            //玄関が上側に入るとき
            if (stairsShiftDirection > 0) {
                //どれだけ回転させるかを決める
                rotationAngle = 180;
            }

            //玄関を回転させる
            correctEntranceCoordinates = cf.Rotation(entranceCoordinates, rotationAngle);

            //玄関の幅・高さを求める
            float width = cf.CalculateRectangleWidth(correctEntranceCoordinates);
            float height = cf.CalculateRectangleHeight(correctEntranceCoordinates);

            //玄関を移動させる
            //x軸方向に移動
            if (placementSideIndex == 0) { //配置する辺の左端に配置
                correctEntranceCoordinates = cf.CorrectCoordinates(correctEntranceCoordinates, new Vector3(placementSide[placementSideIndex].x + width / 2, 0, 0));
            }
            else if (placementSideIndex == 1) { //配置する辺の右端に配置
                correctEntranceCoordinates = cf.CorrectCoordinates(correctEntranceCoordinates, new Vector3(placementSide[placementSideIndex].x - width / 2, 0, 0));
            }

            //y軸方向に移動
            if (stairsShiftDirection > 0) {
                correctEntranceCoordinates = cf.CorrectCoordinates(correctEntranceCoordinates, new Vector3(0, placementSide[0].y + height / 2, 0));
            }
            else if (stairsShiftDirection < 0) {
                correctEntranceCoordinates = cf.CorrectCoordinates(correctEntranceCoordinates, new Vector3(0, placementSide[0].y - height / 2, 0));
            }
        }

        //配置結果に追加
        result["Dwelling" + (dwellingIndex + 1)].Add("Entrance", correctEntranceCoordinates);

        return result;
    }
}
