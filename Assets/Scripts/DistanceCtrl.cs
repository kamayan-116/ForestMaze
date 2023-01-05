using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// プレイヤーと歩く犬の距離を測るプログラム
public class DistanceCtrl : MonoBehaviour
{
    [SerializeField] GameObject Player;
    [SerializeField] GameObject Dog;
    Vector3 PlayerPos;  // プレイヤーの座標
    Vector3 DogPos;  // 犬の座標
    float distance;  //　プレイヤーと犬の距離
    [SerializeField] Text disText;
    float time = 0.0f; // 距離のテキストを表示している時間
    [SerializeField] float TimeLimit; // 距離のテキストを表示する最大時間

    // テキストが表示されるごとにtimeが0に初期化
    void OnEnable()
    {
        time = 0.0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayerPos = Player.transform.position;
        DogPos = Dog.transform.position;
        // プレイヤーと犬の距離をdistanceに代入
        distance = Vector3.Distance(PlayerPos, DogPos);

        // 最大表示時間を過ぎるとテキストを非表示
        if(time > TimeLimit)
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
