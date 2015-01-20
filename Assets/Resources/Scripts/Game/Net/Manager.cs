using UnityEngine;
using System.Collections;

using Game.Web;

namespace Game.Net {
	public class Manager : MonoBehaviour {

		public GameObject playerPrefab;

		private int maxPlayers = 4;
		private Vector3 spawn = new Vector3(40, 1, 0);
		private Vector3 offset = new Vector3(15, 0, 0);

		// Spawn level on network and player
		void OnServerInitialized(){
			spawnPlayer();
			Game.Controller.getInstance ().serverStartGame();
			if(Network.connections.Length == maxPlayers){
				Network.maxConnections = 0; 
			}
			// Network.Instantiate (level, levelSpawn, Quaternion.identity, 0);
		}

		// Remove player on disconnect
		void OnPlayerDisconnected(NetworkPlayer player) {
			Network.RemoveRPCs(player);
			Network.DestroyPlayerObjects(player);
		}

		// Let new player spawn on connect
		void OnPlayerConnected(NetworkPlayer player){
			networkView.RPC ("spawning", player, Network.connections.Length);
			if (Network.connections.Length == maxPlayers){
				Network.maxConnections = 0; 
			}
		}
		 
		// Instantiate player1 on network
		void spawnPlayer(){
			Network.Instantiate(playerPrefab, spawn, Quaternion.identity, 0);
			setCanvas ();
		}

		void setCanvas(){
			CanvasController canvascontroller = GameObject.Find ("Canvas").GetComponent<CanvasController>();
			canvascontroller.setCanvasCamera ();
		}

		public void setMaxPlayers(int max){
			maxPlayers = (max-1);
		}

		// Spawn players next to each other
		[RPC]
		void spawning(int num){
			spawn = spawn + offset * num;
			spawnPlayer();
		}
	}
}