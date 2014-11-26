using UnityEngine;
using System.Collections;

public enum wheelDrive {
	Front = 0,
	Back = 1,
	All = 2
}


public class WheelCar2 : MonoBehaviour { 

	//Wielen introduceren
	public Transform wheelFR; 
	public Transform wheelFL; 
	public Transform wheelBR; 
	public Transform wheelBL; 
	
	//eigenschappen van de wielen
	public float suspensionDistance = 0.2f; 
	public float springs = 1000.0f; 
	public float dampers = 2f; 
	public float wheelRadius = 0.25f; 
	public float torque = 100f; 
	public float brakeTorque = 2000f; 
	public float wheelWeight = 3f;
	public Vector3 shiftCentre = new Vector3(0.0f, -0.5f, 0.0f); 
	public float fwdStiffness = 0.1f; //stijfheid wielen wordt slip mee bepaald
	public float swyStiffness = 0.1f; 	
	public float maxSteerAngle = 30.0f; 
	public wheelDrive wheelDrive = wheelDrive.Front; 
	
	//Schakel voorwaarden
	public float shiftDownRPM = 1500.0f; 
	public float shiftUpRPM = 2500.0f; 
	public float[] gears = { -10f, 9f, 6f, 4.5f, 3f, 2.5f };
	float[] efficiencyTable = { 0.6f, 0.65f, 0.7f, 0.75f, 0.8f, 0.85f, 0.9f, 1.0f, 1.0f, 0.95f, 0.80f, 0.70f, 0.60f, 0.5f, 0.45f, 0.40f, 0.36f, 0.33f, 0.30f, 0.20f, 0.10f, 0.05f };
	float efficiencyTableStep = 250.0f;
	int currentGear = 1; 
	
	// alle info van de wielen wordt hierin opgeslagen
	class WheelData {
		public Transform transform;
		public GameObject go;
		public WheelCollider col;
		public Vector3 startPos;
		public float rotation = 0.0f;
		public float maxSteer;
		public bool motor;
	};

	//Er wordt een array aangemaakt waar per wiel data instaat
	WheelData[] wheels; 



	WheelData SetWheelParams(Transform wheel, float maxSteer, bool motor) {
		WheelData result = new WheelData(); // the container of wheel specific data
		
		// we create a new gameobject for the collider and move, transform it to match
		// the position of the wheel it represents. This allows us to do transforms
		// on the wheel itself without disturbing the collider.
		GameObject go = new GameObject("WheelCollider");
		go.transform.parent = transform; // the car, not the wheel is parent
		go.transform.position = wheel.position; // match wheel pos
		//go.transform.rotation = wheel.rotation; 
		
		// create the actual wheel collider in the collider game object
		WheelCollider col = (WheelCollider) go.AddComponent(typeof(WheelCollider));
		col.motorTorque = 0.0f;
		
		// store some useful references in the wheeldata object
		result.transform = wheel; // access to wheel transform 
		result.go = go; // store the collider game object
		result.col = col; // store the collider self
		result.startPos = go.transform.localPosition; // store the current local pos of wheel
		result.maxSteer = maxSteer; // store the max steering angle allowed for wheel
		result.motor = motor; // store if wheel is connected to engine
		
		return result; // return the WheelData
	}
	
	// Use this for initialization
	void Start () {
		//Eerst auto goed zwaartepunt geven
		rigidbody.centerOfMass += shiftCentre;

		//De wielen in een WheelData array plaatsen en de settings maken
		wheels = new WheelData[4];		
		bool frontDrive = (wheelDrive == wheelDrive.Front) || (wheelDrive == wheelDrive.All);
		bool backDrive = (wheelDrive == wheelDrive.Back) || (wheelDrive == wheelDrive.All);	
		//print (frontDrive);
		//print (backDrive);
		//print (wheelDrive.Front);
		//print (wheelDrive.Back);
		wheels[0] = SetWheelParams(wheelFR, maxSteerAngle, frontDrive);
		wheels[1] = SetWheelParams(wheelFL, maxSteerAngle, frontDrive);
		wheels[2] = SetWheelParams(wheelBR, 0.0f, backDrive);
		wheels[3] = SetWheelParams(wheelBL, 0.0f, backDrive);
		
		//Na gegevens aan de wielen gegeven te hebben en colliders aangemaakt te hebben nog wat laatste aanpassingen
		foreach (WheelData w in wheels) {
			WheelCollider col = w.col;
			col.suspensionDistance = suspensionDistance;
			JointSpring js = col.suspensionSpring;
			js.spring = springs;
			js.damper = dampers;            
			col.suspensionSpring = js;
			col.radius = wheelRadius;
			col.mass = wheelWeight;

			WheelFrictionCurve fc = col.forwardFriction;
			fc.asymptoteValue = 5000.0f;
			fc.extremumSlip = 2.0f;
			fc.asymptoteSlip = 20.0f;
			fc.stiffness = fwdStiffness;
			col.forwardFriction = fc;
			fc = col.sidewaysFriction;
			fc.asymptoteValue = 7500.0f;
			fc.asymptoteSlip = 2.0f;
			fc.stiffness = swyStiffness;
			col.sidewaysFriction = fc;
			w.col=col; //zelf toegevoegd
		}

	}
	
