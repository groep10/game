using UnityEngine;
using System.Collections;

public class Kill : MonoBehaviour
{
	void OnCollisionEnter (Collision col)
	{
		if(col.gameObject.tag == "Enemy")
		{
			Destroy(col.gameObject);
		}
	}
}