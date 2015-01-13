using UnityEngine;
using System.Collections;

namespace Game {
	public class CanvasController : MonoBehaviour {
		public Canvas canvas;
		private GameObject player;

		public void setCanvasCamera(){
			Controller gamecontroller = GameObject.Find ("GameController").GetComponent<Controller>();
			player = gamecontroller.getActivePlayer ();
			Transform cam = player.transform.Find("Camera1");
			canvas.worldCamera = cam.GetComponent<Camera>();
		}
	}
}

