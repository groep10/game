using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Game.UI {
	public class Explanation : MonoBehaviour {

		public Text explanationText;

		// Sets the explanation Text to the string that explains the next minigame
		public void setExplanation(string explanation){
			explanationText.enabled = true;
			explanationText.text = explanation;

			Invoke("endExplanation", 4);
		}

		// Resets the explanationText and sets it enabled to false
		void endExplanation(){
			explanationText.enabled = false;
			explanationText.text = "";
		}
	}
}