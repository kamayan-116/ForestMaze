using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCtrl : MonoBehaviour, IInteractive
{
    [SerializeField] PlayerCtrl playerCtrl;
    
    // プレイヤーと当たった際銀の鍵を具現化
    public void Interact()
    {
        playerCtrl.SetActiveKey(3);
    }
}
