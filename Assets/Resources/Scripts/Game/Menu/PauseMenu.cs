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
				GameObject.FindGameObjectWithTag ("Shooter").SetActive (false);
			}
		}
		else {
			if (Input.GetKeyUp ("escape")) {
				Target.SetActive (true);
				GameObject.FindGameObjectWithTag ("Shooter").SetActive (true);
			}
		}
	}
}
