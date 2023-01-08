using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// コイン数の管理を行うプログラム
public class HandCoinCtrl : MonoBehaviour
{
    [SerializeField] private Button distanceButton;
    [SerializeField] bool disButtonEnable;  // Distanceボタンが使えるか否か
    [SerializeField] ButtonCtrl[] buttonCtrls = new ButtonCtrl[4];  // 各ボタンを入れた配列
    [SerializeField] Text handcoinText;
    /// <summary>
    /// 現在の所持コイン数
    /// </summary>
    public int coinNum = 0;
    /// <summary>
    /// ステージの合計獲得コイン数
    /// </summary>
    public int stageCoinNum = 0;

    public static HandCoinCtrl instance;
    public static HandCoinCtrl Instance {get => instance;}

    private void Awake()
    {
        instance = this.GetComponent<HandCoinCtrl>();
    }

    // Start is called before the first frame update
    void Start()
    {
        ButtonManagement();
        // ステージが5未満の際はDistanceボタンを使えない
        if(NonGameCanvasCtrl.Instance.stageNo < 5)
        {
            disButtonEnable = false;
            distanceButton.image.color = new Color32(255, 255, 255, 100);
        }
        else
        {
            disButtonEnable = true;
        }
        distanceButton.interactable = disButtonEnable;
    }

    /// <summary>
    /// コインをGetした際に呼ばれる関数
    /// </summary>
    /// <param name="CoinValue">1コインの価値</param>
    public void GetMoney(int CoinValue)
    {
        coinNum += CoinValue;
        stageCoinNum += CoinValue;
        // 現在のコイン数を表示
        handcoinText.text = "：" + coinNum.ToString("D3");
        ButtonManagement();
    }

    /// <summary>
    /// コインを使用（ボタンをクリック）した際に呼ばれる関数
    /// </summary>
    /// <param name="UseCoin">使うコインの枚数</param>
    public void UseMoney(int UseCoin)
    {
        coinNum -= UseCoin;
        // 現在のコイン数を表示
        handcoinText.text = "：" + coinNum.ToString("D3");
        ButtonManagement();
    }

    // 現在の所持コイン数に応じてボタンの表示非表示を管理する関数
    private void ButtonManagement()
    {
        foreach (ButtonCtrl buttonCtrl in buttonCtrls)
        {
            if(coinNum > buttonCtrl.pushableCoinNum)
            {
                buttonCtrl.ButtonAppear();
            }
            else
            {
                buttonCtrl.ButtonHide();
            }
        }
    }
}
