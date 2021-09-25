using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public GameObject camera;
	public float speed = 1f;
	public float mouseSens = 1f;
	float xAngle, yAngle;
	
    // Start is called before the first frame update
    void Start()
    {
		Cursor.visible = false;
        xAngle = yAngle = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        move();
		rotate();
    }
	
	void move(){
		if(Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)){
			transform.Translate(Vector3.forward * speed * Time.deltaTime);
		}
		if(Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)){
			transform.Translate(Vector3.back * speed * Time.deltaTime);
		}
		if(Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)){
			transform.Translate(Vector3.left * speed * Time.deltaTime);
		}
		if(Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)){
			transform.Translate(Vector3.right * speed * Time.deltaTime);
		}
		if(Input.GetKey(KeyCode.Space)){
			//transform.Translate(Vector3.up * speed * Time.deltaTime);
			GetComponent<Rigidbody>().velocity = new Vector3(0f, speed, 0f);
		}
		//GetComponent<Rigidbody>().velocity = Vector3.zero;
		GetComponent<Rigidbody>().velocity = new Vector3(0f, GetComponent<Rigidbody>().velocity.y, 0f);
	}
	
	void rotate(){
		yAngle += Input.GetAxis("Mouse X") * mouseSens * Time.deltaTime;
		xAngle -= Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime;
		
		//yAngle = Mathf.Clamp(yAngle, -90f, 90f);
		//xAngle = Mathf.Clamp(xAngle, -90f, 90f);
		
		transform.localRotation = Quaternion.Euler(0f, yAngle, 0f);
		camera.transform.Rotate(-Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime, 0f, 0f, Space.Self);
	}
}
