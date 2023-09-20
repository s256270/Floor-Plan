using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* プラン作成で完成したものを表示する */
public class PlanMaker : MonoBehaviour
{    
    //プラン作成時の部屋を管理するリスト
    public List<Dictionary<string, Vector3[]>> plan = new List<Dictionary<string, Vector3[]>>() {
        //階段室
        new Dictionary<string, Vector3[]>() {
            {
                "Stairs",
                new Vector3[]{
                    new Vector3(-1850, 3150, 0),
                    new Vector3(1150, 3150, 0),
                    new Vector3(1150, -2550, 0),
                    new Vector3(-1850, -2550, 0)
                }
            }
        },

        //1K
        new Dictionary<string, Vector3[]>() {
            {
                "1K(29.64m^2)",
                new Vector3[]{
                    new Vector3(-9650, 3650, 0),
                    new Vector3(-1850, 3650, 0),
                    new Vector3(-1850, -150, 0),
                    new Vector3(-9650, -150, 0)
                }
            },

            {
                "Balconyof1K(29.64m^2)",
                new Vector3[]{
                    new Vector3(-10650, 2850, 0),
                    new Vector3(-9650, 2850, 0),
                    new Vector3(-9650, -150, 0),
                    new Vector3(-10650, -150, 0)
                }
            }
        },

        //1K
        new Dictionary<string, Vector3[]>() {
            {
                "1K(30.05m^2)",
                new Vector3[]{
                    new Vector3(1150, 3150, 0),
                    new Vector3(9650, 3150, 0),
                    new Vector3(9650, -150, 0),
                    new Vector3(2150, -150, 0),
                    new Vector3(2150, -2200, 0),
                    new Vector3(1150, -2200, 0)
                }
            },

            {
                "Balconyof1K(30.05m^2)",
                new Vector3[]{
                    new Vector3(9650, 3150, 0),
                    new Vector3(10650, 3150, 0),
                    new Vector3(10650, -150, 0),
                    new Vector3(9650, -150, 0)
                }
            }
        },

        //1K
        new Dictionary<string, Vector3[]>() {
            {
                "1K(28.02m^2)",
                new Vector3[]{
                    new Vector3(-9650, -150, 0),
                    new Vector3(-1850, -150, 0),
                    new Vector3(-1850, -2550, 0),
                    new Vector3(-350, -2550, 0),
                    new Vector3(-350, -3550, 0),
                    new Vector3(-9650, -3550, 0)
                }
            },

            {
                "Balconyof1K(28.02m^2)",
                new Vector3[]{
                    new Vector3(-9650, -150, 0),
                    new Vector3(-10650, -150, 0),
                    new Vector3(-10650, -3550, 0),
                    new Vector3(-9650, -3550, 0)
                }
            }
        },

        //1K
        new Dictionary<string, Vector3[]>() {
            {
                "1K(28.40m^2)",
                new Vector3[]{
                    new Vector3(-350, -2550, 0),
                    new Vector3(1150, -2550, 0),
                    new Vector3(1150, -2200, 0),
                    new Vector3(2150, -2200, 0),
                    new Vector3(2150, -150, 0),
                    new Vector3(9650, -150, 0),
                    new Vector3(9650, -3550, 0),
                    new Vector3(-350, -3550, 0)
                }
            },

            {
                "Balconyof1K(28.40m^2)",
                new Vector3[]{
                    new Vector3(9650, -150, 0),
                    new Vector3(10650, -150, 0),
                    new Vector3(10650, -3550, 0),
                    new Vector3(9650, -3550, 0)
                }
            }
        }
    };
    
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
}
