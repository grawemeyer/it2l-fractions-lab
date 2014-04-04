using UnityEngine;
using System.Collections;
using SBS.Math;
using fractionslab;
using fractionslab.meshes;
using fractionslab.utils;
using pumpkin.display;
using pumpkin.text;
using pumpkin.events;

public class InteractiveMovieClipElement : WSElement, IWSElement
{
    #region Public Fields
    #endregion

    #region Protected Fields
    protected float scale = 2.0f;
    //protected float width = 0.0f;
    //protected float height = 4.0f;
    protected InteractiveMovieClipBehaviour mcb = null;
    protected MovieClip mcArrowUp = null;
    protected MovieClip mcArrowDown = null;
    protected GameObject root = null;
    protected bool isEnabled = true;

    protected GameObject mcObj = null;
    protected BoxCollider coll = null;
    protected bool sendMouseDown = false;
    #endregion

    #region Unity Callbacks
    void Awake()
    {
        root = transform.parent.parent.gameObject;

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

        mcb = mcObj.AddComponent<InteractiveMovieClipBehaviour>();
    }

    void OnGUI()
    {
        GUI.skin = Workspace.Instance.skin;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);

		if (GUI.Button(new Rect(screenPos.x - (10 * Screen.width / 800.0f), Screen.height - screenPos.y - (10 * Screen.height / 600.0f), 22.0f * Screen.width / 800.0f, 20.0f * Screen.height / 600.0f), ""))
        {
            OnClick();
        }
    }

    void Update()
    {
        if (mode == InteractionMode.Scaling)
        {
            Vector3 mPos = transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            mPos.z = bounds.center.z;
            if (bounds.ContainsPointXY(mPos))
            {
                if (Input.GetMouseButtonDown(0))
                {
					Debug.Log("SendMouseDown");
                    sendMouseDown = true;
                    root.SendMessage("OnPressScaleModifier", gameObject.name);
                }
            }
            if (Input.GetMouseButtonUp(0) && sendMouseDown)
            {
                sendMouseDown = false;
                root.SendMessage("OnReleaseScaleModifier", gameObject.name);
            }
        }
    }
    #endregion

    #region Public Methods
    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Draw(int zIndex)
    {
        base.Draw(zIndex);
    }

    public override SBSBounds GetBounds()
    {
        bounds = new SBSBounds(transform.position, new SBSVector3(width, height, 0.0f));
        return bounds;
    }
    #endregion

    #region Protected Methods
    protected void OnClick()
    {
        if (!isEnabled)
            return;

        switch (mcb.movieClip.name)
        {
            case("mcChangeValueUpClass"):
                root.SendMessage("OnClickArrowUp");
                break;
            case ("mcChangeValueDownClass"):
                root.SendMessage("OnClickArrowDown");
                break;
        }
    }
    #endregion

    #region Messages
    void SetupMovieclip(string mcPath)
    {
        string[] path = mcPath.Split(':');
        mcb.swf = path[0];
        mcb.symbolName = path[1];
        mcb.mouseInputCamera = Camera.main;
        mcObj.renderer.material = new Material(Shader.Find("Transparent/DiffuseDoubeSided"));
        mcObj.renderer.material.mainTexture = Resources.Load("Flash/i_talk_2_learn.swf/i_talk_2_learn.swf_Tex0") as Texture2D;
        mcb.enableKeyboard = false;
        mcb.multitouch = false;
        mcObj.transform.localScale = new Vector3(scale, scale, scale);
        mcb.movieClip = new MovieClip(mcPath);
        mcb.Awake();
        mcb.movieClip.name = path[1];
        SetupButton(mcb.movieClip);
    }

    void SetEnabled(bool enable)
    {
        isEnabled = enable;
        if (isEnabled)
            SetupButton(mcb.movieClip);
        else
            DisableButton(mcb.movieClip);
    }

    void SetSize(Vector2 size)
    {
        Width = size.x;
        Height = size.y;
        GetBounds();
    }
    #endregion

    #region Buttons Functions
    public void SetupButton(MovieClip mc)
    {
        mc.gotoAndStop("up");
        mc.addEventListener(MouseEvent.MOUSE_UP, OnBtnsClick);
        mc.addEventListener(MouseEvent.MOUSE_ENTER, OnBtnsEnter);
        mc.addEventListener(MouseEvent.MOUSE_LEAVE, OnBtnsLeave);
    }

    public void DisableButton(MovieClip mc, string text=null)
    {
        mc.removeEventListener(MouseEvent.MOUSE_UP, OnBtnsClick);
        mc.removeEventListener(MouseEvent.MOUSE_ENTER, OnBtnsEnter);
        mc.removeEventListener(MouseEvent.MOUSE_LEAVE, OnBtnsLeave);
        mc.gotoAndStop("gh");

        if (text != null)
            mc.getChildByName<TextField>("tfLabel").text = text;
    }

    public void RemoveAllEventFromButton(MovieClip m)
    {
        m.removeAllEventListeners(MouseEvent.MOUSE_DOWN);
        m.removeAllEventListeners(MouseEvent.MOUSE_ENTER);
        m.removeAllEventListeners(MouseEvent.MOUSE_LEAVE);
    }

    void Click(MovieClip mc)
    {
        Debug.Log("UP " + (mc.name));
    }

    void OnBtnsClick(CEvent evt)
    {
        MovieClip mc = evt.currentTarget as MovieClip;
        //Debug.Log("OnBtnsClick " + (mc.name));
        mc.gotoAndStop("dn");
        switch (mc.name)
        {

        }
    }

    void OnBtnsEnter(CEvent evt)
    {
        MovieClip mc = evt.currentTarget as MovieClip;
        mc.gotoAndStop("dn");
        switch (mc.name)
        {
        }
    }

    void OnBtnsLeave(CEvent evt)
    {
        MovieClip mc = evt.currentTarget as MovieClip;
        mc.gotoAndStop("dn");
        switch (mc.name)
        {
        }
    }
    #endregion
}
