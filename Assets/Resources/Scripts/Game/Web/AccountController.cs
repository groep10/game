﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game.Web
{
    class AccountController
    {

        private static AccountController instance;
        public static AccountController getInstance()
        {
            if (instance == null)
            {
                instance = new AccountController();
            }
            return instance;
        }

        private String curUsername, curPassword;

        private Hashtable user;

        private Boolean loggedIn;
        private String accessToken;

        public delegate void handleHash(Hashtable result);
        public delegate void handleData(byte[] data);
        public delegate void handleTexture(Texture2D tex);
        public delegate void handleSprite(Sprite sprite);

        public void register(String username, String password, handleHash callback)
        {
            curUsername = username;
            curPassword = password;

            WWWForm form = new WWWForm();
            form.AddField("username", username);
            form.AddField("password", password);

            Http request = new Http("http://so.meaglin.com/api.php?action=register", form);
            request.getText((responseText) =>
            {
                Debug.Log(responseText);
                Hashtable json = (Hashtable)JSON.JsonDecode(responseText);
                this.afterRegister(json, callback);
            });
        }

        public void afterRegister(Hashtable json, handleHash callback)
        {
            if (!json.ContainsKey("success") || !(bool)json["success"])
            {
                json["success"] = false;
                if (!json.ContainsKey("error"))
                {
                    json["error"] = "Server Error";
                }
                Debug.Log("[register]fail " + json["error"]);
                callback(json);
                return;
            }
            Debug.Log("[register]After");
            login(curUsername, curPassword, callback);
        }

        public void login(String username, String password, handleHash callback)
        {
            curUsername = username;
            curPassword = password;

            WWWForm form = new WWWForm();
            form.AddField("username", username);
            form.AddField("password", password);

            Http request = new Http("http://so.meaglin.com/api.php?action=login", form);
            request.getText((responseText) =>
            {
                Debug.Log(responseText);
                Hashtable json = (Hashtable)JSON.JsonDecode(responseText);
                this.afterLogin(json, callback);
            });
        }

        public void afterLogin(Hashtable json, handleHash callback)
        {
            if (!json.ContainsKey("success") || !(bool)json["success"])
            {
                json["success"] = false;
                if (!json.ContainsKey("error"))
                {
                    json["error"] = "Server Error";
                }
                Debug.Log("[login]fail " + json["error"]);
                callback(json);
                return;
            }
            Hashtable data = (Hashtable)json["data"];
            user = (Hashtable)data["user"];
            accessToken = (String)data["token"];

            curUsername = null;
            curPassword = null;

            Debug.Log("[login]After");
            callback(json);
        }

        public void getUserAvatar(handleTexture callback)
        {
            WWWForm form = new WWWForm();
            form.AddField("token", this.getAccessToken());

            Http request = new Http("http://so.meaglin.com/api.php?action=getavatar&raw", form);
            request.getData((www) =>
            {
                callback(www.texture);
            });
        }

        public void uploadAvatar(String filePath, handleHash callback)
        {
            Http req = new Http("file:///" + filePath);
            req.getData(www =>
            {
                WWWForm form = new WWWForm();
                form.AddField("token", this.getAccessToken());
                form.AddBinaryData("avatar", www.bytes);

                Http setreq = new Http("http://so.meaglin.com/api.php?action=setavatar", form);
                setreq.getText(responseText =>
                {
                    Debug.Log(responseText);
                    Hashtable json = (Hashtable)JSON.JsonDecode(responseText);
                    if (!json.ContainsKey("success") || !(bool)json["success"])
                    {
                        json["success"] = false;
                        if (!json.ContainsKey("error"))
                        {
                            json["error"] = "Server Error";
                        }
                        Debug.Log("[avatar]fail " + json["error"]);
                    }
                    callback(json);
                });
                //  Debug.Log("data " + www.bytes.Length);
            });
        }

        public String getAccessToken()
        {
            return accessToken;
        }

        public Hashtable getUser()
        {
            return user;
        }
    }

}