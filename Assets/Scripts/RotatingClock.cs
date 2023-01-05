using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// UIの時計に関するプログラム
public class RotatingClock : MonoBehaviour
{
    [SerializeField] GameObject LongHand;  // 長針
    [SerializeField] GameObject ShortHand;  // 短針
    [SerializeField] RotatingSun rotatingSun;

    // Update is called once per frame
    void Update()
    {
        // Lightの角度に合わせてそれぞれの針を回転
        // Lightが真上の時をAM0:00、真下の時をPM0:00で合うように針を合わせる
        ShortHand.transform.eulerAngles = new Vector3(0, 0, -(rotatingSun.rottmp + 90) * 2);
        LongHand.transform.eulerAngles = new Vector3(0, 0, -rotatingSun.rottmp * 24);
    }
}