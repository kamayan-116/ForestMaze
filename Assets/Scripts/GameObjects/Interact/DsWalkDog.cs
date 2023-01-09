using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 歩く犬に関するプログラム
public class DsWalkDog : MonoBehaviour, IInteractive
{
    [SerializeField] GameObject player;  // プレイヤーオブジェクト
    [SerializeField] GameObject kusoObj;  // 糞オブジェクト
    [SerializeField] GameObject nowWalkDogPos;  // マップでの犬の滞在地の表示用Cube
    [SerializeField] GameObject disButton;
    [SerializeField] MakeMaze makeMaze;
    [SerializeField] bool isPlayerTouch = false;  // プレイヤーに捕まったか否か
    int[] dist_x = new int[5];  // 巡回xセル
    int[] dist_z = new int[5];  // 巡回yセル
    Vector2Int[] distCell = new Vector2Int[5];  //巡回セル
    Vector3[] destination = new Vector3[5];  // 巡回目的地座標
    int distArray = 0;  // 現在の目的地配列番号
    Vector2Int startCell; // 順路徘徊のスタートセル
    Vector2Int goalCell; // 順路徘徊のゴールセル
    List<Vector2Int> pathdata = new List<Vector2Int>(); // ダイクストラ法により最短距離を入れたリスト
    Vector3 startPos; // 順路徘徊のスタート地点
    Vector3 goalPos; // 順路徘徊のゴール地点
    float nowgoalDistance;  //目的地と犬の距離
    Queue<Vector3> pvec3 = new Queue<Vector3>(); // 巡回座標を入れたキュー
    [SerializeField] private float speed;  // 犬の歩くスピード
    [SerializeField] private bool isDijkstra = true;// ダイクストラ法を計算するか否か
    private bool isPlus = false;  // 目的地配列番号を増やすか否か
    private bool isDestination = false;// ゴール座標についたか否か
    float time = 0.0f;
    float spanTime = 5.0f;  // 糞を生成する間隔
    int count = 0;  // 生成した糞の数
    GameObject[] kuso = new GameObject[3];

    // Start is called before the first frame update
    void Start()
    {
        for (int i=0; i<5; i++)
        {
            // itemCellsのListから1つランダムに取ってdistCell[i]に代入
            int rnd = Random.Range(0, makeMaze.itemCells.Count);
            dist_x[i] = makeMaze.itemCells[rnd].x;
            dist_z[i] = makeMaze.itemCells[rnd].y;

            distCell[i] = new Vector2Int(dist_x[i], dist_z[i]);

            // distCellをVector3に変換し、destinationに代入
            destination[i] = new Vector3(distCell[i].x * 40, 0, distCell[i].y * 40);

            // 同じ値が選ばれないよう、distCellをitemCellsから消去
            makeMaze.itemCells.Remove(distCell[i]);
            // Debug.Log($"{distCell[i]}");
        }

        startCell = makeMaze.walkDogStartCell;
        goalCell = distCell[0];
    }

    // Update is called once per frame
    void Update()
    {
        if(isPlayerTouch) return;

        #region // ダイクストラ法の計算と犬の徘徊プログラム
        if(isDijkstra)  // ダイクストラ法の計算
        {
            pathdata = new Dijkstra(makeMaze).DijkstraFinding(startCell, goalCell);

            // 計算結果をpvec3のキューに代入
            foreach(var p in pathdata)
            {
                pvec3.Enqueue(new Vector3(p.x * 40, 0, p.y * 40));
            }

            startPos = this.transform.position;
            goalPos = destination[distArray];

            isDijkstra = false;
        }
        else  // 計算結果に伴う犬の移動
        {
            // キューの一番最初の値を読み取りnowgoalPosに代入
            var nowgoalPos = pvec3.Peek();
            nowgoalDistance = Vector3.Distance(this.transform.position, nowgoalPos);
            // Debug.Log(nowgoalDistance);

            // nowgoalPosとの距離が0.01以上の時移動
            if(nowgoalDistance > 0.01f)
            {
                // nowgoalPosへの向きに応じて向きの変更
                if(this.transform.position.x < nowgoalPos.x)
                {
                    this.transform.rotation = Quaternion.Euler(0, 90, 0);
                }
                if(this.transform.position.x > nowgoalPos.x)
                {
                    this.transform.rotation = Quaternion.Euler(0, -90, 0);
                }
                if(this.transform.position.z < nowgoalPos.z)
                {
                    this.transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                if(this.transform.position.z > nowgoalPos.z)
                {
                    this.transform.rotation = Quaternion.Euler(0, 180, 0);
                }
                // 現在の位置からnowgoalPosへspeedの速さで移動
                transform.position = Vector3.MoveTowards(this.transform.position, nowgoalPos, Time.deltaTime * speed);
                //Debug.Log(this.transform.position + ":" + nowgoalPos);
            }
            else
            {
                // キューの一番最初の値を捨てる
                pvec3.Dequeue();

                // キューの数が0になればゴールに到達、さもなければ同様に繰り返す
                if(pvec3.Count == 0)
                {
                    isDestination = true;
                }
                else
                {
                    nowgoalPos = pvec3.Peek();
                }
            }
            
            if(isDestination)  // ゴールに着いたときに行う処理
            {
                // Debug.Log($"a = {a} の時到着");

                // 次のスタートセルを現在のゴールにする
                startCell = distCell[distArray];

                // 目的地配列番号を1つ増やし、5以上になれば0に戻す
                isPlus = true;
                if(isPlus)
                {
                    distArray++;
                    isPlus = false;
                }
                if (distArray >= 5)
                {
                    distArray = 0;
                }

                // 次のゴールセルを更新し、再度ダイクストラ法を行う
                goalCell = distCell[distArray];
                isDijkstra = true;
                isDestination = false;
            }
        }
        #endregion
                
        time += Time.deltaTime;

        if (time >= spanTime)
        {
            time = 0.0f;
            // 糞の生成数が3つ未満の際生成を行う
            if(count < 3)
            {
                kuso[count] = Instantiate(kusoObj, new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), Quaternion.identity);
                count++;
                // Debug.Log($"count = {count}");
            }
        }
    }

    public void Interact()
    {
        this.GetComponent<BoxCollider>().enabled = false;
        this.GetComponent<AudioSource>().enabled = false;

        Destroy(nowWalkDogPos);

        CanvasManager.instance.ButtonHide(CanvasManager.instance.buttons[0]);
        isPlayerTouch = true;

        foreach (var item in kuso)
        {
            Destroy(item);
        }

        // プレイヤーの子オブジェクトにして、プレイヤーの背後につける
        transform.parent = player.gameObject.transform;
        this.transform.localPosition = new Vector3(0.0f, 0.0f, -5.0f);
        this.transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
    }
}
