using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]

// プレイヤーに関するプログラム
public class PlayerCtrl : MonoBehaviour
{
    CharacterController charCtrl;
    Animator animCtrl;
    [SerializeField] float speed = 5;  // 移動スピード
    [SerializeField] bool useCameraDir = true;  // カメラが使えるか否か
    [SerializeField] float movedirOffset = 0;
    [SerializeField] bool zEnable = true;  // z方向の動きができるか否か
    [SerializeField] bool xEnable = true;  // x方向の動きができるか否か
    [SerializeField] MakeMaze makeMaze;  // 迷路の自動生成のスクリプト
    public int captureDog = 0;  // 捕まえた犬の数
    Vector3 forwardVec;  // 前向きのVector
    Vector3 rightVec;  // 右向きのVector

    // Start is called before the first frame update
    void Start()
    {
        charCtrl = GetComponent<CharacterController>();
        animCtrl = GetComponent<Animator>();

        var angles = new Vector3(0, movedirOffset, 0);
        forwardVec = Quaternion.Euler(angles) * Vector3.forward;
        rightVec = Quaternion.Euler(angles) * Vector3.right;
    }

    float fallpow = -2.0f;

    // Update is called once per frame
    void Update()
    {

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
        charCtrl.Move(((new Vector3(0, fallpow, 0) + (moveDir * speed)) * Time.deltaTime));

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
        
        // dogのタグと当たれば、captureDogを増やす
        if (collision.gameObject.tag == "dog")
        {
            captureDog++;
            // 捕まえた犬の数がgoalConditionと同じになれば金の鍵を具現化
            if(captureDog == makeMaze.goalCondition)
            {
                SetActiveKey(4);
            }
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

        // Switchのタグと当たれば、銀の鍵を具現化
        if (other.gameObject.tag == "Switch")
        {
            SetActiveKey(3);
        }

        // Goalのタグと当たればNonGameSceneにシーンチェンジ
        if (other.gameObject.tag == "Goal")
        {
            GameManager.instance.SetGameResult(0);
        }
    }

    // 引数の子オブジェクトを具現化する
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
