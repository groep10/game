using UnityEngine;
using System.Collections;

public class checkpoint : MonoBehaviour {

	public GameObject level;
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(new Vector3(45, 45, 45) * Time.deltaTime);
	}
	
}
