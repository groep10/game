
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Game.UI {
	public class CountDown : MonoBehaviour {
		private int timeRemaining = 3;
		public Text timeleft;

		public void beginCountdown () {
			timeleft.enabled = true;
			Invoke ("Countdown", 1);

		}

		void Countdown() {
			if (timeRemaining > 1) { 
				timeRemaining -= 1;
				timeleft.text = timeRemaining.ToString();
				Invoke ("Countdown", 1);
			} else {
				timeleft.text = "GO!";
				Invoke ("eindCountdown", 1);
			}
		}

		public void eindCountdown() {
			CancelInvoke ();
			timeRemaining = 3;
			timeleft.enabled = false;
			timeleft.text = timeRemaining.ToString ();
		}
	}
}