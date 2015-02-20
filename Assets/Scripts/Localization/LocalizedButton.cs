using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedButton
{
    #region Protected members
    public Button mc;
    protected Text textfield;
    protected string text;
    protected InterfaceBehaviour interfaces;
    #endregion


    #region Ctor
    public LocalizedButton(Button _mc, string _text)
    {
        mc = _mc;
        text = _text;
        interfaces = GameObject.FindGameObjectWithTag("Interface").GetComponent<InterfaceBehaviour>();
        SetButtonState();
    }

    public void SetButtonState()
    {
        Text txtField = mc.GetComponentInChildren<Text>();
        LocalizedText localizedText = new LocalizedText(txtField, text);
      /*  RectTransform rectButton = mc.gameObject.GetComponent<RectTransform>();
        RectTransform rectText = txtField.gameObject.GetComponent<RectTransform>();
        rectButton.sizeDelta = new Vector2(rectText.sizeDelta.x, rectButton.sizeDelta.y);
        for (int i = 0; i < mc.transform.childCount; i++) 
        {
            if (mc.transform.GetChild(i).tag == "Underline") 
            {
                mc.transform.GetChild(i).GetComponent<RectTransform>().sizeDelta = new Vector2(rectText.sizeDelta.x, mc.transform.GetChild(i).GetComponent<RectTransform>().sizeDelta.y);
                mc.transform.GetChild(i).GetComponent<Image>().color = mc.GetComponentInChildren<Text>().color;
            }
        }*/
    }
    #endregion

    #region Public methods
    public void Destroy()
    {
        //Translations.Instance.RemoveElement(this);
        //base.Destroy();
    }

    public void OnLanguageChanged(string lang)
    {
        SetButtonState();
    }
    #endregion
}
