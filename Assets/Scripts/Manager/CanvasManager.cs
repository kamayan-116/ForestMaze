using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// GameSceneのCanvasに関するプログラム
public class CanvasManager : MonoBehaviour
{
    #region   // パラメータ設定
    [SerializeField] private GameObject player;  // プレイヤーオブジェクト
    [SerializeField] private GameObject dog;  // 歩く犬のオブジェクト
    private Vector3 playerPos;  // プレイヤーの座標
    private Vector3 dogPos;  // 犬の座標
    private float distance;  //　プレイヤーと歩く犬との距離
    [SerializeField] private GameObject longHand;  // 長針
    [SerializeField] private GameObject shortHand;  // 短針
    [SerializeField] private Text handcoinText;  // コインの枚数を表示するテキスト
    [SerializeField] private float upSpeed;  // プレイヤーの1回あたりのアップするスピード
    /// <summary>
    /// 各ボタンを入れた配列(Dis,Speed,Time,Map)
    /// </summary>
    public Button[] buttons = new Button[4];
    [SerializeField] private int[] pushableCoinNum;  // 各ボタンを押せる条件や押した際に使うコイン数
    /// <summary>
    /// 各ボタンを押せる回数
    /// </summary>
    public int[] countPush;
    [SerializeField] private GameObject timeBackObj;  // 時を戻そう
    [SerializeField] private float timeBackLimit; // 時を戻そうを表示する最大時間
    [SerializeField] private GameObject disObj;  // プレイヤーと犬との距離のオブジェクト
    [SerializeField] private Text disText;  // プレイヤーとの距離を表示するテキスト
    [SerializeField] private float disTimeLimit; // 距離のテキストを表示する最大時間
    [SerializeField] private GameObject mapObj;  // Mapのオブジェクト
    [SerializeField] private float mapTimeLimit; // Mapを表示する最大時間
    #endregion

    private static CanvasManager instance;
    public static CanvasManager Instance {get => instance;}

    private void Awake()
    {
        instance = this.GetComponent<CanvasManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        ButtonManagement(GameManager.Instance.coinNum);
        // ステージが5未満の際はDistanceボタンを使えない
        if(NonGameCanvasManager.Instance.stageNo < 5)
        {
            countPush[0] = 0;
        }
        else
        {
            countPush[0] = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        #region   // Distance
        // プレイヤーと歩く犬の距離を測るプログラム
        playerPos = player.transform.position;
        dogPos = dog.transform.position;
        // プレイヤーと犬の距離をdistanceに代入
        distance = Vector3.Distance(playerPos, dogPos);
        disText.text = "犬との距離は" + distance.ToString("f2") + "m";
        #endregion

        // UIの時計の針回転に関するプログラム
        // Lightの角度に合わせてそれぞれの針を回転
        // Lightが真上の時をAM0:00、真下の時をPM0:00で合うように針を合わせる
        shortHand.transform.eulerAngles = new Vector3(0, 0, -(GameManager.Instance.playTime + 90) * 2);
        longHand.transform.eulerAngles = new Vector3(0, 0, -GameManager.Instance.playTime * 24);
    }

    /// <summary>
    /// 現在のコイン数を表示する関数
    /// </summary>
    /// <param name="_coinNum">現在のコインの枚数</param>
    public void SetValueCoin(int _coinNum)
    {
        handcoinText.text = "：" + _coinNum.ToString("D3");
    }
    
    /// <summary>
    /// 現在の所持コイン数に応じてボタンの表示非表示を管理する関数
    /// </summary>
    /// <param name="_coinNum">現在のコインの枚数</param>
    public void ButtonManagement(int _coinNum)
    {
        for(int i=0; i<buttons.Length; i++)
        {
            //  各ボタンが押せる回数があり、押すために必要なコイン以上あればボタン有効化
            if(_coinNum >= pushableCoinNum[i] && countPush[i] > 0)
            {
                ButtonAppear(buttons[i]);
            }
            else
            {
                ButtonHide(buttons[i]);
            }
        }
    }

    #region  // 各ボタンの処理
    // CanvasManager
    // 左上のTimeBackボタンを押した際に呼ばれる関数
    public void TimeClick()
    {
        //Debug.Log("TimeBack押された");
        GameManager.Instance.UseMoney(pushableCoinNum[2]);
        countPush[2]--;
        timeBackObj.SetActive(true);
        StartCoroutine("BackInactive");
        GameManager.Instance.SunBack(90.0f);
    }

    // 右上のSpeedボタンを押した際に呼ばれる関数
    public void SpeedClick()
    {
        //Debug.Log("Speed押された");
        GameManager.Instance.UseMoney(pushableCoinNum[1]);
        player.GetComponent<PlayerCtrl>().SpeedUp(upSpeed);
        countPush[1]--;
    }

    // 左下のDistanceボタンを押した際に呼ばれる関数
    public void DistanceClick()
    {
        //Debug.Log("Distance押された");
        GameManager.Instance.UseMoney(pushableCoinNum[0]);
        disObj.SetActive(true);
        StartCoroutine("DistanceInactive");
    }

    // 右下のMapボタンを押した際に呼ばれる関数
    public void MapClick()
    {
        ///Debug.Log("Map押された");
        GameManager.Instance.UseMoney(pushableCoinNum[3]);
        mapObj.SetActive(true);
        StartCoroutine("MapInactive");
        GameManager.Instance.moveClock = false;
    }
    #endregion

    /// <summary>
    /// ボタン有効化と表示
    /// </summary>
    public void ButtonAppear(Button _button)
    {
        _button.interactable = true;
    }

    /// <summary>
    /// ボタン無効化と非表示
    /// </summary>
    public void ButtonHide(Button _button)
    {
        _button.interactable = false;
    }

    // timeBackLimit後に時を戻そうのオブジェクトを非表示にするコルーチン関数
    private IEnumerator BackInactive()
    {
        yield return new WaitForSeconds(timeBackLimit);

        timeBackObj.SetActive(false);
    }

    // disTimeLimit後にdisObjを非表示にするコルーチン関数
    private IEnumerator DistanceInactive()
    {
        yield return new WaitForSeconds(disTimeLimit);

        disObj.SetActive(false);
    }

    // mapTimeLimit後にMapを非表示にするコルーチン関数
    private IEnumerator MapInactive()
    {
        yield return new WaitForSeconds(mapTimeLimit);

        mapObj.SetActive(false);
        GameManager.Instance.moveClock = true;
    }
}
