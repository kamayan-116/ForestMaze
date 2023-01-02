using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MakeMaze : MonoBehaviour
{
    //1つ先のx,y座標を計算
    public static readonly Vector2Int[] Positions = {
        new Vector2Int (0, -1),
        new Vector2Int (-1, 0),
        new Vector2Int (0, 1),
        new Vector2Int (1, 0),
    };

    //2つ先のx,y座標を仮計算
    public static readonly Vector2Int[] Positions2 = {
        new Vector2Int (0, -2),
        new Vector2Int (-2, 0),
        new Vector2Int (0, 2),
        new Vector2Int (2, 0),
    };

    // 設定する値
    [SerializeField] public int max = 5;        //縦横のサイズ ※必ず奇数にすること
    [SerializeField] private GameObject wall;    //壁用オブジェクト
    [SerializeField] private GameObject floor;   //床用オブジェクト
    [SerializeField] private GameObject start = null;   //スタート地点に配置するオブジェクト
    [SerializeField] private GameObject goal = null;    //ゴール地点に配置するオブジェクト
    [SerializeField] private GameObject player;    //プレイヤーオブジェクト
    [SerializeField] private GameObject[] itemObj;    //ステージレベルに応じた各仕掛けをまとめた配列
    private GameObject[] warpInChild;    //ワープに入る各オブジェクト
    private GameObject[] warpOutChild;    //ワープから出る各オブジェクト
    [SerializeField] private int moveMap = 0; //ゴール地点までに移動した回数
    [SerializeField] private bool isGoalSet;

    // 内部パラメータ
    public enum CellType {Wall, Path};   //セルの種類
    public CellType[,] cells;
    
    private Stack<Vector2Int> startCells;   // 穴掘り開始候補座標
    public List<Vector2Int> itemCells;   // 各仕掛け配置候補座標
    public List<Vector2Int> noPassCells;   // 行き詰まり座標
    private List<Vector2Int> stayDogCells = new List<Vector2Int>();   // 止まっている犬の配置候補座標
    public Vector2Int walkDogStartCell;
    public int goalCondition;

    private Vector2Int startPos;    //スタートの座標
    private Vector2Int goalPos;     //ゴールの座標


    private void Start ()
    {
        warpInChild = new GameObject[itemObj[0].transform.childCount];
        for(int i=0; i<itemObj[0].transform.childCount; i++)
        {
            warpInChild[i] = itemObj[0].transform.GetChild(i).gameObject;
        }

        warpOutChild = new GameObject[itemObj[1].transform.childCount];
        for(int i=0; i<itemObj[1].transform.childCount; i++)
        {
            warpOutChild[i] = itemObj[1].transform.GetChild(i).gameObject;
        }

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
                var tmpStart = startCells.Peek();
                MakeMapInfo(tmpStart);
            }

            //行き詰まりの場所をリストに入れる
            StalemateCheck(itemCells);

            //行き詰まりから4マス連続の場所をリストに入れる
            FourSquareCheck(noPassCells);
        }

        //マップの状態に応じて壁と通路を生成する
        BuildDungeon();

        //各地点にオブジェクトを配置する
        ObjectArrange();
    }

    // ゴール生成
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

            //元の地点と通路にした座標の間を通路にする
            var xPos = tmpPos.x + (tmpStartPos.x - tmpPos.x) / 2;
            var yPos = tmpPos.y + (tmpStartPos.y - tmpPos.y) / 2;
            SetPath(xPos, yPos);
            moveMap++;

            //移動後の座標を一時変数に格納し、再度移動可能な座標を探索する
            tmpStartPos = tmpPos;
            movablePositions = GetMovablePositions(tmpStartPos);
        }

        if(movablePositions == null)
        {
            startCells.Pop();
        }

        if(moveMap > 30)
        {
            isGoalSet = true;
        }

        //探索終了時の座標を返す
        return tmpStartPos;
    }

    // マップ生成
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

            //移動可能な座標からランダムで1つ取得し通路にする
            var tmpPos = movablePositions[Random.Range(0, movablePositions.Count)];
            SetPath(tmpPos.x, tmpPos.y);
            itemCells.Add(new Vector2Int(tmpPos.x, tmpPos.y));

            //元の地点と通路にした座標の間を通路にする
            var xPos = tmpPos.x + (tmpStartPos.x - tmpPos.x) / 2;
            var yPos = tmpPos.y + (tmpStartPos.y - tmpPos.y) / 2;
            SetPath(xPos, yPos);
            itemCells.Add(new Vector2Int(xPos, yPos));

            //移動後の座標を一時変数に格納し、再度移動可能な座標を探索する
            tmpStartPos = tmpPos;
            movablePositions = GetMovablePositions(tmpStartPos);
        }

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
        //移動方向毎に移動先の座標が範囲内かつ壁であるかを判定する
        //真であれば、返却用リストに追加する
        var movablePositions = new List<Vector2Int>();

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
            // 穴掘り候補座標
            startCells.Push(new Vector2Int(x, y));
        }
    }

    //与えられたx、y座標が範囲外の場合真を返す
    public bool IsOutOfBounds( int x, int y ) => ( x < 0 || y < 0 || x >= max || y >= max );

    //パラメータに応じてオブジェクトを生成する
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
        //スタート地点とゴール地点にオブジェクトを配置する
        //初回で取得したスタート地点とゴール地点は必ずつながっているので破綻しない
        var (startdirection, startNextPos) = GetMovableDirection(startPos);
        var (goaldirection, goalNextPos) = GetMovableDirection(goalPos);
        var startObj = Instantiate(start, new Vector3(startPos.x * 40, 0.01f, startPos.y * 40), Quaternion.Euler(0, 90 * startdirection, 0));
        GameObject goalObj = null;
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

        startObj.transform.parent = this.transform;
        goalObj.transform.parent = this.transform;

        //まずはプレイヤーをスタート地点に配置
        player.transform.position = startObj.transform.position;
        player.transform.parent = this.transform;

        //ステージ難易度に応じてオブジェクトの出現を調整
        var activeRandom = new List<GameObject>();
        switch(NonGameCanvasCtrl.Instance.stageNo)
        {
            case 1:
                for(int i=0; i<=1; i++)
                {
                    itemObj[i].SetActive(true);
                    if(i<=1)
                    {
                        for(int j=0; j<itemObj[i].transform.childCount; j++)
                        {
                            activeRandom.Add(itemObj[i].transform.GetChild(j).gameObject);
                        }
                    }
                }
                for(int i=2; i<7; i++)
                {
                    itemObj[i].SetActive(false);
                    
                }
                break;
            case 2:
                for(int i=0; i<=2; i++)
                {
                    itemObj[i].SetActive(true);
                    if(i<=1)
                    {
                        for(int j=0; j<itemObj[i].transform.childCount; j++)
                        {
                            activeRandom.Add(itemObj[i].transform.GetChild(j).gameObject);
                        }
                    }
                    
                }
                itemObj[2].transform.position = new Vector3(goalNextPos.x * 40, 10, goalNextPos.y * 40);
                itemObj[2].transform.rotation = goalObj.transform.rotation;
                for(int i=3; i<7; i++)
                {
                    itemObj[i].SetActive(false);
                }
                break;
            case 3:
                goalCondition = 1;
                for(int i=0; i<=3; i++)
                {
                    itemObj[i].SetActive(true);
                    if(i == 0)
                    {
                        for(int j=0; j<itemObj[i].transform.childCount; j++)
                        {
                            activeRandom.Add(itemObj[i].transform.GetChild(j).gameObject);
                        }
                    }
                    if(i == 1)
                    {
                        for(int j=0; j<itemObj[i].transform.childCount-1; j++)
                        {
                            activeRandom.Add(itemObj[i].transform.GetChild(j).gameObject);
                        }
                    }
                }
                itemObj[2].transform.position = new Vector3(goalNextPos.x * 40, 10, goalNextPos.y * 40);
                itemObj[2].transform.rotation = goalObj.transform.rotation;
                var warpPos = SetObjectPosition(stayDogCells);
                var (warpdirection, warpNextPos) = GetMovableDirection(warpPos);
                itemObj[1].transform.GetChild(10).transform.position = new Vector3(warpPos.x * 40, 0, warpPos.y * 40);
                switch(warpdirection)
                {
                    case 0:
                        itemObj[3].transform.position = new Vector3(warpNextPos.x * 40, 0, (warpNextPos.y - 1) * 40);
                        itemCells.Remove(new Vector2Int(warpNextPos.x, warpNextPos.y - 1));
                        break;
                    case 1:
                        itemObj[3].transform.position = new Vector3((warpNextPos.x - 1) * 40, 0, warpNextPos.y * 40);
                        itemCells.Remove(new Vector2Int(warpNextPos.x - 1, warpNextPos.y));
                        break;
                    case 2:
                        itemObj[3].transform.position = new Vector3(warpNextPos.x * 40, 0, (warpNextPos.y + 1) * 40);
                        itemCells.Remove(new Vector2Int(warpNextPos.x, warpNextPos.y + 1));
                        break;
                    case 3:
                        itemObj[3].transform.position = new Vector3((warpNextPos.x + 1) * 40, 0, warpNextPos.y * 40);
                        itemCells.Remove(new Vector2Int(warpNextPos.x + 1, warpNextPos.y));
                        break;
                }
                itemObj[3].transform.rotation = Quaternion.Euler(0, 90 * (warpdirection+2), 0);
                itemCells.Remove(warpPos);
                for(int i=4; i<7; i++)
                {
                    itemObj[i].SetActive(false);
                }
                break;
            case 4:
                goalCondition = 1;
                for(int i=0; i<=5; i++)
                {
                    itemObj[i].SetActive(true);
                    if(i == 0)
                    {
                        for(int j=0; j<itemObj[i].transform.childCount; j++)
                        {
                            activeRandom.Add(itemObj[i].transform.GetChild(j).gameObject);
                        }
                    }
                    if(i == 1)
                    {
                        for(int j=0; j<itemObj[i].transform.childCount-1; j++)
                        {
                            activeRandom.Add(itemObj[i].transform.GetChild(j).gameObject);
                        }
                    }
                }
                itemObj[2].transform.position = new Vector3(goalNextPos.x * 40, 10, goalNextPos.y * 40);
                itemObj[2].transform.rotation = goalObj.transform.rotation;
                warpPos = SetObjectPosition(stayDogCells);
                (warpdirection, warpNextPos) = GetMovableDirection(warpPos);
                itemObj[1].transform.GetChild(10).transform.position = new Vector3(warpPos.x * 40, 0, warpPos.y * 40);
                switch(warpdirection)
                {
                    case 0:
                        itemObj[3].transform.position = new Vector3(warpNextPos.x * 40, 0, (warpNextPos.y - 1) * 40);
                        itemObj[5].transform.position = new Vector3(warpNextPos.x * 40, 10, (warpNextPos.y - 2) * 40);
                        itemCells.Remove(new Vector2Int(warpNextPos.x, warpNextPos.y - 1));
                        itemCells.Remove(new Vector2Int(warpNextPos.x, warpNextPos.y - 2));
                        break;
                    case 1:
                        itemObj[3].transform.position = new Vector3((warpNextPos.x - 1) * 40, 0, warpNextPos.y * 40);
                        itemObj[5].transform.position = new Vector3((warpNextPos.x - 2) * 40, 10, warpNextPos.y * 40);
                        itemCells.Remove(new Vector2Int(warpNextPos.x - 1, warpNextPos.y));
                        itemCells.Remove(new Vector2Int(warpNextPos.x - 2, warpNextPos.y));
                        break;
                    case 2:
                        itemObj[3].transform.position = new Vector3(warpNextPos.x * 40, 0, (warpNextPos.y + 1) * 40);
                        itemObj[5].transform.position = new Vector3(warpNextPos.x * 40, 10, (warpNextPos.y + 2) * 40);
                        itemCells.Remove(new Vector2Int(warpNextPos.x, warpNextPos.y + 1));
                        itemCells.Remove(new Vector2Int(warpNextPos.x, warpNextPos.y + 2));
                        break;
                    case 3:
                        itemObj[3].transform.position = new Vector3((warpNextPos.x + 1) * 40, 0, warpNextPos.y * 40);
                        itemObj[5].transform.position = new Vector3((warpNextPos.x + 2) * 40, 10, warpNextPos.y * 40);
                        itemCells.Remove(new Vector2Int(warpNextPos.x + 1, warpNextPos.y));
                        itemCells.Remove(new Vector2Int(warpNextPos.x + 2, warpNextPos.y));
                        break;
                }
                itemObj[3].transform.rotation = Quaternion.Euler(0, 90 * (warpdirection+2), 0);
                itemObj[4].transform.position = new Vector3(warpNextPos.x * 40, 0.5f, warpNextPos.y * 40);
                itemObj[4].transform.rotation = Quaternion.Euler(0, 90 * warpdirection, 0);
                itemObj[5].transform.rotation = Quaternion.Euler(0, 90 * warpdirection, 0);
                itemCells.Remove(warpPos);
                itemCells.Remove(warpNextPos);
                itemObj[6].SetActive(false);
                break;
            case 5:
            case 6:
                goalCondition = 2;
                for(int i=0; i<7; i++)
                {
                    itemObj[i].SetActive(true);
                    if(i == 0)
                    {
                        for(int j=0; j<itemObj[i].transform.childCount; j++)
                        {
                            activeRandom.Add(itemObj[i].transform.GetChild(j).gameObject);
                        }
                    }
                    if(i == 1)
                    {
                        for(int j=0; j<itemObj[i].transform.childCount-1; j++)
                        {
                            activeRandom.Add(itemObj[i].transform.GetChild(j).gameObject);
                        }
                    }
                }
                itemObj[2].transform.position = new Vector3(goalNextPos.x * 40, 10, goalNextPos.y * 40);
                itemObj[2].transform.rotation = goalObj.transform.rotation;
                warpPos = SetObjectPosition(stayDogCells);
                (warpdirection, warpNextPos) = GetMovableDirection(warpPos);
                itemObj[1].transform.GetChild(10).transform.position = new Vector3(warpPos.x * 40, 0, warpPos.y * 40);
                switch(warpdirection)
                {
                    case 0:
                        itemObj[3].transform.position = new Vector3(warpNextPos.x * 40, 0, (warpNextPos.y - 1) * 40);
                        itemObj[5].transform.position = new Vector3(warpNextPos.x * 40, 10, (warpNextPos.y - 2) * 40);
                        itemCells.Remove(new Vector2Int(warpNextPos.x, warpNextPos.y - 1));
                        itemCells.Remove(new Vector2Int(warpNextPos.x, warpNextPos.y - 2));
                        break;
                    case 1:
                        itemObj[3].transform.position = new Vector3((warpNextPos.x - 1) * 40, 0, warpNextPos.y * 40);
                        itemObj[5].transform.position = new Vector3((warpNextPos.x - 2) * 40, 10, warpNextPos.y * 40);
                        itemCells.Remove(new Vector2Int(warpNextPos.x - 1, warpNextPos.y));
                        itemCells.Remove(new Vector2Int(warpNextPos.x - 2, warpNextPos.y));
                        break;
                    case 2:
                        itemObj[3].transform.position = new Vector3(warpNextPos.x * 40, 0, (warpNextPos.y + 1) * 40);
                        itemObj[5].transform.position = new Vector3(warpNextPos.x * 40, 10, (warpNextPos.y + 2) * 40);
                        itemCells.Remove(new Vector2Int(warpNextPos.x, warpNextPos.y + 1));
                        itemCells.Remove(new Vector2Int(warpNextPos.x, warpNextPos.y + 2));
                        break;
                    case 3:
                        itemObj[3].transform.position = new Vector3((warpNextPos.x + 1) * 40, 0, warpNextPos.y * 40);
                        itemObj[5].transform.position = new Vector3((warpNextPos.x + 2) * 40, 10, warpNextPos.y * 40);
                        itemCells.Remove(new Vector2Int(warpNextPos.x + 1, warpNextPos.y));
                        itemCells.Remove(new Vector2Int(warpNextPos.x + 2, warpNextPos.y));
                        break;
                }
                itemObj[3].transform.rotation = Quaternion.Euler(0, 90 * (warpdirection+2), 0);
                itemObj[4].transform.position = new Vector3(warpNextPos.x * 40, 0.5f, warpNextPos.y * 40);
                itemObj[4].transform.rotation = Quaternion.Euler(0, 90 * warpdirection, 0);
                itemObj[5].transform.rotation = Quaternion.Euler(0, 90 * warpdirection, 0);
                itemCells.Remove(warpPos);
                itemCells.Remove(warpNextPos);
                var dogCell = SetObjectPosition(itemCells);
                walkDogStartCell = dogCell;
                itemObj[6].transform.position = new Vector3(walkDogStartCell.x * 40, 0f, walkDogStartCell.y * 40);
                itemCells.Remove(walkDogStartCell);
                break;
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
            var coinBar = Instantiate(itemObj[7], new Vector3(noPass.x * 40, 0.6f, noPass.y * 40), Quaternion.Euler(0, 90 * noPassdirection, 0));
            coinBar.transform.parent = this.transform;
        }
    }

    // オブジェクト配置地点の取得
    private Vector2Int SetObjectPosition(List<Vector2Int> argCell)
    {
        int rnd = Random.Range(0, argCell.Count);
        return argCell[rnd];
    }

    // オブジェクト配置方向を取得する
    private (int, Vector2Int) GetMovableDirection(Vector2Int _objectPos)
    {
        //向く方向毎に1つ先のx,y座標を仮計算するリストを作成
        List<Vector2Int> positions = new List<Vector2Int>();

        //1つ先のx,y座標を仮計算
        foreach(var position in Positions)
        {
            var tmppos = (_objectPos + position);
            positions.Add(new Vector2Int(tmppos.x, tmppos.y));
        }

        //向く方向毎に向く先の座標が道であるかを判定する
        //真であれば、返却用変数に追加する
        int direction = default;

        foreach(var position in positions)
        {
            if (!IsOutOfBounds(position.x, position.y) && cells[position.x, position.y] == CellType.Path)
                direction = positions.IndexOf(position);
        }

        return (direction, positions[direction]);
    }

    // 行き止まりのリストを取得する
    private void StalemateCheck(List<Vector2Int> _itemCell)
    {
        foreach(var cell in _itemCell)
        {
            //前後左右の座標が壁であるかを判定する
            //3つ壁であれば、行き詰まり用Listに追加する
            int wallNum = 0;

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
        foreach(var noPass in _noPassCell)
        {
            //可読性のため座標を変数に格納
            var x = noPass.x;
            var y = noPass.y;

            //前後左右の座標が3つ先まで道かを判定し、道であれば候補リストに追加する
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