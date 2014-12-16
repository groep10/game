using UnityEngine;
using System.Collections;

public class Fire : MonoBehaviour
{
	public Rigidbody bullet;
	public float speed = 10f;
	public float FireRate = .2f;
	public Transform player;

	void Start () {
//		player = GameObject.FindGameObjectWithTag("Player").transform;
		}

	void FireBullet () {
		Rigidbody bulletClone = (Rigidbody) Instantiate(bullet, player.position, player.rotation);
		bulletClone.velocity = player.forward * speed;
	}


	// Calls the fire method when holding down left mouse button
	void Update () {
		player = GameObject.FindGameObjectWithTag("Player").transform;
		if (Input.GetButtonDown("Fire1")) {
			FireBullet ();
//			Destroy (GameObject.Find("Cube(Clone)"));
		}
	}
}