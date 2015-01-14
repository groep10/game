using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PauseMenu : MonoBehaviour {

	public GameObject Target;
	private bool active;

	void Update () {

		showPauseMenu ();

	}

	void showPauseMenu () {
	
		active = Target.activeSelf;
		

		if (active) {
			if (Input.GetKeyUp ("escape")) {
				Target.SetActive (false);
				GameObject.Find ("Shooter").SetActive (true);
			}
		}
		else {
			if (Input.GetKeyUp ("escape")) {
				Target.SetActive (true);
				GameObject.Find ("Shooter").SetActive (true);
			}
		}
	}
}
