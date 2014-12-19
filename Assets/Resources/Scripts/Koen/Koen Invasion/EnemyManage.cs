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
		if(GameObject.FindGameObjectsWithTag ("Enemy").Length < 30){
			// Find a random index between zero and one less than the number of spawn points.
			int spawnX = Random.Range (-500, 500);
			int spawnZ = Random.Range (-500, 500);
			spawnPoint = new Vector3 (spawnX, 3f, spawnZ);
			// Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
			Network.Instantiate (enemy, spawnPoint, Quaternion.identity,0);
		}
	}

}