using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 糞を犬に対応して随時移動させるプログラム
public class KusoPosition : MonoBehaviour
{
    GameObject Dog;
    [SerializeField] private float remainTime;  // 同位置に滞在させる時間
    [SerializeField] bool continuous;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TimeWait());
        Dog = GameObject.Find("WalkDog");
    }

    // remainTime後、糞オブジェクトを犬の位置に移動させるコルーチン関数
    private IEnumerator TimeWait()
    {
        do
        {
            yield return new WaitForSeconds(remainTime);
            transform.position = new Vector3(Dog.transform.position.x, Dog.transform.position.y + 1f, Dog.transform.position.z);
        } while (continuous);
    }
}
