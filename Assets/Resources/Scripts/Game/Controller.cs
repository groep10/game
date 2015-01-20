using UnityEngine;
using System.Collections;

using Game.UI;
using Game.Menu;
using Game.Level; 

namespace Game {
	public class Controller : MonoBehaviour {

		[Header ("Scores")]
		public ScoreController scores;
		public MenuList minigameScores;
		public MenuList overallScores;

		[Header ("Modes")]
		public BaseMode mainMode;
		public BaseMode[] miniModes;
		
		[Header ("Other")]
		public CountDown countdown;
		public Explanation explanation;
		public TerrainManager terrainManager;
		public LevelTour leveltour;

		public Mode activeMode;


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
			if(Network.isServer) {
				Network.RemoveRPCs(terrainManager.networkView.viewID);
				terrainManager.networkView.RPC("updateTerrain", RPCMode.AllBuffered, Random.value);
			}
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
			if(Network.isServer) {
				Network.RemoveRPCs(terrainManager.networkView.viewID);
				terrainManager.networkView.RPC("updateTerrain", RPCMode.AllBuffered, Random.value);
			}
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