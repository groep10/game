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
            if (networkView.isMine)
            {
                username = AccountController.getInstance().getUser()["displayname"] as string;
                userid = AccountController.getInstance().getUser()["id"] as string;
                networkView.RPC("setInformation", RPCMode.AllBuffered, username, userid);
            }
        }

        [RPC]
        public void setInformation(String username, String userid)
        {
            this.username = username;
            this.userid = userid;
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
