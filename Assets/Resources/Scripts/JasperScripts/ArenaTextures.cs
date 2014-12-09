using UnityEngine;
using System.Collections;

public class ArenaTextures : MonoBehaviour {

	public Terrain Arena;

	public Texture2D grass;
	public Texture2D cliff;
	public Texture2D rocks;
	public Texture2D dirt;
	private Texture2D tex;
	private Texture2D tex2;

	//******
	//add randomTextures() to the level generation script
	//******


	// assign 2 textures to terrain based on a random number
	void randomTextures(){
		SplatPrototype[] arenaTexture = new SplatPrototype[2];
		arenaTexture [0] = new SplatPrototype ();
		arenaTexture [1] = new SplatPrototype ();
		
		float num = Random.value;
		if(num<0.25){
			tex = rocks;
			tex2 = grass;
		}				
		else if(num>=0.25 && num<0.5){
			tex = cliff;
			tex2 = grass;
		}
		else if(num>=0.5 && num<0.75){
			tex = rocks;	
			tex2= dirt;
		}	
		else {
			tex = cliff;
			tex2 = dirt;
		}
		arenaTexture [0].texture = tex; 
		arenaTexture [1].texture = tex2;
		Arena.terrainData.splatPrototypes = arenaTexture;
	}


	// apply selected textures to terrain based on the slope
	// tex will be onn the wall and tex2 on the ground
	void applyTextures(){
		float[,,] map = new float[Arena.terrainData.alphamapWidth, Arena.terrainData.alphamapHeight, 2];
		
		// For each point on the alphamap...
		for (var y = 0; y < Arena.terrainData.alphamapHeight; y++) {
			for (var x = 0; x < Arena.terrainData.alphamapWidth; x++) {
				// Get the normalized terrain coordinate that
				// corresponds to the the point.
				var normX = x * 1.0 / (Arena.terrainData.alphamapWidth - 1);
				var normY = y * 1.0 / (Arena.terrainData.alphamapHeight - 1);
				
				// Get the steepness value at the normalized coordinate.
				var angle = Arena.terrainData.GetSteepness((float)normX, (float)normY);
				
				// Steepness is given as an angle, 0..90 degrees. Divide
				// by 90 to get an alpha blending value in the range 0..1.
				var frac = angle / 90.0;
				map[x, y, 0] = (float)frac;
				map[x, y, 1] = 1 - (float)frac;
			}
		}
		Arena.terrainData.SetAlphamaps(0, 0, map);
	}
}
