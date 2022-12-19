using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactTrigger : TriggerBase {

   private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        action.Invoke();
    }
}
