
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using Game.UI;
using Game.Menu;
using Game.Level; 
using Game.Web;

namespace Game {
	public class Controller : MonoBehaviour {

		[Header ("Scores")]
		public ScoreController scores;
		public MenuList minigameScores;
		public MenuList overallScores;
		public FinalResultText number1;
		public FinalResultText number2;
		public FinalResultText number3;

		[Header ("Modes")]
		public BaseMode mainMode;
		public BaseMode[] miniModes;
		
		[Header ("Other")]
		public CountDown countdown;
		public Explanation explanation;
		public TerrainManager terrainManager;
		public LevelTour leveltour;
		public CountDownMiniGame countdownmg;

		public Mode activeMode;

		public GameObject[] getPlayers() {
			return GameObject.FindGameObjectsWithTag("Player");
		}

		public GameObject getActivePlayer() {
			foreach(GameObject player in getPlayers()) {
				Debug.Log(player.name);
				if(player.networkView.isMine) {
					return player;
				}
			}
			return null;
		}

		public void initialzeGame() {

			float rnd = Random.value;
			setupGame(rnd); // Prevent contention issues.
			terrainManager.placeAssets ();
			networkView.RPC("setupGame", RPCMode.OthersBuffered, rnd);
		}

		[RPC]
		public void setupGame(float rnd) {
			AudioSource backgroundaudio = GameObject.Find ("Background music").audio;
			if (!backgroundaudio.isPlaying) {
				backgroundaudio.Play();
			}
			terrainManager.updateTerrain(rnd);
		}

		public void serverBegin() {
			serverStartMiniGame();
			networkView.RPC("enableCamera", RPCMode.AllBuffered);
		}

		[RPC]
		public void enableCamera() {
			getActivePlayer().GetComponent<CameraFollower>().enabled = true;
		}

		public void serverStartMiniGame() {
			if(Network.isClient) { return; }
			if (activeMode != null) {
				AccountController.getInstance().createMinigameGameScores(activeMode.getScores(), (res) => {
					activeMode = null;
					serverStartMiniGame();
				});
				return;
			}
			// Server decides what minigame to play next.
			int nextId = Random.Range(0, miniModes.Length);
			AccountController.getInstance().createMinigameGame(miniModes[nextId].getName(), (res) => {
				networkView.RPC("startMiniGame", RPCMode.AllBuffered, nextId);
			});
		}

		[RPC]
		public void startMiniGame(int minigame) {
			if(Network.isServer) {
				Network.RemoveRPCs(terrainManager.networkView.viewID);
				terrainManager.networkView.RPC("randomTextures", RPCMode.AllBuffered, Random.value);
			}
			Debug.Log("starting mini");
			activeMode = miniModes[minigame];
			activeMode.beginMode(() => {
				if(Network.isServer) {
					// Remove previous start games from buffer.
					Network.RemoveRPCs(networkView.viewID);

					serverStartMiniGame();
				}
			});
		}

		public void disablePlayer() {
			GameObject active = getActivePlayer();
			if(active == null) { // player just joined, by default he is disabled.
				return;
			}

			active.rigidbody.useGravity = false;
			active.rigidbody.velocity = Vector3.zero;
			active.rigidbody.constraints = RigidbodyConstraints.FreezeAll;
		}

		public void enablePlayer() {
			GameObject active = getActivePlayer();

			active.rigidbody.useGravity = true;
			active.rigidbody.constraints = RigidbodyConstraints.None;
		}

		public void finishGame(){
			Invoke("endGame", 10);
		}

		// displays the final scare and ends the total game
		private void endGame(){
			Network.Disconnect();
			MasterServer.UnregisterHost();
			Application.LoadLevel("Menu");
		}

		void Update() {
			if (activeMode != null && activeMode.isActive()) {
				activeMode.onTick();
			}
		}

		public static Controller getInstance() {
			return GameObject.FindObjectOfType<Game.Controller>();
		}
		
	}
}