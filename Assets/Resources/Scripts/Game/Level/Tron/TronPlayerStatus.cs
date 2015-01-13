using UnityEngine;
using System.Collections;

using Game;

namespace Game.Level.Tron {

	public class TronPlayerStatus : MonoBehaviour {

		public bool dead = false;

		CarController control;

		void Start () {
			control = GetComponent<CarController> ();
		}

		void OnCollisionEnter(Collision coll) {
			if (dead) {
				return;
			}
		
			if (coll.gameObject.tag == "TronLineSegment") {
				Debug.Log ("Hit line");
				dead = true;
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
