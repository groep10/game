using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level : MonoBehaviour {

	public Terrain Arena;
	public int xResolution;
	public int zResolution;
	public bool makeLarger;
	public float terrainRadius = 200;
	public GameObject checkpoint;
	private float checkpointTimer = 60;

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

	// sets the checkpoint in the arena
	void setCheckpoint(){
		float x = Random.Range (-300, 300);
		float z = Random.Range (-300, 300);
		Vector3 location = new Vector3(x, 50f, z);
		GameObject cpnt = (GameObject) Instantiate (checkpoint, location, Quaternion.identity);
		Destroy (cpnt, checkpointTimer);
		Invoke("setCheckpoint", checkpointTimer);
	}

	/* ------------ GENETIC ALGORITHM TO PLACE THE CHECKPOINT -------------------------- */
	// Genetic Algorithm variables
	private int amountOfChromosomes = 30;

	// returns a  Vector2 chromosome representing the possible x and y coordinates of the checkpoint
	Vector2 createChromosome(){
		float x = Random.Range (-300, 300);
		float z = Random.Range (-300, 300);
		Vector2 result = new Vector2 (x, z);
		return result;
	}

	// returns a generation of coordian
	List<Vector2> createFirstGeneration(){
		List<Vector2> result = new List<Vector2>();
		for (int i = 0; i < amountOfChromosomes; i++){
			result.Add(createChromosome());
		}
		return result;
	}

	// mutates the chromosome by setting either the x or the z value randomly within a certain range
	void mutate(Vector2 chrom){
		float chance = Random.Range (0f, 1f);
		if (chance >= 0.5){
			chrom.x = Random.Range (-300, 300);
		}
		else{
			chrom.y = Random.Range(-300, 300);
		}
	}

	// replace 2 parent chromosomes with 2 child chromosomes using crossover mixing their coordinates
	void crossover(Vector2 chrom1, Vector2 chrom2){
		// coordinates
		float x1 = chrom1.x;
		float z1 = chrom1.y;
		float x2 = chrom2.x;
		float z2 = chrom2.y;

		// create children
		Vector2 child1 = new Vector2 (x1, z2);
		Vector2 child2 = new Vector2 (x2, z1);

		// replace parents with children
		chrom1 = child1;
		chrom2 = child2;
	}






	/* ---------------- END OF GENETIC ALGORITHM ------------------------------ */


	void Start(){
		// creates an arena terrain with radius 200
		editTerrain ();
		Invoke ("setCheckpoint", 5);
	}
}


