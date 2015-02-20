using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SBS.Math;
using fractionslab;
using fractionslab.meshes;
using fractionslab.utils;



public class PartitionMCElement : WSElement, IWSElement
{
    #region Public Fields
    public Text partitionsText;
    public GameObject btnpartitions;
    #endregion

    #region Protected Fields
    protected int lastPartitions = 0;
    protected float scale = 2.0f;
    //protected float width = 0.0f;
    //protected float height = 4.0f;
   // protected GameObject root = null;

    protected GameObject mcObj = null;
    #endregion

    #region Unity Callbacks
    void Awake()
    {
       /* mcObj = new GameObject("mc");
        mcObj.transform.parent = transform;*/

        float offsX = 0.5f * scale,
              offsY = 0.5f * scale;
        if (Application.platform == RuntimePlatform.OSXWebPlayer)
			offsX = offsY = 0.0f;
#if UNITY_IPHONE  || UNITY_ANDROID
        offsX = offsY = 0.0f;
#endif

       /* mcObj.transform.position = transform.TransformPoint(new Vector3(offsX, offsY, 0.0f));

        mcb = mcObj.AddComponent<MovieClipBehaviour>();
        mcb.swf = "Flash/i_talk_2_learn.swf";
        mcb.symbolName = "mcPartitionClass";
        mcb.movieClip = new MovieClip(mcb.swf + ":" + mcb.symbolName);*/
      

    }


    void Start()
    {
        /*mcObj.renderer.material = new Material(Shader.Find("Transparent/DiffuseDoubeSided"));
        mcObj.renderer.material.mainTexture = Resources.Load("Flash/i_talk_2_learn.swf/i_talk_2_learn.swf_Tex0") as Texture2D;
        mcb.Awake();

        mcObj.transform.localScale = new Vector3(scale, scale, scale);*/
        root = transform.parent.gameObject;
        partitionsText.color = new Color(0.2784f, 0.4510f, 0.1922f);
        partitionsText.text = root.GetComponent<RootElement>().partitions.ToString();
        partitions = root.GetComponent<RootElement>().partitions;
        lastPartitions = root.GetComponent<RootElement>().partitions;
        Initialize();
    }

    void Update()
    {
        if (partitions != lastPartitions)
        {
            partitionsText.color = new Color(0.2784f, 0.4510f, 0.1922f);
            partitionsText.text = partitions.ToString();
            lastPartitions = partitions;
        }
    }

    void OnGUI()
    {
       /* Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
		if (root.GetComponent<RootElement>().mode == InteractionMode.Freeze  || root.GetComponent<RootElement>().mode == InteractionMode.LookAt)
            return;
        if (!root.GetComponent<RootElement>().inputEnabled)
            return;
        if (GUI.Button(new Rect(screenPos.x - (10 * Screen.width / 800.0f), Screen.height - screenPos.y - (12 * Screen.height / 600.0f), 21.0f * Screen.width / 800.0f, 22.0f * Screen.height / 600.0f), ""))
        {
            if (root.GetComponent<RootElement>().mode != InteractionMode.LookAt)
            {
                Workspace.Instance.SendMessage("SetFocusOn", root);
                root.SendMessage("SetMode", InteractionMode.Partitioning);
                root.SendMessage("PlaceButtons");
            }
        }*/
    }
    #endregion

    #region Protected Methods

    void OnMouseDown()
    {
       // Debug.Log("SingleFaction onmousedown");
        root.GetComponent<RootElement>().inputByChild = true;
        root.GetComponent<RootElement>().OnMouseDown();
    }

    void OnMouseDrag()
    {
        root.GetComponent<RootElement>().OnMouseDrag();
    }

    void OnMouseUp()
    {    
        if (root.GetComponent<RootElement>().hasDragged)
            root.GetComponent<RootElement>().OnMouseUp();
        root.GetComponent<RootElement>().inputByChild = false;
    }
    #endregion

    #region Public Methods
    public void OnSelectPartitions()
    {
        if (root.GetComponent<RootElement>().mode == InteractionMode.Freeze || root.GetComponent<RootElement>().mode == InteractionMode.LookAt)
            return;
        if (!root.GetComponent<RootElement>().inputEnabled)
            return;
        if (root.GetComponent<RootElement>().mode != InteractionMode.LookAt)
        {
            //Debug.Log("onselectpartition");
            Workspace.Instance.SendMessage("SetFocusOn", root);
            root.SendMessage("SetMode", InteractionMode.Partitioning);
            root.SendMessage("PlaceButtons", 2);
            btnpartitions.SetActive(true);
        }
       
    }


    public void HideButtons()
    {
       // Debug.Log("HideButtons");
        btnpartitions.SetActive(false);

    }

    public void ShowButtons()
    {
        btnpartitions.SetActive(true);

    }

    public void ChangeStateButtons(bool isEnable) 
    {
        foreach(Button bt in btnpartitions.GetComponentsInChildren<Button>()) 
        {
            bt.interactable = isEnable;
            if (null != bt.gameObject.GetComponent<UIButton>())
            {
                if(!isEnable)
                    bt.gameObject.GetComponent<UIButton>().DisableBtn(false);
                else
                    bt.gameObject.GetComponent<UIButton>().EnableBtn(false);

            }
        }
    }

    public void OnClickArrowUp()
    {
        root.GetComponent<RootElement>().OnClickArrowUp(2);

    }

    public void OnClickArrowDown()
    {
        root.GetComponent<RootElement>().OnClickArrowDown(2);
    }


    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Draw(int zIndex)
    {
    }
    #endregion

    #region Messages
    void SetSize(Vector2 size)
    {
        Width = size.x;
        Height = size.y;
    }
    #endregion
}
