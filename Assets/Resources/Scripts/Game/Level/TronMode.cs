using UnityEngine;
using System.Collections;

using Game.Level.Tron;

namespace Game.Level {
	public class TronMode : BaseMode {

		public float finishTimer = 120f;
		private bool finished;
		private GameObject[] deadPlayers;

		private Hashtable dead;

		public override void beginMode(System.Action finishHandler) {
			base.beginMode (finishHandler);

			Debug.Log("Starting Tron");

			dead = new Hashtable ();

			GameObject[] players = Game.Controller.getInstance ().getPlayers ();
			for(int i = 0; i < players.Length; i+= 1) {
				GameObject player = players[i];
				TronLineRenderer line = player.AddComponent<TronLineRenderer>();
				line.setColor(i);
				player.AddComponent<TronPlayerStatus>();
			}
			if (Network.isServer) {
				Invoke("onTimerEnd", finishTimer);
			}
		}

		// called when a player dies in tron
		public void isPlayerDead(){

		}

		public override void onTick() {
			if (Network.isClient) {
				return;
			}
			GameObject[] players = Game.Controller.getInstance().getPlayers();
			if(players.Length <= 1) { // Singleplayer ish mode
				return;
			}
			int alive = 0;
			foreach(GameObject player in players) {
				TronPlayerStatus ps = player.GetComponent<TronPlayerStatus> ();
				if(ps != null && !ps.dead) {
					alive++;
				}
			}
			if(alive > 1 || alive == 0) {
				return;
			}
			networkView.RPC("onGameFinish", RPCMode.All);
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

			Invoke("endMode", 5);
		}

		public override void endMode() {
			Debug.Log("Finish Tron");

			// increase the overall score of the winner by 1
			Game.Controller.getInstance().scores.endMinigame();
			
			base.endMode();
		}

		public override void reset() {
			finished = false;

			base.reset();
		}

		public override string getName() {
			return "Tron";
		}
	}
}