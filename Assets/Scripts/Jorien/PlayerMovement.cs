using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	public float torque = 100f;
	public float speed = 50f;
	public float maxSpeed = 200f;
	private float curSpeed;
	private Vector3 blaSpeed;
	private bool up;
	private bool down;
	private bool left;
	private bool right;

	 


	// Use this for initialization
	void Awake () {
		up = false;
		down = false;
		left = false;
		right = false;

	}
		
	// Update is called once per frame
	void FixedUpdate () {
		if(Input.GetKey(KeyCode.UpArrow)) { up=true;	} 
		else if(Input.GetKey(KeyCode.DownArrow)) { down = true;		} 
		else if(Input.GetKey(KeyCode.RightArrow)){ right = true; }
		else if(Input.GetKey(KeyCode.LeftArrow)){ left = true; 	}

		if (up && right) { //Voort- en rechtsbewegen
						blaSpeed = transform.InverseTransformDirection (rigidbody.velocity);
						curSpeed = rigidbody.velocity.magnitude;
						rigidbody.AddTorque (Vector3.up * torque);
						if (blaSpeed.z >= 0) {
								rigidbody.AddForce (transform.right * curSpeed);
						} else {
								rigidbody.AddForce (transform.right * -curSpeed);
						} 
						rigidbody.AddForce (transform.forward * speed);
		} 
		else if (up && left) {  //Voort- en linksbewegen
						blaSpeed = transform.InverseTransformDirection (rigidbody.velocity);
						curSpeed = rigidbody.velocity.magnitude;
						rigidbody.AddTorque (Vector3.up * -torque); 
						if (blaSpeed.z >= 0) {
								rigidbody.AddForce (-transform.right * curSpeed);
						} else {
								rigidbody.AddForce (-transform.right * -curSpeed);
						}
						rigidbody.AddForce (transform.forward * speed);
	   }
	   if (down && right) { //acheruit- en rechtsbewegen
			            blaSpeed = transform.InverseTransformDirection (rigidbody.velocity);
			            curSpeed = rigidbody.velocity.magnitude;
						rigidbody.AddTorque (Vector3.up * torque);
						if (blaSpeed.z >= 0) {
							rigidbody.AddForce (transform.right * curSpeed);
						} else {
							rigidbody.AddForce (transform.right * -curSpeed);
						} 
						rigidbody.AddForce (transform.forward * -speed);
		} 
		else if (down && left) {  //Achteruit- en linksbewegen
						blaSpeed = transform.InverseTransformDirection (rigidbody.velocity);
						curSpeed = rigidbody.velocity.magnitude;
						rigidbody.AddTorque (Vector3.up * -torque); 
						if (blaSpeed.z >= 0) {
							rigidbody.AddForce (-transform.right * curSpeed);
						} else {
						rigidbody.AddForce (-transform.right * -curSpeed);
						}
						rigidbody.AddForce (transform.forward * -speed);
		}
		else if ( down && up) {}
		else if ( left && right) {}
		else if (up){ //voortbewegen
						rigidbody.AddForce (transform.forward * speed);
		}
		else if(down) { //achteruitbewegen
			 			rigidbody.AddForce (transform.forward * -speed); 
		} 
		else if(right){ //roteren naar rechts en snelheid splitsen in twee vectoren
						blaSpeed = transform.InverseTransformDirection(rigidbody.velocity);
						curSpeed = rigidbody.velocity.magnitude;
						rigidbody.AddTorque(Vector3.up*torque);
						if(blaSpeed.z>=0){
							rigidbody.AddForce (transform.right * curSpeed);
						}
						else{
							rigidbody.AddForce (transform.right * -curSpeed);
						}
		} 
		else if(left){ //roteren naar links en snelheid splitsen in twee vectorebn
						blaSpeed = transform.InverseTransformDirection(rigidbody.velocity);
						curSpeed = rigidbody.velocity.magnitude;
						rigidbody.AddTorque(Vector3.up*-torque); 
						if(blaSpeed.z>=0){
							rigidbody.AddForce (-transform.right * curSpeed);
						}
						else{
							rigidbody.AddForce (-transform.right * -curSpeed);
						}
		}


		if(rigidbody.velocity.magnitude > maxSpeed){
						rigidbody.velocity = rigidbody.velocity.normalized * maxSpeed;
		}
		

		up = false;
		down = false;
		left = false;
		right = false;

	
	}
}
