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
    public GameObject CreateRoom(string name, Vector3[] positions) {
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

    public void CreateRoom(string name, Vector3[] positions, UnityEngine.Color color) {
        CreateRoom(name, positions);

        //色の変更
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }

    /// <summary>
    /// 線分と多角形の接している座標を求める
    /// </summary>
    /// <param name="line">線分</param>
    /// <param name="polygon">多角形</param>
    /// <returns>線分と多角形に接しているときその座標配列を返し，接していないとき空の配列を返す</returns>
    public Vector3[] ContactCoordinates(Vector3[] line, Vector3[] polygon) {
        try {
            //lineが線分でなかった場合にエラーを出す
            if (line.Length != 2) {
                throw new Exception("\"line\" is not line segment in \"ContactCoodinates\"");
            }
        }
        catch (Exception e) {
            //エラーを出力
            Debug.LogError(e.Message);
        }

        //4点の座標を格納する変数
        Vector3 A1, A2, B1, B2;

        //部屋Aのすべての辺を調べる   
        A1 = line[0];
        A2 = line[1];
        
        //部屋のすべての辺を調べる
        for (int j = 0; j < polygon.Length; j++) {
            B1 = polygon[j];
            B2 = polygon[(j+1)%polygon.Length];

            //2つの部屋が接しているとき
            string positionRelation = LinePositionRelation(line, new Vector3[] {B1, B2});
            if (positionRelation == "match" || positionRelation == "include" || positionRelation == "overlap") {
                //座標をソートするためのリスト
                List<Vector3> coordinatesListToSort = new List<Vector3>(){A1, A2, B1, B2};

                //x座標の昇順にソート
                coordinatesListToSort.Sort((a, b) => a.x.CompareTo(b.x));

                //対象の辺がy軸平行のとき
                if (A1.x == A2.x) {
                    //y座標の昇順にソート
                    coordinatesListToSort.Sort((a, b) => a.y.CompareTo(b.y));
                }

                //接している辺の共通部分（昇順の真ん中2つ）を返す
                return new Vector3[] {coordinatesListToSort[1], coordinatesListToSort[2]};
            }
        }

        //どれにも当てはまらなかったとき，空の配列を返す
        return new Vector3[]{};
    }

    /// <summary>
    /// 線分と多角形が接しているかどうかを判定
    /// </summary>
    /// <param name="line">線分</param>
    /// <param name="polygon">多角形</param>
    /// <returns>線分と多角形に接しているときTrue，接していないときFalseを返す</returns>
    public bool ContactJudge(Vector3[] line, Vector3[] polygon) {
        try {
            //lineが線分でなかった場合にエラーを出す
            if (line.Length != 2) {
                throw new Exception("line is not line segment in ContactCoodinates");
            }
        }
        catch (Exception e) {
            //エラーを出力
            Debug.LogError(e.Message);
        }

        //4点の座標を格納する変数
        Vector3 A1, A2, B1, B2;

        //部屋Aのすべての辺を調べる   
        A1 = line[0];
        A2 = line[1];
        
        //部屋のすべての辺を調べる
        for (int j = 0; j < polygon.Length; j++) {
            B1 = polygon[j];
            B2 = polygon[(j+1)%polygon.Length];

            //2つの部屋が接しているとき
            string positionRelation = LinePositionRelation(line, new Vector3[] {B1, B2});
            if (positionRelation == "match" || positionRelation == "include" || positionRelation == "overlap") {
                //trueを返す
                return true;
            }
        }

        //falseを返す
        return false;
    }

    /// <summary>
    /// 一直線上にある2つの線分がどのような位置関係にあるかを判定
    /// </summary>
    /// <param name="lineA">線分A</param>
    /// <param name="lineB">線分B</param>
    /// <returns>一直線上に無い場合not straight，一致する場合match，端点のみが重なる場合point overlap，完全に包含されてる場合include，一部重なっている場合overlap，全く重なっていない場合not overlap</returns>
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
        if ((s1 == 0 && (line1[0] - line2[0]) == 0 && (line1[1] - line2[1]) == 0) && s2 < 0) {
            //一致
            return "match";
        }
        else if (s1 > 0 && s2 == 0) {
            //端点が重なっている
            return "point overlap";
        }
        else if (s1 <= 0 && s2 < 0) {
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
    /// 多角形を原点周りに90, 180, 270°回転させる
    /// </summary> 
    /// <param name="polygon">回転させたい多角形の座標配列</param>
    /// <param name="angle">回転させたい角度</param>
    /// <returns>回転させた後の座標配列</returns>
    public Vector3[] Rotation(Vector3[] polygon, int angle) {
        //回転させた後の座標を格納する配列
        Vector3[] rotatedCoordinates = new Vector3[polygon.Length];

        //90°回転させる
        if (angle == 90) {
            for (int i = 0; i < polygon.Length; i++) {
                rotatedCoordinates[i].x = - polygon[i].y;
                rotatedCoordinates[i].y = polygon[i].x;
            }
            
            //先頭の座標が一番左上になるように座標を並び替える
            rotatedCoordinates = topArrange(rotatedCoordinates);
        }
        //180°回転させる
        else if (angle == 180) {
            for (int i = 0; i < polygon.Length; i++) {
                rotatedCoordinates[i].x = - polygon[i].x;
                rotatedCoordinates[i].y = - polygon[i].y;
            }

            //先頭の座標が一番左上になるように座標を並び替える
            rotatedCoordinates = topArrange(rotatedCoordinates);
        }
        //270°回転させる
        else if (angle == 270) {
            for (int i = 0; i < polygon.Length; i++) {
                rotatedCoordinates[i].x = polygon[i].y;
                rotatedCoordinates[i].y = - polygon[i].x;
            }

            //先頭の座標が一番左上になるように座標を並び替える
            rotatedCoordinates = topArrange(rotatedCoordinates);
        }

        return rotatedCoordinates;
    }

    /// <summary>
    /// 先頭の座標が一番左上になるように座標を並び替える
    /// </summary> 
    /// <param name="coordinates">並び替えたい座標配列</param>
    /// <returns>並び替えた座標配列</returns>
    public Vector3[] topArrange(Vector3[] coodinates) {
        //並べ替えた座標配列
        Vector3[] arrangedCoordinates = new Vector3[coodinates.Length];

        //一番小さいx座標を見つける
        float minX = coodinates[0].x;
        for (int i = 1; i < coodinates.Length; i++) {
            if (minX > coodinates[i].x) {
                minX = coodinates[i].x;
            }
        }

        //x座標が小さいもののうち，一番大きいy座標を探す
        float maxY = float.MinValue;
        for (int i = 1; i < coodinates.Length; i++) {
            if (maxY < coodinates[i].y) {
                //x座標が一番小さいもののうち，このy座標が存在するとき
                if (coodinates.Contains(new Vector3(minX, coodinates[i].y, 0))) {
                    maxY = coodinates[i].y;
                }
            }
        }

        //先頭の座標
        Vector3 topCoordinates = new Vector3(minX, maxY, 0);
        
        //先頭の座標と一致する座標のインデックスを見つける
        int topIndex = 0;
        for (int i = 0; i < coodinates.Length; i++) {
            if (coodinates[i] == topCoordinates) {
                topIndex = i;
                break;
            }
        }

        //topIndexが先頭になるように並び替える
        for (int i = 0; i < coodinates.Length; i++) {
            arrangedCoordinates[i] = coodinates[(topIndex+i)%coodinates.Length];
        }

        return arrangedCoordinates;
    }

    /// <summary>
    /// 全座標が0かどうかを判定
    /// </summary>
    /// <param name="line">判定する座標</param>
    /// <returns>全ての座標が0ならtrue，そうでなければfalse</returns>
    public bool ZeroJudge(Vector3[] polygon) {
        bool flag = false;
        
        if (polygon.All(i => i == Vector3.zero)) {
            flag = true;
        }

        return flag;
    }

    /// <summary>
    /// 線分の傾きを求める
    /// </summary>
    /// <param name="line">傾きを求める線分</param>
    /// <returns>線分の傾き, y軸平行のときのみ無限を返す</returns>
    public float Slope(Vector3[] line) {
        //傾き
        float slope = 0f;

        //線分の端点の座標
        Vector3 l1 = line[0];
        Vector3 l2 = line[1];

        //線分がy軸平行のとき
        if (l1.x == l2.x) {
            slope = Mathf.Infinity;
        }
        //それ以外のとき
        else {
            slope = (l2.y - l1.y) / (l2.x - l1.x);
        }
        
        //傾きを返す
        return slope;
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