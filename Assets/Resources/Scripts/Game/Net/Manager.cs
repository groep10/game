using UnityEngine;
using System.Collections;

using Game.Web;
using Game.Menu;

namespace Game.Net {
	public class Manager : MonoBehaviour {

		public GameObject playerPrefab;
		public GameObject menuObjects;

		private int maxPlayers = 4;
		private int readyPlayers = 0;
		private Vector3 spawn = new Vector3(40, 1, 0);
		private Vector3 offset = new Vector3(15, 0, 0);

		// Spawn level on network and player
		void OnServerInitialized(){
			spawnPlayer();
			Game.Controller.getInstance ().initialzeGame();
			checkPlayerCount();
		}
		
		void checkPlayerCount() {
			if(Network.isClient){
				return;
			}

			if (Network.connections.Length >= maxPlayers){
				Network.maxConnections = 0;
				Debug.Log("game is full close server");
			}
		}

		void OnFailedToConnect(NetworkConnectionError error) {
			if(error == NetworkConnectionError.TooManyConnectedPlayers){
				menuObjects.GetComponentInChildren<ServerListController> ().connectionStatus (false, "full");
			}
			else{
				menuObjects.GetComponentInChildren<ServerListController> ().connectionStatus (false, "error");
			}
		}
		
		void OnConnectedToServer() {
			menuObjects.GetComponentInChildren<ServerListController> ().connectionStatus (true, "");
		}

		// Remove player on disconnect
		void OnPlayerDisconnected(NetworkPlayer player) {
			Network.RemoveRPCs(player);
			Network.DestroyPlayerObjects(player);
		}

		// Let new player spawn on connect
		void OnPlayerConnected(NetworkPlayer player){
			networkView.RPC ("spawning", player, Network.connections.Length);
			checkPlayerCount();
		}
		 
		// Instantiate player1 on network
		void spawnPlayer(){
			Network.Instantiate(playerPrefab, spawn, Quaternion.identity, 0);
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

		[RPC]
		void onReady() {
			Debug.Log("ready received");
			if(Network.isClient) {
				return;
			}
			readyPlayers++;
			Debug.Log("ready " + readyPlayers + " " + maxPlayers);
			if(readyPlayers >= maxPlayers) {
				Game.Controller.getInstance ().serverBegin();
			}
		}
	}
}