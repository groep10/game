﻿using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {
	public GameObject player1;
	public GameObject player2;
	public Transform spawnObject;

	private string IP = "127.0.0.1";
	public int port = 25101;

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

	//Instantiate players
	void spawnPlayer1(){
		Network.Instantiate(player1, spawnObject.position, Quaternion.identity, 0);
	}

	void spawnPlayer2(){
		Network.Instantiate(player2, spawnObject.position, Quaternion.identity, 0);
	}
	

	//Spawn players on initialization and connection
	void OnServerInitialized(){
		Debug.Log("Server initialized");
		spawnPlayer1();
	}

	void OnConnectedToServer(){
		spawnPlayer2();
	}
	
	//GUI
	void OnGUI(){
		if (!Network.isClient && !Network.isServer){
			if(GUI.Button(new Rect(btnX, btnY, btnW, btnH), "Create Game")){
				Debug.Log("Start server");
				Network.InitializeServer(4, port, true);
			}
			
			if(GUI.Button(new Rect(btnX, btnY *1.2f +btnH, btnW, btnH), "Find Game")){
				Debug.Log("Searching");
				Network.Connect(IP,port);
			}	
		}
	}
}