using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 止まる犬に関するプログラム
public class StayDog : MonoBehaviour
{
    [SerializeField] GameObject NowStayDogPos;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            this.GetComponent<BoxCollider>().enabled = false;
            this.GetComponent<AudioSource>().enabled = false;

            Destroy(NowStayDogPos);

            transform.parent = collision.gameObject.transform;
            this.transform.localPosition = new Vector3(0.0f, 0.0f, -5.0f);
            this.transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
        }
    }
}
