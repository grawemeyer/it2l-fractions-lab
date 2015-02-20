using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIHUDPage : MonoBehaviour {

    public InterfaceBehaviour interfaceB;
    public GameObject toolTeacher;
    public List<Button> toolButton;
    public List<Button> OperationButton;
    public Button taskDescription;
    public GameObject toolMenu;
    public Text user;
    public UnityEngine.Sprite spriteEditUp;
    public UnityEngine.Sprite spriteUp;




	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	   /* if(!toolMenu.activeSelf)
            toolMenu.SetActive(true);*/
	}

    void Initialize() 
    {
        user.text = interfaceB.user;
        if (interfaceB.isStudent)
        {
            taskDescription.GetComponent<UIButton>().icons[0].sprite = spriteUp;
            taskDescription.GetComponent<UIButton>().icons[0].gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, 0.0f);
            taskDescription.GetComponent<UIButton>().icons[0].gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2( 35.0f, 45.0f);
            toolTeacher.SetActive(false);
            user.color = InterfaceBehaviour.ClearGreen;
           
        }
        else
        {
            taskDescription.GetComponent<UIButton>().icons[0].sprite = spriteEditUp;
            taskDescription.GetComponent<UIButton>().icons[0].gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-4.6f, 0.0f);
            taskDescription.GetComponent<UIButton>().icons[0].gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(48.0f, 47.0f);

            toolTeacher.SetActive(true);
            user.color = InterfaceBehaviour.Orange;
        }
        
    }
}
