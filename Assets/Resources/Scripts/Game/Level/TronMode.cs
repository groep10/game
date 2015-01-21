using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Game.Level.Tron;
using Game.Net;

namespace Game.Level {
	public class TronMode : BaseMode {

		public float finishTimer = 120f;
		private bool finished;
		private bool singleplayer;
		private GameObject[] deadPlayers;
		private List<GameObject> graveStones = new List<GameObject>(4);

		private Hashtable dead;
		private int alive;

		public override void beginMode(System.Action finishHandler) {
			base.beginMode (finishHandler);

			Debug.Log("Starting Tron");

			dead = new Hashtable ();

			Game.Controller.getInstance ().scores.initializeTronScores ();


			Game.Controller.getInstance ().disablePlayer();

			Game.Controller.getInstance ().countdown.beginCountdown ();
			Invoke ("starting", 3);
			loadGravestones();


		}

		public void starting() {
			Game.Controller.getInstance ().enablePlayer();

			Game.Controller.getInstance ().explanation.setExplanation("Don't bump in to a line! Stay alive to win a point!");
			GameObject[] players = Game.Controller.getInstance ().getPlayers ();
			alive = players.Length;

			Game.Controller.getInstance ().countdownmg.beginCountdownmg ();
			for (int i = 0; i < players.Length; i += 1) {
				GameObject player = players[i];
				TronLineRenderer line = player.AddComponent<TronLineRenderer>();
				line.setColor(i);
				TronPlayerStatus comp = player.AddComponent<TronPlayerStatus>();
				comp.mode = this;
			}

			if (Network.isServer) {
				Invoke("onTimerEnd", finishTimer);
			}
		}

		void loadGravestones(){
			foreach (Object o in Resources.LoadAll("Prefabs/Game/Level")) {
				if (!(o is GameObject)) {
					continue;
				}
				GameObject go = (GameObject) o;
				if (go.tag == "TronGrave") {
					graveStones.Add(go);
				}
			}
		}

		// called when a player dies in tron
		[RPC]
		public void notifyDeath(string playername) {
			Game.Controller.getInstance ().scores.deadTronPlayer (playername);			
			if (Network.isClient) {
				return;
			}

			dead [playername] = true;
			alive--;
			if(singleplayer){
				onTimerEnd();
			}
		}

		[RPC]
		public void placeGravestone(string playername){
			if (alive <= 1){
				return;
			}
			GameObject[] players = Game.Controller.getInstance().getPlayers();
			foreach (GameObject go in players){
				string name = go.GetComponent<PlayerInfo>().getUsername();
				if (name.Equals(playername)){
					Vector3 deathLocation = go.transform.position;
					GameObject graveStone = (GameObject) Network.Instantiate (graveStones[0], deathLocation, Quaternion.Euler(0f, 0f, 0f), 0);
					
					//go.active = false;
				}
			}
			
		}

		public override void onTick() {
			if (Network.isClient) {
				return;
			}
			GameObject[] players = Game.Controller.getInstance().getPlayers();
			if (players.Length <= 1) { // Singleplayer ish mode
				singleplayer = true;
				return;
			}
			if (alive != 1) {
				return;
			}

			alive--;

			// string playername = null;
			// foreach (GameObject player in players) {
			// 	if (dead.ContainsKey(player.GetComponent<PlayerInfo>().getUsername())) {
			// 		continue;
			// 	}
			// 	playername = player.GetComponent<PlayerInfo>().getUsername();
			// }

			// if (playername != null) {
			// 	networkView.RPC("winTron", RPCMode.All, playername);
			// }
			networkView.RPC("onGameFinish", RPCMode.All);
		}

		public void onTimerEnd() {
			networkView.RPC("onGameEnd", RPCMode.All);
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
		public void onGameFinish() {
			if (finished) {
				return;
			}
			onGameDone();
		}

		private void onGameDone() {
			Game.Controller.getInstance ().countdownmg.eindCountdown ();
			finished = true;

			TronLineRenderer[] rend = GameObject.FindObjectsOfType<TronLineRenderer> ();
			foreach (TronLineRenderer ren in rend) {
				Destroy(ren);
			}

			TronPlayerStatus[] stats = GameObject.FindObjectsOfType<TronPlayerStatus> ();
			foreach (TronPlayerStatus stat in stats) {
				Destroy(stat);
			}
			if(Network.isServer) {
				GameObject[] graves = GameObject.FindGameObjectsWithTag("TronGrave");	
				foreach (GameObject go in graves){
					Network.Destroy(go);
				}
			}
			GameObject[] players = Game.Controller.getInstance().getPlayers();
			foreach (GameObject go in players){
				go.active = true;
			}
			// Gives the winner(s) an overall point
			Game.Controller.getInstance().scores.endMinigameScoreHandling();

			Invoke("endMode", 5);
		}

		public override void endMode() {
			Debug.Log("Finish Tron");

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
			return "Tron";
		}
	}
}