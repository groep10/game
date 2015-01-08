using UnityEngine;

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
			Arena = this.gameObject.AddComponent<Terrain>();
			Arena.enabled = false;

			TerrainData terrainData = new TerrainData ();
			CopyTerrainDataFromTo(referenceTerrain.terrainData, ref terrainData);

			Arena.terrainData = terrainData;
			GetComponent<TerrainCollider> ().terrainData = Arena.terrainData;
		}

		// edits the terrain according to the radius that is set.
		public void updateTerrain() {
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

			// server applies textures
			if (Network.isServer) {
				float rnum = Random.value;
				networkView.RPC("randomTextures", RPCMode.AllBuffered, rnum);
			}
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
			tDataTo.splatPrototypes = tDataFrom.splatPrototypes;
		}
	}

}