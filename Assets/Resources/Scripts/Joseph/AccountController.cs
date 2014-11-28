using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


class AccountController {

    private static AccountController instance;
    public static AccountController getInstance() {
        if (instance == null)
        {
            instance = new AccountController();
        }
        return instance;
    }

    private String curUsername, curPassword;

    private Boolean loggedIn;
    private String accessToken;

    public void register(String username, String password) {
        curUsername = username;
        curPassword = password;

        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        HTTP.Request someRequest = new HTTP.Request("post", "http://so.meaglin.com/api.php?action=register", form);
        someRequest.synchronous = true;
        someRequest.Send((request) =>
        {
            Debug.Log(request.response.Text);
            Hashtable json = (Hashtable)JSON.JsonDecode(request.response.Text);
            this.afterRegister(json);
        });
    }

    public void afterRegister(Hashtable json)
    {
        if (!(bool)json["success"])
        {

            Debug.Log("fail " + json["error"]);
            // TODO: handle
            return;
        }
        // Authenticate and get token.
        Login(curUsername, curPassword);
        Debug.Log("After");
    }

    public void Login(String username, String password)
    {
        curUsername = username;
        curPassword = password;

        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        HTTP.Request someRequest = new HTTP.Request("post", "http://so.meaglin.com/api.php?action=login", form);
        someRequest.synchronous = true;
        someRequest.Send((request) =>
        {
            Debug.Log(request.response.Text);
            Hashtable json = (Hashtable)JSON.JsonDecode(request.response.Text);
            this.afterLogin(json);
        });
    }

    public void afterLogin(Hashtable json)
    {
        if (!(bool)json["success"])
        {

            Debug.Log("fail " + json["error"]);
            // TODO: handle
            return;
        }
        Hashtable data = (Hashtable)json["data"];

        // Don't keep in memory
        curUsername = null;
        curPassword = null;

        accessToken = (String)data["token"];
        loggedIn = true;
    }
}

