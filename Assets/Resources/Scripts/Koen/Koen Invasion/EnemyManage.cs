using UnityEngine;

public class EnemyManage : MonoBehaviour
{
	public GameObject enemy;              
	public float spawnTime = 3f;            
	public Transform[] spawnPoints;         
	
	
	void Start ()
	{
		// Use Spawn function after a delay of the spawnTime and then continue to call after the same amount of time.
		InvokeRepeating ("Spawn", spawnTime, spawnTime);
	}

	void Update ()
	{
		enemy = GameObject.FindGameObjectWithTag ("Enemy");
	}
	
	void Spawn ()
	{
		// Find a random index between zero and one less than the number of spawn points.
		int spawnPointIndex = Random.Range (0, spawnPoints.Length);
		
		// Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
		Instantiate (enemy, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);
	}
}