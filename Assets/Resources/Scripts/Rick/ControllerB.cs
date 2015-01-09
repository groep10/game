using UnityEngine;
using System.Collections;

public class ControllerB : MonoBehaviour {
	
	public float MotorForce;

	public WheelCollider WheelColFR;
	public WheelCollider WheelColFL;
	public WheelCollider WheelColRR;
	public WheelCollider WheelColRL;
	
	public Transform wheelFR; 
	public Transform wheelFL; 
	public Transform wheelRR; 
	public Transform wheelRL; 
	
	public float SteerForce;
	public float BrakeForce;
	
	private float RotationValue = 0.0f;
	
	Vector3 startRotFL = new Vector3(0.0f,0.0f,0.0f);
	Vector3 startRotFR = new Vector3(0.0f,0.0f,0.0f);
	// Vector3 startRotRL = new Vector3(0.0f,0.0f,0.0f); 
	// Vector3 startRotRR = new Vector3(0.0f,0.0f,0.0f);
	// Vector3 startPosFL = new Vector3(0.0f,0.0f,0.0f);
	// Vector3 startPosRL = new Vector3(0.0f,0.0f,0.0f);
	// Vector3 startPosRR = new Vector3(0.0f,0.0f,0.0f);
	// Vector3 startPosFR = new Vector3(0.0f,0.0f,0.0f);
	public Vector3 shiftCentre = new Vector3(0.0f, -0.5f, 0.0f); 
	
	// Use this for initialization
	void Start () {
		
		startRotFL = wheelFL.localRotation.eulerAngles;
		startRotFR = wheelFR.localRotation.eulerAngles;
		// startRotRL = wheelRL.localRotation.eulerAngles;
		// startRotRR = wheelRR.localRotation.eulerAngles;
		
		// startPosFL = wheelFL.localPosition;
		// startPosFR = wheelFR.localPosition;
		// startPosRL = wheelRL.localPosition;
		// startPosRR = wheelRR.localPosition;
		
		rigidbody.centerOfMass += shiftCentre;
		
		
		
	}
	
	// Update is called once per frame
	void Update () {


		float v = Input.GetAxis ("Vertical") * MotorForce;
		float h = Input.GetAxis ("Horizontal") * SteerForce;
		
		
		WheelColRL.motorTorque = v;
		WheelColRR.motorTorque = v;
		
		WheelColFL.steerAngle = h;
		WheelColFR.steerAngle = h;
		
		if(Input.GetKey (KeyCode.Space))
		{
			WheelColRL.brakeTorque = BrakeForce;
			WheelColRR.brakeTorque = BrakeForce;
			
		}
		
		if(Input.GetKeyUp (KeyCode.Space))
		{
			WheelColRL.brakeTorque = 0;
			WheelColRR.brakeTorque = 0;
		}


		
		wheelFL.transform.localRotation = WheelColFL.transform.rotation * Quaternion.Euler (startRotFL[0] , startRotFL[1]+ WheelColFL.steerAngle, startRotFL[2]);
		wheelFR.transform.localRotation = WheelColFR.transform.rotation * Quaternion.Euler (startRotFR[0] , startRotFR[1]+ WheelColFL.steerAngle, startRotFR[2]);
		//wheelRL.transform.localRotation = WheelColRL.transform.rotation * Quaternion.Euler (startRotRL[0] , startRotRL[1], startRotRL[2]);
		//wheelRR.transform.localRotation = WheelColRR.transform.rotation * Quaternion.Euler (startRotRR[0] , startRotRR[1], startRotRR[2]);



		wheelFL.Rotate (0f,-RotationValue, 0f);
		wheelFR.Rotate (0f,-RotationValue, 0f);
		wheelRL.Rotate (0f,-RotationValue, 0f);
		wheelRR.Rotate (0f,-RotationValue, 0f);
		
		RotationValue += WheelColFL.rpm * ( 360/60 ) * Time.deltaTime;


		WheelColFL.transform.localPosition = new Vector3(wheelFL.localPosition[0],wheelFL.localPosition[1],wheelFL.localPosition[2]);
		
		WheelColFL.center = Vector3.zero;
		
		WheelColFR.transform.localPosition = new Vector3(wheelFR.localPosition[0],wheelFR.localPosition[1],wheelFR.localPosition[2]);
		
		WheelColFR.center = Vector3.zero;

		 
		WheelColRL.transform.localPosition = new Vector3(wheelRL.localPosition[0],wheelRL.localPosition[1],wheelRL.localPosition[2]);
	
		WheelColRL.center = Vector3.zero;

		WheelColRR.transform.localPosition = new Vector3(wheelRR.localPosition[0],wheelRR.localPosition[1],wheelRR.localPosition[2]);
		
		WheelColRR.center = Vector3.zero;








		// define a hit point for the raycast collision
		RaycastHit hit;
		// Find the collider's center point, you need to do this because the center of the collider might not actually be
		// the real position if the transform's off.
		Vector3 ColliderCenterPoint = WheelColRL.transform.TransformPoint(WheelColRL.center);
		
		// now cast a ray out from the wheel collider's center the distance of the suspension, if it hit something, then use the "hit"
		// variable's data to find where the wheel hit, if it didn't, then se tthe wheel to be fully extended along the suspension.
		if (Physics.Raycast(ColliderCenterPoint, -WheelColRL.transform.up, out hit, WheelColRL.suspensionDistance + WheelColRL.radius))
		{
			//wheelRL.transform.localPosition.Set (startPosRL[0], startPosRL[1] + hit.point[1] + WheelColRL.radius, startPosRL[2]);
			//= startPosFL + Vector3(0f, (hit.point[1] + WheelColFL.radius), 0f);
			Debug.Log ("Hit " + wheelRL.transform.localPosition);
		}
		else
		{
			//wheelRL.transform.localPosition.Set (startPosRL[0], startPosRL[1] + ColliderCenterPoint[1] - WheelColRL.suspensionDistance, startPosRL[2]);
			//= startPosFL + Vector3(0f, (ColliderCenterPoint[1] - WheelColFL.suspensionDistance),0f);
			Debug.Log ("No Hit " + wheelRL.transform.localPosition);
		}
		
		// now set the wheel rotation to the rotation of the collider combined with a new rotation value. This new value
		// is the rotation around the axle, and the rotation from steering input.
		//transform.rotation = CorrespondingCollider.transform.rotation * Quaternion.Euler(RotationValue, CorrespondingCollider.steerAngle, 0);
		// increase the rotation value by the rotation speed (in degrees per second)
		//RotationValue += CorrespondingCollider.rpm * 6 * Time.deltaTime; // 6 = 360 degrees per revolution / 60 seconds per minute
		
		// define a wheelhit object, this stores all of the data from the wheel collider and will allow us to determine
		// the slip of the tire.
		WheelHit CorrespondingGroundHit;
		WheelColFL.GetGroundHit(out CorrespondingGroundHit);
		
		// if the slip of the tire is greater than 2.0, and the slip prefab exists, create an instance of it on the ground at
		// a zero rotation.
		if (Mathf.Abs(CorrespondingGroundHit.sidewaysSlip) > 2.0)
		{
			// Sliding. Launch dust effect
		}
		
		
	}
}