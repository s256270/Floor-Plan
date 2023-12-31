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

    /*
    public void planMake()
    {
        //1LDKの作成
        Vector3[] ldk1 = new Vector3[]{
            new Vector3(-8300, -950, 0),
            new Vector3(-1100, -950, 0),
            new Vector3(-1100, -6050, 0),
            new Vector3(-8300, -6050, 0)
        };
        createRoom("1LDK(36.72m^2)", ldk1);
        plan.Add(GameObject.Find("1LDK(36.72m^2)"));

        //1LDKの作成
        Vector3[] ldk2 = new Vector3[]{
            new Vector3(-8300, 6050, 0),
            new Vector3(-5600, 6050, 0),
            new Vector3(-5600, 4850, 0),
            new Vector3(-2600, 4850, 0),
            new Vector3(-2600, -950, 0),
            new Vector3(-8300, -950, 0)
        };
        createRoom("1LDK(36.30m^2)", ldk2);
        plan.Add(GameObject.Find("1LDK(36.30m^2)"));

        //1LDKの作成
        Vector3[] ldk3 = new Vector3[]{
            new Vector3(-1100, -950, 0),
            new Vector3(7300, -950, 0),
            new Vector3(7300, -4850, 0),
            new Vector3(-1100, -4850, 0)
        };
        createRoom("1LDK(32.76m^2)", ldk3);
        plan.Add(GameObject.Find("1LDK(32.76m^2)"));

        //1Kの作成
        Vector3[] k = new Vector3[]{
            new Vector3(400, 4850, 0),
            new Vector3(2300, 4850, 0),
            new Vector3(2300, 2850, 0),
            new Vector3(7300, 2850, 0),
            new Vector3(7300, -950, 0),
            new Vector3(400, -950, 0)
        };
        createRoom("1K(30.02m^2)", k);
        plan.Add(GameObject.Find("1K(30.02m^2)"));

        //階段室の作成
        Vector3[] stairs = new Vector3[]{
            new Vector3(-2600, 4850, 0),
            new Vector3(400, 4850, 0),
            new Vector3(400, -950, 0),
            new Vector3(-2600, -950, 0)
        };
        createRoom("Stairs", stairs);
        plan.Add(GameObject.Find("Stairs"));

        //バルコニーの作成
        Vector3[] balcony1 = new Vector3[]{
            new Vector3(7300, 2850, 0),
            new Vector3(8300, 2850, 0),
            new Vector3(8300, -950, 0),
            new Vector3(7300, -950, 0)
        };
        createRoom("Balcony1", balcony1);
        plan.Add(GameObject.Find("Balcony1"));

        //バルコニーの作成
        Vector3[] balcony2 = new Vector3[]{
            new Vector3(7300, -950, 0),
            new Vector3(8300, -950, 0),
            new Vector3(8300, -4850, 0),
            new Vector3(7300, -4850, 0)
        };
        createRoom("Balcony2", balcony2);
        plan.Add(GameObject.Find("Balcony2"));

        //バルコニーの作成
        Vector3[] balcony3 = new Vector3[]{
            new Vector3(-1100, -4850, 0),
            new Vector3(1900, -4850, 0),
            new Vector3(1900, -6050, 0),
            new Vector3(-1100, -6050, 0)
        };
        createRoom("Balcony3", balcony3);
        plan.Add(GameObject.Find("Balcony3"));

        //バルコニーの作成
        Vector3[] balcony4 = new Vector3[]{
            new Vector3(-5600, 6050, 0),
            new Vector3(-2600, 6050, 0),
            new Vector3(-2600, 4850, 0),
            new Vector3(-5600, 4850, 0)
        };
        createRoom("Balcony4", balcony4);
        plan.Add(GameObject.Find("Balcony4"));

        //駐輪場の作成
        Vector3[] bicycleParking = new Vector3[]{
            new Vector3(2300, 4850, 0),
            new Vector3(7300, 4850, 0),
            new Vector3(7300, 2850, 0),
            new Vector3(2300, 2850, 0)
        };
        createRoom("Bicycle Parking", bicycleParking);
        plan.Add(GameObject.Find("Bicycle Parking"));
    }
    */

    /*
    //配置結果確認用
    public void Check(Dictionary<string, Dictionary<string, Vector3[]>> pattern) {
        foreach (KeyValuePair<string, Dictionary<string, Vector3[]>> space in pattern) {
            Debug.Log(space.Key + "の要素");
            foreach (KeyValuePair<string, Vector3[]> spaceElements in space.Value) {
                Debug.Log("スペース名: " + spaceElements.Key);

                for (int i = 0; i < spaceElements.Value.Length; i++) {
                    Debug.Log("座標" + i + ": " + spaceElements.Value[i]);
                }
            }
        }
    }
    */
}
