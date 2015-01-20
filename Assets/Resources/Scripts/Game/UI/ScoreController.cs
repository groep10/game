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
		public void addMinigameScore(string text) {
			GameObject obj = Instantiate(prefab) as GameObject;
			obj.GetComponent<Text>().text = text;

			Game.Controller.getInstance().minigameScores.addItem(obj);
		}

		// resets the minigamescores
		public void resetMinigameScores() {
			Game.Controller.getInstance().minigameScores.setItems(new GameObject[0]);
		}

		// Adds a score entry to the menulist of the overallscores
		public void addOverallScore(string text) {
			GameObject obj = Instantiate(prefab) as GameObject;
			obj.GetComponent<Text>().text = text;

			Game.Controller.getInstance().overallScores.addItem(obj);
		}

		// checks if a player has reached the winning overall score
		public void endGameByOverallScore() {
			foreach (DictionaryEntry de in overall) {
				if ((int)de.Value == winningOverallScore) {

				}
			}
		}

		/* ------------------------------------ OVERALL SCORES  ----------------------------------- */

		// Sets the overallScores of all players in the game to 0
		public void initializeOverallScores() {
			GameObject[] players = Game.Controller.getInstance().getPlayers();
			foreach (GameObject player in players) {
				PlayerInfo inf = player.GetComponent<PlayerInfo>();
				overall[inf.getUsername()] = 0;
			}
		}

		// resets the overallcores
		public void resetOverallScores() {
			Game.Controller.getInstance().overallScores.setItems(new GameObject[0]);
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
			resetOverallScores();
			addOverallScore("Overall");

			foreach (DictionaryEntry de in overall) {
				addOverallScore(de.Key + ": " + de.Value);
			}
		}

		/* ==================================== MINIGAMES ========================================= */

		/* ------------------------------------ GENERAL FUNCTIONS --------------------------------- */
		public void endMinigameScoreHandling() {
			int numberOfPlayers = Game.Controller.getInstance().getPlayers().Length;

			int[] playerscores = new int[numberOfPlayers];
			string[] playernames = new string[numberOfPlayers];

			string playername = null;

			int i = 0;
			// fill the playerscores list and find the maximum value
			foreach (DictionaryEntry de in minigame){
				playerscores[i] = (int) de.Value;
				i++;
			}
			int max = Mathf.Max(playerscores);

			int j = 0;
			// find out which player(s) has/have the maximum score and won the zombiegame
			foreach (DictionaryEntry de in minigame) {
				if((int) de.Value == max) {
					playernames[j] = (string)de.Key;
					j++;
				}
			}

			// increase the overall score of the winner(s) by 1
			foreach(string name in playernames){
				if (name == null){
					continue;
				}
				increaseOverallScore(name);
			}
		}

		/* ------------------------------------ RACING MINIGAME ----------------------------------- */

		public int rank = 0;

		// initializes the race minigame hashtable
		public void initializeRaceScores() {
			minigame.Clear();
			rank = 0;
			GameObject[] players = Game.Controller.getInstance().getPlayers();
			foreach (GameObject player in players) {
				PlayerInfo inf = player.GetComponent<PlayerInfo>();
				minigame[inf.getUsername()] = "not finished yet";
			}
			updateRaceScores();
		}

		// Add player to the score line
		public void raceAddFinishedPlayer(string playername) {
			rank++;

			Debug.Log("Rank = " + rank);

			string text;
			switch (rank) {
			case 1:
				text = "1st";
				break;
			case 2:
				text = "2nd";
				break;
			case 3:
				text = "3rd";
				break;
			case 4:
				text = "4th";
				break;
			default:
				text = "not finished yet";
				break;
			}
			minigame[playername] = text;

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

		public void endRaceMode() {
			/* Overall score moet niet toenemen na race mode

			string playername = null;
			foreach (DictionaryEntry de in minigame) {
				if((string) de.Value == "1st") {
					playername = (string)de.Key;
					break;
				}
			}
			if(playername != null) {
				increaseOverallScore(playername);
			}
			*/
		}

		/* ------------------------------------ ZOMBIE MINIGAME ----------------------------------- */

		// initializes the zombie minigame hashtable
		public void initializeZombieScores() {
			minigame.Clear();
			GameObject[] players = Game.Controller.getInstance().getPlayers();
			foreach (GameObject player in players) {
				PlayerInfo inf = player.GetComponent<PlayerInfo>();
				minigame[inf.getUsername()] = 0;
			}
			updateZombieScores();
		}

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

		/* Called at the end of the zombie mode to deal with the overall score increase */
		/*
		public void endZombieMode() {
			int numberOfPlayers = Game.Controller.getInstance().getPlayers().Length;

			int[] playerscores = new int[numberOfPlayers];
			string[] playernames = new string[numberOfPlayers];

			string playername = null;

			int i = 0;
			// fill the playerscores list and find the maximum value
			foreach (DictionaryEntry de in minigame){
				playerscores[i] = (int) de.Value;
				i++;
			}
			int max = Mathf.Max(playerscores);

			int j = 0;
			// find out which player(s) has/have the maximum score and won the zombiegame
			foreach (DictionaryEntry de in minigame) {
				if((int) de.Value == max) {
					playernames[j] = (string)de.Key;
					j++;
				}
			}

			// increase the overall score of the winner(s) by 1
			foreach(string name in playernames){
				increaseOverallScore(name);
			}
		}
		*/

		/* ------------------------------------ RACE-TO-THE-TOP MINIGAME ----------------------------------- */

		// initializes the top race minigame hashtable
		public void initializeTopRaceScores() {
			minigame.Clear();
			GameObject[] players = Game.Controller.getInstance().getPlayers();
			foreach (GameObject player in players) {
				PlayerInfo inf = player.GetComponent<PlayerInfo>();
				minigame[inf.getUsername()] = 0;
			}
			updateRaceToTheTopScores();
		}

		// Sets the race to the top score equal to the current floor the player is at
		public void setPlayerRaceToTheTopScore(string playername, int floor) {
			if (!minigame.ContainsKey(playername)) {
				minigame[playername] = 0;
			}
			if (floor > (int)minigame[playername]) {
				minigame[playername] = floor;
				updateRaceToTheTopScores();
			}
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

		// initializes the tron minigame hashtable
		public void initializeTronScores() {
			minigame.Clear();
			GameObject[] players = Game.Controller.getInstance().getPlayers();
			foreach (GameObject player in players) {
				PlayerInfo inf = player.GetComponent<PlayerInfo>();
				minigame[inf.getUsername()] = 1;
			}
			updateTronScores();
		}

		// Set a player score to "dead"
		public void deadTronPlayer(string player) {
			minigame[player] = 0;
			updateTronScores ();
		}

		// Updates the tron scores of all players
		public void updateTronScores() {
			resetMinigameScores();
			addMinigameScore("Mode: Tron");
			foreach (DictionaryEntry de in minigame) {
				if((int) de.Value == 1){
					addMinigameScore(de.Key + ": alive");
				}
				else{
					addMinigameScore(de.Key + ": dead");
				}				
			}
		}

		/* ------------------------------------ AWAKE, START & UPDATE ----------------------------------- */

		void Start() {
			Debug.Log("Starting ScoreController");
		}

	}
}
