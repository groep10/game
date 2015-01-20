using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using Game.UI;
using Game.Net;
using Game.Level.TopRace;

namespace Game.Level {
	public class TopRaceMode : BaseMode {

		public GameObject planePrefab;
		public GameObject topCheckpoint;

		int numberOfPlanes = 6;

		int planeSpacing = 25;

		public float finishTimer = 120f;

		private bool finished = false;

		private List<GameObject> createdPlanes = new List<GameObject>();
		private GameObject currentCheckpoint;

		public override void beginMode(System.Action finishHandler) {
			base.beginMode (finishHandler);

			Debug.Log("Starting TopRace");

			Game.Controller.getInstance().scores.initializeTopRaceScores();

			if (Network.isServer) {
				generatePlanes();

				Vector3 checkpointLocation = new Vector3(0, numberOfPlanes * planeSpacing, 0);
				currentCheckpoint = Network.Instantiate(topCheckpoint, checkpointLocation, Quaternion.identity, 0) as GameObject;
				currentCheckpoint.GetComponent<topCheckpoint>().mode = this;
			}

			Game.Controller.getInstance ().disablePlayer();

			Game.Controller.getInstance ().leveltour.beginTour (() => {
				Transform camera = Game.Controller.getInstance ().getActivePlayer ().transform.FindChild ("Camera1");
				camera.gameObject.SetActive (true);

				Game.Controller.getInstance ().countdown.beginCountdown ();
				Game.Controller.getInstance ().explanation.setExplanation("Race to the top of the building! Reach the checkpoint to win!");
				Invoke ("starting", 3);
			});
		}

		void starting() {
			Game.Controller.getInstance ().enablePlayer();

			if (Network.isServer) {
				Invoke("onGameEnd", finishTimer);
			}
		}

		public override void onTick() {
			if(finished) {
				return;
			}

			GameObject[] players = Game.Controller.getInstance().getPlayers();

			foreach (GameObject player in players) {
				PlayerInfo info = player.GetComponent<PlayerInfo>();
				string name = info.getUsername();

				float height = player.transform.position.y;
				int floor = -1;
				for (float tracker = height + 1; tracker > 0; tracker -= 25) {
					floor++;
				}

				Game.Controller.getInstance().scores.setPlayerRaceToTheTopScore(name, floor);
			}
		}

		public void onReachCheckpoint(string playername) {
			networkView.RPC("onGameFinish",  RPCMode.All, playername);
		}

		// generates the planes including their connection ramps
		void generatePlanes() {
			Vector3 location = new Vector3(-250, 0, -250);
			for (int i = 0; i < numberOfPlanes; i++) {
				location.y += planeSpacing;
				createdPlanes.Add(Network.Instantiate(planePrefab, location, Quaternion.identity, 0) as GameObject);
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

		[RPC]
		public void onGameFinish(string playername) {
			if (finished) {
				return;
			}

			Game.Controller.getInstance().scores.setPlayerRaceToTheTopScore(playername, 99);

			onGameDone();
		}

		private void onGameDone() {
			finished = true;

			foreach (GameObject plane in createdPlanes) {
				plane.GetComponent<PlaneRenderer>().cleanupChildren();
				Network.Destroy(plane);
				Network.RemoveRPCs(plane.networkView.viewID);
				//Debug.Log("plane removed from toprace-minigame");
			}

			if (Network.isServer) {
				Network.Destroy(currentCheckpoint);
				Network.RemoveRPCs(currentCheckpoint.networkView.viewID);
			}
			currentCheckpoint = null;

			// Gives the winner(s) an overall point
			Game.Controller.getInstance().scores.endMinigameScoreHandling();

			// Respawns the player on the ground
			Game.Controller.getInstance().getActivePlayer().GetComponent<respawn>().resetPlayer();

			Invoke("endMode", 5);
		}

		public override void endMode() {
			Debug.Log("Finish TopRace");

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
			return "TopRace";
		}
	}
}