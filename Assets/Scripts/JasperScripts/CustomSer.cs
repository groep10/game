using UnityEngine;
using System.Collections;

public class CustomSer : MonoBehaviour {
	public GameObject enemy;
	public Transform enemySpawn;

	private float moveH;
	private float moveV;
	public float speed;
	private Vector3 movement;
	
	private float distance;
	private Vector3 realpos;
	private Vector3 pos;
	private Vector3 velo;
	private Vector3 realvelo;

	void FixedUpdate () {
		//check if you are controller the object
		if(networkView.isMine){		
			var playercam = transform.Find("Camera1").gameObject;
			playercam.SetActive(true);
			
			moveH = Input.GetAxis ("Horizontal");
			moveV = Input.GetAxis ("Vertical");
			movement = new Vector3 (moveH, 0.0f, moveV);
			
			rigidbody.AddForce(movement*speed);
			
			// shoot 
			if (Input.GetKeyDown(KeyCode.Space)){
				Shoot ();
			}
		}
		//Lerp (interpolation) other player positions for smooth gameplay
		else{
			transform.position = Vector3.Lerp(transform.position, realpos, 0.25f)
				+ realvelo * Time.deltaTime;
		}
	}
	//Write and read variables
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		//write your movements
		if (stream.isWriting) {
			pos = transform.position; 
			velo = rigidbody.velocity;
			stream.Serialize(ref pos);
			stream.Serialize(ref velo);
		} 
		//read other movements
		else {
			stream.Serialize(ref pos);
			stream.Serialize(ref velo);
			realpos = pos;
			realvelo = velo;
		}
	}
	//When someone enters the trigger spawn an enemy (their can only be one)
	void OnTriggerEnter(Collider other){
		if(Network.isServer && GameObject.Find("Enemy(Clone)")==null && other.tag == "EnemyTrigger"){
			Network.Instantiate(enemy, enemySpawn.position, Quaternion.identity, 0);
		}
	}
	//Shoot
	void Shoot(){
			RaycastHit hit;
			Ray shotRay = new Ray (transform.position, Vector3.forward);
			
			if(Physics.Raycast(shotRay, out hit)){
				if(hit.transform.tag == "Enemy"){
					//Let other players know you hit an enemy and dealt damage
					hit.transform.transform.networkView.RPC("Damage",RPCMode.All,5,this.name);
				}
				else if(hit.transform.tag == "Player"){
					//Do stuff
				}
			}
	}
}











