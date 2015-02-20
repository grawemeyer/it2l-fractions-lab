using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Operations : MonoBehaviour {

    public List<GameObject> fractions;
    public Text result;
    public Sprite tabEnable;
    public Sprite tabDisable;
    public Button tab;
    public bool isOpen = false;
    public InterfaceBehaviour interfaceB;
    public int operationIndex;
    public List<GameObject> fractionLines;


	// Use this for initialization
	void Start () {
        if(operationIndex == 0)
            interfaceB.localizationUtils.AddTranslationButton(tab, "{operation_2}");
        else if(operationIndex == 1)
            interfaceB.localizationUtils.AddTranslationButton(tab, "{operation_3}");
        Reset();
	}

    protected void Reset()
    {   
           foreach (GameObject fr in fractions) 
        {
           // fr.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            foreach (Text tx in fr.GetComponentsInChildren<Text>()) 
            {
                if (tx.gameObject.name == "Instruction")
                {
                    interfaceB.localizationUtils.AddTranslationText(tx , "{drag_fraction}");
                }
                else
                    tx.text = "";
            }
        }
        result.text = "?";
        foreach (GameObject line in fractionLines)
            line.SetActive(false);
    }

    public void setFractioValue(float numerator, float denominator, int index)
    {
       // Debug.Log("OPERATION " + numerator + " " + denominator + " " + index);
        //fractions[index].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        fractionLines[index].SetActive(true);
        foreach (Text tx in fractions[index].GetComponentsInChildren<Text>()) 
        {
            if (tx.gameObject.name == "Numerator") 
            {
                tx.text = numerator.ToString();
            }
            else if (tx.gameObject.name == "Denominator") 
            {
                tx.text = denominator.ToString();
            }
            else if (tx.gameObject.name == "Instruction")
            {
                tx.text = "";
            }
        }
    }

    public void InitializeFractioValue(int index)
    {
        foreach (Text tx in fractions[index].GetComponentsInChildren<Text>())
        {
            tx.text = "";
        }
    }

    public void setResult(string _result) 
    {
        result.text = _result;
    }

    public void ChangeTab() 
    {
        //Debug.Log("changeTab " + isOpen);
        if (isOpen)
        {
            tab.image.sprite = tabDisable;
            isOpen = false;
        }
        else 
        {
            tab.image.sprite = tabEnable;
            isOpen = true;
        }

    }

    public void InitializeOperation() 
    {
        Reset();
        if (operationIndex == 0)
            isOpen = false;
        else
            isOpen = true;
    }
}
