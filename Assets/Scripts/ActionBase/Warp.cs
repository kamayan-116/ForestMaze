using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warp : MonoBehaviour ,ActionBase
{
	[SerializeField] Transform TargetObject;

    public  void Action()
    {
        CharacterController ccnt = TargetObject.GetComponent<CharacterController>();
        if(ccnt != null)
        {
            ccnt.enabled = false;
            TargetObject.position = transform.position;
            ccnt.enabled = true;
        }
        else
        {
            TargetObject.position = transform.position;
        }

        if(NonGameCanvasCtrl.Instance.stageNo == 2)
        {
            TargetObject.gameObject.GetComponent<PlayerCtrl>().SetActiveKey(5);
        }
    }
}
