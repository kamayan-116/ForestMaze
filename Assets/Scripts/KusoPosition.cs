using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KusoPosition : MonoBehaviour
{
    GameObject Dog;
    [SerializeField] private float remainTime;
    [SerializeField] bool continuous;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TimeWait());
        Dog = GameObject.Find("WalkDog");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator TimeWait()
    {
        do
        {
            yield return new WaitForSeconds(remainTime);
            transform.position = new Vector3(Dog.transform.position.x, Dog.transform.position.y + 0.5f, Dog.transform.position.z);
        } while (continuous);
    }
}
