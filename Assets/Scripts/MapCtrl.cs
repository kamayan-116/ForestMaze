using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCtrl : MonoBehaviour
{
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
        if(time > TimeLimit)
        {
            gameObject.SetActive(false);
        } else
        {
            time += Time.deltaTime;
            // Debug.Log("Maptime=" + time);
        }
    }
}
