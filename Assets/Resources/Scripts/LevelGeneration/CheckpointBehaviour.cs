using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class CheckpointBehaviour : MonoBehaviour {

	private ArrayList playerOrder = new ArrayList();
	private GameObject arena;
	public GameObject enemyManager;
	private float racingTimeLimit = 10;

	private bool runningMiniGame = false;

	void OnTriggerEnter(Collider other){
		if(other.gameObject.tag == "Player"){
			Debug.Log ("Player entered the Checkpoint trigger");
			Debug.Log ("Player viewID is: " + other.gameObject.networkView.viewID);

			if(!playerOrder.Contains(other.gameObject.networkView.viewID)){
				playerOrder.Add(other.gameObject.networkView.viewID);		
			}

			for(int i = 0; i<playerOrder.Count; i++){
				int rank = i + 1;
				Debug.Log ("Player" + rank + ": " + playerOrder[i]);
			}
		}
	}
	
	void startMinigame(){
		if(!runningMiniGame){
			runningMiniGame = true;

			Debug.Log ("Starting minigame.....");
			// instantiate the enemies from the server
			if(Network.isServer){
				arena = GameObject.FindGameObjectWithTag("Level");
				arena.GetComponent<Level>().destroyCP();
				arena.GetComponent<Level> ().editTerrain ();

				Invoke("arena.GetComponent<Level>().setCheckpoint", 10);
				Instantiate(enemyManager);
			}
		}
	}

	void Start(){
		Invoke ("startMinigame", racingTimeLimit);
	}

	void Update(){
		if(playerOrder.Count > 1){
			startMinigame();
		}
	}

}