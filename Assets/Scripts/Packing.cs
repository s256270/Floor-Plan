using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Packing : CreateRoom
{
    void Start()
    {
        Vector3[] range = new Vector3[]{new Vector3(-2050, 1850, 0), new Vector3(2050, 1850, 0), new Vector3(2050, -50, 0), new Vector3(1050, -50, 0), new Vector3(1050, -1850, 0), new Vector3(-2050, -1850, 0)};
        createRoom("range", range);

        Vector3[] entrance = new Vector3[]{new Vector3(1050, -50, 0), new Vector3(2050, -50, 0), new Vector3(2050, -1550, 0), new Vector3(1050, -1550, 0)};
        createRoom("entrance", entrance);

        Vector3[] mbps = new Vector3[]{new Vector3(1050, -1550, 0), new Vector3(2050, -1550, 0), new Vector3(2050, -1850, 0), new Vector3(1050, -1850, 0)};
        createRoom("mbps", mbps);

        
    }
}
