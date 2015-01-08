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
        private int winningOverallScore = 3;

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

        // checks if a player has reached the winning overall score
        public void endGameByOverallScore()
        {
            foreach(DictionaryEntry de in overall)
            {
                if((int)de.Value == winningOverallScore)
                {

                }
            }
        }

/* ---------------------------------- GENERAL MINIGAME FUNCTIONS ------------------------- */
        // Ends the minigame and resets the minigamescore hashtable
        public void endMinigame()
        {
            DictionaryEntry highest = new DictionaryEntry("hoi", -1);
            foreach(DictionaryEntry de in minigame)
            {
                if((int)highest.Value == -1 || (int)highest.Value < (int)de.Value) 
                {
                    highest = de;
                }
            }

            // increase the overall score of the best playerin the minigame
            increaseOverallScore((string)highest.Key);

            // empty the minigame hastable
            minigame.Clear();
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

            foreach (DictionaryEntry de in overall) 
            {
                Game.Controller.getInstance().overallScores.addScore(de.Key + ": " + de.Value);
            }
        }

/* ------------------------------------ RACING MINIGAME ----------------------------------- */

/* ------------------------------------ ZOMBIE MINIGAME ----------------------------------- */

        // Increases the zombie score of a player by 1
        public void increasePlayerZombieScore(string playername) 
        {
            if (!minigame.ContainsKey(playername)) 
            {
                minigame[playername] = 0;
            }
            minigame[playername] = (int)minigame[playername] + 1;
            updateZombieScores();
        }

        // Updates the Scores of all players
        public void updateZombieScores() 
        {
            Game.Controller.getInstance().minigameScores.reset();
            Game.Controller.getInstance().minigameScores.addScore("Mode: zombie");

            foreach (DictionaryEntry de in minigame) 
            {
                Game.Controller.getInstance().minigameScores.addScore(de.Key + ": " + de.Value);
            }
        }

/* ------------------------------------ RACE-TO-THE-TOP MINIGAME ----------------------------------- */

        // Sets the race to the top score equal to the current floor the player is at
        public void setPlayerRaceToTheTopScore(string playername, int floor) 
        {
            if (!minigame.ContainsKey(playername)) 
            {
                minigame[playername] = 0;
            }
            minigame[playername] = floor;
            updateRaceToTheTopScores();
        }


        // Updates the Scores of all players
        public void updateRaceToTheTopScores() 
        {
            Game.Controller.getInstance().minigameScores.reset();
            Game.Controller.getInstance().minigameScores.addScore("Mode: Race to the top");
            foreach (DictionaryEntry de in minigame) 
            {
                Game.Controller.getInstance().minigameScores.addScore(de.Key + ": floor " + de.Value);
            }
        }

/* ------------------------------------ TRON MINIGAME ----------------------------------- */

    }
}
