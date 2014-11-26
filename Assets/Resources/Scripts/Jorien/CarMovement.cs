/*
using UnityEngine;
using System.Collections;

public class CarMovement : MonoBehaviour {
	private Transform[] wheels;
	private float enginePower=150.0;
	private float power=0.0;
	private float brake=0.0;
	private float steer=0.0;
	private float maxSteer = 52.0;
	// Use this for initialization
	void Start () {
		rigidbody.centerOfMass = Vector3 (0, -0.5, 0.3);
	
	}
	
	// Update is called once per frame
	void Update () {
		power=Input.GetAxis("Vertical") * enginePower * Time.deltaTime * 250.0;
		steer=Input.GetAxis("Horizontal") * maxSteer;
		brake=Input.GetKey("space") ? rigidbody.mass * 0.1: 0.0;
		
		GetCollider(0).steerAngle=steer;
		GetCollider(1).steerAngle=steer;
		
		if(brake > 0.0){
			GetCollider(0).brakeTorque=brake;
			GetCollider(1).brakeTorque=brake;
			GetCollider(2).brakeTorque=brake;
			GetCollider(3).brakeTorque=brake;
			GetCollider(2).motorTorque=0.0;
			GetCollider(3).motorTorque=0.0;
		} else {
			GetCollider(0).brakeTorque=0;
			GetCollider(1).brakeTorque=0;
			GetCollider(2).brakeTorque=0;
			GetCollider(3).brakeTorque=0;
			GetCollider(2).motorTorque=power;
			GetCollider(3).motorTorque=power;
		}
	}

	function GetCollider(n : int) : WheelCollider{
		return wheels[n].gameObject.GetComponent(WheelCollider);
	}
	

}*/
