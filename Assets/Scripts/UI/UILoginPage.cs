using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class UILoginPage : MonoBehaviour {
    public bool fromLogout;
    public Button question;
    public InputField username;
    public InputField password;
    public Text  usernameOv;
    public Text passwordOv;
    public Text error;
    public Button loginbtn;
    public Image underlineRegister;
    public Image underlineQuestion;
    protected string nextName;
    protected bool isStudent;
    public  bool isFirstTime;
    protected int lastCharCountUser;
    protected int lastCharCountPwd;
    public InterfaceBehaviour interfaceB;
    public Animation logoAnimation;
    public Animation textAnimation;
	void Start () {
        //interfaceB = GameObject.FindGameObjectWithTag("Interface").GetComponent<InterfaceBehaviour>();
      //  isFirstTime = true;
        lastCharCountUser = 0;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Selectable next = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();

            if (next != null)
            {
                InputField inputfield = next.GetComponent<InputField>();               
                if (inputfield != null)
                {
                    nextName = inputfield.gameObject.name;
                    inputfield.OnPointerClick(new PointerEventData(EventSystem.current));
                    CancelText(nextName);
                }
                EventSystem.current.SetSelectedGameObject(next.gameObject, new BaseEventData(EventSystem.current));
            }
        }
      //  if ( && username.text.Length == 0)
        if (username.text.Length > 0)
            usernameOv.gameObject.SetActive(false);
        if (password.text.Length > 0)
            passwordOv.gameObject.SetActive(false);
        if (isStudent)
        {
            if (usernameOv.gameObject.activeSelf)
                usernameOv.GetComponentInChildren<Text>().color = InterfaceBehaviour.DarkGreen;
            if (passwordOv.gameObject.activeSelf)
                passwordOv.GetComponentInChildren<Text>().color = InterfaceBehaviour.DarkGreen;
        }
        else 
        {
            if (usernameOv.gameObject.activeSelf)
                usernameOv.GetComponentInChildren<Text>().color = InterfaceBehaviour.Orange;
            if (passwordOv.gameObject.activeSelf)
                passwordOv.GetComponentInChildren<Text>().color = InterfaceBehaviour.Orange;
        }
	}

    #region Funtionality

    void Initialize() 
    {
        //Debug.Log("INITILIZE "+ fromLogout);
        isStudent = true;
        username.text = "";
        password.text = "";
        usernameOv.gameObject.SetActive(true);
        passwordOv.gameObject.SetActive(true);
        question.GetComponentInChildren<Text>().color = InterfaceBehaviour.Orange;
        underlineQuestion.color = InterfaceBehaviour.Orange;
        username.GetComponentInChildren<Text>().color = InterfaceBehaviour.DarkGreen;
        password.GetComponentInChildren<Text>().color = InterfaceBehaviour.DarkGreen;
        usernameOv.GetComponentInChildren<Text>().color = InterfaceBehaviour.DarkGreen;
        passwordOv.GetComponentInChildren<Text>().color = InterfaceBehaviour.DarkGreen;
        interfaceB.localizationUtils.AddTranslationText(usernameOv, "{username}");    
        interfaceB.localizationUtils.AddTranslationText(passwordOv, "{password}");
        interfaceB.localizationUtils.AddTranslationButton(loginbtn, "{login}");
        interfaceB.localizationUtils.AddTranslationButton(question, "{are_you_teacher}");
        if (isFirstTime)
        {
            //Debug.Log(logoAnimation["loginPage"].speed);
            logoAnimation["loginPage"].speed = 1.5f;
            textAnimation["loginPage3"].speed = 1.5f;
            logoAnimation.Play("loginPage");
            textAnimation.Play("loginPage3");
            isFirstTime = false;
        }
    }

    public void CancelText(string txt) 
    {
        if (txt == "Username")
        {
            usernameOv.gameObject.SetActive(false);
        }
        if (txt == "Pwd")
            passwordOv.gameObject.SetActive(false);
    }

    public void ShowTextLabel(string txt)
    {
        if (txt == "username" && username.text.Length == 0)
            usernameOv.gameObject.SetActive(true);
        if (txt == "Pwd" && password.text.Length == 0)
            passwordOv.gameObject.SetActive(true);
    }

    public void SetIsStudent() 
    {
        isStudent = !isStudent;
        if (isStudent)
        {
            username.GetComponentInChildren<Text>().color = InterfaceBehaviour.DarkGreen;
            password.GetComponentInChildren<Text>().color = InterfaceBehaviour.DarkGreen;
            if (usernameOv.gameObject.activeSelf)
                usernameOv.GetComponentInChildren<Text>().color = InterfaceBehaviour.DarkGreen;
            if (passwordOv.gameObject.activeSelf)
                passwordOv.GetComponentInChildren<Text>().color = InterfaceBehaviour.DarkGreen;
            question.GetComponentInChildren<Text>().color = InterfaceBehaviour.Orange;
            underlineQuestion.color = InterfaceBehaviour.Orange;
            interfaceB.localizationUtils.AddTranslationButton(question, "{are_you_teacher}");
           // question.text = "Are you a teacher?";
        }
        else 
        {
            //Debug.Log("are you student?");
            username.GetComponentInChildren<Text>().color = InterfaceBehaviour.Orange;
            password.GetComponentInChildren<Text>().color = InterfaceBehaviour.Orange;
            if(usernameOv.gameObject.activeSelf)
                usernameOv.GetComponentInChildren<Text>().color = InterfaceBehaviour.Orange;
            if (passwordOv.gameObject.activeSelf)
                passwordOv.GetComponentInChildren<Text>().color = InterfaceBehaviour.Orange;
            question.GetComponentInChildren<Text>().color = InterfaceBehaviour.ClearGreen;
            underlineQuestion.color = InterfaceBehaviour.ClearGreen;
            interfaceB.localizationUtils.AddTranslationButton(question, "{are_you_student}");
           // question.text = "Are you a student?";
        }
    }

    public void SetFromLogout(bool fromlog) 
    {
        fromLogout = fromlog;
        //Debug.Log("SetFromLogout " + fromLogout);

    }

    public void CheckLogin()
    {
        if (!interfaceB.CheckLogin(username.text.ToString(), password.text.ToString()))
        {
            setError("{error_login}");
        }
        else 
        {
            setError("");

        }
    }


    #endregion

    #region Message

    public void setError(string err) 
    {
        error.text = err;
        interfaceB.localizationUtils.AddTranslationText(error, err);
    }

    public void CheckEmptyString(string textname)
    {
        if (textname == "username")
        {
            if (username.text.Length == 0 && lastCharCountUser >= 1)
                usernameOv.gameObject.SetActive(true);
            lastCharCountUser = username.text.Length;
        }
        else if (textname == "password")
        {
            if (password.text.Length == 0 && lastCharCountPwd >= 1)
                passwordOv.gameObject.SetActive(true);
            lastCharCountPwd = password.text.Length;
        }
       
    /*    if (username.text.Length == 0 && lastCharCountUser == 1 && textname == "username")
        {
            interfaceB.localizationUtils.AddTranslationText(usernameOv, "{username}");
        }
        else if (textname == "username")
        {
            //Debug.Log("usernameOv.text " + usernameOv.text +" "+ usernameOv.text.Length);
            interfaceB.localizationUtils.AddTranslationText(usernameOv, "");
            lastCharCountUser = username.text.Length;
        }

        if (password.text.Length == 0 && lastCharCountPwd == 1 && textname == "password")
        {
            interfaceB.localizationUtils.AddTranslationText(passwordOv, "{password}");
        }
        else if (textname == "password")
        {
            interfaceB.localizationUtils.AddTranslationText(passwordOv, "");
            lastCharCountPwd = password.text.Length;
        }
        */
        
    }

    #endregion


}

