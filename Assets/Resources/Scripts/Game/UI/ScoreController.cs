using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;
using Game.Web;
using Game.Menu;

namespace Game.UI
{
    public class ScoreController : MonoBehaviour {

        public GameObject prefab;

        List<GameObject> scores = new List<GameObject>();

        public void addScore(String text)
        {
            GameObject obj = Instantiate(prefab) as GameObject;
            obj.GetComponent<Text>().text = text;
            scores.Add(obj); // Is this needed?

            GetComponent<MenuList>().addItem(obj);
        }

        public void reset()
        {
            scores.Clear();
            GetComponent<MenuList>().setItems(new GameObject[0]);
        }

    }
}
