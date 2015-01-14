using UnityEngine;
using System.Collections.Generic;
using System.Collections;

using Game.Net;
using Game.UI;

namespace Game.Level.TopRace {
	public class topCheckpoint : MonoBehaviour {

		private List<NetworkViewID> playerOrder = new List<NetworkViewID>();

		void OnTriggerEnter(Collider other) {

				GameObject obj = other.gameObject;
				if (obj.tag == "Player") {
					Debug.Log ("Player entered the topCheckpoint trigger");
					Debug.Log ("Player viewID is: " + obj.networkView.viewID);

					if (!playerOrder.Contains(obj.networkView.viewID)) {
						networkView.RPC("playerReachedCheckpoint", RPCMode.AllBuffered, obj.networkView.viewID);
					}

				}
		}

		[RPC]
		public void playerReachedCheckpoint(NetworkViewID id) {
			playerOrder.Add(id);
			
			// find the players name based on the viewID
			PlayerInfo info = NetworkView.Find(id).gameObject.GetComponent<PlayerInfo>();
			string name = info.getUsername();

			// make the player win the mini-game
			Game.Controller.getInstance().scores.playerWinsTopRace(name);
		}
	}
}