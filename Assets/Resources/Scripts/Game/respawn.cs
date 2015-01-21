
using UnityEngine;
using System.Collections;
using Game.Level;
using UnityEngine.UI;

namespace Game {

	public class respawn : MonoBehaviour {

		private bool upsideDown = false;
		public Text respawnText;


		// rotates the player up again.
		void resetRotation(){
			float x = transform.rotation.eulerAngles.x;
			float y = transform.rotation.eulerAngles.y;

			transform.rotation = Quaternion.Euler(x, y, 0f);
			upsideDown = false;
		}

		// resets the player to a random location within the arena
		public void resetPlayer(){
			float x = Random.Range (-500, 500);
			float z = Random.Range (-500, 500);

			transform.position = new Vector3 (x, 3f, z);
			transform.rotation = Quaternion.identity;

			rigidbody.velocity = Vector3.zero;
		}
		
		// Update is called once per frame
		void Update () {
			if (transform.rotation.eulerAngles.z > 90 && transform.rotation.eulerAngles.z < 270) {
				if(!upsideDown){
					upsideDown = true;
					//Debug.Log("Player is upside-down");
					Invoke("resetRotation", 3);
				}
			}
			
			respawnText.text = "Resetting";

			if(Input.GetKeyDown(KeyCode.R)){
				if(Game.Controller.getInstance().activeMode.ToString() == "TronMode (Game.Level.TronMode)"){
					Debug.Log ("tronmode");
					return;
				}
				Debug.Log("Resetting player....");

				Invoke("resetPlayer", 5);
				respawnText.enabled = true; 
				}
			}	
		}
	}