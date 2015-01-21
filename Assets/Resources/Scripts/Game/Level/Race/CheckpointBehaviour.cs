using UnityEngine;
using System.Collections.Generic;
using System.Collections;

using Game.Net;
using Game.UI;

namespace Game.Level.Race {

	public class CheckpointBehaviour : MonoBehaviour {


		public RaceMode mode;

		void OnTriggerEnter(Collider other) {
			if (Network.isClient) {
				return;
			}

			Debug.Log("checkpoint trigger entered");
			GameObject obj = other.gameObject;
			string name = obj.GetComponent<PlayerInfo>().getUsername();

			mode.onCheckpointReached(name);
		}

	}
}