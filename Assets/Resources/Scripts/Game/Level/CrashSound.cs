using UnityEngine;
using System.Collections;

public class CrashSound : MonoBehaviour {
	
	void OnCollisionEnter(Collision other) {
		if(other.gameObject.tag == "ArenaAssets"){
			float volume = (other.relativeVelocity.magnitude / 100);
			audio.Play ();
			audio.volume = volume;
		}
	}
}