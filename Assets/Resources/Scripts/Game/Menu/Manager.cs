using UnityEngine;
using System.Collections;

namespace Game.Menu
{
    public class Manager : MonoBehaviour {
	    public Item currentMenu;

	    public void Start()
	    {
		    ShowMenu (currentMenu);
	    }

	    public void ShowMenu(Item menu)
	    {
            if (currentMenu != null)
            {
			    currentMenu.IsOpen = false;
            }
		    currentMenu = menu;
		    currentMenu.IsOpen = true;
	    }

        public void Quit()
        {
            Application.Quit();
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
    }
}
