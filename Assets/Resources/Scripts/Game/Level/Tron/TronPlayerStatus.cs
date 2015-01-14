using UnityEngine;
using System.Collections;

using Game;
using Game.Net;

namespace Game.Level.Tron {

	public class TronPlayerStatus : MonoBehaviour {

		public bool dead = false;
		CarController control;

		[HideInInspector]
		public TronMode mode;

		void Start () {
			control = GetComponent<CarController> ();
		}

		void OnCollisionEnter(Collision coll) {
			if (dead) {
				return;
			}

			if (!networkView.isMine) {
				return;
			}
			
			if (coll.gameObject.tag == "TronLineSegment") {
				Debug.Log (GetComponent<PlayerInfo>().getUsername() + " hit line");
				dead = true;
				mode.networkView.RPC ("notifyDeath", RPCMode.All, GetComponent<PlayerInfo>().getUsername()); 
			}
		}

		void Update () {
			if (dead) {
				this.rigidbody.velocity = Vector3.zero;
				return;
			}
			control.reversing = false;
			if (control.currentGear < 2) {
				control.currentGear = 2;
			}
			if (control.CurrentSpeed < 40) {
				rigidbody.velocity += transform.TransformDirection(new Vector3(0, 0, 40 - control.CurrentSpeed));
			}
		}
		
	}
}
