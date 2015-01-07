using UnityEngine;
using System.Collections;

public class CrashSound : MonoBehaviour {
	
	void OnCollisionEnter(Collision other) {
		if(other.gameObject.tag == "Player"){
			Debug.Log ("hit");
			audio.Play ();
		}
	}
}
