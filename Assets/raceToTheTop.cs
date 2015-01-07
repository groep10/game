using UnityEngine;
using System.Collections;

public class raceToTheTop : MonoBehaviour {

	int numberOfPlanes = 10;
	int planeSpacing = 100;

	public GameObject plane;

	// generates the planes including their connection ramps
	void generatePlanes()
	{
		Vector3 location = new Vector3(0, 0, 0);
		for (int i = 0; i < numberOfPlanes; i++)
		{
			location.y += planeSpacing;
			Instantiate(plane, location, Quaternion.identity);
		}
	}

	// Use this for initialization
	void Start () {
		generatePlanes();
	}
}
