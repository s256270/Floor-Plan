using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parts : CreateRoom
{
    //玄関の座標
    [HideInInspector]
    public Vector3[] entranceCoordinates = new Vector3[]{
        new Vector3(-750, 500, 0),
        new Vector3(750, 500, 0),
        new Vector3(750, -500, 0),
        new Vector3(-750, -500, 0)
    };

    //MBPSの座標
    [HideInInspector]
    public Vector3[] twoDwellingMbpsCoordinates = new Vector3[]{
        new Vector3(-175, 500, 0),
        new Vector3(175, 500, 0),
        new Vector3(175, -500, 0),
        new Vector3(-175, -500, 0)
    };

    [HideInInspector]
    public Vector3[] oneDwellingMbpsCoordinates = new Vector3[]{
        new Vector3(-350, 350, 0),
        new Vector3(350, 350, 0),
        new Vector3(350, -350, 0),
        new Vector3(-350, -350, 0)
    };

    //洗面室の座標
    [HideInInspector]
    public Vector3[] washroom_coordinates = new Vector3[]{
        new Vector3(-800, 800, 0),
        new Vector3(800, 800, 0),
        new Vector3(800, -800, 0),
        new Vector3(-800, -800, 0)
    };

    //UBの座標
    [HideInInspector]
    public Vector3[] ub_coordinates = new Vector3[]{
        new Vector3(-900, 700, 0),
        new Vector3(900, 700, 0),
        new Vector3(900, -700, 0),
        new Vector3(-900, -700, 0)
    };

    //トイレの座標
    [HideInInspector]
    public Vector3[] toilet_coordinates = new Vector3[]{
        new Vector3(-450, 700, 0),
        new Vector3(450, 700, 0),
        new Vector3(450, -700, 0),
        new Vector3(-450, -700, 0)
    };

    //キッチンの座標
    [HideInInspector]
    public Vector3[] kitchen_coordinates = new Vector3[]{
        new Vector3(-1200, 350, 0),
        new Vector3(1200, 350, 0),
        new Vector3(1200, -350, 0),
        new Vector3(-1200, -350, 0)
    };
}
