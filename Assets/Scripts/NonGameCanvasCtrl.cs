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
    [SerializeField] Sprite[] stageButtonImage;
    [SerializeField] Text resultText;
    [SerializeField] Text resultCoinText;
    [SerializeField] Text scoreText;
    [SerializeField] Text highScoreText;
    [SerializeField] Text stageCoinText;
    [SerializeField] GameObject newRecord;
    public int stageNo;
    public int clearStageNo;
    private int stageGetCoinNum;
    [SerializeField] private int totalGetCoinNum;
    private int score = 0;
    private int highScore;

    public static NonGameCanvasCtrl instance;
    public static NonGameCanvasCtrl Instance {get => instance;}

    private void Awake()
    {
        instance = this.GetComponent<NonGameCanvasCtrl>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
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
        PlayerPrefs.DeleteKey("Clear");
        PlayerPrefs.DeleteKey("TotalCoin");
        AblePushStageSelect();
    }

    // ステージ選択ボタンの可否
    public void AblePushStageSelect()
    {
        //どのステージまでクリアしているのかをロード（セーブされていなければ「0」）
        clearStageNo = PlayerPrefs.GetInt("Clear", 0);
        
        for(int i = 0; i <= stageButton.GetUpperBound(0); i++)
        {
            bool buttonEnable;

            if(clearStageNo < i)
            {
                buttonEnable = false;
                stageButton[i].GetComponent<Image>().sprite = stageButtonImage[1];
            }
            else
            {
                buttonEnable = true;
                stageButton[i].GetComponent<Image>().sprite = stageButtonImage[0];
            }

            stageButton[i].GetComponent<Button>().interactable = buttonEnable;
        }

        if(totalGetCoinNum > 1200 && clearStageNo == 5)
        {
            stageButton[5].GetComponent<Button>().interactable = true;
        }
        else
        {
            stageButton[5].GetComponent<Button>().interactable = false;
        }
    }

    // ステージ選択ボタンを押した
    public void PushStageSelectButton(int _stageNo)
    {
        stageNo = _stageNo;
        PlayerPrefs.SetInt("Stage", stageNo);
        PlayerPrefs.Save();
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
    public void ResultPanel(int  _result, int _getCoinNum, float _clearTime, int _remaingCoin, int _timePush)
    {
        titlePanel.SetActive(false);
        resultPanel.SetActive(true);
        
        stageGetCoinNum = _getCoinNum;

        totalGetCoinNum = PlayerPrefs.GetInt("TotalCoin", 0);
        totalGetCoinNum += _getCoinNum;
        PlayerPrefs.SetInt("TotalCoin", totalGetCoinNum);

        stageNo = PlayerPrefs.GetInt("Stage", 0);
        clearStageNo = PlayerPrefs.GetInt("Clear", 0);

        if(_result == 0)
        {
            resultText.text = "Game Clear";
            mainCamera.backgroundColor = new Color32(210, 210, 210, 255);
            if(clearStageNo < stageNo)
            {
                clearStageNo = stageNo;
                PlayerPrefs.SetInt("Clear", clearStageNo);
            }
        }
        else if(_result == 1)
        {
            resultText.text = "Game Over";
            mainCamera.backgroundColor = new Color32(0, 0, 0, 255);
        }
        
        score = (int)(_clearTime * 2.5f * _timePush) + _remaingCoin * 5;

        highScore = PlayerPrefs.GetInt("Score" + stageNo, 0);
        if(score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("Score" + stageNo, highScore);
            newRecord.SetActive(true);
        }

        PlayerPrefs.Save();

        resultCoinText.text = "：" + stageGetCoinNum.ToString("D3") + "枚";
        scoreText.text = "Score：" + score.ToString("D5");
        highScoreText.text = "HighScore：" + highScore.ToString("D5");
        stageCoinText.text = "：" + totalGetCoinNum.ToString("D3") + "枚";

        // Debug.Log("total：" + totalGetCoinNum);
        // Debug.Log("clear：" + clearStageNo);
    }

    // 結果画面でステージ選択に戻るボタンを押した
    public void BackToStageSelect()
    {
        resultPanel.SetActive(false);
        stageSelectPanel.SetActive(true);
        mainCamera.backgroundColor = new Color32(67, 164, 87, 255);
        AblePushStageSelect();
    }
}
