using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraNonVRController : MonoBehaviour
{

    public float horizontralSens = 100.0f;
    public float verticalSens = 100.0f;

    float rotationClamp = 0f;
    float rotationY = 0.0f;

    public Transform player;

    // Start is called before the first frame update
    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //Rotate the player
        float mouseX = horizontralSens * Input.GetAxis("Mouse X") * Time.deltaTime;
        float mouseY = verticalSens * Input.GetAxis("Mouse Y") * Time.deltaTime;


        rotationClamp -= mouseY;
        //rotationY+= mouseX;

        rotationClamp = Mathf.Clamp(rotationClamp, -90, 90); 

        this.transform.localRotation = Quaternion.Euler(rotationClamp,0f,0f);

        player.Rotate(Vector3.up * mouseX);
    }
}
