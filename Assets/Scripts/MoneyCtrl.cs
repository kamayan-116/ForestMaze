using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyCtrl : MonoBehaviour
{
    float speed = 100f;
    bool isGet = false;
    float lifetime = 0.5f;
    GameObject HandCoinText;

    // Start is called before the first frame update
    void Start()
    {
        HandCoinText = GameObject.Find("HandCoin");
    }

    // Update is called once per frame
    void Update()
    {
        if(isGet == true)
        {
            transform.Rotate(Vector3.up * speed * 10f * Time.deltaTime, Space.World);

            lifetime -= Time.deltaTime;

            if(lifetime <= 0)
            {
                Destroy(gameObject);
            }
        } else
        {
            if(this.gameObject.tag == "Coin")
            {
                transform.Rotate(Vector3.up * speed * Time.deltaTime, Space.World);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!isGet && other.CompareTag("Player"))
        {
            isGet = true;

            transform.position += Vector3.up * 1.5f;
            if(this.gameObject.tag == "GoldBar")
            {
                // Debug.Log("BarGet");
                HandCoinText.GetComponent<HandCoinCtrl>().GetMoney(20);
            }
            if(this.gameObject.tag == "Coin")
            {
                // Debug.Log("CoinGet");
                HandCoinText.GetComponent<HandCoinCtrl>().GetMoney(1);
            }
        }        
    }
}
