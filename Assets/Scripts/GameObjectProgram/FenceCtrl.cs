using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// フェンスに関するプログラム
public class FenceCtrl : MonoBehaviour
{
    [SerializeField] GameObject keyObj;  // 当たり判定に対応するKeyオブジェクト
    
    // 対応するKeyと当たった際FenceとKeyを非表示にする
    private void OnCollisionEnter(Collision collision)
    {
        // Debug.Log(collision.gameObject.name);
        
        if (collision.gameObject == keyObj)
        {
            this.gameObject.SetActive(false);
            collision.gameObject.SetActive(false);
        }
    }
}
