using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArenaAssetPlacement : MonoBehaviour {

	private List<GameObject> assets;
	//public float assetTimer = 5;
	private int assetsInArena = 10;


    // returns an ArrayList of all GameObjects with tag "Platform"
	void loadAssets()
	{
        assets = new List<GameObject>();
		foreach (GameObject go in Resources.LoadAll("Prefabs/Arena/ArenaAssets"))
		{
			if(go.tag == "ArenaAsset")
			{
                assets.Add(go);
			}
		}
	}

	// returns the amounts of assets in the assets ArrayList
	int getAmountOfAssets()
	{
		return assets.Count;
	}

	public void placeAsset()
	{
		// randomise the location within x and z boundaries
		float x = Random.Range (-500, 500);
		float z = Random.Range (-500, 500);
		Vector3 location = new Vector3(x, 50f, z);

		// randomise the asset to be placed
		int assetIndex = Mathf.RoundToInt(Random.Range(0, getAmountOfAssets()));
		GameObject asset = assets[assetIndex];

		// The time it takes before the asset is refreshed
		//float assetTimer = Random.Range(1, 15);

		// instantiate the asset
		GameObject currentAsset = (GameObject) Instantiate (asset, location, Quaternion.identity);
		currentAsset.GetComponent<FadeBehaviour> ().setParent (this);
		//Debug.Log("Object created");
	}

	// Called at the start of the game
	void Start()
	{
		loadAssets();
		for (int i = 0; i < assetsInArena; i++)
		{
			placeAsset();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
