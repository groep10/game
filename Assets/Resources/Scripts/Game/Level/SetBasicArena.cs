using UnityEngine;
using System.Collections;


namespace Game.Level {
	public class SetBasicArena : MonoBehaviour {

		public GameObject Level;

		// loads the basic level into the game
		void Awake(){
			Instantiate (Level);
		}
	}

}