using UnityEngine;
using System.Collections;

public class Level : MonoBehaviour {

	public Terrain Arena;
	public int xResolution;
	public int zResolution;
	public bool makeLarger;
	public float radius = 200;

	void Start(){
		// creates an arena terrain with radius 200
		editTerrain ();
	}

	// Use this for initialization
	void Update () {
		// makes the arena subsequently smaller and larger with a radius ranging between 150 and 200
		adjustTerrainRadius ();
	}

	void FixedUpdate(){
	}

	void editTerrain(){
		// Take the resolution of the terrain as the boundaries
		xResolution = Arena.terrainData.heightmapWidth;
		zResolution = Arena.terrainData.heightmapHeight;
		// Retrieve the heightmap of the terrain
		float[,] heights = Arena.terrainData.GetHeights (0, 0, xResolution, zResolution);
		// Loop through all values and create a circle with the set radius
		for (int z = -zResolution/2; z < zResolution/2; z++) {
			for(int x = -xResolution/2; x < xResolution/2; x++){
				if(Mathf.Sqrt(Mathf.Pow(x ,2) + Mathf.Pow(z ,2)) >= radius){
					heights[x + xResolution/2,z + zResolution/2] = 0.5f;
				}
				else
					heights[x + xResolution/2, z + zResolution/2] = 0;
			}
		}
		Arena.terrainData.SetHeights (0, 0, heights);
	}
	
	void adjustTerrainRadius(){
		if (makeLarger) {
			if(radius < 200) {
				radius += Time.deltaTime * 5;
			} else {
				makeLarger = false;
			}
		} else {
			if(radius > 150) {
				radius -= Time.deltaTime * 5;
			} else {
				makeLarger = true;
			}
		}
		editTerrain ();
	}

}
