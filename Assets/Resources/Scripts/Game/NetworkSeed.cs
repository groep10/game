using UnityEngine;
using System.Collections;

public class NetworkSeed : MonoBehaviour {

	public int syncSeed;
	// Use this for initialization
	void Start () {
		if (Network.isServer) {
			int seed = Random.Range (0,100);
			networkView.RPC("setSeed",RPCMode.AllBuffered,seed);
		}
	}

	[RPC]
	void setSeed(int seed){
		syncSeed = seed;
		transform.GetComponentInChildren<Fence_placement> ().onStart (syncSeed);
	}


}
