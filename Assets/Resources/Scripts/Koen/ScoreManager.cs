using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

	
	// The player's score.
	// The player's name
	
	Text text;                      // Reference to the Text component.
	
	
	void Awake ()
	{
		text = GetComponent <Text> ();
		
		// Reset the score:
	}
	
	
	void Update ()
	{
		// Set the displayed text to be the word "Score" followed by the score value.
		text.text = "Name + Score";
	}
}