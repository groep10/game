using UnityEngine;
using System.Collections.Generic;
using System.Collections;

using Game.Net;
using Game.UI;

namespace Game.Level.TopRace {
	public class topCheckpoint : MonoBehaviour {

		public GameObject winner;
		
		public bool winnerReachedCheckpoint = false;

		public TopRaceMode mode;

		void OnTriggerEnter(Collider other) {
			GameObject obj = other.gameObject;
			if(Network.isClient) {
				return;
			}
			if (obj.tag == "Player") {
				if(winnerReachedCheckpoint) {
					return;
				}
				winnerReachedCheckpoint = true;
				mode.onReachCheckpoint(obj.GetComponent<PlayerInfo>().getUsername());
			}
		}
	}
}