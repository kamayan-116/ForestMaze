using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomWarp : MonoBehaviour, IInteractive
{
    [SerializeField] Warp[] warpOut = new Warp[10];

    public void Interact()
    {
        var i = Random.Range(0, 10);
        warpOut[i].WarpOut();
    }
}
