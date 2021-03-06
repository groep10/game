using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text.RegularExpressions;

using Game.Web;

namespace Game.Menu {

	public class ServerListController : MonoBehaviour
	{
			
		private string globalName = "MinorProject_Arena_RacingGame";

		public InputField gameName;
		public Button createServer;
		public Slider maxPlayerNumber;

		public MenuList list;
		public ServerListItem itemPrefab;


		private HostData[] hostdata;

        public GameObject errorWindow;

		// Use this for initialization
		void Start ()
		{
			if (createServer != null) {
                createServer.onClick.AddListener(onCreateClick);
				gameName.onEndEdit.AddListener(onCreateEndEdit);
			}
            InvokeRepeating("requestList", 5f, 5f);
		}

		public void onCreateEndEdit(string action) {
			if (Input.GetKeyDown(KeyCode.Return)) {
				onCreateClick();
			}
		}

        void onCreateClick()
        {
			string name = gameName.text;
			int maxNumPlayers = (int) maxPlayerNumber.value;
			Game.Net.Manager manager = GameObject.Find ("NetworkManager").GetComponent<Game.Net.Manager> ();
			manager.setMaxPlayers(maxNumPlayers);

            if (name == null || !Regex.IsMatch(name, @"^[\w .\-!+&]+$", RegexOptions.IgnoreCase))
            {
                GameObject err = (GameObject)Instantiate(errorWindow);
                err.GetComponentInChildren<Text>().text = "Invalid game name";
                err.transform.SetParent(transform, false);
                return;
            }
            doCreateServer();
        }

		void doCreateServer() {
			// TODO: find more elegant way to disable menu.
			GameObject.Find ("MenuObjects").SetActive (false);
			//GameObject.Find ("MainObjects").GetComponentInChildren<Game.Net.List>().gameObject.SetActive (true);

			string name = gameName.text;

			AccountController.getInstance().createGame(name, (res) => {
				int port = Random.Range (20000, 25000);
				Network.InitializeServer(4, port, false);
				MasterServer.RegisterHost(globalName, name, "Arena racing");
			});
		}
	
		void requestList () {
			Debug.Log("requesting list");
			MasterServer.RequestHostList (globalName);
		}

		// Masterserver event 
		void OnMasterServerEvent(MasterServerEvent mse){
			if (mse == MasterServerEvent.HostListReceived){
				Debug.Log("received list");
				hostdata = MasterServer.PollHostList ();
				updateList();
			}
		}

		public void connectionStatus(bool connected, string message){
			if (connected) {
				this.transform.root.gameObject.SetActive(false);
			}
			else{
				GameObject err = (GameObject)Instantiate(errorWindow);
				err.transform.SetParent(transform, false);
				if(message=="full"){
					err.GetComponentInChildren<Text>().text = "Game is full";
				}
				else{
					err.GetComponentInChildren<Text>().text = "Failed to connect";
				}
			}
		}

		void updateList() {
			GameObject[] items = new GameObject[hostdata.Length];
			for (int i = 0; i < hostdata.Length; i += 1) {
				ServerListItem item = Instantiate(itemPrefab) as ServerListItem;
				item.setHostData(hostdata[i]);
				items[i] = item.gameObject;
			}
			list.setItems (items);
		}
	}

}
