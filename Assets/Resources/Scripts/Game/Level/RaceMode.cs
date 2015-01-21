using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using Game;
using Game.UI;
using Game.Net;
using Game.Level.Race;

namespace Game.Level {
	public class RaceMode : BaseMode {
		// Prefab for the checkpoint
		public GameObject checkpointPrefab;
		private GameObject activeCheckpoint;
		private int count = 0;

		public static GeneticPlacement algorithm = new GeneticPlacement();

		public float finishTimer = 120f;
		public int winScore = 10;

		private bool finished;


		private int rank = 0;
		private int players = 0;
		private Hashtable reached = new Hashtable();

		public override void beginMode(System.Action finishHandler) {
			base.beginMode (finishHandler);

			Debug.Log("Starting Race");

			Game.Controller.getInstance().scores.initializeRaceScores();

			players = Network.connections.Length + 1;

			Game.Controller.getInstance ().disablePlayer();

			if (count == 0) {
				Game.Controller.getInstance ().countdown.beginCountdown ();
				count = 1;

				Invoke ("starting", 3);
			} else {
				Game.Controller.getInstance ().countdown.beginCountdown ();

				Invoke ("starting", 3);
			}

		}

		void starting() {
			Game.Controller.getInstance ().explanation.setExplanation("Race to the top of the checkpoints!");
			Game.Controller.getInstance ().enablePlayer();

			Game.Controller.getInstance ().countdownmg.beginCountdownmg ();

			if (Network.isServer) {
				placeCheckpoint();
			}

			if (Network.isServer) {
				Invoke ("onTimerEnd", finishTimer);
			}
		}

		private bool moving = false;

		public void onCheckpointReached(string playername) {
			if(reached.ContainsKey(playername) || finished || moving) {
				return;
			}

			rank++;
			int points = 4 - rank;
			
			addScore(playername, points);
			networkView.RPC("addScore", RPCMode.Others, playername, points);

			reached[playername] = true;

			if(Game.Controller.getInstance ().scores.bestScore() >= 10) {
				onGameFinish();
				networkView.RPC("onGameFinish", RPCMode.Others);
				return;
			}

			if(rank >= (players - 1)) {
				rank = 0;
				reached.Clear();
				moving = true;
				Invoke("refreshCheckpoint", 1);
			}
		}

		[RPC]
		public void addScore(string playername, int points) {
			Game.Controller.getInstance ().scores.raceAddScore(playername, points);
		}

		// replaces the old checkpoint with a new one
		public void refreshCheckpoint() {
			Debug.Log("Refreshing checkpoint");
			moving = false;
			destroyCheckpoint();
			placeCheckpoint();
		}

		// sets the checkpoint in the arena
		public void placeCheckpoint() {
			Vector3 location;
			Game.Controller.getInstance ().terrainManager.cacheAssets();
			do {
				Vector2 locXZ = algorithm.runGeneticAlgorithm ();
				float locX = locXZ.x;
				float locZ = locXZ.y;
				location = new Vector3(locX, 0f, locZ);
			} while (Game.Controller.getInstance ().terrainManager.checkLocation(location) == false);

			activeCheckpoint = (GameObject) Network.Instantiate (checkpointPrefab, location, Quaternion.identity, 0);
			activeCheckpoint.GetComponent<CheckpointBehaviour>().mode = this;
		}

		// Destroys the checkpoint
		public void destroyCheckpoint() {
			if (!Network.isServer) {
				return;
			}

			Network.Destroy (activeCheckpoint);
			Network.RemoveRPCs (activeCheckpoint.networkView.viewID);
		}

		public override void onTick() {

		}

		public void onTimerEnd() {
			networkView.RPC("onGameEnd",  RPCMode.All);
		}

		[RPC]
		public void onGameEnd() {
			if (finished) {
				return;
			}

			finished = true;
			Invoke("endMode", 5);
		}

		[RPC]
		public void onGameFinish() {
			if (finished) {
				return;
			}

			Game.Controller.getInstance().scores.endMinigameScoreHandling();

			finished = true;
			Invoke("endMode", 5);
		}

		public override void endMode() {
			destroyCheckpoint();

			Game.Controller.getInstance ().countdownmg.eindCountdown ();

			Debug.Log("Ending Race");

			base.endMode();
		}

		public override void reset() {
			finished = false;
			moving = false;
			activeCheckpoint = null;

			rank = 0;

			reached.Clear();

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
			return "Race";
		}
	}
}

