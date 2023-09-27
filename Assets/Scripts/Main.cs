using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    List<Dictionary<string, Dictionary<string, Vector3[]>>> allPattern = new List<Dictionary<string, Dictionary<string, Vector3[]>>>();

    [SerializeField] PlanReader pr;
    [SerializeField] FloorPlanner fp;
    [SerializeField] Evaluation ev;
    [SerializeField] CreateRoom cr;

    int count = 0;
    int limit = 0;

    void Start()
    {
        //全パターンのリストにプラン図を追加
        allPattern.Add(pr.plan);

        //部屋を配置
        //allPattern.AddRange(fp.Placement());

        //重複を削除
        //allPattern = RemoveDuplicates(allPattern);

        //間取図を評価
        //allPattern = ev.EvaluateFloorPlan(allPattern);

        limit = allPattern.Count;
        Debug.Log("総パターン数：" + limit);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) {
            if (count < limit) {
                Debug.Log((count+1) + "パターン目");

                if (GameObject.Find("FloorPlan")) {
                    Destroy(GameObject.Find("FloorPlan"));
                }

                Dictionary<string, Dictionary<string, Vector3[]>> currentPattern = allPattern[count];

                Display(currentPattern);

                count++;
            } else {
                Debug.Log("終了");
            }
        }
    }

    //間取図を表示
    public void Display(Dictionary<string, Dictionary<string, Vector3[]>> currentPattern) {
        /* 空のオブジェクトによって，階層をつくる */
        //間取図オブジェクトを作成
        GameObject floorPlan = new GameObject("FloorPlan");
        
        foreach (KeyValuePair<string, Dictionary<string, Vector3[]>> space in currentPattern) {
            //大まかなスペースごとのオブジェクトを作成
            GameObject spaceObject = new GameObject(space.Key);
            //間取図オブジェクトの子オブジェクトにする
            spaceObject.transform.SetParent(floorPlan.transform);

            foreach (KeyValuePair<string, Vector3[]> spaceElements in space.Value) {
                GameObject spaceElementsObject = cr.createRoom(spaceElements.Key, spaceElements.Value);
                spaceElementsObject.transform.SetParent(spaceObject.transform);
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
