using UnityEngine;
using System.Collections;

namespace Game.Net {
	public class Manager : MonoBehaviour {

		public GameObject player1Prefab;
		public GameObject player2Prefab;

		private Vector3 spawn = new Vector3(20, 1, 0);
		private Vector3 offset = new Vector3(15, 0, 0);

		// Spawn level on network and player
		void OnServerInitialized(){
			Game.Controller.getInstance ().networkView.RPC ("startGame", RPCMode.AllBuffered);
			// Network.Instantiate (level, levelSpawn, Quaternion.identity, 0);
			spawnPlayer1();
		}

		// Remove player on disconnect
		void OnPlayerDisconnected(NetworkPlayer player) {
			Network.RemoveRPCs(player);
			Network.DestroyPlayerObjects(player);
		}

		// Let new player spawn on connect
		void OnPlayerConnected(NetworkPlayer player){
			networkView.RPC ("spawning", player, Network.connections.Length);
			if (Network.connections.Length == 3){
				// value 0 nobody can join even if someone leaves
				// value -1 when someone leaves a new player can join
				Network.maxConnections = 0; 
			}
		}
		 
		// Instantiate player1 on network
		void spawnPlayer1(){
			Network.Instantiate(player1Prefab, spawn, Quaternion.identity, 0);
			setCanvas ();
		}

		// Instantiate player2 on network
		void spawnPlayer2(){
			Network.Instantiate(player2Prefab, spawn, Quaternion.identity, 0);
			setCanvas ();
		}

		void setCanvas(){
			CanvasController canvascontroller = GameObject.Find ("Canvas").GetComponent<CanvasController>();
			canvascontroller.setCanvasCamera ();
		}
		
		// Spawn players next to each other
		[RPC]
		void spawning(int num){
			spawn = spawn + offset * num;
			spawnPlayer2();
		}
	}
}