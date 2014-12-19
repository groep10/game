﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections;

using Game.Net;
using Game.UI;

public class CheckpointBehaviour : MonoBehaviour {

	private ArrayList playerOrder = new ArrayList();
	private GameObject arena;
	public GameObject enemyManager;
	private float racingTimeLimit = 10;

	private bool runningMiniGame = false;

    private ScoreController scores;

    void Awake()
    {
        //InvokeRepeating("findPlayers", 5f, 5f);   
        scores = GameObject.FindObjectOfType<ScoreController>();
        scores.addScore("Mode: racing");

        Invoke("startMinigame", racingTimeLimit);
    }

    void findPlayers()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        
        foreach (GameObject player in players)
        {
            if (player.GetComponent<PlayerInfo>() == null)
            {
                Debug.Log("cannot find component " + player.networkView.viewID + " " + player.networkView.owner.ipAddress);
                continue;
            }
            //string name = player.GetComponent<PlayerInfo>().getUsername();
            Debug.Log(player.GetComponent<PlayerInfo>().getUsername() + "[" + player.GetComponent<PlayerInfo>().getUserId() + "] " + player.networkView.viewID + " " + player.networkView.owner.ipAddress);
        }
    }

	void OnTriggerEnter(Collider other){
        GameObject obj = other.gameObject;
		if(obj.tag == "Player"){
			Debug.Log ("Player entered the Checkpoint trigger");
			Debug.Log ("Player viewID is: " + obj.networkView.viewID);

			if(!playerOrder.Contains(obj.networkView.viewID)){
                networkView.RPC("playerReachedCheckpoint", RPCMode.AllBuffered, obj.networkView.viewID);
			}

			for(int i = 0; i < playerOrder.Count; i++){
				int rank = i + 1;
				Debug.Log ("Player" + rank + ": " + playerOrder[i]);
			}
		}
	}

    [RPC]
    public void playerReachedCheckpoint(NetworkViewID id)
    {
        playerOrder.Add(id);
        scores.addScore(NetworkView.Find(id).gameObject.GetComponent<PlayerInfo>().getUsername() + ": #" + playerOrder.Count);
    }

    void startMinigame()
    {
        if (!runningMiniGame)
        {
            runningMiniGame = true;

            Debug.Log("Starting minigame.....");
            // instantiate the enemies from the server
            if (Network.isServer)
            {
                arena = GameObject.FindGameObjectWithTag("Level");
                arena.GetComponent<Level>().destroyCP();
                arena.GetComponent<Level>().editTerrain();

                networkView.RPC("minigameStart", RPCMode.AllBuffered);
                //Invoke("arena.GetComponent<Level>().setCheckpoint", 10);
                Instantiate(enemyManager);
            }
        }
    }

    [RPC]
    public void minigameStart()
    {
        GameObject.FindObjectOfType<Level>().updateMiniGameScores();
    }

	void Update(){
		if(playerOrder.Count > 1){
			startMinigame();
		}
	}

}