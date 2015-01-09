using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Level.Race {
	public class GeneticPlacement {

		// Variables
		private int chromosomesPerGeneration = 20; // must be an even number
		private int maxGenerations = 30;
		private float mutationProb = 0.1f;
		private float crossoverProb = 0.7f;

		/* ------------ GENETIC ALGORITHM TO PLACE THE CHECKPOINT -------------------------- */

		// returns a  Vector2 chromosome representing the possible x and y coordinates of the checkpoint
		Vector2 createChromosome() {
			float x = Random.Range (-300, 300);
			float z = Random.Range (-300, 300);
			Vector2 result = new Vector2 (x, z);
			return result;
		}

		// returns a generation of coordian
		List<Vector2> createFirstGeneration() {
			List<Vector2> result = new List<Vector2>();
			for (int i = 0; i < chromosomesPerGeneration; i++) {
				result.Add(createChromosome());
			}
			return result;
		}

		// mutates the chromosome by setting either the x or the z value randomly within a certain range
		void mutate(Vector2 chrom) {
			float range = Random.Range (-100f, 100f);
			float var = Random.Range (0f, 1f);
			if (var < 0.5f) {
				chrom.x = Mathf.Min(Mathf.Max(chrom.x + range, -300), 300);
			} else {
				chrom.y = Mathf.Min(Mathf.Max(chrom.y + range, -300), 300);
			}
		}

		// replace 2 parent chromosomes with 2 child chromosomes using crossover mixing their coordinates
		void crossover(Vector2 chrom1, Vector2 chrom2) {
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
		float calculateFitness(Vector2 chrom, List<Vector2> players) {
			List<float> distances = new List<float>();
			// calculate the distances between the chromosome and the player
			for (int i = 0; i < players.Count; i++) {
				distances.Add(Vector2.Distance (chrom, players[i]));
			}

			float min = Mathf.Min (distances.ToArray());
			float max = Mathf.Max (distances.ToArray());
			float difference = max - min;
			if(difference == 0) {
				return 0;
			}
			float fitness = 1 / difference;
			return fitness;
		}

		// returns a list floats containing all the fitnesses of a generation
		List<float> fitnessOfGeneration(List<Vector2> generation, List<Vector2> players) {
			List<float> result = new List<float>();
			// calculate the fitness for each chromosome in the generation
			foreach (Vector2 chrom in generation) {
				result.Add(calculateFitness(chrom, players));
			}
			return result;
		}

		// calculates the sum of all the elements of an array of floats
		float sum(List<float> array) {
			float sum = 0f;
			foreach (float e in array) {
				sum += e;
			}
			return sum;
		}

		float average(List<float> array) {
			float total = sum (array);
			float average = total / array.Count;
			return average;
		}

		// returns a piechart distribution array of the fitness of a generation
		List<float> fitnessPiechart(List<float> fitnesses) {
			float totalFitness = sum (fitnesses);
			List<float> piechart = new List<float>();

			//piechart.Add((fitnesses[0] / totalFitness) * 100);
			for (int i = 0; i < fitnesses.Count; i++) {
				piechart.Add((fitnesses [i] / totalFitness) * 100);
			}
			return piechart;
		}

		// returns the index of a randomly picked element from the piechart
		int pickFromPiechart(List<float> piechart) {
			float number = Random.Range (0f, 100f);
			for (int i = 0; i < piechart.Count; i++) {
				float value = piechart[i];
				number -= value;
				if (number <= 0) {
					return i;
				}
			}
			return -1;
		}

		List<Vector2> getPlayers() {
			List<Vector2> list = new List<Vector2>();

			foreach(GameObject obj in Game.Controller.getInstance().getPlayers()) {
				list.Add(new Vector2(obj.transform.position.x, obj.transform.position.z));
			}
			return list;
		}

		public Vector2 runGeneticAlgorithm() {
			List<Vector2> players = getPlayers();
			if(players.Count < 2) {
				return new Vector2(0, 0);
			}

			// run the actual GA
			float overallBestFitness = 0f;
			Vector2 overallBestChrom = new Vector2 ();
			List<Vector2> currentGeneration = createFirstGeneration ();

			for (int h = 0; h < maxGenerations; h++) {
				//Debug.Log("Generation:" + h);

				List<float> fitnesses = fitnessOfGeneration (currentGeneration, players);
				List<float> piechart = fitnessPiechart (fitnesses);

				// extract information from the generation
				float generationBestFitness = Mathf.Max(fitnesses.ToArray());
				if (generationBestFitness > overallBestFitness) {
					overallBestFitness = generationBestFitness;
					overallBestChrom = currentGeneration[fitnesses.IndexOf(generationBestFitness)];
				}
				//float generationAverage = average(fitnesses);

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
					if (chance <= mutationProb) {
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
	}
}

