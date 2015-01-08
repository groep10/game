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

        private Hashtable minigame = new Hashtable();
        private Hashtable overall = new Hashtable();

/* ============================================== FUNCTIONS ===================================== */

/* -------------------------------------- GENERAL FUNCTIONS ------------------------------ */
        // Adds a score entry to the menulist of the ScoreController
        public void addScore(String text)
        {
            GameObject obj = Instantiate(prefab) as GameObject;
            obj.GetComponent<Text>().text = text;
            scores.Add(obj); // Is this needed?

            GetComponent<MenuList>().addItem(obj);
        }

        // resets the ScoreController
        public void reset()
        {
            scores.Clear();
            GetComponent<MenuList>().setItems(new GameObject[0]);
        }

/* ------------------------------------ OVERALL SCORES  ----------------------------------- */

// Increases the overall score of a player by 1
    public void increaseOverallScore(string playername) 
    {
        if (!overall.ContainsKey(playername)) 
        {
            overall[playername] = 0;
        }
        overall[playername] = (int)overall[playername] + 1;
        updateOverallScores();
    }

    // Updates the Overall Scores of all players
    public void updateOverallScores() 
    {
        Game.Controller.getInstance().overallScores.reset();
        Game.Controller.getInstance().overallScores.addScore("Mode: Overall");

        foreach (DictionaryEntry de in minigame) 
        {
            Game.Controller.getInstance().overallScores.addScore(de.Key + ": " + de.Value);
        }
    }



/* ------------------------------------ ZOMBIE MINIGAME ----------------------------------- */

        // Increases the score of a player by 1
        public void increasePlayerZombieScore(string playername) 
        {
            if (!minigame.ContainsKey(playername)) 
            {
                minigame[playername] = 0;
            }
            minigame[playername] = (int)minigame[playername] + 1;
            updateZombieScores();
        }

        // Updates the zombieScores of all players
        public void updateZombieScores() 
        {
            Game.Controller.getInstance().minigameScores.reset();
            Game.Controller.getInstance().minigameScores.addScore("Mode: zombie");

            foreach (DictionaryEntry de in minigame) 
            {
                Game.Controller.getInstance().minigameScores.addScore(de.Key + ": " + de.Value);
            }
        }

        // Ends the zombie minigame and resets the minigamescore hashtable
        public void endZombieGame()
        {
            DictionaryEntry highest = new DictionaryEntry("hoi", -1);
            foreach(DictionaryEntry de in minigame)
            {
                if((int)highest.Value == -1 || (int)highest.Value < (int)de.Value) {
                    highest = de;
                }
            }

            // empty the minigame hastable
            minigame.Clear();
        }

/* ------------------------------------ RACE-TO-THE-TOP MINIGAME ----------------------------------- */

/* ------------------------------------ TRON MINIGAME ----------------------------------- */

    }
}
