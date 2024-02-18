using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* プラン作成で完成したものを表示する */
public class PlanReader : MonoBehaviour
{    
    //プラン作成時の部屋を管理するリスト
    public Dictionary<string, Dictionary<string, Vector3[]>> plan = new Dictionary<string, Dictionary<string, Vector3[]>>() {
        //階段室
        {
            "Stairs",
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
            }
        },

        //住戸1
        {
            "Dwelling1",
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
            }
        },

        //住戸2
        {
            "Dwelling2",
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
            }
        },

        //住戸3
        {
            "Dwelling3",
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
            }
        },

        //住戸4
        {
            "Dwelling4",
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
        }
    };

    // public Dictionary<string, Dictionary<string, Vector3[]>> plan = new Dictionary<string, Dictionary<string, Vector3[]>>() {
    //     //階段室
    //     {
    //         "Stairs",
    //         new Dictionary<string, Vector3[]>() {
    //             {
    //                 "Stairs",
    //                 new Vector3[]{
    //                     new Vector3(-1650, 1800, 0),
    //                     new Vector3(1350, 1800, 0),
    //                     new Vector3(1350, -4200, 0),
    //                     new Vector3(-1650, -4200, 0)
                        
    //                 }
    //             }
    //         }
    //     },

    //     //住戸1
    //     {
    //         "Dwelling1",
    //         new Dictionary<string, Vector3[]>() {
    //             {
    //                 "1K",
    //                 new Vector3[]{
    //                     new Vector3(-6950, 6300, 0),
    //                     new Vector3(-150, 6300, 0),
    //                     new Vector3(-150, 1800, 0),
    //                     new Vector3(-6950, 1800, 0)
    //                 }
    //             },

    //             {
    //                 "Balcony",
    //                 new Vector3[]{
    //                     new Vector3(-6950, 1800, 0),
    //                     new Vector3(-5750, 1800, 0),
    //                     new Vector3(-5750, -1200, 0),
    //                     new Vector3(-6950, -1200, 0)
    //                 }
    //             }
    //         }
    //     },

    //     //住戸2
    //     {
    //         "Dwelling2",
    //         new Dictionary<string, Vector3[]>() {
    //             {
    //                 "1K",
    //                 new Vector3[]{
    //                     new Vector3(-150, 6300, 0),
    //                     new Vector3(6950, 6300, 0),
    //                     new Vector3(6950, 4800, 0),
    //                     new Vector3(5750, 4800, 0),
    //                     new Vector3(5750, 1800, 0),
    //                     new Vector3(-150, 1800, 0)
    //                 }
    //             },

    //             {
    //                 "Balcony",
    //                 new Vector3[]{
    //                     new Vector3(5750, 4800, 0),
    //                     new Vector3(6950, 4800, 0),
    //                     new Vector3(6950, 1800, 0),
    //                     new Vector3(5750, 1800, 0)
    //                 }
    //             }
    //         }
    //     },


    //     //住戸3
    //     {
    //         "Dwelling3",
    //         new Dictionary<string, Vector3[]>() {
    //             {
    //                 "1K",
    //                 new Vector3[]{
    //                     new Vector3(-5750, 1800, 0),
    //                     new Vector3(-1650, 1800, 0),
    //                     new Vector3(-1650, -5100, 0),
    //                     new Vector3(-5750, -5100, 0)
    //                 }
    //             },

    //             {
    //                 "Balcony",
    //                 new Vector3[]{
    //                     new Vector3(-4650, -5100, 0),
    //                     new Vector3(-1650, -5100, 0),
    //                     new Vector3(-1650, -6300, 0),
    //                     new Vector3(-4650, -6300, 0)
    //                 }
    //             }
    //         }
    //     },

    //     //住戸4
    //     {
    //         "Dwelling4",
    //         new Dictionary<string, Vector3[]>() {
    //             {
    //                 "1K",
    //                 new Vector3[]{
    //                     new Vector3(1350, 1800, 0),
    //                     new Vector3(5750, 1800, 0),
    //                     new Vector3(5750, -1200, 0),
    //                     new Vector3(6950, -1200, 0),
    //                     new Vector3(6950, -4200, 0),
    //                     new Vector3(1350, -4200, 0)
    //                 }
    //             },

    //             {
    //                 "Balcony",
    //                 new Vector3[]{
    //                     new Vector3(5750, 1800, 0),
    //                     new Vector3(6950, 1800, 0),
    //                     new Vector3(6950, -1200, 0),
    //                     new Vector3(5750, -1200, 0)
    //                 }
    //             }
    //         }
    //     }
    // };

    // public Dictionary<string, Dictionary<string, Vector3[]>> plan = new Dictionary<string, Dictionary<string, Vector3[]>>() {
    //     //階段室
    //     {
    //         "Stairs",
    //         new Dictionary<string, Vector3[]>() {
    //             {
    //                 "Stairs",
    //                 new Vector3[]{
    //                     new Vector3(-4200, 1650, 0),
    //                     new Vector3(1800, 1650, 0),
    //                     new Vector3(1800, -1350, 0),
    //                     new Vector3(-4200, -1350, 0)
                        
    //                 }
    //             }
    //         }
    //     },

    //     //住戸1
    //     {
    //         "Dwelling1",
    //         new Dictionary<string, Vector3[]>() {
    //             {
    //                 "1K",
    //                 new Vector3[]{
    //                     new Vector3(-5100, 5750, 0),
    //                     new Vector3(1800, 5750, 0),
    //                     new Vector3(1800, 1650, 0),
    //                     new Vector3(-5100, 1650, 0)
    //                 }
    //             },

    //             {
    //                 "Balcony",
    //                 new Vector3[]{
    //                     new Vector3(-6300, 4650, 0),
    //                     new Vector3(-5100, 4650, 0),
    //                     new Vector3(-5100, 1650, 0),
    //                     new Vector3(-6300, 1650, 0)
    //                 }
    //             }
    //         }
    //     },

    //     //住戸2
    //     {
    //         "Dwelling2",
    //         new Dictionary<string, Vector3[]>() {
    //             {
    //                 "1K",
    //                 new Vector3[]{
    //                     new Vector3(1800, 6950, 0),
    //                     new Vector3(6300, 6950, 0),
    //                     new Vector3(6300, 150, 0),
    //                     new Vector3(1800, 150, 0)
    //                 }
    //             },

    //             {
    //                 "Balcony",
    //                 new Vector3[]{
    //                     new Vector3(-1200, 6950, 0),
    //                     new Vector3(1800, 6950, 0),
    //                     new Vector3(1800, 5750, 0),
    //                     new Vector3(-1200, 5750, 0)
    //                 }
    //             }
    //         }
    //     },


    //     //住戸3
    //     {
    //         "Dwelling3",
    //         new Dictionary<string, Vector3[]>() {
    //             {
    //                 "1K",
    //                 new Vector3[]{
    //                     new Vector3(-4200, -1350, 0),
    //                     new Vector3(1800, -1350, 0),
    //                     new Vector3(1800, -5750, 0),
    //                     new Vector3(-1200, -5750, 0),
    //                     new Vector3(-1200, -6950, 0),
    //                     new Vector3(-4200, -6950, 0)
    //                 }
    //             },

    //             {
    //                 "Balcony",
    //                 new Vector3[]{
    //                     new Vector3(-1200, -5750, 0),
    //                     new Vector3(1800, -5750, 0),
    //                     new Vector3(1800, -6950, 0),
    //                     new Vector3(-1200, -6950, 0)
    //                 }
    //             }
    //         }
    //     },

    //     //住戸4
    //     {
    //         "Dwelling4",
    //         new Dictionary<string, Vector3[]>() {
    //             {
    //                 "1K",
    //                 new Vector3[]{
    //                     new Vector3(1800, 150, 0),
    //                     new Vector3(6300, 150, 0),
    //                     new Vector3(6300, -6950, 0),
    //                     new Vector3(4800, -6950, 0),
    //                     new Vector3(4800, -5750, 0),
    //                     new Vector3(1800, -5750, 0)
    //                 }
    //             },

    //             {
    //                 "Balcony",
    //                 new Vector3[]{
    //                     new Vector3(1800, -5750, 0),
    //                     new Vector3(4800, -5750, 0),
    //                     new Vector3(4800, -6950, 0),
    //                     new Vector3(1800, -6950, 0)
    //                 }
    //             },

    //             {
    //                 "Mbps",
    //                 new Vector3[]{
    //                     new Vector3(1800, 150, 0),
    //                     new Vector3(2900, 150, 0),
    //                     new Vector3(2900, -200, 0),
    //                     new Vector3(1800, -200, 0)
    //                 }
    //             },

    //             {
    //                 "Entrance",
    //                 new Vector3[]{
    //                     new Vector3(1800, -200, 0),
    //                     new Vector3(2900, -200, 0),
    //                     new Vector3(2900, -1350, 0),
    //                     new Vector3(1800, -1350, 0)
    //                 }
    //             }
    //         }
    //     }
    // };

    public Vector3[] stairsCoordinates;
    public List<Vector3[]> dwellingCoordinates;

    /// <summary>
    /// 階段室の座標の取得
    /// </summary> 
    public Vector3[] getStairsCoordinates() {
        stairsCoordinates = plan["Stairs"]["Stairs"];

        return stairsCoordinates;
    }

    /// <summary>
    /// 住戸の座標のリストを作成
    /// </summary> 
    public List<Vector3[]> getDwellingCoordinatesList() {
        //作成結果
        var result = new List<Vector3[]>();

        foreach (KeyValuePair<string, Dictionary<string, Vector3[]>> planParts in plan) {
            if (planParts.Key.Contains("Dwelling")) {
                foreach (KeyValuePair<string, Vector3[]> planPartsElements in planParts.Value) {
                    if (planPartsElements.Key.Contains("1K")) {
                        result.Add(planPartsElements.Value);
                    }
                }
            }
        }

        return result;
    }
}
