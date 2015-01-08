using UnityEngine;
using System.Collections;

using Game.Net;

public class AIEnemy : MonoBehaviour
{
	float distance, lookAtDistance = 150f, attackRange = 100f, moveSpeed = 20.0f, damping = 6.0f;
	private float currentDistance;
	public GameObject[] Targets;
	private GameObject targ;
	private int health = 100;
    private bool dead = false;

	void Start(){
		Targets = GameObject.FindGameObjectsWithTag("Player");
	}﻿
	void Update()
	{
		if(health <= 0 && Network.isServer){
			Network.Destroy(this.gameObject);
			Network.RemoveRPCs(networkView.viewID);
            return;
		}
		currentDistance = int.MaxValue;
		foreach (GameObject go in Targets){
			distance = Vector3.Distance(go.transform.position, transform.position);
			if(distance<currentDistance){
				currentDistance = distance;
				targ = go;
			}
		}
		if(distance < lookAtDistance){
			LookAt();
		}
		if (distance < attackRange){
			AttackPlayer();
		}
	}
	
	void LookAt(){
		Quaternion rotation = Quaternion.LookRotation(targ.transform.position - transform.position);
		Vector3 eulerRot = new Vector3 (0, rotation.eulerAngles.y, 0);
		transform.rotation = Quaternion.Euler (eulerRot);
			//(transform.rotation.eulerAngles, eulerRot, Time.deltaTime * damping);
	}
	
	void AttackPlayer(){
		transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
	}

	//RPC calls
	[RPC]
	void damage(int dam, NetworkViewID shooter){
		health -= dam;
		Debug.Log("health: " + health);
		if (health <= 0 && Network.isServer && !dead) {
            dead = true;
			Debug.Log("killed by: " + shooter);
            //minigamePoint(NetworkView.Find(shooter).gameObject.GetComponent<PlayerInfo>().getUsername());
            networkView.RPC("minigamePoint", RPCMode.AllBuffered, NetworkView.Find(shooter).gameObject.GetComponent<PlayerInfo>().getUsername());
		}
	}

    [RPC]
    void minigamePoint(string playername)
    {
        // GameObject.FindObjectOfType<Game.Level.Manager>().increasePlayerMinigameScore(playername);
    }
}