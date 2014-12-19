using UnityEngine;
using System.Collections;

public class respawn : MonoBehaviour {

	private bool upsideDown = false;

	void spawnPlayer(){
		transform.rotation = Quaternion.identity;
		upsideDown = false;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log("Rotation z: " + transform.rotation.eulerAngles.z);
		if (transform.rotation.eulerAngles.z > 120 && transform.rotation.eulerAngles.z < 240) {
			if(!upsideDown){
				upsideDown = true;
				//Debug.Log("Player is upside-down");
				Invoke("spawnPlayer", 3);
			}
		}
	
	}
}
