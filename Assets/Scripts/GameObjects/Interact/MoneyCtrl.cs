using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// コインやコインバーに関するプログラム
public class MoneyCtrl : MonoBehaviour, IInteractive
{
    GameObject player;  // プレイヤーオブジェクト
    Vector3 playerPos;  // プレイヤーの座標
    float distance;  //　プレイヤーとコインとの距離
    [SerializeField] float actionDistance;  // 回転やコライダーができる距離
    float speed = 100f;  // コインの回転スピード
    float getSpeed = 10f;  // 獲得時の回転倍率
    bool isRotate;  // コインが回転できるか否か
    bool isGet = false;  // コインをゲットしたか否か
    float lifetime = 0.5f;  // Getしてから消えるまでの時間
    GameObject handCoinText;
    [SerializeField] int coinValue;  // 各コインの価値
    
    // Start is called before the first frame update
    void Start()
    {
        handCoinText = GameObject.Find("HandCoinText");
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        playerPos = player.transform.position;
        // プレイヤーとコインの距離をdistanceに代入
        distance = Vector3.Distance(playerPos, this.transform.position);

        if(distance < actionDistance)
        {
            isRotate = true;
            this.GetComponent<MeshRenderer>().enabled = true;
            this.GetComponent<SphereCollider>().enabled = true;
        }
        else
        {
            isRotate = false;
            this.GetComponent<MeshRenderer>().enabled = false;
            this.GetComponent<SphereCollider>().enabled = false;
        }

        // Getしたら変数speedの10倍で回転し、lifetime後に消す
        if(isGet)
        {
            transform.Rotate(Vector3.up * speed * getSpeed * Time.deltaTime, Space.World);

            lifetime -= Time.deltaTime;

            if(lifetime <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public void Interact()
    {
        if(!isGet)
        {
            isGet = true;
            transform.position += Vector3.up * 1.5f;
            GameManager.instance.GetMoney(coinValue);
        }
    }
    
    // コインがカメラに映っている際回転させるプログラム
    private void OnWillRenderObject()
    {
        if(Camera.current.tag == "MainCamera" && this.gameObject.tag == "Coin" && isRotate)
        {
            transform.Rotate(Vector3.up * speed * Time.deltaTime, Space.World);
        }   
    }
}
