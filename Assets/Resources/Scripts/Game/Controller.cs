﻿using UnityEngine;
using System.Collections;

using Game.UI;

namespace Game {
	public class Controller : MonoBehaviour {

		public ScoreController minigameScores;
		public ScoreController globalScores;

		public GameObject[] getPlayers() {
			return GameObject.FindGameObjectsWithTag("Player");
		}

		public GameObject getActivePlayer() {
			foreach(GameObject player in getPlayers()) {
				if(player.networkView.isMine) {
					return player;
				}
			}
			return null;
		}

		public static Controller getInstance() {
			return GameObject.FindObjectOfType<Game.Controller>();
		}
		
	}
}