using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DistanceCtrl : MonoBehaviour
{
    [SerializeField] GameObject Player;
    [SerializeField] GameObject Dog;
    Vector3 PlayerPos;
    Vector3 DogPos;
    float Distance;
    [SerializeField] Text disText;
    float time = 0.0f;
    [SerializeField] float TimeLimit;

    void OnEnable()
    {
        time = 0.0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayerPos = Player.transform.position;
        DogPos = Dog.transform.position;
        Distance = Vector3.Distance(PlayerPos, DogPos);

        if(time > TimeLimit)
        {
            gameObject.SetActive(false);
        } else
        {
            time += Time.deltaTime;
            // Debug.Log("Distime=" + time);
            disText.text = "犬との距離は" + Distance.ToString("f2") + "m";
        }
    }
}
