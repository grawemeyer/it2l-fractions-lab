using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SBS.Math;
using fractionslab;
using fractionslab.meshes;
using fractionslab.utils;



public class SmallLabelMCElement : WSElement, IWSElement
{
    #region Public Fields
    public GameObject mcObj = null;
    #endregion

    #region Protected Fields
    protected string value = string.Empty;
    protected string lastValue = string.Empty;
    protected float scale = 0.0333334f;
 /*   [SerializeField]
    protected float width = 0.0f;
    [SerializeField]
    protected float height = 0.0f;*/
    protected GameObject root = null;
    #endregion

    #region Unity Callbacks
    void Awake()
    {
        root = transform.parent.gameObject;
        if (null == gameObject.GetComponent<Canvas>())
            gameObject.AddComponent<Canvas>();

        gameObject.GetComponent<Canvas>().worldCamera = Camera.main;
        if (mcObj == null)
        {
            mcObj = new GameObject("mc");
            mcObj.transform.parent = transform;
            mcObj.AddComponent<Text>();
        }
    }

    void Start()
    {

    }

    void Update()
    {
        if (value != lastValue)
        {
            mcObj.GetComponent<Text>().color = color;
            mcObj.GetComponent<Text>().text = value;
            lastValue = value;
        }
    }
    #endregion

    #region Protected Methods
    #endregion

    #region Public Methods
    public override void Initialize()
    {
        mcObj.GetComponent<Text>().font = Workspace.Instance.fontInUse;
        mcObj.GetComponent<RectTransform>().sizeDelta = new Vector2(31.0f, 20.0f);
        mcObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, 0.0f);
        mcObj.GetComponent<Text>().fontSize = 12;
        mcObj.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;

        gameObject.transform.localScale = new Vector3(scale, scale, scale);
        gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.55f, 2f);

        base.Initialize();

    }

    public override void Draw(int zIndex)
    {
        Color meshColor = color;

        if (mode == InteractionMode.Freeze)
            meshColor += new Color(0.4f, 0.4f, 0.4f);

        if (null != mcObj)
            mcObj.GetComponent<Text>().color = meshColor;
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
        /*Width = size.x;
        Height = size.y;*/
    }

    void SetValue(string val)
    {
        value = val;
    }
    #endregion
}
