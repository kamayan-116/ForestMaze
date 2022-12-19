using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disappear : MonoBehaviour,ActionBase {

    public  void Action()
    {
        gameObject.SetActive(false);
    }
}
