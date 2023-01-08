using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// 穴掘り法による迷路の自動生成に関するプログラム
public class MakeMaze : MonoBehaviour
{
    /// <summary>
    /// 1つ先のx,y座標を計算
    /// </summary>
    /// <value>down,left,up,right</value>
    public static readonly Vector2Int[] Positions = {
        new Vector2Int (0, -1),
        new Vector2Int (-1, 0),
        new Vector2Int (0, 1),
        new Vector2Int (1, 0),
    };

    //2つ先のx,y座標を仮計算
    private static readonly Vector2Int[] Positions2 = {
        new Vector2Int (0, -2),
        new Vector2Int (-2, 0),
        new Vector2Int (0, 2),
        new Vector2Int (2, 0),
    };

    #region   // パラメータ設定
    /// <summary>
    /// Mapの縦横のサイズ
    /// </summary>
    public int max = 5;  //必ず奇数にすること
    [SerializeField] private int moveMap = 0; //ゴール地点までに穴掘り法を行った回数
    [SerializeField] private bool isGoalSet;  // 穴掘り法のルートが30以上か否か
    /// <summary>
    /// ゴールに必要な犬の数
    /// </summary>
    public int goalCondition;

    [Header("オブジェクト設定")]
    [SerializeField] private GameObject wall;    //壁用オブジェクト
    [SerializeField] private GameObject floor;   //床用オブジェクト
    [SerializeField] private GameObject start = null;   //スタート地点に配置するオブジェクト
    [SerializeField] private GameObject goal = null;    //ゴール地点に配置するオブジェクト
    [SerializeField] private GameObject player;    //プレイヤーオブジェクト
    [SerializeField] private GameObject warpIn;  // ワープに入る親オブジェクト
    [SerializeField] private GameObject warpOut;  // ワープから出る親オブジェクト
    [SerializeField] private GameObject goalFence;  // ゴール前フェンスオブジェクト
    [SerializeField] private GameObject stayDog;  // 止まっている犬オブジェクト
    [SerializeField] private GameObject switchObj;  // スイッチオブジェクト
    [SerializeField] private GameObject dogFence;  // 止まっている犬のフェンスオブジェクト
    [SerializeField] private GameObject walkDog;  // 歩く犬オブジェクト
    [SerializeField] private GameObject goldBar;  // 金の延べ棒オブジェクト
    private GameObject[] warpInChild;    //ワープに入る各オブジェクト
    private GameObject[] warpOutChild;    //ワープから出る各オブジェクト
    private Stack<Vector2Int> startCells;   // 穴掘り開始候補座標スタック
    [Header("各候補座標")]
    /// <summary>
    /// 各仕掛け配置候補座標リスト
    /// </summary>
    public List<Vector2Int> itemCells;
    [SerializeField] private List<Vector2Int> noPassCells;   // 行き詰まり座標リスト
    [SerializeField] private List<Vector2Int> stayDogCells = new List<Vector2Int>();   // 止まっている犬の配置候補座標リスト
    /// <summary>
    /// 歩く犬のスタート配置セル
    /// </summary>
    public Vector2Int walkDogStartCell;
    // 内部パラメータ
    /// <summary>
    /// セルの種類
    /// </summary>
    public enum CellType {Wall, Path};
    public CellType[,] cells;
    private Vector2Int startPos;    //穴掘り法のスタートの座標
    private Vector2Int goalPos;     //穴掘り法のゴールの座標
    #endregion

