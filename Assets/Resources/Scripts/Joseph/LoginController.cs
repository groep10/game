using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginController : MonoBehaviour {

    public Button register, login;

    public InputField email, password;

    public GameObject loading, errorWindow, userPanel;

    void Start()
    {
        if (login != null) login.onClick.AddListener(onLoginPressed);
        if (register != null) register.onClick.AddListener(onRegisterPressed);
    }

    public void onLoginPressed()
    {
        Debug.Log("login");
        setLoading(true);
        AccountController.getInstance().login(email.text, password.text, this.onResult);
    }

    public void onRegisterPressed()
    {
        Debug.Log("register");
        setLoading(true);
        AccountController.getInstance().register(email.text, password.text, this.onResult);
    }

    void setLoading(bool isloading)
    {
        if (loading != null)
        {
            loading.SetActive(isloading);   
        }
    }

    public void onResult(Hashtable result)
    {
        setLoading(false);
        if (!(bool)result["success"])
        {
            GameObject err = (GameObject) Instantiate(errorWindow);
            err.GetComponentInChildren<Text>().text = (String) result["error"];
            err.transform.SetParent(transform, false);
            
            return;
        }

        if (userPanel != null)
        {
            String displayname = (String)AccountController.getInstance().getUser()["displayname"];
            userPanel.GetComponentInChildren<Text>().text = displayname;

            AccountController.getInstance().getUserAvatar(texture =>
            {
                userPanel.GetComponentInChildren<RawImage>().texture = texture;
                Debug.Log("done");
            });
        }

    }

    Sprite tex2sprite(Texture2D tex)
    {
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }
}

