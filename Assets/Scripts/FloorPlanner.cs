using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorPlanner : CreateRoom
{
    [SerializeField] PlanMaker pm;
    [SerializeField] Parts pa;

    GameObject[] outerRooms;
    List<List<int>> contact_stairs = new List<List<int>>();//階段室の1辺の座標（小さい方のリスト番号）とそれに接してる部屋のリスト番号の組み合わせ
    List<int> contact_room_number = new List<int>();//階段室と接している部屋のうち，同じ辺に2つの部屋が接している部屋のリスト番号
    List<List<int>> contact_stairs_all_pattern = new List<List<int>>();//階段室の1つの辺に2つの部屋が接しているものから，被りがないように選んだ全パターン

    void Start()
    {
        //プランを表示
        pm.planMake2();

        outerRooms = new GameObject[]{pm.plan[0], pm.plan[1], pm.plan[2], pm.plan[3]};

        
        placement();

        /*
        //水回り(仮)
        //outerRooms[0]
        CreateAndCorrect("Toilet", Rotation(pa.toilet_coordinates), outerRooms[0], -3550f, 300f);
        CreateAndCorrect("Kitchen", pa.kitchen_coordinates, outerRooms[0], -5450f, 200f);
        CreateAndCorrect("Ub", Rotation(pa.ub_coordinates), outerRooms[0], -2550f, 2700f);
        CreateAndCorrect("Washroom", pa.washroom_coordinates, outerRooms[0], -4050f, 2850f);
        CreateAndCorrect("Western", new Vector3[]{new Vector3(-9650, 3650, 0), new Vector3(-4850, 3650, 0), new Vector3(-4850, 2050, 0), new Vector3(-6650, 2050, 0), new Vector3(-6650, -150, 0), new Vector3(-9650, -150, 0)}, outerRooms[0], 0, 0);

        //outerRooms[1]
        CreateAndCorrect("Toilet", Rotation(pa.toilet_coordinates), outerRooms[1], 5250f, 300f);
        CreateAndCorrect("Kitchen", pa.kitchen_coordinates, outerRooms[1], 3350f, 200f);
        CreateAndCorrect("Ub", pa.ub_coordinates, outerRooms[1], 2100f, 2450f);
        CreateAndCorrect("Washroom", pa.washroom_coordinates, outerRooms[1], 3850f, 2350f);
        CreateAndCorrect("Western", new Vector3[]{new Vector3(4650, 3150, 0), new Vector3(9650, 3150, 0), new Vector3(9650, -150, 0), new Vector3(5950, -150, 0), new Vector3(5950, 1550, 0), new Vector3(4650, 1550, 0)}, outerRooms[0], 0, 0);

        //outerRooms[2]
        CreateAndCorrect("Toilet", Rotation(pa.toilet_coordinates), outerRooms[2], -1050f, -3100f);
        CreateAndCorrect("Kitchen", pa.kitchen_coordinates, outerRooms[2], -4400f, -3200f);
        CreateAndCorrect("Ub", Rotation(pa.ub_coordinates), outerRooms[2], -3550f, -1100f);
        CreateAndCorrect("Washroom", pa.washroom_coordinates, outerRooms[2], -5050f, -950f);
        CreateAndCorrect("Western", new Vector3[]{new Vector3(-9650, -150, 0), new Vector3(-5850, -150, 0), new Vector3(-5850, -1750, 0), new Vector3(-5600, -1750, 0), new Vector3(-5600, -3550, 0), new Vector3(-9650, -3550, 0)}, outerRooms[0], 0, 0);

        //outerRooms[3]
        CreateAndCorrect("Toilet", pa.toilet_coordinates, outerRooms[3], 2600f, -850f);
        CreateAndCorrect("Kitchen", pa.kitchen_coordinates, outerRooms[3], 4400f, -3200f);
        CreateAndCorrect("Ub", pa.ub_coordinates, outerRooms[3], 5600f, -850f);
        CreateAndCorrect("Washroom", pa.washroom_coordinates, outerRooms[3], 3850f, -950f);
        CreateAndCorrect("Western", new Vector3[]{new Vector3(6550, -150, 0), new Vector3(9650, -150, 0), new Vector3(9650, -3550, 0), new Vector3(5600, -3550, 0), new Vector3(5600, -1550, 0), new Vector3(6550, -1550, 0)}, outerRooms[0], 0, 0);
        */
    }

    void Update()
    {   
        /*
        if (Input.GetKeyDown("return")) {
            outerRooms = new GameObject[]{pm.plan[0], pm.plan[1], pm.plan[2], pm.plan[3]};

            placement();

        }
        */
    }

    
    /*
    部屋を配置していく
    */
    public void placement() {

        placeMbps();

        placeEntrance();

        placeWestern();
    }
    
    /***

    組み合わせに関するメソッド

    ***/
    void combination(int[] pattern, List<int> elems, int n, int num_decided) {

        int num_selected = getNumSelected(pattern, num_decided);

        if (num_decided == n) {
            /* n個全ての要素に対して"選ぶ"or"選ばない"が決定ずみ */
            if (num_selected == 2) {
                /* 2個だけ選ばれている場合のみ、選ばれた要素を表示 */
                makeCombinationList(pattern, elems, n);
            }
            return;
        }

        /* num_decided個目の要素を"選ぶ"場合のパターンを作成 */
        pattern[num_decided] = 1;
        combination(pattern, elems, elems.Count, num_decided + 1);

        /* num_decided個目の要素を"選ばない"場合のパターンを作成 */
        pattern[num_decided] = 0;
        combination(pattern, elems, elems.Count, num_decided + 1);
    }

    int getNumSelected(int[] pattern, int n) {
        /* "選ぶ"と決定された要素の数を計算 */
        
        int num_selected = 0;
        for (int i = 0; i < n; i++) {
            num_selected += pattern[i];
        }

        return num_selected;
    }

    void makeCombinationList(int[] pattern, List<int> elems, int n) {
        List<int> contact_stairs_one_pattern = new List<int>();

        for (int i = 0; i < n; i++) {
            for (int j = 0; j < pattern[i]; j++) {
                contact_stairs_one_pattern.Add(elems[i]);
            }
        }

        //階段室の1辺の座標
        GameObject stairs = GameObject.Find("Stairs");
        Vector3[] stairs_coordinates = new Vector3[stairs.GetComponent<LineRenderer>().positionCount];
        for (int i = 0; i < stairs.GetComponent<LineRenderer>().positionCount; i++) {
            stairs_coordinates[i] = new Vector3(stairs.GetComponent<LineRenderer>().GetPosition(i).x + stairs.transform.position.x, stairs.GetComponent<LineRenderer>().GetPosition(i).y + stairs.transform.position.y, 0);
        }

        for (int i = 0; i < stairs_coordinates.Length; i++) {
            int count = 0;
            for (int j = 0; j < contact_stairs.Count; j++) {
                if (contact_stairs[j][0] == i) {
                    if ((contact_stairs[j][1] == contact_stairs_one_pattern[0]) || (contact_stairs[j][1] == contact_stairs_one_pattern[1])) {
                        count++;
                    }
                }
            }

            if (count > 1) {
                contact_stairs_all_pattern.Add(contact_stairs_one_pattern);
            }
        }
    }

    public void makeAllPatternList() {
        //List<List<int>> contact_stairs = new List<List<int>>();//階段室の1辺の座標（小さい方のリスト番号）とそれに接してる部屋のリスト番号の組み合わせ
        //List<int> contact_room_number = new List<int>();//階段室と接している部屋のうち，同じ辺に2つの部屋が接している部屋のリスト番号
        
        /* 階段室の1辺と部屋の接してる座標を求め, リストを作成 */
        //階段室の1辺の座標
        Vector3[] stairs_coordinates = coordinatesOfRoom(GameObject.Find("Stairs"));

        //階段室の1辺とそこに接している部屋の組み合わせのリストを作成し，それら全てを管理するリストを作成
        for (int i = 0; i < stairs_coordinates.Length; i++) {
            Vector3 s1, s2;
            s1 = stairs_coordinates[i];
            s2 = stairs_coordinates[(i+1)%stairs_coordinates.Length];
            
            for (int j = 0; j < outerRooms.Length; j++) {
                Vector3[] contact_coodinates = contact(new Vector3[]{s1, s2}, outerRooms[j]);
                Vector3[] zero = new Vector3[] {Vector3.zero, Vector3.zero};
                
                if ((contact_coodinates[0] != zero[0]) && (contact_coodinates[1] != zero[1])) {
                    List<int> contact_room = new List<int>();
                    contact_room.Add(i);
                    contact_room.Add(j);
                    contact_stairs.Add(contact_room);
                }
            }
        }

        for (int i = 0; i < stairs_coordinates.Length; i++) {
            int count = 0;
            for (int j = 0; j < contact_stairs.Count; j++) {
                if (contact_stairs[j][0] == i) {
                    count++;
                }
            }
            
            if (count < 2) {
                for (int j = contact_stairs.Count - 1; j >= 0; j--) {
                    if (contact_stairs[j][0] == i) {
                        contact_stairs.RemoveAt(j);
                    }
                }
            }
        }

        /* リストからMBPSがまたぐ部屋の組み合わせを作成 */
        for (int i = 0; i < contact_stairs.Count; i++) {
            for (int j = 0; j < contact_stairs[i].Count; j++) {
                if (!contact_room_number.Contains(contact_stairs[i][j])) {
                    contact_room_number.Add(contact_stairs[i][j]);
                }
            }
        }
        contact_room_number.Sort();

        
        int[] pattern = new int[contact_room_number.Count];

        combination(pattern, contact_room_number, contact_room_number.Count, 0);
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

    public void placeMbpsInTwoRoomsRoop(int[] mbpsExist) {
        makeAllPatternList();

        while (contact_room_number.Count >= 2) {
            //これからMBPSを配置する2部屋の組み合わせ
            int[] thisPattern = new int[2];

            if (contact_room_number.Count >= 4) {
                //ここのcontact_stairs_all_pattern[ここ][0]をいじる必要あり
                thisPattern[0] = contact_stairs_all_pattern[0][0];
                thisPattern[1] = contact_stairs_all_pattern[0][1];

                placeMbpsInTwoRooms(mbpsExist, thisPattern);

                contact_room_number.Remove(thisPattern[0]);
                contact_room_number.Remove(thisPattern[1]);
            } else if (contact_room_number.Count >= 2) {
                thisPattern[0] = contact_room_number[0];
                thisPattern[1] = contact_room_number[1];

                placeMbpsInTwoRooms(mbpsExist, thisPattern);

                contact_room_number.Remove(thisPattern[0]);
                contact_room_number.Remove(thisPattern[1]);
            }
        }
    }

    public void placeMbpsInTwoRooms(int[] mbpsExist, int[] pattern) {
        //階段室の座標
        Vector3[] stairs_coordinate = coordinatesOfRoom(GameObject.Find("Stairs"));

        //MBPSをくっつける階段室の辺を求める
        int stairs_num = 0;

        for (int i = 0; i < stairs_coordinate.Length; i++) {
            int count = 0;
            for (int j = 0; j < contact_stairs.Count; j++) {
                if (contact_stairs[j][0] == i) {
                    if ((contact_stairs[j][1] == pattern[0]) || (contact_stairs[j][1] == pattern[1])) {
                        count++;
                    }
                }
            }

            if (count > 1) {
                stairs_num = i;
            }
        }

        for (int i = 0; i < pattern.Length; i++) {
            Vector3 s1 = new Vector3(0, 0, 0);
            Vector3 s2 = new Vector3(0, 0, 0);

            Vector3[] contact_coodinates = contact(new Vector3[]{stairs_coordinate[stairs_num], stairs_coordinate[(stairs_num+1)%stairs_coordinate.Length]}, outerRooms[pattern[i]]);
            Vector3[] zero = new Vector3[] {Vector3.zero, Vector3.zero};
            
            if ((contact_coodinates[0] != zero[0]) && (contact_coodinates[1] != zero[1])) {
                s1 = contact_coodinates[0];
                s2 = contact_coodinates[1];
            }
            

            for (int j = 0; j < pa.mbps_coordinate.Length; j++) {
                if (positionalRelation(new Vector3[]{s1, s2}, new Vector3[]{pa.mbps_coordinate[j], pa.mbps_coordinate[(j+1)%pa.mbps_coordinate.Length]})[2] == 1.00f) {
                    Vector3[] mbps_coordinate = Rotation(pa.mbps_coordinate);

                    float gap_x = positionalRelation(new Vector3[]{s1, s2}, new Vector3[]{mbps_coordinate[j], mbps_coordinate[(j+1)%mbps_coordinate.Length]})[0] - positionalRelation(new Vector3[]{s1, s2}, new Vector3[]{mbps_coordinate[j], mbps_coordinate[(j+1)%mbps_coordinate.Length]})[1];
                    float gap_y;
                    float gap_y_A = 0;
                    float gap_y_B = 0;
                    
                    Vector3[] outerroom_coordinates = coordinatesOfRoom(outerRooms[pattern[i]]);

                    for (int k = 0; k < outerroom_coordinates.Length; k++) {
                        Vector3[] contact_outerrooms_coodinates = contact(new Vector3[] {outerroom_coordinates[k], outerroom_coordinates[(k+1)%outerroom_coordinates.Length]}, outerRooms[pattern[(i+1)%pattern.Length]]); 
                        if ((contact_outerrooms_coodinates[0] != zero[0]) && (contact_outerrooms_coodinates[1] != zero[1])) {
                            if (CheckPoint(GameObject.Find("Stairs"), new Vector3 (contact_outerrooms_coodinates[0].x + 1, contact_outerrooms_coodinates[0].y, 0))) {
                                gap_y_A = contact_outerrooms_coodinates[0].y + Mathf.Abs(mbps_coordinate[0].y) + 1;
                                gap_y_B = contact_outerrooms_coodinates[0].y - Mathf.Abs(mbps_coordinate[0].y) - 1;
                            } else if (CheckPoint(GameObject.Find("Stairs"), new Vector3 (contact_outerrooms_coodinates[0].x - 1, contact_outerrooms_coodinates[0].y, 0))) {
                                gap_y_A = contact_outerrooms_coodinates[0].y + Mathf.Abs(mbps_coordinate[0].y) + 1;
                                gap_y_B = contact_outerrooms_coodinates[0].y - Mathf.Abs(mbps_coordinate[0].y) - 1;
                            } else if (CheckPoint(GameObject.Find("Stairs"), new Vector3 (contact_outerrooms_coodinates[1].x + 1, contact_outerrooms_coodinates[1].y, 0))) {
                                gap_y_A = contact_outerrooms_coodinates[0].y + Mathf.Abs(mbps_coordinate[0].y) + 1;
                                gap_y_B = contact_outerrooms_coodinates[0].y - Mathf.Abs(mbps_coordinate[0].y) - 1;
                            } else if (CheckPoint(GameObject.Find("Stairs"), new Vector3(contact_outerrooms_coodinates[1].x - 1, contact_outerrooms_coodinates[1].y, 0))) {
                                gap_y_A = contact_outerrooms_coodinates[0].y + Mathf.Abs(mbps_coordinate[0].y) + 1;
                                gap_y_B = contact_outerrooms_coodinates[0].y - Mathf.Abs(mbps_coordinate[0].y) - 1;
                            }
                        }
                    }

                    int need_count = 0;
                    int act_count_A = 0;
                    int act_count_B = 0;
                    for (int k = 0; k < mbps_coordinate.Length; k++) {
                        if ((mbps_coordinate[k] != mbps_coordinate[j]) && (mbps_coordinate[k] != mbps_coordinate[(j+1)%mbps_coordinate.Length])) {
                            need_count++;
                            if (CheckPoint(outerRooms[pattern[i]], new Vector3(mbps_coordinate[k].x + gap_x, mbps_coordinate[k].y + gap_y_A, 0))) {
                                act_count_A++;
                            }
                            if (CheckPoint(outerRooms[pattern[i]], new Vector3(mbps_coordinate[k].x + gap_x, mbps_coordinate[k].y + gap_y_B, 0))) {
                                act_count_B++;
                            }
                        }
                    }
                    
                    
                    if (need_count == act_count_A) {
                        gap_y = gap_y_A - 1;

                        createRoom("MbpsOf" + outerRooms[pattern[i]].name, mbps_coordinate);
                        //createRoom("MbpsOf" + outerRooms[pattern[i]].name, mbps_coordinate, Color.red);

                        Vector3 mbps_pos = GameObject.Find("MbpsOf" + outerRooms[pattern[i]].name).transform.position;

                        mbps_pos.x += gap_x;
                        mbps_pos.y += gap_y;
                        
                        GameObject.Find("MbpsOf" + outerRooms[pattern[i]].name).transform.position = mbps_pos;

                        mbpsExist[pattern[i]] = 1;
                    }
                    if (need_count == act_count_B) {
                        gap_y = gap_y_B + 1;

                        createRoom("MbpsOf" + outerRooms[pattern[i]].name, mbps_coordinate);
                        //createRoom("MbpsOf" + outerRooms[pattern[i]].name, mbps_coordinate, Color.red);

                        Vector3 mbps_pos = GameObject.Find("MbpsOf" + outerRooms[pattern[i]].name).transform.position;

                        mbps_pos.x += gap_x;
                        mbps_pos.y += gap_y;
                        
                        GameObject.Find("MbpsOf" + outerRooms[pattern[i]].name).transform.position = mbps_pos;

                        mbpsExist[pattern[i]] = 1;
                    }        
                }
                else if (positionalRelation(new Vector3[]{s1, s2}, new Vector3[]{pa.mbps_coordinate[j], pa.mbps_coordinate[(j+1)%pa.mbps_coordinate.Length]})[2] == 2.00f) {
                    float gap_x = 0;
                    float gap_x_A = 0;
                    float gap_x_B = 0;
                    
                    Vector3[] outerroom_coordinates = coordinatesOfRoom(outerRooms[pattern[i]]);

                    for (int k = 0; k < outerroom_coordinates.Length; k++) {
                        Vector3[] contact_outerrooms_coodinates = contact(new Vector3[] {outerroom_coordinates[k], outerroom_coordinates[(k+1)%outerroom_coordinates.Length]}, outerRooms[pattern[(i+1)%pattern.Length]]); 
                        if ((contact_outerrooms_coodinates[0] != zero[0]) && (contact_outerrooms_coodinates[1] != zero[1])) {
                            if (CheckPoint(GameObject.Find("Stairs"), new Vector3 (contact_outerrooms_coodinates[0].x, contact_outerrooms_coodinates[0].y + 1, 0))) {
                                gap_x_A = contact_outerrooms_coodinates[0].x + Mathf.Abs(pa.mbps_coordinate[0].x) + 1;
                                gap_x_B = contact_outerrooms_coodinates[0].x - Mathf.Abs(pa.mbps_coordinate[0].x) - 1;
                            } else if (CheckPoint(GameObject.Find("Stairs"), new Vector3 (contact_outerrooms_coodinates[0].x, contact_outerrooms_coodinates[0].y - 1, 0))) {
                                gap_x_A = contact_outerrooms_coodinates[0].x + Mathf.Abs(pa.mbps_coordinate[0].x) + 1;
                                gap_x_B = contact_outerrooms_coodinates[0].x - Mathf.Abs(pa.mbps_coordinate[0].x) - 1;
                            } else if (CheckPoint(GameObject.Find("Stairs"), new Vector3 (contact_outerrooms_coodinates[1].x, contact_outerrooms_coodinates[1].y + 1, 0))) {
                                gap_x_A = contact_outerrooms_coodinates[0].x + Mathf.Abs(pa.mbps_coordinate[0].x) + 1;
                                gap_x_B = contact_outerrooms_coodinates[0].x - Mathf.Abs(pa.mbps_coordinate[0].x) - 1;
                            } else if (CheckPoint(GameObject.Find("Stairs"), new Vector3(contact_outerrooms_coodinates[1].x, contact_outerrooms_coodinates[1].y - 1, 0))) {
                                gap_x_A = contact_outerrooms_coodinates[0].x + Mathf.Abs(pa.mbps_coordinate[0].x) + 1;
                                gap_x_B = contact_outerrooms_coodinates[0].x - Mathf.Abs(pa.mbps_coordinate[0].x) - 1;
                            }
                        }
                    }

                    float gap_y = positionalRelation(new Vector3[]{s1, s2}, new Vector3[]{pa.mbps_coordinate[j], pa.mbps_coordinate[(j+1)%pa.mbps_coordinate.Length]})[0] - positionalRelation(new Vector3[]{s1, s2}, new Vector3[]{pa.mbps_coordinate[j], pa.mbps_coordinate[(j+1)%pa.mbps_coordinate.Length]})[1];

                    int need_count = 0;
                    int act_count_A = 0;
                    int act_count_B = 0;
                    for (int k = 0; k < pa.mbps_coordinate.Length; k++) {
                        if ((pa.mbps_coordinate[k] != pa.mbps_coordinate[j]) && (pa.mbps_coordinate[k] != pa.mbps_coordinate[(j+1)%pa.mbps_coordinate.Length])) {
                            need_count++;
                            if (CheckPoint(outerRooms[pattern[i]], new Vector3(pa.mbps_coordinate[k].x + gap_x_A, pa.mbps_coordinate[k].y + gap_y, 0))) {
                                act_count_A++;
                            }
                            if (CheckPoint(outerRooms[pattern[i]], new Vector3(pa.mbps_coordinate[k].x + gap_x_B, pa.mbps_coordinate[k].y + gap_y, 0))) {
                                act_count_B++;
                            }
                        }
                    }
                    
                    
                    if (need_count == act_count_A) {
                        gap_x = gap_x_A - 1;

                        createRoom("MbpsOf" + outerRooms[pattern[i]].name, pa.mbps_coordinate);
                        //createRoom("MbpsOf" + outerRooms[pattern[i]].name, pa.mbps_coordinate, Color.red);

                        Vector3 mbps_pos = GameObject.Find("MbpsOf" + outerRooms[pattern[i]].name).transform.position;

                        mbps_pos.x += gap_x;
                        mbps_pos.y += gap_y;
                        
                        GameObject.Find("MbpsOf" + outerRooms[pattern[i]].name).transform.position = mbps_pos;

                        mbpsExist[pattern[i]] = 1;
                    }
                    if (need_count == act_count_B) {
                        gap_x = gap_x_B + 1;

                        createRoom("MbpsOf" + outerRooms[pattern[i]].name, pa.mbps_coordinate);
                        //createRoom("MbpsOf" + outerRooms[pattern[i]].name, pa.mbps_coordinate, Color.red);

                        Vector3 mbps_pos = GameObject.Find("MbpsOf" + outerRooms[pattern[i]].name).transform.position;

                        mbps_pos.x += gap_x;
                        mbps_pos.y += gap_y;
                        
                        GameObject.Find("MbpsOf" + outerRooms[pattern[i]].name).transform.position = mbps_pos;

                        mbpsExist[pattern[i]] = 1;
                    }
                }
            }
        }
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

    /***

    洋室
    
    ***/
    public void placeWestern() {
        GameObject[] balcony = new GameObject[] {GameObject.Find("Balcony1"), GameObject.Find("Balcony2"), GameObject.Find("Balcony3"), GameObject.Find("Balcony4")};

        for (int i = 0; i < outerRooms.Length; i++) {
            Vector3[] western = new Vector3[4];

            //住戸とバルコニーの接する座標
            Vector3 c1 = new Vector3(0, 0, 0);
            Vector3 c2 = new Vector3(0, 0, 0);

            Vector3[] outerRoom_coordinates = coordinatesOfRoom(outerRooms[i]);
            for (int j = 0; j < balcony.Length; j++) {
                for (int k = 0; k < outerRoom_coordinates.Length; k++) {
                    Vector3[] contact_coodinates = contact(new Vector3[] {outerRoom_coordinates[k], outerRoom_coordinates[(k+1)%outerRoom_coordinates.Length]}, balcony[j]);
                    Vector3[] zero = new Vector3[] {Vector3.zero, Vector3.zero};
                    
                    if ((contact_coodinates[0] != zero[0]) && (contact_coodinates[1] != zero[1])) {
                        c1 = outerRoom_coordinates[k];
                        c2 = outerRoom_coordinates[(k+1)%outerRoom_coordinates.Length];
                    }
                }
            }

            western[0] = c1;
            western[1] = c2;

            float outerRoom_length = 0;

            if ((28 <= areaCalculation(outerRooms[i])) && (areaCalculation(outerRooms[i]) < 29)) {
                outerRoom_length = 8.5f * 1620000 / Vector3.Distance(c1, c2);
            }
            else if ((29 <= areaCalculation(outerRooms[i])) && (areaCalculation(outerRooms[i]) < 30)) {
                outerRoom_length = 9.5f * 1620000 / Vector3.Distance(c1, c2);
            }
            else if ((30 <= areaCalculation(outerRooms[i])) && (areaCalculation(outerRooms[i]) < 31)) {
                outerRoom_length = 10.5f * 1620000 / Vector3.Distance(c1, c2);
            }

            if (western[0].x == western[1].x) {
                if (CheckPoint(outerRooms[i], new Vector3(western[1].x + outerRoom_length, western[1].y - 1, 0)) && CheckPoint(outerRooms[i], new Vector3(western[0].x + outerRoom_length, western[0].y + 1, 0))) {
                    western[2] = new Vector3(western[1].x + outerRoom_length, western[1].y, 0);
                    western[3] = new Vector3(western[0].x + outerRoom_length, western[0].y, 0);
                }
                else {
                    western[2] = new Vector3(western[1].x - outerRoom_length, western[1].y, 0);
                    western[3] = new Vector3(western[0].x - outerRoom_length, western[0].y, 0);
                }
            }
            else if (western[0].y == western[1].y) {
                if (CheckPoint(outerRooms[i], new Vector3(western[1].x - 1, western[1].y + outerRoom_length, 0)) && CheckPoint(outerRooms[i], new Vector3(western[0].x + 1, western[0].y + outerRoom_length, 0))) {
                    western[2] = new Vector3(western[1].x + outerRoom_length, western[1].y, 0);
                    western[3] = new Vector3(western[0].x + outerRoom_length, western[0].y, 0);
                }
                else {
                    western[2] = new Vector3(western[1].x - outerRoom_length, western[1].y, 0);
                    western[3] = new Vector3(western[0].x - outerRoom_length, western[0].y, 0);
                }
            }

            createRoom("WesternOf" + outerRooms[i].name, western);
            //createRoom("WesternOf" + outerRooms[i].name, western, Color.yellow);
        }
    }

    /***

    リビング・ダイニング(未完成)
    
    ***/
    public void placeLd(GameObject outerRoom) {
        
    }

    /***

    直線と直線が平行かどうかを確認し，
    平行な直線どうしの切片を返すメソッド

    ***/
    public float[] positionalRelation(Vector3[] sideA, Vector3[] sideB) {
        float m1, m2, n1, n2;

        //y軸平行の時で直線と部屋が平行
        if (sideA[0].x == sideA[1].x) {
            n1 = sideA[0].x;
            if (sideB[0].x == sideB[1].x) {
                n2 = sideB[0].x;

                return new float[] {n1, n2, 1.00f};
            }
        }
        //それ以外で直線と部屋が平行
        else {
            m1 = (sideA[1].y - sideA[0].y) / (sideA[1].x - sideA[0].x);
            n1 = (sideA[1].x * sideA[0].y - sideA[0].x * sideA[1].y) / (sideA[1].x - sideA[0].x);
            if (sideB[0].x != sideB[1].x) {
                m2 = (sideB[1].y - sideB[0].y) / (sideB[1].x - sideB[0].x);
                n2 = (sideB[1].x * sideB[0].y - sideB[0].x * sideB[1].y) / (sideB[1].x - sideB[0].x);
                if (m1 == m2) {

                    return new float[] {n1, n2, 2.00f};
                }
            }     
        }

        //直線と部屋が平行でない
        return new float[] {0.00f, 0.00f, 0.00f};
    }

    /***

    部屋を生成して，位置を調整

    ***/
    public void CreateAndCorrect(string name, Vector3[] room, GameObject outerRoom, float gap_x, float gap_y) {
        createRoom(name + "Of" + outerRoom.name, room);

        Vector3 mbps_pos = GameObject.Find(name + "Of" + outerRoom.name).transform.position;

        mbps_pos.x += gap_x;
        mbps_pos.y += gap_y;
        
        GameObject.Find(name + "Of" + outerRoom.name).transform.position = mbps_pos;
    }

    /***

    座標を原点周りに90°回転させて返す

    ***/
    public Vector3[] Rotation(Vector3[] room) {
        Vector3[] rotatedCoordinates = new Vector3[room.Length];
        for (int i = 0; i < room.Length; i++) {
            rotatedCoordinates[i].x = - room[(i+1)%room.Length].y;
            rotatedCoordinates[i].y = room[(i+1)%room.Length].x;
        }

        return rotatedCoordinates;
    }

    /*** 
    
    部屋のゲームオブジェクトの座標データ
    
    ***/
    public Vector3[] coordinatesOfRoom(GameObject room) {
        Vector3[] coordinates = new Vector3[room.GetComponent<LineRenderer>().positionCount];
        for (int i = 0; i < room.GetComponent<LineRenderer>().positionCount; i++) {
            coordinates[i] = new Vector3(room.GetComponent<LineRenderer>().GetPosition(i).x + room.transform.position.x, room.GetComponent<LineRenderer>().GetPosition(i).y + room.transform.position.y, 0);
        }

        return coordinates;
    }

    /*
    /// <summary>
    /// 図形Aに対する図形の内分，重なり判定
    /// </summary>
    /// <param name="A">判定に使う図形の座標集合</param>
    /// <param name="target">判定をする図形の座標集合</param>
    /// <returns>内分："inside",外分："outside"，重なり："overlap"</returns>
    string JudgeLayer(Vector3[] A, Vector3[] target) {

        bool flag_side = false;

        for (int i = 0;i < target.Length; i++) {
            if (i == 0) {
                flag_side = CheckPoint(A, target[i]);
                continue;
            }

            if (flag_side != CheckPoint(A, target[i])) {
                //Debug.Log(i + " " + flag_side + " " +  CheckPoint(A, target[i]) + " " + target[i]);
                return "FigStatus_overlap";
            }
        }

        return flag_side ? "FigStatus_inside" : "FigStatus_outside";
    }
    */

    /// <summary>
    /// 図形に対する点の内外判定
    /// </summary> 
    /// <param name="points">図形の座標配列</param>
    /// <param name="target">判定する点の座標</param>
    /// <returns>内分の場合True，外分の場合Flase</returns>
    public bool CheckPoint(GameObject room, Vector3 target) {
        Vector3[] points = new Vector3[room.GetComponent<LineRenderer>().positionCount];
        for (int i = 0; i < room.GetComponent<LineRenderer>().positionCount; i++) {
            points[i] = new Vector3(room.GetComponent<LineRenderer>().GetPosition(i).x, room.GetComponent<LineRenderer>().GetPosition(i).y, 0);
        }

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
