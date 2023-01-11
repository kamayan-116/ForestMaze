using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// フェンスに関するプログラム
public class FenceCtrl : MonoBehaviour, IInteractive
{
    [SerializeField] GameObject keyObj;  // 当たり判定に対応するKeyオブジェクト
    
    // 対応するKeyが表示される時に当たった際FenceとKeyを非表示にする
    public void Interact()
    {
        if (keyObj.activeSelf)
        {
            this.gameObject.SetActive(false);
            keyObj.gameObject.SetActive(false);
        }
    }
}
