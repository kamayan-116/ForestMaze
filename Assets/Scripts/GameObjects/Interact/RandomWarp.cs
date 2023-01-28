using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Switch以外のワープに入るプログラム
public class RandomWarp : MonoBehaviour, IInteractive
{
    [SerializeField] private Warp[] warpOut = new Warp[10];

    public void Interact()
    {
        // 10個のワープからランダムに移動先を決める
        var i = Random.Range(0, 10);
        warpOut[i].WarpOut();
    }
}
