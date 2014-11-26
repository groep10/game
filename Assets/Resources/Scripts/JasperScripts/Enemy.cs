using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	private GameObject closest;
	private GameObject[] playerlist;
	public int health;
	public float speed;
	public float maxspeed;

	private Vector3 Rpos = new Vector3(0,1,30); // start positie voor client
	private Vector3 Epos;
	private Vector3 Rvelo;
	private Vector3 Evelo;

	void Update(){
		if(health <= 0 && Network.isServer){
			Network.Destroy(this.gameObject);
			Network.RemoveRPCs(networkView.viewID);
		}
	}
	
	void FixedUpdate()
	{
		if(Network.isServer && networkView.isMine){
			if(rigidbody.velocity.magnitude < maxspeed){
				Vector3 PlayerDir = closestPlayer().transform.position - transform.position;
				rigidbody.AddForce(PlayerDir*speed);
			}
		}
		else {
			//Client use lerp for smooth enemy position
			transform.position = Vector3.Lerp(transform.position, Rpos, 0.25f)
				+ Rvelo * Time.deltaTime;
		}
	}
	//Write and read variables
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

	GameObject closestPlayer(){
		playerlist = GameObject.FindGameObjectsWithTag("Player");
		float closestDistance = Mathf.Infinity;
		foreach (GameObject player in playerlist) {
			float distance = Vector3.Distance(transform.position,player.transform.position);
			if (distance < closestDistance) {
				closest = player;
				closestDistance = distance;
			}
		}
		return closest;
	}

	//RPC calls
	[RPC]
	void Damage(int dam, string shooter){
		health -= dam;
		Debug.Log("health: "+health);
		Debug.Log("shooter: "+shooter);
		if(health <= 0){
			Debug.Log("killed by: "+shooter);
		}
	}
}