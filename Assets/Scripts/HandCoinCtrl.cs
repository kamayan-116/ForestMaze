using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// コイン数の管理を行うプログラム
public class HandCoinCtrl : MonoBehaviour
{
    [SerializeField] private Button distanceButton;
    [SerializeField] bool disButtonEnable;  // Distanceボタンが使えるか否か
    [SerializeField] ButtonCtrl timeCtrl;
    [SerializeField] ButtonCtrl speedCtrl;
    [SerializeField] ButtonCtrl distanceCtrl;
    [SerializeField] ButtonCtrl mapCtrl;
    [SerializeField] Text handcoinText;
    public int CoinNum = 0;  // 現在の所持コイン数
    public int stageCoinNum = 0;  // ステージの合計獲得コイン数
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

    // Update is called once per frame
    void Update()
    {
        // 現在のコイン数を表示
        handcoinText.text = "：" + CoinNum.ToString("D3");
    }

    // コインをGetした際に呼ばれる関数
    public void GetMoney(int CoinValue)
    {
        CoinNum += CoinValue;
        stageCoinNum += CoinValue;
        ButtonManagement();
    }

    // コインを使用（ボタンをクリック）した際に呼ばれる関数
    public void UseMoney(int UseCoin)
    {
        CoinNum -= UseCoin;
        ButtonManagement();
    }

    // 現在の所持コイン数に応じてボタンの表示非表示を管理する関数
    public void ButtonManagement()
    {
        switch(CoinNum){
            case < 25:
                timeCtrl.ButtonHide();
                speedCtrl.ButtonHide();
                distanceCtrl.ButtonHide();
                mapCtrl.ButtonHide();
                break;
            case < 50:
                timeCtrl.ButtonHide();
                speedCtrl.ButtonHide();
                if(disButtonEnable) distanceCtrl.ButtonAppear();
                mapCtrl.ButtonHide();
                break;
            case < 75:
                timeCtrl.ButtonHide();
                speedCtrl.ButtonAppear();
                if(disButtonEnable) distanceCtrl.ButtonAppear();
                mapCtrl.ButtonHide();
                break;
            case < 100:
                timeCtrl.ButtonAppear();
                speedCtrl.ButtonAppear();
                if(disButtonEnable) distanceCtrl.ButtonAppear();
                mapCtrl.ButtonHide();
                break;
            default:
                timeCtrl.ButtonAppear();
                speedCtrl.ButtonAppear();
                if(disButtonEnable) distanceCtrl.ButtonAppear();
                mapCtrl.ButtonAppear();
                break;
        }
    }
}
