using UnityEngine;
using System.Collections;

using Game.Level.Tron;
using Game.Net;

namespace Game.Level {
	public class TronMode : BaseMode {

		public float finishTimer = 120f;
		private bool finished;
		private GameObject[] deadPlayers;

		private Hashtable dead;
		private int alive;

		public override void beginMode(System.Action finishHandler) {
			base.beginMode (finishHandler);

			Debug.Log("Starting Tron");

			dead = new Hashtable ();

			Game.Controller.getInstance ().scores.initializeTronScores ();

			Game.Controller.getInstance ().countdown.beginCountdown ();

			Invoke ("starting", 3);
		}

		public void starting() {
			GameObject[] players = Game.Controller.getInstance ().getPlayers ();
			alive = players.Length;
			
			for(int i = 0; i < players.Length; i+= 1) {
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

		// called when a player dies in tron
		[RPC]
		public void notifyDeath(string playername){
			Game.Controller.getInstance ().scores.deadTronPlayer (playername);

			if (Network.isClient) {
				return;
			}

			dead [playername] = true;
			alive--;
		}

		public override void onTick() {
			if (Network.isClient) {
				return;
			}
			GameObject[] players = Game.Controller.getInstance().getPlayers();
			if(players.Length <= 1) { // Singleplayer ish mode
				return;
			}
			if (alive != 1) {
				return;
			}

			alive--;
			
			string playername = null;
			foreach(GameObject player in players) {
				if(dead.ContainsKey(player.GetComponent<PlayerInfo>().getUsername())) {
					continue;
				}
				playername = player.GetComponent<PlayerInfo>().getUsername();
			}

			if (playername != null) {
				networkView.RPC("winTron", RPCMode.All, playername);
			}
			networkView.RPC("onGameFinish", RPCMode.All);
		}

		[RPC]
		public void winTron(string playername) {
			Game.Controller.getInstance ().scores.increaseOverallScore (playername);
		}


		public void onTimerEnd() {
			networkView.RPC("onGameEnd", RPCMode.All);
		}

		// Called when game ends by timer
		[RPC]
		public void onGameEnd() {
			if(finished) {
				return;
			}
			onGameDone();
		}

		[RPC]
		public void onGameFinish() {
			if(finished) {
				return;
			}
			onGameDone();
		}

		private void onGameDone() {
			finished = true;

			TronLineRenderer[] rend = GameObject.FindObjectsOfType<TronLineRenderer> ();
			foreach (TronLineRenderer ren in rend) {
				Destroy(ren);			
			}

			TronPlayerStatus[] stats = GameObject.FindObjectsOfType<TronPlayerStatus> ();
			foreach (TronPlayerStatus stat in stats) {
				Destroy(stat);			
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
				scores[i]["score"] = (string) Game.Controller.getInstance().scores.getMinigameScore(pi.getUsername()) == "alive" ? 1 : 0;
			}
			return scores;
		}

		public override string getName() {
			return "Tron";
		}
	}
}