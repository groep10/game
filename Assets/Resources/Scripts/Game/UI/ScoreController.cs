using System;
using System.Collections;
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

/* ============================================== Variables ==================================== */

        public GameObject prefab;

        List<GameObject> scores = new List<GameObject>();

        private Hashtable table = new Hashtable();

/* ============================================== FUNCTIONS ===================================== */

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

/* ------------------------------------ OVERALL SCORES  ----------------------------------- */

/* ------------------------------------ ZOMBIE MINIGAME ----------------------------------- */

        public void increasePlayerZombieScore(string playername) {
            if (!table.ContainsKey(playername)) {
                table[playername] = 0;
            }
            table[playername] = (int)table[playername] + 1;
            updateZombieScores();
        }

        public void updateZombieScores() {
            Game.Controller.getInstance().minigameScores.reset();
            Game.Controller.getInstance().minigameScores.addScore("Mode: zombie");

            foreach (DictionaryEntry de in table) {
                Game.Controller.getInstance().minigameScores.addScore(de.Key + ": " + de.Value);
            }
        }

/* ------------------------------------ RACE-TO-THE-TOP MINIGAME ----------------------------------- */

/* ------------------------------------ TRON MINIGAME ----------------------------------- */

    }
}
