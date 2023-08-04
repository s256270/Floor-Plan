using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parts : CreateRoom
{
    //玄関の座標
    public Vector3[] entrance_coordinate_x = new Vector3[]{
        new Vector3(-600, 350, 0),
        new Vector3(600, 350, 0),
        new Vector3(600, -350, 0),
        new Vector3(-600, -350, 0)
    };

    public Vector3[] entrance_coordinate_y = new Vector3[]{
        new Vector3(-350, 600, 0),
        new Vector3(350, 600, 0),
        new Vector3(350, -600, 0),
        new Vector3(-350, -600, 0)
    };

    //MBPSの座標
    public Vector3[] mbps_coordinate = new Vector3[]{
        new Vector3(-175, 500, 0),
        new Vector3(175, 500, 0),
        new Vector3(175, -500, 0),
        new Vector3(-175, -500, 0)
    };

    public Vector3[] mbps_coordinate2 = new Vector3[]{
        new Vector3(-350, 350, 0),
        new Vector3(350, 350, 0),
        new Vector3(350, -350, 0),
        new Vector3(-350, -350, 0)
    };

    //洗面室の座標
    public Vector3[] washroom_coordinates = new Vector3[]{
        new Vector3(-800, 800, 0),
        new Vector3(800, 800, 0),
        new Vector3(800, -800, 0),
        new Vector3(-800, -800, 0)
    };

    //UBの座標
    public Vector3[] ub_coordinates = new Vector3[]{
        new Vector3(-950, 700, 0),
        new Vector3(950, 700, 0),
        new Vector3(950, -700, 0),
        new Vector3(-950, -700, 0)
    };

    //トイレの座標
    public Vector3[] toilet_coordinates = new Vector3[]{
        new Vector3(-450, 700, 0),
        new Vector3(450, 700, 0),
        new Vector3(450, -700, 0),
        new Vector3(-450, -700, 0)
    };

    //キッチンの座標
    public Vector3[] kitchen_coordinates = new Vector3[]{
        new Vector3(-1200, 350, 0),
        new Vector3(1200, 350, 0),
        new Vector3(1200, -350, 0),
        new Vector3(-1200, -350, 0)
    };
}
