/*using UnityEngine;
using System.Collections;

//http://docs.unity3d.com/ScriptReference/WheelCollider.html 

public class Carcontroller : MonoBehaviour {

	public float maxSteerAngle = 30.0f; // max angle of steering wheels

	// Use this for initialization
	void Start () {
		// setup wheels
		bool frontDrive = (wheelDrive == JWheelDrive.Front) || (wheelDrive == JWheelDrive.All);
		bool backDrive = (wheelDrive == JWheelDrive.Back) || (wheelDrive == JWheelDrive.All);
		
		// we use 4 wheels, but you can change that easily if neccesary.
		// this is the only place that refers directly to wheelFL, ...
		// so when adding wheels, you need to add the public transforms,
		// adjust the array size, and add the wheels initialisation here.
		wheels[0] = SetWheelParams(wheelFR, maxSteerAngle, frontDrive);
		wheels[1] = SetWheelParams(wheelFL, maxSteerAngle, frontDrive);
		wheels[2] = SetWheelParams(wheelBR, 0.0f, backDrive);
		wheels[3] = SetWheelParams(wheelBL, 0.0f, backDrive);
	
	}

	float steer = 0; 
	float accel = 0; 
	if ((checkForActive == null) || checkForActive.active) {
		// we only look at input when the object we monitor is
		// active (or we aren't monitoring an object).
		steer = Input.GetAxis("Horizontal");
		accel = Input.GetAxis("Vertical");
		brake = Input.GetButton("Jump");
	}

	if ((currentGear == 1) && (accel < 0.0f)) {
		ShiftDown(); // reverse
	}
	else if ((currentGear == 0) && (accel > 0.0f)) {
		ShiftUp(); // go from reverse to first gear
	}
	else if ((motorRPM > shiftUpRPM) && (accel > 0.0f)) {
		ShiftUp(); // shift up
	}
	else if ((motorRPM < shiftDownRPM) && (currentGear > 1)) {
		ShiftDown(); // shift down
	}
	if ((currentGear == 0)) {
		accel = - accel; // in automatic mode we need to hold arrow down for reverse
	}
	if (accel < 0.0f) {
		// if we try to decelerate we brake.
		brake = true;
		accel = 0.0f;
		wantedRPM = 0.0f;
	}
	col.steerAngle = steer * w.maxSteer;


	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.UpArrow))

	
	}
}*/


