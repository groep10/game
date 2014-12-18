using UnityEngine;
using System.Collections;

public class AIEnemy : MonoBehaviour
{
	float distance, lookAtDistance = 25.0f, attackRange = 15.0f, moveSpeed = 5.0f, damping = 6.0f;
	private float currentDistance = int.MaxValue;
	public GameObject[] Targets;
	private GameObject targ;

	void Start()
	{
		Targets = GameObject.FindGameObjectsWithTag("Player");
	}﻿
	void Update()
	{
		foreach (GameObject go in Targets){
			distance = Vector3.Distance(go.transform.position, transform.position);
			if(distance<currentDistance){
				currentDistance = distance;
				targ = go;
			}
		}
		
		if(distance < lookAtDistance)
		{
			LookAt();
		}
		
		if (distance < attackRange)
		{
			AttackPlayer();
		}
	}
	
	void LookAt()
	{
		Quaternion rotation = Quaternion.LookRotation(targ.transform.position - transform.position);
		transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
	}
	
	void AttackPlayer()
	{
		transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
	}

	void OnCollisionEnter (Collision col)
	{
		if(col.gameObject.name == "bullet(Clone)")
		{
			Destroy(col.gameObject);
		}
	}
}