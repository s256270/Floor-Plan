using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    List<Dictionary<string, Dictionary<string, Vector3[]>>> allPattern = new List<Dictionary<string, Dictionary<string, Vector3[]>>>();
    List<Dictionary<string, Vector3[]>> dwellingPattern = new List<Dictionary<string, Vector3[]>>();
    List<Dictionary<string, Vector3[]>> correctedDwellingPattern = new List<Dictionary<string, Vector3[]>>();

    [SerializeField] PlanReader pr;
    [SerializeField] CommonFunctions cf;
    [SerializeField] FloorPlanner fp;
    [SerializeField] Evaluation ev;

    int count = 0;
    int limit = 0;
    bool individual = false;

    void Start()
    {
        //間取図を作成
        allPattern = fp.Placement();

        //間取図を評価  
        //allPattern = ev.EvaluateFloorPlan(allPattern);

        //1住戸のみのリストを作成
        for (int i = 0; i < allPattern.Count; i++) {
            foreach (KeyValuePair<string, Dictionary<string, Vector3[]>> pattern in allPattern[i]) {
                if (pattern.Key.Contains("Dwelling1")) {
                //if (pattern.Key.Contains("Dwelling4")) {
                    dwellingPattern.Add(pattern.Value);
                }
            }
        }

        //1住戸のみのリストの座標を補正
        for (int i = 0; i < dwellingPattern.Count; i++) {
            var tempDictinary = new Dictionary<string, Vector3[]>(); 
            foreach (KeyValuePair<string, Vector3[]> pattern in dwellingPattern[i]) {
                tempDictinary.Add(pattern.Key, cf.CorrectCoordinates(pattern.Value, new Vector3(6250, -1750, 0)));
                //tempDictinary.Add(pattern.Key, cf.CorrectCoordinates(pattern.Value, new Vector3(-5000, 1700, 0)));
            }
            correctedDwellingPattern.Add(tempDictinary);
        }

        limit = allPattern.Count;
        Debug.Log("総パターン数：" + limit);
    }

    void Update()
    {
        //上矢印キーを押すと，間取図を一括表示
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            Debug.Log("間取図を一括表示");
            individual = false;

            //前の住戸を削除
            if (GameObject.Find("Dwelling")) {
                Destroy(GameObject.Find("Dwelling"));
            }

            GameObject dwellings = new GameObject("Dwellings");

            //カメラサイズ13000がちょうどいい（普段は2600）
            GameObject.Find("Main Camera").GetComponent<Camera>().orthographicSize = 15000;
            for (int i = 0; i < 5; i++) {
                for (int j = 0; j < 3; j++) {
                    GameObject dwelling = new GameObject("Dwelling" + (i + j * 5));
                    foreach (KeyValuePair<string, Vector3[]> room in correctedDwellingPattern[i + j * 5]) {
                        //10パターン表示
                        // if (j == 0) {
                        //     GameObject roomObject = cf.CreateRoom(room.Key, cf.CorrectCoordinates(room.Value, new Vector3(10000f * (i - 2), 3800 * 0.75f, 0)));
                        //     roomObject.transform.SetParent(dwelling.transform);
                        // }
                        // else if (j == 1) {
                        //     GameObject roomObject = cf.CreateRoom(room.Key, cf.CorrectCoordinates(room.Value, new Vector3(10000f * (i - 2), 3800 * -0.75f, 0)));
                        //     roomObject.transform.SetParent(dwelling.transform);
                        // }

                        //20パターン表示
                        if (j == 0) {
                            GameObject roomObject = cf.CreateRoom(room.Key, cf.CorrectCoordinates(room.Value, new Vector3(10000f * (i - 2), 3800 * 2.25f, 0)));
                            //GameObject roomObject = cf.CreateRoom(room.Key, cf.CorrectCoordinates(room.Value, new Vector3(12000f * (i - 2), 3500 * 2.25f, 0)));
                            roomObject.transform.SetParent(dwelling.transform);
                        }
                        else if (j == 1) {
                            GameObject roomObject = cf.CreateRoom(room.Key, cf.CorrectCoordinates(room.Value, new Vector3(10000f * (i - 2), 3800 * 0.75f, 0)));
                            //GameObject roomObject = cf.CreateRoom(room.Key, cf.CorrectCoordinates(room.Value, new Vector3(12000f * (i - 2), 3500 * 0.75f, 0)));
                            roomObject.transform.SetParent(dwelling.transform);
                        }
                        else if (j == 2) {
                            GameObject roomObject = cf.CreateRoom(room.Key, cf.CorrectCoordinates(room.Value, new Vector3(10000f * (i - 2), 3800 * -0.75f, 0)));
                            //GameObject roomObject = cf.CreateRoom(room.Key, cf.CorrectCoordinates(room.Value, new Vector3(12000f * (i - 2), 3500 * -0.75f, 0)));
                            roomObject.transform.SetParent(dwelling.transform);
                        }
                        else if (j == 3) {
                            GameObject roomObject = cf.CreateRoom(room.Key, cf.CorrectCoordinates(room.Value, new Vector3(10000f * (i - 2), 3800 * -2.25f, 0)));
                            //GameObject roomObject = cf.CreateRoom(room.Key, cf.CorrectCoordinates(room.Value, new Vector3(12000f * (i - 2), 3500 * -2.25f, 0)));
                            roomObject.transform.SetParent(dwelling.transform);
                        }
                    }
                    dwelling.transform.SetParent(dwellings.transform);
                }
            }
        }
        
        //下矢印キーを押すと，間取図を1つずつ表示
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            Debug.Log("間取図を1つずつ表示");
            count = 0;
            //GameObject.Find("Main Camera").GetComponent<Camera>().orthographicSize = 2600;
            //前の住戸を削除
            if (GameObject.Find("Dwellings")) {
                Destroy(GameObject.Find("Dwellings"));
            }
            if (GameObject.Find("Dwelling")) {
                Destroy(GameObject.Find("Dwelling"));
            }
            individual = true;
        }
        
        if (individual) { 
            //Enterキーを押すと，間取図を表示
            if (Input.GetKeyDown(KeyCode.Return)) {
                if (count < limit) {
                    Debug.Log((count+1) + "パターン目");

                    // //前の間取図を削除
                    // if (GameObject.Find("FloorPlan")) {
                    //     Destroy(GameObject.Find("FloorPlan"));
                    // }

                    // Dictionary<string, Dictionary<string, Vector3[]>> currentPattern = allPattern[count];

                    // //間取図を表示
                    // DisplayFloorPlan(currentPattern);

                    //前の住戸を削除
                    if (GameObject.Find("Dwelling")) {
                        Destroy(GameObject.Find("Dwelling"));
                    }

                    Dictionary<string, Vector3[]> currentPattern = correctedDwellingPattern[count];

                    //住戸を表示
                    DisplayDwelling(currentPattern);

                    count++;
                } else {
                    Debug.Log("終了");
                }
            }
        }
    }

    //間取図を表示
    public void DisplayFloorPlan(Dictionary<string, Dictionary<string, Vector3[]>> currentPattern) {
        //空のオブジェクトによって，階層をつくる
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

    //住戸を表示
    public void DisplayDwelling(Dictionary<string, Vector3[]> currentPattern) {
        //空のオブジェクトによって，階層をつくる
        //住戸オブジェクトを作成
        GameObject dwelling = new GameObject("Dwelling");
        
        foreach (KeyValuePair<string, Vector3[]> room in currentPattern) {
            //部屋ごとのオブジェクトを作成
            GameObject roomObject = cf.CreateRoom(room.Key, room.Value);
            //住戸オブジェクトの子オブジェクトにする
            roomObject.transform.SetParent(dwelling.transform);
        }
    }
}
