using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

/* 部屋などの出力をするクラス */
public class CreateRoom : MonoBehaviour
{
    //[SerializeField] private GameObject roomNamePrefab; //部屋名のプレハブ

    //private GameObject room; //部屋と部屋名をまとめるのオブジェクト
    //private GameObject roomObject; //部屋のオブジェクト
    public LineRenderer lineRenderer; //linerendererコンポーネント

    //部屋を描画する
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

        /*
        if (nameChange(name) != "") {
            //テキストを生成
            roomName = Instantiate(roomNamePrefab, new Vector3(0, 0, 0), Quaternion.identity);

            //テキストの座標を求める
            Vector3 textPosition = new Vector3(0, 0, 0);
            for (int i = 0; i < positions.Length; i++) {
                textPosition.x += positions[i].x;
                textPosition.y += positions[i].y;
            }
            textPosition.x /= positions.Length;
            textPosition.y /= positions.Length;

            //テキストの座標を設定
            roomName.transform.GetChild(0).GetComponent<TextMeshProUGUI>().rectTransform.localPosition = textPosition;

            //テキストのフォントサイズを設定
            roomName.transform.GetChild(0).GetComponent<TextMeshProUGUI>().fontSize = 200;

            //部屋名を変更
            roomName.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = nameChange(name);

            //部屋名を親にまとめる
            roomName.transform.SetParent(room.transform);
        }
        */
    }

    public void createRoom(string name, Vector3[] positions, UnityEngine.Color color) {
        createRoom(name, positions);

        //色の変更
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }

    //部屋の英名に対応した日本語名を返す
    public string nameChange(string enName) {
        string jpName = "";

        if (enName == "entrance") {
            jpName = "玄関";
        }
        else if (enName == "mbps") {
            jpName = "MBPS";
        }
        else if (enName == "western") {
            jpName = "洋室";
        }
        else if (enName == "UB") {
            jpName = "ユニットバス";
        }
        else if (enName == "Washroom") {
            jpName = "洗面室";
        }
        else if (enName == "Toilet") {
            jpName = "トイレ";
        }
        else if (enName == "Kitchen") {
            jpName = "キッチン";
        }
        else if (enName == "balcony") {
            jpName = "バルコニー";
        }

        return jpName;
    }
    
    //部屋と部屋が接している座標を返す
    public Vector3[] contact(GameObject roomA, GameObject roomB) {
        //2つの部屋の座標などの情報
        LineRenderer linerendererOfRoomA = roomA.GetComponent<LineRenderer>();
        LineRenderer linerendererOfRoomB = roomB.GetComponent<LineRenderer>();
        
        //すべてがゼロの座標（接しているかどうかの判定に使用）
        Vector3[] zero = new Vector3[] {Vector3.zero, Vector3.zero};

        //接している（すべての座標がゼロでない）とき，その座標を返す
        if ((contact_xy(linerendererOfRoomA, linerendererOfRoomB)[0] != zero[0]) && (contact_xy(linerendererOfRoomA, linerendererOfRoomB)[1] != zero[1])) {
            return contact_xy(linerendererOfRoomA, linerendererOfRoomB);
        }
        //接していない場合，すべてがゼロの座標を返す
        else {
            return new Vector3[] {Vector3.zero, Vector3.zero};
        }
    }

    //部屋の1辺と部屋が接している座標を返す
    public Vector3[] contact(Vector3[] side, GameObject room) {
        //部屋の座標などの情報
        LineRenderer linerendererOfRoom = room.GetComponent<LineRenderer>();
        
        //すべてがゼロの座標（接しているかどうかの判定に使用）
        Vector3[] zero = new Vector3[] {Vector3.zero, Vector3.zero};

        //接している（すべての座標がゼロでない）とき，その座標を返す
        if ((contact_xy(side, linerendererOfRoom)[0] != zero[0]) && (contact_xy(side, linerendererOfRoom)[1] != zero[1])) {
            return contact_xy(side, linerendererOfRoom);
        }
        //接していない場合，すべてがゼロの座標を返す
        else {
            return new Vector3[] {Vector3.zero, Vector3.zero};
        }
    }

    //部屋の1辺と部屋が接している座標を返す
    public Vector3[] contact(Vector3[] side, Vector3[] room) {
        //すべてがゼロの座標（接しているかどうかの判定に使用）
        Vector3[] zero = new Vector3[] {Vector3.zero, Vector3.zero};

        //接している（すべての座標がゼロでない）とき，その座標を返す
        if ((contact_xy(side, room)[0] != zero[0]) && (contact_xy(side, room)[1] != zero[1])) {
            return contact_xy(side, room);
        }
        //接していない場合，すべてがゼロの座標を返す
        else {
            return new Vector3[] {Vector3.zero, Vector3.zero};
        }
    }

    //部屋の1辺と部屋の1辺が接している座標を返す
    public Vector3[] ContactSide(Vector3[] sideA, Vector3[] sideB) {
        //すべてがゼロの座標（接しているかどうかの判定に使用）
        Vector3[] zero = new Vector3[] {Vector3.zero, Vector3.zero};

        //接している（すべての座標がゼロでない）とき，その座標を返す
        if ((contact_side_xy(sideA, sideB)[0] != zero[0]) && (contact_side_xy(sideA, sideB)[1] != zero[1])) {
            return contact_side_xy(sideA, sideB);
        }
        //接していない場合，すべてがゼロの座標を返す
        else {
            return new Vector3[] {Vector3.zero, Vector3.zero};
        }
    }
    
    //x座標もy座標も異なるが部屋が接しているときの座標を返す
    public Vector3[] contact_xy(LineRenderer linerendererOfRoomA, LineRenderer linerendererOfRoomB) {
        //4点の座標を格納する変数
        Vector3 A1, A2, B1, B2;

        //部屋Aのすべての辺を調べる
        for (int i = 0; i < linerendererOfRoomA.positionCount; i++) {
            if (i == linerendererOfRoomA.positionCount - 1) {
                A1 = linerendererOfRoomA.GetPosition(i);
                A2 = linerendererOfRoomA.GetPosition(0);
            } else {
                A1 = linerendererOfRoomA.GetPosition(i);
                A2 = linerendererOfRoomA.GetPosition(i+1);
            }
            //部屋Bのすべての辺を調べる
            for (int j = 0; j < linerendererOfRoomB.positionCount; j++) {
                if (j == linerendererOfRoomB.positionCount - 1) {
                    B1 = linerendererOfRoomB.GetPosition(j);
                    B2 = linerendererOfRoomB.GetPosition(0);
                } else {
                    B1 = linerendererOfRoomB.GetPosition(j);
                    B2 = linerendererOfRoomB.GetPosition(j+1);
                }

                //座標をx, yごとに分けるためのリスト
                List<float> coordinates_x = new List<float>();
                List<float> coordinates_y = new List<float>();

                //4点が一直線上に並んでいるとき（端から真ん中，真ん中から反対の端の距離の和が端から端の距離と同じとき × 2）
                if ( (Vector3.Distance(A1, A2) + Vector3.Distance(A2, B1) == Vector3.Distance(A1, B1)) || (Vector3.Distance(B1, A1) + Vector3.Distance(A1, A2) == Vector3.Distance(B1, A2)) || (Vector3.Distance(A2, B1) + Vector3.Distance(B1, A1) == Vector3.Distance(A2, A1)) ) {
                    if ( (Vector3.Distance(A1, A2) + Vector3.Distance(A2, B2) == Vector3.Distance(A1, B2)) || (Vector3.Distance(B2, A1) + Vector3.Distance(A1, A2) == Vector3.Distance(B2, A2)) || (Vector3.Distance(A2, B2) + Vector3.Distance(B2, A1) == Vector3.Distance(A2, A1)) ) {
                        //2つの部屋を通り抜けられるように接しているとき（片方の辺の最小の点がもう片方の辺の最大の点より大きいとき）
                        if (! (((Mathf.Max(A1.x, A2.x) <= Mathf.Min(B1.x, B2.x)) || (Mathf.Min(A1.x, A2.x) >= Mathf.Max(B1.x, B2.x))) && ((Mathf.Max(A1.y, A2.y) <= Mathf.Min(B1.y, B2.y)) || (Mathf.Min(A1.y, A2.y) >= Mathf.Max(B1.y, B2.y)))) ) {
                            /*** 
                            xとyを別々にソートしてる
                            今はいいけど絶対によくない
                            後で変更する　多分
                            ***/

                            //x座標を昇順にソート
                            coordinates_x.Add(A1.x);
                            coordinates_x.Add(A2.x);
                            coordinates_x.Add(B1.x);
                            coordinates_x.Add(B2.x);

                            coordinates_x.Sort();

                            //y座標を昇順にソート
                            coordinates_y.Add(A1.y);
                            coordinates_y.Add(A2.y);
                            coordinates_y.Add(B1.y);
                            coordinates_y.Add(B2.y);

                            coordinates_y.Sort();

                            //接している辺の共通部分（昇順の真ん中2つ）を返す
                            return new Vector3[] {new Vector3(coordinates_x[1], coordinates_y[1], 0), new Vector3(coordinates_x[2], coordinates_y[2], 0)};
                        }
                    }
                }
            }
        }

        //どれにも当てはまらなかったとき，すべてが0の点を返す
        return new Vector3[] {Vector3.zero, Vector3.zero};            
    }

    //x座標もy座標も異なるが部屋が接しているときの座標を返す
    public Vector3[] contact_xy(Vector3[] side, LineRenderer linerendererOfRoom) {
        //4点の座標を格納する変数
        Vector3 A1, A2, B1, B2;

        //部屋Aのすべての辺を調べる   
        A1 = side[0];
        A2 = side[1];
        
        //部屋のすべての辺を調べる
        for (int j = 0; j < linerendererOfRoom.positionCount; j++) {
            if (j == linerendererOfRoom.positionCount - 1) {
                B1 = linerendererOfRoom.GetPosition(j);
                B2 = linerendererOfRoom.GetPosition(0);
            } else {
                B1 = linerendererOfRoom.GetPosition(j);
                B2 = linerendererOfRoom.GetPosition(j+1);
            }

            //座標をx, yごとに分けるためのリスト
            List<float> coordinates_x = new List<float>();
            List<float> coordinates_y = new List<float>();

            //4点が一直線上に並んでいるとき（端から真ん中，真ん中から反対の端の距離の和が端から端の距離と同じとき × 2）
            if ( (Vector3.Distance(A1, A2) + Vector3.Distance(A2, B1) == Vector3.Distance(A1, B1)) || (Vector3.Distance(B1, A1) + Vector3.Distance(A1, A2) == Vector3.Distance(B1, A2)) || (Vector3.Distance(A2, B1) + Vector3.Distance(B1, A1) == Vector3.Distance(A2, A1)) ) {
                if ( (Vector3.Distance(A1, A2) + Vector3.Distance(A2, B2) == Vector3.Distance(A1, B2)) || (Vector3.Distance(B2, A1) + Vector3.Distance(A1, A2) == Vector3.Distance(B2, A2)) || (Vector3.Distance(A2, B2) + Vector3.Distance(B2, A1) == Vector3.Distance(A2, A1)) ) {
                    //2つの部屋を通り抜けられるように接しているとき（片方の辺の最小の点がもう片方の辺の最大の点より大きいとき）
                    if (! (((Mathf.Max(A1.x, A2.x) <= Mathf.Min(B1.x, B2.x)) || (Mathf.Min(A1.x, A2.x) >= Mathf.Max(B1.x, B2.x))) && ((Mathf.Max(A1.y, A2.y) <= Mathf.Min(B1.y, B2.y)) || (Mathf.Min(A1.y, A2.y) >= Mathf.Max(B1.y, B2.y)))) ) {
                        /*** 
                        xとyを別々にソートしてる
                        x軸平行，y軸平行のときのみ成立
                        後で変更する　多分
                        ***/

                        //x座標を昇順にソート
                        coordinates_x.Add(A1.x);
                        coordinates_x.Add(A2.x);
                        coordinates_x.Add(B1.x);
                        coordinates_x.Add(B2.x);

                        coordinates_x.Sort();

                        //y座標を昇順にソート
                        coordinates_y.Add(A1.y);
                        coordinates_y.Add(A2.y);
                        coordinates_y.Add(B1.y);
                        coordinates_y.Add(B2.y);

                        coordinates_y.Sort();

                        //接している辺の共通部分（昇順の真ん中2つ）を返す
                        return new Vector3[] {new Vector3(coordinates_x[1], coordinates_y[1], 0), new Vector3(coordinates_x[2], coordinates_y[2], 0)};
                    }
                }
            }
        }

        //どれにも当てはまらなかったとき，すべてが0の点を返す
        return new Vector3[] {Vector3.zero, Vector3.zero};            
    }

    //x座標もy座標も異なるが部屋が接しているときの座標を返す
    public Vector3[] contact_xy(Vector3[] side, Vector3[] room) {
        //4点の座標を格納する変数
        Vector3 A1, A2, B1, B2;

        //部屋Aのすべての辺を調べる   
        A1 = side[0];
        A2 = side[1];
        
        //部屋のすべての辺を調べる
        for (int j = 0; j < room.Length; j++) {
            if (j == room.Length - 1) {
                B1 = room[j];
                B2 = room[0];
            } else {
                B1 = room[j];
                B2 = room[j+1];
            }

            //座標をx, yごとに分けるためのリスト
            List<float> coordinates_x = new List<float>();
            List<float> coordinates_y = new List<float>();

            //4点が一直線上に並んでいるとき（端から真ん中，真ん中から反対の端の距離の和が端から端の距離と同じとき × 2）
            if ( (Vector3.Distance(A1, A2) + Vector3.Distance(A2, B1) == Vector3.Distance(A1, B1)) || (Vector3.Distance(B1, A1) + Vector3.Distance(A1, A2) == Vector3.Distance(B1, A2)) || (Vector3.Distance(A2, B1) + Vector3.Distance(B1, A1) == Vector3.Distance(A2, A1)) ) {
                if ( (Vector3.Distance(A1, A2) + Vector3.Distance(A2, B2) == Vector3.Distance(A1, B2)) || (Vector3.Distance(B2, A1) + Vector3.Distance(A1, A2) == Vector3.Distance(B2, A2)) || (Vector3.Distance(A2, B2) + Vector3.Distance(B2, A1) == Vector3.Distance(A2, A1)) ) {
                    //2つの部屋を通り抜けられるように接しているとき（片方の辺の最小の点がもう片方の辺の最大の点より大きいとき）
                    if (! (((Mathf.Max(A1.x, A2.x) <= Mathf.Min(B1.x, B2.x)) || (Mathf.Min(A1.x, A2.x) >= Mathf.Max(B1.x, B2.x))) && ((Mathf.Max(A1.y, A2.y) <= Mathf.Min(B1.y, B2.y)) || (Mathf.Min(A1.y, A2.y) >= Mathf.Max(B1.y, B2.y)))) ) {
                        /*** 
                        xとyを別々にソートしてる
                        x軸平行，y軸平行のときのみ成立
                        後で変更する　多分
                        ***/

                        //x座標を昇順にソート
                        coordinates_x.Add(A1.x);
                        coordinates_x.Add(A2.x);
                        coordinates_x.Add(B1.x);
                        coordinates_x.Add(B2.x);

                        coordinates_x.Sort();

                        //y座標を昇順にソート
                        coordinates_y.Add(A1.y);
                        coordinates_y.Add(A2.y);
                        coordinates_y.Add(B1.y);
                        coordinates_y.Add(B2.y);

                        coordinates_y.Sort();

                        //接している辺の共通部分（昇順の真ん中2つ）を返す
                        return new Vector3[] {new Vector3(coordinates_x[1], coordinates_y[1], 0), new Vector3(coordinates_x[2], coordinates_y[2], 0)};
                    }
                }
            }
        }

        //どれにも当てはまらなかったとき，すべてが0の点を返す
        return new Vector3[] {Vector3.zero, Vector3.zero};            
    }

    //x座標もy座標も異なるが部屋が接しているときの座標を返す
    public Vector3[] contact_side_xy(Vector3[] sideA, Vector3[] sideB) {
        //4点の座標を格納する変数
        Vector3 A1, A2, B1, B2;

        //辺Aの点   
        A1 = sideA[0];
        A2 = sideA[1];

        //辺Bの点   
        B1 = sideB[0];
        B2 = sideB[1];

        //座標をx, yごとに分けるためのリスト
        List<float> coordinates_x = new List<float>();
        List<float> coordinates_y = new List<float>();

        //4点が一直線上に並んでいるとき（端から真ん中，真ん中から反対の端の距離の和が端から端の距離と同じとき × 2）
        if ( (Vector3.Distance(A1, A2) + Vector3.Distance(A2, B1) == Vector3.Distance(A1, B1)) || (Vector3.Distance(B1, A1) + Vector3.Distance(A1, A2) == Vector3.Distance(B1, A2)) || (Vector3.Distance(A2, B1) + Vector3.Distance(B1, A1) == Vector3.Distance(A2, A1)) ) {
            if ( (Vector3.Distance(A1, A2) + Vector3.Distance(A2, B2) == Vector3.Distance(A1, B2)) || (Vector3.Distance(B2, A1) + Vector3.Distance(A1, A2) == Vector3.Distance(B2, A2)) || (Vector3.Distance(A2, B2) + Vector3.Distance(B2, A1) == Vector3.Distance(A2, A1)) ) {
                //2つの部屋を通り抜けられるように接しているとき（片方の辺の最小の点がもう片方の辺の最大の点より大きいとき）
                if (! (((Mathf.Max(A1.x, A2.x) <= Mathf.Min(B1.x, B2.x)) || (Mathf.Min(A1.x, A2.x) >= Mathf.Max(B1.x, B2.x))) && ((Mathf.Max(A1.y, A2.y) <= Mathf.Min(B1.y, B2.y)) || (Mathf.Min(A1.y, A2.y) >= Mathf.Max(B1.y, B2.y)))) ) {
                    /*** 
                    xとyを別々にソートしてる
                    x軸平行，y軸平行のときのみ成立
                    後で変更する　多分
                    ***/

                    //x座標を昇順にソート
                    coordinates_x.Add(A1.x);
                    coordinates_x.Add(A2.x);
                    coordinates_x.Add(B1.x);
                    coordinates_x.Add(B2.x);

                    coordinates_x.Sort();

                    //y座標を昇順にソート
                    coordinates_y.Add(A1.y);
                    coordinates_y.Add(A2.y);
                    coordinates_y.Add(B1.y);
                    coordinates_y.Add(B2.y);

                    coordinates_y.Sort();

                    //接している辺の共通部分（昇順の真ん中2つ）を返す
                    return new Vector3[] {new Vector3(coordinates_x[1], coordinates_y[1], 0), new Vector3(coordinates_x[2], coordinates_y[2], 0)};
                }
            }
        }

        //どれにも当てはまらなかったとき，すべてが0の点を返す
        //ここなにもデータ入れずに返せばいいのでは？
        //いつか修正する
        return new Vector3[] {Vector3.zero, Vector3.zero};            
    }

    /***

    全座標が0かどうかを判定

    ***/
    public bool ZeroJudge(Vector3[] room) {
        bool flag = false;
        
        if (room.All(i => i == Vector3.zero)) {
            flag =true;
        }

        return flag;
    }

    //部屋の面積を求めるメソッド
    public float areaCalculation(GameObject room) {
        LineRenderer linerendererOfRoom = room.GetComponent<LineRenderer>();

        float area = 0;
        for (int i = 0; i < linerendererOfRoom.positionCount; i++) {
            float x, y_before_x, y_after_x;

            x = linerendererOfRoom.GetPosition(i).x + room.transform.position.x;

            if (i == 0) {
                y_before_x = linerendererOfRoom.GetPosition(linerendererOfRoom.positionCount-1).y + room.transform.position.y;
                y_after_x = linerendererOfRoom.GetPosition(i+1).y + room.transform.position.y;
            }
            else if (i == linerendererOfRoom.positionCount - 1) {
                y_before_x = linerendererOfRoom.GetPosition(i-1).y + room.transform.position.y;
                y_after_x = linerendererOfRoom.GetPosition(0).y + room.transform.position.y;
            }
            else {
                y_before_x = linerendererOfRoom.GetPosition(i-1).y + room.transform.position.y;
                y_after_x = linerendererOfRoom.GetPosition(i+1).y + room.transform.position.y;
            }

            area += x * (y_after_x - y_before_x);
        }

        return Mathf.Abs((area / 2) / 1000000);
    }

    //部屋の面積を求めるメソッド
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
