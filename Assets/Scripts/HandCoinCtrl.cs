using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandCoinCtrl : MonoBehaviour
{
    [SerializeField] private Button distanceButton;
    [SerializeField] bool disButtonEnable;
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
        handcoinText.text = "：" + CoinNum.ToString("D3");
    }

    public void GetMoney(int CoinValue)
    {
        CoinNum += CoinValue;
        stageCoinNum += CoinValue;
        ButtonManagement();
    }

    public void UseMoney(int UseCoin)
    {
        CoinNum -= UseCoin;
        ButtonManagement();
    }

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
