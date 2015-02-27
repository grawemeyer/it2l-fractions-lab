using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using fractionslab.utils;

public class ToolCustomizationWindow : MonoBehaviour {

    public InterfaceBehaviour interfaceB;
    public Text explanation;
    public List<Toggle> toggles;
	void Start () {
        interfaceB.localizationUtils.AddTranslationText(explanation, "{tool_customization_explanation}");
        
	}

    void OnEnable() 
    {
        for (int i = 0; i < toggles.Count; i++)
        {
            if (TaskManager.Instance.initialConfiguration[((configurationName)i).ToString()])
            {
                if (toggles[i].interactable)
                    toggles[i].isOn = true;
            }
            else
            {
                if (toggles[i].interactable)
                    toggles[i].isOn = false;
            }
        }
    }

    public void RestorePreviousState() 
    {
        OnEnable();
    }

    public void ConfirmConfiguration() 
    {
        for (int i = 0; i < toggles.Count; i++)
        {
            //Debug.Log("toggles" + toggles[i].name);
            if(toggles[i].IsInteractable())
                interfaceB.ChangeStateButton(i, toggles[i].isOn);            
        }
        /*foreach (Toggle tg in GetComponentsInChildren<Toggle>())
        {
                interfaceB.ChangeStateButton(tg.name, tg.isOn);
        }*/
        interfaceB.PopPopup();
    }

    public void ModifyStateTool(string btname) 
    {
        //interfaceB.ChangeStateButton(btname);
    }

    public void CleanStateToggle() 
    {
      //  Debug.Log("cleanstatetoggle");
        foreach (Toggle tg in GetComponentsInChildren<Toggle>(true))
        {
            if (tg.interactable)
                tg.isOn = true;
        }
    }
}
