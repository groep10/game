using UnityEngine;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Game.UI {
	public class CountDownMiniGame : MonoBehaviour {
		private float timeRemaining = 120f;
		public Text timeleft;
		
		public void beginCountdownmg () {
			timeleft.enabled = true;
			Invoke ("Countdownmg", 1);
			
		}
		
		void Countdownmg() {
			if (timeRemaining > 1) { 
				timeRemaining -= 1;
				timeleft.text = "Time left: " + timeRemaining.ToString();
				Invoke ("Countdownmg", 1);
			} else {
				timeleft.text = "Time left: 0";
				Invoke ("eindCountdownmg", 1);
			}
		}
		
		public void eindCountdown() {
			CancelInvoke ();
			timeRemaining = 120f;
			timeleft.enabled = false;
			timeleft.text = "Time left: " + timeRemaining.ToString();
		}
	}
}