using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

// Gameの進行管理プログラム
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// プレイヤーのカメラが操作できるか否か
    /// </summary>
    public bool isPlayerOpe;  //  trueはPlayer操作、falseはマウス操作
    public float playTime = 0;  // ゲームの経過時間
    [SerializeField] private float playSpeed = 1.0f;  // プレイスピード
    private bool isBack = false;  // ライトが戻るか否か(TimeBack)
    private float startBack;  // 戻る際の初期時間
    private float finishBack; // 戻る際の終了時間
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
    [SerializeField] private int resultNum;  // ゲーム結果の番号（0はクリア,1はGameOver）
    [SerializeField] private CinemachineBrain mainCinema;
    [SerializeField] private PlayerCtrl playerCtrl;  // プレイヤーのスクリプト
    [SerializeField] private RotatingSun rotatingSun;  // Lightのスクリプト

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
        // Eキーを押すとSwitch関数の実行
        if(Input.GetKeyDown(KeyCode.E))
        {
            Switch();
        }

        // プレイ状況に応じた使用状況の変化
        SetCinema();
        SetCursor();

        // ゲーム時間の進行
        if(!isBack && moveClock)
        {
            playTime += playSpeed * Time.deltaTime;
        }
        else if(isBack && finishBack < playTime)
        {
            playTime -= playSpeed * Time.deltaTime * 20.0f;
            if(playTime <= 0)
            {
                playTime = 0;
                isBack = false;
                moveClock = true;
            }
        }
        else
        {
            isBack = false;
        }
        
        rotatingSun.MoveLight(playTime);

        // playTimeが195を超えるとGameOver
        if(playTime > 195.0f)
        {
            SetGameResult(1);
        }
    }

    // プレイヤーとマウスの操作の変更を行う関数
    private void Switch()
    {
        isPlayerOpe = !isPlayerOpe;
    }

    // CinemaChineの使用可否
    private void SetCinema()
    {
        mainCinema.enabled = isPlayerOpe;
    }

    // マウスカーソルの有無
    private void SetCursor()
    {
        Cursor.visible = !isPlayerOpe;
    }

    /// <summary>
    /// TimeBackの処理
    /// </summary>
    /// <param name="back">戻す時間</param>
    public void SunBack(float back)
    {
        isBack = true;
        startBack = playTime;
        finishBack = playTime - back;
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
        if(captureDog >= MakeMaze.Instance.goalCondition)
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
        CanvasManager.Instance.SetValueCoin(coinNum);
        CanvasManager.Instance.ButtonManagement(coinNum);
    }

    /// <summary>
    /// コインを使用（ボタンをクリック）した際に呼ばれる関数
    /// </summary>
    /// <param name="_useCoin">使うコインの枚数</param>
    public void UseMoney(int _useCoin)
    {
        coinNum -= _useCoin;
        CanvasManager.Instance.SetValueCoin(coinNum);
        CanvasManager.Instance.ButtonManagement(coinNum);
    }

    /// <summary>
    /// ゲーム結果に応じてシーンチェンジする関数
    /// </summary>
    /// <param name="_result">ゲームの結果の番号</param>
    public void SetGameResult(int _result)
    {
        resultNum = _result;
        SceneManager.sceneLoaded += GameSceneLoaded;
        SceneManager.LoadScene("NonGameScene");
    }

    // SceneChangeする際に、呼ばれる関数
    public void GameSceneLoaded(Scene nongame, LoadSceneMode mode)
    {
        // シーンチェンジ先のNonGameCanvasCtrlスクリプトのResultPanel関数を呼ぶ
        NonGameCanvasManager.Instance.ResultPanel(resultNum, stageCoinNum, 195.0f - playTime, coinNum, CanvasManager.Instance.countPush[2] + 1);
        SceneManager.sceneLoaded -= GameSceneLoaded;
    }
}