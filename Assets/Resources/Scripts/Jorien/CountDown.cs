using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CountDown : MonoBehaviour
{
	    public int timeRemaining = 3;
		public float seccount = 0f;
		public Text timeleft;
	   		
		void Start ()
		{
			Countdown ();
		}

		void Countdown(){
				if (timeRemaining > 1) {
						timeRemaining -= 1;
						timeleft.text = timeRemaining.ToString();
		} else {
						timeleft.text = "Go"; 
				}
		}
		
		void Update(){
			Invoke ("Countdown", 1);
		}
}
