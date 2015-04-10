using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SBS.Math;
using fractionslab.meshes;
using fractionslab.utils;
using fractionslab.behaviours;

public class LookAtFraction : MonoBehaviour
{
    #region Public Fields
    public bool isLookAtActive = false;
    #endregion

    #region Protected Fields
    protected InterfaceBehaviour interfaceB;
    protected Workspace workspace;
    protected GameObject fractionOnFocus;
    protected bool zoom = false;
    protected bool zoomOut = false;
    protected bool posCtrl = false;
    protected bool sizeCtrl = false;
    protected float newSize = 10;
    protected float startingSize = 10;
    protected Transform newPosition;
    protected Vector3 initialPosition;
    protected Vector3 finalPosition;
    protected Vector3 initialScale;
    protected Vector3 finalScale;
    protected Vector3 initialSymPos;
    protected Vector3 finalSymPos;
    protected Vector3 initialPartPos;
    protected Vector3 finalPartPos;
    protected float duration = 0.25f;
    protected float startTime = 0;
    protected float widthSize;
    protected float heightSize;
    protected float scaleFactorH = 1;
    protected float scaleFactorW = 1;
    protected GameObject symbol;
    protected GameObject partition;
    protected bool isFinished = false;
    #endregion

    #region Unity Callbacks
    void Start () {
        duration = 0.25f;
        workspace = GameObject.FindGameObjectWithTag("Workspace").GetComponent<Workspace>();
        initialPosition = this.transform.position;
        interfaceB = GameObject.FindGameObjectWithTag("Interface").GetComponent<InterfaceBehaviour>();
        widthSize = Camera.main.orthographicSize * 2.0f * Camera.main.aspect;
        heightSize = widthSize / Camera.main.aspect;
        isFinished = false;

	}

  /*  void Update()
    {
        if (zoom && null != fractionOnFocus)
        {
            transform.position = Vector3.Lerp(initialPosition, finalPosition, (Time.time - startTime) / duration);
            camera.orthographicSize = Mathf.Lerp(startingSize, newSize, (Time.time - startTime) / duration);
            symbol.GetComponent<RectTransform>().localScale = Vector3.Lerp(initialScale, finalScale, (Time.time - startTime) / duration);
            symbol.GetComponent<RectTransform>().localPosition = Vector3.Lerp(initialSymPos, finalSymPos, (Time.time - startTime) / duration);
            if (fractionOnFocus.GetComponent<RootElement>().PartitionActive && null != partition)
            {
                partition.GetComponent<RectTransform>().localScale = Vector3.Lerp(initialScale, finalScale, (Time.time - startTime) / duration);
                partition.GetComponent<RectTransform>().localPosition = Vector3.Lerp(initialPartPos, finalPartPos, (Time.time - startTime) / duration);
            }
            if (camera.orthographicSize == newSize && transform.position == finalPosition)
            {              
                zoom = false;
                isFinished = true;
            }
        }
    }*/
    #endregion


    #region Internal Utilities
   
    #endregion

