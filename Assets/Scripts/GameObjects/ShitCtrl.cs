using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 糞を犬に対応して随時移動させるプログラム
public class ShitCtrl : MonoBehaviour
{
    private GameObject dog;
    public float shitYPos;  // 糞のY座標
    [SerializeField] private float remainTime;  // 同位置に滞在させる時間
    [SerializeField] private bool continuous;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TimeWait());
        dog = GameObject.Find("WalkDog");
    }

    // remainTime後、糞オブジェクトを犬の位置に移動させるコルーチン関数
    private IEnumerator TimeWait()
    {
        do
        {
            yield return new WaitForSeconds(remainTime);
            transform.position = new Vector3(dog.transform.position.x, dog.transform.position.y + shitYPos, dog.transform.position.z);
        } while (continuous);
    }
}
