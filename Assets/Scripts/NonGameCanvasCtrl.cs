using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NonGameCanvasCtrl : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] GameObject titlePanel;
    [SerializeField] GameObject stageSelectPanel;
    [SerializeField] GameObject rulePanel;
    [SerializeField] GameObject resultPanel;
    [SerializeField] GameObject[] stageButton;
    [SerializeField] Text resultText;
    public int stageNo;
    public int clear;

    public static NonGameCanvasCtrl instance;
    public static NonGameCanvasCtrl Instance {get => instance;}

    private void Awake()
    {
        instance = this.GetComponent<NonGameCanvasCtrl>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //どのステージまでクリアしているのかをロード（セーブされていなければ「0」）
        // int clearStageNo = PlayerPrefs.GetInt("Clear", 0); 

        // for(int i = 0; i <= stageButton.GetUpperBound(0); i++)
        // {
        //     bool buttonEnable;

        //     if(clearStageNo < i)
        //     {
        //         buttonEnable = false;
        //     }
        //     else
        //     {
        //         buttonEnable = true;
        //     }

        //     stageButton[i].GetComponent<Button>().interactable = buttonEnable;
        // }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // タイトルのスタートボタンを押した
    public void GotoStageSelect()
    {
        titlePanel.SetActive(false);
        stageSelectPanel.SetActive(true);
        mainCamera.backgroundColor = new Color32(67, 164, 87, 255);
    }

    // ステージ選択ボタンを押した
    public void PushStageSelectButton(int _stageNo)
    {
        stageNo = _stageNo;
        stageSelectPanel.SetActive(false);
        rulePanel.SetActive(true);
    }

    // ゲームスタートボタンを押した
    public void GotoGame()
    {
        rulePanel.SetActive(false);
        SceneManager.LoadScene("GameScene");
    }

    // 結果を表示する関数
    public void ResultPanel(int  _result)
    {
        titlePanel.SetActive(false);
        resultPanel.SetActive(true);

        if(_result == 0)
        {
            resultText.text = "Game Clear";
            mainCamera.backgroundColor = new Color32(210, 210, 210, 255);
        }
        else if(_result == 1)
        {
            resultText.text = "Game Over";
            mainCamera.backgroundColor = new Color32(0, 0, 0, 255);
        }
    }

    // 結果画面でタイトルに戻るボタンを押した
    public void BackToTitle()
    {
        resultPanel.SetActive(false);
        titlePanel.SetActive(true);
    }

    // 結果画面でステージ選択に戻るボタンを押した
    public void BackToStageSelect()
    {
        resultPanel.SetActive(false);
        stageSelectPanel.SetActive(true);
        mainCamera.backgroundColor = new Color32(67, 164, 87, 255);
    }
}
