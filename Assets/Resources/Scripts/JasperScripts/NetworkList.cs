using UnityEngine;
using System.Collections;

public class NetworkList : MonoBehaviour {

	public GameObject player1;
	public GameObject player2;
	public GameObject level;
	
	private int port;
	private bool searching;
	private bool create;
	private HostData[] hostdata;

	private string gName = "MinorProject_Arena_RacingGame";
	private string customName = "Game name";

	private Vector3 spawn = new Vector3(-10,1,0);
	private Vector3 offset = new Vector3(5,0,0);
	private Vector3 levelSpawn = new Vector3(0,0,0);

	private float btnX;
	private float btnY;
	private float btnW;
	private float btnH;
	
	// variables for buttons
	void Start () {
		btnX = Screen.width * 0.1f;
		btnY = Screen.height * 0.1f;
		btnW = Screen.width * 0.1f;
		btnH = Screen.width * 0.05f;
	}

	// Keep searching until you find a server
	void Update(){
		if(searching){
			if(MasterServer.PollHostList ().Length > 0){
				searching=false;
				hostdata = MasterServer.PollHostList (); 
			}
		}
	}

	// Start a server and register on masterserver
	void startServer(){
		port = Random.Range (20000, 25000);
		Network.InitializeServer(4, port, false);
		MasterServer.RegisterHost(gName,customName,"Arena racing");
	}

	// Spawn level on network and player
	void OnServerInitialized(){
		Network.Instantiate (level, levelSpawn, Quaternion.identity, 0);
		spawnPlayer1();
	}

	// Start seaching for active servers
	void findGames(){
		MasterServer.RequestHostList (gName);
		searching = true;
		MasterServer.PollHostList ();
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
		Network.Instantiate(player1, spawn, Quaternion.identity, 0);
	}

	// Instantiate player2 on network
	void spawnPlayer2(){
		Network.Instantiate(player2, spawn, Quaternion.identity, 0);
	}
	
	// Spawn players next to each other
	[RPC]
	void spawning(int num){
		spawn = spawn + offset * num;
		spawnPlayer2();
	}
	
	
	//GUI
	void OnGUI(){
		if (!Network.isClient && !Network.isServer){
			// Create game button
			if(GUI.Button(new Rect(btnX, btnY, btnW, btnH), "Create Game")){
				create = true;
			}
			// When creating a game show texfield and start button
			if(create){
				customName = GUI.TextField(new Rect(btnX, btnY+btnH*1.5f, btnW, btnH*0.5f), customName);
				if(GUI.Button(new Rect(btnX, btnY+btnH*2, btnW, btnH*0.5f), "Start")){
					startServer();
				}
			}
			// Hide Find game button when creating a game 
			if(!create){
				//join game button
				if(GUI.Button(new Rect(btnX, btnY*5 +btnH, btnW, btnH), "Find Game")){
					findGames();
				}
			}
			// Create button for all found servers
			if(hostdata!=null){
				for(int i=0;i<hostdata.Length;i++){
					if(GUI.Button(new Rect(btnX*3, btnY*(i+1), btnW, btnH), hostdata[i].gameName)){
						Network.Connect(hostdata[i]);
					}
				}
			}
		}
	}
}
