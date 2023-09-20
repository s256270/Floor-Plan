using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PackingTest : CreateRoom
{
    [SerializeField] Packing pack;
    [SerializeField] CreateRoom cr;

    void Start()
    {
        /*
        //要らない座標を取り除けるかテスト
        Vector3[] polygon = new Vector3[]{new Vector3(-3400, 1900, 0), new Vector3(-2300, 1900, 0), new Vector3(-2300, 1200, 0), new Vector3(100, 1200, 0), new Vector3(100, 500, 0), new Vector3(1000, 500, 0), new Vector3(2800, 500, 0), new Vector3(2800, 300, 0), new Vector3(4400, 300, 0), new Vector3(4400, -50, 0), new Vector3(3400, -50, 0), new Vector3(3400, -1900, 0), new Vector3(-3400, -1900, 0)};

        List<Vector3> polygonList = polygon.ToList();

        for (int i = 0; i < polygonList.Count; i++) {
            //調べる辺
            Vector3[] sideA = new Vector3[]{polygonList[(i+1)%polygonList.Count], polygonList[i]};
            Vector3[] sideB = new Vector3[]{polygonList[(i+1)%polygonList.Count], polygonList[(i+2)%polygonList.Count]};

            //座標配列で連続する2辺の内積によって調べる
            Debug.Log(Vector3.Dot(sideA[1] - sideA[0], sideB[1] - sideB[0]));
            if (Vector3.Dot(sideA[1] - sideA[0], sideB[1] - sideB[0]) == -Vector3.Distance(sideA[1], sideA[0]) * Vector3.Distance(sideB[1], sideB[0])) {
                //座標配列から削除
                polygonList.Remove(sideA[0]);
                i = -1;
            }
        }

        createRoom("polygon", polygonList.ToArray(), Color.red);
        */



        /*
        //CrossJudgeテスト
        Vector3[] lineA = new Vector3[]{new Vector3(-2000, 1900, 0), new Vector3(1000, 1900, 0)};
        Vector3[] lineB = new Vector3[]{new Vector3(1000, 1000, 0), new Vector3(1000, 2000, 0)};
        Debug.Log(pack.CrossJudge(lineA, lineB));
        */

        //OnLineSegmentテスト
        /*
        Vector3[] lineSegment = new Vector3[]{new Vector3(-1000, 00, 0), new Vector3(1000, 0, 0)};
        Vector3 point = new Vector3(1000, 0, 0);
        Debug.Log(pack.OnLineSegment(lineSegment, point));
        */

        /*
        //CheckPointテスト
        Vector3[] polygon = new Vector3[]{new Vector3(-2000, 1900, 0), new Vector3(1000, 1900, 0), new Vector3(1000, 0, 0), new Vector3(-2000, 0, 0)};
        Vector3 point = new Vector3(-2000, 1900, 0);
        createRoom("polygon", polygon);
        Debug.Log(pack.CheckPoint(polygon, point));
        */

        /*
        //flagPatternテスト
        List<int[]> test = pack.flagPatternList(4);
        for (int i = 0; i < test.Count; i++) {
            Debug.Log("パターン" + (i+1));
            for (int j = 0; j < test[i].Length; j++) {
                Debug.Log(test[i][j]);
            }
        }
        */
                
        /*
        //JudgeInsideテスト
        Vector3[] range = new Vector3[]{new Vector3(-3400, 1900, 0), new Vector3(4400, 1900, 0), new Vector3(4400, -50, 0), new Vector3(3400, -50, 0), new Vector3(3400, -1900, 0), new Vector3(-3400, -1900, 0)};
        Vector3[] kitchen = new Vector3[]{new Vector3(3700, 2350, 0), new Vector3(4400, 2350, 0), new Vector3(4400, -50, 0), new Vector3(3700, -50, 0)};
        createRoom("range", range, Color.red);
        createRoom("kitchen", kitchen, Color.gray);
        
        Debug.Log(pack.JudgeInside(range, kitchen));
        */
        
        /*
        //FrameSliceテスト
        List<Vector3[]> test = pack.FrameSlice(pack.range, new Vector3[]{new Vector3(-2050, 1850, 0), new Vector3(1050, 1850, 0)}, new Vector3[]{new Vector3(1050, 350, 0), new Vector3(-2050, -1850, 0)});
        
        Debug.Log("A");
        for (int i = 0; i < test[0].Length; i++) {
            Debug.Log(test[0][i]);
        }
        Debug.Log("B");
        for (int i = 0; i < test[1].Length; i++) {
            Debug.Log(test[1][i]);
        }
        */
        
        /*
        //PointAddテスト
        Vector3 testPoint = new Vector3(1050, -1550, 0);
        for (int i = 0; i < pack.PointAdd(pack.range, testPoint).Length; i++) {
            Debug.Log(pack.PointAdd(pack.range, testPoint)[i]);
        }
        */
        
        /*
        //FrameChangeテスト
        Vector3[] test = new Vector3[]{new Vector3(-2050, 1850, 0), new Vector3(-500, 1850, 0), new Vector3(-500, 1350, 0), new Vector3(-1000, 1350, 0), new Vector3(-1000, 1000, 0), new Vector3(-2050, 1000, 0)};
        createRoom("test", test);
        for (int i = 0; i < pack.FrameChange(pack.range, test).Length; i++) {
            Debug.Log(pack.FrameChange(pack.range, test)[i]);
        }
        */

        /*
        //AllPermutationテスト
        int[] roomsIndex = new int[]{0, 1, 2, 3};

        Debug.Log("パターン数:" + pack.AllPermutation(roomsIndex).Count);
        for (int i = 0; i < pack.AllPermutation(roomsIndex).Count; i++) {
            Debug.Log("---");
            for (int j = 0; j < pack.AllPermutation(roomsIndex)[i].Length; j++) {
                Debug.Log(pack.AllPermutation(roomsIndex)[i][j]);
            }
        }
        Debug.Log("---");
        */
    }

}
