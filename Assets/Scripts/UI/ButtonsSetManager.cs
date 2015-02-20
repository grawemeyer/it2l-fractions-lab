using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ButtonsSetManager : MonoBehaviour {

    public List<Sprite> plusSprite;
    public List<Sprite> minusSprite;
    public Image btnPlus;
    public Image btnMinus;
    public bool isPlusActive;
    public InterfaceBehaviour interfaceB;
    public bool lockInteraction;

    public void Initialize() 
    {
        lockInteraction = false;
        btnPlus.sprite = plusSprite[0];
        btnMinus.sprite = minusSprite[3];
        isPlusActive = true;
        interfaceB.SetOperation(InterfaceBehaviour.FractionsOperations.ADDITION);
    }

    public void ChangeStateButton() 
    {
        if (isPlusActive)
        {
            btnPlus.sprite = plusSprite[3];
            btnMinus.sprite = minusSprite[0];
            isPlusActive = false;
            interfaceB.SetOperation(InterfaceBehaviour.FractionsOperations.SUBTRACTION);
            interfaceB.CheckFractionOperation();
            ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", " OperationSwitchMinus");
        }
        else 
        {
            btnPlus.sprite = plusSprite[0];
            btnMinus.sprite = minusSprite[3];
            isPlusActive = true;
            interfaceB.SetOperation(InterfaceBehaviour.FractionsOperations.ADDITION);
            interfaceB.CheckFractionOperation();
            ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "OperationSwitchPlus");

        }
    }

    public void pointerEnter(string btn) 
    {
        if (lockInteraction)
            return;
        if(btn == "plus" && isPlusActive)
            btnPlus.sprite = plusSprite[1];
        if (btn == "minus" && !isPlusActive)
            btnMinus.sprite = minusSprite[1];   
    }

    public void pointerExit(string btn)
    {
        if (lockInteraction)
            return;
        if (btn == "plus" && isPlusActive)
            btnPlus.sprite = plusSprite[0];
        if (btn == "minus" && !isPlusActive)
            btnMinus.sprite = minusSprite[0];
    }

    public void pointerDown(string btn)
    {
        if (lockInteraction)
            return;
        if (btn == "plus" && isPlusActive)
            btnPlus.sprite = plusSprite[2];
        if (btn == "minus" && !isPlusActive)
            btnMinus.sprite = minusSprite[2];
    }

}
