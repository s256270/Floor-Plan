using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Packing : CreateRoom
{
    void Start()
    {
        Vector3[] range = new Vector3[]{new Vector3(-2050, 1350, 0), new Vector3(2050, 1350, 0), new Vector3(2050, 0, 0), new Vector3(1050, 0, 0), new Vector3(1050, -1350, 0), new Vector3(-2050, -1350, 0)};
        createRoom("range", range);


    }
}
