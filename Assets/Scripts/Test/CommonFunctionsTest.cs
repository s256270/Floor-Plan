using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonFunctionsTest : MonoBehaviour
{
    [SerializeField] CommonFunctions cf;

    void Start()
    {
        //ThreePointStraightJudgeTest();
        //LinePositionRelationTest();
    }

    /// <summary>
    /// ThreePointStraightJudgeの動作テスト
    /// </summary>
    public void ThreePointStraightJudgeTest() {
        //Trueになって欲しい
        Debug.Log("expect: True, result: " + cf.ThreePointStraightJudge(new Vector3(-5, -5, 0), new Vector3(0, 0, 0), new Vector3(2, 2, 0)));
        Debug.Log("expect: True, result: " + cf.ThreePointStraightJudge(new Vector3(-5, -5, 0), new Vector3(2, 2, 0), new Vector3(0, 0, 0)));
        Debug.Log("expect: True, result: " + cf.ThreePointStraightJudge(new Vector3(0, 0, 0), new Vector3(2, 2, 0), new Vector3(-5, -5, 0)));

        //falseになって欲しい
        Debug.Log("expect: False, result: " + cf.ThreePointStraightJudge(new Vector3(-5, -5, 0), new Vector3(0, 0, 0), new Vector3(2, 3, 0)));
    }

    /// <summary>
    /// LinePositionRelationの動作テスト
    /// </summary>
    public void LinePositionRelationTest() {
        //not straightになって欲しい
        // Debug.Log("expect: not straight, result: " + cf.LinePositionRelation(new Vector3[] {new Vector3(-5, -5, 0), new Vector3(5, 5, 0)}, new Vector3[] {new Vector3(-5, 0, 0), new Vector3(5, 10, 0)}));

        //matchになって欲しい
        // Debug.Log("expect: match, result: " + cf.LinePositionRelation(new Vector3[] {new Vector3(-5, -5, 0), new Vector3(5, 5, 0)}, new Vector3[] {new Vector3(-5, -5, 0), new Vector3(5, 5, 0)}));
        // Debug.Log("expect: match, result: " + cf.LinePositionRelation(new Vector3[] {new Vector3(0, -5, 0), new Vector3(0, 5, 0)}, new Vector3[] {new Vector3(0, -5, 0), new Vector3(0, 5, 0)}));
        // Debug.Log("expect: match, result: " + cf.LinePositionRelation(new Vector3[] {new Vector3(5, 5, 0), new Vector3(-5, -5, 0)}, new Vector3[] {new Vector3(5, 5, 0), new Vector3(-5, -5, 0)}));
        // Debug.Log("expect: match, result: " + cf.LinePositionRelation(new Vector3[] {new Vector3(0, 5, 0), new Vector3(0, -5, 0)}, new Vector3[] {new Vector3(0, 5, 0), new Vector3(0, -5, 0)}));

        //point overlapになって欲しい
        // Debug.Log("expect: point overlap, result: " + cf.LinePositionRelation(new Vector3[] {new Vector3(-5, -5, 0), new Vector3(0, 0, 0)}, new Vector3[] {new Vector3(0, 0, 0), new Vector3(5, 5, 0)}));
        // Debug.Log("expect: point overlap, result: " + cf.LinePositionRelation(new Vector3[] {new Vector3(0, 0, 0), new Vector3(5, 5, 0)}, new Vector3[] {new Vector3(-5, -5, 0), new Vector3(0, 0, 0)}));
        // Debug.Log("expect: point overlap, result: " + cf.LinePositionRelation(new Vector3[] {new Vector3(0, -5, 0), new Vector3(0, 0, 0)}, new Vector3[] {new Vector3(0, 0, 0), new Vector3(0, 5, 0)}));
        // Debug.Log("expect: point overlap, result: " + cf.LinePositionRelation(new Vector3[] {new Vector3(0, 0, 0), new Vector3(0, 5, 0)}, new Vector3[] {new Vector3(0, -5, 0), new Vector3(0, 0, 0)}));

        //includeになって欲しい
        // Debug.Log("expect: include, result: " + cf.LinePositionRelation(new Vector3[] {new Vector3(5, 5, 0), new Vector3(-5, -5, 0)}, new Vector3[] {new Vector3(-3, -3, 0), new Vector3(3, 3, 0)}));
        // Debug.Log("expect: include, result: " + cf.LinePositionRelation(new Vector3[] {new Vector3(0, -3, 0), new Vector3(0, 3, 0)}, new Vector3[] {new Vector3(0, -5, 0), new Vector3(0, 5, 0)}));

        //overlapになって欲しい
        // Debug.Log("expect: overlap, result: " + cf.LinePositionRelation(new Vector3[] {new Vector3(3, 3, 0), new Vector3(-5, -5, 0)}, new Vector3[] {new Vector3(-3, -3, 0), new Vector3(5, 5, 0)}));
        // Debug.Log("expect: overlap, result: " + cf.LinePositionRelation(new Vector3[] {new Vector3(0, -3, 0), new Vector3(0, 5, 0)}, new Vector3[] {new Vector3(0, 3, 0), new Vector3(0, -5, 0)}));

        //not overlapになって欲しい
        // Debug.Log("expect: not overlap, result: " + cf.LinePositionRelation(new Vector3[] {new Vector3(-5, -5, 0), new Vector3(-3, -3, 0)}, new Vector3[] {new Vector3(3, 3, 0), new Vector3(5, 5, 0)}));
        // Debug.Log("expect: not overlap, result: " + cf.LinePositionRelation(new Vector3[] {new Vector3(-3, -3, 0), new Vector3(-5, -5, 0)}, new Vector3[] {new Vector3(3, 3, 0), new Vector3(5, 5, 0)}));
        // Debug.Log("expect: not overlap, result: " + cf.LinePositionRelation(new Vector3[] {new Vector3(-5, -5, 0), new Vector3(-3, -3, 0)}, new Vector3[] {new Vector3(5, 5, 0), new Vector3(3, 3, 0)}));
        // Debug.Log("expect: not overlap, result: " + cf.LinePositionRelation(new Vector3[] {new Vector3(-3, -3, 0), new Vector3(-5, -5, 0)}, new Vector3[] {new Vector3(5, 5, 0), new Vector3(3, 3, 0)}));

        // Debug.Log("expect: not overlap, result: " + cf.LinePositionRelation(new Vector3[] {new Vector3(3, 3, 0), new Vector3(5, 5, 0)}, new Vector3[] {new Vector3(-5, -5, 0), new Vector3(-3, -3, 0)}));
        // Debug.Log("expect: not overlap, result: " + cf.LinePositionRelation(new Vector3[] {new Vector3(5, 5, 0), new Vector3(3, 3, 0)}, new Vector3[] {new Vector3(-5, -5, 0), new Vector3(-3, -3, 0)}));
        // Debug.Log("expect: not overlap, result: " + cf.LinePositionRelation(new Vector3[] {new Vector3(3, 3, 0), new Vector3(5, 5, 0)}, new Vector3[] {new Vector3(-3, -3, 0), new Vector3(-5, -5, 0)}));
        // Debug.Log("expect: not overlap, result: " + cf.LinePositionRelation(new Vector3[] {new Vector3(5, 5, 0), new Vector3(3, 3, 0)}, new Vector3[] {new Vector3(-3, -3, 0), new Vector3(-5, -5, 0)}));

        // Debug.Log("expect: not overlap, result: " + cf.LinePositionRelation(new Vector3[] {new Vector3(0, -5, 0), new Vector3(0, -3, 0)}, new Vector3[] {new Vector3(0, 3, 0), new Vector3(0, 5, 0)}));
        // Debug.Log("expect: not overlap, result: " + cf.LinePositionRelation(new Vector3[] {new Vector3(0, -3, 0), new Vector3(0, -5, 0)}, new Vector3[] {new Vector3(0, 3, 0), new Vector3(0, 5, 0)}));
        // Debug.Log("expect: not overlap, result: " + cf.LinePositionRelation(new Vector3[] {new Vector3(0, -5, 0), new Vector3(0, -3, 0)}, new Vector3[] {new Vector3(0, 5, 0), new Vector3(0, 3, 0)}));
        // Debug.Log("expect: not overlap, result: " + cf.LinePositionRelation(new Vector3[] {new Vector3(0, -3, 0), new Vector3(0, -5, 0)}, new Vector3[] {new Vector3(0, 5, 0), new Vector3(0, 3, 0)}));

        // Debug.Log("expect: not overlap, result: " + cf.LinePositionRelation(new Vector3[] {new Vector3(0, 3, 0), new Vector3(0, 5, 0)}, new Vector3[] {new Vector3(0, -5, 0), new Vector3(0, -3, 0)}));
        // Debug.Log("expect: not overlap, result: " + cf.LinePositionRelation(new Vector3[] {new Vector3(0, 5, 0), new Vector3(0, 3, 0)}, new Vector3[] {new Vector3(0, -5, 0), new Vector3(0, -3, 0)}));
        // Debug.Log("expect: not overlap, result: " + cf.LinePositionRelation(new Vector3[] {new Vector3(0, 3, 0), new Vector3(0, 5, 0)}, new Vector3[] {new Vector3(0, -3, 0), new Vector3(0, -5, 0)}));
        // Debug.Log("expect: not overlap, result: " + cf.LinePositionRelation(new Vector3[] {new Vector3(0, 5, 0), new Vector3(0, 3, 0)}, new Vector3[] {new Vector3(0, -3, 0), new Vector3(0, -5, 0)}));
    }
}
