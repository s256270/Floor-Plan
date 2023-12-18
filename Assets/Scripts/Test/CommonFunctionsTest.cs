using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonFunctionsTest : CommonFunctions
{
    void Start()
    {
        //ThreePointStraightJudgeTest();
        //LinePositionRelationTest();
        //ContactCoordinatesTest();
        //ContactJudgeTest();
        //topArrangeTest();
        RotationTest();
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
    public void ContactCoordinatesTest() {
        //接しているとき
        Debug.Log("testcase1");
        Debug.Log("expect0: (-3, 3, 0), result0: " + ContactCoordinates(new Vector3[] {new Vector3(-5, 3, 0), new Vector3(5, 3, 0)}, new Vector3[] {new Vector3(-3, 3, 0), new Vector3(3, 3, 0), new Vector3(3, -3, 0), new Vector3(-3, -3, 0)})[0]);
        Debug.Log("expect1: (3, 3, 0), result1: " + ContactCoordinates(new Vector3[] {new Vector3(-5, 3, 0), new Vector3(5, 3, 0)}, new Vector3[] {new Vector3(-3, 3, 0), new Vector3(3, 3, 0), new Vector3(3, -3, 0), new Vector3(-3, -3, 0)})[1]);

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
    
    /// <summary>
    /// topArrangeの動作テスト
    /// </summary>
    public void topArrangeTest() {
        // Debug.Log("testcase1");
        // Debug.Log("expect: (-3, 3, 0), result: " + topArrange(new Vector3[] {new Vector3(0, 0, 0), new Vector3(-3, 0, 0), new Vector3(-3, 3, 0), new Vector3(3, 3, 0), new Vector3(3, -5, 0), new Vector3(0, -5, 0)})[0]);
        // Debug.Log("expect: (3, 3, 0), result: " + topArrange(new Vector3[] {new Vector3(0, 0, 0), new Vector3(-3, 0, 0), new Vector3(-3, 3, 0), new Vector3(3, 3, 0), new Vector3(3, -5, 0), new Vector3(0, -5, 0)})[1]);
        // Debug.Log("expect: (3, -5, 0), result: " + topArrange(new Vector3[] {new Vector3(0, 0, 0), new Vector3(-3, 0, 0), new Vector3(-3, 3, 0), new Vector3(3, 3, 0), new Vector3(3, -5, 0), new Vector3(0, -5, 0)})[2]);
        // Debug.Log("expect: (0, -5, 0), result: " + topArrange(new Vector3[] {new Vector3(0, 0, 0), new Vector3(-3, 0, 0), new Vector3(-3, 3, 0), new Vector3(3, 3, 0), new Vector3(3, -5, 0), new Vector3(0, -5, 0)})[3]);
        // Debug.Log("expect: (0, 0, 0), result: " + topArrange(new Vector3[] {new Vector3(0, 0, 0), new Vector3(-3, 0, 0), new Vector3(-3, 3, 0), new Vector3(3, 3, 0), new Vector3(3, -5, 0), new Vector3(0, -5, 0)})[4]);
        // Debug.Log("expect: (-3, 0, 0), result: " + topArrange(new Vector3[] {new Vector3(0, 0, 0), new Vector3(-3, 0, 0), new Vector3(-3, 3, 0), new Vector3(3, 3, 0), new Vector3(3, -5, 0), new Vector3(0, -5, 0)})[5]);

        Debug.Log("testcase2");
        Debug.Log("expect: (-5, 0, 0), result: " + topArrange(new Vector3[] {new Vector3(3, 3, 0), new Vector3(3, -3, 0), new Vector3(-5, -3, 0), new Vector3(-5, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 3, 0)})[0]);
        Debug.Log("expect: (0, 0, 0), result: " + topArrange(new Vector3[] {new Vector3(3, 3, 0), new Vector3(3, -3, 0), new Vector3(-5, -3, 0), new Vector3(-5, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 3, 0)})[1]);
        Debug.Log("expect: (0, 3, 0), result: " + topArrange(new Vector3[] {new Vector3(3, 3, 0), new Vector3(3, -3, 0), new Vector3(-5, -3, 0), new Vector3(-5, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 3, 0)})[2]);
        Debug.Log("expect: (3, 3, 0), result: " + topArrange(new Vector3[] {new Vector3(3, 3, 0), new Vector3(3, -3, 0), new Vector3(-5, -3, 0), new Vector3(-5, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 3, 0)})[3]);
        Debug.Log("expect: (3, -3, 0), result: " + topArrange(new Vector3[] {new Vector3(3, 3, 0), new Vector3(3, -3, 0), new Vector3(-5, -3, 0), new Vector3(-5, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 3, 0)})[4]);
        Debug.Log("expect: (-5, -3, 0), result: " + topArrange(new Vector3[] {new Vector3(3, 3, 0), new Vector3(3, -3, 0), new Vector3(-5, -3, 0), new Vector3(-5, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 3, 0)})[5]);
    }

    /// <summary>
    /// Rotationの動作テスト
    /// </summary>
    public void RotationTest() {
        Vector3[] testPolygon = new Vector3[] {new Vector3(-3, 3, 0), new Vector3(3, 3, 0), new Vector3(3, -5, 0), new Vector3(0, -5, 0), new Vector3(0, 0, 0), new Vector3(-3, 0, 0)};
        int topIndex = 0;
        float angle = 0f;
        float x = 0;
        float y = 0;

        Debug.Log("testcase1: 90°回転");
        topIndex = 1;
        angle = Mathf.PI / 2;
        for (int i = 0; i < testPolygon.Length; i++) {
            x = testPolygon[(topIndex+i)%testPolygon.Length].x * Mathf.Cos(angle) - testPolygon[(topIndex+i)%testPolygon.Length].y * Mathf.Sin(angle);
            y = testPolygon[(topIndex+i)%testPolygon.Length].x * Mathf.Sin(angle) + testPolygon[(topIndex+i)%testPolygon.Length].y * Mathf.Cos(angle);

            Debug.Log("expect: " + new Vector3(x, y, 0) + ", result: " + Rotation(testPolygon, 90)[i]);
        }

        Debug.Log("testcase2: 180°回転");
        topIndex = 2;
        angle = Mathf.PI;
        for (int i = 0; i < testPolygon.Length; i++) {
            x = testPolygon[(topIndex+i)%testPolygon.Length].x * Mathf.Cos(angle) - testPolygon[(topIndex+i)%testPolygon.Length].y * Mathf.Sin(angle);
            y = testPolygon[(topIndex+i)%testPolygon.Length].x * Mathf.Sin(angle) + testPolygon[(topIndex+i)%testPolygon.Length].y * Mathf.Cos(angle);

            Debug.Log("expect: " + new Vector3(x, y, 0) + ", result: " + Rotation(testPolygon, 180)[i]);
        }

        Debug.Log("testcase3: 270°回転");
        topIndex = 3;
        angle = Mathf.PI * 3 / 2;
        for (int i = 0; i < testPolygon.Length; i++) {
            x = testPolygon[(topIndex+i)%testPolygon.Length].x * Mathf.Cos(angle) - testPolygon[(topIndex+i)%testPolygon.Length].y * Mathf.Sin(angle);
            y = testPolygon[(topIndex+i)%testPolygon.Length].x * Mathf.Sin(angle) + testPolygon[(topIndex+i)%testPolygon.Length].y * Mathf.Cos(angle);

            Debug.Log("expect: " + new Vector3(x, y, 0) + ", result: " + Rotation(testPolygon, 270)[i]);
        }
    }
}
