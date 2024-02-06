using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CommonFunctionsTest : CommonFunctions
{
    void Start()
    {
        //ThreePointStraightJudgeTest();
        //LinePositionRelationTest();
        //ContactCoordinatesTest();
        //ContactJudgeTest();
        //TopArrangeTest();
        //RotationTest();
        //SideSubstractionTest();
        //OnLineSegmentTest();

        Dictionary<string, Vector3[]> roomPattern = new Dictionary<string, Vector3[]>();
        Vector3[] Toilet = new Vector3[] {new Vector3(-16500, 4750, 0), new Vector3(-15600, 4750, 0), new Vector3(-15600, 3350, 0), new Vector3(-16500, 3350, 0)};
        roomPattern.Add("Toilet", Toilet);
        Vector3[] UB = new Vector3[] {new Vector3(-18300, 4750, 0), new Vector3(-16500, 4750, 0), new Vector3(-16500, 3350, 0), new Vector3(-18300, 3350, 0)};
        roomPattern.Add("UB", UB);
        Vector3[] Washroom = new Vector3[] {new Vector3(-19200, 4750, 0), new Vector3(-18300, 4750, 0), new Vector3(-18300, 3250, 0), new Vector3(-17800, 3250, 0), new Vector3(-17800, 2350, 0), new Vector3(-19200, 2350, 0)};
        roomPattern.Add("Washroom", Washroom);
        Vector3[] Kitchen = new Vector3[] {new Vector3(-19000, 1650, 0), new Vector3(-16600, 1650, 0), new Vector3(-16600, 950, 0), new Vector3(-19000, 950, 0)};
        roomPattern.Add("Kitchen", Kitchen);
        Vector3[] Western = new Vector3[] {new Vector3(-23400, 4750, 0), new Vector3(-19200, 4750, 0), new Vector3(-19200, 2350, 0), new Vector3(-19000, 2350, 0), new Vector3(-19000, 950, 0), new Vector3(-23400, 950, 0)};
        roomPattern.Add("Western", Western);
        
        Debug.Log(SecureWidth(roomPattern));
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
    public void TopArrangeTest() {
        // Debug.Log("testcase1");
        // Debug.Log("expect: (-3, 3, 0), result: " + topArrange(new Vector3[] {new Vector3(0, 0, 0), new Vector3(-3, 0, 0), new Vector3(-3, 3, 0), new Vector3(3, 3, 0), new Vector3(3, -5, 0), new Vector3(0, -5, 0)})[0]);
        // Debug.Log("expect: (3, 3, 0), result: " + topArrange(new Vector3[] {new Vector3(0, 0, 0), new Vector3(-3, 0, 0), new Vector3(-3, 3, 0), new Vector3(3, 3, 0), new Vector3(3, -5, 0), new Vector3(0, -5, 0)})[1]);
        // Debug.Log("expect: (3, -5, 0), result: " + topArrange(new Vector3[] {new Vector3(0, 0, 0), new Vector3(-3, 0, 0), new Vector3(-3, 3, 0), new Vector3(3, 3, 0), new Vector3(3, -5, 0), new Vector3(0, -5, 0)})[2]);
        // Debug.Log("expect: (0, -5, 0), result: " + topArrange(new Vector3[] {new Vector3(0, 0, 0), new Vector3(-3, 0, 0), new Vector3(-3, 3, 0), new Vector3(3, 3, 0), new Vector3(3, -5, 0), new Vector3(0, -5, 0)})[3]);
        // Debug.Log("expect: (0, 0, 0), result: " + topArrange(new Vector3[] {new Vector3(0, 0, 0), new Vector3(-3, 0, 0), new Vector3(-3, 3, 0), new Vector3(3, 3, 0), new Vector3(3, -5, 0), new Vector3(0, -5, 0)})[4]);
        // Debug.Log("expect: (-3, 0, 0), result: " + topArrange(new Vector3[] {new Vector3(0, 0, 0), new Vector3(-3, 0, 0), new Vector3(-3, 3, 0), new Vector3(3, 3, 0), new Vector3(3, -5, 0), new Vector3(0, -5, 0)})[5]);

        Debug.Log("testcase2");
        Debug.Log("expect: (-5, 0, 0), result: " + TopArrange(new Vector3[] {new Vector3(3, 3, 0), new Vector3(3, -3, 0), new Vector3(-5, -3, 0), new Vector3(-5, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 3, 0)})[0]);
        Debug.Log("expect: (0, 0, 0), result: " + TopArrange(new Vector3[] {new Vector3(3, 3, 0), new Vector3(3, -3, 0), new Vector3(-5, -3, 0), new Vector3(-5, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 3, 0)})[1]);
        Debug.Log("expect: (0, 3, 0), result: " + TopArrange(new Vector3[] {new Vector3(3, 3, 0), new Vector3(3, -3, 0), new Vector3(-5, -3, 0), new Vector3(-5, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 3, 0)})[2]);
        Debug.Log("expect: (3, 3, 0), result: " + TopArrange(new Vector3[] {new Vector3(3, 3, 0), new Vector3(3, -3, 0), new Vector3(-5, -3, 0), new Vector3(-5, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 3, 0)})[3]);
        Debug.Log("expect: (3, -3, 0), result: " + TopArrange(new Vector3[] {new Vector3(3, 3, 0), new Vector3(3, -3, 0), new Vector3(-5, -3, 0), new Vector3(-5, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 3, 0)})[4]);
        Debug.Log("expect: (-5, -3, 0), result: " + TopArrange(new Vector3[] {new Vector3(3, 3, 0), new Vector3(3, -3, 0), new Vector3(-5, -3, 0), new Vector3(-5, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 3, 0)})[5]);
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

    /// <summary>
    /// SideSubstractionの動作テスト
    /// </summary>
    public void SideSubstractionTest() {
        Vector3[] testLineA = new Vector3[2];
        Vector3[] testLineB = new Vector3[2];

        Debug.Log("testcase1: match");
        testLineA = new Vector3[] {new Vector3(-5, 0, 0), new Vector3(5, 0, 0)};
        testLineB = new Vector3[] {new Vector3(-5, 0, 0), new Vector3(5, 0, 0)};
        Debug.Log("expect: (0, 0, 0) ~ (0, 0, 0), result: " + SideSubstraction(testLineA, testLineB)[0][0] + " ~ " + SideSubstraction(testLineA, testLineB)[0][1]);

        Debug.Log("testcase2: overlap1");
        testLineA = new Vector3[] {new Vector3(-5, 0, 0), new Vector3(3, 0, 0)};
        testLineB = new Vector3[] {new Vector3(-3, 0, 0), new Vector3(5, 0, 0)};
        Debug.Log("expect: (-5, 0, 0) ~ (-3, 0, 0), result: " + SideSubstraction(testLineA, testLineB)[0][0] + " ~ " + SideSubstraction(testLineA, testLineB)[0][1]);

        Debug.Log("testcase2: overlap2");
        testLineA = new Vector3[] {new Vector3(-3, 0, 0), new Vector3(5, 0, 0)};
        testLineB = new Vector3[] {new Vector3(-5, 0, 0), new Vector3(3, 0, 0)};
        Debug.Log("expect: (3, 0, 0) ~ (5, 0, 0), result: " + SideSubstraction(testLineA, testLineB)[0][0] + " ~ " + SideSubstraction(testLineA, testLineB)[0][1]);

        Debug.Log("testcase3: include1");
        testLineA = new Vector3[] {new Vector3(-5, 0, 0), new Vector3(5, 0, 0)};
        testLineB = new Vector3[] {new Vector3(-5, 0, 0), new Vector3(3, 0, 0)};
        Debug.Log("expect: (3, 0, 0) ~ (5, 0, 0), result: " + SideSubstraction(testLineA, testLineB)[0][0] + " ~ " + SideSubstraction(testLineA, testLineB)[0][1]);

        Debug.Log("testcase3: include2");
        testLineA = new Vector3[] {new Vector3(-5, 0, 0), new Vector3(5, 0, 0)};
        testLineB = new Vector3[] {new Vector3(-3, 0, 0), new Vector3(5, 0, 0)};
        Debug.Log("expect: (-5, 0, 0) ~ (-3, 0, 0), result: " + SideSubstraction(testLineA, testLineB)[0][0] + " ~ " + SideSubstraction(testLineA, testLineB)[0][1]);

        Debug.Log("testcase3: include3");
        testLineA = new Vector3[] {new Vector3(-5, 0, 0), new Vector3(5, 0, 0)};
        testLineB = new Vector3[] {new Vector3(-3, 0, 0), new Vector3(3, 0, 0)};
        Debug.Log("expect: (-5, 0, 0) ~ (-3, 0, 0), result: " + SideSubstraction(testLineA, testLineB)[0][0] + " ~ " + SideSubstraction(testLineA, testLineB)[0][1]);
        Debug.Log("expect: (3, 0, 0) ~ (5, 0, 0), result: " + SideSubstraction(testLineA, testLineB)[1][0] + " ~ " + SideSubstraction(testLineA, testLineB)[1][1]);

        Debug.Log("testcase3: include4");
        testLineA = new Vector3[] {new Vector3(-5, 0, 0), new Vector3(3, 0, 0)};
        testLineB = new Vector3[] {new Vector3(-5, 0, 0), new Vector3(5, 0, 0)};
        Debug.Log("expect: (0, 0, 0) ~ (0, 0, 0), result: " + SideSubstraction(testLineA, testLineB)[0][0] + " ~ " + SideSubstraction(testLineA, testLineB)[0][1]);

        Debug.Log("testcase3: include5");
        testLineA = new Vector3[] {new Vector3(-3, 0, 0), new Vector3(5, 0, 0)};
        testLineB = new Vector3[] {new Vector3(-5, 0, 0), new Vector3(5, 0, 0)};
        Debug.Log("expect: (0, 0, 0) ~ (0, 0, 0), result: " + SideSubstraction(testLineA, testLineB)[0][0] + " ~ " + SideSubstraction(testLineA, testLineB)[0][1]);

        Debug.Log("testcase3: include6");
        testLineA = new Vector3[] {new Vector3(-3, 0, 0), new Vector3(3, 0, 0)};
        testLineB = new Vector3[] {new Vector3(-5, 0, 0), new Vector3(5, 0, 0)};
        Debug.Log("expect: (0, 0, 0) ~ (0, 0, 0), result: " + SideSubstraction(testLineA, testLineB)[0][0] + " ~ " + SideSubstraction(testLineA, testLineB)[0][1]);

        Debug.Log("testcase4: not overlap1");
        testLineA = new Vector3[] {new Vector3(-5, 0, 0), new Vector3(5, 0, 0)};
        testLineB = new Vector3[] {new Vector3(-11, 0, 0), new Vector3(-6, 0, 0)};
        Debug.Log("expect: (-5, 0, 0) ~ (5, 0, 0), result: " + SideSubstraction(testLineA, testLineB)[0][0] + " ~ " + SideSubstraction(testLineA, testLineB)[0][1]);

        Debug.Log("testcase4: not overlap2");
        testLineA = new Vector3[] {new Vector3(-5, 0, 0), new Vector3(5, 0, 0)};
        testLineB = new Vector3[] {new Vector3(6, 0, 0), new Vector3(11, 0, 0)};
        Debug.Log("expect: (-5, 0, 0) ~ (5, 0, 0), result: " + SideSubstraction(testLineA, testLineB)[0][0] + " ~ " + SideSubstraction(testLineA, testLineB)[0][1]);

        Debug.Log("testcase5: point overlap1");
        testLineA = new Vector3[] {new Vector3(-5, 0, 0), new Vector3(5, 0, 0)};
        testLineB = new Vector3[] {new Vector3(-10, 0, 0), new Vector3(-5, 0, 0)};
        Debug.Log("expect: (-5, 0, 0) ~ (5, 0, 0), result: " + SideSubstraction(testLineA, testLineB)[0][0] + " ~ " + SideSubstraction(testLineA, testLineB)[0][1]);

        Debug.Log("testcase5: point overlap2");
        testLineA = new Vector3[] {new Vector3(-5, 0, 0), new Vector3(5, 0, 0)};
        testLineB = new Vector3[] {new Vector3(5, 0, 0), new Vector3(10, 0, 0)};
        Debug.Log("expect: (-5, 0, 0) ~ (5, 0, 0), result: " + SideSubstraction(testLineA, testLineB)[0][0] + " ~ " + SideSubstraction(testLineA, testLineB)[0][1]);

        Debug.Log("testcase6: not straight");
        testLineA = new Vector3[] {new Vector3(-5, 0, 0), new Vector3(5, 0, 0)};
        testLineB = new Vector3[] {new Vector3(-5, 1, 0), new Vector3(5, 1, 0)};
        Debug.Log("expect: (-5, 0, 0) ~ (5, 0, 0), result: " + SideSubstraction(testLineA, testLineB)[0][0] + " ~ " + SideSubstraction(testLineA, testLineB)[0][1]);
    }

    /// <summary>
    /// OnLineSegmentの動作テスト
    /// </summary>
    public void OnLineSegmentTest() {
        Debug.Log("testcase1: True");
        Debug.Log("expect: True, result: " + OnLineSegment(new Vector3[]{new Vector3(-5, 0, 0), new Vector3(5, 0, 0)}, new Vector3[]{new Vector3(-3, 0, 0), new Vector3(5, 0, 0)}));
        Debug.Log("expect: True, result: " + OnLineSegment(new Vector3[]{new Vector3(0, -5, 0), new Vector3(0, 5, 0)}, new Vector3[]{new Vector3(0, -5, 0), new Vector3(0, 5, 0)}));
        Debug.Log("expect: True, result: " + OnLineSegment(new Vector3[]{new Vector3(-5, -5, 0), new Vector3(5, 5, 0)}, new Vector3[]{new Vector3(-3, -3, 0), new Vector3(3, 3, 0)}));

        Debug.Log("testcase2: False");
        Debug.Log("expect: False, result: " + OnLineSegment(new Vector3[]{new Vector3(-5, 0, 0), new Vector3(5, 0, 0)}, new Vector3[]{new Vector3(-3, 0, 0), new Vector3(6, 0, 0)}));
        Debug.Log("expect: False, result: " + OnLineSegment(new Vector3[]{new Vector3(0, -5, 0), new Vector3(0, 5, 0)}, new Vector3[]{new Vector3(0, -3, 0), new Vector3(0, 6, 0)}));
        Debug.Log("expect: False, result: " + OnLineSegment(new Vector3[]{new Vector3(-5, -5, 0), new Vector3(5, 5, 0)}, new Vector3[]{new Vector3(-3, -3, 0), new Vector3(6, 6, 0)}));
    }

    /// <summary>
    /// 部屋を利用するための幅が取れていないものを除く
    /// </summary> 
    /// <param name="roomPattern">部屋の配置パターン（座標）</param>
    /// <returns>幅が取れている場合True, そうでない場合False</returns>
    public bool SecureWidth(Dictionary<string, Vector3[]> roomPattern) {
        Vector3[] range = new Vector3[]{new Vector3(-9650, 3650, 0), new Vector3(-1850, 3650, 0), new Vector3(-1850, 1600, 0), new Vector3(-2850, 1600, 0), new Vector3(-2850, -150, 0), new Vector3(-9650, -150, 0)};
        CreateRoom("", range);

        //判定結果
        bool flag = true;

        //調べる部屋を決める
        foreach (KeyValuePair<string, Vector3[]> checkRoom in roomPattern) {
            //調べる部屋が洗面室，トイレ，キッチン以外の場合
            if (!(checkRoom.Key == "Washroom" || checkRoom.Key == "Toilet" || checkRoom.Key == "Kitchen")) {
                //スキップ
                continue;
            }

            Debug.Log("checkRoom: " + checkRoom.Key);

            //調べる部屋を1辺ずつに分割
            List<Vector3[]> checkRoomSides = new List<Vector3[]>(); //調べる部屋の辺のリスト
            Vector3[] checkRoomCoordinates = roomPattern[checkRoom.Key]; //調べる部屋の座標
            //調べる部屋の辺のリストを作成
            for (int i = 0; i < checkRoomCoordinates.Length; i++) {
                checkRoomSides.Add(new Vector3[]{checkRoomCoordinates[i], checkRoomCoordinates[(i+1)%checkRoomCoordinates.Length]});
            }

            //水回り範囲との共通部分を調べる
            //水回り範囲の各辺について
            for (int i = 0; i < range.Length; i++) {
                Vector3[] checkRangeSide = new Vector3[]{range[i], range[(i+1)%range.Length]};

                //調べる部屋の各辺について
                for (int j = 0; j < checkRoomSides.Count; j++) {
                    Vector3[] checkRoomSide = checkRoomSides[j];

                    //2辺が接しているとき
                    if (ContactJudge(checkRoomSide, checkRangeSide)) {
                        //水回り範囲との共通部分を除いた辺が存在しないとき
                        if (GetLength(SideSubstraction(checkRoomSide, checkRangeSide)[0]) == 0.0f) {
                            //その辺を削除
                            checkRoomSides.RemoveAt(j);
                            j--;
                        }
                        else {
                            //共通部分を除いた辺に更新
                            checkRoomSides[j] = SideSubstraction(checkRoomSide, checkRangeSide)[0];
                        }
                    }
                }
            }

            //周りの部屋について調べる
            foreach (KeyValuePair<string, Vector3[]> surroundingRoom in roomPattern) {
                //調べる部屋と周りの部屋が同じ場合
                if (surroundingRoom.Key == checkRoom.Key) {
                    //スキップ
                    continue;
                }

                //調べる部屋と周りの部屋が接しているとき
                if (ContactJudge(surroundingRoom.Value, checkRoomCoordinates)) {
                    //周りの部屋との共通部分につい調べる
                    //周りの部屋の各辺について
                    for (int i = 0; i < surroundingRoom.Value.Length; i++) {
                        Vector3[] surroundingRoomSide = new Vector3[]{surroundingRoom.Value[i], surroundingRoom.Value[(i+1)%surroundingRoom.Value.Length]};

                        //調べる部屋の各辺について
                        for (int j = 0; j < checkRoomSides.Count; j++) {
                            Vector3[] checkRoomSide = checkRoomSides[j];

                            //2辺が接しているとき
                            if (ContactJudge(checkRoomSide, surroundingRoomSide)) {
                                //周りの部屋との共通部分を除いた辺が存在しないとき
                                if (GetLength(SideSubstraction(checkRoomSide, surroundingRoomSide)[0]) == 0.0f) {
                                    //その辺を削除
                                    checkRoomSides.RemoveAt(j);
                                    j--;
                                }
                                else {
                                    //共通部分を除いた辺に更新
                                    checkRoomSides[j] = SideSubstraction(checkRoomSide, surroundingRoomSide)[0];
                                }
                            }
                        }
                    }
                }
            }

            //残った辺が必要な長さがあるかを調べる
            //残った辺が存在するとき
            if (checkRoomSides.Count > 0) {     
                //調べる部屋の辺の一番長いものを求める
                Vector3[] checkRoomSide = checkRoomSides[0];
                float checkRoomSideLength = GetLength(checkRoomSides[0]);
                for (int i = 0; i < checkRoomSides.Count; i++) {
                    if (checkRoomSideLength < GetLength(checkRoomSides[i])) {
                        checkRoomSide = checkRoomSides[i];
                        checkRoomSideLength = GetLength(checkRoomSides[i]);
                    }
                }

                //最大辺の前のスペースに幅が取れているか調べる
                //スペースが取れているか調べるための長方形の座標
                Vector3[] judgeSquare = new Vector3[4];

                //辺がy軸平行の場合
                if (Slope(checkRoomSide) == Mathf.Infinity) {
                    judgeSquare = new Vector3[]{new Vector3(-400, checkRoomSideLength/2, 0), new Vector3(400, checkRoomSideLength/2, 0), new Vector3(400, -checkRoomSideLength/2, 0), new Vector3(-400, -checkRoomSideLength/2, 0)};

                    //動かす方向
                    int wetareasShiftDirection = ShiftJudge(checkRoom.Value, checkRoomSide);

                    //x座標・y座標の移動
                    //調べるための長方形を辺の左側にするとき
                    if (wetareasShiftDirection > 0) {
                        judgeSquare = CorrectCoordinates(judgeSquare, new Vector3(checkRoomSide[0].x - 400, (checkRoomSide[0].y + checkRoomSide[1].y)/2, 0));
                    }
                    //部屋を辺の右側にするとき
                    else if (wetareasShiftDirection < 0) {
                        judgeSquare = CorrectCoordinates(judgeSquare, new Vector3(checkRoomSide[0].x + 450, (checkRoomSide[0].y + checkRoomSide[1].y)/2, 0));
                    }
                }
                //辺がy軸平行の場合
                else if (Slope(checkRoomSide) == 0.0f) {
                    judgeSquare = new Vector3[]{new Vector3(checkRoomSideLength/2, -400, 0), new Vector3(checkRoomSideLength/2, 400, 0), new Vector3(-checkRoomSideLength/2, 400, 0), new Vector3(-checkRoomSideLength/2, -400, 0)};

                    //動かす方向
                    int wetareasShiftDirection = ShiftJudge(checkRoom.Value, checkRoomSide);

                    //x座標・y座標の移動
                    //調べるための長方形を辺の下側にするとき
                    if (wetareasShiftDirection > 0) {
                        judgeSquare = CorrectCoordinates(judgeSquare, new Vector3((checkRoomSide[0].x + checkRoomSide[1].x)/2, checkRoomSide[0].y - 400, 0));
                    }
                    //部屋を辺の上側にするとき
                    else if (wetareasShiftDirection < 0) {
                        judgeSquare = CorrectCoordinates(judgeSquare, new Vector3((checkRoomSide[0].x + checkRoomSide[1].x)/2, checkRoomSide[0].y + 400, 0));
                    }
                }

                //調べる長方形が他の部屋の外側にあるかを調べる
                foreach (KeyValuePair<string, Vector3[]> surroundingRoom in roomPattern) {
                    //調べる部屋と他の部屋が同じ場合
                    if (surroundingRoom.Key == checkRoom.Key) {
                        //スキップ
                        continue;
                    }

                    //調べる長方形が他の部屋の外側にないとき
                    if (!JudgeOutside(surroundingRoom.Value, judgeSquare)) {
                        flag = false;
                        return flag;
                    }
                    //他の部屋が調べる長方形外側にないとき
                    if (!JudgeOutside(judgeSquare, surroundingRoom.Value)) {
                        flag = false;
                        return flag;
                    }
                }
            }
            //残った辺が存在しないとき
            else {
                if (checkRoom.Key == "Washroom" || checkRoom.Key == "Toilet" || checkRoom.Key == "Kitchen") {
                    flag = false;
                    return flag;
                }
            }
        }
        
        return flag;
    }
}
