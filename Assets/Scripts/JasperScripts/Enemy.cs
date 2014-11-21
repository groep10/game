using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	public int health;
	public float speed;
	public float maxspeed;
	private Vector3 left = new Vector3(-1,0,0);
	private Vector3 right = new Vector3(1,0,0);
	private bool moveR = true;

	private Vector3 Rpos;
	private Vector3 Epos;
	private Vector3 Rvelo;
	private Vector3 Evelo;

	void Update(){
		if(health <= 0 && Network.isServer){
			Network.Destroy(this.gameObject);
		}
	}
	
	void FixedUpdate()
	{
		if(Network.isServer && networkView.isMine){
			// move enemy between x position -6 and 6 
			if (rigidbody.position.x < -6){
				moveR = true;
			}
			if (rigidbody.position.x > 6){
				moveR = false;
			}
			if(rigidbody.velocity.magnitude < maxspeed){
				if(moveR){
					rigidbody.AddForce(right*speed);
				}
				else{
					rigidbody.AddForce(left*speed);
				}
			}
		}
		else {
			transform.position = Vector3.Lerp(transform.position, Rpos, 0.25f)
				+ Rvelo * Time.deltaTime;
		}
	}

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		//write your movements
		if (stream.isWriting) {
			Epos = transform.position; 
			Evelo = rigidbody.velocity;
			stream.Serialize(ref Epos);
			stream.Serialize(ref Evelo);
		} 
		//read other movements
		else {
			stream.Serialize(ref Epos);
			stream.Serialize(ref Evelo);
			Rpos = Epos;
			Rvelo = Evelo;
		}
	}

	//RPC calls
	[RPC]
	void Damage(int dam){
		health -= dam;
		Debug.Log("health: "+health);
	}

}