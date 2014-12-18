using UnityEngine;

public class EnemyManage : MonoBehaviour
{
	public GameObject enemy;              
	public float spawnTime = 3f;            
	private Vector3 spawnPoint;         
	
	
	void Start ()
	{
		// Use Spawn function after a delay of the spawnTime and then continue to call after the same amount of time.
		InvokeRepeating ("Spawn", spawnTime, spawnTime);
	}
	
	void Spawn ()
	{
		// Find a random index between zero and one less than the number of spawn points.
		int spawnX = Random.Range (-150, 150);
		int spawnZ = Random.Range (-150, 150);
		spawnPoint = new Vector3 (spawnX, 3f, spawnZ);
		// Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
		Network.Instantiate (enemy, spawnPoint, Quaternion.identity,0);
	}
}