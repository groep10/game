using UnityEngine;
using System.Collections;

using Game.Net;

namespace Game.Level.Zombie {
	public class AIEnemy : MonoBehaviour {
		float distance; 
		float lookAtDistance = 150f; 
		float attackRange = 100f;
		float moveSpeed = 20.0f;
		
		private float currentDistance;
		public GameObject[] Targets;
		private GameObject targ;
		private int health = 100;
		private bool dead = false;

		void Start() {
			Targets = Game.Controller.getInstance ().getPlayers ();
		}﻿


		void Update() {
			if (dead) {
				if(Network.isServer && !dead) {
					dead = true;
				}
				return;
			}
			currentDistance = int.MaxValue;
			foreach (GameObject go in Targets) {
				distance = Vector3.Distance(go.transform.position, transform.position);
				if (distance < currentDistance) {
					currentDistance = distance;
					targ = go;
				}
			}
			if (currentDistance < lookAtDistance) {
				LookAt();
			}
			if (currentDistance < attackRange) {
				AttackPlayer();
			}
		}

		void LookAt() {
			Vector3 relatPos = new Vector3 (targ.transform.position.x-transform.position.x,0,targ.transform.position.z-transform.position.z);
			Quaternion rotation = Quaternion.LookRotation(relatPos);
			transform.rotation = Quaternion.Slerp(transform.rotation,rotation,0.1f);
		}

		void AttackPlayer() {
			transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
		}

		//RPC calls
		[RPC]
		void damage(int dam, NetworkViewID shooter) {
			health -= dam;
			Debug.Log("health: " + health);
			if (health <= 0 && Network.isServer && !dead) {
				dead = true;
				Debug.Log("killed by: " + shooter);
				//minigamePoint(NetworkView.Find(shooter).gameObject.GetComponent<PlayerInfo>().getUsername());
				networkView.RPC("minigamePoint", RPCMode.AllBuffered, NetworkView.Find(shooter).gameObject.GetComponent<PlayerInfo>().getUsername());
			}
		}

		[RPC]
		void minigamePoint(string playername) {
			Game.Controller.getInstance().scores.increasePlayerZombieScore(playername);
			if(Network.isServer) {
				Network.Destroy(this.gameObject);
				Network.RemoveRPCs(networkView.viewID);
			}
			// GameObject.FindObjectOfType<Game.Level.Manager>().increasePlayerMinigameScore(playername);
		}
	}
}