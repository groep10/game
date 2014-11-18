using UnityEngine;
using System.Collections;

public class CustomSer : MonoBehaviour {
	public GameObject shotPrefab;
	public Transform shotSpawn;

	private float moveH;
	private float moveV;
	public float speed;
	private Vector3 movement;
	
	private float distance;
	private Vector3 realpos;
	private Vector3 pos;
	private Vector3 velo;
	private Vector3 realvelo;

	// Update is called once per frame
	void FixedUpdate () {
		if(networkView.isMine){		
			var playercam = transform.Find("Camera1").gameObject;
			playercam.SetActive(true);
			
			moveH = Input.GetAxis ("Horizontal");
			moveV = Input.GetAxis ("Vertical");
			movement = new Vector3 (moveH, 0.0f, moveV);
			
			rigidbody.AddForce(movement*speed);
			
			// shoot 
			if (Input.GetKeyDown(KeyCode.Space)){
				Network.Instantiate(shotPrefab, shotSpawn.position, Quaternion.identity,0);
			}
		}
		//Lerp (interpolation) other player positions for smooth gameplay
		else{
			transform.position = Vector3.Lerp(transform.position, realpos, 0.25f)
				+ realvelo * Time.deltaTime;
		}
	}

	//register player collisions
	void OnCollisionEnter(Collision collision){
		if(collision.gameObject.tag == "Player"){
			Debug.Log("Collision detected");

		}
	}

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
}










