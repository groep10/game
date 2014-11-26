using UnityEngine;
using System.Collections;

public class Level : MonoBehaviour {

	public Terrain Arena;
	public int xResolution;
	public int zResolution;
	public bool makeLarger;
	public float terrainRadius = 200;
	public GameObject checkpoint;

	private float checkpointTimer = 60;

	void Start(){
		// creates an arena terrain with radius 200
		editTerrain ();
		Invoke ("setCheckpoint", 5);
	}

	// Use this for initialization
	void Update () {
	}

	// sets the checkpoint in the arena
	void setCheckpoint(){
		float x = Random.Range (-300, 300);
		float z = Random.Range (-300, 300);
		Vector3 location = new Vector3(x, 50f, z);
		GameObject cpnt = (GameObject) Instantiate (checkpoint, location, Quaternion.identity);
		Destroy (cpnt, checkpointTimer);
		Invoke("setCheckpoint", checkpointTimer);
	}

	// edits the terrain according to the radius that is set.
	void editTerrain(){
		// Take the resolution of the terrain as the boundaries
		xResolution = Arena.terrainData.heightmapWidth;
		zResolution = Arena.terrainData.heightmapHeight;
		// Retrieve the heightmap of the terrain
		float[,] heights = Arena.terrainData.GetHeights (0, 0, xResolution, zResolution);
		// Loop through all values and create a circle with the set radius
		for (int z = -zResolution/2; z < zResolution/2; z++) {
			for(int x = -xResolution/2; x < xResolution/2; x++){
				if(Mathf.Sqrt(Mathf.Pow(x ,2) + Mathf.Pow(z ,2)) >= terrainRadius){
					heights[x + xResolution/2,z + zResolution/2] = 0.5f;
				}
				else
					heights[x + xResolution/2, z + zResolution/2] = 0;
			}
		}
		Arena.terrainData.SetHeights (0, 0, heights);
	}

}
