using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CommonFunctions : MonoBehaviour
{
    [SerializeField] PlanReader pr;

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
    /// 多角形と多角形の接している座標のリストを求める
    /// </summary>
    /// <param name="polygonA">多角形A</param>
    /// <param name="polygonB">多角形B</param>
    /// <returns>多角形と多角形に接しているときその座標配列を返し，接していないとき空の配列を返す</returns>
    public List<Vector3[]> ContactCoordinates(Vector3[] polygonA, Vector3[] polygonB) {
        //返すリスト
        List<Vector3[]> contactCoordinatesList = new List<Vector3[]>();

        for (int i = 0; i < polygonA.Length; i++) {
            //lineを定義
            Vector3[] line = new Vector3[] {polygonA[i], polygonA[(i+1)%polygonA.Length]};

            //lineとpolygonBが接しているとき
            if (ContactJudge(line, polygonB)) {
                //接している座標を求める
                Vector3[] contactCoordinates = LineContactCoordinates(line, polygonB);

                //結果に追加
                contactCoordinatesList.Add(contactCoordinates);
            }
        }

        //結果を返す
        return contactCoordinatesList;
    }
    
    /// <summary>
    /// 線分と多角形の接している座標を求める
    /// </summary>
    /// <param name="line">線分</param>
    /// <param name="polygon">多角形</param>
    /// <returns>線分と多角形に接しているときその座標配列を返し，接していないとき空の配列を返す</returns>
    public Vector3[] LineContactCoordinates(Vector3[] line, Vector3[] polygon) {
        try {
            //lineが線分でなかった場合にエラーを出す
            if (line.Length != 2) {
                throw new Exception("\"line\" is not line segment in \"ContactCoordinates\"");
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
    /// 多角形と多角形が接しているかどうかを判定
    /// </summary>
    /// <param name="polygonA">多角形A</param>
    /// <param name="polygonB">多角形B</param>
    /// <returns>多角形と多角形に接しているときTrue，接していないときFalseを返す</returns>
    public bool ContactJudge(Vector3[] polygonA, Vector3[] polygonB) {
        //polygonAのすべての辺を調べる
        for (int i = 0; i < polygonA.Length; i++) {
            //lineを定義
            Vector3[] line = new Vector3[] {polygonA[i], polygonA[(i+1)%polygonA.Length]};

            //lineとpolygonBが接しているとき
            if (LineContactJudge(line, polygonB)) {
                //trueを返す
                return true;
            }
        }

        //falseを返す
        return false;
    }
    
    /// <summary>
    /// 線分と多角形が接しているかどうかを判定
    /// </summary>
    /// <param name="line">線分</param>
    /// <param name="polygon">多角形</param>
    /// <returns>線分と多角形に接しているときTrue，接していないときFalseを返す</returns>
    public bool LineContactJudge(Vector3[] line, Vector3[] polygon) {
        try {
            //lineが線分でなかった場合にエラーを出す
            if (line.Length != 2) {
                throw new Exception("line is not line segment in ContactCoordinates");
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
    /// 辺Aから辺Aと辺Bの重なっている部分を除いた辺を返す
    /// </summary>
    /// <param name="sideA">辺Aの座標</param>
    /// <param name="sideB">辺Bの座標</param>
    /// <returns>辺Aから辺Aと辺Bの重なっている部分を除いた辺の座標</returns>
    public List<Vector3[]> SideSubstraction(Vector3[] sideA, Vector3[] sideB) {
        //結果
        List<Vector3[]> result = new List<Vector3[]>();

        //2辺の位置関係
        string positionRelation = LinePositionRelation(sideA, sideB);

        //線で重なっている部分がない場合
        if (positionRelation == "not overlap" || positionRelation == "point overlap" || positionRelation == "not straight") {
            result.Add(sideA);
        }
        //エラー
        else if (positionRelation == "error") {
            Debug.LogError("Error in SideSubstraction");
        }
        //線で重なっている部分がある場合
        else {
            //辺Aと辺Bの重なっている部分
            Vector3[] overlap = ContactCoordinates(sideA, sideB)[0];

            //辺Aと重なっている部分の座標の順が逆の場合
            if (Vector3.Dot(sideA[1] - sideA[0], overlap[1] - overlap[0]) < 0) {
                //重なっている部分の座標を入れ替え
                Array.Reverse(sideA);
            }

            //辺Aと辺Bが一致している場合
            if (positionRelation == "match") {
                //重なっている部分を除いた辺の座標を返す
                result.Add(new Vector3[] {Vector3.zero, Vector3.zero});
            }
            //どちらかが包含している場合
            else if (positionRelation == "include") {
                //辺Aが辺Bを包含している場合
                if (Vector3.Distance(sideA[0], sideA[1]) > Vector3.Distance(sideB[0], sideB[1])) {
                    //端点が重なっている場合
                    if (sideA[0] == overlap[0]) {
                        //重なっている部分を除いた辺の座標を返す
                        result.Add(new Vector3[] {overlap[1], sideA[1]});
                    } else if (sideA[1] == overlap[1]) {
                        //重なっている部分を除いた辺の座標を返す
                        result.Add(new Vector3[] {sideA[0], overlap[0]});
                    }
                    //端点が重なっていない場合
                    else {
                        //重なっている部分を除いた辺の座標を返す
                        result.Add(new Vector3[] {sideA[0], overlap[0]});
                        result.Add(new Vector3[] {overlap[1], sideA[1]});
                    }
                }
                //辺Bが辺Aを包含している場合
                else if (Vector3.Distance(sideA[0], sideA[1]) < Vector3.Distance(sideB[0], sideB[1])) {
                    //重なっている部分を除いた辺の座標を返す
                    result.Add(new Vector3[] {Vector3.zero, Vector3.zero});
                }
            }
            //重なっている場合
            else if (positionRelation == "overlap") {
                if (sideA[0] == overlap[0]) {
                    //重なっている部分を除いた辺の座標を返す
                    result.Add(new Vector3[] {overlap[1], sideA[1]});
                } else if (sideA[1] == overlap[1]) {
                    //重なっている部分を除いた辺の座標を返す
                    result.Add(new Vector3[] {sideA[0], overlap[0]});
                }
            }
        }

        return result;
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
    /// ある線分が別のある線分上にあるかどうかの判定
    /// </summary>
    /// <param name="outerSide">線分の座標配列</param>
    /// <param name="innerSide">線分上にあってほしい線分の座標配列</param>
    /// <returns>線分が線分上にある場合True，ない場合Flase</returns>
    public bool OnLineSegment(Vector3[] outerSide, Vector3[] innerSide) {
        bool flag = true;
        
        //線分上にあってほしい線分について
        for (int i = 0; i < innerSide.Length; i++) {
            //全ての点が線分上にあるかどうか
            if (!(flag && OnLineSegment(outerSide, innerSide[i]))) {
                flag = false;
                break;
            }
        }

        return flag;
    }

    /// <summary>
    /// 多角形の辺上(頂点含む)に点が含まれるかどうか
    /// </summary> 
    /// <param name="polygon">多角形の座標配列</param>
    /// <param name="point">含まれるか調べる点の座標</param>
    /// <returns>内分の場合true，外分の場合flase</returns>
    public bool OnPolyogon(Vector3[] polygon, Vector3 point) {
        //判定
        bool flag = false;

        //多角形の辺に含まれるかどうか
        for (int i = 0; i < polygon.Length; i++) {
            //多角形の辺
            Vector3[] polyognSide = new Vector3[]{polygon[i], polygon[(i+1)%polygon.Length]};

            //多角形の辺上に点が含まれる場合
            if (OnLineSegment(polyognSide, point)) {
                flag = true;
                break;
            }
        }

        return flag;
    }

    /// <summary>
    /// 多角形の辺上(頂点含む)に線分が含まれるかどうか
    /// </summary> 
    /// <param name="polygon">多角形の座標配列</param>
    /// <param name="line">含まれるか調べる線分の座標</param>
    /// <returns>内分の場合true，外分の場合flase</returns>
    public bool OnPolyogon(Vector3[] polygon, Vector3[] line) {
        //判定
        bool flag = false;

        //多角形の辺に含まれるかどうか
        for (int i = 0; i < polygon.Length; i++) {
            //多角形の辺
            Vector3[] polyognSide = new Vector3[]{polygon[i], polygon[(i+1)%polygon.Length]};

            //多角形の辺上に線分が含まれる場合
            if (OnLineSegment(polyognSide, line)) {
                flag = true;
                break;
            }
        }

        return flag;
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

        //回転させる角度をラジアンに変換
        float rad = angle * Mathf.Deg2Rad; //Deg2Rad = (PI * 2) / 360 
        
        //回転させる
        for (int i = 0; i < polygon.Length; i++) {
            rotatedCoordinates[i].x = Mathf.Round(polygon[i].x * Mathf.Cos(rad) - polygon[i].y * Mathf.Sin(rad));
            rotatedCoordinates[i].y = Mathf.Round(polygon[i].x * Mathf.Sin(rad) + polygon[i].y * Mathf.Cos(rad));
        }
            
        //先頭の座標が一番左上になるように座標を並び替える
        rotatedCoordinates = TopArrange(rotatedCoordinates);

        //return VectorClean(rotatedCoordinates);
        return rotatedCoordinates;
    }

    /// <summary>
    /// 先頭の座標が一番左上になるように座標を並び替える
    /// </summary> 
    /// <param name="coordinates">並び替えたい座標配列</param>
    /// <returns>並び替えた座標配列</returns>
    public Vector3[] TopArrange(Vector3[] coordinates) {
        //並べ替えた座標配列
        Vector3[] arrangedCoordinates = new Vector3[coordinates.Length];

        //一番小さいx座標を見つける
        float minX = coordinates[0].x;
        for (int i = 1; i < coordinates.Length; i++) {
            if (minX > coordinates[i].x) {
                minX = coordinates[i].x;
            }
        }

        //x座標が小さいもののうち，一番大きいy座標を探す
        float maxY = float.MinValue;
        for (int i = 1; i < coordinates.Length; i++) {
            if (maxY < coordinates[i].y) {
                //x座標が一番小さいもののうち，このy座標が存在するとき
                if (coordinates.Contains(new Vector3(minX, coordinates[i].y, 0))) {
                    maxY = coordinates[i].y;
                }
            }
        }

        //先頭の座標
        Vector3 topCoordinates = new Vector3(minX, maxY, 0);
        
        //先頭の座標と一致する座標のインデックスを見つける
        int topIndex = 0;
        for (int i = 0; i < coordinates.Length; i++) {
            if (coordinates[i] == topCoordinates) {
                topIndex = i;
                break;
            }
        }

        //topIndexが先頭になるように並び替える
        for (int i = 0; i < coordinates.Length; i++) {
            arrangedCoordinates[i] = coordinates[(topIndex+i)%coordinates.Length];
        }

        return arrangedCoordinates;
    }

    /// <summary>
    /// 要らない座標（頂点を含まない辺上にある座標）を除く
    /// </summary> 
    /// <param name="coordinates">調べたい座標配列</param>
    /// <returns>要らない座標（頂点を含まない辺上にある座標）を除いた座標配列</returns>
    public Vector3[] RemoveExtraPoint(Vector3[] coordinates) {
        //要らない座標を除いた座標配列
        List<Vector3> result = new List<Vector3>();

        //座標配列をリストに変換
        List<Vector3> coordinatesList = new List<Vector3>(coordinates);

        //それぞれの辺について
        for (int i = 0; i < coordinatesList.Count; i++) {
            //調べる辺
            Vector3[] sideA = new Vector3[]{coordinatesList[(i+1)%coordinatesList.Count], coordinatesList[i]};
            Vector3[] sideB = new Vector3[]{coordinatesList[(i+1)%coordinatesList.Count], coordinatesList[(i+2)%coordinatesList.Count]};

            //座標配列で連続する2辺の内積を計算
            if (Vector3.Dot(sideA[1] - sideA[0], sideB[1] - sideB[0]) == - Vector3.Distance(sideA[1], sideA[0]) * Vector3.Distance(sideB[1], sideB[0])) {
                //座標配列から削除
                coordinatesList.Remove(sideA[0]);
                i--;
            }
        }

        //リストを配列に変換して返す
        return coordinatesList.ToArray();
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
    /// 線分の長さを求める
    /// </summary>
    /// <param name="line">長さを求めたい線分</param>
    /// <returns>線分の長さ</returns>
    public float GetLength(Vector3[] line) {
        try {
            //lineが線分でなかった場合にエラーを出す
            if (line.Length != 2) {
                throw new Exception("line is not line segment in Length");
            }
        }
        catch (Exception e) {
            //エラーを出力
            Debug.LogError(e.Message);
        }

        //線分の長さ
        float length = 0f;

        //長さを求める
        length = Vector3.Distance(line[0], line[1]); 

        //線分の長さを返す
        return length;
    }

    /// <summary>
    /// 多角形のxかy座標の最大値か最小値を求める
    /// </summary>
    /// <param name="polygon">多角形の座標</param>
    /// <param name="axis">x, y座標どちらを求めるか</param>
    /// <param name="maxOrMin">最大値と最小値どちらを求めるか</param>
    /// <returns>多角形のxかy座標の最大値か最小値</returns>
    public float GetMaxOrMin(Vector3[] polygon, string axis, string maxOrMin) {
        //求める値
        float value = 0f;

        //求める値を初期化
        if (axis == "x") {
            value = polygon[0].x;
        }
        else if (axis == "y") {
            value = polygon[0].y;
        }
            

        //最大値か最小値を求める
        for (int i = 1; i < polygon.Length; i++) {
            if (axis == "x") {
                if (maxOrMin == "max") {
                    if (value < polygon[i].x) {
                        value = polygon[i].x;
                    }
                }
                else if (maxOrMin == "min") {
                    if (value > polygon[i].x) {
                        value = polygon[i].x;
                    }
                }
            
            }
            else if (axis == "y") {
                if (maxOrMin == "max") {
                    if (value < polygon[i].y) {
                        value = polygon[i].y;
                    }
                }
                else if (maxOrMin == "min") {
                    if (value > polygon[i].y) {
                        value = polygon[i].y;
                    }
                }
            }
        }

        return value;
    }
    
    /// <summary>
    /// 部屋の面積を計算
    /// </summary>
    /// <param name="room">計算する座標</param>
    /// <returns>部屋の面積(m^2)</returns>
    public float AreaCalculation(Vector3[] room) {
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

    /// <summary>
    /// 上書きを防ぐための複製
    /// </summary> 
    /// <param name="originalDictionary">複製したい辞書</param>
    /// <returns>複製したい辞書に影響を与えない同じ内容の辞書を返す</returns>
    public Dictionary<string, Dictionary<string, Vector3[]>> DuplicateDictionary(Dictionary<string, Dictionary<string, Vector3[]>> originalDictionary) {
        var result = new Dictionary<string, Dictionary<string, Vector3[]>>();

        foreach (KeyValuePair<string, Dictionary<string, Vector3[]>> space in originalDictionary) {
            var spaceResult = new Dictionary<string, Vector3[]>();

            foreach (KeyValuePair<string, Vector3[]> spaceElements in space.Value) {
                spaceResult.Add(spaceElements.Key, spaceElements.Value);
            }

            result.Add(space.Key, spaceResult);
        }

        return result;
    }

    /// <summary>
    /// 上書きを防ぐための複製
    /// </summary> 
    /// <param name="originalList">複製したいリスト</param>
    /// <returns>複製したいリストに影響を与えない同じ内容のリストを返す</returns>
    public List<Dictionary<string, Dictionary<string, Vector3[]>>> DuplicateList(List<Dictionary<string, Dictionary<string, Vector3[]>>> originalList) {
        var result = new List<Dictionary<string, Dictionary<string, Vector3[]>>>();

        foreach (Dictionary<string, Dictionary<string, Vector3[]>> originalDictionary in originalList) {
            var dictionaryResult = new Dictionary<string, Dictionary<string, Vector3[]>>();

            foreach (KeyValuePair<string, Dictionary<string, Vector3[]>> space in originalDictionary) {
                var spaceResult = new Dictionary<string, Vector3[]>();

                foreach (KeyValuePair<string, Vector3[]> spaceElements in space.Value) {
                    spaceResult.Add(spaceElements.Key, spaceElements.Value);
                }

                dictionaryResult.Add(space.Key, spaceResult);
            }

            result.Add(dictionaryResult);
        }

        return result;
    }

    /// <summary>
    /// 多角形とその辺上にある線分について，線分から上下左右どちらに動かすと多角形内部に入るかを判定
    /// 線分の中点を上下・左右どちらかに1だけずらす．y軸平行の時は左右，それ以外のときは上下にずらす
    /// </summary>
    /// <param name="polygon">多角形の座標</param>
    /// <param name="line">線分の座標</param>
    /// <returns>右にずらすと内部にあるとき1, 左にずらすと内部にあるとき-1, 上にずらすと内部にあるとき2, 下にずらすと内部にあるとき-2, それ以外は0を返す</returns>
    public int ShiftJudge(Vector3[] polygon, Vector3[] line) {
        //返す値
        int result = 0;

        //エラー処理
        try {
            //lineが多角形の辺上になかった場合にエラーを出す
            for (int i = 0; i < polygon.Length; i++) {
                if (OnLineSegment(new Vector3[]{polygon[i], polygon[(i+1)%polygon.Length]}, line[0]) && OnLineSegment(new Vector3[]{polygon[i], polygon[(i+1)%polygon.Length]}, line[1])) {
                    break;
                }
            }

            //TODO: エラー処理をちゃんとする
            //throw new Exception("\"line\" is not on the egde of \"polygon\" in \"ShiftJudge\"");
        }
        catch (Exception e) {
            //エラーを出力
            Debug.LogError(e.Message);
        }

        //線分の中点
        Vector3 midPoint = new Vector3((line[0].x + line[1].x) / 2, (line[0].y + line[1].y) / 2, 0);

        //線分がy軸平行のとき
        if (Slope(line) == Mathf.Infinity) {
            //線分の中点を左右に1だけずらした点が多角形の内部にあるかどうかを判定
            if (CheckPoint(polygon, new Vector3(midPoint.x - 1, midPoint.y, 0))) {
                result = -1;
            }
            else if (CheckPoint(polygon, new Vector3(midPoint.x + 1, midPoint.y, 0))) {
                result = 1;
            }
        }
        //線分がy軸平行でないとき
        else {
            //線分の中点を上下に1だけずらした点が多角形の内部にあるかどうかを判定
            if (CheckPoint(polygon, new Vector3(midPoint.x, midPoint.y - 1, 0))) {
                result = -2;
            }
            else if (CheckPoint(polygon, new Vector3(midPoint.x, midPoint.y + 1, 0))) {
                result = 2;
            }
        }

        //結果を返す
        return result;
    }

    /// <summary>
    /// 多角形が長方形かの判定
    /// </summary> 
    /// <param name="polygon">多角形の座標</param>
    /// <returns>多角形がピッタリ収まる長方形の座標配列</returns>
    public bool RectangleJudge(Vector3[] polygon) {
        //返す値
        bool flag = false;

        //頂点が4つのとき
        if (polygon.Length == 4) {
            //多角形の1辺の長さ
            float sideLength = Vector3.Distance(polygon[0], polygon[1]);
            //向かい合わないもう1辺の長さ
            float anotherSideLength = Vector3.Distance(polygon[1], polygon[2]);

            //多角形の1辺の長さと向かい合わないもう1辺の長さが異なるとき
            if (sideLength != anotherSideLength) {
                flag = true;
            }
        }
        
        //結果を返す
        return flag;
    }

    /// <summary>
    /// 多角形（x軸，y軸に平行な辺をしかない）がピッタリ収まる長方形の座標を求める
    /// </summary> 
    /// <param name="polygon">多角形の座標</param>
    /// <returns>多角形がピッタリ収まる長方形の座標配列</returns>
    public Vector3[] MakeRectangle(Vector3[] polygon) {
        //返す長方形の座標配列
        Vector3[] rectangle = new Vector3[4];

        //多角形のx座標の最大値と最小値
        float maxX = polygon[0].x;
        float minX = polygon[0].x;

        //多角形のy座標の最大値と最小値
        float maxY = polygon[0].y;
        float minY = polygon[0].y;

        //多角形のx座標の最大値と最小値を求める
        for (int i = 1; i < polygon.Length; i++) {
            if (polygon[i].x > maxX) {
                maxX = polygon[i].x;
            }
            else if (polygon[i].x < minX) {
                minX = polygon[i].x;
            }
        }

        //多角形のy座標の最大値と最小値を求める
        for (int i = 1; i < polygon.Length; i++) {
            if (polygon[i].y > maxY) {
                maxY = polygon[i].y;
            }
            else if (polygon[i].y < minY) {
                minY = polygon[i].y;
            }
        }

        //長方形の座標を求める
        rectangle[0] = new Vector3(minX, maxY, 0);
        rectangle[1] = new Vector3(maxX, maxY, 0);
        rectangle[2] = new Vector3(maxX, minY, 0);
        rectangle[3] = new Vector3(minX, minY, 0);

        return rectangle;
    }

    /// <summary>
    /// 多角形（x軸，y軸に平行な辺をしかない）がピッタリ収まる長方形の幅を求める
    /// </summary> 
    /// <param name="polygon">多角形の座標</param>
    /// <returns>多角形がピッタリ収まる長方形の幅</returns>
    public float CalculateRectangleWidth(Vector3[] polygon) {
        //返す長方形の幅
        float width = MakeRectangle(polygon)[1].x - MakeRectangle(polygon)[0].x;

        return width;
    }

    /// <summary>
    /// 多角形（x軸，y軸に平行な辺をしかない）がピッタリ収まる長方形の高さを求める
    /// </summary> 
    /// <param name="polygon">多角形の座標</param>
    /// <returns>多角形がピッタリ収まる長方形の高さ</returns>
    public float CalculateRectangleHeight(Vector3[] polygon) {
        //返す長方形の高さ
        float height = MakeRectangle(polygon)[0].y - MakeRectangle(polygon)[3].y;

        return height;
    }

    /// <summary>
    /// 全ての座標を同じだけ移動させる
    /// </summary> 
    /// <param name="shapes">動かしたい座標配列</param>
    /// <param name="correctValue">移動させる距離</param>
    /// <returns>移動させた後の座標配列</returns>
    public Vector3[] CorrectCoordinates(Vector3[] shapes, Vector3 correctValue) {
        Vector3[] correctedCoordinates = new Vector3[shapes.Length];

        for (int i = 0; i < shapes.Length; i++) {
            correctedCoordinates[i] = new Vector3(shapes[i].x + correctValue.x, shapes[i].y + correctValue.y, 0);
        }

        return correctedCoordinates;
    }

    /// <summary>
    /// 図形に対する点の内外判定
    /// </summary> 
    /// <param name="points">図形の座標配列</param>
    /// <param name="target">判定する点の座標</param>
    /// <returns>内分の場合True，外分の場合Flase</returns>
    public bool CheckPoint(Vector3[] points, Vector3 target) {
        Vector3 normal = new Vector3(1f, 0f, 0f);
        //Vector3 normal = Vector3.up;//(0, 1, 0)
        // XY平面上に写像した状態で計算を行う
        Quaternion rot = Quaternion.FromToRotation(normal, -Vector3.forward);

        Vector3[] rotPoints = new Vector3[points.Length];

        for (int i = 0; i < rotPoints.Length; i++) {
            rotPoints[i] = rot * points[i];
        }

        target = rot * target;

        int wn = 0;
        float vt = 0;

        for (int i = 0; i < rotPoints.Length; i++) {
            // 上向きの辺、下向きの辺によって処理を分ける

            int cur = i;
            int next = (i + 1) % rotPoints.Length;

            // 上向きの辺。点PがY軸方向について、始点と終点の間にある。（ただし、終点は含まない）
            if ((rotPoints[cur].y <= target.y) && (rotPoints[next].y > target.y)) {
                // 辺は点Pよりも右側にある。ただし重ならない
                // 辺が点Pと同じ高さになる位置を特定し、その時のXの値と点PのXの値を比較する
                vt = (target.y - rotPoints[cur].y) / (rotPoints[next].y - rotPoints[cur].y);

                if (target.x < (rotPoints[cur].x + (vt * (rotPoints[next].x - rotPoints[cur].x)))) {
                    // 上向きの辺と交差した場合は+1
                    wn++;
                }
            }
            else if ((rotPoints[cur].y > target.y) && (rotPoints[next].y <= target.y)) {
                // 辺は点Pよりも右側にある。ただし重ならない
                // 辺が点Pと同じ高さになる位置を特定し、その時のXの値と点PのXの値を比較する
                vt = (target.y - rotPoints[cur].y) / (rotPoints[next].y - rotPoints[cur].y);

                if (target.x < (rotPoints[cur].x + (vt * (rotPoints[next].x - rotPoints[cur].x)))) {
                    // 下向きの辺と交差した場合は-1
                    wn--;
                }
            }
        }

        return wn != 0;
    }

    /// <summary>
    /// 多角形が別の多角形の内部（辺上も可）にあるかどうかの判定
    /// </summary> 
    /// <param name="outer">外側にあってほしい多角形の座標配列</param>
    /// <param name="inner">内側にあってほしい多角形の座標配列</param>
    /// <returns>多角形が別の多角形の内部にある場合True，ない場合Flase</returns>
    public bool JudgeInside(Vector3[] outer, Vector3[] inner) {
        bool flag = false;
        //点がいくつ内側にあるかを数える
        int insideCounter = 0;

        //内側にあってほしい多角形の頂点を全て調べる
        for (int i = 0; i < inner.Length; i++) {
            //まず辺上にあるかどうかを調べる
            bool onLineFlag = false;
            for (int j = 0; j < outer.Length; j++) {
                if (OnLineSegment(new Vector3[]{outer[j], outer[(j+1)%outer.Length]}, new Vector3(inner[i].x, inner[i].y, 0))) {
                    insideCounter++;
                    onLineFlag = true;
                    break;
                }
            }
            
            //辺上にある場合は次の頂点へ
            if (onLineFlag) {
                continue;
            }

            //次に，内部にあるかどうかを調べる
            if (CheckPoint(outer, new Vector3(inner[i].x, inner[i].y, 0))) {
                insideCounter++;
            }
        }

        //内側にある点の数が内側の多角形の頂点の数と同じ場合，内側にあると判定
        if (insideCounter == inner.Length) {
            flag = true;
        }

        return flag;
    }

    /// <summary>
    /// 多角形が別の多角形の外部（辺上も可）にあるかどうかの判定
    /// </summary> 
    /// <param name="polygonA">多角形Aの座標配列</param>
    /// <param name="polygonB">多角形Bの座標配列</param>
    /// <returns>多角形が別の多角形の外部にある場合True，ない場合Flase</returns>
    public bool JudgeOutside(Vector3[] polygonA, Vector3[] polygonB) {
        bool flag = false;
        //点がいくつ外側にあるかを数える
        int outsideCounter = 0;

        //多角形Bの頂点を全て調べる
        for (int i = 0; i < polygonB.Length; i++) {
            //まず辺上にあるかどうかを調べる
            bool onLineFlag = false;
            for (int j = 0; j < polygonA.Length; j++) {
                if (OnLineSegment(new Vector3[]{polygonA[j], polygonA[(j+1)%polygonA.Length]}, new Vector3(polygonB[i].x, polygonB[i].y, 0))) {
                    outsideCounter++;
                    onLineFlag = true;
                    break;
                }
            }

            //辺上にある場合は次の頂点へ
            if (onLineFlag) {
                continue;
            }

            //次に，外部にあるかどうかを調べる
            if (!CheckPoint(polygonA, new Vector3(polygonB[i].x, polygonB[i].y, 0))) {
                outsideCounter++;
            }
        }
        
        //外側にある点の数が多角形Bの頂点の数と同じ場合，外側にあると判定
        if (outsideCounter == polygonB.Length) {
            flag = true;
        }

        return flag;
    }

    /// <summary>
    /// 間取図の重複を削除
    /// </summary> 
    /// <param name="allPattern">全ての配置結果</param>
    /// <returns>重複を削除した結果</returns>
    public List<Dictionary<string, Dictionary<string, Vector3[]>>> RemoveDuplicates(List<Dictionary<string, Dictionary<string, Vector3[]>>> allPattern) {
        int trueCounter = 0;
        int dwellingNumber = 1/*pr.getDwellingCoordinatesList().Count*/;

        //重複を削除
        for (int i = 0; i < allPattern.Count - 1; i++) {
            for (int j = i + 1; j < allPattern.Count; j++) {

                trueCounter = 0;
                for (int k = 0; k < dwellingNumber; k++) {
                    if (DictionaryEquals(allPattern[i]["Dwelling" + (k + 1)], allPattern[j]["Dwelling" + (k + 1)])) {
                        trueCounter++;
                    }  
                }

                if (trueCounter == dwellingNumber) {
                    allPattern.RemoveAt(j);
                    j--;
                }
            }
        }

        return allPattern;
    }

    /// <summary>
    /// 2つの辞書が等しいかどうかを判定
    /// </summary> 
    /// <param name="dictionaryA">辞書A</param>
    /// <param name="dictionaryB">辞書B</param>
    /// <returns>2つの辞書が等しいときtrue, そうでないときfalse</returns>
    public bool DictionaryEquals(Dictionary<string, Vector3[]> dictionaryA, Dictionary<string, Vector3[]> dictionaryB) {
        //判定結果
        bool flag = false;

        //辞書の要素数が異なる場合
        if (dictionaryA.Count != dictionaryB.Count) {
            return flag;
        }

        //辞書のキーが異なる場合
        foreach (string key in dictionaryA.Keys) {
            if (!dictionaryB.ContainsKey(key)) {
                return flag;
            }
        }

        //辞書の値の長さが異なる場合
        foreach (string key in dictionaryA.Keys) {
            if (dictionaryA[key].Length != dictionaryB[key].Length) {
                return flag;
            }
        }

        //辞書の値が異なる場合
        foreach (string key in dictionaryA.Keys) {
            //住戸とバルコニーは判定しない
            if (key.Contains("1K") || key.Contains("Balcony")) {
                continue;
            }

            for (int i = 0; i < dictionaryA[key].Length; i++) {
                dictionaryA[key][i].x = NumberClean(dictionaryA[key][i].x);
                if (NumberClean(dictionaryA[key][i].x) != NumberClean(dictionaryB[key][i].x)) {
                    return flag;
                }

                if (NumberClean(dictionaryA[key][i].y) != NumberClean(dictionaryB[key][i].y)) {
                    return flag;
                }
            }
        }

        flag = true;

        return flag;
    }

    /// <summary>
    /// 数字の小数点以下を切り捨てる
    /// </summary> 
    /// <param name="num">数字</param>
    /// <returns>小数点以下を切り捨てた数字</returns>
    public float NumberClean (float num) {
        //返す数字
        float cleanNum = 0;
        
        //小数点以下を切り捨てる
        cleanNum = (float) Math.Truncate(num);
        
        return cleanNum;
    }

    /// <summary>
    /// 配列の数値の小数点以下を切り捨てる
    /// </summary> 
    /// <param name="coordinates">座標配列</param>
    /// <returns>小数点以下を切り捨てた数字</returns>
    public Vector3[] VectorClean (Vector3[] coordinates) {
        //返す数字
        Vector3[] cleanVec = new Vector3[coordinates.Length];
        
        //配列の全ての要素について
        for (int i = 0; i < coordinates.Length; i++) {
            //小数点以下を切り捨てる
            cleanVec[i] = new Vector3(NumberClean(coordinates[i].x), NumberClean(coordinates[i].y), 0);
        }
        
        return cleanVec;
    }

    /// <summary>
    /// 部屋が重なっているパターンを削除
    /// </summary> 
    /// <param name="allPattern">全ての配置結果</param>
    /// <param name="roomNameToCheck">重なっているか調べたい部屋の名前</param>
    /// <returns>部屋が重なったものを削除した結果</returns>
    public List<Dictionary<string, Dictionary<string, Vector3[]>>> RemoveOverlap(List<Dictionary<string, Dictionary<string, Vector3[]>>> allPattern, string roomNameToCheck) {
        //削除した結果
        List<Dictionary<string, Dictionary<string, Vector3[]>>> result = DuplicateList(allPattern);

        //各パターンについて調べる
        for (int i = 0; i < result.Count; i++) {
            //問題があるかどうか
            bool problemFlag = false;

            //住戸・階段室のループ
            foreach (KeyValuePair<string, Dictionary<string, Vector3[]>> planParts in result[i]) {
                //問題があるとき
                if (problemFlag) {
                    //このパターンはもう調べる必要がないので次のパターンへ
                    break;
                }

                //住戸のとき
                if (planParts.Key.Contains("Dwelling")) {
                    //部屋のループ
                    foreach (KeyValuePair<string, Vector3[]> room in planParts.Value) {
                        //調べたい部屋かバルコニーのとき
                        if (room.Key.Contains(roomNameToCheck) || room.Key.Contains("Balcony")) {
                            //次の部屋へ
                            continue;
                        }

                        //住戸のとき
                        if (room.Key.Contains("1K")) {
                            //roomNameToCheckが住戸内にないとき
                            if (!JudgeInside(room.Value, planParts.Value[roomNameToCheck])) {
                                //問題あり
                                problemFlag = true;

                                //このパターンはもう調べる必要がないので次のパターンへ
                                break;
                            }
                        }
                        //その他の部屋について
                        else {
                            //roomNameToCheckがその部屋外にないとき 
                            if (!JudgeOutside(room.Value, planParts.Value[roomNameToCheck])) {
                                //問題あり
                                problemFlag = true;

                                //このパターンはもう調べる必要がないので次のパターンへ
                                break;
                            }
                        }
                    }            
                }
            }

            //問題があるとき
            if (problemFlag) {
                //このパターンは削除
                result.RemoveAt(i);
                i--;
            }
        }

        //結果を返す
        return result;
    }

    /// <summary>
    /// 辺からはみ出しているパターンを削除
    /// </summary>
    /// <param name="allPattern">全ての配置結果</param>
    /// <returns>MBPSが辺からはみ出しているパターンを削除した結果</returns>
    public List<Dictionary<string, Dictionary<string, Vector3[]>>> RemoveNotOnEdge(List<Dictionary<string, Dictionary<string, Vector3[]>>> allPattern, string roomNameToCheck) {
        //削除した結果
        List<Dictionary<string, Dictionary<string, Vector3[]>>> result = DuplicateList(allPattern);

        //各パターンについて調べる
        for (int i = 0; i < result.Count; i++) {
            //問題があるかどうか
            bool problemFlag = false;

            //住戸・階段室のループ
            foreach (KeyValuePair<string, Dictionary<string, Vector3[]>> planParts in result[i]) {
                //問題があるとき
                if (problemFlag) {
                    //このパターンはもう調べる必要がないので次のパターンへ
                    break;
                }

                //住戸のとき
                if (planParts.Key.Contains("Dwelling")) {
                    //部屋のループ
                    foreach (KeyValuePair<string, Vector3[]> room in planParts.Value) {
                        if (problemFlag) {
                            //このパターンはもう調べる必要がないので次のパターンへ
                            break;
                        }
                        //調べたい部屋のとき
                        if (room.Key.Contains(roomNameToCheck)) {
                            //調べたい部屋の座標
                            Vector3[] roomCoordinates = room.Value;
                            //住戸の座標
                            Vector3[] dwellingCoordinates = planParts.Value["1K"];
                            
                            //調べたい部屋の1辺について
                            for (int j = 0; j < room.Value.Length; j++) {
                                if (problemFlag) {
                                    //このパターンはもう調べる必要がないので次のパターンへ
                                    break;
                                }

                                //調べたい部屋のj番目の辺の座標
                                Vector3[] roomEdge = new Vector3[]{roomCoordinates[j], roomCoordinates[(j+1)%roomCoordinates.Length]};
                                //住戸の1辺について
                                for (int k = 0; k < planParts.Value["1K"].Length; k++) {
                                    //住戸のk番目の辺の座標
                                    Vector3[] dwellingEdge = new Vector3[]{dwellingCoordinates[k], dwellingCoordinates[(k+1)%dwellingCoordinates.Length]};

                                    //1辺が重なっているとき
                                    if (ContactJudge(roomEdge, dwellingEdge)) {
                                        //調べたい部屋の端点が住戸の辺上にないとき
                                        if (!OnLineSegment(dwellingEdge, roomEdge[0]) || !OnLineSegment(dwellingEdge, roomEdge[1])) {
                                            //問題あり
                                            problemFlag = true;

                                            //このパターンはもう調べる必要がないので次のパターンへ
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }            
                }
            }

            //問題があるとき
            if (problemFlag) {
                //このパターンは削除
                result.RemoveAt(i);
                i--;
            }
        }

        //結果を返す
        return result;
    }

    /// <summary>
    /// 辺からはみ出している，他の部屋と重なっている部屋が含まれるパターン，重複を削除
    /// </summary> 
    /// <param name="allPattern">全ての配置結果</param>
    /// <returns>辺上にない，他の部屋と重なっている部屋が含まれるパターン，重複を削除した結果</returns>
    public List<Dictionary<string, Dictionary<string, Vector3[]>>> RemoveIrregularPattern(List<Dictionary<string, Dictionary<string, Vector3[]>>> allPattern, string roomNameToCheck) {
        //結果のリスト
        var result = DuplicateList(allPattern);

        //重複を削除
        result = RemoveDuplicates(result);

        //他の部屋と重なっているパターンを削除
        result = RemoveOverlap(result, roomNameToCheck);

        //辺上にないパターンを削除
        result = RemoveNotOnEdge(result, roomNameToCheck);

        return result;
    }
}
