using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parts : CreateRoom
{
    //2住戸のMBPSの座標
    [HideInInspector]
    public List<List<Vector3[]>> twoDwellingsMbpsCoordinatesList = new List<List<Vector3[]>>() {
        //その1
        new List<Vector3[]>(){
            //左側
            new Vector3[]{
                new Vector3(-175, 500, 0),
                new Vector3(175, 500, 0),
                new Vector3(175, -500, 0),
                new Vector3(-175, -500, 0)
            },
            //右側
            new Vector3[]{
                new Vector3(-175, 500, 0),
                new Vector3(175, 500, 0),
                new Vector3(175, -500, 0),
                new Vector3(-175, -500, 0)
            }
        },

        //その2
        new List<Vector3[]>(){
            //左側
            new Vector3[]{
                new Vector3(-525, 175, 0),
                new Vector3(525, 175, 0),
                new Vector3(525, -175, 0),
                new Vector3(-525, -175, 0)
            },
            //右側
            new Vector3[]{
                new Vector3(-175, 525, 0),
                new Vector3(175, 525, 0),
                new Vector3(175, -525, 0),
                new Vector3(-175, -525, 0)
            }
        },

        //その3
        new List<Vector3[]>(){
            //左側
            new Vector3[]{
                new Vector3(-350, 350, 0),
                new Vector3(350, 350, 0),
                new Vector3(350, -350, 0),
                new Vector3(-350, -350, 0)
            },
            //右側
            new Vector3[]{
                new Vector3(-350, 350, 0),
                new Vector3(350, 350, 0),
                new Vector3(350, -350, 0),
                new Vector3(-350, -350, 0)
            }
        }
    };
    
    //1住戸のMBPSの座標
    [HideInInspector]
    public List<Vector3[]> oneDwellingsMbpsCoordinatesList = new List<Vector3[]>() {
        //その1
        new Vector3[]{
            new Vector3(-350, 350, 0),
            new Vector3(350, 350, 0),
            new Vector3(350, -350, 0),
            new Vector3(-350, -350, 0)
        },

        //その2
        new Vector3[]{
            new Vector3(-700, 200, 0),
            new Vector3(700, 200, 0),
            new Vector3(700, -200, 0),
            new Vector3(-700, -200, 0)
        },

        //その3
        new Vector3[]{
            new Vector3(-350, 500, 0),
            new Vector3(350, 500, 0),
            new Vector3(350, -500, 0),
            new Vector3(0, -500, 0),
            new Vector3(0, 200, 0),
            new Vector3(-350, 200, 0)
        }
    };

    //玄関の座標
    [HideInInspector]
    public List<Vector3[]> entranceCoordinatesList = new List<Vector3[]>(){
        //その1
        new Vector3[]{
            new Vector3(-750, 500, 0),
            new Vector3(750, 500, 0),
            new Vector3(750, -500, 0),
            new Vector3(-750, -500, 0)
        }
    };

    //洗面室の座標
    //その1
    [HideInInspector]
    public List<Vector3[]> washroomCoordinatesList = new List<Vector3[]>(){
        //その1
        new Vector3[]{
            new Vector3(-800, 800, 0),
            new Vector3(800, 800, 0),
            new Vector3(800, -800, 0),
            new Vector3(-800, -800, 0)
        },

        //その2
        new Vector3[]{
            new Vector3(-1300, 450, 0),
            new Vector3(1300, 450, 0),
            new Vector3(1300, -450, 0),
            new Vector3(-1300, -450, 0)
        },

        //その3
        new Vector3[]{
            new Vector3(-700, 1200, 0),
            new Vector3(200, 1200, 0),
            new Vector3(200, -300, 0),
            new Vector3(700, -300, 0),
            new Vector3(700, -1200, 0),
            new Vector3(-700, -1200, 0)
        }
    };

    //UBの座標
    [HideInInspector]
    public List<Vector3[]> ubCoordinatesList = new List<Vector3[]>(){
        //その1
        new Vector3[]{
            new Vector3(-900, 700, 0),
            new Vector3(900, 700, 0),
            new Vector3(900, -700, 0),
            new Vector3(-900, -700, 0)
        }
    };

    //洗面室&UBの座標
    // [HideInInspector]
    // public List<Vector3[]> washroomAndUbCoordinates = new List<Vector3[]>(){
    //     //その1
    //     new Vector3[]{
    //         new Vector3(-800, 800, 0),
    //         new Vector3(800, 800, 0),
    //         new Vector3(800, -800, 0),
    //         new Vector3(-800, -800, 0)
    //     },
    // };

    //トイレの座標
    [HideInInspector]
    public List<Vector3[]> toiletCoordinatesList = new List<Vector3[]>(){
        //その1
        new Vector3[]{
            new Vector3(-450, 700, 0),
            new Vector3(450, 700, 0),
            new Vector3(450, -700, 0),
            new Vector3(-450, -700, 0)
        }
    };

    //キッチンの座標
    [HideInInspector]
    public List<Vector3[]> kitchenCoordinatesList = new List<Vector3[]>(){
        //その1
        new Vector3[]{
            new Vector3(-1200, 350, 0),
            new Vector3(1200, 350, 0),
            new Vector3(1200, -350, 0),
            new Vector3(-1200, -350, 0)
        },

        //その2
        new Vector3[]{
            new Vector3(-850, 350, 0),
            new Vector3(850, 350, 0),
            new Vector3(850, -350, 0),
            new Vector3(-850, -350, 0)
        }
    };
}
