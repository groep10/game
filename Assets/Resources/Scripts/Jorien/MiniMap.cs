using UnityEngine;
using System.Collections;

public class MiniMap : MonoBehaviour
{

	public Transform PlayerCar;

	[Header ("Textures")]
	public Texture field;
	public Texture otherPlayer;
	public Texture player;
	public Texture CheckPoint;
	public Texture Enemy;
	public Texture CheckPointU;
	public Texture CheckPointD;
	public Texture CheckPointL;
	public Texture CheckPointR;


	[Header ("Tags")]
	public string otherPlayerTag = "OtherPlayer";
	public string CheckPointTag = "CheckPoint";
	public string EnemyTag = "Enemy";


	[Header ("Appearence map")]
	public float mapScale = 0.3f;
	public float mapSizePercent = 15f;
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
		drawBlip (Player, player, false);
		DrawBlipsForOtherPlayers ();
		DrawBlipsForCheckPoints ();
		DrawBlipsForEnemys ();
	}
	
	void drawBlip(GameObject go,Texture aTexture, bool check){
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
		else if (check) {
			float bX2 = mapWidth*.45f * Mathf.Cos (deltay * Mathf.Deg2Rad);
			float bY2 = mapWidth*.45f * Mathf.Sin (deltay * Mathf.Deg2Rad);
			//als tussen -0.25pi en 0.25 pi dan right
			//als tussen 0.25pi en 0.75pi dan up
			//als tussen 0.75pi en -0.75pi dan left
			//als tussen -0.75pi en -0.25pi dan down
			if(Mathf.Cos (deltay * Mathf.Deg2Rad)>0.71){
				GUI.DrawTexture(new Rect(mapCenter.x+bX2,mapCenter.y+bY2,SizePlayers,SizePlayers),CheckPointR);
			}else if(Mathf.Cos (deltay * Mathf.Deg2Rad)<-0.71){
				GUI.DrawTexture(new Rect(mapCenter.x+bX2,mapCenter.y+bY2,SizePlayers,SizePlayers),CheckPointL);
			}else if(Mathf.Sin (deltay * Mathf.Deg2Rad)>0.71){
				GUI.DrawTexture(new Rect(mapCenter.x+bX2,mapCenter.y+bY2,SizePlayers,SizePlayers),CheckPointD);
			}else{
				GUI.DrawTexture(new Rect(mapCenter.x+bX2,mapCenter.y+bY2,SizePlayers,SizePlayers),CheckPointU);
			}
		}

		
	}
	
	void DrawBlipsForOtherPlayers(){
		GameObject[] gos;
		gos = GameObject.FindGameObjectsWithTag (otherPlayerTag);		
		foreach (GameObject go in gos) {
			drawBlip(go,otherPlayer, false);
		}
		
	}

	void DrawBlipsForCheckPoints(){ 
		GameObject[] gos;
		gos = GameObject.FindGameObjectsWithTag (CheckPointTag);
		foreach (GameObject go in gos) {
			drawBlip(go,CheckPoint, true);
		}


	}

	void DrawBlipsForEnemys (){
		GameObject[] gos;
		gos = GameObject.FindGameObjectsWithTag (EnemyTag);
		foreach (GameObject go in gos) {
			drawBlip(go,Enemy, false);
		}

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