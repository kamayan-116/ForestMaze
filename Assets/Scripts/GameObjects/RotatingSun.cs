using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Lightの動きに関するプログラム
public class RotatingSun : MonoBehaviour
{
    [SerializeField] private Vector3 rot = new Vector3(0f, 330f, 0f);  // Lightの向き
    [SerializeField] float fIntensity;  // ライトの明るさ
    
    // Start is called before the first frame update
    void Start()
    {
        transform.localRotation = Quaternion.Euler(rot);
        fIntensity = this.GetComponent<Light>().intensity;
    }

    // Update is called once per frame
    void Update()
    {
        // PM6:00~AM6:00はライトの明かりを弱くする
        if(transform.eulerAngles.x > 180.0f && transform.eulerAngles.x < 360.0f)
        {
            fIntensity = 0.2f;
        } else
        {
            fIntensity = 1.0f;
        }
        this.GetComponent<Light>().intensity = fIntensity;
    }

    /// <summary>
    /// ゲームの経過時間をLightの回転に入れる関数
    /// </summary>
    /// <param name="_rottmp">ゲームの経過時間</param>
    public void MoveLight(float _rottmp)
    {
        transform.eulerAngles = new Vector3(-_rottmp, -30, 0);
    }
}
