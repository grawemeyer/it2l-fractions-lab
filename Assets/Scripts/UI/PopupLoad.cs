using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using fractionslab.utils;


public class PopupLoad : MonoBehaviour {

    public GameObject taskVoice;
    public GameObject scrollpanel;
    public GameObject scrollbar;
    public GameObject popupTaskDesc;
    protected List<Task> taskList;
    protected float yStart = -70;
    protected float yOffset = 150;
    protected string lipsum;
    
	// Use this for initialization
	void Start () {
        lipsum = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed sed auctor dolor. Vivamus nibh diam, rutrum quis mauris ac, lacinia lacinia sem. Vestibulum tincidunt turpis et odio ultrices porttitor. In hac habitasse platea dictumst. Aenean dolor magna, varius non vestibulum non, congue feugiat quam. Nullam sollicitudin lacinia lacus, in sagittis odio elementum vulputate. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Donec tristique nisl nec justo porta, a placerat massa sodales. In aliquam lorem justo, vitae ullamcorper nisi iaculis in. Suspendisse enim est, eleifend eget volutpat nec, laoreet in mauris. Nulla sollicitudin fringilla nibh, sed cursus nulla vehicula vitae. In ligula risus, efficitur ac orci nec, sagittis bibendum lacus. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Curabitur suscipit auctor nulla, ac ultrices velit euismod non.";
        GameObject task;
        taskList = new List<Task>();
        Task tmpTask = new Task();
        tmpTask.title = "FirstTask";
        tmpTask.description = "FirstTask description " + lipsum;
        tmpTask.taskState = TaskState.New;
        tmpTask.number = 1;
        taskList.Add(tmpTask);
        tmpTask.title = "SecondTask";
        tmpTask.description = "SecondTask description " + lipsum;
        tmpTask.taskState = TaskState.Visited;
        tmpTask.number = 2;
        taskList.Add(tmpTask);
        tmpTask.title = "ThirdTask";
        tmpTask.description = "ThirdTask description " + lipsum;
        tmpTask.taskState = TaskState.New;
        tmpTask.number = 3;
        taskList.Add(tmpTask);
        tmpTask.title = "FourthTask";
        tmpTask.description = "FourthTask description " + lipsum;
        tmpTask.taskState = TaskState.Locked;
        tmpTask.number = 4;
        taskList.Add(tmpTask);
        int j = 0;
        scrollpanel.GetComponent<RectTransform>().sizeDelta = new Vector2(scrollpanel.GetComponent<RectTransform>().sizeDelta.x, yOffset*taskList.Count);
        scrollbar.GetComponent<Scrollbar>().value = 1;
        foreach (Task ts in taskList) 
        {
            task = GameObject.Instantiate(taskVoice) as GameObject;
            task.GetComponent<TaskVoice>().Initialize(ts);
            task.transform.parent = scrollpanel.transform;
            task.transform.localPosition = new Vector3(-20.0f, yStart - (yOffset * (j++)), task.transform.localPosition.z);
            task.GetComponent<TaskVoice>().popupDescriptor = popupTaskDesc;
        }
	
	}
	
}