    #region Messages
    public void StartLookAt() 
    {
       // Debug.Log("isFinished " + isFinished);
        if (isLookAtActive)
            return;
       
        fractionOnFocus = workspace.ElementOnFocus;

        if (null == fractionOnFocus)
            return;
        isLookAtActive = true;
        isFinished = false;
        foreach (BoxCollider bc in fractionOnFocus.GetComponents<BoxCollider>()) 
        {
            bc.enabled = false;
        }

        interfaceB.SendMessage("LookAtDisabled");
        fractionOnFocus.GetComponent<RootElement>().mode = InteractionMode.LookAt;
        fractionOnFocus.GetComponent<RootElement>().DetachButtons();

        workspace.SendMessage("SetElementVisibility", false);

       
        Vector2 sizeFraction = new Vector2(fractionOnFocus.GetComponent<RootElement>().width, fractionOnFocus.GetComponent<RootElement>().height);

        //scaleFactorW = (sizeFraction.x + 5.0f) / widthSize;
        scaleFactorW = sizeFraction.x / (widthSize - 7f);
        scaleFactorH = sizeFraction.y / (heightSize - 6f);
        float finalScaleFactor = Mathf.Max(Mathf.Max(scaleFactorW, scaleFactorH), (3f/10f));
        newSize = 10 * finalScaleFactor;
        float tmp = fractionOnFocus.GetComponentInChildren<BoxCollider>().center.x;
        float offsetX = 1.5f * finalScaleFactor;
        initialPosition = new Vector3(0.0f, 0.0f, -10.0f);
        finalPosition = new Vector3(fractionOnFocus.transform.position.x + tmp + offsetX, fractionOnFocus.transform.position.y + (finalScaleFactor * (0.8f)), -10);

        symbol = fractionOnFocus.GetComponent<RootElement>().symbol;
        initialScale = symbol.GetComponent<RectTransform>().localScale;
        finalScale = symbol.GetComponent<RectTransform>().localScale * finalScaleFactor;
        initialSymPos = symbol.GetComponent<RectTransform>().localPosition;
        finalSymPos = fractionOnFocus.GetComponent<RootElement>().GetSymbolPosition(false,  finalScaleFactor);
        if (fractionOnFocus.GetComponent<RootElement>().PartitionActive && null != fractionOnFocus.GetComponent<RootElement>().partMod)
        {
            partition = fractionOnFocus.GetComponent<RootElement>().partMod;
            initialPartPos = partition.GetComponent<RectTransform>().localPosition;
            finalPartPos = fractionOnFocus.GetComponent<RootElement>().GetSymbolPosition(true, finalScaleFactor);
        }
        startTime = Time.time;
        startingSize = camera.orthographicSize;
        if (null == fractionOnFocus)
            return;
        zoom = true;
       
    }


    public void ResetLookAt()
    {
        if (!isLookAtActive || null == fractionOnFocus)
            return;
        if (!isFinished)
            return;
        startingSize = camera.orthographicSize;
        newSize = 10;
       
        finalPosition = new Vector3 (0.0f, 0.0f, -10.0f);
        initialPosition = this.transform.position;
        initialScale = symbol.GetComponent<RectTransform>().localScale;
        finalScale = new Vector3(0.03f,0.03f,0.03f);

        initialSymPos = symbol.GetComponent<RectTransform>().localPosition;
        finalSymPos = fractionOnFocus.GetComponent<RootElement>().GetSymbolPosition(false, 1.0f);

        if (fractionOnFocus.GetComponent<RootElement>().PartitionActive && null != fractionOnFocus.GetComponent<RootElement>().partMod)
        {
            partition = fractionOnFocus.GetComponent<RootElement>().partMod;
            initialPartPos = partition.GetComponent<RectTransform>().localPosition;
            finalPartPos = fractionOnFocus.GetComponent<RootElement>().GetSymbolPosition(true, 1.0f);
        }

        startTime = Time.time;
        zoom = true;
        if (null == fractionOnFocus)
            return;
        StartCoroutine(EnableInputCoroutine());
       

    }

    
    IEnumerator EnableInputCoroutine() 
    {
        yield return new WaitForSeconds(duration);
        if (null != fractionOnFocus)
        {
            fractionOnFocus.GetComponent<RootElement>().mode = InteractionMode.Moving;
            interfaceB.SendMessage("LookAtEnabled");
            workspace.SendMessage("SetElementVisibility", true);
            foreach (BoxCollider bc in fractionOnFocus.GetComponents<BoxCollider>())
            {
                bc.enabled = true;
            }
            isLookAtActive = false;
        }
    }


    public void RapidResetLookAt()
    {
        if (!isLookAtActive || null == fractionOnFocus)
            return;
        if (!isFinished)
            return;
        newSize = 10;
        finalPosition = new Vector3(0.0f, 0.0f, -10.0f);
        transform.position = finalPosition;
        camera.orthographicSize = newSize;      
    }
    #endregion
}
