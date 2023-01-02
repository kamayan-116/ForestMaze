using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DsWalkDog : MonoBehaviour
{
    [SerializeField] GameObject kuso;
    [SerializeField] GameObject NowWalkDogPos;
    [SerializeField] GameObject DisButton;
    [SerializeField] MakeMaze makeMaze;
    [SerializeField] bool isPlayerTouch = false;  // プレイヤーに捕まったか否か
    int[] dist_x = new int[5];  // 巡回xセル
    int[] dist_z = new int[5];  // 巡回yセル
    Vector2Int[] distCell = new Vector2Int[5];  //巡回セル
    int a = 0;  // 現在の目的地配列番号
    Vector2Int startCell; // 順路徘徊のスタートセル
    Vector2Int goalCell; // 順路徘徊のゴールセル
    List<Vector2Int> pathdata = new List<Vector2Int>(); // ダイクストラ法により最短距離を入れたスタック
    Vector3 startPos; // 順路徘徊のスタート地点
    Vector3 goalPos; // 順路徘徊のゴール地点
    float nowgoalDistance;  // ダイクストラ法による最短距離
    Vector3[] destination = new Vector3[5];  // 巡回目的地座標
    Queue<Vector3> pvec3 = new Queue<Vector3>(); // 巡回座標
    [SerializeField] private float speed;
    [SerializeField] private bool isDijkstra = true;
    private bool isPlus = false;
    private bool isDestination = false;
    float time = 0.0f;
    float SpanTime = 5.0f;
    int count = 0;
    GameObject[] Kuso = new GameObject[3];

    // Start is called before the first frame update
    void Start()
    {
        for (int i=0; i<5; i++)
        {
            int rnd = Random.Range(0, makeMaze.itemCells.Count);
            dist_x[i] = makeMaze.itemCells[rnd].x;
            dist_z[i] = makeMaze.itemCells[rnd].y;

            distCell[i] = new Vector2Int(dist_x[i], dist_z[i]);

            destination[i] = new Vector3(distCell[i].x * 40, 0, distCell[i].y * 40);
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
        if(isDijkstra)
        {
            pathdata = new Dijkstra(makeMaze).DijkstraFinding(startCell, goalCell);
            foreach(var p in pathdata)
            {
                pvec3.Enqueue(new Vector3(p.x * 40, 0, p.y * 40));
            }

            startPos = this.transform.position;
            goalPos = destination[a];

            isDijkstra = false;
        }
        else
        {
            var nowgoalPos = pvec3.Peek();
            nowgoalDistance = Vector3.Distance(this.transform.position, nowgoalPos);
            // Debug.Log(nowgoalDistance);
            if(nowgoalDistance > 0.01f)
            {
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
                transform.position = Vector3.MoveTowards(this.transform.position, nowgoalPos, Time.deltaTime * speed);
                //Debug.Log(this.transform.position + ":" + nowgoalPos);
            }
            else
            {
                pvec3.Dequeue();
                if(pvec3.Count == 0)
                {
                    isDestination = true;
                }
                else
                {
                    nowgoalPos = pvec3.Peek();
                }
            }
            
            if(isDestination)
            {
                // Debug.Log($"a = {a} の時到着");
                startCell = distCell[a];
                isPlus = true;
                if(isPlus)
                {
                    a++;
                    isPlus = false;
                }
                if (a >= 5)
                {
                    a = 0;
                }
                goalCell = distCell[a];
                isDijkstra = true;
                isDestination = false;
            }
        }
                
        time += Time.deltaTime;

        if (time >= SpanTime)
        {
            time = 0.0f;
            if(count < 3)
            {
                Kuso[count] = Instantiate(kuso, new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), Quaternion.identity);
                count++;
                // Debug.Log($"count = {count}");
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            this.GetComponent<BoxCollider>().enabled = false;
            this.GetComponent<AudioSource>().enabled = false;
            
            Destroy(NowWalkDogPos);

            DisButton.GetComponent<ButtonCtrl>().DistanceCount(0);
            isPlayerTouch = true;

            foreach (var item in Kuso)
            {
                Destroy(item);
            }

            transform.parent = collision.gameObject.transform;
            this.transform.localPosition = new Vector3(0.0f, 0.0f, -5.0f);
        }
    }
}
