#pragma strict

//using UnityEngine;
//using UnityEngine.UI;
//using System.Collections;

public class Movement extends MonoBehaviour {
  public var torque: float;
  public var speed: float;
  public var amount : float = 50f;

  function Start () {
  }

  function FixedUpdate () {
  if (Input.GetKey(KeyCode.UpArrow)) { rigidbody.AddForce (Vector3.forward * speed); }
  if (Input.GetKey(KeyCode.DownArrow)) { rigidbody.AddForce (Vector3.forward * -speed); }
  //if (Input.GetKey(KeyCode.LeftArrow)) { rigidbody.AddTorque (transform.up * torque); }
  //if (Input.GetKey(KeyCode.RightArrow)) { rigidbody.AddTorque (transform.up * -torque); }
  var h : float = Input.GetAxis("Horizontal") * amount * Time.deltaTime;
  rigidbody.AddTorque(0, 10 * h,0);
     
   
		
	}
	
	
  

}

						 
						
