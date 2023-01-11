using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// プレイヤーに関するプログラム
public class PlayerCtrl : MonoBehaviour
{
    Rigidbody rb;
    Animator animCtrl;
    [SerializeField] float speed = 5;  // 移動スピード
    [SerializeField] bool useCameraDir = true;  // カメラが使えるか否か
    [SerializeField] float movedirOffset = 0;
    [SerializeField] bool zEnable = true;  // z方向の動きができるか否か
    [SerializeField] bool xEnable = true;  // x方向の動きができるか否か
    Vector3 forwardVec;  // 前向きのVector
    Vector3 rightVec;  // 右向きのVector

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animCtrl = GetComponent<Animator>();

        var angles = new Vector3(0, movedirOffset, 0);
        forwardVec = Quaternion.Euler(angles) * Vector3.forward;
        rightVec = Quaternion.Euler(angles) * Vector3.right;
    }

    // Update is called once per frame
    void Update()
    {
        if(!GameManager.instance.isPlayerOpe) return;

        // プレイヤーの入力キー
        float xaxis = Input.GetAxis("Horizontal");
        float yaxis = Input.GetAxis("Vertical");

        Vector3 cameraFwdVec = forwardVec;
        Vector3 cameraRightVec = rightVec;

        // カメラの動き
        if(useCameraDir) {
            cameraFwdVec = Camera.main.transform.TransformDirection(forwardVec);
            cameraFwdVec.Scale(new Vector3(1, 0, 1));
            cameraFwdVec.Normalize();

            cameraRightVec = Camera.main.transform.TransformDirection(rightVec);
            cameraRightVec.Scale(new Vector3(1, 0, 1));
            cameraRightVec.Normalize();
        }

        // 入力キーに応じた動きの変数
        var movementxaxis = xaxis;
        var movementyaxis = yaxis;

        if (!zEnable)
        {
            movementyaxis = 0;
        }
        if (!xEnable)
        {
            movementxaxis = 0;
        }

        // アニメーションの動きの設定
        Vector3 moveDir = cameraFwdVec * movementyaxis + cameraRightVec * movementxaxis;
        animCtrl.SetFloat("Speed", moveDir.magnitude);

        // プレイヤーの移動
        rb.velocity = moveDir * speed;
        
        // プレイヤーの回転
        var rotdir = cameraFwdVec * yaxis + cameraRightVec * xaxis;
        if (rotdir.magnitude > 0)
        {
            transform.eulerAngles = new Vector3(
                0, Vector3.SignedAngle(Vector3.forward, rotdir, Vector3.up), 0);
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        try
        {
            //相手がInteractiveインターフェイスを持ってたら、相手が持つInteract関数を実行
            collision.gameObject.GetComponent<IInteractive>().Interact();
        }
        catch
        {
            //失敗してもエラーは出ない
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        try
        {
            //相手がInteractiveインターフェイスを持ってたら、相手が持つInteract関数を実行
            other.gameObject.GetComponent<IInteractive>().Interact();
        }
        catch
        {
            //失敗してもエラーは出ない
        }
    }

    /// <summary>
    /// 引数の子オブジェクトを具現化する
    /// </summary>
    /// <param name="childNum">子オブジェクトの番号</param>
    public void SetActiveKey(int childNum)
    {
        this.transform.GetChild(childNum).gameObject.SetActive(true);
    }

    // スピードアップ
    public void SpeedUp(float up)
    {
        speed += up;
    }
}
