using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIPopup : MonoBehaviour {

    public bool isDraggable;
    public bool isBlockable;
    public bool isHighFeedback;
    public bool destroyOnClose;
    public Button okButton;
    public Text text;
    public InterfaceBehaviour interfaceB;

	void Start () {
        //isHighFeedback = false;
        interfaceB = GameObject.FindGameObjectWithTag("Interface").GetComponent<InterfaceBehaviour>();
        if(null != okButton)
            interfaceB.localizationUtils.AddTranslationButton(okButton, "{ok}");
	}
	
    public void ClosePopup() 
    {
        if (isHighFeedback)
            ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "CloseFeedbackPopup");
        interfaceB.PopPopup();
        interfaceB.RemoveFeedbackPopup();
    }

    public void setText(string _text)
    {
        text.text = _text;
    }

    public void SetColor(Color _color)
    {
        text.color = _color;

    }
    public void Close()
    {
       gameObject.SetActive(false);
    }
}
