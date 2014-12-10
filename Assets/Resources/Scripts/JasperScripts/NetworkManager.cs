using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {
	public GameObject player1;
	public GameObject player2;
	public GameObject level;

	private string IP = "IP";
	public int port = 21234;

	private bool connected;
	private float btnX;
	private float btnY;
	private float btnW;
	private float btnH;

	private Vector3 spawn = new Vector3(-10,1,0);
	private Vector3 offset = new Vector3(5,0,0);
	private Vector3 levelSpawn = new Vector3(0,0,0);

	// variables for buttons
	void Start () {
		btnX = Screen.width * 0.1f;
		btnY = Screen.height * 0.1f;
		btnW = Screen.width * 0.1f;
		btnH = Screen.width * 0.05f;
	}
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


	//GUI
	void OnGUI(){
		if (!Network.isClient && !Network.isServer){
			if(GUI.Button(new Rect(btnX, btnY, btnW, btnH), "Create Game")){
				Debug.Log("Start server");
				Network.InitializeServer(4, port, true);
			}

			IP = GUI.TextField(new Rect(btnX, btnY*4f +btnH, btnW, btnH*0.5f), IP);
			if(GUI.Button(new Rect(btnX, btnY *5f +btnH, btnW, btnH), "Find Game")){
				Debug.Log("joining game, IP: "+IP);
				Network.Connect(IP,port);
			}	
		}
	}
}