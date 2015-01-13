using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Fence_placement : MonoBehaviour {

	private int fenceLength = 10;
	private Quaternion initRot = Quaternion.Euler (0f, 0f, 0f);

	private int totalAngle = 90;
	float partLength = 19.5f;
	private List<GameObject> parts;
	private Vector3 startPos = new Vector3 (0f, 0f, 0f);
	private Vector3 nextPos = new Vector3(0f,0f,19.5f);


	// Use this for initialization
	void Start () {
		LoadParts ();
		Instantiate (parts [0], startPos, initRot);
		Instantiate (parts [0], nextPos , initRot);
		float length = parts[0].renderer.bounds.extents.magnitude;
		Vector3 center = parts[0].renderer.bounds.center;
		Debug.Log (length);
		Debug.Log (center);
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void LoadParts() {
		parts = new List<GameObject>();

		GameObject fence = (GameObject)Resources.Load ("Prefabs/Game/Arena/ArenaAssets/Fence");
		GameObject fence_end = (GameObject)Resources.Load ("Prefabs/Game/Arena/ArenaAssets/Fence_end");

		parts.Add (fence);
		parts.Add (fence_end);
	}
}


