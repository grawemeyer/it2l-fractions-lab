using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopupConfirm : MonoBehaviour {

    public Text message;
    public InterfaceBehaviour interfaceB;
    protected string operation;

    public void Initialize(string msg) 
    {
        operation = msg;
        switch (msg) 
        {
            case "home":
                interfaceB.localizationUtils.AddTranslationText(message, "{confirm_home}");
                break;
            case "logout":
                interfaceB.localizationUtils.AddTranslationText(message, "{confirm_logout}");
                break;
        }
    }

    public void Confirm() 
    {
        if (operation == "home")
        {
            ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "ConfirmHome");
            interfaceB.CleanWorkspace();
        }
        else if (operation == "logout")
            interfaceB.Logout();
    }

    public void Cancel() 
    {
        if (operation == "home")
        {
            ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "CancelHome");
        }
    }

	
}
