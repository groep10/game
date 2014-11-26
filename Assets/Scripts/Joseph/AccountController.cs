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

    public String register(String email, String password) {

        String message = null;
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("password", password);
        WWW request = new WWW("http://so.meaglin.com/api.php?action=register", form);
        
        
        return message;
    }
}

