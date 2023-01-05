using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// プレイ画面の各ボタンに関するプログラム
public class ButtonCtrl : MonoBehaviour
{
    [SerializeField] PlayerCtrl player;
    [SerializeField] RotatingSun sun;
    [SerializeField] Button  button;
    public int count;  // ボタンを押せる回数
    [SerializeField] GameObject Effect;  // 各ボタンを押した際に表示されるもの（Mapや犬との距離など）

    // Start is called before the first frame update
    void Start()
    {
        button.image.color = new Color32(255, 255, 255, 100);
    }

    #region  // 各ボタンの処理
    // 左上のTimeBackボタンを押した際に呼ばれる関数
    public void TimeClick()
    {
        if(count > 0)
        {
            //Debug.Log("TimeBack押された");
            HandCoinCtrl.instance.UseMoney(75);
            count--;
            Effect.gameObject.SetActive(true);
            sun.SunBack(90.0f);
        } else
        {
            // ボタンの押せる回数が0になると半透明にする
            button.image.color = new Color32(255, 255, 255, 100);
        }
    }

    // 右上のSpeedボタンを押した際に呼ばれる関数
    public void SpeedClick()
    {
        if(count > 0)
        {
            //Debug.Log("Speed押された");
            HandCoinCtrl.instance.UseMoney(50);
            player.SpeedUp(3.0f);
            count--;
        } else
        {
            button.image.color = new Color32(255, 255, 255, 100);
        }
    }

    // 左下のDistanceボタンを押した際に呼ばれる関数
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

    // 右下のMapボタンを押した際に呼ばれる関数
    public void MapClick()
    {
        if(count > 0)
        {
            ///Debug.Log("Map押された");
            HandCoinCtrl.instance.UseMoney(100);
            Effect.gameObject.SetActive(true);
        } else
        {
            button.image.color = new Color32(255, 255, 255, 100);
        }
    }
    #endregion

    // ボタン有効化と表示
    public void ButtonAppear(){
        if(count > 0) {
            button.interactable = true;
            button.image.color = new Color32(255, 255, 255, 255);
        }
    }

    // ボタン無効化と非表示
    public void ButtonHide(){
        button.interactable = false;
        button.image.color = new Color32(255, 255, 255, 100);
    }

    // Distanceボタンの非表示
    public void DistanceCount(int num)
    {
        count = num;
        button.image.color = new Color32(255, 255, 255, 100);
    }
}
