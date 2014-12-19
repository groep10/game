using UnityEngine;
using System.Collections;

public class respawn : MonoBehaviour {

	private bool upsideDown = false;

	void spawnPlayer(){
		transform.rotation = Quaternion.identity;
		upsideDown = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.rotation.eulerAngles.z > 120 && transform.rotation.eulerAngles.z < 240) {
			if(!upsideDown){
				upsideDown = true;
				//Debug.Log("Player is upside-down");
				Invoke("spawnPlayer", 3);
			}
		}
	
	}
}
