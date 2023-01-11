using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    [SerializeField] GameObject player;  // プレイヤーオブジェクト
    // [SerializeField] Vector3 playerPos;
    // [SerializeField] float height = 2;  // プレイヤーの目線分の高さ
    public Vector3 camDir = new Vector3(0, 4, -8);  // カメラの方向
    public Vector3 lookPos = new Vector3(0, 2f, 0);  // 注視点の高さ
    public float nearest = 8f;  // 撮影の最短距離
    [SerializeField] float moveSpeed = 2f;  // カメラの移動スピード

    private void Start()
    {
        
    }

    private void LateUpdate()
    {
        // var playerGaze = new Vector3(player.transform.position.x, height, player.transform.position.z);
        // playerPos = playerGaze;
        // var nextGaze = player.transform.TransformDirection(camDir);
        // nextGaze.y += height;
        
        // カメラの新方向newDirをプレイヤー位置から算出する
        // Vector3 newDir = Vector3.Slerp(
        //     transform.position - playerGaze,  // 現状のカメラ方向
        //     nextGaze,  // 目的のカメラ方向
        //     Time.fixedDeltaTime * moveSpeed
        // );  // その差の割合（0.0f～1.0f）
        Vector3 newDir = Vector3.Slerp(
            transform.position - player.transform.position,  // 現状のカメラ方向
            player.transform.TransformDirection(camDir),  // 目的のカメラ方向
            Time.fixedDeltaTime * moveSpeed
        );  // その差の割合（0.0f～1.0f）

        //注視点から新方向までの途中に何か当たるか検査
        // Ray camRay = new Ray(playerGaze, newDir);
        Ray camRay = new Ray(player.transform.position, newDir); 
        RaycastHit hitInfo; //当たった時の情報を入れる物

        if (Physics.Raycast(camRay, out hitInfo, newDir.magnitude))
        {
            if (hitInfo.distance > nearest)
            {
                //カメラは当たった壁の少し手前へ
                transform.position = hitInfo.point - newDir.normalized * 0.2f;
            }
            else
            {
                //カメラは最短距離へ
                // transform.position = playerGaze + newDir.normalized * nearest;
                transform.position = player.transform.position + newDir.normalized * nearest;
            }
        }
        else
        {
            // transform.position = playerGaze + newDir;  // カメラは新座標へ
            transform.position = player.transform.position + newDir;  // カメラは新座標へ
        }
        
        //カメラを注視点向きに回転
        // transform.LookAt(playerGaze + lookPos);
        transform.LookAt(player.transform.position + lookPos);
    }
}
