using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseCtrl : MonoBehaviour,  IInteractive
{
    // Goalのタグと当たればNonGameSceneにシーンチェンジ
    public void Interact()
    {
        GameManager.instance.SetGameResult(0);
    }
}
