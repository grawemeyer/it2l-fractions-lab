using UnityEngine;
using System.Collections;
using SBS.Math;
using fractionslab;
using fractionslab.meshes;
using fractionslab.utils;
using pumpkin.display;
using pumpkin.text;
using pumpkin.events;


public class SmallLabelMCElement : WSElement, IWSElement
{
    #region Public Fields
    #endregion

    #region Protected Fields
    protected string value = string.Empty;
    protected string lastValue = string.Empty;
    protected float scale = 2.0f;
    protected float width = 0.0f;
    protected float height = 0.0f;
    protected MovieClipBehaviour mcb = null;
    protected GameObject root = null;
    #endregion

    #region Unity Callbacks
    void Awake()
    {
        root = transform.parent.gameObject;
        /*mcb = gameObject.AddComponent<MovieClipBehaviour>();
        mcb.swf = "Flash/i_talk_2_learn.swf";
        mcb.symbolName = "mcSmallLabelClass";
        mcb.movieClip = new MovieClip(mcb.swf + ":" + mcb.symbolName);*/
    }

    void Start()
    {
        /*renderer.material = new Material(Shader.Find("Transparent/DiffuseDoubeSided"));
        renderer.material.mainTexture = Resources.Load("Flash/i_talk_2_learn.swf/i_talk_2_learn.swf_Tex0") as Texture2D;
        mcb.Awake();

        transform.localScale = new Vector3(scale, scale, scale);
        Initialize();*/
    }

    void Update()
    {
        if (value != lastValue)
        {
            mcb.movieClip.getChildByName<TextField>("tfValue").colorTransform = color;
            mcb.movieClip.getChildByName<TextField>("tfValue").text = value;
            lastValue = value;
        }
    }
    #endregion

    #region Protected Methods
    #endregion

    #region Public Methods
    public override void Initialize()
    {
        mcb = gameObject.AddComponent<MovieClipBehaviour>();
        mcb.swf = "Flash/i_talk_2_learn.swf";
        mcb.symbolName = "mcSmallLabelClass";
        mcb.movieClip = new MovieClip(mcb.swf + ":" + mcb.symbolName);

        renderer.material = new Material(Shader.Find("Transparent/DiffuseDoubeSided"));
        renderer.material.mainTexture = Resources.Load("Flash/i_talk_2_learn.swf/i_talk_2_learn.swf_Tex0") as Texture2D;
        mcb.Awake();

        transform.localScale = new Vector3(scale, scale, scale);

        base.Initialize();
    }

    public override void Draw(int zIndex)
    {
        Color meshColor = color;
        if (mode == InteractionMode.Freeze)
            meshColor += new Color(0.4f, 0.4f, 0.4f);

        if (null != mcb)
            mcb.movieClip.getChildByName<TextField>("tfValue").colorTransform = meshColor;
    }

    public override SBSBounds GetBounds()
    {
        //bounds = new SBSBounds(transform.position - Vector3.up * 1.4f, new SBSVector3(width, height, 0.0f));
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
