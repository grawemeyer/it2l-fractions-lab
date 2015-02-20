using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class LocalizedText
{
    #region Protected members
    protected Text textfield;
    protected string text;
    #endregion

    #region Public properties
    public string Text
    {
        get
        {
            return text;
        }
        set
        {
            text = value;
            textfield.text = Localizations.Instance.replaceText(text);
        }
    }

    public string BaseText
    {
        get
        {
            return textfield.text;
        }
    }
    #endregion

    #region Ctor
    public LocalizedText(Text _textfield, string _text)
    {
        textfield = _textfield;
        Text = _text;
        textfield.text = Localizations.Instance.replaceText(text);
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
        textfield.text = Localizations.Instance.replaceText(text);
    }
    #endregion
}