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
}
