using UnityEngine;
using System.Collections;

using Game.UI;

namespace Game {
	public class GameController : MonoBehaviour {

		public ScoreController minigameScores;
		public ScoreController globalScores;

		public static GameController getInstance() {
			return GameObject.FindObjectOfType<GameController>();
		}
		
	}
}