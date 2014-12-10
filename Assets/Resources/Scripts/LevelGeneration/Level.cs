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
		//float x = Random.Range (-300, 300);
		//float z = Random.Range (-300, 300);

		Vector2 locXZ = runGeneticAlgorithm ();
		float locX = locXZ.x;
		float locZ = locXZ.y;
		Vector3 location = new Vector3(locX, 50f, locZ);

		GameObject cpnt = (GameObject) Instantiate (checkpoint, location, Quaternion.identity);
		Destroy (cpnt, checkpointTimer);
		Invoke("setCheckpoint", checkpointTimer);
	}

	/* ------------ GENETIC ALGORITHM TO PLACE THE CHECKPOINT -------------------------- */
	// Variables
	private int chromosomesPerGeneration = 30;
	private int maxGenerations = 50;
	private float mutationProb = 0.1f;
	private float crossoverProb = 0.7f;

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
		for (int i = 0; i < chromosomesPerGeneration; i++){
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

        chrom1.y = z2;
        chrom2.x = x1;

        // create children
        //Vector2 child1 = new Vector2 (x1, z2);
        //Vector2 child2 = new Vector2 (x2, z1);

        // replace parents with children
        //chrom1 = child1;
        //chrom2 = child2;
	}

	// returns the fitness as a float for a pair of coordinates
	float calculateFitness(Vector2 chrom, List<Vector2> players){
		List<float> distances = new List<float>();
		// calculate the distances between the chromosome and the player
		for (int i =0; i < players.Count; i++){
			distances.Add(Vector2.Distance (chrom, players[i]));
		}

		float min = Mathf.Min (distances[0], distances[1], distances[2], distances[3]);
		float max = Mathf.Max (distances[0], distances[1], distances[2], distances[3]);
		float difference = max - min;

		float fitness = 1 / difference;
		return fitness;
	}

	// returns a list floats containing all the fitnesses of a generation
	List<float> fitnessOfGeneration(List<Vector2> generation, List<Vector2> players){
		List<float> result = new List<float>();
		// calculate the fitness for each chromosome in the generation
		foreach (Vector2 chrom in generation){
			result.Add(calculateFitness(chrom, players));
		}
		return result;
	}

	// calculates the sum of all the elements of an array of floats
	float sum(List<float> array){
		float sum = 0f;
		foreach (float e in array){
			sum += e;
		}
		return sum;
	}

	float average(List<float> array){
		float total = sum (array);
		float average = total / array.Count;
		return average;
	}

	// returns a piechart distribution array of the fitness of a generation
	List<float> fitnessPiechart(List<float> fitnesses){
		float totalFitness = sum (fitnesses);
		List<float> piechart = new List<float>();

        //piechart.Add((fitnesses[0] / totalFitness) * 100);
		for (int i = 0; i < fitnesses.Count; i++) {
			piechart.Add((fitnesses [i] / totalFitness) * 100);
		}
		return piechart;
	}

	// returns the index of a randomly picked element from the piechart
	int pickFromPiechart(List<float> piechart){
		float number = Random.Range (0f, 100f);
		for (int i = 0; i < piechart.Count; i++) {
			float value = piechart[i];
            number -= value;
            if (number <= 0)
            {
                return i;
            }
		}
		return -1;
	}

	Vector2 runGeneticAlgorithm(){
		// made up player-coordinates to test (ideal result with these coordinates is 0f,0f)
		Vector2 player1 = new Vector2 (-200f, 200f);
		Vector2 player2 = new Vector2 (200f, 200f);
		Vector2 player3 = new Vector2 (200f, -200f);
		Vector2 player4 = new Vector2 (-200f, -200f);

		// create a list containing the coordinates of the players
		List<Vector2> players = new List<Vector2> ();
		players.Add (player1);
		players.Add (player2);
		players.Add (player3);
		players.Add (player4);

		// run the actual GA
		float overallBestFitness = 0f;
		Vector2 overallBestChrom = new Vector2 ();
		List<Vector2> currentGeneration = createFirstGeneration ();
		
		for (int h=0; h < maxGenerations; h++) {
			Debug.Log("Generation:" + h);

			List<float> fitnesses = fitnessOfGeneration (currentGeneration, players);
			List<float> piechart = fitnessPiechart (fitnesses);

			// extract information from the generation
			float generationBestFitness = Mathf.Max(fitnesses.ToArray());
			if (generationBestFitness > overallBestFitness){
				overallBestFitness = generationBestFitness;
				overallBestChrom = currentGeneration[fitnesses.IndexOf(generationBestFitness)];
			}
			float generationAverage = average(fitnesses);

			Debug.Log("generationAverage:" + generationAverage);
			Debug.Log("generationBestFitness:" + generationBestFitness);
			Debug.Log("overallBestFitness:" + overallBestFitness);


			// create a new generation
			List<Vector2> newGeneration = new List<Vector2> ();
			for (int i = 0; i < chromosomesPerGeneration; i++) {
				int index = pickFromPiechart (piechart);
				newGeneration.Add (currentGeneration [index]);
			}

			/* apply crossover and mutation */
			// crossover
			for (int j = 0; j < newGeneration.Count; j += 2) {
				float crossoverChance = Random.Range (0f, 1f);
				if (crossoverChance <= crossoverProb) {
					crossover (newGeneration [j], newGeneration [j + 1]);
				}
			}
			// mutation
			for (int k = 0; k < newGeneration.Count; k++) {
				float mutationChance = Random.Range (0f, 1f);
				if (mutationChance <= mutationProb) {
					mutate (newGeneration [k]);
				}
			}

			// set newGeneration as currentGeneration
			currentGeneration = newGeneration;
		}
		return overallBestChrom;
	}

	/* ---------------- END OF GENETIC ALGORITHM ------------------------------ */


	void Start(){
		// creates an arena terrain with radius 200
		editTerrain ();
		Invoke ("setCheckpoint", 5);
	}
}


