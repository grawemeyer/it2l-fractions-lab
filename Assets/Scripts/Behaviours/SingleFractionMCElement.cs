using UnityEngine;
using System.Collections;
using SBS.Math;
using fractionslab;
using fractionslab.meshes;
using fractionslab.utils;
using pumpkin.display;
using pumpkin.text;
using pumpkin.events;

public class SingleFractionMCElement : WSElement, IWSElement
{
    #region Public Fields
    public bool showNumbers = false;
    public GameObject mcObj = null;
    #endregion

    #region Protected Fields
    protected int lastNumerator = 0;
    protected int lastDenominator = 0;
    protected int lastPartNumerator = 0;
    protected int lastPartDenominator = 0;
    protected float scale = 2.0f;
    protected float width = 0.0f;
    protected float height = 4.0f;
    protected MovieClipBehaviour mcb = null;
    protected GameObject root = null;
    #endregion

    #region Unity Callbacks
    void Awake()
    {
        root = transform.parent.gameObject;

        //if(transform.parent.GetComponentInChildren<MovieClipBehaviour>() != null)
        //    DestroyImmediate(transform.parent.GetComponentInChildren<MovieClipBehaviour>().gameObject);

        if (mcObj == null)
        {
            mcObj = new GameObject("mc");
            mcb = mcObj.AddComponent<MovieClipBehaviour>();
            mcb.swf = "Flash/i_talk_2_learn.swf";
            mcb.symbolName = "mcSingleFractionClass";
            mcb.movieClip = new MovieClip(mcb.swf + ":" + mcb.symbolName);
        }
        else
        {
            mcb = mcObj.GetComponent<MovieClipBehaviour>();
        }
        
        mcObj.transform.parent = transform;

        float offsX = 0.5f * scale,
                offsY = 0.5f * scale;
        if (Application.platform == RuntimePlatform.OSXWebPlayer)
            offsX = offsY = 0.0f;
#if UNITY_IPHONE
	offsX = offsY = 0.0f;
#endif

        mcObj.transform.position = transform.TransformPoint(new Vector3(offsX, offsY, 0.0f));

        /*mcb = mcObj.AddComponent<MovieClipBehaviour>();
        mcb.swf = "Flash/i_talk_2_learn.swf";
        mcb.symbolName = "mcSingleFractionClass";
        mcb.movieClip = new MovieClip(mcb.swf + ":" + mcb.symbolName);*/
        
        BoxCollider coll = gameObject.AddComponent<BoxCollider>();
        coll.size = new Vector3(0.6f * scale, 1.15f * scale, 1.0f);
    }

    void Start()
    {
        mcObj.renderer.material = new Material(Shader.Find("Transparent/DiffuseDoubeSided"));
        mcObj.renderer.material.mainTexture = Resources.Load("Flash/i_talk_2_learn.swf/i_talk_2_learn.swf_Tex0") as Texture2D;
        mcb.Awake();

        mcObj.transform.localScale = new Vector3(scale, scale, scale);
        Initialize();

        mcb.movieClip.getChildByName<TextField>("tfValue1").colorTransform = new Color(0.2784f, 0.4510f, 0.1922f);
        mcb.movieClip.getChildByName<TextField>("tfValue2").colorTransform = new Color(0.2784f, 0.4510f, 0.1922f);

        mcb.movieClip.getChildByName<TextField>("tfValue1").visible = showNumbers;
        mcb.movieClip.getChildByName<TextField>("tfValue2").visible = showNumbers;

        mcb.movieClip.getChildByName<TextField>("tfValue1").text = numerator.ToString();
        
        UpdateNumerator();
        UpdateDenominator();
    }
    
    void Update()
    {
        if (numerator != lastNumerator || partNumerator != lastPartNumerator)
        {
            UpdateNumerator();
        }

        if (denominator != lastDenominator || partDenominator != lastPartDenominator)
        {
            UpdateDenominator();
        }
    }

    void OnGUI()
    {
        GUI.skin = Workspace.Instance.skin;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);

        if (root.GetComponent<RootElement>().mode == InteractionMode.Freeze)
            return;

        if (!root.GetComponent<RootElement>().inputEnabled)
            return;

		if (GUI.Button(new Rect(screenPos.x - (10 * Screen.width / 800.0f), Screen.height - screenPos.y - (24 * Screen.height / 600.0f), 21.0f * Screen.width / 800.0f, 22.0f * Screen.height / 600.0f), ""))
        {
            if (denominator > 0)
            {
                Workspace.Instance.SendMessage("SetFocusOn", root);
                root.SendMessage("SetMode", InteractionMode.Changing);
                root.SendMessage("OnSelectFractionPart", FractionPart.Numerator);
            }
        }

		if (GUI.Button(new Rect(screenPos.x - (10 * Screen.width / 800.0f), Screen.height - screenPos.y + (4 * Screen.height / 600.0f), 21.0f * Screen.width / 800.0f, 22.0f * Screen.height / 600.0f), ""))
        {
            Workspace.Instance.SendMessage("SetFocusOn", root);
            root.SendMessage("SetMode", InteractionMode.Changing);
            root.SendMessage("OnSelectFractionPart", FractionPart.Denominator);
        }
    }
    #endregion

    #region Protected Methods
    protected void UpdateNumerator()
    {
        if (partitions == 1)
        {
            mcb.movieClip.getChildByName<TextField>("tfValue1").text = numerator.ToString();
            lastNumerator = numerator;
        }

        mcb.movieClip.getChildByName<TextField>("tfValue1").text = partNumerator.ToString();
        lastPartNumerator = partNumerator;
    }

    protected void UpdateDenominator()
    {
        if (partitions == 1)
        {
            mcb.movieClip.getChildByName<TextField>("tfValue2").text = denominator.ToString();
            lastDenominator = denominator;
        }

        mcb.movieClip.getChildByName<TextField>("tfValue2").text = partDenominator.ToString();
        lastPartDenominator = partDenominator;
    }

    protected void OnSelectNumerator()
    {
        root.SendMessage("OnSelectFractionPart", FractionPart.Numerator);
    }

    protected void OnSelectDenominator()
    {
        root.SendMessage("OnSelectFractionPart", FractionPart.Denominator);
    }
    #endregion

    #region Public Methods
    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Draw(int zIndex)
    {
        //Update();
    }

    public override SBSBounds GetBounds()
    {
        bounds.Reset();
        return bounds;
    }
    #endregion

    #region Messages
    void SetSize(Vector2 size)
    {
        Width = size.x;
        Height = size.y;
    }

    void ShowFlashNumbers(bool flag)
    {
        showNumbers = flag;
        mcb.movieClip.getChildByName<TextField>("tfValue1").visible = flag;
        mcb.movieClip.getChildByName<TextField>("tfValue2").visible = flag;
    }

    void UpdatePartitionDenominator(int value)
    {
        partDenominator = value;
    }
    #endregion
}