    private void Start ()
    {
        // WarpInの子オブジェクトを取得しwarpInChildに代入
        warpInChild = new GameObject[warpIn.transform.childCount];
        for(int i=0; i<warpIn.transform.childCount; i++)
        {
            warpInChild[i] = warpIn.transform.GetChild(i).gameObject;
        }

        // WarpOutの子オブジェクトを取得しwarpOutChildに代入
        warpOutChild = new GameObject[warpOut.transform.childCount];
        for(int i=0; i<warpOut.transform.childCount; i++)
        {
            warpOutChild[i] = warpOut.transform.GetChild(i).gameObject;
        }

        // ルートが30未満か滞在する犬が配置できない時、while文を継続
        while(!isGoalSet || stayDogCells.Count == 0)
        {
            //マップ状態初期化
            cells = new CellType[max, max];
            startCells = new Stack<Vector2Int>();
            itemCells = new List<Vector2Int>();
            noPassCells = new List<Vector2Int>();
            stayDogCells = new List<Vector2Int>();
            isGoalSet = false;

            //スタート地点の取得
            startPos = new Vector2Int(0, 0);
            startCells.Push(startPos);

            //通路の生成
            //初回はゴール地点を設定する
            goalPos = MakeGoalInfo(startPos);

            //通路生成を繰り返して袋小路を減らす
            while(startCells.Count > 0)
            {
                // 穴掘り開始座標を入れたスタックの一番上を読み取り、その座標から穴掘り法を行う
                var tmpStart = startCells.Peek();
                MakeMapInfo(tmpStart);
            }

            //行き詰まりの場所をリストに入れる
            StalemateCheck(itemCells);

            //行き詰まりから4マス連続閉じ込められている場所をリストに入れる
            FourSquareCheck(noPassCells);
        }

        //マップの状態に応じて壁と通路を生成する
        BuildDungeon();

        //各地点にオブジェクトを配置する
        ObjectArrange();
    }

    // ゴール生成を行う穴掘り法
    private Vector2Int MakeGoalInfo(Vector2Int _startPos)
    {
        //スタート位置配列を複製
        var tmpStartPos = _startPos;
        moveMap = 0;

        //移動可能な座標のリストを取得
        var movablePositions = GetMovablePositions(tmpStartPos);

        //移動可能な座標がなくなるまで探索を繰り返す
        while (movablePositions != null)
        {
            // 指定座標を通路とし穴掘り候補座標から削除
            SetPath(tmpStartPos.x, tmpStartPos.y);

            //移動可能な座標からランダムで1つ取得し通路にする
            var tmpPos = movablePositions[Random.Range(0, movablePositions.Count)];
            SetPath(tmpPos.x, tmpPos.y);

            //元の地点と通路にした座標の間を通路にし、穴掘り法を行った回数の変数moveMapを増やす
            var xPos = tmpPos.x + (tmpStartPos.x - tmpPos.x) / 2;
            var yPos = tmpPos.y + (tmpStartPos.y - tmpPos.y) / 2;
            SetPath(xPos, yPos);
            moveMap++;

            //移動後の座標を一時変数に格納し、再度移動可能な座標を探索する
            tmpStartPos = tmpPos;
            movablePositions = GetMovablePositions(tmpStartPos);
        }

        // 移動可能な座標がなくなれば、そのセルから穴掘り法ができないためstartCellsから除外する
        if(movablePositions == null)
        {
            startCells.Pop();
        }

        // 穴掘り法を行った回数が30回以上であればisGoalSetをtrueにする
        if(moveMap > 30)
        {
            isGoalSet = true;
        }

        //探索終了時の座標を返す
        return tmpStartPos;
    }

    // 袋小路を減らす穴掘り法
    private Vector2Int MakeMapInfo(Vector2Int _startPos)
    {
        //スタート位置配列を複製
        var tmpStartPos = _startPos;

        //移動可能な座標のリストを取得
        var movablePositions = GetMovablePositions(tmpStartPos);

        //移動可能な座標がなくなるまで探索を繰り返す
        while (movablePositions != null)
        {
            // 指定座標を通路とし穴掘り候補座標から削除
            SetPath(tmpStartPos.x, tmpStartPos.y);

            //移動可能な座標からランダムで1つ取得し通路にし、itemCellsのリストに入れる
            var tmpPos = movablePositions[Random.Range(0, movablePositions.Count)];
            SetPath(tmpPos.x, tmpPos.y);
            itemCells.Add(new Vector2Int(tmpPos.x, tmpPos.y));

            //元の地点と通路にした座標の間を通路にし、itemCellsのリストに入れる
            var xPos = tmpPos.x + (tmpStartPos.x - tmpPos.x) / 2;
            var yPos = tmpPos.y + (tmpStartPos.y - tmpPos.y) / 2;
            SetPath(xPos, yPos);
            itemCells.Add(new Vector2Int(xPos, yPos));

            //移動後の座標を一時変数に格納し、再度移動可能な座標を探索する
            tmpStartPos = tmpPos;
            movablePositions = GetMovablePositions(tmpStartPos);
        }

        // 移動可能な座標がなくなれば、そのセルから穴掘り法ができないためstartCellsから除外する
        if(movablePositions == null)
        {
            startCells.Pop();
        }
        
        //探索終了時の座標を返す
        return tmpStartPos;
    }

