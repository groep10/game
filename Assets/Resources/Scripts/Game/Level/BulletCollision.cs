using UnityEngine;
using System.Collections;

public class BulletCollision : MonoBehaviour {

	//When bullet particle hits something
	void OnParticleCollision(GameObject other) {
		if(other.rigidbody != null){
			Rigidbody body = other.rigidbody;
			if (networkView.isMine && body.tag == "Enemy") {
				other.networkView.RPC("damage", RPCMode.AllBuffered, 10, transform.parent.gameObject.networkView.viewID);
			} 
			else if(networkView.isMine && body.tag == "Player"){
				//Do RPC stuff
			}
		}
	}
}
