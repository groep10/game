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
	private float checkpointTimer = 10;

	public Texture2D grass;
	public Texture2D cliff;
	public Texture2D rocks;
	public Texture2D dirt;
	private Texture2D tex;
	private Texture2D tex2;

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

		// server applies textures
		if(Network.isServer){
			float rnum = Random.value;
			networkView.RPC("randomTextures",RPCMode.AllBuffered,rnum);
		}
	}

	[RPC]
	void randomTextures(float num){
		// assign 2 textures to terrain
		SplatPrototype[] arenaTexture = new SplatPrototype[2];
		arenaTexture [0] = new SplatPrototype ();
		arenaTexture [1] = new SplatPrototype ();
		
		if(num<0.25){
			tex = rocks;
			tex2 = grass;
		}				
		else if(num>=0.25 && num<0.5){
			tex = cliff;
			tex2 = grass;
		}
		else if(num>=0.5 && num<0.75){
			tex = rocks;	
			tex2= dirt;
		}	
		else {
			tex = cliff;
			tex2 = dirt;
		}
		arenaTexture [0].texture = tex; 
		arenaTexture [1].texture = tex2;
		Arena.terrainData.splatPrototypes = arenaTexture;
		applyTextures ();
	}
	
	void applyTextures(){
		float[,,] map = new float[Arena.terrainData.alphamapWidth, Arena.terrainData.alphamapHeight, 2];
		
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
	void setCheckpoint(){
		//float x = Random.Range (-300, 300);
		//float z = Random.Range (-300, 300);

		Vector2 locXZ = runGeneticAlgorithm ();
		float locX = locXZ.x;
		float locZ = locXZ.y;
		Vector3 location = new Vector3(locX, 0f, locZ);

		GameObject cpnt = (GameObject) Network.Instantiate (checkpoint, location, Quaternion.identity, 0);
		Network.Destroy (cpnt, checkpointTimer);
		Invoke("setCheckpoint", checkpointTimer);
	}

	/* ------------ GENETIC ALGORITHM TO PLACE THE CHECKPOINT -------------------------- */
	// Variables
	private int chromosomesPerGeneration = 20; // must be an even number
	private int maxGenerations = 30;
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
		float range = Random.Range (-100f, 100f);
		float var = Random.Range (0f, 1f);
		if (var < 0.5f) {
			chrom.x = Mathf.Min(Mathf.Max(chrom.x + range, -300), 300);
		} else {
			chrom.y = Mathf.Min(Mathf.Max(chrom.y + range, -300), 300);
		}
	}

	// replace 2 parent chromosomes with 2 child chromosomes using crossover mixing their coordinates
	void crossover(Vector2 chrom1, Vector2 chrom2){
		// coordinates
		float x1 = chrom1.x;
		float z1 = chrom1.y;
		float x2 = chrom2.x;
		float z2 = chrom2.y;

		float var = Random.Range (0f, 1f);
		if (var < 0.5f) {
			chrom1.y = z2;
			chrom2.x = x1;
		} else {
			chrom1.x = x2;
			chrom2.y = z1;
		}
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
			//Debug.Log("Generation:" + h);

			List<float> fitnesses = fitnessOfGeneration (currentGeneration, players);
			List<float> piechart = fitnessPiechart (fitnesses);

			// extract information from the generation
			float generationBestFitness = Mathf.Max(fitnesses.ToArray());
			if (generationBestFitness > overallBestFitness){
				overallBestFitness = generationBestFitness;
				overallBestChrom = currentGeneration[fitnesses.IndexOf(generationBestFitness)];
			}
			float generationAverage = average(fitnesses);

			//Debug.Log("generationAverage:" + generationAverage);
			//Debug.Log("generationBestFitness:" + generationBestFitness);
			//Debug.Log("overallBestFitness:" + overallBestFitness);


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
					continue;
				} 
				float chance = Random.Range(0f, 1f);
				if(chance <= mutationProb) {
					mutate (newGeneration[j]);
					mutate (newGeneration[j + 1]);
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
		if(Network.isServer){
			Invoke ("setCheckpoint", 5);
		}
	}
}


