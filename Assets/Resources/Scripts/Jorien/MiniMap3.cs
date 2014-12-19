using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MiniMap3 : MonoBehaviour
{

	public Transform PlayerCar;
	public GameObject veld;
	public Transform veldTr;
	public float size;

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
	public string RecttangleTag = "Recttangle";


	[Header ("Appearence map")]
	public float mapScale = 0.3f;
	public float mapSizePercent = 15f;
	public float SizePlayers = 8;
	public enum radarLocationValues {topLeft, topRight, bottomLeft, bottomRight}
	public radarLocationValues radarLocation; 
	private float mapWidth;
	private float mapHeight;
	private Vector2 mapCenter;
	private GameObject Player;

	
	void Start () {
		Player = PlayerCar.gameObject;
		mapWidth = Screen.width * mapSizePercent / 100.0f;
		mapHeight = mapWidth;
	
	}
	
	void Update() {
		//Maak map aan
		setMapLocation ();

		drawBlip (Player, player, false);
		DrawBlipsForOtherPlayers ();
		DrawBlipsForCheckPoints ();
		DrawBlipsForEnemys ();
		RemoveBlips ();
	}
	
	void drawBlip(GameObject go,Texture aTexture, bool check){
		Vector3 centerPos = PlayerCar.position;
		Vector3 extPos = go.transform.position;

		float dist = Vector3.Distance (centerPos, extPos);
		float dx = centerPos.x - extPos.x;
		float dz = centerPos.z - extPos.z; 

		//float deltay = Mathf.Atan2 (dx, dz) * Mathf.Rad2Deg - 270 - PlayerCar.eulerAngles.y;

		float bX = dx * mapScale;
		float bY = dz * mapScale;

		if(dist<=mapWidth*.5/mapScale){ 
			RectTransform temp = veld.GetComponent<RectTransform> ();
			GameObject Blip = new GameObject();
			Blip.tag = "Recttangle";
			RawImage img = Blip.AddComponent<RawImage>();
			img.texture = aTexture; 
			RectTransform blip = Blip.GetComponent<RectTransform>();
			blip.SetParent(temp);
			Vector2 middel = temp.localPosition; 
			float height = temp.rect.height; 
			float paddingf = height/2 + bX;
			float padding2f = height/2 + bY;
			int padding = (int) paddingf;
			int padding2 = (int) padding2f;
			blip.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, padding, SizePlayers);
			blip.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, padding2, SizePlayers);

		}/*
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
		}*/
		
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

	void RemoveBlips(){
		GameObject[] rects;
		rects = GameObject.FindGameObjectsWithTag (RecttangleTag);
		foreach (GameObject rect in rects) {
			//rect.Destroy();
		}


		}
	
	void setMapLocation () {
		RectTransform temp = veld.GetComponent<RectTransform> ();
		float height = temp.rect.height; 

		int padding = 5;

		if (radarLocation == radarLocationValues.topLeft) {
			temp.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, padding, height);
			temp.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, padding, height);
		} else if(radarLocation == radarLocationValues.topRight){
			temp.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, padding, height);
			temp.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, padding, height);
		} else if(radarLocation == radarLocationValues.bottomLeft){
			temp.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, padding, height);
			temp.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, padding, height);
		} else if(radarLocation == radarLocationValues.bottomRight){
			temp.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, padding, height);
			temp.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, padding, height);
		} 


	}
}