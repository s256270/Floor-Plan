using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorPlanner : CreateRoom
{
    [SerializeField] public Parts pa;
    [SerializeField] CreateMbps cm;
    [SerializeField] CreateEntrance ce;
    [SerializeField] CreateWetareas cwa;
    [SerializeField] CreateWestern cws;
    
    public List<Dictionary<string, Dictionary<string, Vector3[]>>> allPattern = new List<Dictionary<string, Dictionary<string, Vector3[]>>>();
    
    /// <summary>
    /// プランを入力すると部屋の配置を行い，間取図を作成する
    /// </summary> 
    /// <param name="plan">プラン図</param>
    /// <returns>間取図（それぞれの部屋名と座標がセットのリスト）</returns>
    public List<Dictionary<string, Dictionary<string, Vector3[]>>> Placement(Dictionary<string, Dictionary<string, Vector3[]>> plan) {
        //配置結果の配置結果にプラン図を追加
        allPattern.Add(plan);

        //全パターンの配置結果にMBPSを配置
        allPattern = cm.PlaceMbps(allPattern);

        //全パターンの配置結果に玄関を配置
        allPattern = ce.PlaceEntrance(allPattern);

        //全パターンの配置結果に水回りの部屋を配置
        allPattern = cwa.PlaceWetareas(allPattern);

        //全パターンの配置結果に洋室を配置
        allPattern = cws.PlaceWestern(allPattern);

        return allPattern;
    }

    /// <summary>
    /// 多角形と線分の平行な線の組み合わせの切片の差を求める
    /// </summary> 
    /// <param name="polygon">多角形の座標</param>
    /// <param name="line">線分の座標</param>
    /// <returns>多角形と線分の平行な線の組み合わせの切片の差の配列</returns>
    public float[] ContactGap(Vector3[] polygon, Vector3[] line) {
        //返す配列
        List<float> result = new List<float>();

        for (int i = 0; i < polygon.Length; i++) {
            if ((positionalRelation(new Vector3[]{polygon[i], polygon[(i+1)%polygon.Length]}, line)[2] == 1.00f) || (positionalRelation(new Vector3[]{polygon[i], polygon[(i+1)%polygon.Length]}, line)[2] == 2.00f)) {
                result.Add(positionalRelation(new Vector3[]{polygon[i], polygon[(i+1)%polygon.Length]}, line)[1] - positionalRelation(new Vector3[]{polygon[i], polygon[(i+1)%polygon.Length]}, line)[0]);
            }
        }

        return result.ToArray();
    }


    /// <summary>
    /// 線分と線分が平行かどうかを確認し，平行な線分のそれぞれの切片を求める
    /// </summary> 
    /// <param name="lineA">線分Aの座標</param>
    /// <param name="lineB">線分Bの座標</param>
    /// <returns>線分のそれぞれの切片と，平行でないとき0, y軸平行のとき1, それ以外で平行のとき2の配列</returns>
    public float[] positionalRelation(Vector3[] lineA, Vector3[] lineB) {
        float m1, m2, n1, n2;

        //y軸平行の時で直線と部屋が平行
        if (lineA[0].x == lineA[1].x) {
            n1 = lineA[0].x;
            if (lineB[0].x == lineB[1].x) {
                n2 = lineB[0].x;

                return new float[] {n1, n2, 1.00f};
            }
        }
        //それ以外で直線と部屋が平行
        else {
            m1 = (lineA[1].y - lineA[0].y) / (lineA[1].x - lineA[0].x);
            n1 = (lineA[1].x * lineA[0].y - lineA[0].x * lineA[1].y) / (lineA[1].x - lineA[0].x);
            if (lineB[0].x != lineB[1].x) {
                m2 = (lineB[1].y - lineB[0].y) / (lineB[1].x - lineB[0].x);
                n2 = (lineB[1].x * lineB[0].y - lineB[0].x * lineB[1].y) / (lineB[1].x - lineB[0].x);
                if (m1 == m2) {

                    return new float[] {n1, n2, 2.00f};
                }
            }     
        }

        //直線と部屋が平行でない
        return new float[] {0.00f, 0.00f, 0.00f};
    }

    /// <summary>
    /// 全ての座標を原点周りに90度回転させる
    /// </summary> 
    /// <param name="shapes">回転させたい座標配列</param>
    /// <returns>回転させた後の座標配列</returns>
    public Vector3[] Rotation(Vector3[] shapes) {
        //回転させた後の座標を格納する配列
        Vector3[] rotatedCoordinates = new Vector3[shapes.Length];

        //回転させる
        for (int i = 0; i < shapes.Length; i++) {
            rotatedCoordinates[i].x = - shapes[(i+1)%shapes.Length].y;
            rotatedCoordinates[i].y = shapes[(i+1)%shapes.Length].x;
        }

        return rotatedCoordinates;
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
    /// 辺Aから辺Aと辺Bの重なっている部分を除いた辺を返す
    /// </summary> 
    /// <param name="sideA">辺Aの座標</param>
    /// <param name="sideB">辺Bの座標</param>
    /// <returns>辺Aから辺Aと辺Bの重なっている部分を除いた辺の座標</returns>
    public List<Vector3[]> SideSubstraction(Vector3[] sideA, Vector3[] sideB) {
        List<Vector3[]> result = new List<Vector3[]>();

        //辺Aと辺Bの重なっている部分
        Vector3[] overlap = ContactSide(sideA, sideB);

        //重なっている部分がある場合
        if (!ZeroJudge(overlap)) {
            //辺Aと重なっている部分の座標の順が逆の場合
            if (Vector3.Dot(sideA[1] - sideA[0], overlap[1] - overlap[0]) < 0) {
                //重なっている部分の座標を入れ替え
                Array.Reverse(overlap);
            }

            /* 重なっている部分を除いた辺の座標を返す */
            if (sideA[0] == overlap[0] && sideA[1] == overlap[1]) {
                result.Add(new Vector3[] {Vector3.zero, Vector3.zero});
            }
            else if (sideA[0] == overlap[0]) {
                result.Add(new Vector3[] {overlap[1], sideA[1]});
            }
            else if (sideA[1] == overlap[1]) {
                result.Add(new Vector3[] {sideA[0], overlap[0]});
            }
            else {
                result.Add(new Vector3[] {sideA[0], overlap[0]});
                result.Add(new Vector3[] {overlap[1], sideA[1]});
            }
        }
        else {
            result.Add(sideA);
        }

        return result;
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
    /// ある点がある線分上にあるかどうかの判定
    /// </summary> 
    /// <param name="side">線分の端点の座標配列</param>
    /// <param name="point">判定する点の座標</param>
    /// <returns>点が線分上にある場合True，ない場合Flase</returns>
    public bool OnLineSegment(Vector3[] side, Vector3 point) {
        if ((side[0].x <= point.x && point.x <= side[1].x) || (side[1].x <= point.x && point.x <= side[0].x)) {
            if ((side[0].y <= point.y && point.y <= side[1].y) || (side[1].y <= point.y && point.y <= side[0].y)) {
                if ((point.y * (side[0].x - side[1].x)) + (side[0].y * (side[1].x - point.x)) + (side[1].y * (point.x - side[0].x)) == 0) {
                    return true;
                }
            }
        }

        return false;
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

    //配置結果確認用
    public void Check(Dictionary<string, Dictionary<string, Vector3[]>> pattern) {
        foreach (KeyValuePair<string, Dictionary<string, Vector3[]>> space in pattern) {
            Debug.Log(space.Key + "の要素");
            foreach (KeyValuePair<string, Vector3[]> spaceElements in space.Value) {
                Debug.Log("スペース名: " + spaceElements.Key);
                /*
                for (int i = 0; i < spaceElements.Value.Length; i++) {
                    Debug.Log("座標" + i + ": " + spaceElements.Value[i]);
                }
                */
            }
        }
    }
}
