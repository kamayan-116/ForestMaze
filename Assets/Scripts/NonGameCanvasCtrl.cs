using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// NonGameSceneのUI管理プログラム
public class NonGameCanvasCtrl : MonoBehaviour
{
    #region 
    [SerializeField] Camera mainCamera;  // カメラオブジェクト
    [SerializeField] GameObject titlePanel;  // タイトルUI
    [SerializeField] GameObject stageSelectPanel;  // ステージセレクトUI
    [SerializeField] GameObject rulePanel;  // ルールUI
    [SerializeField] GameObject resultPanel;  // ゲーム結果UI
    [SerializeField] GameObject[] stageButton;  // ステージセレクトでのボタンUI
    [SerializeField] Sprite[] stageButtonImage;  // ステージセレクトでのボタンの画像配列
    [SerializeField] Text resultText;  // ゲーム結果を表示するテキスト
    [SerializeField] Text resultCoinText;  // ゲーム結果での獲得コインを表示するテキスト
    [SerializeField] Text scoreText;  // ゲーム結果でのスコアを表示するテキスト
    [SerializeField] Text highScoreText;  // ゲーム結果でのハイスコアを表示するテキスト
    [SerializeField] Text stageCoinText;  // ステージセレクトでの累計獲得コインを表示するテキスト
    [SerializeField] GameObject newRecord;  // ハイスコア更新した際のnew Recordオブジェクト
    #endregion
    public int stageNo;  // 現在プレイするステージ
    public int clearStageNo;  // 現在クリアしている最高ステージ
    private int stageGetCoinNum;  // 現在プレイしたステージでの獲得コイン
    [SerializeField] private int totalGetCoinNum;  // 累計獲得コイン
    private int score = 0;  // 現在のステージでのスコア
    private int highScore;  // 現在のステージでのハイスコア

    public static NonGameCanvasCtrl instance;
    public static NonGameCanvasCtrl Instance {get => instance;}

    private void Awake()
    {
        instance = this.GetComponent<NonGameCanvasCtrl>();
    }

    // タイトルのスタートボタンを押した
    public void GotoStageSelect()
    {
        // タイトルUIを非表示にし、ステージセレクトUIを表示
        titlePanel.SetActive(false);
        stageSelectPanel.SetActive(true);
        // ステージの背景色を緑色に変更
        mainCamera.backgroundColor = new Color32(67, 164, 87, 255);
        // ClearとTotalCoinの保存を初期化
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
            bool buttonEnable;  // ボタンが使えるか否か

            // 現在のクリアステージより1つ大きな数字までしかボタンの有効化を行わないようにする
            if(clearStageNo < i)
            {
                buttonEnable = false;
                // 無効化の場合画像を鍵の画像にする
                stageButton[i].GetComponent<Image>().sprite = stageButtonImage[1];
            }
            else
            {
                buttonEnable = true;
                stageButton[i].GetComponent<Image>().sprite = stageButtonImage[0];
            }

            // ボタンの有効化
            stageButton[i].GetComponent<Button>().interactable = buttonEnable;
        }

        // ステージ5までクリアし、累計獲得コインが1200を超えるとステージ6を有効化
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
        // stageNoをStageに保存
        PlayerPrefs.SetInt("Stage", stageNo);
        PlayerPrefs.Save();
        // ステージセレクトUIを非表示にし、ルールUIを表示
        stageSelectPanel.SetActive(false);
        rulePanel.SetActive(true);
    }

    // ゲームスタートボタンを押した
    public void GotoGame()
    {
        // ルールUIを非表示にし、GameSceneにシーンチェンジし、ゲームスタート
        rulePanel.SetActive(false);
        SceneManager.LoadScene("GameScene");
    }

    // 結果を表示する関数
    public void ResultPanel(int  _result, int _getCoinNum, float _clearTime, int _remaingCoin, int _timePush)
    {
        // タイトルUIを非表示にし、結果UIを表示
        titlePanel.SetActive(false);
        resultPanel.SetActive(true);
        
        // 引数の_getCoinNumをstageGetCoinNumに代入
        stageGetCoinNum = _getCoinNum;

        // 累計獲得コインをロード（セーブされていなければ「0」）
        totalGetCoinNum = PlayerPrefs.GetInt("TotalCoin", 0);
        // 引数の_getCoinNumをtotalGetCoinNumに代入し、TotalCoinに保存
        totalGetCoinNum += _getCoinNum;
        PlayerPrefs.SetInt("TotalCoin", totalGetCoinNum);

        // ステージレベルとクリアステージレベルをロード（セーブされていなければ「0」）
        stageNo = PlayerPrefs.GetInt("Stage", 0);
        clearStageNo = PlayerPrefs.GetInt("Clear", 0);

        if(_result == 0)
        {
            // ゲームクリア（背景を白）
            resultText.text = "Game Clear";
            mainCamera.backgroundColor = new Color32(210, 210, 210, 255);
            // クリアステージレベルが更新されれば、現在のステージレベルをクリアステージレベルに代入し保存
            if(clearStageNo < stageNo)
            {
                clearStageNo = stageNo;
                PlayerPrefs.SetInt("Clear", clearStageNo);
            }
        }
        else if(_result == 1)
        {
            // ゲームオーバー（背景を黒）
            resultText.text = "Game Over";
            mainCamera.backgroundColor = new Color32(0, 0, 0, 255);
        }
        // スコア計算
        score = (int)(_clearTime * 2.5f * _timePush) + _remaingCoin * 5;

        // ステージレベル毎のハイスコアをロード（セーブされていなければ「0」）
        highScore = PlayerPrefs.GetInt("Score" + stageNo, 0);

        if(score > highScore)
        {
            // ハイスコアが更新されれば、そのスコアを保存し、new Recordオブジェクトを出現
            highScore = score;
            PlayerPrefs.SetInt("Score" + stageNo, highScore);
            newRecord.SetActive(true);
        }

        PlayerPrefs.Save();

        //　各テキストを更新して表示
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
        // 結果UIを非表示にし、ステージレベルUIを表示（背景を緑色に変更）
        resultPanel.SetActive(false);
        stageSelectPanel.SetActive(true);
        mainCamera.backgroundColor = new Color32(67, 164, 87, 255);
        AblePushStageSelect();
    }
}
