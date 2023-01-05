using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Lightの動きに関するプログラム
public class RotatingSun : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 1.0f;  // 回転スピード
    [SerializeField] private Vector3 rot = new Vector3(0f, 330f, 0f);  // Lightの向き
    [SerializeField] GameObject BackImage;
    [SerializeField] ButtonCtrl timebuttonClick;
    public float rottmp = 0;  // Lightの角度のX座標
    public GameObject slight;
    [SerializeField] float Intensity;  // ライトの明るさ
    bool isBack = false;  // ライトが戻るか否か
    float startBack;  // 戻る際の初期X角度
    float finishBack; // 戻る際の終了X角度
    public bool moveClock = true;  // Map表示時はfalseで移動停止

    
    // Start is called before the first frame update
    void Start()
    {
        transform.localRotation = Quaternion.Euler(rot);
        Intensity = slight.GetComponent<Light>().intensity;
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
            StartCoroutine("BackDes");
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
        transform.eulerAngles = new Vector3(-rottmp, -30, 0);
        // Debug.Log(rottmp);

        // rottmpが195を超えるとGameOver
        if(rottmp > 195.0f)
        {
            SceneManager.sceneLoaded += GameSceneLoaded;
            SceneManager.LoadScene("NonGameScene");
        }

        // PM6:00~AM6:00はライトの明かりを弱くする
        if(transform.eulerAngles.x > 180.0f && transform.eulerAngles.x < 360.0f)
        {
            Intensity = 0.2f;
        } else
        {
            Intensity = 1.0f;
        }
        slight.GetComponent<Light>().intensity = Intensity;
    }

    // SceneChangeする際に、呼ばれる関数
    public void GameSceneLoaded(Scene nongame, LoadSceneMode mode)
    {
        // シーンチェンジ先のCanvas内のNonGameCanvasCtrlスクリプトのResultPanel関数を呼ぶ
        var canvasManager = GameObject.Find("Canvas").GetComponent<NonGameCanvasCtrl>();

        canvasManager.ResultPanel(1, HandCoinCtrl.instance.stageCoinNum, 195.0f - rottmp, HandCoinCtrl.instance.CoinNum, timebuttonClick.count + 1);
        SceneManager.sceneLoaded -= GameSceneLoaded;
    }

    // TimeBackする際のlightの処理
    public void SunBack(float back)
    {
        isBack = true;
        startBack = rottmp;
        finishBack = rottmp - back;
        if(finishBack <= 0)
        {
            finishBack = 0;
        }
        // Debug.Log(FinishBack);
    }

    // 2秒後に時を戻そうのオブジェクトを非表示にするコルーチン関数
    private IEnumerator BackDes()
    {
        yield return new WaitForSeconds(2);

        BackImage.gameObject.SetActive(false);
    }
}
