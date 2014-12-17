﻿using UnityEngine;
using System.Collections;

public class CheckpointBehaviour : MonoBehaviour {

	void OnTriggerEnter(Collider other){
		Debug.Log ("Object entered the Checkpoint trigger");
		Debug.Log ("Player viewID is: " + other.gameObject.networkView.viewID);
	}
}
