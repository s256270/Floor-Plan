using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    List<Dictionary<string, Dictionary<string, Vector3[]>>> allPattern = new List<Dictionary<string, Dictionary<string, Vector3[]>>>();

    [SerializeField] PlanReader pr;
    [SerializeField] CommonFunctions cf;
    [SerializeField] FloorPlanner fp;
    [SerializeField] Evaluation ev;

    int count = 0;
    int limit = 0;

    void Start()
    {
        //部屋を配置
        allPattern = fp.Placement();

        //allPattern.Add(pr.plan);

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

                Dictionary<string, Dictionary<string, Vector3[]>> currentPattern = allPattern[count];

                //間取図を表示
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
        
        foreach (KeyValuePair<string, Dictionary<string, Vector3[]>> planParts in currentPattern) {
            //大まかなスペースごとのオブジェクトを作成
            GameObject planPartsObject = new GameObject(planParts.Key);
            //間取図オブジェクトの子オブジェクトにする
            planPartsObject.transform.SetParent(floorPlan.transform);

            foreach (KeyValuePair<string, Vector3[]> planPartsElements in planParts.Value) {
                GameObject planPartsElementsObject = cf.CreateRoom(planPartsElements.Key, planPartsElements.Value);
                planPartsElementsObject.transform.SetParent(planPartsObject.transform);
            }      
        }
    }
}
