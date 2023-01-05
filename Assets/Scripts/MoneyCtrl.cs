using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// コインやコインバーに関するプログラム
public class MoneyCtrl : MonoBehaviour
{
    float speed = 100f;  // コインの回転スピード
    bool isGet = false;  // コインをゲットしたか否か
    float lifetime = 0.5f;  // Getしてから消えるまでの時間
    GameObject HandCoinText;
    [SerializeField] int coinValue;  // 各コインの価値
    
    // Start is called before the first frame update
    void Start()
    {
        HandCoinText = GameObject.Find("HandCoinText");
    }

    // Update is called once per frame
    void Update()
    {
        // Getしたら変数speedの10倍で回転し、lifetime後に消す
        if(isGet)
        {
            transform.Rotate(Vector3.up * speed * 10f * Time.deltaTime, Space.World);

            lifetime -= Time.deltaTime;

            if(lifetime <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!isGet && other.CompareTag("Player"))
        {
            isGet = true;
            transform.position += Vector3.up * 1.5f;
            HandCoinText.GetComponent<HandCoinCtrl>().GetMoney(coinValue);
        }        
    }

    // コインがカメラに映っている際回転させるプログラム
    private void OnWillRenderObject()
    {
        if(Camera.current.tag == "MainCamera" && this.gameObject.tag == "Coin")
        {
            transform.Rotate(Vector3.up * speed * Time.deltaTime, Space.World);
        }   
    }
}
