﻿
using UnityEngine;
using System.Collections;

using Game.UI;
using Game.Menu;
using Game.Level; 
using Game.Web;

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

		public void serverStartGame() {
			if(Network.isClient) { return; }
			if (activeMode != null) {
				AccountController.getInstance().createMinigameGameScores(activeMode.getScores(), (res) => {
					activeMode = null;
					serverStartGame();
				});
				return;
			}
			AccountController.getInstance().createMinigameGame(mainMode.getName(), (res) => {
				networkView.RPC("startGame", RPCMode.AllBuffered);
			});
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

					serverStartMiniGame();
				}
			});
		}

		public void serverStartMiniGame() {
			if(Network.isClient) { return; }
			if (activeMode != null) {
				AccountController.getInstance().createMinigameGameScores(activeMode.getScores(), (res) => {
					activeMode = null;
					serverStartMiniGame();
				});
				return;
			}
			// Server decides what minigame to play next.
			int nextId = Random.Range(0, miniModes.Length);
			AccountController.getInstance().createMinigameGame(miniModes[nextId].getName(), (res) => {
				networkView.RPC("startMiniGame", RPCMode.AllBuffered, nextId);
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

					serverStartGame();
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