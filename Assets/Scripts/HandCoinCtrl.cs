using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandCoinCtrl : MonoBehaviour
{
    [SerializeField] private int stageNo;

    [SerializeField] private Button distanceButton;
    [SerializeField] bool disButtonEnable;
    [SerializeField] ButtonCtrl timeCtrl;
    [SerializeField] ButtonCtrl speedCtrl;
    [SerializeField] ButtonCtrl distanceCtrl;
    [SerializeField] ButtonCtrl mapCtrl;
    [SerializeField] Text handcoinText;
    public int CoinNum = 0;

    // Start is called before the first frame update
    void Start()
    {
        if(stageNo < 5)
        {
            disButtonEnable = false;
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
        handcoinText.text = "HandCoin:" + CoinNum.ToString("D3") + "枚";
    }

    public void GetMoney(int CoinValue)
    {
        CoinNum += CoinValue;
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
                distanceCtrl.ButtonAppear();
                mapCtrl.ButtonHide();
                break;
            case < 75:
                timeCtrl.ButtonHide();
                speedCtrl.ButtonAppear();
                distanceCtrl.ButtonAppear();
                mapCtrl.ButtonHide();
                break;
            case < 100:
                timeCtrl.ButtonAppear();
                speedCtrl.ButtonAppear();
                distanceCtrl.ButtonAppear();
                mapCtrl.ButtonHide();
                break;
            default:
                timeCtrl.ButtonAppear();
                speedCtrl.ButtonAppear();
                distanceCtrl.ButtonAppear();
                mapCtrl.ButtonAppear();
                break;
        }
    }
}
