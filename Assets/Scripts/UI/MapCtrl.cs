using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Mapの表示に関するプログラム
public class MapCtrl : MonoBehaviour
{
    [SerializeField] RotatingSun rotatingSun;
    [SerializeField] float time = 0.0f;  // Mapの表示時間
    [SerializeField] float timeLimit;  // Mapの最大表示時間

    // Mapが表示されるごとにtimeが0に初期化と時計の動きを止める
    void OnEnable()
    {
        time = 0.0f;
        rotatingSun.moveClock = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(time > timeLimit)
        {
            gameObject.SetActive(false);
            rotatingSun.moveClock = true;
        } else
        {
            time += Time.deltaTime;
        }
    }
}
