using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Gameの進行管理プログラム
public class GameManager : MonoBehaviour
{
    public float rottmp = 0;  // ゲームの経過時間
    [SerializeField] private float rotateSpeed = 1.0f;  // 回転スピード
    bool isBack = false;  // ライトが戻るか否か(TimeBack)
    float startBack;  // 戻る際の初期時間
    float finishBack; // 戻る際の終了時間
    /// <summary>
    /// Map表示時はfalseで時間停止
    /// </summary>
    public bool moveClock = true;
    /// <summary>
    /// 現在の所持コイン数
    /// </summary>
    public int coinNum = 0;
    /// <summary>
    /// ステージの合計獲得コイン数
    /// </summary>
    public int stageCoinNum = 0;
    /// <summary>
    /// 捕まえた犬の数
    /// </summary>
    public int captureDog = 0;
    [SerializeField] int resultNum;  // ゲーム結果の番号（0はクリア,1はGameOver）
    [SerializeField] PlayerCtrl playerCtrl;  // プレイヤーのスクリプト
    [SerializeField] RotatingSun rotatingSun;  // Lightのスクリプト

    public static GameManager instance;
    public static GameManager Instance {get => instance;}

    private void Awake()
    {
        instance = this.GetComponent<GameManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!isBack && moveClock)
        {
            rottmp += rotateSpeed * Time.deltaTime;
        }
        else if(isBack && finishBack < rottmp)
        {
            rottmp -= rotateSpeed * Time.deltaTime * 20.0f;
            if(rottmp <= 0)
            {
                rottmp = 0;
                isBack = false;
                moveClock = true;
            }
        }
        else
        {
            isBack = false;
        }

        rottmp %= 360.0f;
        rotatingSun.MoveLight(rottmp);

        // rottmpが195を超えるとGameOver
        if(rottmp > 195.0f)
        {
            GameManager.instance.SetGameResult(1);
        }
    }

    /// <summary>
    /// TimeBackの処理
    /// </summary>
    /// <param name="back">戻す時間</param>
    public void SunBack(float back)
    {
        isBack = true;
        startBack = rottmp;
        finishBack = rottmp - back;
        if(finishBack <= 0)
        {
            finishBack = 0;
        }
    }

    /// <summary>
    /// 捕まえた犬の数を増やす関数
    /// </summary>
    public void SetCaptureDog()
    {
        captureDog++;
        // 捕まえた犬の数がgoalConditionと同じになれば金の鍵を具現化
        if(captureDog >= MakeMaze.instance.goalCondition)
        {
            playerCtrl.SetActiveKey(4);
        }
    }

    /// <summary>
    /// コインをGetした際に呼ばれる関数
    /// </summary>
    /// <param name="_coinValue">1コインの価値</param>
    public void GetMoney(int _coinValue)
    {
        coinNum += _coinValue;
        stageCoinNum += _coinValue;
        CanvasManager.instance.SetValueCoin(coinNum);
        CanvasManager.instance.ButtonManagement(coinNum);
    }

    /// <summary>
    /// コインを使用（ボタンをクリック）した際に呼ばれる関数
    /// </summary>
    /// <param name="_useCoin">使うコインの枚数</param>
    public void UseMoney(int _useCoin)
    {
        coinNum -= _useCoin;
        CanvasManager.instance.SetValueCoin(coinNum);
        CanvasManager.instance.ButtonManagement(coinNum);
    }

    // ゲーム結果に応じてシーンチェンジする関数
    public void SetGameResult(int _result)
    {
        resultNum = _result;
        SceneManager.sceneLoaded += GameSceneLoaded;
        SceneManager.LoadScene("NonGameScene");
    }

    // SceneChangeする際に、呼ばれる関数
    public void GameSceneLoaded(Scene nongame, LoadSceneMode mode)
    {
        // シーンチェンジ先のCanvas内のNonGameCanvasCtrlスクリプトのResultPanel関数を呼ぶ
        var canvasManager = GameObject.Find("Canvas").GetComponent<NonGameCanvasCtrl>();

        canvasManager.ResultPanel(resultNum, GameManager.instance.stageCoinNum, 195.0f - rottmp, GameManager.instance.coinNum, CanvasManager.instance.countPush[2] + 1);
        SceneManager.sceneLoaded -= GameSceneLoaded;
    }
}