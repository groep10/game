using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {
	public GameObject player1;
	public GameObject player2;
	public Transform spawnObject;

	private string IP = "IP";
	public int port = 21234;

	private bool connected;
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
	//Instantiate player1 on network
	void spawnPlayer1(){
		Network.Instantiate(player1, spawnObject.position, Quaternion.identity, 0);
	}
	//Instantiate player2 on network
	void spawnPlayer2(){
		Network.Instantiate(player2, spawnObject.position, Quaternion.identity, 0);
	}
	//Spawn host
	void OnServerInitialized(){
		spawnPlayer1();
	}
	//Spawn clients
	void OnConnectedToServer(){
		spawnPlayer2();
	}
	//Remove player on disconnect
	void OnPlayerDisconnected(NetworkPlayer player) {
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
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