	float shiftDelay = 0.0f;
	
	// handle shifting a gear up
	public void ShiftUp() {
		float now = Time.timeSinceLevelLoad;
		
		// check if we have waited long enough to shift
		if (now < shiftDelay) return;
		
		// check if we can shift up
		if (currentGear < gears.Length - 1) {
			currentGear ++;
			
			// we delay the next shift with 1s. (sorry, hardcoded)
			shiftDelay = now + 1.0f;
		}
	}
	
	// handle shifting a gear down
	public void ShiftDown() {
		float now = Time.timeSinceLevelLoad;
		
		// check if we have waited long enough to shift
		if (now < shiftDelay) return;
		
		// check if we can shift down (note gear 0 is reverse)
		if (currentGear > 0) {
			currentGear --;
			
			// we delay the next shift with 1/10s. (sorry, hardcoded)
			shiftDelay = now + 0.1f;
		}
	}
	
	float wantedRPM = 0.0f; // rpm the engine tries to reach
	float motorRPM = 0.0f;

	
	// handle the physics of the engine
	void FixedUpdate () {

		

		/*foreach (WheelData w in wheels) {
			if(w.transform.position.y<0){
				Transform.position = new Vector3 (Transform.position.x,1,Transform.position.z);
			}
		}*/

		float delta = Time.fixedDeltaTime;
		
		float steer = 0; // steering -1.0 .. 1.0
		float accel = 0; // accelerating -1.0 .. 1.0
		bool brake = false; // braking (true is brake)
		

		// we only look at input when the object we monitor is
		// active (or we aren't monitoring an object).
		steer = Input.GetAxis("Horizontal");
		accel = Input.GetAxis("Vertical");
		brake = Input.GetButton("Jump");

				
		// handle automatic shifting
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
		
		// the RPM we try to achieve.
		wantedRPM = (5500.0f * accel) * 0.1f + wantedRPM * 0.9f;
		
		float rpm = 0.0f;
		int motorizedWheels = 0;
		bool floorContact = false;
		
		// calc rpm from current wheel speed and do some updating
		foreach (WheelData w in wheels) {
			WheelHit hit;
			WheelCollider col = w.col;
			
			// only calculate rpm on wheels that are connected to engine
			if (w.motor) {
				rpm += col.rpm;
				motorizedWheels++;
			}
			
			// calculate the local rotation of the wheels from the delta time and rpm
			// then set the local rotation accordingly (also adjust for steering)
			w.rotation = Mathf.Repeat(w.rotation + delta * col.rpm * 360.0f / 60.0f, 360.0f);
			w.transform.localRotation = Quaternion.Euler(w.rotation, col.steerAngle, 0.0f);
			
			// let the wheels contact the ground, if no groundhit extend max suspension distance
			Vector3 lp = w.transform.localPosition;
			if (col.GetGroundHit(out hit)) {
				lp.y -= Vector3.Dot(w.transform.position - hit.point, transform.up) - col.radius;
				floorContact = floorContact || (w.motor);
			}
			else {
				lp.y = w.startPos.y - suspensionDistance;
			}
			w.transform.localPosition = lp;
		}
		// calculate the actual motor rpm from the wheels connected to the engine
		// note we haven't corrected for gear yet.
		if (motorizedWheels > 1) {
			rpm = rpm / motorizedWheels;
		}
		
		// we do some delay of the change (should take delta instead of just 95% of
		// previous rpm, and also adjust or gears.
		motorRPM = 0.95f * motorRPM + 0.05f * Mathf.Abs(rpm * gears[currentGear]);
		if (motorRPM > 5500.0f) motorRPM = 5500.0f;
		
		// calculate the 'efficiency' (low or high rpm have lower efficiency then the
		// ideal efficiency, say 2000RPM, see table
		int index = (int) (motorRPM / efficiencyTableStep);
		if (index >= efficiencyTable.Length) index = efficiencyTable.Length - 1;
		if (index < 0) index = 0;
		
		// calculate torque using gears and efficiency table
		float newTorque = torque * gears[currentGear] * efficiencyTable[index];
		
		// go set torque to the wheels
		foreach (WheelData w in wheels) {
			WheelCollider col = w.col;
			
			// of course, only the wheels connected to the engine can get engine torque
			if (w.motor) {
				// only set torque if wheel goes slower than the expected speed
				if (Mathf.Abs(col.rpm) > Mathf.Abs(wantedRPM)) {
					// wheel goes too fast, set torque to 0
					col.motorTorque = 0;
				}
				else {
					// 
					float curTorque = col.motorTorque;
					col.motorTorque = curTorque * 0.9f + newTorque * 0.1f;
				}
			}
			// check if we have to brake
			col.brakeTorque = (brake)?brakeTorque:0.0f;
			//if(brake){col.brakeTorque= brakeTorque; brakeTorque =- 500f;}
			
			// set steering angle
			col.steerAngle = steer * w.maxSteer;
		}
		

	}
	

}

