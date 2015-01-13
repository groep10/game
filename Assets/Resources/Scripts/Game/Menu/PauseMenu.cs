using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour {

	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
						Debug.Log ("esc is pressed");
						GameObject.Find ("PauseMenuObjects").SetActive (true);
				}
		else
			GameObject.Find ("PauseMenuObjects").SetActive (false);

	}
}
