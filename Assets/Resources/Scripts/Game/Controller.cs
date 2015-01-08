using UnityEngine;
using System.Collections;

using Game.UI;

namespace Game {
	public class Controller : MonoBehaviour {

/* ==================================== VARIABLES ======================================================= */

		public ScoreController minigameScores;
		public ScoreController globalScores;

		private Mode activeMode;

/* ===================================== FUNCTIONS ======================================================= */

		// Returns a list of all the players in the game
		public GameObject[] getPlayers() {
			return GameObject.FindGameObjectsWithTag("Player");
		}

		// Returns the active player GameObject
		public GameObject getActivePlayer() {
			foreach(GameObject player in getPlayers()) {
				if(player.networkView.isMine) {
					return player;
				}
			}
			return null;
		}

		// Returns the current working Controller instance
		public static Controller getInstance() {
			return GameObject.FindObjectOfType<Game.Controller>();
		}

/* ==================================== AWAKE, START & UPDATE ============================================== */

		// Called every frame
		void Update() {
			if (activeMode != null && activeMode.isActive()) {
				activeMode.onTick();
			}
		}
		
	}
}