using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* プラン作成で完成したものを表示する */
public class PlanReader : MonoBehaviour
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

        //住戸1
        new Dictionary<string, Vector3[]>() {
            {
                "1K",
                new Vector3[]{
                    new Vector3(-9650, 3650, 0),
                    new Vector3(-1850, 3650, 0),
                    new Vector3(-1850, -150, 0),
                    new Vector3(-9650, -150, 0)
                }
            },

            {
                "Balcony",
                new Vector3[]{
                    new Vector3(-10650, 2850, 0),
                    new Vector3(-9650, 2850, 0),
                    new Vector3(-9650, -150, 0),
                    new Vector3(-10650, -150, 0)
                }
            }
        },

        //住戸2
        new Dictionary<string, Vector3[]>() {
            {
                "1K",
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
                "Balcony",
                new Vector3[]{
                    new Vector3(9650, 3150, 0),
                    new Vector3(10650, 3150, 0),
                    new Vector3(10650, -150, 0),
                    new Vector3(9650, -150, 0)
                }
            }
        },

        //住戸3
        new Dictionary<string, Vector3[]>() {
            {
                "1K",
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
                "Balcony",
                new Vector3[]{
                    new Vector3(-9650, -150, 0),
                    new Vector3(-10650, -150, 0),
                    new Vector3(-10650, -3550, 0),
                    new Vector3(-9650, -3550, 0)
                }
            }
        },

        //住戸4
        new Dictionary<string, Vector3[]>() {
            {
                "1K",
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
                "Balcony",
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
    //旧プラン作成時の部屋を管理するリスト
    public List<GameObject> plan2 = new List<GameObject>();

    public void planMake2()
    {
        //1Kの作成
        Vector3[] k1 = new Vector3[]{
            new Vector3(-9650, 3650, 0),
            new Vector3(-1850, 3650, 0),
            new Vector3(-1850, -150, 0),
            new Vector3(-9650, -150, 0)
        };
        createRoom("1K(29.64m^2)", k1);
        plan2.Add(GameObject.Find("1K(29.64m^2)"));

        //1Kの作成
        Vector3[] k2 = new Vector3[]{
            new Vector3(1150, 3150, 0),
            new Vector3(9650, 3150, 0),
            new Vector3(9650, -150, 0),
            new Vector3(2150, -150, 0),
            new Vector3(2150, -2200, 0),
            new Vector3(1150, -2200, 0)
        };
        createRoom("1K(30.05m^2)", k2);
        plan2.Add(GameObject.Find("1K(30.05m^2)"));

        //1Kの作成
        Vector3[] k3 = new Vector3[]{
            new Vector3(-9650, -150, 0),
            new Vector3(-1850, -150, 0),
            new Vector3(-1850, -2550, 0),
            new Vector3(-350, -2550, 0),
            new Vector3(-350, -3550, 0),
            new Vector3(-9650, -3550, 0)
        };
        createRoom("1K(28.02m^2)", k3);
        plan2.Add(GameObject.Find("1K(28.02m^2)"));

        //1Kの作成
        Vector3[] k4 = new Vector3[]{
            new Vector3(-350, -2550, 0),
            new Vector3(1150, -2550, 0),
            new Vector3(1150, -2200, 0),
            new Vector3(2150, -2200, 0),
            new Vector3(2150, -150, 0),
            new Vector3(9650, -150, 0),
            new Vector3(9650, -3550, 0),
            new Vector3(-350, -3550, 0)
        };
        createRoom("1K(28.40m^2)", k4);
        plan2.Add(GameObject.Find("1K(28.40m^2)"));

        //階段室の作成
        Vector3[] stairs = new Vector3[]{
            new Vector3(-1850, 3150, 0),
            new Vector3(1150, 3150, 0),
            new Vector3(1150, -2550, 0),
            new Vector3(-1850, -2550, 0)
        };
        createRoom("Stairs", stairs);
        plan2.Add(GameObject.Find("Stairs"));

        //バルコニーの作成
        Vector3[] balcony1 = new Vector3[]{
            new Vector3(-10650, 2850, 0),
            new Vector3(-9650, 2850, 0),
            new Vector3(-9650, -150, 0),
            new Vector3(-10650, -150, 0)
        };
        createRoom("Balcony1", balcony1);
        plan2.Add(GameObject.Find("Balcony1"));

        //バルコニーの作成
        Vector3[] balcony2 = new Vector3[]{
            new Vector3(9650, 3150, 0),
            new Vector3(10650, 3150, 0),
            new Vector3(10650, -150, 0),
            new Vector3(9650, -150, 0)
        };
        createRoom("Balcony2", balcony2);
        plan2.Add(GameObject.Find("Balcony2"));

        //バルコニーの作成
        Vector3[] balcony3 = new Vector3[]{
            new Vector3(-9650, -150, 0),
            new Vector3(-10650, -150, 0),
            new Vector3(-10650, -3550, 0),
            new Vector3(-9650, -3550, 0)
        };
        createRoom("Balcony3", balcony3);
        plan2.Add(GameObject.Find("Balcony3"));

        //バルコニーの作成
        Vector3[] balcony4 = new Vector3[]{
            new Vector3(9650, -150, 0),
            new Vector3(10650, -150, 0),
            new Vector3(10650, -3550, 0),
            new Vector3(9650, -3550, 0)
        };
        createRoom("Balcony4", balcony4);
        plan2.Add(GameObject.Find("Balcony4"));
    }
    */
}
