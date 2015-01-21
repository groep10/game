using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using Game.UI;
using Game.Level.Race;
using Game.Level.Zombie;
using Game.Net;

namespace Game.Level {
	public class ZombieMode : BaseMode {

		public GameObject enemyPrefab;
		public float spawnTime = 3f;

		// TODO: combine with score management code
		public int maxScore = 30;

		public float finishTimer = 120f;

		private bool finished;

		private Vector3 spawnPoint;

		public override void beginMode(System.Action finishHandler) {
			base.beginMode (finishHandler);

			Debug.Log("Starting Zombie");

			Game.Controller.getInstance().scores.initializeZombieScores();

			Game.Controller.getInstance ().disablePlayer();

			Game.Controller.getInstance ().countdown.beginCountdown ();
			Game.Controller.getInstance ().explanation.setExplanation("Shoot the zombies using Ctrl! Most kills wins!");
			Invoke ("starting", 3);
		}

		void starting() {

			Game.Controller.getInstance ().enablePlayer();

			Game.Controller.getInstance ().countdownmg.beginCountdownmg ();

			if (Network.isServer) {
				InvokeRepeating ("spawnEnemy", spawnTime, spawnTime);
				Invoke("onTimerEnd", finishTimer);
			}
		}

		public void broadcastPoint(string playername) {
			networkView.RPC("minigamePoint", RPCMode.All, playername);
		}

		[RPC]
		void minigamePoint(string playername) {
			Game.Controller.getInstance().scores.increasePlayerZombieScore(playername);
		}

		public override void onTick() {

		}

		void spawnEnemy () {
			if (GameObject.FindGameObjectsWithTag ("Enemy").Length < 30) {
				// Find a random index between zero and one less than the number of spawn points.
				// Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
				GameObject obj = Network.Instantiate (enemyPrefab, new Vector3 (Random.Range (-500, 500), 3f, Random.Range (-500, 500)), Quaternion.identity, 0) as GameObject;
				obj.GetComponent<AIEnemy> ().mode = this;
			}
		}

		public void onTimerEnd() {
			networkView.RPC("onGameEnd",  RPCMode.All);
		}

		// Called when game ends by timer
		[RPC]
		public void onGameEnd() {
			if (finished) {
				return;
			}
			onGameDone();
		}

		// TODO: Called when game ends by some1 reaching max score.
		[RPC]
		public void onGameFinish() {
			if (finished) {
				return;
			}
			onGameDone();
		}

		private void onGameDone() {
			finished = true;
			Game.Controller.getInstance ().countdownmg.eindCountdown ();

			CancelInvoke();

			GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
			foreach (GameObject enemy in enemies) {
				Network.Destroy(enemy);
				Debug.Log("Enemy removed from zombie-minigame");
			}

			// Gives the winner(s) an overall point
			Game.Controller.getInstance().scores.endMinigameScoreHandling();

			Invoke("endMode", 5);
		}

		public override void endMode() {
			Debug.Log("Finish Zombie");

			base.endMode();
		}

		public override void reset() {
			finished = false;

			base.reset();
		}

		public override Hashtable[] getScores () {
			GameObject[] playash = Game.Controller.getInstance ().getPlayers ();
			Hashtable[] scores = new Hashtable[playash.Length];
			for (int i = 0; i < playash.Length; i += 1) {
				scores[i] = new Hashtable();
				PlayerInfo pi = playash[i].GetComponent<PlayerInfo> ();
				scores[i]["id"] = pi.getUserId();
				scores[i]["score"] = Game.Controller.getInstance().scores.getMinigameScore(pi.getUsername());
			}
			return scores;
		}

		public override string getName() {
			return "Zombie";
		}
	}
}