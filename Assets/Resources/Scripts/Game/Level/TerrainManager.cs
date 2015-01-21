using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace Game.Level {

	public class TerrainManager : MonoBehaviour {

		public Terrain referenceTerrain;

		private Terrain Arena;

		public float terrainRadius = 200;

		private int xResolution;
		private int zResolution;

		public Texture2D grass;
		public Texture2D cliff;
		public Texture2D rocks;
		public Texture2D dirt;

		private Texture2D tex;
		private Texture2D tex2;

		public void Start() {
			Arena = this.GetComponent<Terrain>();
			Arena.enabled = false;

			TerrainData terrainData = new TerrainData ();
			CopyTerrainDataFromTo(referenceTerrain.terrainData, ref terrainData);

			Arena.terrainData = terrainData;
			GetComponent<TerrainCollider> ().terrainData = Arena.terrainData;
		}

		// edits the terrain according to the radius that is set.
		public void updateTerrain(float num) {
			if (!Arena.enabled) {
				Arena.enabled = true;
			}
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
			randomTextures(num);
		}

		[RPC]
		public void randomTextures(float num) {
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

		void CopyTerrainDataFromTo(TerrainData tDataFrom, ref TerrainData tDataTo) {
			tDataTo.SetDetailResolution(tDataFrom.detailResolution, 8);
			tDataTo.heightmapResolution = tDataFrom.heightmapResolution;
			tDataTo.alphamapResolution = tDataFrom.alphamapResolution;
			tDataTo.baseMapResolution = tDataFrom.baseMapResolution;
			tDataTo.size = tDataFrom.size;
			// tDataTo.splatPrototypes = tDataFrom.splatPrototypes;
		}


		private List<GameObject> assets;
		private GameObject[] placedAssets;
		private int assetsInArena = 10;

		public void placeAssets() {
			loadAssets();
			for (int i = 0; i < assetsInArena; i++) {
				placeAsset();
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

		public void cacheAssets() {
			placedAssets = GameObject.FindGameObjectsWithTag ("ArenaAsset");
		}

		public void placeAsset() {
			// randomise the location within x and z boundaries
			cacheAssets();
			Vector3 location = findLocation();

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

		public Vector3 findLocation() {
			float x = 0;
			float z = 0;
			Vector3 location = new Vector3(x, 0f, z);
			bool fits = true;
			int it = 0;
			do {
				fits = true;
				x = Random.Range (-500, 500);
				z = Random.Range (-500, 500);
				location = new Vector3(x, 0f, z);
				fits = checkLocation(location);
				it++;
			} while (fits == false && it < 100);
			return location;

		}

		//checks if a new location is too close to any existing arena assets.
		public bool checkLocation(Vector3 location) {
			foreach (GameObject go in placedAssets) {
				Vector3 pos = go.transform.position;

				if (Vector3.Distance(location, pos) < 25) {
					return false;
				}
			}
			return true;
		}
	}

}