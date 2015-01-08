using UnityEngine;
using System.Collections;

using Game.UI;
using Game.Level; 

namespace Game {
	public class Controller : MonoBehaviour {

		public ScoreController minigameScores;
		public ScoreController globalScores;

		public TerrainManager terrainManager;

		private Mode activeMode;

		public BaseMode mainMode;
		public BaseMode[] miniModes;

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

		[RPC]
		public void startGame() {
			activeMode = mainMode;
			activeMode.beginMode(() => {
				startMiniGame();
			});
		}

		[RPC]
		public void startMiniGame() {
			activeMode = miniModes[Random.Range(0, miniModes.Length)];
			activeMode.beginMode(() => {
				startGame();
			});
		}

		void Update() {
			if (activeMode != null && activeMode.isActive()) {
				activeMode.onTick();
			}
		}

		public static Controller getInstance() {
			return GameObject.FindObjectOfType<Game.Controller>();
		}
		
	}
}