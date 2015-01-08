using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using Game.UI;
using Game.Level.Race;

namespace Game.Level {
	public class ZombieMode : BaseMode {

		public GameObject enemyPrefab;
		public float spawnTime = 3f;

		// TODO: combine with score management code
		public int maxScore = 30;

		public float finishTimer = 120f;

		private bool finished;

		private Vector3 spawnPoint;

		public override void beginMode(System.Action finishHandler) {
			base.beginMode (finishHandler);

			Debug.Log("Starting Zombie");

			InvokeRepeating ("spawnEnemy", spawnTime, spawnTime);
		}

		public override void onTick() {

		}

		void spawnEnemy () {
			if (GameObject.FindGameObjectsWithTag ("Enemy").Length < 30) {
				// Find a random index between zero and one less than the number of spawn points.
				// Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
				Network.Instantiate (enemyPrefab, new Vector3 (Random.Range (-500, 500), 3f, Random.Range (-500, 500)), Quaternion.identity, 0);
			}
		}

		public void onGameEnd() {
			if(finished) {
				return;
			}

			finished = true;
			Invoke("endMode", 5);
		}

		public void onGameFinish() {
			if(finished) {
				return;
			}

			finished = true;
			Invoke("endMode", 5);
		}

		public override void endMode() {
			Debug.Log("Finish Zombie");

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