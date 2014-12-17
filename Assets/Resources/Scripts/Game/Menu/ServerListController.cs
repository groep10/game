using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Game.Menu {

	public class ServerListController : MonoBehaviour
	{
			
		private string globalName = "MinorProject_Arena_RacingGame";

		public InputField gameName;
		public Button createServer;

		public MenuList list;
		public ServerListItem itemPrefab;


		private HostData[] hostdata;

		// Use this for initialization
		void Start ()
		{
			if (createServer != null) {
				createServer.onClick.AddListener (doCreateServer);
			}
			InvokeRepeating ("requestList", 5f, 5f);
		}

		void doCreateServer() {
			// TODO: find more elegant way to disable menu.
			GameObject.Find ("MenuObjects").SetActive (false);
			//GameObject.Find ("MainObjects").GetComponentInChildren<Game.Net.List>().gameObject.SetActive (true);

			string name = gameName.text;
			int port = Random.Range (20000, 25000);
			Network.InitializeServer(4, port, false);
			MasterServer.RegisterHost(globalName, name, "Arena racing");
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