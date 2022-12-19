using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    [SerializeField] private MakeMaze makeMaze;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] float placementInterval = 10.0f;
    [SerializeField] float coinPositionY = 1.0f;
    GameObject floorObj;
    Vector3 floorSize;
    Ray ray;
    RaycastHit rayCastHit;
    Vector3 rayPosition;
    List<Vector3> positions = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        floorObj = GameObject.FindGameObjectWithTag("Floor");

        GetFloorSize();
        SetSpawnPosition();
        CreateCoins();
    }

    void GetFloorSize()
    {
        MeshRenderer floorRenderer = floorObj.GetComponent<MeshRenderer>();
        floorSize = new Vector3(floorRenderer.bounds.size.x * makeMaze.max, floorRenderer.bounds.size.y, floorRenderer.bounds.size.z * makeMaze.max);
    }

    void SetSpawnPosition()
    {
        float x = 0;
        float z = 0;

        for(int i=0; i< Mathf.RoundToInt(floorSize.z / placementInterval); i++)
        {
            x = 0;
            for(int j=0; j< Mathf.RoundToInt(floorSize.x / placementInterval); j++)
            {
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

    void CreateCoins()
    {
        foreach(Vector3 position in positions)
        {
            var coinObj = Instantiate(coinPrefab, new Vector3(position.x, position.y + coinPositionY, position.z), Quaternion.Euler(0, 0, 0));
            coinObj.transform.parent = this.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
