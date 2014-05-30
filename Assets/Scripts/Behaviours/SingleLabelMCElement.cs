using UnityEngine;
using System.Collections;
using SBS.Math;
using fractionslab;
using fractionslab.meshes;
using fractionslab.utils;
using pumpkin.display;
using pumpkin.text;
using pumpkin.events;


public class SingleLabelMCElement : WSElement, IWSElement
{
    #region Public Fields
    public GameObject mcObj = null;
    #endregion

    #region Protected Fields
    protected string value = string.Empty;
    //protected string lastValue = string.Empty;
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

        if (mcObj == null)
        {
            mcObj = new GameObject("mc");
            mcObj.transform.parent = transform;
            mcb = mcObj.AddComponent<MovieClipBehaviour>();
            mcb.swf = "Flash/i_talk_2_learn.swf";
            mcb.symbolName = "mcNumberClass";
            mcb.movieClip = new MovieClip(mcb.swf + ":" + mcb.symbolName);
        }
        else
        {
            mcb = mcObj.GetComponent<MovieClipBehaviour>();
        }

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
        mcb.symbolName = "mcNumberClass";
        mcb.movieClip = new MovieClip(mcb.swf + ":" + mcb.symbolName);*/
    }
    #endregion

    #region Protected Methods
    #endregion

    #region Public Methods
    public override void Initialize()
    {
        mcObj.renderer.material = new Material(Shader.Find("Transparent/DiffuseDoubeSided"));
        mcObj.renderer.material.mainTexture = Resources.Load("Flash/i_talk_2_learn.swf/i_talk_2_learn.swf_Tex0") as Texture2D;
        mcb.Awake();

        mcObj.transform.localScale = new Vector3(scale, scale, scale);

        base.Initialize();
    }

    public override void Draw(int zIndex)
    {
        Color meshColor = color;
        if (mode == InteractionMode.Freeze)
            meshColor += new Color(0.4f, 0.4f, 0.4f);

        if (null != mcb)
        {
            mcb.movieClip.getChildByName<MovieClip>("mcLabel").getChildByName<TextField>("tfValue").colorTransform = meshColor;
            mcb.movieClip.getChildByName<MovieClip>("mcLabel").getChildByName<TextField>("tfValue").text = value;
        }
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
