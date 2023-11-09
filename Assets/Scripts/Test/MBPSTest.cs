using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MBPSTest : MonoBehaviour
{
    [SerializeField] CreateRoom cr;
    [SerializeField] PlanReader pr;

    Vector3[] dwelling;
    Vector3[] balcony;

    void Start()
    {
        /*
        //住戸作成
        dwelling = new Vector3[]{new Vector3(-3400, 1900, 0), new Vector3(4400, 1900, 0), new Vector3(4400, -1900, 0), new Vector3(-3400, -1900, 0)};
        cr.createRoom("dwelling", dwelling);

        //バルコニー作成
        balcony = new Vector3[]{new Vector3(-4400, 1100, 0), new Vector3(-3400, 1100, 0), new Vector3(-3400, -1900, 0), new Vector3(-4400, -1900, 0)};
        cr.createRoom("balcony", balcony);
        */
    }
}
