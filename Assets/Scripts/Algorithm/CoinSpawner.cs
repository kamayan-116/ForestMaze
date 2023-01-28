using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ゲーム内にコインを自動生成するプログラム
public class CoinSpawner : MonoBehaviour
{
    [SerializeField] private GameObject coinPrefab;  // コイン
    [SerializeField] private float placementInterval = 10.0f;  // コインを設置する間隔
    [SerializeField] private float coinPositionY = 1.0f;  // コイン自体のサイズ（サイズ分上に生成する）
    private GameObject floorObj;  // 迷路1つずつのオブジェクト
    private Vector3 floorSize;  // 迷路1つのサイズ
    private Ray ray;
    private RaycastHit rayCastHit;
    private List<Vector3> positions = new List<Vector3>();  // コインを生成する場所のリスト

    // Start is called before the first frame update
    void Start()
    {
        floorObj = GameObject.FindGameObjectWithTag("Floor");

        GetFloorSize();
        SetSpawnPosition();
        CreateCoins();
    }

    // Rayを飛ばす範囲の決定
    void GetFloorSize()
    {
        MeshRenderer floorRenderer = floorObj.GetComponent<MeshRenderer>();
        floorSize = new Vector3(floorRenderer.bounds.size.x * MakeMaze.Instance.max, floorRenderer.bounds.size.y, floorRenderer.bounds.size.z * MakeMaze.Instance.max);
    }

    // コインの生成場所の決定
    void SetSpawnPosition()
    {
        float x = 0;
        float z = 0;

        // 上記の範囲内で変数placementIntervalの間隔でRayを飛ばし、生成場所をリストに入れる
        for(int i=0; i< Mathf.RoundToInt(floorSize.z / placementInterval); i++)
        {
            x = 0;
            for(int j=0; j< Mathf.RoundToInt(floorSize.x / placementInterval); j++)
            {
                // オブジェクトのある位置から真下にRayを飛ばす
                ray = new Ray(new Vector3(x, transform.position.y, z), transform.TransformDirection(Vector3.down));
                if(Physics.Raycast(ray, out rayCastHit, transform.position.y))
                {
                    if(rayCastHit.collider.tag == "Floor")
                    {
                        positions.Add(rayCastHit.point);
                    }
                }
                x += placementInterval;
            }
            z += placementInterval;
        }
    }

    //コインを生成する関数
    void CreateCoins()
    {
        // positionsのリスト内の各場所にコインを生成し、コインをこのオブジェクトの子オブジェクトにする
        foreach(Vector3 position in positions)
        {
            var coinObj = Instantiate(coinPrefab, new Vector3(position.x, position.y + coinPositionY, position.z), Quaternion.Euler(0, 0, 0));
            coinObj.transform.parent = this.transform;
        }
    }
}
