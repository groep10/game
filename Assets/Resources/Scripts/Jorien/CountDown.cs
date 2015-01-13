using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CountDown : MonoBehaviour
{
	    private int timeRemaining = 3;
		public Text timeleft;
		public GameObject timetext;
	   		
		void Start ()
		{
			timetext.SetActive(true);
			Invoke ("Countdown", 1);
		}

		void Countdown(){
				if (timeRemaining > 1) {
						timeRemaining -= 1;
						timeleft.text = timeRemaining.ToString();
						Invoke ("Countdown", 1);
		} else {
						timeleft.text = "GO!";
						Invoke ("eind", 1);
						
				}
		}
		
		void Update(){
			
		}
	
		
		void eind(){
			timeRemaining = 3;
			timetext.SetActive (false);
			timeleft.text = timeRemaining.ToString ();
			return;
		}
}
