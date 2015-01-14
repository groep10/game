using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Fence_placement : MonoBehaviour {

	private int fenceLength ;
	private Quaternion initRot;
	float partLength = 19.5f;
	private List<GameObject> parts;
	private Vector3 startPos;
	private Vector3 nextPos;
	private Vector3 currentPos;
	private float[] angles;




	// Use this for initialization
	void Start () {
		startPos = transform.position;
		currentPos = startPos;
		initRot = transform.rotation;
		Debug.Log ("initRot"+ initRot);
		fenceLength = Random.Range(10,20);
		LoadParts ();
		SetAngles (false);

		PlaceFence ();

	
	}

	void PlaceFence() {
		//List<GameObject> fenceparts = new List<GameObject> (fenceLength + 1);
		GameObject temp = (GameObject)Instantiate (parts [0], startPos, Quaternion.Euler (0f, angles[0],0f));
		temp.transform.parent = transform;

		for (int i = 0; i < fenceLength -1; i++) {
			temp = (GameObject)Instantiate (parts [0], getNextPos (currentPos, -angles[i]) , Quaternion.Euler (0f, angles[i+1], 0f));
			temp.transform.parent = transform;
		}
		temp = (GameObject)Instantiate(parts[1],getNextPos (currentPos, -angles[fenceLength-1]), Quaternion.Euler (-90f,0f,0f));
		temp.transform.parent = transform;
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

	float[] SetAngles(bool type) {

		angles = new float[fenceLength];
		for (int i = 0; i < fenceLength; i++) {
			angles [i] = -initRot[1]*180/Mathf.PI; 
		}

		if (type == true) {
				int corner1 = (int) Random.Range (0, fenceLength);
				int corner2 = corner1;
				angles [corner1] += 90f;

				do {
						corner2 = (int) Random.Range (0, fenceLength);
				} while (corner1 == corner2);
				angles [corner2] += 90f;
			return angles;
			
		} else {
				int totalAngle = (int) Random.Range (-170, 170);
			Debug.Log (totalAngle);
				float angleIncrement = totalAngle / fenceLength;
			Debug.Log (angleIncrement);
				for (int i = 0; i < fenceLength; i++) {
						angles [i] += i * angleIncrement; 
				}
			Debug.Log (angles[1]);
			return angles;
				
		}

	}

}
	