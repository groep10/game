using UnityEngine;
using System.Collections;

public class MiniMap2 : MonoBehaviour
{

	//For placing the image of the mini map.
	public GUIStyle miniMap;
	//Two transform variables, one for the player's and the enemy's,
	public Transform player;
	public Transform enemy;
	//Icon images for the player and enemy(s) on the map.
	public GUIStyle playerIcon;
	public GUIStyle enemyIcon;
	//Offset variables (X and Y) - where you want to place your map on screen.
	public float mapOffSetX = 762f;
	public float mapOffSetY = 510;
	//The width and height of your map as it'll appear on screen,
	public float mapWidth = 200f;
	public float mapHeight = 200f;
	//Width and Height of your scene, or the resolution of your terrain.
	public float sceneWidth = 500f;
	public float sceneHeight = 500f;
	//The size of your player's and enemy's icon on the map.
	public float iconSize = 10f;
	private float iconHalfSize;
	
	void Update () { //So that the pivot point of the icon is at the middle of the image.
		//You'll know what it means later...
		iconHalfSize = iconSize/2f;
	} 

	float GetMapPos(float pos, float mapSize, float sceneSize) {
		return pos * mapSize/sceneSize;
	}

	
	void OnGUI() {
		Rect rect1 = new Rect(mapOffSetX,mapOffSetY,mapWidth,mapHeight);
		GUI.BeginGroup(rect1, miniMap);
		float pX = GetMapPos(transform.position.x, mapWidth, sceneWidth);
		float pZ = GetMapPos(transform.position.z, mapHeight, sceneHeight);
		float playerMapX = pX - iconHalfSize;
		float playerMapZ = ((pZ * - 1) - iconHalfSize) + mapHeight;
		Rect rect2 = new Rect (playerMapX, playerMapZ, iconSize, iconSize);
		GUI.Box(rect2, "MiniMap", playerIcon);
		GUI.EndGroup();
	}

}

