using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Fence_placement : MonoBehaviour {

	private int fenceLength = 10;
	private Quaternion initRot = Quaternion.Euler (0f, 0f, 0f);
	float partLength = 19.5f;
	private List<GameObject> parts;
	public Vector3 startPos = new Vector3 (0f, 0f, 0f);
	private Vector3 nextPos;
	private Vector3 currentPos = new Vector3 (0f, 0f, 0f);
	private List<float> angles;
	private Random rnd = new Random ();



	// Use this for initialization
	void Start () {
		LoadParts ();
		SetAngles (true);
		Debug.Log (initRot);



		Instantiate (parts [0], getNextPos (currentPos, -90f) , Quaternion.Euler (0f, 0f, 0f));
		Instantiate (parts [0], getNextPos (currentPos, 0f) , Quaternion.Euler (0f, 0f, 0f));
		Instantiate (parts [0], getNextPos (currentPos, 0f) , Quaternion.Euler (0f, 0f, 0f));
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void PlaceFence() {
		Instantiate (parts [0], startPos, Quaternion.Euler (0f,this.angles[0],0f));
		Quaternion currentAngle = initRot;
		for (int i = 0; i < fenceLength -1; i++) {
			Instantiate (parts [0], getNextPos (currentPos, this.angles[i]) , Quaternion.Euler (0f, angles[i+1], 0f));

		}
	}

	Vector3 AngleToVector(float angle) {
		return new Vector3 ((float)Mathf.Cos ((angle/180)*Mathf.PI),0f, (float)Mathf.Sin ((angle/180)*Mathf.PI));
	}

	void LoadParts() {
		parts = new List<GameObject>();

		GameObject fence = (GameObject)Resources.Load ("Prefabs/Game/Arena/ArenaAssets/Fence2");
		GameObject fence_end = (GameObject)Resources.Load ("Prefabs/Game/Arena/ArenaAssets/Fence_end");
		parts.Add (fence);
		parts.Add (fence_end);
	}

	Vector3 getNextPos(Vector3 currentPos, float angle) {

		Vector3 nextPos = currentPos + partLength* AngleToVector (angle);
		Debug.Log (nextPos);
		this.currentPos = nextPos;
		return nextPos;
	}

	List<float> SetAngles(bool type) {

		List<float> angles = new List<float> (fenceLength);
		if (type == true) {
				int corner1 = Random.Range (0, fenceLength);
				int corner2 = corner1;
				angles [corner1] = 90f;

				do {
						corner2 = Random.Range (0, fenceLength);
				} while (corner1 == corner2);
				angles [corner2] = 90f;
			
		} else {
				int totalAngle = Random.Range (30, 120);
				float angleIncrement = totalAngle / this.fenceLength;
				for (int i = 0; i < this.fenceLength; i++) {
						angles [i] = angleIncrement; 
				}
				
		}
		return angles;
	}

}
	