using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using Game;
using Game.UI;
using Game.Level.Race;

using Game.Net;

namespace Game.Level {
	public class RaceMode : BaseMode {
		// Prefab for the checkpoint
		public GameObject checkpointPrefab;
		private GameObject activeCheckpoint;
		private int count = 0;

		public static GeneticPlacement algorithm = new GeneticPlacement();

		private List<GameObject> assets;
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
			 
			//Game.Controller.getInstance ().getActivePlayer ().GetComponent<CarController> ().enabled = false;
			if(count==0){
			Game.Controller.getInstance ().leveltour.beginTour (() => {
				Transform camera = Game.Controller.getInstance ().getActivePlayer ().transform.FindChild ("Camera1");
				camera.gameObject.SetActive (true);
					Game.Controller.getInstance ().getActivePlayer ().rigidbody.velocity = new Vector3(0,0,0);
				Game.Controller.getInstance ().countdown.beginCountdown ();
				count=1;
				Invoke ("starting", 3);
				});
			}else{
				Game.Controller.getInstance ().getActivePlayer ().rigidbody.useGravity = false;
				Game.Controller.getInstance ().getActivePlayer ().transform.position = new Vector3 (0,1,0) + Game.Controller.getInstance ().getActivePlayer ().transform.position;
				Transform camera = Game.Controller.getInstance ().getActivePlayer ().transform.FindChild ("Camera1");
				camera.gameObject.SetActive (true);
				Game.Controller.getInstance ().getActivePlayer ().rigidbody.velocity = new Vector3(0,0,0);
				Game.Controller.getInstance ().countdown.beginCountdown ();
				//Game.Controller.getInstance ().explanation.setExplanation("Race to the top of the checkpoint! Be 1st to gain an advantage!");
				Invoke ("starting", 3);
			}
		  
		}

		void starting() {
			Game.Controller.getInstance ().explanation.setExplanation("Race to the top of the checkpoint! Be 1st to gain an advantage!");
			Game.Controller.getInstance ().getActivePlayer ().rigidbody.useGravity = true;
			//Game.Controller.getInstance ().getActivePlayer ().GetComponent<CarController> ().enabled = true;
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

		public void placeAsset() {
			// randomise the location within x and z boundaries
			float x = Random.Range (-500, 500);
			float z = Random.Range (-500, 500);
			Vector3 location = new Vector3(x, 0f, z);

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

		// sets the checkpoint in the arena
		public void placeCheckpoint() {
			Vector2 locXZ = algorithm.runGeneticAlgorithm ();
			float locX = locXZ.x;
			float locZ = locXZ.y;
			Vector3 location = new Vector3(locX, 0f, locZ);

			activeCheckpoint = (GameObject) Network.Instantiate (checkpointPrefab, location, Quaternion.identity, 0);
			Invoke("refreshCheckpoint", checkpointMoveTimer);
		}

		public void refreshCheckpoint() {
			destroyCheckpoint();
			placeCheckpoint();
		}

		// Destroys the checkpoint
		public void destroyCheckpoint() {
			if(!Network.isServer) {
				return;
			}

			Network.Destroy (activeCheckpoint);
			Network.RemoveRPCs (activeCheckpoint.networkView.viewID);
		}

		public override void onTick() {
			if(finished) {
				return;
			}

			if(!Network.isServer) {
				return;
			}

			if(activeCheckpoint != null) {
				int cnt = activeCheckpoint.GetComponent<CheckpointBehaviour>().getReachedCount();
				if(cnt > 0 && cnt >= (Network.connections.Length - 1)) {
					networkView.RPC("onGameFinish",  RPCMode.All);
				}
			}
		}

		public void onTimerEnd() {
			networkView.RPC("onGameEnd",  RPCMode.All);
		}

		[RPC]
		public void onGameEnd() {
			if(finished) {
				return;
			}

			finished = true;
			Invoke("endMode", 5);
		}

		[RPC]
		public void onGameFinish() {
			if(finished) {
				return;
			}

			Game.Controller.getInstance().scores.endRaceMode();

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
				scores[i]["score"] = fromPosition((string) Game.Controller.getInstance().scores.getMinigameScore(pi.getUsername()));
			}
			return scores;
		}

		public int fromPosition(string pos) {
			switch (pos) {
				case "1st": return 1;
				case "2nd":	return 2;
				case "3rd": return 3;
				case "4th": return 4;
				default: return 0;
			}
		}

		public override string getName() {
			return "Race";
		}
	}
}

