using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingClock : MonoBehaviour
{
    [SerializeField] GameObject LongHand;
    [SerializeField] GameObject ShortHand;
    [SerializeField] RotatingSun rotatingSun;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ShortHand.transform.eulerAngles = new Vector3(0, 0, -(rotatingSun.rottmp + 90) * 2);
        LongHand.transform.eulerAngles = new Vector3(0, 0, -rotatingSun.rottmp * 24);
    }
}