using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class CheckpointBehaviour : MonoBehaviour {

	private ArrayList playerOrder = new ArrayList();

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
}
