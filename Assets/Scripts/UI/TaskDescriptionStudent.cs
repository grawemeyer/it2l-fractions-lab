using UnityEngine;
using System.Collections;

public class TaskDescriptionStudent : MonoBehaviour {

    protected bool inputEnabled;
    protected bool isDragged;
    protected bool hasDragged;
    protected Vector3 deltaTouch;
    protected Vector3 initialMouseDownPos;
    protected Vector3 touchPos;
    Vector2 offset;
    protected const float deltaDrag = 10.0f;
    public InterfaceBehaviour interfaceB;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        inputEnabled = interfaceB.InputEnabled;
        if (Input.GetMouseButtonUp(0))
            interfaceB.isDragWindow = false;
	}

    public void OnMouseDown()
    {
        offset = (Input.mousePosition - GetComponent<RectTransform>().position);
        interfaceB.isDragWindow = true;
    }

    public void OnMouseDrag()
    {

        float hMargin = Camera.main.orthographicSize * Screen.width / Screen.height;
        float vMargin = Camera.main.orthographicSize;
        float marginOffset = -1.0f;

        float leftMargin = -(hMargin - marginOffset);
        float rightMargin = hMargin - marginOffset;
        float topMargin = vMargin;
        float bottomMargin = -vMargin;
       
        touchPos.x = Input.mousePosition.x - offset.x;
        touchPos.y = Input.mousePosition.y - offset.y;
       
        if (touchPos.x >= 0 && touchPos.x <= Screen.width && touchPos.y <= Screen.height && touchPos.y >= 0)
        {     
            GetComponent<RectTransform>().position = touchPos;
        }

     }


}
