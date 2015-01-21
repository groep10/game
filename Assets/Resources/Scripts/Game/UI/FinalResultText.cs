using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Game.UI {
	public class FinalResultText : MonoBehaviour {

		public Text resultText;

		// Sets the resultText to visible and displaying the correct result
		public void setResultText(string result){
			resultText.enabled = true;
			resultText.text = result;
		}

	}
}