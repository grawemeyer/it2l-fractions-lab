using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using fractionslab.utils;

public class TopBottom : MonoBehaviour {

    public bool isClose;
   /* public GameObject plusOperation;
    public GameObject minusOperation;
    public GameObject searchOperation;*/
    public GameObject operationsPanel;
    public GameObject operationsButton;
    public Button oper2;
    public Button oper3;
    public GameObject operationsSet;
    public List<GameObject> panels;
    public InterfaceBehaviour interfaceB;

    protected int operationIndex;

    protected int operation;

	// Use this for initialization
	void Start () {
        operationIndex = 0;
        isClose = true;
        oper2.gameObject.SetActive(false);
        oper3.gameObject.SetActive(false);
        operationsSet.gameObject.SetActive(false);
    }

    public void OpenTab(int index)
    {
        if (operationIndex == index)
            return;

       // Debug.Log("OpenTab " + index);
        if (index == 1)
        {
            foreach (Button bt in operationsSet.GetComponentsInChildren<Button>())
                bt.interactable = true;
            operationsSet.GetComponent<ButtonsSetManager>().Initialize();
            ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "3OperandTab");
        }
        if (index == 0)
        {
            foreach (Button bt in operationsSet.GetComponentsInChildren<Button>())
                bt.interactable = false;
            ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "2OperandTab");
        }
        operationIndex = index;
        panels[operationIndex].transform.SetAsLastSibling();
        panels[operationIndex].GetComponent<Operations>().ChangeTab();
        panels[1 - operationIndex].GetComponent<Operations>().ChangeTab();
        interfaceB.EnablePlaceHolder(index);
        interfaceB.SetOperation((InterfaceBehaviour.FractionsOperations)index);
    }

    public void ChangeStatePanel() 
    {
        ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "Lab");
        if (isClose)
        {
            operationsPanel.GetComponent<Animation>().Play("operationsPanel");
            operationsButton.GetComponent<Animation>().Play("btnLabAnimation");
            isClose = false;
           // Debug.Log("panels " + panels.Count );
            if (panels.Count > 0)
            {
                panels[0].transform.SetAsLastSibling();       
                panels[0].GetComponent<Operations>().InitializeOperation();
                panels[1].GetComponent<Operations>().InitializeOperation();
                panels[0].GetComponent<Operations>().ChangeTab();
                panels[1].GetComponent<Operations>().ChangeTab();
            }
            oper2.gameObject.SetActive(true);
            oper3.gameObject.SetActive(true);
            operationsSet.gameObject.SetActive(true);
            oper2.interactable = true;
            oper3.interactable = true;
            operationIndex = 0;
            interfaceB.EnablePlaceHolder(0);
            interfaceB.InitializeOperationMenu();
            interfaceB.SetOperation(InterfaceBehaviour.FractionsOperations.FIND);
        }
        else 
        {
            operationsPanel.GetComponent<Animation>().Play("operationsPanelClose");
            operationsButton.GetComponent<Animation>().Play("btnLabAnimationClose");
            isClose = true;
            oper2.gameObject.SetActive(false);
            oper3.gameObject.SetActive(false);
            operationsSet.gameObject.SetActive(false);
            oper2.interactable = false;
            oper3.interactable = false;
            interfaceB.DisablePlaceHolders();
            foreach (Button bt in operationsSet.GetComponentsInChildren<Button>())
                bt.interactable = false;
        }
    }
    
    public void OpenTopBottom(int _operation) 
    {
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(gameObject.GetComponent<RectTransform>().sizeDelta.x, 187.0f);
        isClose = false;
        operation = _operation;
        switch(_operation)
        {
         /*   case 0:
                plusOperation.SetActive(true);
                minusOperation.SetActive(false);
                searchOperation.SetActive(false);
                for (int i = 0; i < 3; i++)
                    plusOperation.GetComponent<Operations>().InitializeFractioValue(i);
                plusOperation.GetComponent<Operations>().setResult("?");
                    break;
            case 1:
                plusOperation.SetActive(false);
                minusOperation.SetActive(true);
                searchOperation.SetActive(false);
                for (int i = 0; i < 3; i++)
                    minusOperation.GetComponent<Operations>().InitializeFractioValue(i);
                minusOperation.GetComponent<Operations>().setResult("?");
                break;
            case 2:
                plusOperation.SetActive(false);
                minusOperation.SetActive(false);
                searchOperation.SetActive(true);
                for (int i = 0; i < 2; i++)
                    searchOperation.GetComponent<Operations>().InitializeFractioValue(i);
                searchOperation.GetComponent<Operations>().setResult("?");
                break;
*/
        }
        
        
    }

    public void CloseTopBottom()
    {
      /*  gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(gameObject.GetComponent<RectTransform>().sizeDelta.x, 87.0f);
        isClose = true;
        plusOperation.SetActive(false);
        minusOperation.SetActive(false);
        searchOperation.SetActive(false);
        operation = -1;*/
    }
    
    public void setFractionValue( float numerator, float denominator, int index) 
    {
       // Debug.Log("fraction value " + numerator + " " + denominator +  " " + index + " " + operationIndex);
        panels[operationIndex].GetComponent<Operations>().setFractioValue(numerator, denominator, index);
       /* switch (operation)
        {
            case 0:
               // plusOperation.GetComponent<Operations>().setFractioValue(numerator, denominator,index);
                break;
            case 1:
               // minusOperation.GetComponent<Operations>().setFractioValue(numerator, denominator,index);
                break;
            case 2:
                //searchOperation.GetComponent<Operations>().setFractioValue(numerator, denominator,index);
                break;
        }*/
    
    }

    public void setResult(string result)
    {
        panels[operationIndex].GetComponent<Operations>().setResult(result);
       /* switch (operation)
        {
            case 0:
               // plusOperation.GetComponent<Operations>().setResult(result);
                break;
            case 1:
               // minusOperation.GetComponent<Operations>().setResult(result);
                break;
            case 2:
               // searchOperation.GetComponent<Operations>().setResult(result);
                break;
        }*/
    }

}
