using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackingTest : CreateRoom
{
    [SerializeField] Packing pack;
    [SerializeField] CreateRoom cr;

    void Start()
    {
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
        //JudgeInscribedテスト
        Vector3[] test = new Vector3[]{new Vector3(150, 1850, 0), new Vector3(2050, 1850, 0), new Vector3(2050, 450, 0), new Vector3(150, 450, 0)};
        
        Debug.Log(pack.JudgeInscribed(range, test));
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
