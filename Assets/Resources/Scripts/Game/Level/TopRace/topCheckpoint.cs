using UnityEngine;
using System.Collections.Generic;
using System.Collections;

using Game.Net;
using Game.UI;

namespace Game.Level.TopRace {
	public class topCheckpoint : MonoBehaviour {

		public NetworkViewID winner;
		public bool winnerReachedCheckpoint = false;

		void OnTriggerEnter(Collider other) {
			GameObject obj = other.gameObject;
			if(Network.isClient) {
				return;
			}
			if (obj.tag == "Player") {
				if(winnerReachedCheckpoint) {
					return;
				}
				networkView.RPC("playerReachedCheckpoint", RPCMode.All, obj.networkView.viewID);
			}
		}

		[RPC]
		public void playerReachedCheckpoint(NetworkViewID id) {
			if(winnerReachedCheckpoint) {
				return;
			}

			winnerReachedCheckpoint = true;

			winner = id;
		}
	}
}