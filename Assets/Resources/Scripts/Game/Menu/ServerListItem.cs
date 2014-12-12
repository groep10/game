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
            Debug.Log("Clicked " + data.gameName);
        }

        public void setHostData(HostData data) {
            this.data = data;
            nameText.text = data.gameName;
            ip_portText.text = "Ip: " + data.ip + "\nPort: " + data.port; 
        }
    }
}