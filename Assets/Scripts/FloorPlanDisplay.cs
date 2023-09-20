using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorPlanDisplay : MonoBehaviour
{
    [SerializeField] PlanMaker pm;
    [SerializeField] FloorPlanner fp;
    [SerializeField] CreateRoom cr;

    GameObject floorPlan; //間取図のオブジェクト
    
    //間取図を表示
    public void Display() {
        floorPlan = new GameObject("FloorPlan");
        
        /*
        //プラン図を表示
        for (int i = 0; i < pm.plan.Count; i++) {
            int loopCounter = 0;
            foreach (KeyValuePair<string, Vector3[]> room in pm.plan[i]) {
                string objectName = "";
                //ここだけで作りたい人生だった  
                if (loopCounter == 0) {
                    a = new GameObject(room.Key);
                }
                GameObject b = cr.createRoom(room.Key, room.Value);
                b.transform.SetParent(a.transform);
            }
            a.transform.SetParent(floorPlan.transform);
        }
        */
    }
}
