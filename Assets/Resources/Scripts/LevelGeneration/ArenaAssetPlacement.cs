using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArenaAssetPlacement : MonoBehaviour {

	private List<GameObject> assets;
	//private int amountOfAssets;
	private int assetsInArena = 10;
	private FadeBehaviour fading;


/* -------------------- FUNCTIONS -------------------------*/
	// returns an ArrayList of all GameObjects with tag "Platform"
	List<GameObject> getAssets()
	{
		List<GameObject> result = new List<GameObject>();
		foreach (GameObject go in Resources.LoadAll("Prefabs/Arena/ArenaAssets"))
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

	void fadeOut(FadeBehaviour fading){
		fading.startFadeOut ();
	}

	// places an asset in the arena at a random location and with a random orientation
	void placeAsset()
	{
		// randomise the location within x and z boundaries
		float x = Random.Range (-500, 500);
		float z = Random.Range (-500, 500);
		Vector3 location = new Vector3(x, 50f, z);

		// randomise the asset to be placed
		int assetIndex = Mathf.RoundToInt(Random.Range(0, getAmountOfAssets()));
		GameObject asset = assets[assetIndex];

		fading = asset.GetComponent<FadeBehaviour>();
		// How long before the asset is refreshed
		float assetTimer = Random.Range(1, 15);

		// instantiate the asset
		GameObject currentAsset = (GameObject) Instantiate (asset, location, Quaternion.identity);
		//Destroy (currentAsset, assetTimer);
		//Invoke ("fadeOut", assetTimer);
		fading.queFadeOut (assetTimer);
		Debug.Log ("invoking fade-out");
		Invoke ("placeAsset", assetTimer);
	}

/* ----------------------- Start & Update ------------------------- */

	// Use this for initialization
	void Start () {
		assets = getAssets ();
		//amountOfAssets = getAmountOfAssets ();

		// place several assets in the arena
		for (int i=0; i < assetsInArena; i++){
			placeAsset ();
		}
	}
}
