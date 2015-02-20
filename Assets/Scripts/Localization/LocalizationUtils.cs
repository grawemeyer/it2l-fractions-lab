using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizationUtils: MonoBehaviour
{
    #region Protected members
    public GameObject interfaces;
    protected Dictionary<Text, LocalizedText> localizedTextList;
    protected Dictionary<Button, LocalizedButton> localizedButtonList;
    #endregion

   
    #region Ctor
    void Awake()
    {
        localizedTextList = new Dictionary<Text, LocalizedText>();
        localizedButtonList = new Dictionary<Button, LocalizedButton>();
    }
    #endregion

    #region Public methods
    public void AddTranslationText(Text _textfield, string txt)
    {
        if (_textfield == null)
            return;

        //Debug.Log("AddTranslationText " + _textfield.name);

        if (localizedTextList.ContainsKey(_textfield))
        {
            LocalizedText _translationText;
            localizedTextList.TryGetValue(_textfield, out _translationText);
            _translationText = new LocalizedText(_textfield, txt);
            //Debug.Log("C?E? GIA? " + _textfield.name);
        }
        else
        {
            localizedTextList.Add(_textfield, new LocalizedText(_textfield, txt));
        }
    }

    public LocalizedButton AddTranslationButton(Button _mc, string _text)
    {
        LocalizedButton localizedButton;
        if (localizedButtonList.ContainsKey(_mc))
        {
            localizedButtonList.TryGetValue(_mc, out localizedButton);
            localizedButton = new LocalizedButton(_mc, _text);
            //_translationText = new TranslationText(_textfield, txt); TODO
        }
        else
        {
            localizedButton = new LocalizedButton(_mc, _text);
            localizedButtonList.Add(_mc, localizedButton);
        }
        return localizedButton;
    }

    public void Destroy()
    {
        //Translations.Instance.RemoveElement(this);
        //base.Destroy();
    }


    public void OnLanguageChanged(string lang)
    {
        foreach (KeyValuePair<Text, LocalizedText> _translationText in localizedTextList)
        {
            if (_translationText.Value != null)
            {
                _translationText.Value.OnLanguageChanged(lang);
            }
        }

        foreach (KeyValuePair<Button, LocalizedButton> _translationButton in localizedButtonList)
        {
            if (_translationButton.Value != null)
            {
                _translationButton.Value.OnLanguageChanged(lang);
            }
        }
    }
    #endregion
}