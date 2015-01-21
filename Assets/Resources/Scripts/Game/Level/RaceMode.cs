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

		public float checkpointMoveTimer = 30f;
		public float finishTimer = 120f;

		private bool finished;

		public override void beginMode(System.Action finishHandler) {
			base.beginMode (finishHandler);

			Debug.Log("Starting Race");

			Game.Controller.getInstance().scores.initializeRaceScores();

			if (Network.isServer) {
				placeCheckpoint();
			}

			Game.Controller.getInstance ().disablePlayer();

			if (count == 0) {
				Game.Controller.getInstance ().leveltour.beginTour (() => {
					Transform camera = Game.Controller.getInstance ().getActivePlayer ().transform.FindChild ("Camera1");
					camera.gameObject.SetActive (true);

					Game.Controller.getInstance ().countdown.beginCountdown ();
					count = 1;

					Invoke ("starting", 3);
				});
			} else {
				Game.Controller.getInstance ().countdown.beginCountdown ();

				Invoke ("starting", 3);
			}

		}

		void starting() {
			Game.Controller.getInstance ().explanation.setExplanation("Race to the top of the checkpoint!");
			Game.Controller.getInstance ().enablePlayer();

			Game.Controller.getInstance ().countdownmg.beginCountdownmg ();

			if (Network.isServer) {
				Invoke ("onTimerEnd", finishTimer);
			}
		}

		// sets the checkpoint in the arena
		public void placeCheckpoint() {
			Vector3 location;
			do {
				Vector2 locXZ = algorithm.runGeneticAlgorithm ();
				float locX = locXZ.x;
				float locZ = locXZ.y;
				location = new Vector3(locX, 0f, locZ);
			} while (Game.Controller.getInstance ().terrainManager.checkLocation(location) == false);


			activeCheckpoint = (GameObject) Network.Instantiate (checkpointPrefab, location, Quaternion.identity, 0);
			Invoke("refreshCheckpoint", checkpointMoveTimer);
		}

		// replaces the old checkpoint with a new one
		public void refreshCheckpoint() {
			Debug.Log("Refreshing checkpoint");
			destroyCheckpoint();
			placeCheckpoint();
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
			if (finished) {
				return;
			}

			if (!Network.isServer) {
				return;
			}

			// refresh the checkpoint if all but one players have reached it
			if (activeCheckpoint != null) {
				int cnt = activeCheckpoint.GetComponent<CheckpointBehaviour>().getReachedCount();
				if (cnt > 0 && cnt >= (Network.connections.Length - 1)) {
					networkView.RPC("onGameFinish", RPCMode.All);
				}
			}

			// end the minigame if the winningscore is reached by the best player
			if (Game.Controller.getInstance().scores.bestScore() >= 10) {
				networkView.RPC("onGameFinish", RPCMode.All);
			}
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
			activeCheckpoint = null;

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

