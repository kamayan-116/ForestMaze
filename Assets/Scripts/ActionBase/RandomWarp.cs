using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomWarp : MonoBehaviour ,ActionBase
{
    [SerializeField] GameObject[] WarpOut = new GameObject[10];

    int i;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public  void Action()
    {
        i = Random.Range(0, 10);
        // Debug.Log(i);
        WarpOut[i].GetComponent<Warp>().Action();
    }
}
