using UnityEngine;
using UnityEngine.UI;
using System.Collections;


namespace Game.Menu
{
    public class ServerListItem : MonoBehaviour
    {

        public Text nameText, ip_portText;
        public Button button;

        private HostData data;

        void Start() {
            button.onClick.AddListener(onClick);
        }

        public void onClick()
        {
            if (data == null)
            {
                return;
            }
			// TODO: find more elegant way to disable menu.
			//GameObject.Find ("MenuObjects").SetActive (false);
			//GameObject.Find ("MainObjects").SetActive (true);

			Network.Connect (data);
            Debug.Log("Clicked " + data.gameName);
        }

        public void setHostData(HostData data) {
            this.data = data;
            nameText.text = data.gameName;
			string tmpIp = "";
			int i = 0;
			while (i < data.ip.Length) {
				tmpIp = data.ip[i] + " ";
				i++;
			}
			ip_portText.text = "Ip: " + tmpIp + "\nPort: " + data.port; 
        }
    }
}