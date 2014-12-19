using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using Game.Web;

namespace Game.Net
{

    public class PlayerInfo : MonoBehaviour {

        [SerializeField]
        private string username;
        [SerializeField]
        private string userid;

        void Awake() {
            username = AccountController.getInstance().getUser()["displayname"] as string;
            userid = AccountController.getInstance().getUser()["id"] as string;
        }

        public string getUserId()
        {
            return userid;
        }

        public String getUsername()
        {
            return username;
        }
    }
}
