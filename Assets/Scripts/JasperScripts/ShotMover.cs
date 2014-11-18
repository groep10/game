using UnityEngine;
using System.Collections;

public class ShotMover : MonoBehaviour {
	public int speed = 10; 

	void FixedUpdate(){
		rigidbody.velocity = new Vector3(0,0,speed);
	}
}
