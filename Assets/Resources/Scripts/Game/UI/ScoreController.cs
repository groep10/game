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

		// returns the best three overall players by their names
		public string[] findTopThree(){
			string[] topThree = new string[3];
			int index = 0;
			int winningScore = bestOverallScore();

			foreach(DictionaryEntry de in overall){
				if((int) de.Value == winningScore){
					topThree[index] = (string) de.Key;
					index++;
				}
			}

			while(index < 2 && index < Game.Controller.getInstance().getPlayers().Length){
				winningScore--;
				foreach(DictionaryEntry de in overall){
					if((int) de.Value == winningScore){
						topThree[index] = (string) de.Key;
						index++;
					}
				}
			}
			return topThree;		
		}

		// returns the best overall score
		private int bestOverallScore(){
			int best = -1;

			foreach (DictionaryEntry de in overall){
				if((int) de.Value > best){
					best = (int) de.Value;
				}
			}
			Debug.Log("best overall score is: " + best);
			return best;
		}

		// displays the best overall top three
		public void displayOverallRanking(){
			Debug.Log("display overall ranking");
			string[] topThree = findTopThree();

			int size = Game.Controller.getInstance().getPlayers().Length;

			switch(size)
			{
			case 1:
				displayTopOne(topThree);
				break;
			case 2:
				displayTopTwo(topThree);
				break;
			default:
				displayTopThree(topThree);
				break;
			}
		}

		// displays the best player
		private void displayTopOne(string[] topThree){
			Game.Controller.getInstance().number1.setResultText("#1 " + topThree[0]);
		}

		// displays the best two players
		private void displayTopTwo(string[] topThree){
			Game.Controller.getInstance().number1.setResultText("#1 " + topThree[0]);
			Game.Controller.getInstance().number2.setResultText("#2 " + topThree[1]);
		}

		// displays the best three players
		private void displayTopThree(string[] topThree){
			Game.Controller.getInstance().number1.setResultText("#1 " + topThree[0]);
			Game.Controller.getInstance().number2.setResultText("#2 " + topThree[1]);
			Game.Controller.getInstance().number3.setResultText("#3 " + topThree[2]);
		}


		/* ==================================== MINIGAMES ========================================= */

		/* ------------------------------------ GENERAL FUNCTIONS --------------------------------- */
		// returns the best minigame score currently in the game
		public int bestScore(){
			int numberOfPlayers = Game.Controller.getInstance().getPlayers().Length;
			int[] playerscores = new int[numberOfPlayers];

			int i = 0;
			// fill the playerscores list and find the maximum value
			foreach (DictionaryEntry de in minigame){
				playerscores[i] = (int) de.Value;
				i++;
			}
			return Mathf.Max(playerscores);
		}

		// returns the playernames of the best players in a minigame
		private string[] bestPlayers(){
			int numberOfPlayers = Game.Controller.getInstance().getPlayers().Length;
			string[] playernames = new string[numberOfPlayers];

			int max = bestScore();

			int j = 0;			

			// find out which player(s) has/have the maximum score and won the zombiegame
			foreach (DictionaryEntry de in minigame) {
				playernames[j] = null;
				if((int) de.Value == max) {
					playernames[j] = (string)de.Key;
					j++;
				}
			}
			return playernames;
		}

		// increases the overall score of the best players by 1
		public void endMinigameScoreHandling() {
			string[] winners = bestPlayers();

			// increase the overall score of the winner(s) by 1
			foreach(string name in winners){
				if (name == null){
					continue;
				}
				increaseOverallScore(name);
			}
			displayMinigameWinners(winners);
		}

		// displays the winners of a minigame at the end of the minigame
		private void displayMinigameWinners(string[] winners){
			string text = "";

			for(int i=0; i < winners.Length; i++){
				text += winners[i];


				if(i < winners.Length - 1 && winners[i+1] != null){
					//Debug.Log("evaluate empty string: " + winners[i+1] != string.Empty);
					//Debug.Log("evaluate null: " + winners[i+1] != null);
					Debug.Log("debug test");
					text += " & ";					
				}
			}
			text += " won the minigame!";
			Game.Controller.getInstance().explanation.setExplanation(text);
		}

		// returns the minigameScore of the given player
		public object getMinigameScore(string key) {
			return minigame [key];
		}

		// returns the overallScore of the given player
		public object getOveralScore(string key) {
			return overall [key];
		}

		/* ------------------------------------ RACING MINIGAME ----------------------------------- */

		// initializes the race minigame hashtable
		public void initializeRaceScores() {
			minigame.Clear();

			GameObject[] players = Game.Controller.getInstance().getPlayers();
			foreach (GameObject player in players) {
				PlayerInfo inf = player.GetComponent<PlayerInfo>();
				minigame[inf.getUsername()] = 0;
			}
			updateRaceScores();
		}

		// Add minigame score for the given playername
		public void raceAddScore(string playername, int points) {
			minigame[playername] = (int) getMinigameScore(playername) + points;
			updateRaceScores();
		}

		// Updates the scores of all players
		public void updateRaceScores() {
			resetMinigameScores();
			addMinigameScore("Mode: race");

			foreach (DictionaryEntry de in minigame) {
				addMinigameScore(de.Key + ": " + de.Value);
			}
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

		void Update(){
			if(bestOverallScore() >= winningOverallScore){
				winningOverallScore = 1000; // disable

				displayOverallRanking();
				Game.Controller.getInstance().finishGame();
				if(Network.isServer) {
					GameObject[] playash = Game.Controller.getInstance ().getPlayers ();
					Hashtable[] scores = new Hashtable[playash.Length];
					for (int i = 0; i < playash.Length; i += 1) {
						scores[i] = new Hashtable();
						PlayerInfo pi = playash[i].GetComponent<PlayerInfo> ();
						scores[i]["id"] = pi.getUserId();
						scores[i]["score"] = overall[pi.getUsername()];
					}
					AccountController.getInstance().createGameScores(scores, (res) => {
						Debug.Log("Scores submitted");
					});
				}
			}
		}

	}
}
