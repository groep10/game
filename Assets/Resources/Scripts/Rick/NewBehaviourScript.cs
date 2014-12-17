using UnityEngine;
using System.Collections;

public class NewBehaviourScript : MonoBehaviour {

	public float MotorForce;
	public WheelCollider WheelColFR;
	public WheelCollider WheelColFL;
	public WheelCollider WheelColRR;
	public WheelCollider WheelColRL;
	public float SteerForce;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		float v = Input.GetAxis ("Vertical") * MotorForce;
		float h = Input.GetAxis ("Horizontal") * SteerForce;

		WheelColRL.motorTorque = v;
		WheelColRR.motorTorque = v;

		WheelColFL.steerAngle = h;
		WheelColFR.steerAngle = h;

	
	}
}
