using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Game.Level {

	public class ArenaAssetPlacement : MonoBehaviour {

		private List<GameObject> assets;
		private int assetsInArena = 10;

		    // returns an ArrayList of all GameObjects with tag "ArenaAsset"
		void loadAssets()
		{
	        assets = new List<GameObject>();
			foreach (Object o in Resources.LoadAll("Prefabs/Game/Arena/ArenaAssets"))
			{
				if (!(o is GameObject)) {
					continue;
				}
				GameObject go = (GameObject) o;
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
			Vector3 location = new Vector3(x, 0f, z);

			float rotationY = Random.Range (0, 360);

			// randomise the asset to be placed
			int assetIndex = Random.Range(0, getAmountOfAssets());
			GameObject asset = assets[assetIndex];

			// instantiate the asset
			GameObject currentAsset = (GameObject) Network.Instantiate (asset, location, Quaternion.Euler(0f, rotationY, 0f),0);
			currentAsset.GetComponent<FadeBehaviour> ().setParent (this);
			//Debug.Log("Object created");
		}

		// Called at the start of the game
		void Start()
		{
			if(Network.isServer){
				loadAssets();
				for (int i = 0; i < assetsInArena; i++)
				{
					placeAsset();
				}
			}
		}

	}
}