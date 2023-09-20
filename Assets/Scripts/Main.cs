using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    [SerializeField] FloorPlanDisplay fpd;

    void Start()
    {
        //間取図を表示
        fpd.Display();
    }
}
