using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MBPSTest : MonoBehaviour
{
    [SerializeField] CreateRoom cr;
    [SerializeField] CreateMbps cm;
    [SerializeField] CreateTwoDwellingMbps ctdm;
    [SerializeField] PlanReader pr;

    Vector3[] dwelling;
    Vector3[] balcony;

    List<Dictionary<string, Vector3[]>[]> allPattern = new List<Dictionary<string, Vector3[]>[]>();

    // int count = 0;
    // int limit = 0;

    void Start()
    {
        // Display(pr.plan);

        // ctdm.placeTwoDwellingsMbps(new Dictionary<string, Vector3[]>[4], new List<int>(){0, 2});
        // ctdm.placeTwoDwellingsMbps(new Dictionary<string, Vector3[]>[4], new List<int>(){1, 3});

        // limit = allPattern.Count;
        // Debug.Log("総パターン数：" + limit);
    }

    // void Update()
    // {
    //     //Enterキーを押すと，間取図を表示
    //     if (Input.GetKeyDown(KeyCode.Return)) {
    //         if (count < limit) {
    //             Debug.Log((count+1) + "パターン目");

    //             //前の間取図を削除
    //             if (GameObject.Find("FloorPlan")) {
    //                 Destroy(GameObject.Find("FloorPlan"));
    //             }

    //             List<Dictionary<string, Vector3[]>> currentPattern = allPattern[count];

    //             //間取図を表示
    //             Display(currentPattern);

    //             count++;
    //         } else {
    //             Debug.Log("終了");
    //         }
    //     }
    // }

    //間取図を表示
    // public void Display(List<Dictionary<string, Vector3[]>> currentPattern) {
    //     /* 空のオブジェクトによって，階層をつくる */
    //     //間取図オブジェクトを作成
    //     GameObject floorPlan = new GameObject("FloorPlan");
        
    //     for (int i = 0; i < pr.plan.Count; i++) {
    //         Dictionary<string, Vector3[]> planParts = pr.plan[i];

    //         //階段室の場合
    //         if (i == 0) {
    //             //階段室オブジェクトを作成
    //             GameObject stairsObject = cr.createRoom("Stairs", planParts["Stairs"]);
    //             //間取図オブジェクトの子オブジェクトにする
    //             stairsObject.transform.SetParent(floorPlan.transform);
    //         }

    //         //住戸の場合
    //         else {
    //             //住戸に関するパーツをまとめるためのオブジェクトを作成
    //             GameObject dwellingObject = new GameObject("Dwelling unit" + i);

    //             //住戸とバルコニーを作成
    //             foreach (KeyValuePair<string, Vector3[]> planPartsElement in planParts) {
    //                 //住戸orバルコニーのオブジェクトを作成
    //                 GameObject planPartsElementObject = cr.createRoom(planPartsElement.Key, planPartsElement.Value);
    //                 //住戸に関するパーツをまとめるためのオブジェクトの子オブジェクトにする
    //                 planPartsElementObject.transform.SetParent(dwellingObject.transform);
    //             }

    //             // //住戸の中身を作成
    //             // foreach (KeyValuePair<string, Vector3[]> currentPatternElement in currentPattern[i-1]) {
    //             //     //住戸の中身のオブジェクトを作成
    //             //     GameObject currentPatternElementObject = cr.createRoom(currentPatternElement.Key, currentPatternElement.Value);
    //             //     //住戸に関するパーツをまとめるためのオブジェクトの子オブジェクトにする
    //             //     currentPatternElementObject.transform.SetParent(dwellingObject.transform);
    //             // }

    //             //間取図オブジェクトの子オブジェクトにする
    //             dwellingObject.transform.SetParent(floorPlan.transform);
    //         }
    //     }
    // }
}
