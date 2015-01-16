using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace Game.Web {
	class Http {

		private static Empty ServerObject {
			get {
				if (_serverObject == null) {
					var gameObj = new GameObject("Server Object");
					_serverObject = gameObj.AddComponent<Empty>();
					//GameObject.DontDestroyOnLoad(_serverObject); // Optional
				}
				return _serverObject;
			}
		}
		private static Empty _serverObject;

		private String url;
		private WWWForm form;

		public Http(String url) {
			this.url = url;
		}

		public Http(String url, WWWForm form) {
			this.url = url;
			this.form = form;
		}

		public delegate void handleHash(Hashtable result);
		public delegate void handleString(String result);
		public delegate void handleWWW(WWW www);

		public void getJson(handleHash callback) {
			getText((text) => {
				Hashtable json = (Hashtable)JSON.JsonDecode(text);
				if (!json.ContainsKey("success") || !(bool)json["success"]) {
					json["success"] = false;
					if (!json.ContainsKey("error")) {
						json["error"] = "Server Error";
					}
				}
                callback(json);
			});
		}

		public void getText(handleString callback) {
			getData((www) => {
				callback(www.text);
			});
		}

		public void getData(handleWWW callback) {
			ServerObject.StartCoroutine(
				doRequest(callback)
			);
		}

		IEnumerator doRequest(handleWWW callback) {
			WWW www;
			if (form != null) {
				www = new WWW(url, form);
			} else {
				www = new WWW(url);
			}
			yield return www;

			callback(www);
		}
	}

}