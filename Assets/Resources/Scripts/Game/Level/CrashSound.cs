using UnityEngine;
using System.Collections;

public class CrashSound : MonoBehaviour {
	
	void OnCollisionEnter(Collision other) {
		if(other.gameObject.tag == "Player"){
			if (other.relativeVelocity.magnitude > 4){
				audio.Play ();
			}
		}
	}
}