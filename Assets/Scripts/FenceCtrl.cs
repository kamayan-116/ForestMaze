using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FenceCtrl : MonoBehaviour
{
    [SerializeField] GameObject keyObj;
    
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
