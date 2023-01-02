using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonCtrl : MonoBehaviour
{
    [SerializeField] PlayerCtrl player;
    [SerializeField] RotatingSun sun;
    [SerializeField] Button  button;
    public int count;
    [SerializeField] GameObject Effect;

    // Start is called before the first frame update
    void Start()
    {
        button.image.color = new Color32(255, 255, 255, 100);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TimeClick()
    {
        if(count > 0)
        {
            //Debug.Log("TimeBack押された");
            HandCoinCtrl.instance.UseMoney(75);
            count--;
            Effect.gameObject.SetActive(true);
            sun.SunBack(90.0f);
            if(count == 0)
            {
                button.image.color = new Color32(255, 255, 255, 100);
            }
        } else
        {
            button.image.color = new Color32(255, 255, 255, 100);
        }
    }

    public void SpeedClick()
    {
        if(count > 0)
        {
            //Debug.Log("Speed押された");
            HandCoinCtrl.instance.UseMoney(50);
            player.SpeedUp(3.0f);
            count--;
            if(count == 0)
            {
                button.image.color = new Color32(255, 255, 255, 100);
            }
        } else
        {
            button.image.color = new Color32(255, 255, 255, 100);
        }
    }

    public void DistanceClick()
    {
        if(count > 0)
        {
            //Debug.Log("Distance押された");
            HandCoinCtrl.instance.UseMoney(25);
            Effect.gameObject.SetActive(true);
        } else
        {
            button.image.color = new Color32(255, 255, 255, 100);
        }
    }

    public void MapClick()
    {
        if(count > 0)
        {
            ///Debug.Log("Map押された");
            HandCoinCtrl.instance.UseMoney(100);
            Effect.gameObject.SetActive(true);
            if(count == 0)
            {
                button.image.color = new Color32(255, 255, 255, 100);
            }
        } else
        {
            button.image.color = new Color32(255, 255, 255, 100);
        }
    }

    public void ButtonAppear(){
        // ボタン有効化
        // ボタンの表示
        if(count > 0) {
            button.interactable = true;
            button.image.color = new Color32(255, 255, 255, 255);
        }
    }

    public void ButtonHide(){
        // ボタン無効化
        // ボタンの非表示
        button.interactable = false;
        button.image.color = new Color32(255, 255, 255, 100);
    }

    public void DistanceCount(int num)
    {
        count = num;
        button.image.color = new Color32(255, 255, 255, 100);
    }
}
