using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using Game.Web;

namespace Game.Menu {
	public class Highscore : MonoBehaviour {
		public MenuList list;
		public GameObject item;

		// Use this for initialization
		void Start () {
			AccountController.getInstance().getScores((json) => {
				list.setItems(new GameObject[0]);
				if(!(bool) json["success"]) {
					return;
				}
				ArrayList items = (ArrayList)json["data"];
				foreach(System.Object obj in items) {
					GameObject go = Instantiate(item) as GameObject;
					go.GetComponentInChildren<Text>().text = (string) obj;
					list.addItem(go);
				}
			});
		}

		// Update is called once per frame
		void Update () {

		}
	}

}