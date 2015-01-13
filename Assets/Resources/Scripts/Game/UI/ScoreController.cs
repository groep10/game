using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;
using Game.Web;
using Game.Menu;
using Game.Net;

namespace Game.UI {
	public class ScoreController : MonoBehaviour {

		/* ============================================== Variables ==================================== */

		public GameObject prefab;
		private int winningOverallScore = 3;

		private Hashtable minigame = new Hashtable();
		private Hashtable overall = new Hashtable();

		/* ============================================== FUNCTIONS ===================================== */

		/* -------------------------------------- GENERAL FUNCTIONS ------------------------------ */
		
		// Adds a score entry to the menulist of the minigamescores
		public void addMinigameScore(String text) {
			GameObject obj = Instantiate(prefab) as GameObject;
			obj.GetComponent<Text>().text = text;

			Game.Controller.getInstance().minigameScores.addItem(obj);
		}

		// resets the minigamescores
		public void resetMinigameScores() {
			Game.Controller.getInstance().minigameScores.setItems(new GameObject[0]);
		}

		// Adds a score entry to the menulist of the overallscores
		public void addOverallScore(String text) {
			GameObject obj = Instantiate(prefab) as GameObject;
			obj.GetComponent<Text>().text = text;

			Game.Controller.getInstance().overallScores.addItem(obj);
		}

		// resets the overallcores
		public void resetOverallScores() {
			Game.Controller.getInstance().overallScores.setItems(new GameObject[0]);
		}
		

		// checks if a player has reached the winning overall score
		public void endGameByOverallScore() {
			foreach (DictionaryEntry de in overall) {
				if ((int)de.Value == winningOverallScore) {

				}
			}
		}

		/* ---------------------------------- GENERAL MINIGAME FUNCTIONS ------------------------- */
		// Ends the minigame and resets the minigamescore hashtable
		public void endMinigame() {
			DictionaryEntry highest = new DictionaryEntry("hoi", -1);
			foreach (DictionaryEntry de in minigame) {
				if ((int)highest.Value == -1 || (int)highest.Value < (int)de.Value) {
					highest = de;
				}
			}

			// increase the overall score of the best playerin the minigame
			increaseOverallScore((string)highest.Key);

			// empty the minigame hastable
			minigame.Clear();
		}

		/* ------------------------------------ OVERALL SCORES  ----------------------------------- */

		// Sets the overallScores of all players in the game to 0
		public void initializeOverallScores(){
			Debug.Log("Initializing overallScores");
			GameObject[] players = Game.Controller.getInstance().getPlayers();
			Debug.Log("players = " + players);
			foreach(GameObject player in players) {
					Debug.Log("going through initializeOverallScores loop");
					PlayerInfo inf = player.GetComponent<PlayerInfo>();
					overall[inf.getUsername()] = 0;
			}
		}

		// Increases the overall score of a player by 1
		public void increaseOverallScore(string playername) {
			if (!overall.ContainsKey(playername)) {
				overall[playername] = 0;
			}
			overall[playername] = (int)overall[playername] + 1;
			updateOverallScores();
		}

		// Updates the Overall Scores of all players
		public void updateOverallScores() {
			Debug.Log("updating overallScores");

			resetOverallScores();
			addOverallScore("Overall");

			foreach (DictionaryEntry de in overall) {
				Debug.Log("going through overall update loop");
				addOverallScore(de.Key + ": " + de.Value);
			}
		}

		/* ------------------------------------ RACING MINIGAME ----------------------------------- */
        // Add player to the score line
        public void raceAddFinishedPlayer(string playername) {
            // TODO: implement
            updateRaceScores();
        }

        // Updates the Scores of all players
        public void updateRaceScores() {
            resetMinigameScores();
            addMinigameScore("Mode: race");

            foreach (DictionaryEntry de in minigame) {
                addMinigameScore(de.Key + ": " + de.Value);
            }
        }

		/* ------------------------------------ ZOMBIE MINIGAME ----------------------------------- */

		// Increases the zombie score of a player by 1
		public void increasePlayerZombieScore(string playername) {
			if (!minigame.ContainsKey(playername)) {
				minigame[playername] = 0;
			}
			minigame[playername] = (int)minigame[playername] + 1;
			updateZombieScores();
		}

		// Updates the Scores of all players
		public void updateZombieScores() {
			resetMinigameScores();
			addMinigameScore("Mode: zombie");

			foreach (DictionaryEntry de in minigame) {
				addMinigameScore(de.Key + ": " + de.Value);
			}
		}

		/* ------------------------------------ RACE-TO-THE-TOP MINIGAME ----------------------------------- */

		// Sets the race to the top score equal to the current floor the player is at
		public void setPlayerRaceToTheTopScore(string playername, int floor) {
			if (!minigame.ContainsKey(playername)) {
				minigame[playername] = 0;
			}
			minigame[playername] = floor;
			updateRaceToTheTopScores();
		}


		// Updates the Scores of all players
		public void updateRaceToTheTopScores() {
			resetMinigameScores();
			addMinigameScore("Mode: Race to the top");
			foreach (DictionaryEntry de in minigame) {
				addMinigameScore(de.Key + ": floor " + de.Value);
			}
		}

		/* ------------------------------------ TRON MINIGAME ----------------------------------- */




		/* ------------------------------------ AWAKE, START & UPDATE ----------------------------------- */

		void Start(){
			Debug.Log("Starting ScoreController");

		}

	}
}
