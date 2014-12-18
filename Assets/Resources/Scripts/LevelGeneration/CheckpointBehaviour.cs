using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class CheckpointBehaviour : MonoBehaviour {

	private ArrayList playerOrder = new ArrayList();
	public Terrain arena;
	public GameObject enemyManager;

	void OnTriggerEnter(Collider other){
		Debug.Log ("Object entered the Checkpoint trigger");
		Debug.Log ("Player viewID is: " + other.gameObject.networkView.viewID);

		if(!playerOrder.Contains(other.gameObject.networkView.viewID)){
			playerOrder.Add(other.gameObject.networkView.viewID);		
		}

		for(int i = 0; i<playerOrder.Count; i++){
			int rank = i + 1;
			Debug.Log ("Player" + rank + ": " + playerOrder[i]);
		}
	}
	
	void startMinigame(){
		Debug.Log ("Starting minigame.....");
		// generate the arena for the minigame
		arena.GetComponent<Level> ().editTerrain ();

		// instantiate the enemies from the server
		if(Network.isServer){
			Instantiate(enemyManager);
		}
	}

	void Start(){
		Invoke ("startMinigame", 10);
	}
}
