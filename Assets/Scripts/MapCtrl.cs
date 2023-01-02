using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCtrl : MonoBehaviour
{
    [SerializeField] RotatingSun rotatingSun;
    [SerializeField] float time = 0.0f;
    [SerializeField] float TimeLimit;

    void OnEnable()
    {
        time = 0.0f;
        rotatingSun.moveClock = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(time > TimeLimit)
        {
            gameObject.SetActive(false);
            rotatingSun.moveClock = true;
        } else
        {
            time += Time.deltaTime;
        }
    }
}
