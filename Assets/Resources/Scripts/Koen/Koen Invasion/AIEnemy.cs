using UnityEngine;
using System.Collections;

public class AIEnemy : MonoBehaviour
{
	float distance, lookAtDistance = 25.0f, attackRange = 15.0f, moveSpeed = 5.0f, damping = 6.0f;
	public Transform Target;

	void Start()
	{
		if(!Target)
		{
			Target = GameObject.FindGameObjectWithTag("Player").transform;
		}
	}﻿
	void Update()
	{
		distance = Vector3.Distance(Target.position, transform.position);
		
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
		Quaternion rotation = Quaternion.LookRotation(Target.position - transform.position);
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