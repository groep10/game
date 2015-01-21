using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Game.UI {
	public class MiniMap : MonoBehaviour {

		private Transform PlayerCar;
		public GameObject veld;
		public RectTransform veldTr;
		public float size = 3;

		[Header ("Textures")]
		//public Texture field;
		public Texture otherPlayer;
		public Texture player;
		public Texture CheckPoint;
		public Texture Enemy;


		[Header ("Tags")]
		public string otherPlayerTag = "Player";
		public string CheckPointTag = "CheckPoint";
		public string EnemyTag = "Enemy";
		public string RecttangleTag = "Recttangle";


		[Header ("Appearence map")]
		public float mapWorldSize = 100f;
		public float mapSizePercent = 10f;
		public float SizePlayers = 8;
		public float rotationOffset = 0;
		public enum radarLocationValues {topLeft, topRight, bottomLeft, bottomRight}
		public radarLocationValues radarLocation;

		private float mapWidth;

		private Vector2 mapCenter;
		private GameObject Player;

		public float crimp = 0.35f;

		void Start () {
			
		}

		void Update() {
			if (PlayerCar == null) {
				Player = Game.Controller.getInstance().getActivePlayer();
				if (Player != null) {
					PlayerCar = Player.transform;
					GetComponent<Image>().enabled = true;
				}
			} else {
				RemoveBlips ();

				setMapLocation ();

				drawBlip (Player, player, false);
				DrawBlipsForOtherPlayers ();
				DrawBlipsForCheckPoints ();
				DrawBlipsForEnemys ();
			}
		}


		void drawBlip(GameObject go, Texture aTexture, bool check) {

			Vector3 centerPos = PlayerCar.position;
			Vector3 extPos = go.transform.position;
			float dist = Vector3.Distance (centerPos, extPos);

			float scale = mapWidth / mapWorldSize;
			//Als object dichtbij genoeg
			if (dist <= mapWidth*crimp/scale) {
				//Schrijf positie om naar positie op MiniMap
				float dx = centerPos.x - extPos.x;
				float dz = centerPos.z - extPos.z;

				float phi = Mathf.Atan2 (dz, dx) * Mathf.Rad2Deg + PlayerCar.eulerAngles.y + rotationOffset;

				float bX = scale * dist * Mathf.Cos (phi * Mathf.Deg2Rad);
				float bY = scale * dist * Mathf.Sin (phi * Mathf.Deg2Rad);
				
				//Maak nieuw object
				GameObject Blip = new GameObject();
				Blip.tag = "Recttangle";
				RectTransform blip = Blip.AddComponent<RectTransform>();
				blip.SetParent(veldTr, false);

				RawImage img = Blip.AddComponent<RawImage>();
				img.texture = aTexture;

				float height = mapWidth;
				float paddingf = height / 2 - bX;
				float padding2f = height / 2 - bY;
				blip.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, paddingf, SizePlayers);
				blip.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, padding2f, SizePlayers);
			}
		}

		void DrawBlipsForOtherPlayers() {
			GameObject[] gos;
			gos = GameObject.FindGameObjectsWithTag (otherPlayerTag);
			foreach (GameObject go in gos) {
				if (go.networkView.isMine) {
					continue;
				}

				drawBlip(go, otherPlayer, false);

			}

		}

		void DrawBlipsForCheckPoints() {
			GameObject[] gos;
			gos = GameObject.FindGameObjectsWithTag (CheckPointTag);
			foreach (GameObject go in gos) {
				drawBlip(go, CheckPoint, true);

			}


		}

		void DrawBlipsForEnemys () {
			GameObject[] gos;
			gos = GameObject.FindGameObjectsWithTag (EnemyTag);
			foreach (GameObject go in gos) {
				drawBlip(go, Enemy, false);
			}

		}

		void RemoveBlips() {
			GameObject[] rects;
			rects = GameObject.FindGameObjectsWithTag (RecttangleTag);
			foreach (GameObject rect in rects) {
				Destroy(rect);
			}


		}

		void setMapLocation () {
			float curWidth = Screen.currentResolution.width * mapSizePercent / 100.0f;
			if(curWidth == mapWidth) {
				return;
			}
			mapWidth = curWidth;
			float padding = size;

			if (radarLocation == radarLocationValues.topLeft) {
				veldTr.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, padding, mapWidth);
				veldTr.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, padding, mapWidth);
			} else if (radarLocation == radarLocationValues.topRight) {
				veldTr.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, padding, mapWidth);
				veldTr.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, padding + 20, mapWidth);
			} else if (radarLocation == radarLocationValues.bottomLeft) {
				veldTr.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, padding, mapWidth);
				veldTr.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, padding, mapWidth);
			} else if (radarLocation == radarLocationValues.bottomRight) {
				veldTr.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, padding, mapWidth);
				veldTr.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, padding, mapWidth);
			}


		}
	}
}