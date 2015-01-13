using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using Game.UI;
using Game.Level.TopRace;

namespace Game.Level {
	public class TopRaceMode : BaseMode {

		public GameObject planePrefab;

		int numberOfPlanes = 6;
		int planeSpacing = 25;

		public float finishTimer = 120f;

		private bool finished;

		private List<GameObject> createdPlanes = new List<GameObject>();

		public override void beginMode(System.Action finishHandler) {
			base.beginMode (finishHandler);

			Debug.Log("Starting TopRace");

			//CountDown ();
			Invoke ("Starting",3);

		}

		void Starting(){
			Game.Controller.getInstance().scores.updateRaceToTheTopScores();
			
			if(Network.isServer) {
				Invoke("onGameEnd", finishTimer);
				generatePlanes();
			}
		}

		public override void onTick() {

		}

		// generates the planes including their connection ramps
		void generatePlanes()
		{
			Vector3 location = new Vector3(0, 0, 0);
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
			onGameDone();
		}

		private void onGameDone() {
			finished = true;

            foreach(GameObject plane in createdPlanes)
            {
            	plane.GetComponent<PlaneRenderer>().cleanupChildren();
                Network.Destroy(plane);
                Network.RemoveRPCs(plane.networkView.viewID);
                Debug.Log("plane removed from toprace-minigame");
            }

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