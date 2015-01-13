﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections;

using Game.Net;
using Game.UI;

namespace Game.Level.Race {

	public class CheckpointBehaviour : MonoBehaviour {

		private List<NetworkViewID> playerOrder = new List<NetworkViewID>();

		void OnTriggerEnter(Collider other) {
			GameObject obj = other.gameObject;
			if (obj.tag == "Player") {
				Debug.Log ("Player entered the Checkpoint trigger");
				Debug.Log ("Player viewID is: " + obj.networkView.viewID);

				if (!playerOrder.Contains(obj.networkView.viewID)) {
					networkView.RPC("playerReachedCheckpoint", RPCMode.AllBuffered, obj.networkView.viewID);
				}

				for (int i = 0; i < playerOrder.Count; i++) {
					int rank = i + 1;
					Debug.Log ("Player" + rank + ": " + playerOrder[i]);
				}
			}
		}

		[RPC]
		public void playerReachedCheckpoint(NetworkViewID id) {
			playerOrder.Add(id);
		}


        public int getReachedCount() {
            return playerOrder.Count;
        }
	}
}