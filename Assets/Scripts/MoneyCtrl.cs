using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyCtrl : MonoBehaviour
{
    float speed = 100f;
    bool isGet = false;
    float lifetime = 0.5f;
    GameObject HandCoinText;
    [SerializeField] int coinValue;
    // private bool _isRendered = false;  // カメラに表示されているか

    // Start is called before the first frame update
    void Start()
    {
        HandCoinText = GameObject.Find("HandCoinText");
    }

    // Update is called once per frame
    void Update()
    {
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

    private void OnWillRenderObject()
    {
        if(Camera.current.tag == "MainCamera" && this.gameObject.tag == "Coin")
        {
            transform.Rotate(Vector3.up * speed * Time.deltaTime, Space.World);
        }   
    }
}
