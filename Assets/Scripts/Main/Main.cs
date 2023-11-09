using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    List<List<Dictionary<string, Vector3[]>>> allPattern = new List<List<Dictionary<string, Vector3[]>>>();

    [SerializeField] PlanReader pr;
    [SerializeField] FloorPlanner fp;
    [SerializeField] Evaluation ev;
    [SerializeField] CreateRoom cr;

    int count = 0;
    int limit = 0;

    void Start()
    {
        Display(pr.plan);

        //2住戸のMBPS配置の例
        cr.createRoom("mbps", fp.CorrectCoordinates(fp.Rotation(new Vector3[]{new Vector3(-175, 500, 0), new Vector3(175, 500, 0), new Vector3(175, -500, 0), new Vector3(-175, -500, 0)}), new Vector3(-2350, 25, 0)));
        cr.createRoom("mbps", fp.CorrectCoordinates(fp.Rotation(new Vector3[]{new Vector3(-175, 500, 0), new Vector3(175, 500, 0), new Vector3(175, -500, 0), new Vector3(-175, -500, 0)}), new Vector3(-2350, -325, 0)));

        //1住戸のMBPS配置の例
        //cr.createRoom("mbps", fp.CorrectCoordinates(fp.Rotation(new Vector3[]{new Vector3(-350, 350, 0), new Vector3(350, 350, 0), new Vector3(350, -350, 0), new Vector3(-350, -350, 0)}), new Vector3(-2200, 3300, 0)));
        //cr.createRoom("mbps", fp.CorrectCoordinates(fp.Rotation(new Vector3[]{new Vector3(-350, 350, 0), new Vector3(350, 350, 0), new Vector3(350, -350, 0), new Vector3(-350, -350, 0)}), new Vector3(-2200, -500, 0)));

        //玄関配置の例
        cr.createRoom("entrance", fp.CorrectCoordinates(fp.Rotation(new Vector3[]{new Vector3(-750, 500, 0), new Vector3(750, 500, 0), new Vector3(750, -500, 0), new Vector3(-750, -500, 0)}), new Vector3(-2350, 950, 0)));

        //部屋を配置
        //allPattern = fp.Placement();

        //重複を削除
        //allPattern = RemoveDuplicates(allPattern);

        //間取図を評価  
        //allPattern = ev.EvaluateFloorPlan(allPattern);

        limit = allPattern.Count;
        Debug.Log("総パターン数：" + limit);
    }

    void Update()
    {
        //Enterキーを押すと，間取図を表示
        if (Input.GetKeyDown(KeyCode.Return)) {
            if (count < limit) {
                Debug.Log((count+1) + "パターン目");

                //前の間取図を削除
                if (GameObject.Find("FloorPlan")) {
                    Destroy(GameObject.Find("FloorPlan"));
                }

                List<Dictionary<string, Vector3[]>> currentPattern = allPattern[count];

                //間取図を表示
                Display(currentPattern);

                count++;
            } else {
                Debug.Log("終了");
            }
        }
    }

    //間取図を表示
    public void Display(List<Dictionary<string, Vector3[]>> currentPattern) {
        /* 空のオブジェクトによって，階層をつくる */
        //間取図オブジェクトを作成
        GameObject floorPlan = new GameObject("FloorPlan");
        
        for (int i = 0; i < pr.plan.Count; i++) {
            Dictionary<string, Vector3[]> planParts = pr.plan[i];

            //階段室の場合
            if (i == 0) {
                //階段室オブジェクトを作成
                GameObject stairsObject = cr.createRoom("Stairs", planParts["Stairs"]);
                //間取図オブジェクトの子オブジェクトにする
                stairsObject.transform.SetParent(floorPlan.transform);
            }

            //住戸の場合
            else {
                //住戸に関するパーツをまとめるためのオブジェクトを作成
                GameObject dwellingObject = new GameObject("Dwelling unit" + i);

                //住戸とバルコニーを作成
                foreach (KeyValuePair<string, Vector3[]> planPartsElement in planParts) {
                    //住戸orバルコニーのオブジェクトを作成
                    GameObject planPartsElementObject = cr.createRoom(planPartsElement.Key, planPartsElement.Value);
                    //住戸に関するパーツをまとめるためのオブジェクトの子オブジェクトにする
                    planPartsElementObject.transform.SetParent(dwellingObject.transform);
                }

                //住戸の中身を作成
                foreach (KeyValuePair<string, Vector3[]> currentPatternElement in currentPattern[i-1]) {
                    //住戸の中身のオブジェクトを作成
                    GameObject currentPatternElementObject = cr.createRoom(currentPatternElement.Key, currentPatternElement.Value);
                    //住戸に関するパーツをまとめるためのオブジェクトの子オブジェクトにする
                    currentPatternElementObject.transform.SetParent(dwellingObject.transform);
                }

                //間取図オブジェクトの子オブジェクトにする
                dwellingObject.transform.SetParent(floorPlan.transform);
            }
        }
    }

    //重複を削除
    public List<Dictionary<string, Dictionary<string, Vector3[]>>> RemoveDuplicates(List<Dictionary<string, Dictionary<string, Vector3[]>>> allPattern) {
        int trueCounter = 0;
        int dwellingNumber = 4;

        //重複を削除
        for (int i = 0; i < allPattern.Count - 1; i++) {
            for (int j = i + 1; j < allPattern.Count; j++) {

                trueCounter = 0;
                for (int k = 0; k < dwellingNumber; k++) {
                    if (DictionaryEquals(allPattern[i]["Dwelling" + (k + 1)], allPattern[j]["Dwelling" + (k + 1)])) {
                        trueCounter++;
                    }  
                }

                if (trueCounter == dwellingNumber) {
                    allPattern.RemoveAt(j);
                    j--;
                }
            }
        }

        return allPattern;
    }

    /// <summary>
    /// 2つの辞書が等しいかどうかを判定
    /// </summary> 
    /// <param name="dictionaryA">辞書A</param>
    /// <param name="dictionaryB">辞書B</param>
    /// <returns>2つの辞書が等しいときtrue, そうでないときfalse</returns>
    public bool DictionaryEquals(Dictionary<string, Vector3[]> dictionaryA, Dictionary<string, Vector3[]> dictionaryB) {
        //判定結果
        bool flag = false;

        //辞書の要素数が異なる場合
        if (dictionaryA.Count != dictionaryB.Count) {
            return flag;
        }

        //辞書のキーが異なる場合
        foreach (string key in dictionaryA.Keys) {
            if (!dictionaryB.ContainsKey(key)) {
                return flag;
            }
        }

        //辞書の値の長さが異なる場合
        foreach (string key in dictionaryA.Keys) {
            if (dictionaryA[key].Length != dictionaryB[key].Length) {
                return flag;
            }
        }

        //辞書の値が異なる場合
        foreach (string key in dictionaryA.Keys) {
            for (int i = 0; i < dictionaryA[key].Length; i++) {
                if (dictionaryA[key][i].x != dictionaryB[key][i].x) {
                    return flag;
                }

                if (dictionaryA[key][i].y != dictionaryB[key][i].y) {
                    return flag;
                }
            }
        }

        flag = true;

        return flag;
    }
}