    // 移動可能な座標のリストを取得する
    private List<Vector2Int> GetMovablePositions(Vector2Int _startPos)
    {
        //前後左右毎に２つ先の座標が範囲内かつ壁であるかを判定する
        //真であれば、返却用リストに追加する
        var movablePositions = new List<Vector2Int>();  // 移動可能な座標を入れたリスト

        foreach(var position in Positions2)
        {
            var tmppos = (_startPos + position);
            if (!IsOutOfBounds(tmppos.x, tmppos.y) && cells[tmppos.x, tmppos.y] == CellType.Wall)
                movablePositions.Add(new Vector2Int(tmppos.x, tmppos.y));
        }

        return movablePositions.Count () != 0 ? movablePositions.ToList () : null;
    }

    // 座標を通路とする(穴掘り開始座標候補の場合は保持)
    private void SetPath(int x, int y)
    {
        cells[x, y] = CellType.Path;
        if (x % 2 == 0 && y % 2 == 0)
        {
            // 穴掘り候補座標であれば、startCellsに入れる
            startCells.Push(new Vector2Int(x, y));
        }
    }

    /// <summary>
    /// 与えられたx、y座標が範囲外の場合真を返す
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool IsOutOfBounds( int x, int y ) => ( x < 0 || y < 0 || x >= max || y >= max );

    //パラメータに応じて迷路のオブジェクトを生成する
    private void BuildDungeon()
    {
        //縦横1マスずつ大きくループを回し、外壁とする
        for(int j= -1; j<= max; j++)
        {
            for(int i= -1; i<=max; i++)
            {
                //範囲外、または壁の場合に壁オブジェクトを生成する
                if (IsOutOfBounds(i, j) || cells[i, j] == CellType.Wall)
                {
                    var wallObj = Instantiate(wall, new Vector3(i * 40 + wall.transform.position.x, 0, j * 40 + wall.transform.position.z), Quaternion.identity);
                    wallObj.transform.parent = this.transform;
                }
                else
                {
                    //通路の場所に床オブジェクトを生成
                    var floorObj = Instantiate(floor, new Vector3(i * 40, 0, j * 40), Quaternion.identity);
                    floorObj.transform.parent = this.transform;
                }
            }
        }
    }

