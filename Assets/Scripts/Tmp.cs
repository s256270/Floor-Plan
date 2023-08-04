using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo : MonoBehaviour
{
    public CreateRoom cr;

    public void demo() {
        //MBPSを作る線が通る点の座標
        Vector3[] mbps1Positions = new Vector3[]{
            new Vector3(-1.45f, -0.95f, 0),
            new Vector3(-0.75f, -0.95f, 0),
            new Vector3(-0.75f, -1.95f, 0),
            new Vector3(-1.45f, -1.95f, 0),
            new Vector3(-1.45f, -0.95f, 0)
        };
        //MBPSを描画
        cr.createRoom("MBPS", mbps1Positions);

        //玄関を作る線が通る点の座標
        Vector3[] entrancePositions = new Vector3[]{
            new Vector3(-3.2f, -0.95f, 0),
            new Vector3(-1.45f, -0.95f, 0),
            new Vector3(-1.45f, -1.95f, 0),
            new Vector3(-3.2f, -1.95f, 0),
            new Vector3(-3.2f, -0.95f, 0)
        };
        //玄関を描画  
        cr.createRoom("Entrance", entrancePositions);

        //トイレを作る線が通る点の座標
        Vector3[] toiletPositions = new Vector3[]{
            new Vector3(-2.0f, -1.95f, 0),
            new Vector3(-1.1f, -1.95f, 0),
            new Vector3(-1.1f, -3.35f, 0),
            new Vector3(-2.0f, -3.35f, 0),
            new Vector3(-2.0f, -1.95f, 0)
        };
        //トイレを描画  
        cr.createRoom("Toilet", toiletPositions);

        //LDを作る線が通る点の座標
        Vector3[] ldPositions = new Vector3[]{
            new Vector3(-5.6f, -3.35f, 0),
            new Vector3(-1.1f, -3.35f, 0),
            new Vector3(-1.1f, -6.05f, 0),
            new Vector3(-5.6f, -6.05f, 0),
            new Vector3(-5.6f, -3.35f, 0)
        };
        //LDを描画  
        cr.createRoom("Living Dining", ldPositions);

        //洋室を作る線が通る点の座標
        Vector3[] westernPositions = new Vector3[]{
            new Vector3(-8.3f, -3.15f, 0),
            new Vector3(-5.6f, -3.15f, 0),
            new Vector3(-5.6f, -6.05f, 0),
            new Vector3(-8.3f, -6.05f, 0),
            new Vector3(-8.3f, -3.15f, 0)
        };
        //洋室を描画  
        cr.createRoom("Western-style Room", westernPositions);

        //クローゼットを作る線が通る点の座標
        Vector3[] closet1Positions = new Vector3[]{
            new Vector3(-8.3f, -2.45f, 0),
            new Vector3(-7.3f, -2.45f, 0),
            new Vector3(-7.3f, -3.15f, 0),
            new Vector3(-8.3f, -3.15f, 0),
            new Vector3(-8.3f, -2.45f, 0)
        };
        //洋室を描画  
        cr.createRoom("CL", closet1Positions);

        //クローゼットを作る線が通る点の座標
        Vector3[] closet2Positions = new Vector3[]{
            new Vector3(-7.3f, -2.45f, 0),
            new Vector3(-5.6f, -2.45f, 0),
            new Vector3(-5.6f, -3.15f, 0),
            new Vector3(-7.3f, -3.15f, 0),
            new Vector3(-7.3f, -2.45f, 0)
        };
        //クローゼットを描画  
        cr.createRoom("CL", closet2Positions);

        //ユニットバスを作る線が通る点の座標
        Vector3[] ubPositions = new Vector3[]{
            new Vector3(-8.3f, -0.95f, 0),
            new Vector3(-6.4f, -0.95f, 0),
            new Vector3(-6.4f, -2.45f, 0),
            new Vector3(-8.3f, -2.45f, 0),
            new Vector3(-8.3f, -0.95f, 0)
        };
        //ユニットバスを描画  
        cr.createRoom("UB", ubPositions);

        //洗面室を作る線が通る点の座標
        Vector3[] bathroomPositions = new Vector3[]{
            new Vector3(-6.4f, -0.95f, 0),
            new Vector3(-4.9f, -0.95f, 0),
            new Vector3(-4.9f, -2.45f, 0),
            new Vector3(-6.4f, -2.45f, 0),
            new Vector3(-6.4f, -0.95f, 0)
        };
        //洗面室を描画  
        cr.createRoom("Bathroom", bathroomPositions);

        //キッチンを作る線が通る点の座標
        Vector3[] kitchen1Positions = new Vector3[]{
            new Vector3(-4.9f, -0.95f, 0),
            new Vector3(-3.2f, -0.95f, 0),
            new Vector3(-3.2f, -3.35f, 0),
            new Vector3(-5.6f, -3.35f, 0),
            new Vector3(-5.6f, -2.45f, 0),
            new Vector3(-4.9f, -2.45f, 0),
            new Vector3(-4.9f, -0.95f, 0)
        };
        //キッチンを描画  
        cr.createRoom("K", kitchen1Positions);

        //キッチンを作る線が通る点の座標
        Vector3[] kitchen2Positions = new Vector3[]{
            new Vector3(-4.9f, -0.95f, 0),
            new Vector3(-3.2f, -0.95f, 0),
            new Vector3(-3.2f, -1.55f, 0),
            new Vector3(-4.9f, -1.55f, 0),
            new Vector3(-4.9f, -0.95f, 0)
        };
        //キッチンを描画  
        cr.createRoom("K", kitchen2Positions);
    }
}
