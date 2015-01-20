
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using Game.Web;

namespace Game.Menu {

	public class LoginController : MonoBehaviour {

		public Button register, login;

		public InputField email, password;

		public GameObject loading, errorWindow, userPanel;

		public Item mainMenu;

		void Start() {
			if (login != null) {
				login.onClick.AddListener(onLoginClicked);
			}
			if (register != null) {
				register.onClick.AddListener(onRegisterClicked);
			}
			if (password != null) {
				password.onEndEdit.AddListener(onPasswordEndEdit);
			}
		}


		void Update() {
			if (Input.GetKeyDown(KeyCode.Tab) &&
					email != null && email.isFocused &&
					password != null) {
				EventSystem.current.SetSelectedGameObject(password.gameObject);
			}
		}

		public void onPasswordEndEdit(String action) {
			if (Input.GetKeyDown(KeyCode.Return)) {
				onLoginClicked();
			}
		}

		public void onAvatarClicked() {

			// TODO: find other method for this.
			// #if UNITY_EDITOR
			//          String path = UnityEditor.EditorUtility.OpenFilePanel(
			//                            "Select new avatar",
			//                            "",
			//                            "*.png;*.jpg;*.gif");

			//          setLoading(true);
			//          AccountController.getInstance().uploadAvatar(path, result => {
			//              setLoading(false);
			//              if (!(bool)result["success"]) {
			//                  GameObject err = (GameObject)Instantiate(errorWindow);
			//                  err.GetComponentInChildren<Text>().text = (String)result["error"];
			//                  err.transform.SetParent(transform, false);

			//                  return;
			//              }
			//              updateUserPanel();
			//          });
			// #endif
			String path = File.open();
			setLoading(true);
			AccountController.getInstance().uploadAvatar(path, result => {
				setLoading(false);
				if (!(bool)result["success"]) {
					GameObject err = (GameObject)Instantiate(errorWindow);
					err.GetComponentInChildren<Text>().text = (String)result["error"];
					err.transform.SetParent(transform, false);

					return;
				}
				updateUserPanel();
			});
		}

		public void onLoginClicked() {
			Debug.Log("login");
			setLoading(true);
			AccountController.getInstance().login(email.text, password.text, this.onResult);
		}

		public void onRegisterClicked() {
			Debug.Log("register");
			setLoading(true);
			AccountController.getInstance().register(email.text, password.text, this.onResult);
		}

		void setLoading(bool isloading) {
			if (loading != null) {
				loading.SetActive(isloading);
			}
		}

		public void onResult(Hashtable result) {
			setLoading(false);
			if (!(bool)result["success"]) {
				GameObject err = (GameObject)Instantiate(errorWindow);
				err.GetComponentInChildren<Text>().text = (String)result["error"];
				err.transform.SetParent(transform, false);
				return;
			}

			GetComponentInParent<Manager>().ShowMenu(mainMenu);

			updateUserPanel();
		}

		public void updateUserPanel() {
			if (userPanel != null) {
				String displayname = (String)AccountController.getInstance().getUser()["displayname"];
				userPanel.GetComponentInChildren<Text>().text = displayname;

				AccountController.getInstance().getUserAvatar(texture => {
					userPanel.GetComponentInChildren<RawImage>().texture = texture;
					userPanel.GetComponentInChildren<RawImage>().enabled = true; // Activate
					Debug.Log("done");
				});
			}
		}

	}


}