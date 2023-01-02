using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RotatingSun : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 1.0f;
    [SerializeField] private Vector3 rot = new Vector3(0f, 330f, 0f);
    [SerializeField] GameObject BackImage;
    [SerializeField] ButtonCtrl timebuttonClick;
    public float rottmp = 0;
    public GameObject slight;
    float Intensity;
    bool isBack = false;
    float StartBack;
    float FinishBack;
    public bool moveClock = true;

    
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
        else if(isBack && FinishBack < rottmp)
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

        if(rottmp > 195.0f)
        {
            SceneManager.sceneLoaded += GameSceneLoaded;
            SceneManager.LoadScene("NonGameScene");
        }

        if(transform.eulerAngles.x > 180.0f && transform.eulerAngles.x < 360.0f)
        {
            Intensity = 0.2f;
            slight.GetComponent<Light>().intensity = Intensity;
        } else
        {
            Intensity = 1.0f;
            slight.GetComponent<Light>().intensity = Intensity;
        }

        // Debug.Log(slight.GetComponent<Light>().intensity);
    }

    public void GameSceneLoaded(Scene nongame, LoadSceneMode mode)
    {
        var canvasManager = GameObject.Find("Canvas").GetComponent<NonGameCanvasCtrl>();

        canvasManager.ResultPanel(1, HandCoinCtrl.instance.stageCoinNum, 195.0f - rottmp, HandCoinCtrl.instance.CoinNum, timebuttonClick.count + 1);

        SceneManager.sceneLoaded -= GameSceneLoaded;
    }

    public void SunBack(float back)
    {
        isBack = true;
        StartBack = rottmp;
        FinishBack = rottmp - back;
        if(FinishBack <= 0)
        {
            FinishBack = 0;
        }
        // Debug.Log(FinishBack);
    }

    private IEnumerator BackDes()
    {
        yield return new WaitForSeconds(2);

        BackImage.gameObject.SetActive(false);
    }
}
