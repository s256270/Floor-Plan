using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class CommonFunctions : MonoBehaviour
{
    public LineRenderer lineRenderer; //linerendererコンポーネント

    /// <summary>
    /// 座標をゲームオブジェクトとして描画する
    /// </summary>
    /// <param name="name">オブジェクト名</param>
    /// <param name="positions">描画したい座標</param>
    /// <returns>描画したオブジェクト</returns>
    public GameObject createRoom(string name, Vector3[] positions) {
        //親となるゲームオブジェクトを生成
        GameObject room = new GameObject(name);

        //部屋のゲームオブジェクトを生成
        GameObject roomObject = new GameObject(name + "Positions");

        // LineRendererコンポーネントをゲームオブジェクトにアタッチする
        lineRenderer = roomObject.AddComponent<LineRenderer>();

        // 点の数を指定する
        lineRenderer.positionCount = positions.Length;

        //座標を指定
        lineRenderer.SetPositions(positions);

        //幅を設定
        lineRenderer.startWidth = 50;

        //色の設定
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

        //ローカル座標を使用
        lineRenderer.useWorldSpace = false;

        //ループ(終点と始点を繋ぐ)させる
        lineRenderer.loop = true;

        //部屋を親にまとめる
        roomObject.transform.SetParent(room.transform);

        //return roomObject;
        return room;
    }

    public void createRoom(string name, Vector3[] positions, UnityEngine.Color color) {
        createRoom(name, positions);

        //色の変更
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }

    // //部屋の1辺と部屋が接している座標を返す
    // public Vector3[] Contact(Vector3[] side, Vector3[] room) {
    //     //すべてがゼロの座標（接しているかどうかの判定に使用）
    //     Vector3[] zero = new Vector3[] {Vector3.zero, Vector3.zero};

    //     //接している（すべての座標がゼロでない）とき，その座標を返す
    //     if ((contact_xy(side, room)[0] != zero[0]) && (contact_xy(side, room)[1] != zero[1])) {
    //         return contact_xy(side, room);
    //     }
    //     //接していない場合，すべてがゼロの座標を返す
    //     else {
    //         return new Vector3[] {Vector3.zero, Vector3.zero};
    //     }
    // }

    // //x座標もy座標も異なるが部屋が接しているときの座標を返す
    // public Vector3[] ContactCoodinates(Vector3[] side, Vector3[] room) {
    //     //4点の座標を格納する変数
    //     Vector3 A1, A2, B1, B2;

    //     //部屋Aのすべての辺を調べる   
    //     A1 = side[0];
    //     A2 = side[1];
        
    //     //部屋のすべての辺を調べる
    //     for (int j = 0; j < room.Length; j++) {
    //         B1 = room[j];
    //         B2 = room[(j+1)%room.Length];

    //         //座標をx, yごとに分けるためのリスト
    //         List<float> coordinates_x = new List<float>();
    //         List<float> coordinates_y = new List<float>();

    //         //4点が一直線上に並んでいるとき（端から真ん中，真ん中から反対の端の距離の和が端から端の距離と同じとき × 2）
    //         if (OnLineSegment(new Vector3[] {A1, A2}, B1) && OnLineSegment(new Vector3[] {A1, A2}, B2)) {
    //             //2つの部屋を通り抜けられるように接しているとき（片方の辺の最小の点がもう片方の辺の最大の点より大きいとき）
    //             if (! (((Mathf.Max(A1.x, A2.x) <= Mathf.Min(B1.x, B2.x)) || (Mathf.Min(A1.x, A2.x) >= Mathf.Max(B1.x, B2.x))) && ((Mathf.Max(A1.y, A2.y) <= Mathf.Min(B1.y, B2.y)) || (Mathf.Min(A1.y, A2.y) >= Mathf.Max(B1.y, B2.y)))) ) {
    //                 /*** 
    //                 xとyを別々にソートしてる
    //                 x軸平行，y軸平行のときのみ成立
    //                 後で変更する　多分
    //                 ***/

    //                 //x座標を昇順にソート
    //                 coordinates_x.Add(A1.x);
    //                 coordinates_x.Add(A2.x);
    //                 coordinates_x.Add(B1.x);
    //                 coordinates_x.Add(B2.x);

    //                 coordinates_x.Sort();

    //                 //y座標を昇順にソート
    //                 coordinates_y.Add(A1.y);
    //                 coordinates_y.Add(A2.y);
    //                 coordinates_y.Add(B1.y);
    //                 coordinates_y.Add(B2.y);

    //                 coordinates_y.Sort();

    //                 //接している辺の共通部分（昇順の真ん中2つ）を返す
    //                 return new Vector3[] {new Vector3(coordinates_x[1], coordinates_y[1], 0), new Vector3(coordinates_x[2], coordinates_y[2], 0)};
    //             }
    //         }
    //     }

    //     //どれにも当てはまらなかったとき，すべてが0の点を返す
    //     return new Vector3[] {Vector3.zero, Vector3.zero};            
    // }

    /// <summary>
    /// 一直線上にある2つの線分がどのような位置関係にあるかを判定
    /// </summary>
    /// <param name="lineA">線分A</param>
    /// <param name="lineB">線分B</param>
    /// <returns></returns>
    public string LinePositionRelation(Vector3[] lineA, Vector3[] lineB) {
        //2線分が一直線上にあるかどうかの判定
        if (!StraightLineJudge(lineA, lineB)) {
            //一直線上でない場合は，ここで終了
            return "not straight";
        }
        
        //2線分の端点である4点を数直線上にあると考え，1つの数で表す(正負が重要なので縮尺が変わるのは問題ない)
        //0番目の要素が小さくなるようにline1を定義
        float[] line1 = new float[] {lineA[0].x, lineA[1].x};
        if (lineA[0].x > lineA[1].x) {
            Array.Reverse(line1);
        }

        //0番目の要素が小さくなるようにline2を定義
        float[] line2 = new float[] {lineB[0].x, lineB[1].x};
        if (lineB[0].x > lineB[1].x) {
            Array.Reverse(line2);
        }

        //0番目の要素が小さい方をline1，大きい方をline2とする
        if (lineA[0].x > lineB[0].x) {
            float[] tmp = line1;
            line1 = line2;
            line2 = tmp;
        }

        //線分がy軸平行のときのみy座標で考える
        if (lineA[0].x == lineA[1].x) {
            //0番目の要素が小さくなるようにline1を定義
            line1 = new float[] {lineA[0].y, lineA[1].y};
            if (lineA[0].y > lineA[1].y) {
                Array.Reverse(line1);
            }

            //0番目の要素が小さくなるようにline2を定義
            line2 = new float[] {lineB[0].y, lineB[1].y};
            if (lineB[0].y > lineB[1].y) {
                Array.Reverse(line2);
            }

            //0番目の要素が小さい方をline1，大きい方をline2とする
            if (lineA[0].y > lineB[0].y) {
                float[] tmp = line1;
                line1 = line2;
                line2 = tmp;
            }
        }

        //2点の差の積を計算
        float s1 = (line1[0] - line2[0]) * (line1[1] - line2[1]);
        float s2 = (line1[0] - line2[1]) * (line1[1] - line2[0]);

        //s1とs2の正負によって位置関係を判定
        if (s1 == 0 && s2 < 0) {
            //一致
            return "match";
        }
        else if (s1 > 0 && s2 == 0) {
            //端点が重なっている
            return "point overlap";
        }
        else if (s1 < 0 && s2 < 0) {
            //包含
            return "include";
        }
        else if (s1 > 0 && s2 < 0) {
            //重なる
            return "overlap";
        }
        else if (s1 > 0 && s2 > 0) {
            //重ならない
            return "not overlap";
        }

        return "error";
    }

    /// <summary>
    /// 2つの線分が一直線上にあるかどうかの判定
    /// </summary>
    /// <param name="lineA">線分A</param>
    /// <param name="lineB">線分B</param>
    /// <returns>点が線分上にある場合True，ない場合Flase</returns>
    public bool StraightLineJudge(Vector3[] lineA, Vector3[] lineB) {
        //線分Aの端点の座標
        Vector3 A1 = lineA[0];
        Vector3 A2 = lineA[1];

        //線分Bの端点の座標
        Vector3 B1 = lineB[0];
        Vector3 B2 = lineB[1];

        if (ThreePointStraightJudge(lineA[0], lineA[1], lineB[0]) && ThreePointStraightJudge(lineA[0], lineA[1], lineB[1])) {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 3点が一直線上にあるかどうかの判定
    /// </summary>
    /// <param name="pointA">点A</param>
    /// <param name="pointB">点B</param>
    /// <param name="pointC">点C</param>
    /// <returns>3点が一直線上にある場合True，ない場合Flase</returns>
    public bool ThreePointStraightJudge(Vector3 pointA, Vector3 pointB, Vector3 pointC) {
        //3点が一直線上にあるかどうか（3点によってできる三角形の面積が0かどうか）
        if ((pointC.y * (pointA.x - pointB.x)) + (pointA.y * (pointB.x - pointC.x)) + (pointB.y * (pointC.x - pointA.x)) == 0) {
            return true;
        }

        return false;
    }

    /// <summary>
    /// ある点がある線分上にあるかどうかの判定
    /// </summary>
    /// <param name="side">線分の端点の座標配列</param>
    /// <param name="point">判定する点の座標</param>
    /// <returns>点が線分上にある場合True，ない場合Flase</returns>
    public bool OnLineSegment(Vector3[] side, Vector3 point) {
        //pointがsideのx座標の範囲内にあるかどうか
        if ((side[0].x <= point.x && point.x <= side[1].x) || (side[1].x <= point.x && point.x <= side[0].x)) {
            //pointがsideのy座標の範囲内にあるかどうか
            if ((side[0].y <= point.y && point.y <= side[1].y) || (side[1].y <= point.y && point.y <= side[0].y)) {
                //3点が一直線上にあるかどうか（3点によってできる三角形の面積が0かどうか）
                if ((point.y * (side[0].x - side[1].x)) + (side[0].y * (side[1].x - point.x)) + (side[1].y * (point.x - side[0].x)) == 0) {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// 全座標が0かどうかを判定
    /// </summary>
    /// <param name="room">判定する座標</param>
    /// <returns>全ての座標が0ならtrue，そうでなければfalse</returns>
    public bool ZeroJudge(Vector3[] room) {
        bool flag = false;
        
        if (room.All(i => i == Vector3.zero)) {
            flag = true;
        }

        return flag;
    }

    /// <summary>
    /// 部屋の面積を計算
    /// </summary>
    /// <param name="room">計算する座標</param>
    /// <returns>部屋の面積(m^2)</returns>
    public float areaCalculation(Vector3[] room) {
        float area = 0;

        for (int i = 0; i < room.Length; i++) {
            float x, y_before_x, y_after_x;

            x = room[i].x;
            y_before_x = room[(i+room.Length-1)%room.Length].y;
            y_after_x = room[(i+1)%room.Length].y;

            area += x * (y_after_x - y_before_x);
        }

        return Mathf.Abs((area / 2) / 1000000);
    }
}
