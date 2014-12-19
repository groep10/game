using UnityEngine;
using System.Collections;

public class BulletCollision : MonoBehaviour {

	//When bullet particle hits something
	void OnParticleCollision(GameObject other) {
		Debug.Log ("collison detected");
		Rigidbody body = other.rigidbody;
		if (networkView.isMine && body.tag == "Enemy") {
			Debug.Log("destroying " +other.tag );
			Network.Destroy(other.networkView.viewID);
			Network.RemoveRPCs(other.networkView.viewID);
		}
		else if(networkView.isMine && body.tag == "Player"){
			//Do RPC stuff
		}
	}
}
