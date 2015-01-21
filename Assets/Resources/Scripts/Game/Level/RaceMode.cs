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

		private List<GameObject> assets;
		private GameObject[] placedAssets;
		private int assetsInArena = 10;

		public float checkpointMoveTimer = 30f;
		public float finishTimer = 120f;

		private bool finished;

		public override void beginMode(System.Action finishHandler) {
			base.beginMode (finishHandler);

			Debug.Log("Starting Race");

			Game.Controller.getInstance().scores.initializeRaceScores();

			if (Network.isServer) {
				loadAssets();
				for (int i = 0; i < assetsInArena; i++) {
					placeAsset();
				}
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
			Game.Controller.getInstance ().explanation.setExplanation("Race to the top of the checkpoint! Be 1st to gain an advantage!");
			Game.Controller.getInstance ().enablePlayer();

			if (Network.isServer) {
				Invoke ("onTimerEnd", finishTimer);
			}
		}

		// returns an ArrayList of all GameObjects with tag "ArenaAsset"
		void loadAssets() {
			assets = new List<GameObject>();
			foreach (Object o in Resources.LoadAll("Prefabs/Game/Arena/ArenaAssets")) {
				if (!(o is GameObject)) {
					continue;
				}
				GameObject go = (GameObject) o;
				if (go.tag == "ArenaAsset") {
					assets.Add(go);
				}
			}
		}

		public void findPlacedAssets() {
			placedAssets = GameObject.FindGameObjectsWithTag("ArenaAsset");
		}

		public void placeAsset() {
			// randomise the location within x and z boundaries
			findPlacedAssets ();
			Vector3 location = findLocation();

			float rotationY = Random.Range (0, 360);

			// randomise the asset to be placed
			int assetIndex = Random.Range(0, assets.Count);
			GameObject asset = assets[assetIndex];

			// instantiate the asset
			GameObject currentAsset = (GameObject) Network.Instantiate (asset, location, Quaternion.Euler(0f, rotationY, 0f), 0);
			currentAsset.GetComponentInChildren<FadeBehaviour> ().setOnDone(() => {
				placeAsset();
			});
		}

		public Vector3 findLocation() {
			float x = 0;
			float z = 0;
			Vector3 location = new Vector3(x, 0f, z);
			bool fits = true;
			do {
				fits = true;
				x = Random.Range (-500, 500);
				z = Random.Range (-500, 500);
				location = new Vector3(x,0f,z);
				fits = checkLocation(location);
			
			} while (fits == false);
			return location;

		}

		//checks if a new location is too close to any existing arena assets.
		public bool checkLocation(Vector3 location){
			foreach (GameObject go in placedAssets) {
						Vector3 pos = go.transform.position;

						if (Vector3.Distance(location,pos) < 25) {
								return false;
						}
				}
				return true;
		}

		// sets the checkpoint in the arena
		public void placeCheckpoint() {
			Vector3 location;
			do {
				Vector2 locXZ = algorithm.runGeneticAlgorithm ();
				float locX = locXZ.x;
				float locZ = locXZ.y;
				location = new Vector3(locX, 0f, locZ);
			}while (checkLocation(location) == false);
			

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
			if (Game.Controller.getInstance().scores.bestScore() >= 10){
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

