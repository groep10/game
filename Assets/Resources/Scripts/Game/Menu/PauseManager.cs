using UnityEngine;
using System.Collections;

namespace Game.Menu
{
    public class PauseManager : MonoBehaviour {
	    public GameObject currentMenu;
		public GameObject Target;
		public void Update()
	    {
			bool active = Target.activeSelf;
			if (active = true)
		    ShowMenu (currentMenu);
	    }

	    public void ShowMenu(GameObject menu)
	    {
            if (currentMenu != null)
            {
				currentMenu.SetActive (true);
            }
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
