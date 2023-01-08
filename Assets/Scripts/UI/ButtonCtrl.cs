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
    /// <summary>
    /// ボタンを押せる条件や押した際に使うコイン数
    /// </summary>
    public int pushableCoinNum;
    /// <summary>
    /// ボタンを押せる回数
    /// </summary>
    public int count;
    [SerializeField] GameObject effect;  // 各ボタンを押した際に表示されるもの（Mapや犬との距離など）

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
            HandCoinCtrl.instance.UseMoney(pushableCoinNum);
            count--;
            effect.gameObject.SetActive(true);
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
            HandCoinCtrl.instance.UseMoney(pushableCoinNum);
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
            HandCoinCtrl.instance.UseMoney(pushableCoinNum);
            effect.gameObject.SetActive(true);
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
            HandCoinCtrl.instance.UseMoney(pushableCoinNum);
            effect.gameObject.SetActive(true);
        } else
        {
            button.image.color = new Color32(255, 255, 255, 100);
        }
    }
    #endregion

    /// <summary>
    /// ボタン有効化と表示
    /// </summary>
    public void ButtonAppear(){
        if(count > 0) {
            button.interactable = true;
            button.image.color = new Color32(255, 255, 255, 255);
        }
    }

    /// <summary>
    /// ボタン無効化と非表示
    /// </summary>
    public void ButtonHide(){
        button.interactable = false;
        button.image.color = new Color32(255, 255, 255, 100);
    }
}