    //ステージの難易度や決められた迷路に応じてオブジェクトを配置する
    private void ObjectArrange()
    {
        // スタート地点とゴール地点にオブジェクトを配置する
        // 初回で取得したスタート地点とゴール地点は必ずつながっているので破綻しない

        // 向く方向と向く先の隣の座標を取得
        var (startdirection, startNextPos) = GetMovableDirection(startPos);
        var (goaldirection, goalNextPos) = GetMovableDirection(goalPos);
        var startObj = Instantiate(start, new Vector3(startPos.x * 40, 0.01f, startPos.y * 40), Quaternion.Euler(0, 90 * startdirection, 0));
        GameObject goalObj = null;

        //向く方向に応じて、ゴールを回転させゴールオブジェクトをcellの中心になるように配置する
        switch(goaldirection)
        {
            case 0:
                goalObj = Instantiate(goal, new Vector3(goalPos.x * 40 + goal.transform.position.x, 0, goalPos.y * 40 + goal.transform.position.z), Quaternion.Euler(0, 90 * goaldirection, 0));
                break;
            case 1:
                goalObj = Instantiate(goal, new Vector3(goalPos.x * 40 + goal.transform.position.z, 0, goalPos.y * 40 - goal.transform.position.x), Quaternion.Euler(0, 90 * goaldirection, 0));
                break;
            case 2:
                goalObj = Instantiate(goal, new Vector3(goalPos.x * 40 - goal.transform.position.x, 0, goalPos.y * 40 - goal.transform.position.z), Quaternion.Euler(0, 90 * goaldirection, 0));
                break;
            case 3:
                goalObj = Instantiate(goal, new Vector3(goalPos.x * 40 - goal.transform.position.z, 0, goalPos.y * 40 + goal.transform.position.x), Quaternion.Euler(0, 90 * goaldirection, 0));
                break;
        }

        // スタートとゴールのオブジェクトを迷路生成オブジェクトの子オブジェクトにする
        startObj.transform.parent = this.transform;
        goalObj.transform.parent = this.transform;

        //プレイヤーをスタート地点に配置し、子オブジェクトにする
        player.transform.position = startObj.transform.position;

        //ステージ難易度に応じてオブジェクトの出現を調整
        var activeRandom = new List<GameObject>();  // ランダムな場所に配置されるオブジェクトリスト
        switch(NonGameCanvasCtrl.Instance.stageNo)
        {
            #region // stageNo = 1
            case 1:
                // ワープのみ出現させ、activeRandomリストに追加
                warpIn.SetActive(true);
                warpOut.SetActive(true);
                foreach(var warpInObj in warpInChild)
                {
                    activeRandom.Add(warpInObj);
                }
                foreach(var warpOutObj in warpOutChild)
                {
                    activeRandom.Add(warpOutObj);
                }
                break;
            #endregion
            #region // stageNo = 2
            case 2:
                // ワープとゴール前フェンスを出現させ、ワープをactiveRandomリストに追加
                warpIn.SetActive(true);
                warpOut.SetActive(true);
                goalFence.SetActive(true);
                foreach(var warpInObj in warpInChild)
                {
                    activeRandom.Add(warpInObj);
                }
                foreach(var warpOutObj in warpOutChild)
                {
                    activeRandom.Add(warpOutObj);
                }
                // ゴール前フェンスをゴール前の座標に配置し、ゴールと同じ向きにする
                goalFence.transform.position = new Vector3(goalNextPos.x * 40, 10, goalNextPos.y * 40);
                goalFence.transform.rotation = goalObj.transform.rotation;
                break;
            #endregion
            #region // stageNo = 3
            case 3:
                // ゴールに必要な犬の数
                goalCondition = 1;
                // ワープとゴール前フェンスと止まる犬を出現
                warpIn.SetActive(true);
                warpOut.SetActive(true);
                goalFence.SetActive(true);
                stayDog.SetActive(true);
                // ワープの入口をactiveRandomリストに追加
                foreach(var warpInObj in warpInChild)
                {
                    activeRandom.Add(warpInObj);
                }
                // 止まる犬に対応した出口を除いたワープの出口をactiveRandomリストに追加
                for(int i=0; i<warpOutChild.Length-1; i++)
                {
                    activeRandom.Add(warpOutChild[i]);
                }
                // ゴール前フェンスをゴール前の座標に配置し、ゴールと同じ向きにする
                goalFence.transform.position = new Vector3(goalNextPos.x * 40, 10, goalNextPos.y * 40);
                goalFence.transform.rotation = goalObj.transform.rotation;
                // stayDogCellsのリストから１つ場所を取得し、その隣の道の座標と向きを取得
                var warpPos = SetObjectPosition(stayDogCells);
                var (warpdirection, warpNextPos) = GetMovableDirection(warpPos);
                // 止まる犬に対応したワープの出口を配置
                warpOut.transform.GetChild(10).transform.position = new Vector3(warpPos.x * 40, 0, warpPos.y * 40);
                // 止まる犬を方向に応じて配置
                switch(warpdirection)
                {
                    case 0:
                        stayDog.transform.position = new Vector3(warpNextPos.x * 40, 0, (warpNextPos.y - 1) * 40);
                        // 止まる犬を配置した場所を他の仕掛けと被らないようitemCellsから除外
                        itemCells.Remove(new Vector2Int(warpNextPos.x, warpNextPos.y - 1));
                        break;
                    case 1:
                        stayDog.transform.position = new Vector3((warpNextPos.x - 1) * 40, 0, warpNextPos.y * 40);
                        itemCells.Remove(new Vector2Int(warpNextPos.x - 1, warpNextPos.y));
                        break;
                    case 2:
                        stayDog.transform.position = new Vector3(warpNextPos.x * 40, 0, (warpNextPos.y + 1) * 40);
                        itemCells.Remove(new Vector2Int(warpNextPos.x, warpNextPos.y + 1));
                        break;
                    case 3:
                        stayDog.transform.position = new Vector3((warpNextPos.x + 1) * 40, 0, warpNextPos.y * 40);
                        itemCells.Remove(new Vector2Int(warpNextPos.x + 1, warpNextPos.y));
                        break;
                }
                // 止まる犬を方向に応じて向きを変更
                stayDog.transform.rotation = Quaternion.Euler(0, 90 * (warpdirection+2), 0);
                // 止まる犬に対応したワープの出口の場所をitemCellsから除外
                itemCells.Remove(warpPos);
                break;
            #endregion
            #region // stageNo = 4
            case 4:
                // ゴールに必要な犬の数
                goalCondition = 1;
                // ワープとゴール前フェンスと止まる犬と犬フェンスとスイッチを出現
                warpIn.SetActive(true);
                warpOut.SetActive(true);
                goalFence.SetActive(true);
                stayDog.SetActive(true);
                switchObj.SetActive(true);
                dogFence.SetActive(true);
                // ワープの入口をactiveRandomリストに追加
                foreach(var warpInObj in warpInChild)
                {
                    activeRandom.Add(warpInObj);
                }
                // 止まる犬に対応した出口を除いたワープの出口をactiveRandomリストに追加
                for(int i=0; i<warpOutChild.Length-1; i++)
                {
                    activeRandom.Add(warpOutChild[i]);
                }
                // ゴール前フェンスをゴール前の座標に配置し、ゴールと同じ向きにする
                goalFence.transform.position = new Vector3(goalNextPos.x * 40, 10, goalNextPos.y * 40);
                goalFence.transform.rotation = goalObj.transform.rotation;
                // stayDogCellsのリストから１つ場所を取得し、その隣の道の座標と向きを取得
                warpPos = SetObjectPosition(stayDogCells);
                (warpdirection, warpNextPos) = GetMovableDirection(warpPos);
                // 止まる犬に対応したワープの出口を配置
                warpOut.transform.GetChild(10).transform.position = new Vector3(warpPos.x * 40, 0, warpPos.y * 40);
                // 止まる犬と犬のフェンスを方向に応じて配置
                switch(warpdirection)
                {
                    case 0:
                        stayDog.transform.position = new Vector3(warpNextPos.x * 40, 0, (warpNextPos.y - 1) * 40);
                        dogFence.transform.position = new Vector3(warpNextPos.x * 40, 10, (warpNextPos.y - 2) * 40);
                        // 止まる犬と犬のフェンスを配置した場所を他の仕掛けと被らないようitemCellsから除外
                        itemCells.Remove(new Vector2Int(warpNextPos.x, warpNextPos.y - 1));
                        itemCells.Remove(new Vector2Int(warpNextPos.x, warpNextPos.y - 2));
                        break;
                    case 1:
                        stayDog.transform.position = new Vector3((warpNextPos.x - 1) * 40, 0, warpNextPos.y * 40);
                        dogFence.transform.position = new Vector3((warpNextPos.x - 2) * 40, 10, warpNextPos.y * 40);
                        itemCells.Remove(new Vector2Int(warpNextPos.x - 1, warpNextPos.y));
                        itemCells.Remove(new Vector2Int(warpNextPos.x - 2, warpNextPos.y));
                        break;
                    case 2:
                        stayDog.transform.position = new Vector3(warpNextPos.x * 40, 0, (warpNextPos.y + 1) * 40);
                        dogFence.transform.position = new Vector3(warpNextPos.x * 40, 10, (warpNextPos.y + 2) * 40);
                        itemCells.Remove(new Vector2Int(warpNextPos.x, warpNextPos.y + 1));
                        itemCells.Remove(new Vector2Int(warpNextPos.x, warpNextPos.y + 2));
                        break;
                    case 3:
                        stayDog.transform.position = new Vector3((warpNextPos.x + 1) * 40, 0, warpNextPos.y * 40);
                        dogFence.transform.position = new Vector3((warpNextPos.x + 2) * 40, 10, warpNextPos.y * 40);
                        itemCells.Remove(new Vector2Int(warpNextPos.x + 1, warpNextPos.y));
                        itemCells.Remove(new Vector2Int(warpNextPos.x + 2, warpNextPos.y));
                        break;
                }
                // 止まる犬と犬のフェンスを方向に応じて向きを変更
                stayDog.transform.rotation = Quaternion.Euler(0, 90 * (warpdirection+2), 0);
                dogFence.transform.rotation = Quaternion.Euler(0, 90 * warpdirection, 0);
                // スイッチを方向に応じて配置し、向きを変更
                switchObj.transform.position = new Vector3(warpNextPos.x * 40, 0.5f, warpNextPos.y * 40);
                switchObj.transform.rotation = Quaternion.Euler(0, 90 * warpdirection, 0);
                // 止まる犬に対応したワープの出口とその隣の座標の場所をitemCellsから除外
                itemCells.Remove(warpPos);
                itemCells.Remove(warpNextPos);
                break;
            #endregion
            #region // stageNo = 5,6
            case 5:
            case 6:
                // ゴールに必要な犬の数
                goalCondition = 2;
                // 全てのオブジェクト（ワープ,ゴール前フェンス,止まる犬,犬フェンス,スイッチ,歩く犬）を出現
                warpIn.SetActive(true);
                warpOut.SetActive(true);
                goalFence.SetActive(true);
                stayDog.SetActive(true);
                switchObj.SetActive(true);
                dogFence.SetActive(true);
                walkDog.SetActive(true);
                // ワープの入口をactiveRandomリストに追加
                foreach(var warpInObj in warpInChild)
                {
                    activeRandom.Add(warpInObj);
                }
                // 止まる犬に対応した出口を除いたワープの出口をactiveRandomリストに追加
                for(int i=0; i<warpOutChild.Length-1; i++)
                {
                    activeRandom.Add(warpOutChild[i]);
                }
                // ゴール前フェンスをゴール前の座標に配置し、ゴールと同じ向きにする
                goalFence.transform.position = new Vector3(goalNextPos.x * 40, 10, goalNextPos.y * 40);
                goalFence.transform.rotation = goalObj.transform.rotation;
                // stayDogCellsのリストから１つ場所を取得し、その隣の道の座標と向きを取得
                warpPos = SetObjectPosition(stayDogCells);
                (warpdirection, warpNextPos) = GetMovableDirection(warpPos);
                // 止まる犬に対応したワープの出口を配置
                warpOut.transform.GetChild(10).transform.position = new Vector3(warpPos.x * 40, 0, warpPos.y * 40);
                // 止まる犬と犬のフェンスを方向に応じて配置
                switch(warpdirection)
                {
                    case 0:
                        stayDog.transform.position = new Vector3(warpNextPos.x * 40, 0, (warpNextPos.y - 1) * 40);
                        dogFence.transform.position = new Vector3(warpNextPos.x * 40, 10, (warpNextPos.y - 2) * 40);
                        // 止まる犬と犬のフェンスを配置した場所を他の仕掛けと被らないようitemCellsから除外
                        itemCells.Remove(new Vector2Int(warpNextPos.x, warpNextPos.y - 1));
                        itemCells.Remove(new Vector2Int(warpNextPos.x, warpNextPos.y - 2));
                        break;
                    case 1:
                        stayDog.transform.position = new Vector3((warpNextPos.x - 1) * 40, 0, warpNextPos.y * 40);
                        dogFence.transform.position = new Vector3((warpNextPos.x - 2) * 40, 10, warpNextPos.y * 40);
                        itemCells.Remove(new Vector2Int(warpNextPos.x - 1, warpNextPos.y));
                        itemCells.Remove(new Vector2Int(warpNextPos.x - 2, warpNextPos.y));
                        break;
                    case 2:
                        stayDog.transform.position = new Vector3(warpNextPos.x * 40, 0, (warpNextPos.y + 1) * 40);
                        dogFence.transform.position = new Vector3(warpNextPos.x * 40, 10, (warpNextPos.y + 2) * 40);
                        itemCells.Remove(new Vector2Int(warpNextPos.x, warpNextPos.y + 1));
                        itemCells.Remove(new Vector2Int(warpNextPos.x, warpNextPos.y + 2));
                        break;
                    case 3:
                        stayDog.transform.position = new Vector3((warpNextPos.x + 1) * 40, 0, warpNextPos.y * 40);
                        dogFence.transform.position = new Vector3((warpNextPos.x + 2) * 40, 10, warpNextPos.y * 40);
                        itemCells.Remove(new Vector2Int(warpNextPos.x + 1, warpNextPos.y));
                        itemCells.Remove(new Vector2Int(warpNextPos.x + 2, warpNextPos.y));
                        break;
                }
                // 止まる犬と犬のフェンスを方向に応じて向きを変更
                stayDog.transform.rotation = Quaternion.Euler(0, 90 * (warpdirection+2), 0);
                dogFence.transform.rotation = Quaternion.Euler(0, 90 * warpdirection, 0);
                // スイッチを方向に応じて配置し、向きを変更
                switchObj.transform.position = new Vector3(warpNextPos.x * 40, 0.5f, warpNextPos.y * 40);
                switchObj.transform.rotation = Quaternion.Euler(0, 90 * warpdirection, 0);
                // 止まる犬に対応したワープの出口とその隣の座標の場所をitemCellsから除外
                itemCells.Remove(warpPos);
                itemCells.Remove(warpNextPos);
                // itemCellsのリストから１つ場所を取得し、walkDogStartCellに代入後その場所に歩く犬を配置
                walkDogStartCell = SetObjectPosition(itemCells);
                walkDog.transform.position = new Vector3(walkDogStartCell.x * 40, 0f, walkDogStartCell.y * 40);
                // walkDogStartCellの場所をitemCellsから除外
                itemCells.Remove(walkDogStartCell);
                break;
            #endregion
        }

        //ワープを配置する
        foreach(GameObject item in activeRandom)
        {
            var itemPos = SetObjectPosition(itemCells);
            item.transform.position = new Vector3(itemPos.x * 40, 0, itemPos.y * 40);
            itemCells.Remove(itemPos);
        }

        // //行き詰まりにコインバーを配置する
        foreach(var noPass in noPassCells)
        {
            var (noPassdirection, noPassNextPos) = GetMovableDirection(noPass);
            var coinBar = Instantiate(goldBar, new Vector3(noPass.x * 40, 0.6f, noPass.y * 40), Quaternion.Euler(0, 90 * noPassdirection, 0));
            coinBar.transform.parent = this.transform;
        }
    }

