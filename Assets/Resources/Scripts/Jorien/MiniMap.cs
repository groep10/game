using UnityEngine;
using System.Collections;

public class MiniMap : MonoBehaviour
{
	public Texture field;
	public Texture otherPlayer;
	public Texture player;
	public Texture CheckPoint;
	
	public Transform PlayerCar;
	public float mapScale = 0.3f;
	public float mapSizePercent = 15f;
	
	public string otherPlayerTag = "OtherPlayer";
	public string CheckPointTag = "CheckPoint";

	public float SizePlayers = 8;
	
	public enum radarLocationValues {topLeft, topRight, middleLeft, middleRight, bottomLeft, bottomRight}
	public radarLocationValues radarLocation; 
	
	private float mapWidth;
	private float mapHeight;
	private Vector2 mapCenter;
	private GameObject Player;

	
	void Start () {
		setMapLocation ();	
		Player = transform.gameObject;
	}
	
	void OnGUI () {
		GUI.DrawTexture (new Rect (mapCenter.x - mapWidth / 2, mapCenter.y - mapHeight / 2, mapWidth, mapHeight), field);
		drawBlip (Player, player);
		DrawBlipsForOtherPlayers ();
		DrawBlipsForCheckPoints ();
	}
	
	void drawBlip(GameObject go,Texture aTexture){
		Vector3 centerPos = PlayerCar.position;
		Vector3 extPos = go.transform.position;

		float dist = Vector3.Distance (centerPos, extPos);
		float dx = centerPos.x - extPos.x;
		float dz = centerPos.z - extPos.z; 
		
		float deltay = Mathf.Atan2 (dx, dz) * Mathf.Rad2Deg - 270 - PlayerCar.eulerAngles.y;

		float bX = dist * Mathf.Cos (deltay * Mathf.Deg2Rad);
		float bY = dist * Mathf.Sin (deltay * Mathf.Deg2Rad);
		
		bX = bX * mapScale; 
		bY = bY * mapScale;
		
		if(dist<=mapWidth*.5/mapScale){ 
			GUI.DrawTexture(new Rect(mapCenter.x+bX,mapCenter.y+bY,SizePlayers,SizePlayers),aTexture);
		}
		/*if (dist > mapWidth * .5 / mapScale && Texture.Equals(aTexture, CheckPointTag)==0) {
			print ( "CheckPoint");
		}*/

		
	}
	
	void DrawBlipsForOtherPlayers(){
		GameObject[] gos;
		gos = GameObject.FindGameObjectsWithTag (otherPlayerTag);		
		foreach (GameObject go in gos) {
			drawBlip(go,otherPlayer);
		}
		
	}

	void DrawBlipsForCheckPoints(){ 
		GameObject[] gos;
		gos = GameObject.FindGameObjectsWithTag (CheckPointTag);
		Vector3 position = transform.position;


	}
	
	void setMapLocation () {
		mapWidth = Screen.width * mapSizePercent / 100.0f;
		mapHeight = mapWidth;
		
		if(radarLocation == radarLocationValues.topLeft){
			mapCenter = new Vector2(mapWidth/2, mapHeight/2);
		} else if(radarLocation == radarLocationValues.topRight){
			mapCenter = new Vector2(Screen.width-mapWidth/2, mapHeight/2);
		} else if(radarLocation == radarLocationValues.middleLeft){
			mapCenter = new Vector2(mapWidth/2, Screen.height/2);
		} else if(radarLocation == radarLocationValues.middleRight){
			mapCenter = new Vector2(Screen.width-mapWidth/2, Screen.height/2);
		} else if(radarLocation == radarLocationValues.bottomLeft){
			mapCenter = new Vector2(mapWidth/2, Screen.height - mapHeight/2);
		} else if(radarLocation == radarLocationValues.bottomRight){
			mapCenter = new Vector2(Screen.width-mapWidth/2, Screen.height - mapHeight/2);
		} 
		
	}
}