using UnityEngine;
using System.Collections;

using Game.Level.Tron;

namespace Game.Level {
	public class TronMode : BaseMode {

		public float finishTimer = 120f;
		private bool finished;

		public override void beginMode(System.Action finishHandler) {
			base.beginMode (finishHandler);

			Debug.Log("Starting Tron");

			foreach(GameObject player in Game.Controller.getInstance().getPlayers()) {
				player.AddComponent<TronLineRenderer>();
				//if(player.networkView.isMine) {
				//}
				player.AddComponent<TronPlayerStatus>();
			}
			if (Network.isServer) {
				Invoke("onTimerEnd", finishTimer);
			}
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
			foreach (TronPlayerStatus stat in rend) {
				Destroy(stat);			
			}

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

		public override string getName() {
			return "Tron";
		}
	}
}