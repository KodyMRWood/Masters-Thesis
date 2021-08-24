using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNonVRController : MonoBehaviour
{
    public float moveSpeed = 100.0f;
    public float camSpeed = 0.01f;



    public CharacterController character;

    // Start is called before the first frame update
    void Start()
    {
        //character = this.gameObject.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKey(KeyCode.W))
        //{
        //    this.transform.localPosition += (this.transform.forward * moveSpeed);
        //}
        //if (Input.GetKey(KeyCode.S))
        //{
        //    this.transform.localPosition += (this.transform.forward * -moveSpeed);
        //}
        //if (Input.GetKey(KeyCode.A))
        //{
        //    this.transform.localPosition += (this.transform.right * -moveSpeed);
        //}
        //if (Input.GetKey(KeyCode.D))
        //{
        //    this.transform.localPosition += (this.transform.right * moveSpeed);
        //}
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector2 movement = (this.transform.right * x) + (this.transform.forward * z);


        character.Move(movement * moveSpeed );

    }
}
