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
		private Vector3 pos, syncPos;
		private Quaternion rot, syncRot;

		public ZombieMode mode;

		void Start() {
			if(Network.isServer){
				Targets = Game.Controller.getInstance ().getPlayers ();
			}
		}﻿

		void Update() {
			if(Network.isServer && dead) {
				Network.Destroy(this.gameObject);
				Network.RemoveRPCs(networkView.viewID);
			}
			if(networkView.isMine){
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
			else{
				transform.position = Vector3.Lerp(transform.position, syncPos, 0.25f);
				transform.rotation = Quaternion.Slerp(transform.rotation, syncRot, 0.25f);
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

		void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
			//write your movements
			if (stream.isWriting) {
				pos = transform.position;
				rot = transform.rotation;
				stream.Serialize(ref pos);
				stream.Serialize(ref rot);
			}
			//read other movements
			else {
				stream.Serialize(ref pos);
				stream.Serialize(ref rot);
				syncPos = pos;
				syncRot = rot;
			}
		}

		//RPC calls
		[RPC]
		void damage(int dam, NetworkViewID shooter) {
			health -= dam;
			if (health <= 0 && Network.isServer && !dead) {
				dead = true;
				mode.broadcastPoint(NetworkView.Find(shooter).gameObject.GetComponent<PlayerInfo>().getUsername());
			}
		}
	}
}