    // オブジェクト配置地点の取得
    private Vector2Int SetObjectPosition(List<Vector2Int> argCell)
    {
        // 引数のリストの数からランダムに１つ取得し、そのリストの番号の値を返す
        int rnd = Random.Range(0, argCell.Count);
        return argCell[rnd];
    }

    // オブジェクト配置方向を取得する
    private (int, Vector2Int) GetMovableDirection(Vector2Int _objectPos)
    {
        // 向く方向毎に1つ先のx,y座標を仮計算するリストを作成
        List<Vector2Int> positions = new List<Vector2Int>();

        // オブジェクトの前後左右を仮計算し、positionsに入れる
        foreach(var position in Positions)
        {
            var tmppos = (_objectPos + position);
            positions.Add(new Vector2Int(tmppos.x, tmppos.y));
        }

        // 向く方向毎に向く先の座標が道であるかを判定する
        // 真であれば、返却用変数に追加する
        int direction = default;  // リストの何番目かを入れる変数

        foreach(var position in positions)
        {
            if (!IsOutOfBounds(position.x, position.y) && cells[position.x, position.y] == CellType.Path)
                direction = positions.IndexOf(position);
        }

        // 方向とその座標を返す
        return (direction, positions[direction]);
    }

    // 行き止まりのリストを取得する
    private void StalemateCheck(List<Vector2Int> _itemCell)
    {
        foreach(var cell in _itemCell)
        {
            //前後左右の座標が壁であるかを判定する
            //3つ壁であれば、行き詰まり用Listに追加する
            int wallNum = 0;  // 壁である数

            foreach(var position in Positions)
            {
                var tmppos = (cell + position);
                if (IsOutOfBounds(tmppos.x, tmppos.y) || cells[tmppos.x, tmppos.y] == CellType.Wall)
                    wallNum++;
            }

            if(wallNum >= 3) noPassCells.Add(new Vector2Int(cell.x, cell.y));
        }
    }

