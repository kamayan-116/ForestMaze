using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WalkDog : MonoBehaviour, IInteractive
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject kusoObj;
    [SerializeField] GameObject nowWalkDogPos;
    [SerializeField] GameObject disButton;
    [SerializeField] bool isShit = true;
    NavMeshAgent dogagent;
    int[] dist_x = new int[5];
    int[] dist_z = new int[5];
    Vector3[] distination = new Vector3[5];

    int a = 0;
    float time = 0.0f;
    float spanTime = 5.0f;
    int count = 0;
    GameObject[] kuso = new GameObject[3];

    // Start is called before the first frame update
    void Start()
    {
        dogagent = GetComponent<NavMeshAgent>();

        for (int i=0; i<5; i++)
        {
            dist_x[i] = Random.Range(4, 97) * 10;
            dist_z[i] = Random.Range(4, 97) * 10;

            distination[i] = new Vector3(dist_x[i], 0, dist_z[i]);
            //Debug.Log($"{distination[i]}");
        }

        dogagent.SetDestination(distination[0]);
        // Debug.Log($"{distination[0]}");
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log($"{dogagent.remainingDistance} {dogagent.hasPath}");

        if (dogagent.hasPath && dogagent.remainingDistance < 10.0f)
        {
            // Debug.Log($"a = {a} の時到着");
            a++;
            if (a >= 5)
            {
                a = 0;
            }
            dogagent.SetDestination(distination[a]);
        }

        time += Time.deltaTime;

        if (time >= spanTime && isShit)
        {
            time = 0.0f;
            if(count < 3)
            {
                kuso[count] = Instantiate(kusoObj, new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), Quaternion.identity);
                count++;
                // Debug.Log($"count = {count}");
            }
        }
    }

    public void Interact()
    {
        this.GetComponent<BoxCollider>().enabled = false;
        this.GetComponent<AudioSource>().enabled = false;
        this.GetComponent<NavMeshAgent>().enabled = false;

        Destroy(nowWalkDogPos);

        disButton.GetComponent<ButtonCtrl>().ButtonHide();
        isShit = false;

        foreach (var item in kuso)
        {
            Destroy(item);
        }

        transform.parent = player.gameObject.transform;
        this.transform.localPosition = new Vector3(0.0f, 0.0f, -5.0f);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            this.GetComponent<BoxCollider>().enabled = false;
            this.GetComponent<AudioSource>().enabled = false;
            this.GetComponent<NavMeshAgent>().enabled = false;

            Destroy(nowWalkDogPos);

            disButton.GetComponent<ButtonCtrl>().ButtonHide();
            isShit = false;

            foreach (var item in kuso)
            {
                Destroy(item);
            }

            transform.parent = collision.gameObject.transform;
            this.transform.localPosition = new Vector3(0.0f, 0.0f, -5.0f);
        }
    }
}
