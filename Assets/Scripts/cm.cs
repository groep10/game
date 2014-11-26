using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
	
	public float rotspeed = 10F;
	public float camUD = 0F;
	float rotYold = 0;
	float rotYnew = 0;
	
	void Update () {
		transform.position = transform.parent.position + new Vector3 (0, 1, 0);    // Keeps camera 1 unit above player
		
		camUD += Input.GetAxis("Mouse Y") * rotspeed * Time.deltaTime;    // Vertical rotation controlled by mouse up/down
		
		rotYnew = transform.parent.eulerAngles.y;    // Target horizontal rotation matches player horizontal rotation
		rotYold = transform.eulerAngles.y;    // Get current horizontal rotation
		transform.Rotate(0, rotYnew - rotYold, 0);    // Rotate camera horizontally
		
		camUD = Mathf.Clamp (camUD, -60, 60);    // Camera can't rotate vertically farther than 60 degrees
		transform.rotation = Quaternion.Euler(-camUD, transform.eulerAngles.y, 0);    // Rotate camera vertically
	}
}