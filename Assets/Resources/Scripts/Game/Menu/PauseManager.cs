using UnityEngine;
using System.Collections;

namespace Game.Menu
{
    public class PauseManager : MonoBehaviour {
	    public Item currentMenu;
		public GameObject Target;
		public void Update()
	    {
			bool active = Target.activeSelf;
			if (active = true)
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
