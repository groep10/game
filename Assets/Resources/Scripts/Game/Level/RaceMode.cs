using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Game.UI;

namespace Game.Level {
	public class RaceMode : BaseMode {

		public Terrain referenceTerrain;
		
		private Terrain Arena;
		
		public float terrainRadius = 200;
		// Prefab for the checkpoint
		public GameObject checkpoint;
		
		private int xResolution;
		private int zResolution;
		
		private float checkpointTimer = 60;
		
		public Texture2D grass;
		public Texture2D cliff;
		public Texture2D rocks;
		public Texture2D dirt;
		
		private Texture2D tex;
		private Texture2D tex2;
		
		private GameObject cpnt;
		
		public static GeneticPlacement algorithm = new GeneticPlacement();

		private EventHandler onDone;
		
		public override void beginMode(EventHandler finishHandler) {
			running = true;
			onDone = finishHandler;

			Arena = this.gameObject.AddComponent<Terrain> ();
			
			TerrainData terrainData = new TerrainData ();
			CopyTerrainDataFromTo(referenceTerrain.terrainData, ref terrainData);
			
			Arena.terrainData = terrainData;
			GetComponent<TerrainCollider> ().terrainData = Arena.terrainData;
			
			editTerrain ();
			randomTextures (0.5f);
			if (Network.isServer) {
				Invoke ("setCheckpoint", 5);
			}
		}
		
		// edits the terrain according to the radius that is set.
		public void editTerrain() {
			// Take the resolution of the terrain as the boundaries
			xResolution = Arena.terrainData.heightmapWidth;
			zResolution = Arena.terrainData.heightmapHeight;
			
			// Debug.Log (Arena.terrainData.size.x + " " + Arena.terrainData.size.y + " " + Arena.terrainData.size.z);
			// Debug.Log (xResolution + " " + zResolution);
			// Retrieve the heightmap of the terrain
			float[,] heights = Arena.terrainData.GetHeights (0, 0, xResolution, zResolution);
			// Loop through all values and create a circle with the set radius
			for (int z = -zResolution / 2; z < zResolution / 2; z++) {
				for (int x = -xResolution / 2; x < xResolution / 2; x++) {
					if (Mathf.Sqrt(Mathf.Pow(x , 2) + Mathf.Pow(z , 2)) >= terrainRadius) {
						heights[x + xResolution / 2, z + zResolution / 2] = 0.5f;
					} else {
						heights[x + xResolution / 2, z + zResolution / 2] = 0;
					}
				}
			}
			Arena.terrainData.SetHeights (0, 0, heights);
			
			// server applies textures
			if (Network.isServer) {
				float rnum = UnityEngine.Random.value;
				networkView.RPC("randomTextures", RPCMode.AllBuffered, rnum);
			}
		}
		
		[RPC]
		void randomTextures(float num) {
			// assign 2 textures to terrain
			SplatPrototype[] arenaTexture = new SplatPrototype[2];
			arenaTexture [0] = new SplatPrototype ();
			arenaTexture [1] = new SplatPrototype ();
			
			if (num < 0.25) {
				tex = rocks;
				tex2 = grass;
			} else if (num >= 0.25 && num < 0.5) {
				tex = cliff;
				tex2 = grass;
			} else if (num >= 0.5 && num < 0.75) {
				tex = rocks;
				tex2 = dirt;
			} else {
				tex = cliff;
				tex2 = dirt;
			}
			arenaTexture [0].texture = tex;
			arenaTexture [1].texture = tex2;
			Arena.terrainData.splatPrototypes = arenaTexture;
			applyTextures ();
		}
		
		void applyTextures() {
			float[, ,] map = new float[Arena.terrainData.alphamapWidth, Arena.terrainData.alphamapHeight, 2];
			
			// For each point on the alphamap...
			for (var y = 0; y < Arena.terrainData.alphamapHeight; y++) {
				for (var x = 0; x < Arena.terrainData.alphamapWidth; x++) {
					// Get the normalized terrain coordinate that
					// corresponds to the the point.
					var normX = x * 1.0 / (Arena.terrainData.alphamapWidth - 1);
					var normY = y * 1.0 / (Arena.terrainData.alphamapHeight - 1);
					
					// Get the steepness value at the normalized coordinate.
					var angle = Arena.terrainData.GetSteepness((float)normX, (float)normY);
					
					// Steepness is given as an angle, 0..90 degrees. Divide
					// by 90 to get an alpha blending value in the range 0..1.
					var frac = angle / 90.0;
					map[x, y, 0] = (float)frac;
					map[x, y, 1] = 1 - (float)frac;
				}
			}
			
			Arena.terrainData.SetAlphamaps(0, 0, map);
		}
		
		
		// sets the checkpoint in the arena
		public void setCheckpoint() {
			//Debug.Log ("Setting new checkpoint");
			//float x = Random.Range (-300, 300);
			//float z = Random.Range (-300, 300);
			
			Vector2 locXZ = algorithm.runGeneticAlgorithm ();
			float locX = locXZ.x;
			float locZ = locXZ.y;
			Vector3 location = new Vector3(locX, 0f, locZ);
			
			cpnt = (GameObject) Network.Instantiate (checkpoint, location, Quaternion.identity, 0);
			Invoke("destroyCP", checkpointTimer);
			Invoke("setCheckpoint", checkpointTimer);
		}
		
		// Destroys the checkpoint
		public void destroyCP() {
			//Debug.Log ("Destroying checkpoint");
			CancelInvoke();
			Network.Destroy (cpnt.networkView.viewID);
			Network.RemoveRPCs (cpnt.networkView.viewID);
		}
		
		private Hashtable table = new Hashtable();
		
		public void increasePlayerMinigameScore(string playername) {
			if (!table.ContainsKey(playername)) {
				table[playername] = 0;
			}
			table[playername] = (int)table[playername] + 1;
			updateMiniGameScores();
		}
		
		public void updateMiniGameScores() {
			Game.Controller.getInstance().minigameScores.reset();
			Game.Controller.getInstance().minigameScores.addScore("Mode: zombie");
			
			foreach (DictionaryEntry de in table) {
				Game.Controller.getInstance().minigameScores.addScore(de.Key + ": " + de.Value);
			}
		}
		
		void CopyTerrainDataFromTo(TerrainData tDataFrom, ref TerrainData tDataTo)
		{
			tDataTo.SetDetailResolution(tDataFrom.detailResolution, 8);
			tDataTo.heightmapResolution = tDataFrom.heightmapResolution;
			tDataTo.alphamapResolution = tDataFrom.alphamapResolution;
			tDataTo.baseMapResolution = tDataFrom.baseMapResolution;
			tDataTo.size = tDataFrom.size;
			tDataTo.splatPrototypes = tDataFrom.splatPrototypes;
		}
		
		public override void onTick() {

		}

		public override string getName() {
			return "race";
		}
	}
}

