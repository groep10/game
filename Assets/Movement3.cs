﻿using UnityEngine;
using System.Collections;

public class Movement3 : MonoBehaviour {

	public float torque = 1000f;
	public float speed = 50f;
	public float amount = 50f;
	public float maxSpeed = 200f;


	// Use this for initialization
	void Start () {

	}

		
	
	// Update is called once per frame
	void FixedUpdate () {
		if(Input.GetKey(KeyCode.UpArrow)) { //Voortbewegen
			rigidbody.AddForce (transform.forward * speed); 
		} 
		else if(Input.GetKey(KeyCode.DownArrow)) { //Remmen en achteruit rijden
			rigidbody.AddForce (transform.forward * -speed); 
		} 
		else if(Input.GetKey(KeyCode.RightArrow)){ //roteren naar richting je wilt rijden 
			rigidbody.AddTorque(Vector3.up*torque);
			rigidbody.AddForce (transform.forward * speed);
		} 
		else if(Input.GetKey(KeyCode.LeftArrow)){
			rigidbody.AddTorque(Vector3.up*-torque); 
			rigidbody.AddForce (transform.forward * speed);
		}
	    if(rigidbody.velocity.magnitude > maxSpeed){
		    rigidbody.velocity = rigidbody.velocity.normalized * maxSpeed;
	    }

	
	}
}
