using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// プレイヤーと歩く犬の距離を測るプログラム
public class DistanceCtrl : MonoBehaviour
{
    [SerializeField] GameObject player;  // プレイヤーオブジェクト
    [SerializeField] GameObject dog;  // 歩く犬のオブジェクト
    Vector3 playerPos;  // プレイヤーの座標
    Vector3 dogPos;  // 犬の座標
    float distance;  //　プレイヤーと歩く犬との距離
    [SerializeField] Text disText;
    float time = 0.0f; // 距離のテキストを表示している時間
    [SerializeField] float timeLimit; // 距離のテキストを表示する最大時間

    // テキストが表示されるごとにtimeが0に初期化
    void OnEnable()
    {
        time = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        playerPos = player.transform.position;
        dogPos = dog.transform.position;
        // プレイヤーと犬の距離をdistanceに代入
        distance = Vector3.Distance(playerPos, dogPos);

        // 最大表示時間を過ぎるとテキストを非表示
        if(time > timeLimit)
        {
            gameObject.SetActive(false);
        } else
        {
            time += Time.deltaTime;
            // Debug.Log("Distime=" + time);
            disText.text = "犬との距離は" + distance.ToString("f2") + "m";
        }
    }
}
