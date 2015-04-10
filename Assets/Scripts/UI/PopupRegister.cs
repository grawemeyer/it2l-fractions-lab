using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;


public class PopupRegister : MonoBehaviour {

    public InterfaceBehaviour interfaceB;

    public InputField username;
    public InputField password;
    public InputField rePassword;
    public Text usernameOv;
    public Text passwordOv;
    public Text rePasswordOv;
    public Text explanation;
    public Text errorText;
    public Button register;
    protected int lastCharCountUser;
    protected int lastCharCountPwd;
    protected int lastCharCountRePwd;

    void OnEnable()
    {
        if (interfaceB.isStudent)
        {
            username.GetComponentInChildren<Text>().color = InterfaceBehaviour.DarkGreen;
            password.GetComponentInChildren<Text>().color = InterfaceBehaviour.DarkGreen;
            rePassword.GetComponentInChildren<Text>().color = InterfaceBehaviour.DarkGreen;
            usernameOv.color = InterfaceBehaviour.DarkGreen;
            passwordOv.color = InterfaceBehaviour.DarkGreen;
            rePasswordOv.color = InterfaceBehaviour.DarkGreen;
        }
        else
        {
            username.GetComponentInChildren<Text>().color = InterfaceBehaviour.Orange;
            password.GetComponentInChildren<Text>().color = InterfaceBehaviour.Orange;
            rePassword.GetComponentInChildren<Text>().color = InterfaceBehaviour.Orange;
            usernameOv.color = InterfaceBehaviour.Orange;
            passwordOv.color = InterfaceBehaviour.Orange;
            rePasswordOv.color = InterfaceBehaviour.Orange;
        }
        username.text = "";
        password.text = "";
        rePassword.text = "";
        usernameOv.gameObject.SetActive(true);
        passwordOv.gameObject.SetActive(true);
        rePasswordOv.gameObject.SetActive(true);

        interfaceB.localizationUtils.AddTranslationText(explanation, "{register_explanation}");
        interfaceB.localizationUtils.AddTranslationText(usernameOv, "{username}");
        interfaceB.localizationUtils.AddTranslationText(passwordOv, "{password}");
        interfaceB.localizationUtils.AddTranslationText(rePasswordOv, "{re-type_password}");
        interfaceB.localizationUtils.AddTranslationButton(register, "{register}");
        lastCharCountUser = 0;
        lastCharCountPwd = 0;
        lastCharCountRePwd = 0;
    }

    public void CheckRegister() 
    {
        if (username.text == ""|| password.text == "" || rePassword.text == "") 
        {
            setError("{error_register_fill}");
            return;
        }

        if(password.text != rePassword.text)
        {
            setError("{error_register_pwd}");
            return;
        }

        if(!interfaceB.CheckRegister(username.text,  password.text))
        {
            setError("{error_register_user}");
        }
    }

    public void setError(string error) 
    {
        errorText.text = error;
        interfaceB.localizationUtils.AddTranslationText(errorText, error);
    }


    public void CancelText(Text txt)
    {
            txt.text = "";
    }

    public void CancelTextLabel(string txt)
    {
        if (txt == "username")
            usernameOv.gameObject.SetActive(false);
        if (txt == "pwd")
            passwordOv.gameObject.SetActive(false);
        if (txt == "repwd")
            rePasswordOv.gameObject.SetActive(false);
    }

    public void ShowTextLabel(string txt)
    {
        if (txt == "username" && username.text.Length == 0)
            usernameOv.gameObject.SetActive(true);
        if (txt == "pwd" && password.text.Length == 0)
            passwordOv.gameObject.SetActive(true);
        if (txt == "repwd" && rePassword.text.Length == 0)
            rePasswordOv.gameObject.SetActive(true);
    }


    public void CheckEmptyString(string textname)
    {
        //Debug.Log(username.text.Length + " " + lastCharCountUser);
        if (textname == "username") 
        {
            if (username.text.Length == 0 && lastCharCountUser >= 1 )
            {
                usernameOv.gameObject.SetActive(true);
            }
            lastCharCountUser = username.text.Length;
        }
        if (textname == "password")
        {
            if (password.text.Length == 0 && lastCharCountPwd >= 1)
            {
                passwordOv.gameObject.SetActive(true);
            }
            lastCharCountPwd = password.text.Length;
        }
        if (textname == "repassword")
        {
            if (rePassword.text.Length == 0 && lastCharCountRePwd >= 1)
            {
                rePasswordOv.gameObject.SetActive(true);
            }
            lastCharCountRePwd = rePassword.text.Length;
        }
    }


    void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Selectable next = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();

            if (next != null)
            {
                InputField inputfield = next.GetComponent<InputField>();
                if (inputfield != null)
                {
                    inputfield.OnPointerClick(new PointerEventData(EventSystem.current));
                    CancelTextLabel(inputfield.gameObject.name);
                }
                EventSystem.current.SetSelectedGameObject(next.gameObject, new BaseEventData(EventSystem.current));
            }
        }
        if (username.text.Length > 0)
            usernameOv.gameObject.SetActive(false);
        if (password.text.Length > 0)
            passwordOv.gameObject.SetActive(false);
        if (rePassword.text.Length > 0)
            rePasswordOv.gameObject.SetActive(false);

        if (interfaceB.isStudent)
        {
            if (usernameOv.gameObject.activeSelf)
                usernameOv.GetComponentInChildren<Text>().color = InterfaceBehaviour.DarkGreen;
            if (passwordOv.gameObject.activeSelf)
                passwordOv.GetComponentInChildren<Text>().color = InterfaceBehaviour.DarkGreen;
            if (rePasswordOv.gameObject.activeSelf)
                rePasswordOv.GetComponentInChildren<Text>().color = InterfaceBehaviour.DarkGreen;
        }
        else
        {
            if (usernameOv.gameObject.activeSelf)
                usernameOv.GetComponentInChildren<Text>().color = InterfaceBehaviour.Orange;
            if (passwordOv.gameObject.activeSelf)
                passwordOv.GetComponentInChildren<Text>().color = InterfaceBehaviour.Orange;
            if (rePasswordOv.gameObject.activeSelf)
                rePasswordOv.GetComponentInChildren<Text>().color = InterfaceBehaviour.Orange;
        }
    }
}
