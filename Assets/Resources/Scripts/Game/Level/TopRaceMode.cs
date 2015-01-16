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

		private bool finished;

		private List<GameObject> createdPlanes = new List<GameObject>();
		private GameObject currentCheckpoint;

		public override void beginMode(System.Action finishHandler) {
			base.beginMode (finishHandler);

			Debug.Log("Starting TopRace");

			Game.Controller.getInstance().scores.initializeTopRaceScores();

			Game.Controller.getInstance ().countdown.beginCountdown ();

			if(Network.isServer) {
				generatePlanes();
				
				Vector3 checkpointLocation = new Vector3(0, numberOfPlanes * planeSpacing, 0);
				Network.Instantiate(topCheckpoint, checkpointLocation, Quaternion.identity, 0);
			}

			Invoke ("starting", 3);
		}

		void starting(){
			Invoke("onGameEnd", finishTimer);
		}

		public override void onTick() {
			if(currentCheckpoint == null) {
				topCheckpoint behav = GameObject.FindObjectOfType<topCheckpoint> ();
				if(behav == null) {
					return;
				}
				currentCheckpoint = behav.gameObject;				
			}
			if(!currentCheckpoint.GetComponent<topCheckpoint>().winnerReachedCheckpoint){
				GameObject[] players = Game.Controller.getInstance().getPlayers();

				foreach (GameObject player in players){
					PlayerInfo info = player.GetComponent<PlayerInfo>();
					string name = info.getUsername();

					float height = player.transform.position.y;
					int floor = -1;
					for (float tracker = height + 1; tracker > 0; tracker -= 25){
						floor++;
					}

					Game.Controller.getInstance().scores.setPlayerRaceToTheTopScore(name, floor);
				}
				return;
			}
			
			if(Network.isClient) {
				return;
			}
			networkView.RPC("onGameFinish", RPCMode.All);
		}

		// generates the planes including their connection ramps
		void generatePlanes()
		{
			Vector3 location = new Vector3(-250, 0, -250);
			for (int i = 0; i < numberOfPlanes; i++)
			{
				location.y += planeSpacing;
				createdPlanes.Add(Network.Instantiate(planePrefab, location, Quaternion.identity, 0) as GameObject);
			}
		}

		public void onTimerEnd() {
			networkView.RPC("onGameFinish",  RPCMode.All);
		}

		// Called when game ends by timer
		[RPC]
		public void onGameEnd() {
			if(finished) {
				return;
			}
			onGameDone();
		}

		// TODO: Called when game ends by some1 reaching the top?
		[RPC]
		public void onGameFinish() {
			if(finished) {
				return;
			}

			// find the players name based on the viewID
			PlayerInfo info = currentCheckpoint.GetComponent<topCheckpoint>().winner.GetComponent<PlayerInfo>();
			string name = info.getUsername();

			// make the player win the mini-game
			//Game.Controller.getInstance().scores.increaseOverallScore(name);

			onGameDone();
		}

		private void onGameDone() {
			finished = true;

            foreach(GameObject plane in createdPlanes)
            {
            	plane.GetComponent<PlaneRenderer>().cleanupChildren();
                Network.Destroy(plane);
                Network.RemoveRPCs(plane.networkView.viewID);
                //Debug.Log("plane removed from toprace-minigame");
            }

            if(Network.isServer) {
            	Network.Destroy(currentCheckpoint);
            	Network.RemoveRPCs(currentCheckpoint.networkView.viewID);
            }
            currentCheckpoint = null;

            // Gives the winner(s) an overall point
            Game.Controller.getInstance().scores.endMinigameScoreHandling();	

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

		public override string getName() {
			return "Zombie";
		}
	}
}