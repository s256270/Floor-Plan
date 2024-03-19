using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CreateWetareas : MonoBehaviour
{
    [SerializeField] CommonFunctions cf;
    [SerializeField] Parts pa;
    [SerializeField] MakeWetareasPermutation mwp;
    [SerializeField] RemoveIrregularWetareas riw;

    //住戸の座標
    Vector3[] dwelling;
    //バルコニーの座標
    Vector3[] balcony;
    //配置範囲の座標
    Vector3[] range;
    //玄関の座標
    Vector3[] entrance;
    //玄関から廊下に続く辺
    List<Vector3[]> entranceToHallway = new List<Vector3[]>();
    //MBPSの座標
    Vector3[] mbps;

    /// <summary>
    /// 水回りの部屋を配置
    /// </summary>
    /// <param name="allPattern">全ての配置結果</param>
    /// <returns>水回りの部屋を全て配置した結果</returns>
    public List<Dictionary<string, Dictionary<string, Vector3[]>>> PlaceWetareas(List<Dictionary<string, Dictionary<string, Vector3[]>>> allPattern) {
        //配置結果のリスト
        var result = new List<Dictionary<string, Dictionary<string, Vector3[]>>>();

        //各パターンについて配置
        for (int i = 0; i < allPattern.Count; i++) {
            //住戸1のとき
            if (!(i % 6 == 0)) {
                continue;
            }

            //住戸2のとき
            // if (!(i == 0 || i == 3)) {
            //     continue;
            // }

            // //住戸3のとき(パターンが少ない)
            // if (!(i == 0 || i == 1 || i == 2)) {
            //     continue;
            // }

            //住戸4のとき
            // if (!(i == 0)) {
            //     continue;
            // }

            //Debug.Log("i: " + i);
            foreach (KeyValuePair<string, Dictionary<string, Vector3[]>> planParts in allPattern[i]) {
                //住戸オブジェクトに配置していく
                if (planParts.Key.Contains("Dwelling1")) {
                    //必要な座標の準備
                    //住戸の座標を取得
                    dwelling = planParts.Value["1K"];

                    //バルコニーの座標を取得
                    balcony = planParts.Value["Balcony"];

                    //玄関の座標を取得
                    entrance = planParts.Value["Entrance"];

                    //MBPSの座標を取得
                    mbps = planParts.Value["Mbps"];

                    //配置範囲の座標を作成
                    range = FrameChange(dwelling, mbps);
                    //cf.CreateRoom("range", range);
                    range = FrameChange(range, entrance);

                    //cf.CreateRoom("range", range);

                    // cf.CreateRoom("dwelling", dwelling);
                    // cf.CreateRoom("range", range);

                    //水回りを配置したリストを作成
                    //List<Dictionary<string, Vector3[]>> placeWetareasResult = MakeWetareasAddList(new int[]{0, 1, 2, 3});
                    List<Dictionary<string, Vector3[]>> placeWetareasResult = MakeWetareasAddList(new int[]{0, 1, 2});

                    //作成したリストを配置結果のリストに追加
                    for (int j = 0; j < placeWetareasResult.Count; j++) {
                        //配置したパターン
                        Dictionary<string, Dictionary<string, Vector3[]>> patternToPlace = cf.DuplicateDictionary(allPattern[i]);

                        foreach (KeyValuePair<string, Vector3[]> wetAreaRoom in placeWetareasResult[j]) {
                            patternToPlace[planParts.Key].Add(wetAreaRoom.Key, wetAreaRoom.Value);
                        }

                        result.Add(patternToPlace);
                    }
                }
            }
        }

        //重複を除く
        cf.RemoveDuplicates(result);

        return result;
    }

    /// <summary>
    /// 水回りの部屋を配置
    /// </summary>
    /// <param name="wetareasKinds">水回りの部屋の種類を指定する配列</param>
    /// <returns>水回りの部屋を全て配置した結果</returns>
    public List<Dictionary<string, Vector3[]>> MakeWetareasAddList(int[] wetareasKinds) {
        //配置結果のリスト
        var result = new List<Dictionary<string, Vector3[]>>();

        //水回りパーツの全並び順
        List<int[]> wetareasAllPermutation = mwp.MakeWetareasKindsPermutation(wetareasKinds);
        //水回りの各並び順について
        for (int i = 0; i < wetareasAllPermutation.Count; i++) {

            //回転のパターンの組み合わせ
            List<int[]> rotationAllPattern = mwp.MakeWetareasRotationPermutation(wetareasAllPermutation[i].Length);
            for (int j = 0; j < rotationAllPattern.Count; j++) {

                //キッチンと洗面室の回転は除く
                if (rotationAllPattern[j][0] == 1 || rotationAllPattern[j][2] == 1) {
                    continue;
                }
                
                //ひとつの並び順，回転パターンの配置結果をリストに追加
                result.AddRange(PlaceWetareasOnePattern(wetareasAllPermutation[i], rotationAllPattern[j]));
            }
        }

        return result;
    }

    /// <summary>
    /// あるパターンについて水回りの部屋を配置
    /// </summary>
    /// <param name="wetareasPermutation">水回りの部屋の並び順</param>
    /// <param name="rotationPattern">水回りの部屋の回転パターン</param>
    /// <returns>あるパターンについて水回りの部屋を配置した結果</returns>
    public List<Dictionary<string, Vector3[]>> PlaceWetareasOnePattern(int[] wetareasPermutation, int[] rotationPattern) {
        //配置結果のリスト
        var result = new List<Dictionary<string, Vector3[]>>();

        //水回りの部屋を配置する領域を作成
        List<List<Vector3[]>> areaToPlaceWetareas = MakeAreaToPlaceWetareas();

        //配置する領域について
        for (int i = 0; i < areaToPlaceWetareas.Count; i++) {
            //結果を一時的に保存するリスト
            List<Dictionary<string, Vector3[]>> currentResult = new List<Dictionary<string, Vector3[]>>();

            //2つの領域に配置
            for (int j = 0; j < areaToPlaceWetareas[i].Count; j++) {
                //cf.CreateRoom("areaToPlaceWetareas", areaToPlaceWetareas[i][j]);
                //配置結果をリストに追加
                currentResult.AddRange(PlaceWetareasOneArea(currentResult, areaToPlaceWetareas[i][j], wetareasPermutation, rotationPattern));
            }

            //問題のある水回りパターンを削除
            //currentResult = riw.RemoveIrregularWetareasPattern(currentResult);

            //必要な部屋がすべて配置されていないものを除く
            for (int j = 0; j < currentResult.Count; j++) {
                //必要な部屋がすべて配置されていないとき
                //if (!(currentResult[j].ContainsKey("UB") && currentResult[j].ContainsKey("Washroom") && currentResult[j].ContainsKey("Toilet") && currentResult[j].ContainsKey("Kitchen"))) {
                if (!(currentResult[j].ContainsKey("UBandWashroom") && currentResult[j].ContainsKey("Toilet") && currentResult[j].ContainsKey("Kitchen"))) {
                    //その配置結果を除く
                    currentResult.RemoveAt(j);
                    j--;
                }
            }
            
            //洗面室とユニットバスが隣り合っていないものを除く
            // for (int j = 0; j < currentResult.Count; j++) {
            //     //洗面室とユニットバスが隣り合っていないとき
            //     if (!cf.ContactJudge(currentResult[j]["UB"], currentResult[j]["Washroom"])) {
            //         //その配置結果を除く
            //         currentResult.RemoveAt(j);
            //         j--;
            //     }
            // }

            //入口がない部屋があるものを除く
            for (int j = 0; j < currentResult.Count; j++) {
                //ある部屋が他の部屋の入口を封鎖しないかを確認
                foreach (KeyValuePair<string, Vector3[]> currentRoom in currentResult[j]) {
                    //部屋の入口が封鎖されているとき
                    if (!SecureNecessarySide(currentRoom.Key, currentResult[j])) {
                        //その配置結果を除く
                        currentResult.RemoveAt(j);
                        j--;
                        //次の配置結果へ
                        break;
                    }
                }
            }

            //廊下がふさがれていそうなものを除く
            //住戸の辺のインデックスを平行なもの同士の組み合わせに分割してリスト化
            List<List<int>> dwellingParallelIndexs = new List<List<int>>();
            for (int j = 0; j < dwelling.Length-1; j++) {
                //チェックの起点となる辺
                Vector3[] checkEdge = new Vector3[]{dwelling[j], dwelling[(j+1)%dwelling.Length]};

                //リスト内に同じ辺がないとき
                if (!dwellingParallelIndexs.Any(x => x.Contains(j))) {

                    List<int> dwellingParallelIndex = new List<int>{j};
                    //他の辺と比較
                    for (int k = j+1; k < dwelling.Length; k++) {
                        //比較する辺
                        Vector3[] comparedEdge = new Vector3[]{dwelling[k], dwelling[(k+1)%dwelling.Length]};

                        //平行なとき
                        if (cf.Slope(checkEdge) == cf.Slope(comparedEdge)) {
                            //リストに追加
                            dwellingParallelIndex.Add(k);
                        }
                    }

                    if (dwellingParallelIndex.Count > 1) {
                        //リストを追加
                        dwellingParallelIndexs.Add(dwellingParallelIndex);
                    }
                }
            }

            //平行な辺に接する住戸をチェック
            for (int j = 0; j < currentResult.Count; j++) {
                //水回りの部屋について
                foreach (KeyValuePair<string, Vector3[]> wetareasRoom1 in currentResult[j]) {
                    bool breakFlag = false;

                    //部屋が接している辺のインデックス
                    List<int> contactEdgeIndex1 = new List<int>();

                    //部屋が接している辺のインデックスを取得
                    for (int k = 0; k < dwelling.Length; k++) {
                        //部屋が接しているとき
                        if (cf.ContactJudge(wetareasRoom1.Value, new Vector3[]{dwelling[k], dwelling[(k+1)%dwelling.Length]})) {
                            //リストに追加
                            contactEdgeIndex1.Add(k);
                        }
                    }

                    //別の水回りの部屋について
                    foreach (KeyValuePair<string, Vector3[]> wetareasRoom2 in currentResult[j]) {
                        //同じ部屋のとき
                        if (wetareasRoom1.Key == wetareasRoom2.Key) {
                            //次の部屋へ
                            continue;
                        }

                        //部屋が接している辺のインデックス
                        List<int> contactEdgeIndex2 = new List<int>();

                        //部屋が接している辺のインデックスを取得
                        for (int k = 0; k < dwelling.Length; k++) {
                            //部屋が接しているとき
                            if (cf.ContactJudge(wetareasRoom2.Value, new Vector3[]{dwelling[k], dwelling[(k+1)%dwelling.Length]})) {
                                //リストに追加
                                contactEdgeIndex2.Add(k);
                            }
                        }
                        
                        //2つのインデックスの組み合わせについて
                        for (int k = 0; k < contactEdgeIndex1.Count; k++) {
                            for (int l = 0; l < contactEdgeIndex2.Count; l++) {
                                //同じインデックスの組み合わせのとき
                                if (contactEdgeIndex1[k] == contactEdgeIndex2[l]) {
                                    //次の組み合わせへ
                                    continue;
                                }

                                //2つの部屋が接している辺が平行なとき
                                if (dwellingParallelIndexs.Any(x => x.Contains(contactEdgeIndex1[k]) && x.Contains(contactEdgeIndex2[l]))) {
                                    //部屋1と部屋2が接しているとき
                                    if (cf.ContactJudge(wetareasRoom1.Value, wetareasRoom2.Value)) {
                                        //その配置結果を除く
                                        currentResult.RemoveAt(j);
                                        j--;
                                        breakFlag = true;
                                        //次の配置結果へ
                                        break;
                                    }
                                }
                            }

                            //次の配置結果へ
                            if (breakFlag) {
                                break;
                            }
                        }

                        //次の配置結果へ
                        if (breakFlag) {
                            break;
                        }
                    }

                    //次の配置結果へ
                    if (breakFlag) {
                        break;
                    }
                }   
            }
            
            //玄関周りの廊下がふさがれているものを除く
            for (int j = 0; j < currentResult.Count; j++) {
                //全ての要素がなくなった時
                if (currentResult.Count == 0) {
                    //終了
                    break;
                }

                //玄関の廊下に続く辺の長さ
                float entranceToHallwayLength = cf.GetLength(entranceToHallway[i]);
                //玄関の廊下に続く辺の座標
                Vector3[] actEntranceToHallway = entranceToHallway[i];
                bool flag = false;

                //配置された水回りの部屋について
                foreach (Vector3[] wetareasRoom in currentResult[j].Values) {

                    //玄関の廊下に続く辺と接しているとき
                    if (cf.ContactJudge(entranceToHallway[i], wetareasRoom)) {
                        //接してない部分の座標
                        actEntranceToHallway = cf.SideSubstraction(entranceToHallway[i], cf.ContactCoordinates(entranceToHallway[i], wetareasRoom)[0])[0];
                        //接してない部分の長さ
                        entranceToHallwayLength = cf.GetLength(actEntranceToHallway);
                        //長さが900mmより短いとき
                        if (entranceToHallwayLength < 900f) {
                            //その配置結果を除く
                            currentResult.RemoveAt(j);
                            j--;
                            //次の配置結果へ
                            flag = true;
                            break;
                        }
                    }
                }

                //長さが900mmより短いとき
                if (flag) {
                    continue;
                }

                //entranceToHallwayLength×900の長方形が入るかを判定
                //判定するための長方形の座標
                Vector3[] judgeSquare = new Vector3[4];
                //配置する辺がy軸に平行なとき
                if (cf.Slope(actEntranceToHallway) == Mathf.Infinity) {
                    judgeSquare = new Vector3[]{new Vector3(-550, entranceToHallwayLength/2, 0), new Vector3(550, entranceToHallwayLength/2, 0), new Vector3(550, -entranceToHallwayLength/2, 0), new Vector3(-550, -entranceToHallwayLength/2, 0)};

                    //動かす方向
                    int wetareasShiftDirection = cf.ShiftJudge(range, actEntranceToHallway);

                    //x座標・y座標の移動
                    //部屋を辺の左側にするとき
                    if (wetareasShiftDirection < 0) {
                        judgeSquare = cf.CorrectCoordinates(judgeSquare, new Vector3(actEntranceToHallway[0].x - 550, (actEntranceToHallway[0].y + actEntranceToHallway[1].y)/2, 0));
                    }
                    //部屋を辺の右側にするとき
                    else if (wetareasShiftDirection > 0) {
                        judgeSquare = cf.CorrectCoordinates(judgeSquare, new Vector3(actEntranceToHallway[0].x + 550, (actEntranceToHallway[0].y + actEntranceToHallway[1].y)/2, 0));
                    }
                }
                //配置する辺がx軸に平行なとき
                else {
                    judgeSquare = new Vector3[]{new Vector3(-entranceToHallwayLength/2, 450, 0), new Vector3(entranceToHallwayLength/2, 450, 0), new Vector3(entranceToHallwayLength/2, -450, 0), new Vector3(-entranceToHallwayLength/2, -450, 0)};

                    //動かす方向
                    int wetareasShiftDirection = cf.ShiftJudge(range, actEntranceToHallway);

                    //x座標・y座標の移動
                    //部屋を辺の上側にするとき
                    if (wetareasShiftDirection > 0) {
                        judgeSquare = cf.CorrectCoordinates(judgeSquare, new Vector3((actEntranceToHallway[0].x + actEntranceToHallway[1].x)/2, actEntranceToHallway[0].y + 450, 0));
                    }
                    //部屋を辺の下側にするとき
                    else if (wetareasShiftDirection < 0) {
                        judgeSquare = cf.CorrectCoordinates(judgeSquare, new Vector3((actEntranceToHallway[0].x + actEntranceToHallway[1].x)/2, actEntranceToHallway[0].y - 450, 0));
                    }
                }

                //配置された水回りの部屋について
                foreach (Vector3[] wetareasRooom in currentResult[j].Values) {
                    //判定するための長方形の外側にないとき
                    if (!cf.JudgeOutside(judgeSquare, wetareasRooom)) {
                        //その配置結果を除く
                        currentResult.RemoveAt(j);
                        j--;
                        //次の配置結果へ
                        break;
                    }
                    if (!cf.JudgeOutside(wetareasRooom, judgeSquare)) {
                        //その配置結果を除く
                        currentResult.RemoveAt(j);
                        j--;
                        //次の配置結果へ
                        break;
                    }
                }
            }

            //部屋が他の部屋と完璧に重なっていないかの判定
            for (int j = 0; j < currentResult.Count; j++) {
                foreach (KeyValuePair<string, Vector3[]> wetareasRoom1 in currentResult[j]) {
                    bool breakFlag = false;
                    foreach (KeyValuePair<string, Vector3[]> wetareasRoom2 in currentResult[j]) {
                        //比較する部屋が同じ部屋のとき
                        if (wetareasRoom1.Key == wetareasRoom2.Key) {
                            //次の部屋へ
                            continue;
                        }

                        //部屋1の各辺について
                        for (int k = 0; k < wetareasRoom1.Value.Length; k++) {
                            //辺の中点
                            Vector3[] midPoint = new Vector3[]{(wetareasRoom1.Value[k] + wetareasRoom1.Value[(k+1)%wetareasRoom1.Value.Length])/2};

                            //部屋1の辺の中点が部屋2の外側にないとき
                            if (!cf.JudgeOutside(wetareasRoom2.Value, midPoint)) {
                                //その配置結果を除く
                                currentResult.RemoveAt(j);
                                j--;
                                breakFlag = true;
                                break;
                            }  
                        }

                        //次の配置結果へ
                        if (breakFlag) {
                            break;
                        }
                    }

                    //次の配置結果へ
                    if (breakFlag) {
                        break;
                    }
                }
            }

            //部屋が他の部屋と重なっていないかの判定
            for (int j = 0; j < currentResult.Count; j++) {
                foreach (KeyValuePair<string, Vector3[]> wetareasRoom1 in currentResult[j]) {
                    bool breakFlag = false;
                    foreach (KeyValuePair<string, Vector3[]> wetareasRoom2 in currentResult[j]) {
                        //比較する部屋が同じ部屋のとき
                        if (wetareasRoom1.Key == wetareasRoom2.Key) {
                            //次の部屋へ
                            continue;
                        }

                        //配置した部屋が他の部屋の外側にないとき
                        if (!cf.JudgeOutside(wetareasRoom1.Value, wetareasRoom2.Value)) {
                            //その配置結果を除く
                            currentResult.RemoveAt(j);
                            j--;
                            breakFlag = true;
                            break;
                        }

                        //次の配置結果へ
                        if (breakFlag) {
                            break;
                        }
                    }

                    //次の配置結果へ
                    if (breakFlag) {
                        break;
                    }
                }
            }

            //結果を追加
            result.AddRange(currentResult);
        }
        
        return result;
    }

    /// <summary>
    /// 水回りの部屋を配置する領域を作成
    /// </summary>
    /// <returns>水回りの部屋を配置する領域</returns>
    public List<List<Vector3[]>> MakeAreaToPlaceWetareas() {
        //配置する領域のリスト
        var result = new List<List<Vector3[]>>();

        //玄関の廊下に続く辺を決める
        List<Vector3[]> entranceSide = cf.ContactCoordinates(entrance, range);
        //最も長い辺を廊下に続く辺とする
        float entranceSideLength = cf.GetLength(entranceSide[0]);
        int entranceSideIndex = 0;
        for (int i = 1; i < entranceSide.Count; i++) {
            if (entranceSideLength < cf.GetLength(entranceSide[i])) {
                entranceSideLength = cf.GetLength(entranceSide[i]);
                entranceSideIndex = i;
            }
        }
        entranceSide = new List<Vector3[]>{entranceSide[entranceSideIndex]};

        //洋室の廊下に続く辺を決める
        List<Vector3[]> westernSide = cf.ContactCoordinates(balcony, range);
        //最も長い辺を廊下に続く辺とする
        float westernSideLength = cf.GetLength(westernSide[0]);
        int westernSideIndex = 0;
        for (int i = 1; i < westernSide.Count; i++) {
            if (westernSideLength < cf.GetLength(westernSide[i])) {
                westernSideLength = cf.GetLength(westernSide[i]);
                westernSideIndex = i;
            }
        }
        westernSide = new List<Vector3[]>{westernSide[westernSideIndex]};

        //玄関の廊下に続く辺について
        for (int i = 0; i < entranceSide.Count; i++) {
            //洋室の廊下に続く辺について
            for (int j = 0; j < westernSide.Count; j++) {
                //水回りを配置する領域
                List<Vector3[]> wetareas = new List<Vector3[]>();

                //玄関から洋室へつながる辺を仮決め
                //辺1
                Vector3[] hallwaySide1 = new Vector3[]{entranceSide[i][0], westernSide[j][0]};
                //辺2
                Vector3[] hallwaySide2 = new Vector3[]{entranceSide[i][1], westernSide[j][1]};

                //辺1を通るように切る
                List<Vector3[]> wetareas1Candidates = Slice(range, hallwaySide1);

                //辺1で分割したそれぞれの領域について
                for (int k = 0; k < wetareas1Candidates.Count; k++) {
                    //wetareasに領域が1つあるとき
                    if (wetareas.Count >= 1) {
                        break;    
                    }

                    //片方の領域に辺2のある端点が含まれているとき
                    if (cf.OnPolyogon(wetareas1Candidates[k], hallwaySide2[0])) {
                        //その領域に辺2のもう一方の端点が含まれているとき
                        if (cf.OnPolyogon(wetareas1Candidates[k], hallwaySide2[1])) {
                            //もう一方の領域を水回りを配置する領域のリストに追加
                            wetareas.Add(wetareas1Candidates[(k+1)%wetareas1Candidates.Count]);
                        }

                        //辺2のもう一方の端点が含まれていないとき
                        else {
                            //玄関から洋室へつながる辺を変更
                            hallwaySide1 = new Vector3[]{entranceSide[i][0], westernSide[j][1]};
                            hallwaySide2 = new Vector3[]{entranceSide[i][1], westernSide[j][0]};
                            //hallwaySide1 = new Vector3[]{entranceSide[k][0], westernSide[k][1]};
                            //hallwaySide2 = new Vector3[]{entranceSide[k][1], westernSide[k][0]};

                            //辺1を通るように切る
                            wetareas1Candidates = Slice(range, hallwaySide1);

                            //辺1で分割したそれぞれの領域について
                            for (int l = 0; l < wetareas1Candidates.Count; l++) {
                                //片方の領域に辺2のある端点が含まれているとき
                                if (cf.OnPolyogon(wetareas1Candidates[l], hallwaySide2[0])) {
                                    //もう一方の領域を水回りを配置する領域のリストに追加
                                    wetareas.Add(wetareas1Candidates[(l+1)%wetareas1Candidates.Count]);
                                }
                                //片方の領域に辺2のある端点が含まれていないとき
                                else {
                                    //その領域を水回りを配置する領域のリストに追加
                                    wetareas.Add(wetareas1Candidates[l]);
                                }
                            }
                        }
                    }
                }

                //辺2を通るように切る
                List<Vector3[]> wetareas2Candidates = Slice(range, hallwaySide2);
                //辺2で分割したそれぞれの領域について
                for (int k = 0; k < wetareas2Candidates.Count; k++) {
                    //wetareasに領域が2つあるとき
                    if (wetareas.Count >= 2) {
                        break;    
                    }

                    //片方の領域に辺1のある端点が含まれているとき
                    if (cf.OnPolyogon(wetareas2Candidates[k], hallwaySide1[1])) {
                        //もう一方の領域を水回りを配置する領域のリストに追加
                        wetareas.Add(wetareas2Candidates[(k+1)%wetareas2Candidates.Count]);
                    }
                    //片方の領域に辺1のある端点が含まれていないとき
                    else {
                        //その領域を水回りを配置する領域のリストに追加
                        wetareas.Add(wetareas2Candidates[k]);
                    }
                }

                //面積で降順にソート
                if (cf.AreaCalculation(wetareas[0]) < cf.AreaCalculation(wetareas[1])) {
                    wetareas.Reverse();
                }

                //水回りを配置する領域のリストに追加
                result.Add(wetareas);

                //玄関の廊下に続く辺を追加
                entranceToHallway.Add(entranceSide[i]);
            }  
        }

        return result;
    }

    /// <summary>
    /// 1つの領域について水回りの部屋を配置
    /// </summary>
    /// <param name="currentResult">現在の配置結果</param>
    /// <param name="areaToPlaceWetareas">水回りの部屋を配置する領域</param>
    /// <param name="wetareasPermutation">水回りの部屋の並び順</param>
    /// <param name="rotationPattern">水回りの部屋の回転パターン</param>
    /// <returns>1つの領域について水回りの部屋を配置した結果</returns>
    public List<Dictionary<string, Vector3[]>> PlaceWetareasOneArea(List<Dictionary<string, Vector3[]>> currentResult, Vector3[] areaToPlaceWetareas, int[] wetareasPermutation, int[] rotationPattern) {
        //配置結果のリスト
        var result = new List<Dictionary<string, Vector3[]>>(currentResult);

        //長い辺から順に並べていく
        int[] longIndex = LongIndex(areaToPlaceWetareas);
        for (int i = 0; i < longIndex.Length; i++) {
            int currentSideIndex = longIndex[i];
            Vector3[] currentSide = new Vector3[] {areaToPlaceWetareas[currentSideIndex], areaToPlaceWetareas[(currentSideIndex+1)%longIndex.Length]};
            
            //辺がrangeの辺上でないとき
            if (!cf.OnPolyogon(range, currentSide)) {
                //スキップ
                continue;
            }

            //パターンがないとき
            if (result.Count == 0) {
                //空のパターンを追加
                result.Add(new Dictionary<string, Vector3[]>());
            }

            //結果を一時的に保存するリスト
            List<Dictionary<string, Vector3[]>> currentResultList = new List<Dictionary<string, Vector3[]>>();
            
            //現在のパターンについて
            for (int j = 0; j < result.Count; j++) {
                //これから配置する部屋の状況（参照するだけ）
                Dictionary<string, Vector3[]> currentPattern = new Dictionary<string, Vector3[]>(result[j]);
                
                //配置する辺の更新
                currentSide = new Vector3[] {areaToPlaceWetareas[currentSideIndex], areaToPlaceWetareas[(currentSideIndex+1)%longIndex.Length]};
                //部屋パーツについて
                for (int k = 0; k < currentPattern.Count; k++) {
                    //配置する辺がなくなったとき
                    if (Vector3.Distance(currentSide[0], currentSide[1]) == 0f) {
                        //スキップ
                        break;
                    }
                    //配置する辺と部屋パーツが接しているとき
                    if (cf.ContactJudge(currentSide, currentPattern.Values.ElementAt(k))) {
                        //接している部分を除く
                        currentSide = cf.SideSubstraction(currentSide, cf.ContactCoordinates(currentSide, currentPattern.Values.ElementAt(k))[0])[0];
                    }
                }

                //全ての部屋が配置されている場合
                //if (currentPattern.ContainsKey("UB") && currentPattern.ContainsKey("Washroom") && currentPattern.ContainsKey("Toilet") && currentPattern.ContainsKey("Kitchen")) {
                if (currentPattern.ContainsKey("UBandWashroom") && currentPattern.ContainsKey("Toilet") && currentPattern.ContainsKey("Kitchen")) {
                    //そのまま追加して，これ以降はスキップ
                    currentResultList.Add(currentPattern);
                    continue;
                }

                //1辺に可能な限り配置する
                List<Dictionary<string, Vector3[]>> placementResultList = PlaceWetareasActually(currentPattern, currentSide, wetareasPermutation, rotationPattern);

                //1辺の配置結果について
                for (int k = 0; k < placementResultList.Count; k++) {
                    //1つの配置結果
                    Dictionary<string, Vector3[]> placementResult = placementResultList[k];

                    //配置結果を追加
                    currentResultList.Add(placementResult);

                    //増えた部屋名を取得
                    string[] differentKeys = KeysDifference(placementResult, currentPattern, wetareasPermutation);

                    //増えた部屋名について
                    for (int l = 1; l < differentKeys.Length + 1; l++) {
                        //後ろからk個の部屋名を取得
                        string[] keysToRemove = differentKeys[^l..];

                        //後ろからk個の部屋を除く
                        placementResult = RemoveDictionaryElement(placementResult, keysToRemove);

                        //除いたパターンも追加する
                        currentResultList.Add(placementResult);
                    }
                }
            }

            //結果を更新
            result = currentResultList;
        }

        return result;
    }

    /// <summary>
    /// 水回りの部屋を実際に配置
    /// </summary>
    /// <param name="areaToPlaceWetareas">水回りの部屋を配置する領域</param>
    /// <returns>1つの領域について水回りの部屋を配置した結果</returns>
    public List<Dictionary<string, Vector3[]>> PlaceWetareasActually(Dictionary<string, Vector3[]> currentPattern, Vector3[] currentSide, int[] wetareasPermutation, int[] rotationPattern) {
        //配置結果
        var result = new List<Dictionary<string, Vector3[]>>();
        result.Add(currentPattern); //現在の状態を追加

        //水回りの部屋について
        for (int i = 0; i < wetareasPermutation.Length; i++) {
            //現在配置しようとしている部屋の座標リスト
            List<Vector3[]> currentRoomCoodinatesList = GetWetareasRoomCoordinatesList(wetareasPermutation[i]);
            //現在配置しようとしている部屋の名前
            string currentRoomName = GetWetareasRoomName(wetareasPermutation[i]);
            //現在配置しようとしている部屋の座標
            Vector3[] currentRoomCoodinates = new Vector3[0];

            //配置しようとしている部屋がすでに配置されているとき
            if (currentPattern.ContainsKey(currentRoomName)) {
                //スキップ
                continue;
            }

            //結果を一時的に保存するリスト
            List<Dictionary<string, Vector3[]>> currentResultList = new List<Dictionary<string, Vector3[]>>();

            //全結果
            for (int j = 0; j < result.Count; j++) {

                //現在配置しようとしている部屋について
                for (int k = 0; k < currentRoomCoodinatesList.Count; k++) {
                    //現在の配置状態
                    Dictionary<string, Vector3[]> currentResult = new Dictionary<string, Vector3[]>(result[j]);

                    //配置する辺
                    Vector3[] sideToPlace = currentSide;                     
                    //配置されている部屋の長さ分，配置する辺を更新
                    for (int l = 0; l < currentResult.Count; l++) {
                        //配置する辺がなくなったとき
                        if (Vector3.Distance(sideToPlace[0], sideToPlace[1]) == 0f) {
                            //スキップ
                            break;
                        }
                        //配置する辺と部屋パーツが接しているとき
                        if (cf.ContactJudge(currentSide, currentResult.Values.ElementAt(l))) {
                            //接している部分を除く
                            sideToPlace = cf.SideSubstraction(sideToPlace, cf.ContactCoordinates(currentSide, currentResult.Values.ElementAt(l))[0])[0];
                        }
                    }

                    //配置したい部屋の座標を決定
                    currentRoomCoodinates = DecideWetareasRoomCoodinates(currentPattern, sideToPlace, currentRoomCoodinatesList[k], rotationPattern[i]);

                    //座標が空でないとき
                    if (currentRoomCoodinates.Length != 0) {
                        //座標を現在の配置状態に追加
                        currentResult.Add(currentRoomName, currentRoomCoodinates);
                        //結果を保存するリストに追加
                        currentResultList.Add(currentResult);
                    }
                }
            }

            //結果が空でないとき
            if (currentResultList.Count != 0) {
                //結果を更新
                result = currentResultList;
            }
        }

        return result;
    }

    /// <summary>
    /// 水回りの部屋を実際に配置
    /// </summary>
    /// <param name="areaToPlaceWetareas">水回りの部屋を配置する領域</param>
    /// <returns>1つの領域について水回りの部屋を配置した結果</returns>
    public Vector3[] DecideWetareasRoomCoodinates(Dictionary<string, Vector3[]> currentPattern, Vector3[] currentSide, Vector3[] currentRoomCoordinates, int rotationIndex) {
        //配置結果の座標
        Vector3[] result = new Vector3[0];

        //配置する辺が短すぎないとき
        if (Vector3.Distance(currentSide[0], currentSide[1]) >= 700f) {
            //配置する部屋の座標
            result = currentRoomCoordinates;

            //配置する辺がy軸に平行なとき
            if (cf.Slope(currentSide) == Mathf.Infinity) {
                //動かす方向
                int wetareasShiftDirection = cf.ShiftJudge(range, currentSide);

                //配置する部屋をデフォルトの向きに回転
                //部屋を辺の左側にするとき
                if (wetareasShiftDirection < 0) {
                    result = cf.Rotation(currentRoomCoordinates, 270);
                }
                //部屋を辺の右側にするとき
                else if (wetareasShiftDirection > 0) {
                    result = cf.Rotation(currentRoomCoordinates, 90);
                }

                //回転させるパターンのとき
                if (rotationIndex == 1) {
                    //配置する部屋が長方形のとき
                    if (cf.RectangleJudge(result)) {
                        //配置する部屋を回転
                        result = cf.Rotation(result, 90);
                    }
                }

                //配置する部屋の幅
                float currentRoomWidth = cf.CalculateRectangleWidth(result);
                //配置する部屋の高さ
                float currentRoomHeight = cf.CalculateRectangleHeight(result);

                //x座標の移動
                //部屋を辺の左側にするとき
                if (wetareasShiftDirection < 0) {
                    //x座標の移動
                    //y座標は一旦配置する辺の中点に配置
                    result = cf.CorrectCoordinates(result, new Vector3(currentSide[0].x - currentRoomWidth/2, (currentSide[0].y + currentSide[1].y)/2, 0));
                }
                //部屋を辺の右側にするとき
                else if (wetareasShiftDirection > 0) {
                    //x座標の移動
                    //y座標は一旦配置する辺の中点に配置
                    result = cf.CorrectCoordinates(result, new Vector3(currentSide[0].x + currentRoomWidth/2, (currentSide[0].y + currentSide[1].y)/2, 0));
                }

                //y座標の移動
                //玄関に近い方に配置
                Vector3 currentSidePointNearEntrance = currentSide[0];
                if (Vector3.Distance(currentSide[0], entrance[0]) > Vector3.Distance(currentSide[1], entrance[0])) {
                    currentSidePointNearEntrance = currentSide[1];
                }

                //y座標の中点のずれ（上下非対称の場合に意味あり）を求める
                //本当の高さの座標
                Vector3[] actHeightCoordinates = cf.ContactCoordinates(new Vector3[]{new Vector3(currentSide[0].x, currentSide[0].y - 20000, 0), new Vector3(currentSide[1].x, currentSide[1].y + 20000, 0)}, result)[0];
                //高さを更新
                currentRoomHeight = Vector3.Distance(actHeightCoordinates[0], actHeightCoordinates[1]);
                //y座標の中点のずれ
                float heightGap = (currentSide[0].y + currentSide[1].y)/2 - (actHeightCoordinates[0].y + actHeightCoordinates[1].y)/2;

                //玄関に近い方が上端のとき
                if (currentSidePointNearEntrance.y == Mathf.Max(currentSide[0].y, currentSide[1].y)) {
                    //y座標の移動
                    result = cf.CorrectCoordinates(result, new Vector3(0, currentSidePointNearEntrance.y - currentRoomHeight/2 + heightGap - (currentSide[0].y + currentSide[1].y)/2, 0));
                }
                //玄関に近い方が下端のとき
                else {
                    //y座標の移動
                    result = cf.CorrectCoordinates(result, new Vector3(0, currentSidePointNearEntrance.y + currentRoomHeight/2 + heightGap - (currentSide[0].y + currentSide[1].y)/2, 0));
                }
            }
            //配置する辺がx軸に平行なとき            
            else {
                //動かす方向
                int wetareasShiftDirection = cf.ShiftJudge(range, currentSide);

                //配置する部屋をデフォルトの向きに回転
                //部屋を辺の上側にするとき
                if (wetareasShiftDirection > 0) {
                    result = cf.Rotation(currentRoomCoordinates, 180);
                }
                //部屋を辺の下側にするときは回転させない

                //回転させるパターンのとき
                if (rotationIndex == 1) {
                    //配置する部屋が長方形のとき
                    if (cf.RectangleJudge(result)) {
                        //配置する部屋を回転
                        result = cf.Rotation(result, 90);
                    }
                }

                //配置する部屋の幅
                float currentRoomWidth = cf.CalculateRectangleWidth(result);
                //配置する部屋の高さ
                float currentRoomHeight = cf.CalculateRectangleHeight(result);

                //y座標の移動
                //部屋を辺の上側にするとき
                if (wetareasShiftDirection > 0) {
                    //y座標の移動
                    //x座標は一旦配置する辺の中点に配置
                    result = cf.CorrectCoordinates(result, new Vector3((currentSide[0].x + currentSide[1].x)/2, currentSide[0].y + currentRoomHeight/2, 0));
                }
                //部屋を辺の下側にするとき
                else if (wetareasShiftDirection < 0) {
                    //y座標の移動
                    result = cf.CorrectCoordinates(result, new Vector3((currentSide[0].x + currentSide[1].x)/2, currentSide[0].y - currentRoomHeight/2, 0));
                }

                //x座標の移動
                //玄関に近い方に配置
                Vector3 currentSidePointNearEntrance = currentSide[0];
                if (Vector3.Distance(currentSide[0], entrance[0]) > Vector3.Distance(currentSide[1], entrance[0])) {
                    currentSidePointNearEntrance = currentSide[1];
                }

                //x座標の中点のずれ（左右非対称の場合に意味あり）を求める
                //本当の幅の座標
                Vector3[] actWidthCoordinates = cf.ContactCoordinates(new Vector3[]{new Vector3(currentSide[0].x - 20000, currentSide[0].y, 0), new Vector3(currentSide[1].x + 20000, currentSide[1].y, 0)}, cf.VectorClean(result))[0];
                //幅の更新
                currentRoomWidth = Vector3.Distance(actWidthCoordinates[0], actWidthCoordinates[1]);
                //x座標の中点のずれ
                float widthGap = (currentSide[0].x + currentSide[1].x)/2 - (actWidthCoordinates[0].x + actWidthCoordinates[1].x)/2;

                //玄関に近い方が左端のとき
                if (currentSidePointNearEntrance.x == Mathf.Min(currentSide[0].x, currentSide[1].x)) {
                    //x座標の移動
                    result = cf.CorrectCoordinates(result, new Vector3(currentSidePointNearEntrance.x + currentRoomWidth/2 + widthGap - (currentSide[0].x + currentSide[1].x)/2 , 0, 0));
                }
                //玄関に近い方が右端のとき
                else {
                    //x座標の移動
                    result = cf.CorrectCoordinates(result, new Vector3(currentSidePointNearEntrance.x - currentRoomWidth/2 + widthGap - (currentSide[0].x + currentSide[1].x)/2, 0, 0));
                }
            }

            //部屋が配置範囲内にないとき
            if (!cf.JudgeInside(range, result)) {
                //座標を空にする
                result = new Vector3[0];
            }

            //部屋が他の部屋と重なっていないかの判定
            foreach (Vector3[] wetareasRoom in currentPattern.Values) {
                //配置した部屋が他の部屋の外側にないとき
                if (!cf.JudgeOutside(wetareasRoom, result)) {
                    //座標を空にする
                    result = new Vector3[0];
                    break;
                }
                //他の部屋が配置した部屋の外側にないとき
                if (!cf.JudgeOutside(result, wetareasRoom)) {
                    //座標を空にする
                    result = new Vector3[0];
                    break;
                }
            }
        }

        return result;
    }
    
    /// <summary>
    /// 番号に対応した水回りの部屋名を取得する
    /// </summary> 
    /// <param name="wetareasIndex">水回り部屋のインデックス</param>
    /// <returns>番号に対応した水回りの部屋名</returns>
    public string GetWetareasRoomName(int wetareasIndex) {
        string result = "";

        if (wetareasIndex == 0) {
            result = "UBandWashroom";
        } 
        // else if (wetareasIndex == 1) {
        //     result = "Washroom";
        // }
        else if (wetareasIndex == 1) {
            result = "Toilet";
        }
        else if (wetareasIndex == 2) {
            result = "Kitchen";
        }

        return result;
    }

    /// <summary>
    /// 番号に対応した水回りの部屋の座標リストを取得する
    /// </summary> 
    /// <param name="wetareasKindsIndex">水回り部屋のインデックス</param>
    /// <returns>番号に対応した水回りの部屋名</returns>
    public List<Vector3[]> GetWetareasRoomCoordinatesList(int wetareasKindsIndex) {
        List<Vector3[]> result = new List<Vector3[]>();

        if (wetareasKindsIndex == 0) {
            result = pa.ubAndWashroomCoordinatesList;
        } 
        // else if (wetareasKindsIndex == 1) {
        //     result = pa.washroomCoordinatesList;
        // }
        else if (wetareasKindsIndex == 1) {
            result = pa.toiletCoordinatesList;
        }
        else if (wetareasKindsIndex == 2) {
            result = pa.kitchenCoordinatesList;
        }

        return result;
    }

    /// <summary>
    /// キー（部屋名）の差分を返す
    /// </summary> 
    /// <param name="addDictionary">追加された辞書</param>
    /// <param name="originalDictionary">追加前の辞書</param>
    /// <param name="wetAreasPermutation">水回りの部屋の順番</param>
    /// <returns>キー（部屋名）の差分の配列</returns>
    public string[] KeysDifference(Dictionary<string, Vector3[]> addDictionary, Dictionary<string, Vector3[]> originalDictionary, int[] wetAreasPermutation) {
        List<string> result = new List<string>();

        for (int i = 0; i < wetAreasPermutation.Length; i++) {
            string key = GetWetareasRoomName(wetAreasPermutation[i]);

            //追加された辞書にキーが含まれていて，追加前の辞書にキーが含まれていないとき
            if (addDictionary.ContainsKey(key) && !originalDictionary.ContainsKey(key)) {
                //そのキーを追加
                result.Add(key);
            }
        }

        //リストを配列に変換して返す
        return result.ToArray();
    }

    /// <summary>
    /// 辞書からキーを除いたものを返す
    /// </summary> 
    /// <param name="dictionary">辞書</param>
    /// <param name="keys">除きたいキーの配列</param>
    /// <returns>キーを除いた辞書</returns>
    public Dictionary<string, Vector3[]> RemoveDictionaryElement(Dictionary<string, Vector3[]> dictionary, string[] keys) {
        Dictionary<string, Vector3[]> result = new Dictionary<string, Vector3[]>(dictionary);

        //1つずつキーを除く
        for (int i = 0; i < keys.Length; i++) {
            result.Remove(keys[i]);
        }

        return result;
    }

    /// <summary>
    /// 部屋の開けておなかなければならない辺が空いているかどうかを判定
    /// </summary> 
    /// <param name="checkRoom">調べる部屋</param>
    /// <param name="surroundingRooms">周りの部屋</param>
    /// <returns>部屋の開けておなかなければならない辺が空いているかどうか</returns>
    public bool SecureNecessarySide(string checkRoomName, Dictionary<string, Vector3[]> surroundingRooms) {
        //判定結果
        bool flag = true;

        //調べる部屋を1辺ずつに分割
        List<Vector3[]> checkRoomSides = new List<Vector3[]>(); //調べる部屋の辺のリスト
        Vector3[] checkRoomCoordinates = surroundingRooms[checkRoomName]; //調べる部屋の座標
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
                if (cf.ContactJudge(checkRoomSide, checkRangeSide)) {
                    //水回り範囲との共通部分を除いた辺が存在しないとき
                    if (cf.GetLength(cf.SideSubstraction(checkRoomSide, checkRangeSide)[0]) == 0.0f) {
                        //その辺を削除
                        checkRoomSides.RemoveAt(j);
                        j--;
                    }
                    else {
                        //共通部分を除いた辺に更新
                        checkRoomSides[j] = cf.SideSubstraction(checkRoomSide, checkRangeSide)[0];
                    }
                }
            }
        }

        //周りの部屋について調べる
        foreach (KeyValuePair<string, Vector3[]> surroundingRoom in surroundingRooms) {
            //調べる部屋と周りの部屋が同じ場合
            if (surroundingRoom.Key == checkRoomName) {
                //スキップ
                continue;
            }

            //調べる部屋と周りの部屋が接しているとき
            if (cf.ContactJudge(surroundingRoom.Value, checkRoomCoordinates)) {
                //周りの部屋との共通部分につい調べる
                //周りの部屋の各辺について
                for (int i = 0; i < surroundingRoom.Value.Length; i++) {
                    Vector3[] surroundingRoomSide = new Vector3[]{surroundingRoom.Value[i], surroundingRoom.Value[(i+1)%surroundingRoom.Value.Length]};

                    //調べる部屋の各辺について
                    for (int j = 0; j < checkRoomSides.Count; j++) {
                        Vector3[] checkRoomSide = checkRoomSides[j];

                        //2辺が接しているとき
                        if (cf.ContactJudge(checkRoomSide, surroundingRoomSide)) {
                            //周りの部屋との共通部分を除いた辺が存在しないとき
                            if (cf.GetLength(cf.SideSubstraction(checkRoomSide, surroundingRoomSide)[0]) == 0.0f) {
                                //その辺を削除
                                checkRoomSides.RemoveAt(j);
                                j--;
                            }
                            else {
                                //共通部分を除いた辺に更新
                                checkRoomSides[j] = cf.SideSubstraction(checkRoomSide, surroundingRoomSide)[0];
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
            float checkRoomSideMax = Vector3.Distance(checkRoomSides[0][0], checkRoomSides[0][1]);
            for (int i = 0; i < checkRoomSides.Count; i++) {
                if (checkRoomSideMax < Vector3.Distance(checkRoomSides[i][0], checkRoomSides[i][1])) {
                    checkRoomSideMax = Vector3.Distance(checkRoomSides[i][0], checkRoomSides[i][1]);
                }
            }

            // if (checkRoomName == "Washroom") {
            //     //洗面室の場合
            //     if (checkRoomSideMax < 900.0f) {
            //         flag = false;
            //     }
            // }
            if (checkRoomName == "Toilet") {
                //トイレの場合
                if (checkRoomSideMax < 800.0f) {
                    flag = false;
                }
            }
            else if (checkRoomName == "Kitchen") {
                //キッチンの場合
                if (checkRoomSideMax < 2400.0f) {
                    flag = false;
                }
            }
        }
        //残った辺が存在しないとき
        else {
            if (checkRoomName == "Washroom" || checkRoomName == "Toilet" || checkRoomName == "Kitchen") {
                flag = false;
            }
        }

        return flag;
    }

    /// <summary>
    /// 多角形の辺上の2点を通るように切る
    /// </summary> 
    /// <param name="polygon">多角形の座標配列</param>
    /// <param name="slicePoints">切る2点の座標配列</param>
    /// <returns>切った2つの多角形の座標配列のリスト</returns>
    public List<Vector3[]> Slice(Vector3[] polygon, Vector3[] slicePoints) {
        //返すリスト
        List<Vector3[]> slicePolygons = new List<Vector3[]>();

        //多角形に切りたい点を追加
        Vector3[] polygonAddedPoints = polygon;
        polygonAddedPoints = AddPoint(polygonAddedPoints, slicePoints[0]);
        polygonAddedPoints = AddPoint(polygonAddedPoints, slicePoints[1]);

        /* 切る */
        //切る処理のためにリストに変換(removeを使うため複数用意)
        List<Vector3> polygonAddedPointsListA = polygonAddedPoints.ToList();
        List<Vector3> polygonAddedPointsListB = polygonAddedPoints.ToList();

        //切る点が含まれるインデックスを求める
        int[] slicePointsIndex = new int[] {polygonAddedPointsListA.IndexOf(slicePoints[0]), polygonAddedPointsListA.IndexOf(slicePoints[1])};
        //昇順にソート
        Array.Sort(slicePointsIndex);

        //切る点のインデックスの間を削除
        polygonAddedPointsListA.RemoveRange(slicePointsIndex[0] + 1, slicePointsIndex[1] - slicePointsIndex[0] - 1);
        //切る点のインデックスの外側を削除
        polygonAddedPointsListB.RemoveRange(slicePointsIndex[1] + 1, polygonAddedPointsListB.Count - slicePointsIndex[1] - 1); //まず後ろ側を削除(インデックスが変わってしまうため)
        polygonAddedPointsListB.RemoveRange(0, slicePointsIndex[0]); //前側を削除

        //返すリストを作成
        slicePolygons.Add(polygonAddedPointsListA.ToArray());
        slicePolygons.Add(polygonAddedPointsListB.ToArray());

        return slicePolygons;
    }

    /// <summary>
    /// 多角形の辺上の点を座標配列に追加
    /// </summary> 
    /// <param name="polygon">多角形の座標配列</param>
    /// <param name="point">追加したい点</param>
    /// <returns>点を追加した座標配列</returns>
    public Vector3[] AddPoint(Vector3[] polygon, Vector3 point) {
        //返すリスト
        List<Vector3> polygonAddedPoint = polygon.ToList();

        if (polygon.Contains(point)) {
            return polygonAddedPoint.ToArray();
        }

        for (int i = 0; i < polygon.Length; i++) {
            if (cf.OnLineSegment(new Vector3[]{polygon[i], polygon[(i+1)%polygon.Length]}, point)) {
                polygonAddedPoint.Insert(i+1, point);

                return polygonAddedPoint.ToArray();
            }
        }

        return polygonAddedPoint.ToArray();
    }

    /// <summary>
    /// 外側の部屋の座標から接している内側の部屋を抜き取る
    /// </summary> 
    /// <param name="outside">外側の部屋の座標</param>
    /// <param name="inside">内側の辺の座標</param>
    /// <returns>外側の部屋の座標から接している内側の部屋を抜き取った座標配列</returns>
    public Vector3[] FrameChange(Vector3[] outside, Vector3[] inside) {
        List<Vector3> newOuter = new List<Vector3>(); //返すリスト
        Vector3 startCoordinates = new Vector3();
        bool startFlag = true;
        Vector3 endCoordinates = new Vector3();
        bool endFlag = true;

        //外側の部屋の必要な座標を追加
        //外側の部屋の辺をひとつずつ確認
        for (int i = 0; i < outside.Length; i++) {
            //Vector3[] contactCoordinates = cf.ContactCoordinates(new Vector3[]{outside[i], outside[(i+1)%outside.Length]}, inside)[0];

            //内側の部屋と接していない場合
            if (!cf.ContactJudge(new Vector3[]{outside[i], outside[(i+1)%outside.Length]}, inside)) {
                newOuter.Add(outside[i]);
            }
            //内側の部屋と接している場合
            else {
                //接している辺の組み合わせを探す
                for (int j = 0; j < inside.Length; j++) {
                    //contactCoordinates = cf.ContactCoordinates(new Vector3[]{outside[i], outside[(i+1)%outside.Length]}, new Vector3[]{inside[j], inside[(j+1)%inside.Length]})[0];

                    //接していない辺の組み合わせの場合
                    if (!cf.ContactJudge(new Vector3[]{outside[i], outside[(i+1)%outside.Length]}, new Vector3[]{inside[j], inside[(j+1)%inside.Length]})) {
                        continue;
                    }

                    //外側と内側の辺のリスト上で先に来る座標についての処理
                    //内側の辺の頂点が外側の辺上にあり，外側の辺と内側の辺の頂点が同じでない場合
                    if (cf.OnLineSegment(new Vector3[]{outside[i], outside[(i+1)%outside.Length]}, inside[j]) && (outside[i] != inside[j])) {
                        //外側の辺の頂点を追加
                        newOuter.Add(outside[i]);

                        //内側の辺の頂点を追加
                        newOuter.Add(inside[j]);
                        //切れ目の始点に設定
                        if (startFlag) {
                            startCoordinates = inside[j];
                            startFlag = false;
                        }
                    }
                    //内側の辺の頂点が外側の辺辺上にない場合
                    else if (!cf.OnLineSegment(new Vector3[]{outside[i], outside[(i+1)%outside.Length]}, inside[j])) {
                        //外側の辺の頂点を追加
                        newOuter.Add(outside[i]);

                        //切れ目の始点に設定
                        if (startFlag) {
                            startCoordinates = outside[i];
                            startFlag = false;
                        }
                    }

                    //外側と内側の辺のリスト上で後に来る座標についての処理
                    //内側の辺の頂点が外側の辺上にあり，外側の辺と内側の辺の頂点が同じでない場合
                    if (cf.OnLineSegment(new Vector3[]{outside[i], outside[(i+1)%outside.Length]}, inside[(j+1)%inside.Length]) && (outside[(i+1)%outside.Length] != inside[(j+1)%inside.Length])) {
                        //内側の辺の頂点を追加
                        newOuter.Add(inside[(j+1)%inside.Length]);
                        //切れ目の終点に設定
                        if (endFlag) {
                            endCoordinates = inside[(j+1)%inside.Length];
                            endFlag = false;
                        }
                    }
                    //外側の辺と内側の辺の頂点が同じ場合
                    else if (!cf.OnLineSegment(new Vector3[]{outside[i], outside[(i+1)%outside.Length]}, inside[(j+1)%inside.Length]) || (outside[(i+1)%outside.Length] == inside[(j+1)%inside.Length])) {
                        //切れ目の終点に設定
                        if (endFlag) {
                            endCoordinates = outside[(i+1)%outside.Length];
                        }
                    }

                    break;
                }
            }
        }

        //内側の部屋の必要な座標を追加
        List<Vector3> needInside = new List<Vector3>();

        //内側の部屋の必要な座標を追加
        //内側の部屋の辺をひとつずつ確認
        for (int i = 0; i < inside.Length; i++) {
            //Vector3[] contactCoordinates = cf.ContactCoordinates(new Vector3[]{inside[i], inside[(i+1)%inside.Length]}, outside)[0];

            //外側の部屋と接していない場合
            if (!cf.ContactJudge(new Vector3[]{inside[i], inside[(i+1)%inside.Length]}, outside)) {
                //リストになければ内側の辺の頂点を追加
                if (!newOuter.Contains(inside[i])) {
                    if (!needInside.Contains(inside[i])) {
                        needInside.Add(inside[i]);
                    }
                }
            }
            //外側の部屋と接している場合
            else {
                //接している辺の組み合わせを探す
                for (int j = 0; j < outside.Length; j++) {
                    //contactCoordinates = cf.ContactCoordinates(new Vector3[]{inside[i], inside[(i+1)%inside.Length]}, new Vector3[]{outside[j], outside[(j+1)%outside.Length]})[0];

                    //接していない辺の組み合わせの場合
                    if (!cf.ContactJudge(new Vector3[]{inside[i], inside[(i+1)%inside.Length]}, new Vector3[]{outside[j], outside[(j+1)%outside.Length]})) {
                        continue;
                    }
                    
                    //内側の辺の先に来る端点が外側の辺の範囲外にある場合
                    if (Vector3.Distance(outside[j], inside[i]) + Vector3.Distance(inside[i], outside[(j+1)%outside.Length]) > Vector3.Distance(outside[j], outside[(j+1)%outside.Length])) {
                        //その端点を追加
                        needInside.Add(inside[i]);
                    }

                    //内側の辺の後に来る端点が外側の辺の範囲外にある場合
                    if (Vector3.Distance(outside[j], inside[(i+1)%inside.Length]) + Vector3.Distance(inside[(i+1)%inside.Length], outside[(j+1)%outside.Length]) > Vector3.Distance(outside[j], outside[(j+1)%outside.Length])) {
                        //その端点を追加
                        needInside.Add(inside[(i+1)%inside.Length]);
                    }

                    break;
                }
            }
        }

        //内側の必要な頂点が2つで，元の内側の部屋の始点と終点の場合のみ順番を入れ替えない
        if (!((needInside.Count == 2) && ((Array.IndexOf(inside, needInside[0]) == 0) && (Array.IndexOf(inside, needInside[1]) == inside.Length - 1)))) {
            needInside.Reverse();
        }

        //外側の頂点と内側の頂点をくっつける
        if (needInside.Count != 0) {
            //endCoodinatesのミスを無理やり修正
            //newOuterに斜めの辺が含まれるとき
            for (int i = 0; i < newOuter.Count; i++) {
                //newOuterの辺
                Vector3[] newOuterSide = new Vector3[]{newOuter[i], newOuter[(i+1)%newOuter.Count]};
                if (cf.Slope(newOuterSide) != 0.0f && cf.Slope(newOuterSide) != Mathf.Infinity) {
                    if (newOuter[i] == startCoordinates) {
                        endCoordinates = newOuter[(i+1)%newOuter.Count];
                    }
                    else if (newOuter[(i+1)%newOuter.Count] == startCoordinates) {
                        endCoordinates = newOuter[i];
                    }
                    break;
                }
            }

            int outside_end_index = newOuter.IndexOf(endCoordinates);
            newOuter.InsertRange(outside_end_index, needInside);
        }

        //要らない座標（頂点を含まない辺上にある座標）を除く
        for (int i = 0; i < newOuter.Count; i++) {
            //調べる辺
            Vector3[] sideA = new Vector3[]{newOuter[(i+1)%newOuter.Count], newOuter[i]};
            Vector3[] sideB = new Vector3[]{newOuter[(i+1)%newOuter.Count], newOuter[(i+2)%newOuter.Count]};

            //座標配列で連続する2辺の内積を計算
            if (Vector3.Dot(sideA[1] - sideA[0], sideB[1] - sideB[0]) == -Vector3.Distance(sideA[1], sideA[0]) * Vector3.Distance(sideB[1], sideB[0])) {
                //座標配列から削除
                newOuter.Remove(sideA[0]);
                i = -1;
            }
        }


        return newOuter.ToArray();
    }

    /// <summary>
    /// 多角形の辺の長い順のインデックスを求める
    /// </summary> 
    /// <param name="polygon">多角形の座標</param>
    /// <returns>多角形の辺の長い順のインデックスの配列</returns>
    public int[] LongIndex(Vector3[] polygon) {
        //結果
        int[] result = new int[polygon.Length];

        //辺のインデックスと長さの辞書を作成
        var length = new Dictionary<int, float>();
        for (int i = 0; i < polygon.Length; i++) {
            length.Add(i, Vector3.Distance(polygon[i], polygon[(i+1)%polygon.Length]));
        }

        //辺の長さの降順にインデックスの配列を作成
        for (int i = 0; i < result.Length; i++) {
            result[i] = length.FirstOrDefault(x => x.Value == length.Values.Max()).Key;
            length.Remove(result[i]);
        }

        return result;
    }
}
