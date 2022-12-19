using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]

public class PlayerCtrl : MonoBehaviour
{
    CharacterController charCtrl;
    Animator animCtrl;
    [SerializeField] float speed = 4;
    [SerializeField] bool useCameraDir = true;
    [SerializeField] float movedirOffset = 0;
    [SerializeField] bool zEnable = true;
    [SerializeField] bool xEnable = true;
    public int Goalnumber = 0;
    GameObject goalwall;
    Vector3 forwardVec;
    Vector3 rightVec;

    // Start is called before the first frame update
    void Start()
    {
        charCtrl = GetComponent<CharacterController>();
        animCtrl = GetComponent<Animator>();

        var angles = new Vector3(0, movedirOffset, 0);
        forwardVec = Quaternion.Euler(angles) * Vector3.forward;
        rightVec = Quaternion.Euler(angles) * Vector3.right;
        goalwall = GameObject.Find("GoalWall");
    }

    float fallpow = -2.0f;

    Transform groundobj = null;
    Transform beforeGroundObj = null;
    Vector3 beforePos;
    Vector3 floorOffset;

    // Update is called once per frame
    void Update()
    {

        // input and calculate move direction
        float xaxis = Input.GetAxis("Horizontal");
        float yaxis = Input.GetAxis("Vertical");

        Vector3 cameraFwdVec = forwardVec;
        Vector3 cameraRightVec = rightVec;
        if(useCameraDir) {
            cameraFwdVec = Camera.main.transform.TransformDirection(forwardVec);
            cameraFwdVec.Scale(new Vector3(1, 0, 1));
            cameraFwdVec.Normalize();

            cameraRightVec = Camera.main.transform.TransformDirection(rightVec);
            cameraRightVec.Scale(new Vector3(1, 0, 1));
            cameraRightVec.Normalize();
        }

        var movementxaxis = xaxis;
        var movementyaxis = yaxis;

        if (!zEnable)
        {
            movementyaxis = 0;
        }
        if (!xEnable)
        {
            movementxaxis = 0;
        }
        Vector3 moveDir = cameraFwdVec * movementyaxis + cameraRightVec * movementxaxis;
        animCtrl.SetFloat("Speed", moveDir.magnitude);

        //moving floor support
        if (groundobj)
        {
            if (groundobj == beforeGroundObj)
            {
                floorOffset = beforePos - groundobj.position;
            }
            else
            {
                beforeGroundObj = groundobj;
                floorOffset = Vector3.zero;
            }
            beforePos = groundobj.position;
            beforeGroundObj = groundobj;
        }

        //Movement
        charCtrl.Move(((new Vector3(0, fallpow, 0) + (moveDir * speed)) * Time.deltaTime) - floorOffset);

        //Rotation
        var rotdir = cameraFwdVec * yaxis + cameraRightVec * xaxis;
        if (rotdir.magnitude > 0)
        {
            transform.eulerAngles = new Vector3(
                0, Vector3.SignedAngle(Vector3.forward, rotdir, Vector3.up), 0);
        }

    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //Ground object detecting
        if (hit.gameObject != gameObject && !hit.collider.isTrigger)
            groundobj = hit.transform;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "dog")
        {
            Goalnumber++;
            // Debug.Log($"{Goalnumber}");
            if(Goalnumber == 2)
            {
                Destroy(goalwall);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Goal")
        {
            SceneManager.sceneLoaded += GameSceneLoaded;
            SceneManager.LoadScene("NonGameScene");
        }
    }

    public void GameSceneLoaded(Scene nongame, LoadSceneMode mode)
    {
        var canvasManager = GameObject.Find("Canvas").GetComponent<NonGameCanvasCtrl>();

        canvasManager.ResultPanel(0);

        SceneManager.sceneLoaded -= GameSceneLoaded;
    }

    public void SpeedUp(float up)
    {
        speed += up;
    }
}