    // ４マス先が行き止まりの地点を取得する
    private void FourSquareCheck(List<Vector2Int> _noPassCell)
    {
        // 行き詰まりのリストを引数として取得し、そのリスト内の座標を随時チェック
        foreach(var noPass in _noPassCell)
        {
            //可読性のため座標を変数に格納
            var x = noPass.x;
            var y = noPass.y;

            // if文の一行目：３つ先まで範囲内かの判定
            // if文の二行目：３つ先まで道かの判定
            // if文の三行目：２つ先の左右が壁かの判定
            //この３つを満たす場合、行き詰まりの座標をstayDogCellsリストに追加する
            if (!IsOutOfBounds(x, y-1) && !IsOutOfBounds(x, y-2) && !IsOutOfBounds(x, y-3) && 
                cells[x, y-1] == CellType.Path && cells[x, y-2] == CellType.Path && cells[x, y-3] == CellType.Path &&
                (IsOutOfBounds(x-1, y-2) || cells[x-1, y-2] == CellType.Wall) && (IsOutOfBounds(x+1, y-2) || cells[x+1, y-2] == CellType.Wall))
                    stayDogCells.Add(new Vector2Int(x, y));
            if (!IsOutOfBounds(x, y+1) && !IsOutOfBounds(x, y+2) && !IsOutOfBounds(x, y+3) &&
                cells[x, y+1] == CellType.Path && cells[x, y+2] == CellType.Path && cells[x, y+3] == CellType.Path &&
                (IsOutOfBounds(x-1, y+2) || cells[x-1, y+2] == CellType.Wall) && (IsOutOfBounds(x+1, y+2) || cells[x+1, y+2] == CellType.Wall))
                    stayDogCells.Add(new Vector2Int(x, y));
            if (!IsOutOfBounds(x-1, y) && !IsOutOfBounds(x-2, y) && !IsOutOfBounds(x-3, y) && 
                cells[x-1, y] == CellType.Path && cells[x-2, y] == CellType.Path && cells[x-3, y] == CellType.Path &&
                (IsOutOfBounds(x-2, y-1) || cells[x-2, y-1] == CellType.Wall) && (IsOutOfBounds(x-2, y+1) || cells[x-2, y+1] == CellType.Wall))
                    stayDogCells.Add(new Vector2Int(x, y));
            if (!IsOutOfBounds(x+1, y) && !IsOutOfBounds(x+2, y) && !IsOutOfBounds(x+3, y) && 
                cells[x+1, y] == CellType.Path && cells[x+2, y] == CellType.Path && cells[x+3, y] == CellType.Path &&
                (IsOutOfBounds(x+2, y-1) || cells[x+2, y-1] == CellType.Wall) && (IsOutOfBounds(x+2, y+1) || cells[x+2, y+1] == CellType.Wall))
                    stayDogCells.Add(new Vector2Int(x, y));
        }
    }
}