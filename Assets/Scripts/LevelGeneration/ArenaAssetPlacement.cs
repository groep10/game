using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArenaAssetPlacement : MonoBehaviour {

	private List<GameObject> assets;
	public float assetTimer = 5;

	// returns an ArrayList of all GameObjects with tag "Platform"
	List<GameObject> getAssets()
	{
		List<GameObject> result = new List<GameObject>();
		foreach (GameObject go in Resources.LoadAll("Prefabs/Arena/Arena assets"))
		{
			if(go.tag == "ArenaAsset")
			{
				result.Add(go);
			}
		}
		return result;
	}

	// returns the amounts of assets in the assets ArrayList
	int getAmountOfAssets()
	{
		return assets.Count;
	}

	void placeAsset()
	{
		// randomise the location within x and z boundaries
		float x = Random.Range (-200, 200);
		float z = Random.Range (-200, 200);
		Vector3 location = new Vector3(x, 50f, z);

		// randomise the asset to be placed
		int assetIndex = Mathf.RoundToInt(Random.Range(0, getAmountOfAssets()));
		GameObject asset = assets[assetIndex];

		// instantiate the asset
		GameObject currentAsset = (GameObject) Instantiate (asset, location, Quaternion.identity);
		Destroy (currentAsset, assetTimer);
		Invoke ("placeAsset", assetTimer);
	}


	// Use this for initialization
	void Start () {
		assets = getAssets ();
		placeAsset ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
