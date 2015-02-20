using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using fractionslab.utils;

public class TaskVoice : MonoBehaviour {

    public Text title;
    public Text description;
    public TaskState state;
    public Text stateText;
    public Text number;
    public UIBitmapTextField bitmapNumber;
    public GameObject maskLock;
    public Button informationTask;
    public InterfaceBehaviour interfaceB;
    public GameObject popupDescriptor;


    protected const int maxStringCountDescr = 150;
    protected const int maxStringCountTitle = 39;
    protected Task task;

	// Use this for initialization
	void Start () {
        interfaceB = GameObject.FindGameObjectWithTag("Interface").GetComponent<InterfaceBehaviour>();
        bitmapNumber.UpdateBitmapText();
        gameObject.GetComponent<RectTransform>().localScale = new Vector3(1.0f,1.0f,1.0f);
	}
	
    public void OpenTaskDescription() 
    {
        interfaceB.PushPopup(popupDescriptor.name, false);
        popupDescriptor.GetComponent<PopupTaskDescriptor>().initialize(task.title, task.description, false); 
    }

    public void Initialize(Task _task) 
    {
        interfaceB = GameObject.FindGameObjectWithTag("Interface").GetComponent<InterfaceBehaviour>();
        //Debug.Log("prova " + interfaceB.name);
        task = _task;
        if (title.text.Length > maxStringCountTitle)
        {
            title.text = title.text.Substring(0, maxStringCountTitle) + "...";
        }
        else
        {
            title.text = _task.title;
        }

        if (_task.description.Length > maxStringCountDescr)
        {
            description.text = _task.description.Substring(0, maxStringCountDescr) + "...";
        }
        else 
        {
            description.text = _task.description;
        }
       
        state = _task.taskState;
        bitmapNumber.text = _task.number.ToString();
        switch (state) 
        { 
            case TaskState.New:
                this.GetComponent<Button>().interactable = true;
                maskLock.SetActive(false);
                informationTask.gameObject.SetActive(true);
                stateText.color = InterfaceBehaviour.Orange;
                interfaceB.localizationUtils.AddTranslationText(stateText, "{new_task}");
                break;
            case TaskState.Visited:
                this.GetComponent<Button>().interactable = true;
                maskLock.SetActive(false);
                informationTask.gameObject.SetActive(true);
                stateText.color = InterfaceBehaviour.ClearGreen;
                interfaceB.localizationUtils.AddTranslationText(stateText, "{visited_task}");
                break;
            case TaskState.Locked:
                this.GetComponent<Button>().interactable = false;
                stateText.color = InterfaceBehaviour.Grey;
                interfaceB.localizationUtils.AddTranslationText(stateText, "{locked_task}");
                maskLock.SetActive(true);
                informationTask.gameObject.SetActive(false);
                break;
        }

    }

    public void OpenTask() 
    {
        Debug.Log("opentask");
        interfaceB.taskRedirect("");
    }
}
