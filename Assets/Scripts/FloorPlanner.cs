using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorPlanner : CreateRoom
{
    [SerializeField] public PlanReader pr;
    [SerializeField] CreateMbps cm;
    [SerializeField] public Parts pa;
    
    public List<Dictionary<string, Dictionary<string, Vector3[]>>> allPattern;
    public Dictionary<string, Dictionary<string, Vector3[]>> plan;
    
    /// <summary>
    /// プランを入力すると部屋の配置を行い，間取図を作成する
    /// </summary> 
    /// <param name="plan">プラン図</param>
    /// <returns>間取図（それぞれの部屋名と座標がセットのリスト）</returns>
    public List<Dictionary<string, Dictionary<string, Vector3[]>>> Placement() {
        plan = new Dictionary<string, Dictionary<string, Vector3[]>>(pr.plan);

        cm.PlaceMbps();

        //PlaceEntrance();

        return allPattern;
    }

    /***

    MBPSの配置

    ***/
    public void placeMbps() {
        //MbPSが生成されているかを判別する配列
        int[] mbpsExist = new int[outerRooms.Length];

        /*2部屋にまたがるMBPSの配置開始 */
        placeMbpsInTwoRoomsRoop(mbpsExist);

        /*1部屋のみのMBPSの配置開始 */
        placeMbpsInOneRoom(mbpsExist);
        
    }

    public void placeMbpsInOneRoom(int[] mbpsExist) {
        //階段室の1辺の座標
        Vector3[] stairs_coordinate = coordinatesOfRoom(GameObject.Find("Stairs"));

        for (int i = 0; i < mbpsExist.Length; i++) {
            if (mbpsExist[i] != 1) {
                //MBPSをくっつける階段室の辺を求める
                Vector3 s1 = new Vector3(0, 0, 0);
                Vector3 s2 = new Vector3(0, 0, 0);

                for (int j = 0; j < stairs_coordinate.Length; j++) {
                    Vector3[] contact_coodinates = contact(new Vector3[]{stairs_coordinate[j], stairs_coordinate[(j+1)%stairs_coordinate.Length]}, outerRooms[i]);
                    Vector3[] zero = new Vector3[] {Vector3.zero, Vector3.zero};
                    
                    if ((contact_coodinates[0] != zero[0]) && (contact_coodinates[1] != zero[1])) {
                        s1 = contact_coodinates[0];
                        s2 = contact_coodinates[1];
                    }
                }
       
                for (int j = 0; j < pa.mbps_coordinate2.Length; j++) {
                    if (positionalRelation(new Vector3[]{s1, s2}, new Vector3[]{pa.mbps_coordinate2[j], pa.mbps_coordinate2[(j+1)%pa.mbps_coordinate2.Length]})[2] == 1.00f) {
                        float gap_x = positionalRelation(new Vector3[]{s1, s2}, new Vector3[]{pa.mbps_coordinate2[j], pa.mbps_coordinate2[(j+1)%pa.mbps_coordinate2.Length]})[0] - positionalRelation(new Vector3[]{s1, s2}, new Vector3[]{pa.mbps_coordinate2[j], pa.mbps_coordinate2[(j+1)%pa.mbps_coordinate2.Length]})[1];
                        float gap_y = contact(new Vector3[]{s1, s2}, outerRooms[i])[0].y + pa.mbps_coordinate2[0].y;

                        int need_count = 0;
                        int act_count = 0;
                        for (int k = 0; k < pa.mbps_coordinate2.Length; k++) {
                            if ((pa.mbps_coordinate2[k] != pa.mbps_coordinate2[j]) && (pa.mbps_coordinate2[k] != pa.mbps_coordinate2[(j+1)%pa.mbps_coordinate2.Length])) {
                                need_count++;
                                if (CheckPoint(outerRooms[i], new Vector3(pa.mbps_coordinate2[k].x + gap_x, pa.mbps_coordinate2[k].y + gap_y, 0))) {
                                    act_count++;
                                } 
                            }
                        }

                        if (need_count == act_count) {
                            createRoom("MbpsOf" + outerRooms[i].name, pa.mbps_coordinate2);
                            //createRoom("MbpsOf" + outerRooms[i].name, pa.mbps_coordinate2, Color.red);

                            Vector3 mbps_pos = GameObject.Find("MbpsOf" + outerRooms[i].name).transform.position;

                            mbps_pos.x += gap_x;
                            mbps_pos.y += gap_y;
                            
                            GameObject.Find("MbpsOf" + outerRooms[i].name).transform.position = mbps_pos;

                            mbpsExist[i] = 1;
                        }                
                    }
                    else if (positionalRelation(new Vector3[]{s1, s2}, new Vector3[]{pa.mbps_coordinate2[j], pa.mbps_coordinate2[(j+1)%pa.mbps_coordinate2.Length]})[2] == 2.00f) {
                        
                    }
                }
            }
        }
    }

    /***

    玄関

    ***/
    public void placeEntrance() {
        //階段室の1辺の座標
        Vector3[] stairs_coordinate = coordinatesOfRoom(GameObject.Find("Stairs"));

        for (int i = 0; i < outerRooms.Length; i++) {
            //玄関をくっつける階段室の辺を求める
            Vector3 s1 = new Vector3(0, 0, 0);
            Vector3 s2 = new Vector3(0, 0, 0);

            for (int j = 0; j < stairs_coordinate.Length; j++) {
                Vector3[] contact_coodinates = contact(new Vector3[]{stairs_coordinate[j], stairs_coordinate[(j+1)%stairs_coordinate.Length]}, outerRooms[i]);
                Vector3[] zero = new Vector3[] {Vector3.zero, Vector3.zero};
                
                if ((contact_coodinates[0] != zero[0]) && (contact_coodinates[1] != zero[1])) {
                    s1 = contact_coodinates[0];
                    s2 = contact_coodinates[1];
                }
            }

            //MBPSの座標
            Vector3[] mbps_coordinate = coordinatesOfRoom(GameObject.Find("MbpsOf" + outerRooms[i].name));

            for (int j = 0; j < stairs_coordinate.Length; j++) {
                Vector3[] contact_coodinates = contact(new Vector3[]{stairs_coordinate[j], stairs_coordinate[(j+1)%stairs_coordinate.Length]}, mbps_coordinate);
                Vector3[] zero = new Vector3[] {Vector3.zero, Vector3.zero};
                
                if ((contact_coodinates[0] != zero[0]) && (contact_coodinates[1] != zero[1])) {
                    if ((s1.x != contact_coodinates[0].x) || (s1.y != contact_coodinates[0].y)) {
                        s2 = contact_coodinates[0];
                    } else if ((s2.x != contact_coodinates[1].x) || (s2.y != contact_coodinates[1].y)) {
                        s1 = contact_coodinates[1];
                    }
                }
            }

            for (int j = 0; j < pa.entrance_coordinate_x.Length; j++) {
                //y軸平行
                if (positionalRelation(new Vector3[]{s1, s2}, new Vector3[]{pa.entrance_coordinate_y[j], pa.entrance_coordinate_y[(j+1)%pa.entrance_coordinate_y.Length]})[2] == 1.00f) {
                    float gap_x = positionalRelation(new Vector3[]{s1, s2}, new Vector3[]{pa.entrance_coordinate_y[j], pa.entrance_coordinate_y[(j+1)%pa.entrance_coordinate_y.Length]})[0] - positionalRelation(new Vector3[]{s1, s2}, new Vector3[]{pa.entrance_coordinate_y[j], pa.entrance_coordinate_y[(j+1)%pa.entrance_coordinate_y.Length]})[1];
                    float gap_y = contact(new Vector3[]{s1, s2}, outerRooms[i])[0].y + Mathf.Abs(pa.entrance_coordinate_y[0].y);

                    int need_count = 0;
                    int act_count = 0;
                    for (int k = 0; k < pa.entrance_coordinate_y.Length; k++) {
                        if ((pa.entrance_coordinate_y[k] != pa.entrance_coordinate_y[j]) && (pa.entrance_coordinate_y[k] != pa.entrance_coordinate_y[(j+1)%pa.entrance_coordinate_y.Length])) {
                            need_count++;
                            if (CheckPoint(outerRooms[i], new Vector3(pa.entrance_coordinate_y[k].x + gap_x, pa.entrance_coordinate_y[k].y + gap_y, 0))) {
                                act_count++;
                            } 
                        }
                    }

                    if (need_count == act_count) {
                        createRoom("EntranceOf" + outerRooms[i].name, pa.entrance_coordinate_y);
                        //createRoom("EntranceOf" + outerRooms[i].name, pa.entrance_coordinate_y, Color.blue);

                        Vector3 entrance_pos = GameObject.Find("EntranceOf" + outerRooms[i].name).transform.position;

                        entrance_pos.x += gap_x;
                        entrance_pos.y += gap_y;
                        
                        GameObject.Find("EntranceOf" + outerRooms[i].name).transform.position = entrance_pos;
                    }
                }
                //x軸平行
                else if (positionalRelation(new Vector3[]{s1, s2}, new Vector3[]{pa.entrance_coordinate_x[j], pa.entrance_coordinate_x[(j+1)%pa.entrance_coordinate_x.Length]})[2] == 2.00f) {
                    float gap_x = 0;
                    float gap_x_A = s1.x + Mathf.Abs(pa.entrance_coordinate_x[0].x) + 1;
                    float gap_x_B = s1.x - Mathf.Abs(pa.entrance_coordinate_x[0].x) - 1;
                    float gap_y = positionalRelation(new Vector3[]{s1, s2}, new Vector3[]{pa.entrance_coordinate_x[j], pa.entrance_coordinate_x[(j+1)%pa.entrance_coordinate_x.Length]})[0] - positionalRelation(new Vector3[]{s1, s2}, new Vector3[]{pa.entrance_coordinate_x[j], pa.entrance_coordinate_x[(j+1)%pa.entrance_coordinate_x.Length]})[1];

                    int need_count = 0;
                    int act_count_A = 0;
                    int act_count_B = 0;
                    for (int k = 0; k < pa.entrance_coordinate_x.Length; k++) {
                        if ((pa.entrance_coordinate_x[k] != pa.entrance_coordinate_x[j]) && (pa.entrance_coordinate_x[k] != pa.entrance_coordinate_x[(j+1)%pa.entrance_coordinate_x.Length])) {
                            need_count++;
                            if (CheckPoint(outerRooms[i], new Vector3(pa.entrance_coordinate_x[k].x + gap_x_A, pa.entrance_coordinate_x[k].y + gap_y, 0))) {
                                act_count_A++;
                            } 
                            if (CheckPoint(outerRooms[i], new Vector3(pa.entrance_coordinate_x[k].x + gap_x_B, pa.entrance_coordinate_x[k].y + gap_y, 0))) {
                                act_count_B++;
                            }
                        }
                    }

                    if (need_count == act_count_A) {
                        gap_x = gap_x_A - 1;

                        createRoom("EntranceOf" + outerRooms[i].name, pa.entrance_coordinate_x);
                        //createRoom("EntranceOf" + outerRooms[i].name, pa.entrance_coordinate_x, Color.blue);

                        Vector3 entrance_pos = GameObject.Find("EntranceOf" + outerRooms[i].name).transform.position;

                        entrance_pos.x += gap_x;
                        entrance_pos.y += gap_y;
                        
                        GameObject.Find("EntranceOf" + outerRooms[i].name).transform.position = entrance_pos;
                    }
                    if (need_count == act_count_B) {
                        gap_x = gap_x_B + 1;

                        createRoom("EntranceOf" + outerRooms[i].name, pa.entrance_coordinate_x);
                        //createRoom("EntranceOf" + outerRooms[i].name, pa.entrance_coordinate_x, Color.blue);

                        Vector3 entrance_pos = GameObject.Find("EntranceOf" + outerRooms[i].name).transform.position;

                        entrance_pos.x += gap_x;
                        entrance_pos.y += gap_y;
                        
                        GameObject.Find("EntranceOf" + outerRooms[i].name).transform.position = entrance_pos;
                    }
                }
            }
        }
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
}
