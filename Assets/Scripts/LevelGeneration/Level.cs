using UnityEngine;
using System.Collections;

public class Level : MonoBehaviour {

	public Terrain Arena;
	public int xResolution;
	public int zResolution;

	public int x = 0;

	// x range is 140 to 370

	// Use this for initialization
	void Update () {
		editTerrain();	
	}

	void editTerrain(){
		xResolution = Arena.terrainData.heightmapWidth;
		zResolution = Arena.terrainData.heightmapHeight;
		float[,] heights = Arena.terrainData.GetHeights (0, 0, xResolution, zResolution);
		for (int z = 0; z < zResolution; z++) {
			heights[x,z] = 0.5f;
		}

		//Arena.terrainData.SetHeights (0, 0, heights);
	}
}
