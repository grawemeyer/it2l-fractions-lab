using UnityEngine;
using System.Collections;
using SBS.Math;
using fractionslab;
using fractionslab.meshes;
using fractionslab.utils;
using pumpkin.display;
using pumpkin.text;
using pumpkin.events;


public class PartitionMCElement : WSElement, IWSElement
{
    #region Public Fields
    #endregion

    #region Protected Fields
    protected int lastPartitions = 0;
    protected float scale = 2.0f;
    protected float width = 0.0f;
    protected float height = 4.0f;
    protected MovieClipBehaviour mcb = null;
    protected GameObject root = null;

    protected GameObject mcObj = null;
    #endregion

    #region Unity Callbacks
    void Awake()
    {
        root = transform.parent.gameObject;

        mcObj = new GameObject("mc");
        mcObj.transform.parent = transform;

        float offsX = 0.5f * scale,
              offsY = 0.5f * scale;
        if (Application.platform == RuntimePlatform.OSXWebPlayer)
			offsX = offsY = 0.0f;
#if UNITY_IPHONE
		offsX = offsY = 0.0f;
#endif

        mcObj.transform.position = transform.TransformPoint(new Vector3(offsX, offsY, 0.0f));

        mcb = mcObj.AddComponent<MovieClipBehaviour>();
        mcb.swf = "Flash/i_talk_2_learn.swf";
        mcb.symbolName = "mcPartitionClass";
        mcb.movieClip = new MovieClip(mcb.swf + ":" + mcb.symbolName);
    }

    void Start()
    {
        mcObj.renderer.material = new Material(Shader.Find("Transparent/DiffuseDoubeSided"));
        mcObj.renderer.material.mainTexture = Resources.Load("Flash/i_talk_2_learn.swf/i_talk_2_learn.swf_Tex0") as Texture2D;
        mcb.Awake();

        mcObj.transform.localScale = new Vector3(scale, scale, scale);
        Initialize();
    }

    void Update()
    {
        if (partitions != lastPartitions)
        {
            mcb.movieClip.getChildByName<TextField>("tfValue1").colorTransform = new Color(0.2784f, 0.4510f, 0.1922f);
            mcb.movieClip.getChildByName<TextField>("tfValue1").text = partitions.ToString();
            lastPartitions = partitions;
        }
    }
    #endregion

    #region Protected Methods
    #endregion

    #region Public Methods
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
