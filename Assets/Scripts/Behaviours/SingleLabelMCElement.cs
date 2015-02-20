using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SBS.Math;
using fractionslab;
using fractionslab.meshes;
using fractionslab.utils;



public class SingleLabelMCElement : WSElement, IWSElement
{
    #region Public Fields
    public GameObject mcObj = null;
    #endregion

    #region Protected Fields
    protected string value = string.Empty;
    //protected string lastValue = string.Empty;
    protected float scale = 0.015f;
    protected float width = 0.0f;
    protected float height = 0.0f;
    protected GameObject root = null;
    #endregion

    #region Unity Callbacks
    void Awake()
    {
        root = transform.parent.gameObject;

        if(null == gameObject.GetComponent<Canvas>())
            gameObject.AddComponent<Canvas>();

        gameObject.GetComponent<Canvas>().worldCamera = Camera.main;

        if (mcObj == null)
        {
            mcObj = new GameObject("mc");
            mcObj.transform.parent = transform;
            mcObj.AddComponent<Text>();
        }

        float offsX = 0.5f * scale,
              offsY = 0.5f * scale;
        if (Application.platform == RuntimePlatform.OSXWebPlayer)
            offsX = offsY = 0.0f;
#if UNITY_IPHONE  || UNITY_ANDROID
        offsX = offsY = 0.0f;
#endif

        mcObj.transform.position = transform.TransformPoint(new Vector3(offsX, offsY, 0.0f));

    }
    #endregion

    #region Protected Methods
    #endregion

    #region Public Methods
    public override void Initialize()
    {
        mcObj.GetComponent<Text>().font = Workspace.Instance.fontInUse;
       // mcObj.GetComponent<Text>().material = new Material(Shader.Find("Transparent/DiffuseDoubeSided"));
        mcObj.GetComponent<RectTransform>().sizeDelta = new Vector2(50.0f, 50.0f);
        mcObj.GetComponent<Text>().fontSize = 50;
        mcObj.GetComponent<Text>().color = color;
        mcObj.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
        mcObj.transform.localScale = new Vector3(scale, scale, scale);
        base.Initialize();
    }

    public override void Draw(int zIndex)
    {
        
        Color meshColor = color;
        if (mode == InteractionMode.Freeze)
            meshColor += new Color(0.4f, 0.4f, 0.4f);
        if (null != mcObj)
        {          
            mcObj.GetComponent<Text>().color = meshColor;
            mcObj.GetComponent<Text>().text = value;
        }
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

    void SetValue(string val)
    {
        value = val;
    }
    #endregion
}
