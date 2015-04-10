using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopupTaskDescriptor : MonoBehaviour {
    public Text explanation;
    public Text title;
    public Text description;
    public Text titleOv;
    public Text descriptionOv;
    public InputField titleText;
    public InputField descriptionText;
    public Button btnOk;
    public Text drag;
    public InterfaceBehaviour interfaceB;
    protected int previousCountTitle;
    protected int previousCountDescr;
	// Use this for initialization
	void OnEnable() {
        if (null != explanation) 
        {
            interfaceB.localizationUtils.AddTranslationText(explanation, "{task_description_Explanation}");
            //Debug.Log("explanations" + explanation.text);

        }
        previousCountTitle = 0;
        previousCountDescr = 0;

        if (null != btnOk) 
        {
            interfaceB.localizationUtils.AddTranslationButton(btnOk, "{ok}");
        }
        if (null != titleOv && !interfaceB.istaskDefined) 
        {
            titleOv.gameObject.SetActive(true);
            interfaceB.localizationUtils.AddTranslationText(titleOv, "{title_here}");
        }
        if (null != descriptionOv && !interfaceB.istaskDefined)
        {
            descriptionOv.gameObject.SetActive(true);
            interfaceB.localizationUtils.AddTranslationText(descriptionOv, "{text_here}");
        }
        if (null != drag)
        {
            interfaceB.localizationUtils.AddTranslationText(drag, "{drag_window}");
        }

	}
	
	// Update is called once per frame
	void Update () {
        if (null != titleText && titleText.text.Length > 0)
            titleOv.gameObject.SetActive(false);
        if (null != descriptionText && descriptionText.text.Length > 0)
            descriptionOv.gameObject.SetActive(false);
        if(Input.GetMouseButtonUp(0) && interfaceB.isBlockingOperation)
            interfaceB.isBlockingOperation = false;
	
	}

    public void initialize(string _title, string _description, bool isStudent) 
    {
        if (!isStudent && null != titleText)
        {
            titleText.text = _title;
            if(null != descriptionText)
                descriptionText.text = _description;
        }
        else 
        {
            title.text = _title;
            description.text = _description;
        }
        
    }

    public void SaveTask() 
    {
        //todo save task information; call intefaceB.SaveTask();
        if (interfaceB.isStudent)
        {
            interfaceB.ChangeTaskDescription(title.text, description.text);
        }
        else
        {
            interfaceB.ChangeTaskDescription(titleText.text, descriptionText.text);
        }
        Close();
    }

    public void Close() 
    {
        //gameObject.SetActive(false);
        interfaceB.PopPopup();
        interfaceB.taskDescription.GetComponent<UIButton>().EnableBtn(false);
    }

    public void cancelText(Text tx) 
    {
        tx.gameObject.SetActive(false);
    }

    public void ChangeText(InputField tx)
    {
        if (tx.name == "Title") 
        {
            if (tx.text.Length == 0 && previousCountTitle >= 1)
                titleOv.gameObject.SetActive(true);
            previousCountTitle = tx.text.Length;
        }
        else if (tx.name == "Description"){
            if (tx.text.Length == 0 && previousCountDescr >= 1)
                descriptionOv.gameObject.SetActive(true);
            previousCountDescr = tx.text.Length;
        }   
    }

    public void UpdateTitle() 
    {
        //Debug.Log("UpdateTitle " + interfaceB.task.title);
        if (interfaceB.task.title.ToString() != "")
        {
            titleText.text = interfaceB.task.title.ToString();
        }
        //Debug.Log("UpdateTitle xx" + title.text);
    }

    public void MouseDown() 
    {
        interfaceB.isBlockingOperation = true;
    }

   /* public void MouseOut() 
    {
        interfaceB.isBlockingOperation = false;
    }*/
}
