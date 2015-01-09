using UnityEngine;
using System.Collections;

using Game.UI;
using Game.Menu;
using Game.Level; 

namespace Game {
	public class Controller : MonoBehaviour {

		public ScoreController scores;
		public MenuList minigameScores;
		public MenuList overalScores;

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
			Debug.Log("starting main");
			activeMode = mainMode;
			activeMode.beginMode(() => {
				if(Network.isServer) {
					// Remove previous start games from buffer.
					Network.RemoveRPCs(networkView.viewID);

					// Server decides what minigame to play next.
					networkView.RPC("startMiniGame", RPCMode.AllBuffered, Random.Range(0, miniModes.Length));
				}
			});
		}

		[RPC]
		public void startMiniGame(int minigame) {
			Debug.Log("starting mini");
			activeMode = miniModes[minigame];
			activeMode.beginMode(() => {
				if(Network.isServer) {
					// Remove previous start games from buffer.
					Network.RemoveRPCs(networkView.viewID);

					networkView.RPC("startGame", RPCMode.AllBuffered);
				}
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