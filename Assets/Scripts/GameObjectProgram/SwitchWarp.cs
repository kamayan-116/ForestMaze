using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchWarp : MonoBehaviour
{
    [SerializeField] Warp switchWarpOut;
    private void OnTriggerEnter(Collider other)
    {
        switchWarpOut.WarpOut(); 
    }
}
