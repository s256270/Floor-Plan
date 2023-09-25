using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    List<Dictionary<string, Dictionary<string, Vector3[]>>> allPattern;

    [SerializeField] PlanReader pr;
    [SerializeField] FloorPlanner fp;
    [SerializeField] CreateRoom cr;

    void Start()
    {
        //間取図を表示
        Display();
    }

    //間取図を表示
    public void Display() {
        /* 空のオブジェクトによって，階層をつくる */
        //間取図オブジェクトを作成
        GameObject floorPlan = new GameObject("FloorPlan");
        
        foreach (KeyValuePair<string, Dictionary<string, Vector3[]>> space in pr.plan) {
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
}
