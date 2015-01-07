using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Game;
public class MiniMap3 : MonoBehaviour
{

	public Transform PlayerCar;
	public GameObject veld;
	public Transform veldTr;
    public float size = 138;

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
	public float mapScale = 0.4f;
	public float mapSizePercent = 30f;
	public float SizePlayers = 8;
	public enum radarLocationValues {topLeft, topRight, bottomLeft, bottomRight}
	public radarLocationValues radarLocation; 
	private float mapWidth;
	private float mapHeight;
	private Vector2 mapCenter;
	private GameObject Player;

	
	void Start () {
        //Player = PlayerCar.gameObject;
		mapWidth = Screen.width * mapSizePercent / 100.0f;
		mapHeight = mapWidth;
	
	}
	
	void Update() {
        if (PlayerCar == null)
        {
			CarController p = GameObject.FindObjectOfType<CarController>();
            if (p == null)
            {
                return;
            }
            PlayerCar = p.transform;
            Player = p.gameObject;
            GetComponent<Image>().enabled = true;
        }
		RemoveBlips ();
		//Maak map aan
		setMapLocation ();

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

		float deltay = Mathf.Atan2 (dx, dz) * Mathf.Rad2Deg - PlayerCar.eulerAngles.y;

		float bX = dx * mapScale;
		float bY = dz * mapScale;

		if(dist<=mapWidth*0.50/mapScale){ 
			RectTransform temp = veld.GetComponent<RectTransform> ();
			GameObject Blip = new GameObject();
			Blip.tag = "Recttangle";
			RawImage img = Blip.AddComponent<RawImage>();
			img.texture = aTexture; 
			RectTransform blip = Blip.GetComponent<RectTransform>();
			blip.SetParent(veldTr);
			Vector2 middel = temp.localPosition; 
			float height = temp.rect.height; 
			float paddingf = height/2 + bX;
			float padding2f = height/2 + bY;
			int padding = (int) paddingf;
			int padding2 = (int) padding2f;
			blip.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, padding, SizePlayers);
			blip.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, padding2, SizePlayers);

		}
		/*else if (check) {
			RectTransform temp = veld.GetComponent<RectTransform> ();
			GameObject Blip = new GameObject();
			Blip.tag = "Recttangle";
			RawImage img = Blip.AddComponent<RawImage>();
			RectTransform blip = Blip.GetComponent<RectTransform>();
			blip.SetParent(veldTr);
			float bX2 = mapWidth*.5f * Mathf.Cos (deltay * Mathf.Deg2Rad);
			float bY2 = mapWidth*.5f * Mathf.Sin (deltay * Mathf.Deg2Rad);
			int padding = (int) bX2;
			int padding2 = (int) bY2;

			//als tussen -0.25pi en 0.25 pi dan right
			//als tussen 0.25pi en 0.75pi dan up
			//als tussen 0.75pi en -0.75pi dan left
			//als tussen -0.75pi en -0.25pi dan down
			int padding3 = (int) bX2;
			int padding4 = (int) bY2;
			if(Mathf.Cos (deltay * Mathf.Deg2Rad)>0.71){
				img.texture = CheckPointR; 
				blip.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, padding3, SizePlayers);
				blip.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, padding4, SizePlayers);
			}else if(Mathf.Cos (deltay * Mathf.Deg2Rad)<-0.71){
				img.texture = CheckPointL; 
				blip.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, padding3, SizePlayers);
				blip.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, padding4, SizePlayers);
			}else if(Mathf.Sin (deltay * Mathf.Deg2Rad)>0.71){
				img.texture = CheckPointD; 
				blip.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, padding3, SizePlayers);
				blip.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, padding4, SizePlayers);
			}else{
				img.texture = CheckPointU; 
				blip.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, padding3, SizePlayers);
				blip.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, padding4, SizePlayers);
			}
		}*/
		
	}
	
	void DrawBlipsForOtherPlayers(){
		GameObject[] gos;
		gos = GameObject.FindGameObjectsWithTag (otherPlayerTag);		
		foreach (GameObject go in gos) {
            if (go == Player)
            {
                continue;
            }
			drawBlip(go, otherPlayer, false);
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
			Destroy(rect);
		}


		}
	
	void setMapLocation () {
		RectTransform temp = veld.GetComponent<RectTransform> ();
		int padding = 5;

		if (radarLocation == radarLocationValues.topLeft) {
			temp.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, padding, mapWidth);
			temp.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, padding, mapWidth);
		} else if(radarLocation == radarLocationValues.topRight){
			temp.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, padding, mapWidth);
			temp.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, padding, mapWidth);
		} else if(radarLocation == radarLocationValues.bottomLeft){
			temp.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, padding, mapWidth);
			temp.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, padding, mapWidth);
		} else if(radarLocation == radarLocationValues.bottomRight){
			temp.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, padding, mapWidth);
			temp.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, padding, mapWidth);
		} 


	}
}