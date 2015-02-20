using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIStartPage : MonoBehaviour {

    public Text user;
    public InterfaceBehaviour interfaceB;
    public Text loadTitle;
    public Text loadContent;
    public Text newTitle;
    public Text newContent;
    public Button logout;

    protected bool isStudent;
	// Use this for initialization
	void Start () {
        Initialize();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void Initialize() 
    {
        isStudent = interfaceB.isStudent;
        interfaceB.localizationUtils.AddTranslationText(loadTitle.GetComponentInChildren<Text>(), "{load}");
        interfaceB.localizationUtils.AddTranslationText(newTitle.GetComponentInChildren<Text>(), "{new}");
        interfaceB.localizationUtils.AddTranslationText(loadContent.GetComponentInChildren<Text>(), "{load_instruction}");
        interfaceB.localizationUtils.AddTranslationText(newContent.GetComponentInChildren<Text>(), "{new_instruction}");
        interfaceB.localizationUtils.AddTranslationButton(logout, "{logout}");
        user.text = interfaceB.user;
        if (isStudent)
        {  
            loadTitle.color = InterfaceBehaviour.ClearGreen;
            newTitle.color = InterfaceBehaviour.ClearGreen;
            user.color = InterfaceBehaviour.ClearGreen;
            
        }
        else 
        {
            loadTitle.color = InterfaceBehaviour.Orange;
            newTitle.color = InterfaceBehaviour.Orange;
            user.color = InterfaceBehaviour.Orange;
        }
    
    }

   

}
