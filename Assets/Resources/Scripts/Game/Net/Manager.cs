using UnityEngine;
using System.Collections;

namespace Game.Net {
	public class Manager : MonoBehaviour {
		public GameObject player1;
		public GameObject player2;
		public GameObject level;

		private string IP = "IP";
		public int port = 21234;

		private bool connected;

		private Vector3 spawn = new Vector3(-10,1,0);
		private Vector3 offset = new Vector3(5,0,0);
		private Vector3 levelSpawn = new Vector3(0,0,0);

		//Instantiate player1 on network
		void spawnPlayer1(){
			Network.Instantiate(player1, spawn, Quaternion.identity, 0);
		}
		//Instantiate player2 on network
		void spawnPlayer2(){
			Network.Instantiate(player2, spawn, Quaternion.identity, 0);
		}
		//Spawn host
		void OnServerInitialized(){
			Network.Instantiate (level, levelSpawn, Quaternion.identity, 0);
			spawnPlayer1();
		}
		//Remove player on disconnect
		void OnPlayerDisconnected(NetworkPlayer player) {
			Network.RemoveRPCs(player);
			Network.DestroyPlayerObjects(player);
		}
		//Let new player spawn on connect
		void OnPlayerConnected(NetworkPlayer player){
			networkView.RPC ("spawning", player, Network.connections.Length);
			if (Network.connections.Length == 3){
				// value 0 nobody can join even if someone leaves
				// value -1 when someone leaves a new player can join
				Network.maxConnections = 0; 
			}
		}
		//RPC calls
		[RPC]
		void spawning(int num){
			spawn = spawn + offset * num;
			spawnPlayer2();
		}
	}
}