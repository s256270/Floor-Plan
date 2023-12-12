using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonFunctionsTest : CommonFunctions
{
    void Start()
    {
        //ThreePointStraightJudgeTest();
        //LinePositionRelationTest();
        //ContactCoodinatesTest();
        ContactJudgeTest();
    }

    /// <summary>
    /// ThreePointStraightJudgeの動作テスト
    /// </summary>
    public void ThreePointStraightJudgeTest() {
        //Trueになって欲しい
        // Debug.Log("expect: True, result: " + cf.ThreePointStraightJudge(new Vector3(-5, -5, 0), new Vector3(0, 0, 0), new Vector3(2, 2, 0)));
        // Debug.Log("expect: True, result: " + cf.ThreePointStraightJudge(new Vector3(-5, -5, 0), new Vector3(2, 2, 0), new Vector3(0, 0, 0)));
        // Debug.Log("expect: True, result: " + cf.ThreePointStraightJudge(new Vector3(0, 0, 0), new Vector3(2, 2, 0), new Vector3(-5, -5, 0)));

        //falseになって欲しい
        // Debug.Log("expect: False, result: " + cf.ThreePointStraightJudge(new Vector3(-5, -5, 0), new Vector3(0, 0, 0), new Vector3(2, 3, 0)));
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
        // Debug.Log("expect: include, result: " + cf.LinePositionRelation(new Vector3[] {new Vector3(5, 5, 0), new Vector3(-3, -3, 0)}, new Vector3[] {new Vector3(-5, -5, 0), new Vector3(5, 5, 0)}));

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

    /// <summary>
    /// ContactCoodinatesの動作テスト
    /// </summary>
    public void ContactCoodinatesTest() {
        //接しているとき
        Debug.Log("testcase1");
        Debug.Log("expect0: (-3, 3, 0), result0: " + ContactCoodinates(new Vector3[] {new Vector3(-5, 3, 0), new Vector3(5, 3, 0)}, new Vector3[] {new Vector3(-3, 3, 0), new Vector3(3, 3, 0), new Vector3(3, -3, 0), new Vector3(-3, -3, 0)})[0]);
        Debug.Log("expect1: (3, 3, 0), result1: " + ContactCoodinates(new Vector3[] {new Vector3(-5, 3, 0), new Vector3(5, 3, 0)}, new Vector3[] {new Vector3(-3, 3, 0), new Vector3(3, 3, 0), new Vector3(3, -3, 0), new Vector3(-3, -3, 0)})[1]);

        // Debug.Log("testcase2");
        // Debug.Log("expect0: (-3, -3, 0), result0: " + cf.ContactCoodinates(new Vector3[] {new Vector3(-3, 5, 0), new Vector3(-3, -5, 0)}, new Vector3[] {new Vector3(-3, 3, 0), new Vector3(3, 3, 0), new Vector3(3, -3, 0), new Vector3(-3, -3, 0)})[0]);
        // Debug.Log("expect1: (-3, 3, 0), result1: " + cf.ContactCoodinates(new Vector3[] {new Vector3(-3, 5, 0), new Vector3(-3, -5, 0)}, new Vector3[] {new Vector3(-3, 3, 0), new Vector3(3, 3, 0), new Vector3(3, -3, 0), new Vector3(-3, -3, 0)})[1]);

        // Debug.Log("testcase3");
        // Debug.Log("expect0: (-3, 3, 0), result0: " + cf.ContactCoodinates(new Vector3[] {new Vector3(-6, 2, 0), new Vector3(6, 6, 0)}, new Vector3[] {new Vector3(-3, 3, 0), new Vector3(3, 5, 0), new Vector3(3, -3, 0), new Vector3(-3, -3, 0)})[0]);
        // Debug.Log("expect1: (3, 5, 0), result1: " + cf.ContactCoodinates(new Vector3[] {new Vector3(-6, 2, 0), new Vector3(6, 6, 0)}, new Vector3[] {new Vector3(-3, 3, 0), new Vector3(3, 5, 0), new Vector3(3, -3, 0), new Vector3(-3, -3, 0)})[1]);

        //接していないとき
        // Debug.Log("testcase1");
        // Debug.Log("expect: 0, result: " + cf.ContactCoodinates(new Vector3[] {new Vector3(-5, 0, 0), new Vector3(5, 0, 0)}, new Vector3[] {new Vector3(-3, 3, 0), new Vector3(3, 3, 0), new Vector3(3, -3, 0), new Vector3(-3, -3, 0)}).Length);
    }

    /// <summary>
    /// ContactJudgeの動作テスト
    /// </summary>
    public void ContactJudgeTest() {
        //接しているとき
        // Debug.Log("testcase1");
        // Debug.Log("expect: True, result: " + cf.ContactJudge(new Vector3[] {new Vector3(-5, 3, 0), new Vector3(5, 3, 0)}, new Vector3[] {new Vector3(-3, 3, 0), new Vector3(3, 3, 0), new Vector3(3, -3, 0), new Vector3(-3, -3, 0)}));

        // Debug.Log("testcase2");
        // Debug.Log("expect: True, result: " + cf.ContactJudge(new Vector3[] {new Vector3(-3, 5, 0), new Vector3(-3, -5, 0)}, new Vector3[] {new Vector3(-3, 3, 0), new Vector3(3, 3, 0), new Vector3(3, -3, 0), new Vector3(-3, -3, 0)}));

        // Debug.Log("testcase3");
        // Debug.Log("expect: True, result: " + cf.ContactJudge(new Vector3[] {new Vector3(-6, 2, 0), new Vector3(6, 6, 0)}, new Vector3[] {new Vector3(-3, 3, 0), new Vector3(3, 5, 0), new Vector3(3, -3, 0), new Vector3(-3, -3, 0)}));

        // //接していないとき
        // Debug.Log("testcase1");
        // Debug.Log("expect: False, result: " + cf.ContactJudge(new Vector3[] {new Vector3(-5, 0, 0), new Vector3(5, 0, 0)}, new Vector3[] {new Vector3(-3, 3, 0), new Vector3(3, 3, 0), new Vector3(3, -3, 0), new Vector3(-3, -3, 0)}));
    }
}
