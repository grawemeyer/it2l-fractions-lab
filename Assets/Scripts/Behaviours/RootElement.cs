using UnityEngine;
using SBS.Math;
using fractionslab;
using fractionslab.behaviours;
using fractionslab.utils;
using fractionslab.meshes;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;


public class RootElement : WSElement, IWSElement
{
    #region Public Fields
    public GameObject parentRef = null;
    public List<GameObject> elements = new List<GameObject>();
    public BBExtend bbExtends = new BBExtend(0.0f,- 0.0f, 0.0f, 0.0f);
    public GameObject symbol_root;
    public GameObject symbol;
    protected bool isSymbolShown = false;
    #endregion

    #region Protected Fields
    protected float lastElementScale = 1.0f;
    protected Vector3 deltaTouch;
    protected Vector3 touchPos;
    //protected List<GameObject> elements = new List<GameObject>();
    protected Vector3 lastStillPosition;
    protected FractionPart selectedFractionPart = FractionPart.Denominator;
    protected GameObject btnUp;
    protected GameObject btnDown;
    protected GameObject partMod;
    protected GameObject scaleModParent;
    protected GameObject[] scaleMod;
    protected GameObject scaleLinesParent;
    protected GameObject[] scaleLines;
    protected InteractionMode lastMode = InteractionMode.Moving;
    protected Camera mainCamera;

    protected Vector2 clampHPosition = new Vector2(0.0f, Screen.width);
    protected Vector2 clampVPosition = new Vector2(0.0f, Screen.height);
    protected Vector2 HPositionOffset = new Vector2(5.0f, -5.0f);
    protected Vector2 VPositionOffset = new Vector2(5.0f, -5.0f);
    protected Vector2 lastClampedValue;

    protected bool isTweening = false;
    protected List<float> tweenScaleQueue = new List<float>();

    protected bool isScaling = false;
    protected float initialScale = 1.0f;
    protected Vector3 initialMousePos = Vector3.zero;
    protected Vector3 scaleDirection = Vector3.up;

    protected bool isParentLineShown = false;

	protected float doubleTapTimer = -1.0f;
    #endregion

    #region Public Getters
    public bool IsScaling
    {
        get
        {
            return isScaling;
        }
    }
    #endregion

    #region Unity Callbacks
    void Awake()
    {
        if (gameObject.GetComponent<BoxCollider>() == null)
            collider = gameObject.AddComponent<BoxCollider>();
        else
            collider = gameObject.GetComponent<BoxCollider>();

        collider.isTrigger = true;
        lastStillPosition = gameObject.transform.position;

        DestroyFindParentLine();
    }

    void OnMouseDown()
    {
        OnMouseDownEvent();
    }

    void OnMouseDownEvent()
    {
        deltaTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        if (!inputEnabled)
            return;

        if (mode == InteractionMode.Moving)
        {
            string typeString = "HRects";
            switch (type)
            {
                case (ElementsType.HRect):
                    typeString = "HRects";
                    break;
                case (ElementsType.VRect):
                    typeString = "VRects";
                    break;
                case (ElementsType.Line):
                    typeString = "NumberedLines";
                    break;
                case (ElementsType.Liquid):
                    typeString = "LiquidMeasures";
                    break;
            }

            if (state == ElementsState.Cut)
                ExternalEventsManager.Instance.SendMessageToSupport("PressCut", typeString, name, partNumerator + "/" + partDenominator, "(" + transform.position.x + ", " + transform.position.y + ")");
            else
                ExternalEventsManager.Instance.SendMessageToSupport("PressFraction", typeString, name, partNumerator + "/" + partDenominator, "(" + transform.position.x + ", " + transform.position.y + ")");
        }

        //Draw(zIndex);
        /*clampHPosition.x = Input.mousePosition.x - Camera.main.WorldToScreenPoint(bounds.min).x;
        clampHPosition.y = Screen.width - (Camera.main.WorldToScreenPoint(bounds.max).x - Input.mousePosition.x);
        clampVPosition.x = Input.mousePosition.y - Camera.main.WorldToScreenPoint(bounds.min).y;
        clampVPosition.y = Screen.height - (Camera.main.WorldToScreenPoint(bounds.max).y - Input.mousePosition.y);*/
        
        if (mode == InteractionMode.Initializing)
        {
            Workspace.Instance.SendMessage("SetFocusOn", gameObject);
            Draw(zIndex);
        }
        else if (mode == InteractionMode.Moving)
        {
            if (Workspace.Instance.OperationPending && state == ElementsState.Cut)
            {
                if (gameObject != Workspace.Instance.FirstCut)
                {
                    mode = InteractionMode.Wait;
                    Workspace.Instance.SendMessage("SetFocusOn", gameObject);
                    Workspace.Instance.SendMessage("SetSecondOperationCut", gameObject);
                    Workspace.Instance.SendMessage("StartCurrentAction");
                }
            }
            else
            {
                Workspace.Instance.SendMessage("CancelOperation");
                Workspace.Instance.SendMessage("SetFocusOn", gameObject);
                if (denominator == 0 && state != ElementsState.Improper)
                    SetMode(InteractionMode.Changing);
            }
        }
        else if (mode == InteractionMode.Changing)
        {
            BroadcastMessage("OnClicked", Camera.main.ScreenToWorldPoint(Input.mousePosition), SendMessageOptions.DontRequireReceiver);
        }
        else if (mode == InteractionMode.Freeze)
        {
            Workspace.Instance.SendMessage("CancelOperation");
        }

        if (mode != InteractionMode.Freeze && mode != InteractionMode.Wait)
            Workspace.Instance.interfaces.SendMessage("OnElementClicked");

        //if (gameObject.transform.position.y < 2.0f)
        lastStillPosition = gameObject.transform.position;

        DestroyFindParentLine();
        Workspace.Instance.interfaces.SendMessage("ShowHint", "");

        if (!Workspace.Instance.OperationPending)
        {
            //GrabToCenter();
        }
    }

    void OnMouseDrag()
    {
        OnMouseDragEvent();
    }

    void OnMouseDragEvent()
    {
        if (!inputEnabled)
            return;
        
        if (mode == InteractionMode.Moving || mode == InteractionMode.Initializing)
        {
            float hMargin = Camera.main.orthographicSize * Screen.width / Screen.height;
            float vMargin = Camera.main.orthographicSize;
            float marginOffset = 1.0f;

            float leftMargin = -(hMargin + width * 0.5f - marginOffset);
            float rightMargin = hMargin + width * 0.5f - marginOffset;
            float topMargin = vMargin + height  * 0.5f - marginOffset;
            float bottomMargin = -(vMargin + height * 0.5f - marginOffset);

            Vector3 clampedMousePos = Input.mousePosition;
            
			if (Input.mousePosition.x > 220.0f * Screen.width / 800.0f && Input.mousePosition.x < 580.0f * Screen.width / 800.0f)
                clampedMousePos.y = Mathf.Clamp(Input.mousePosition.y, 0.0f, Screen.height - (100.0f * Screen.height / 600.0f));

            Vector3 currentTouch = Camera.main.ScreenToWorldPoint(clampedMousePos);
            touchPos = currentTouch - deltaTouch;

            if (touchPos.x >= leftMargin && touchPos.x <= rightMargin && touchPos.y <= topMargin && touchPos.y >= bottomMargin)
            {
                transform.position = touchPos;
            }

            Vector3 tmpPos = transform.position;
            tmpPos.z = zIndex;
            transform.position = tmpPos;

            /*if (Input.mousePosition.y < 110.0f * Screen.height / 600.0f)
                clampedMousePos.x = Mathf.Clamp(Input.mousePosition.x, 0.0f, Screen.width);
            else
                clampedMousePos.x = Mathf.Clamp(Input.mousePosition.x, 0.0f, Screen.width);

			if (Input.mousePosition.x < 220.0f * Screen.width / 800.0f)
                clampedMousePos.y = Mathf.Clamp(Input.mousePosition.y, 0.0f, Screen.height);
			else if (Input.mousePosition.x < 580.0f * Screen.width / 800.0f)
                clampedMousePos.y = Mathf.Clamp(Input.mousePosition.y, 0.0f, Screen.height - 100.0f);
            else
                clampedMousePos.y = Mathf.Clamp(Input.mousePosition.y, 0.0f, Screen.height);

            Vector3 currentTouch = Camera.main.ScreenToWorldPoint(clampedMousePos);
            touchPos = currentTouch - deltaTouch;
            transform.position = touchPos;

            Vector3 pos = transform.position;
            pos.z = zIndex;
            transform.position = pos;*/
        }
    }

    void OnMouseOver()
    {
        OnMouseOverEvent();
    }

    void OnMouseOverEvent()
    {
        if (!inputEnabled)
            return;

        if (mode == InteractionMode.Moving && !Input.GetMouseButton(0))
        {
            Workspace.Instance.interfaces.SendMessage("ShowHint", "{hint_onshape}");
        }

		if (denominator > 0 && Input.GetMouseButtonUp(1) && mode != InteractionMode.Freeze && mode != InteractionMode.Wait)
        {
			RightClick();
        }
    }

	void RightClick()
    {
        if (mode == InteractionMode.Moving)
        {
            string typeString = "HRects";
            switch (type)
            {
                case (ElementsType.HRect):
                    typeString = "HRects";
                    break;
                case (ElementsType.VRect):
                    typeString = "VRects";
                    break;
                case (ElementsType.Line):
                    typeString = "NumberedLines";
                    break;
                case (ElementsType.Liquid):
                    typeString = "LiquidMeasures";
                    break;
            }

            if (state == ElementsState.Cut)
                ExternalEventsManager.Instance.SendMessageToSupport("SecActionCut", typeString, name, partNumerator + "/" + partDenominator, "(" + transform.position.x + ", " + transform.position.y + ")");
            else
                ExternalEventsManager.Instance.SendMessageToSupport("SecActionFraction", typeString, name, partNumerator + "/" + partDenominator, "(" + transform.position.x + ", " + transform.position.y + ")");
        }

		Workspace.Instance.SendMessage("SetFocusOn", gameObject);
		Workspace.Instance.interfaces.SendMessage("ShowHint", "");
		mode = InteractionMode.Moving;
		BroadcastMessage("SetMode", InteractionMode.Moving);
		
		if (state == ElementsState.Fraction || state == ElementsState.Result)
			Workspace.Instance.interfaces.SendMessage("OnShowContextMenu", gameObject);
		else if (state == ElementsState.Cut)
			Workspace.Instance.interfaces.SendMessage("OnShowActionsMenu", gameObject);
		
		if (state != ElementsState.Cut)
            Workspace.Instance.interfaces.SendMessage("OnElementReleased", gameObject);
	}

    void OnMouseExit()
    {
        OnMouseExitEvent();
    }

    void OnMouseExitEvent()
    {
        if (!inputEnabled)
            return;
        Workspace.Instance.interfaces.SendMessage("ShowHint", "");
    }

    void OnMouseUp()
    {
        OnMouseUpEvent();
    }

    void OnMouseUpEvent()
    {
        if (mode == InteractionMode.Moving)
        {
            string typeString = "HRects";
            switch (type)
            {
                case (ElementsType.HRect):
                    typeString = "HRects";
                    break;
                case (ElementsType.VRect):
                    typeString = "VRects";
                    break;
                case (ElementsType.Line):
                    typeString = "NumberedLines";
                    break;
                case (ElementsType.Liquid):
                    typeString = "LiquidMeasures";
                    break;
            }

            if (state == ElementsState.Cut)
                ExternalEventsManager.Instance.SendMessageToSupport("ReleaseCut", typeString, name, partNumerator + "/" + partDenominator, "(" + transform.position.x + ", " + transform.position.y + ")");
            else
                ExternalEventsManager.Instance.SendMessageToSupport("ReleaseFraction", typeString, name, partNumerator + "/" + partDenominator, "(" + transform.position.x + ", " + transform.position.y + ")");
        }

        Workspace.Instance.interfaces.SendMessage("OnElementReleased", gameObject);

#if UNITY_IPHONE
		if (doubleTapTimer < 0.0)
		{
			doubleTapTimer = Time.time;
		}
		else if (Time.time - doubleTapTimer < 0.4f)
		{
			doubleTapTimer = Time.time;
			if (denominator > 0 && mode == InteractionMode.Moving)
			{
				RightClick();
			}
		}
		else
		{
			doubleTapTimer = Time.time;
		}
#endif
    }

    void Update()
    {
        if (mode != lastMode)
        {
            if (mode == InteractionMode.Moving)
            {
                DetachModifierScale();
                DetachModifierSymbol();
                DetachModifierPartition();
            }
            else if (mode == InteractionMode.Initializing)
            {
                DetachModifierScale();
                DetachModifierPartition();
                AttachModifierSymbol();
            }
            else if (mode == InteractionMode.Changing || mode == InteractionMode.Initializing)
            {
                DetachModifierScale();
                DetachModifierPartition();
                AttachModifierSymbol();
            }
            else if (mode == InteractionMode.Partitioning)
            {
                DetachModifierScale();
                DetachModifierSymbol();
                AttachModifierPartition();
            }
            else if (mode == InteractionMode.Scaling)
            {
                DetachModifierSymbol();
                DetachModifierPartition();
                AttachModifierScale();
            }
            else if (mode == InteractionMode.Freeze)
            {
                BroadcastMessage("Freeze", SendMessageOptions.DontRequireReceiver);
            }

            if (lastMode == InteractionMode.Freeze)
            {
                BroadcastMessage("UnFreeze", SendMessageOptions.DontRequireReceiver);
            }

            lastMode = mode;
        }

        if (!inputEnabled)
            return;

        if (mode == InteractionMode.Moving)
            CheckScaleTween();

        if (mode == InteractionMode.Scaling && isScaling)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 diff = mousePos - initialMousePos;
            float diffGap = Vector3.Dot(diff, scaleDirection);
            SetElementScale(initialScale + diffGap * 0.282f);
        }
    }
    #endregion

    #region Public Methods
    public override void SetElementScale(float scale)
    {
        base.SetElementScale(scale);
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).SendMessage("SetElementScale", scale, SendMessageOptions.DontRequireReceiver);
        Draw(zIndex);
    }

    public override SBSBounds GetBounds()
    {
        bounds.Reset();
        for (int i = 0; i < transform.childCount; i++)
        {
             bounds.Encapsulate(transform.GetChild(i).GetComponent<WSElement>().GetBounds());
        }

        bounds.min -= new SBSVector3(bbExtends.left, bbExtends.bottom, 0.0f);
        bounds.max += new SBSVector3(bbExtends.right, bbExtends.top, 0.0f);

        return bounds;
    }

    public override void UpdateCollider(SBSBounds bounds)
    {
        SBSVector3 pos = transform.position;
        collider.center = ((bounds.max + bounds.min) * 0.5f) - pos;
        if (state == ElementsState.Cut || state == ElementsState.Result)
        {
            collider.size = (bounds.max - bounds.min);
        }
        else
        {
            collider.size = (bounds.max - bounds.min);
        }
    }

    public override void Draw(int zIndex)
    {
        base.Draw(zIndex);

        if (mode == InteractionMode.Changing && zIndex > 0)
        {
            mode = InteractionMode.Moving;
            BroadcastMessage("SetMode", mode);
        }

        Vector3 pos = transform.position;
        pos.z = zIndex;
        transform.position = pos;

        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).SendMessage("Draw", zIndex, SendMessageOptions.DontRequireReceiver);

        UpdateCollider(GetBounds());
        if (state == ElementsState.Cut || state == ElementsState.Result)
            UpdatePosSizeOnBB();

        if (mode == InteractionMode.Scaling)
        {
            if (isSymbolShown)
            {
                Vector3 loacalPos = GetSymbolPosition(false);
                symbol.transform.position = transform.TransformPoint(new Vector3(loacalPos.x, loacalPos.y, loacalPos.z));
            }
            PlaceModifierScale();
        }
    }

    public override void SetMode(InteractionMode mode)
    {
        base.SetMode(mode);
    }

    public override bool CheckCut()
    {
        return (partNumerator > 0);
    }

    public override bool CheckPartition()
    {
        if (denominator == 0)
            return false;

        for (int i = 0; i < transform.childCount; i++)
        {
            if (!transform.GetChild(i).GetComponent<WSElement>().CheckPartition())
                return false;
        }
        return true;
    }

    public void Cut()
    {
        isSymbolShown = false;
        DetachModifierSymbol();

        for (int i = 0; i < elements.Count; i++)
            elements[i].SendMessage("Cut", SendMessageOptions.DontRequireReceiver);

        switch (type)
        {
            case (ElementsType.HRect):
                ExternalEventsManager.Instance.SendMessageToSupport("CutGenerated", "HRects", name);
                break;
            case (ElementsType.VRect):
                ExternalEventsManager.Instance.SendMessageToSupport("CutGenerated", "HRects", name);
                break;
            case (ElementsType.Liquid):
                ExternalEventsManager.Instance.SendMessageToSupport("CutGenerated", "LiquidMeasures", name);
                break;
            case (ElementsType.Line):
                ExternalEventsManager.Instance.SendMessageToSupport("CutGenerated", "NumberedLines", name);
                break;
        }
    }

    public void UpdateGraphics()
    {
        StartCoroutine(UpdateCoroutine());
    }

    IEnumerator UpdateCoroutine()
    {
        yield return new WaitForEndOfFrame();
        GetBounds();
        Draw(zIndex);
    }

    public void UpdatePosSizeOnBB()
    {
        Vector3 offset = Vector3.zero;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform childTr = transform.GetChild(i);
            offset = collider.center;
            childTr.position -= offset;
            collider.center = Vector3.zero;
        }
        transform.position += offset;

        width = collider.size.x;
        height = collider.size.y;
    }
    #endregion

    #region Messages
    void InitializeAs(ElementsType elemType)
    {
        type = elemType;
        switch (type)
        {
            case (ElementsType.VRect):
            case (ElementsType.HRect):
                InitRectangle(type);
                break;
            case (ElementsType.Line):
                InitLine(type);
                break;
            case (ElementsType.Liquid):
                InitLiquid(type);
                break;
        }
    }

    void SetEnableCollider(bool flag)
    {
        collider.enabled = flag;
    }

    void ResetLastPosition()
    {
        gameObject.transform.position = lastStillPosition;
    }

    void OnSelectFractionPart(FractionPart part)
    {

        selectedFractionPart = part;
        PlaceButtons();
    }

    void OnClickArrowUp()
    {
        if (mode == InteractionMode.Changing || mode == InteractionMode.Initializing)
        {
            Workspace.Instance.interfaces.SendMessage("ShowSuggestion", "");
            if (denominator == 0)
            {
                mode = InteractionMode.Changing;
                Workspace.Instance.interfaces.SendMessage("EnableHUD");
            }

            if (selectedFractionPart == FractionPart.Numerator)
                BroadcastMessage("IncreaseNumerator");
            else if (selectedFractionPart == FractionPart.Denominator)
                BroadcastMessage("IncreaseDenominator");
        }
        else if (mode == InteractionMode.Partitioning)
        {
            BroadcastMessage("IncreasePartitions");
        }

        Draw(zIndex);
    }

    void OnClickArrowDown()
    {
        if (mode == InteractionMode.Changing || mode == InteractionMode.Initializing)
        {
            if (mode == InteractionMode.Initializing)
                Workspace.Instance.interfaces.SendMessage("ShowSuggestion", "");

            if (selectedFractionPart == FractionPart.Numerator)
                BroadcastMessage("DecreaseNumerator");
            else if (selectedFractionPart == FractionPart.Denominator)
                BroadcastMessage("DecreaseDenominator");
        }
        else if (mode == InteractionMode.Partitioning)
        {
            BroadcastMessage("DecreasePartitions");
        }

        Draw(zIndex);
    }

    void ToggleSymbol()
    {
        isSymbolShown = !isSymbolShown;
        if (isSymbolShown)
            AttachSymbol(true);
        else
            DetachSymbol();
    }

    void ScaleDown()
    {
        if (mode == InteractionMode.Moving)
            tweenScaleQueue.Add(0.25f);
    }

    void ScaleUp()
    {
        if (mode == InteractionMode.Moving)
            tweenScaleQueue.Add(1.0f);
    }

    void CheckScaleTween()
    {
        if (!isTweening && tweenScaleQueue.Count > 0)
        {
            float scaleTo = tweenScaleQueue[0];
            Tweener.CreateNewTween(transform.localScale, new Vector3(scaleTo, scaleTo, scaleTo), 0.25f, "easeOutCubic", 0.0f, TweenScaleS, TweenScaleU, TweenScaleC);
            tweenScaleQueue.RemoveAt(0);
            
            deltaTouch = Vector3.zero;
            OnMouseDrag();
        }
    }

    void GrabToCenter()
    {
        if (!inputEnabled)
            return;

        Tweener.CreateNewTween(deltaTouch, Vector3.zero, 0.1f, "easeOutCubic", 0.0f, TweenMoveS, TweenMoveU, TweenMoveC);
    }

    void SetBBExtends(BBExtend bbe)
    {
        bbExtends = bbe;
    }

    void OnPressScaleModifier(string modfierName)
    {
        Debug.Log("OnPressScaleModifier " + modfierName);
        isScaling = true;
        initialMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        initialScale = elementScale;

        switch(modfierName)
        {
            case ("square0"):
                scaleDirection = (Vector3.up + Vector3.left).normalized;
                break;
            case ("square1"):
                scaleDirection = (Vector3.up + Vector3.right).normalized;
                break;
            case ("square2"):
                scaleDirection = (Vector3.down + Vector3.left).normalized;
                break;
            case ("square3"):
                scaleDirection = (Vector3.down + Vector3.right).normalized;
                break;
            default:
                scaleDirection = Vector3.up;
                break;
        }
    }

    void OnReleaseScaleModifier(string modfierName)
    {
        Debug.Log("OnReleaseScaleModifier " + modfierName);
        isScaling = false;
    }

    void CutFraction()
    {
        Workspace.Instance.SendMessage("CutFraction", gameObject);
    }

    void Copy()
    {
        Workspace.Instance.SendMessage("CreateCopy", gameObject);
    }

    void FindParent()
    {
        if (null != parentRef)
        {
            Vector3[] topList = { transform.position, parentRef.transform.position };
            topList[1].z = 0.1f;

            DestroyFindParentLine();
            GameObject pLine = new GameObject("parent_line");
            pLine.transform.position = Vector3.zero + Vector3.forward * 0.1f;
            MeshLineElement cLine = pLine.AddComponent<MeshLineElement>();
            cLine.lineWidth = 0.06f;
            cLine.pointsList = topList;
            cLine.isClosed = false;
            cLine.color = Workspace.Instance.greenResult;
            cLine.Initialize();
            isParentLineShown = true;

            ExternalEventsManager.Instance.SendMessageToSupport("FindParent", name);
        }
    }

    void DestroyFindParentLine()
    {
        //if (isParentLineShown)
        //{
            GameObject pLine = GameObject.Find("parent_line");
            if (null != pLine)
            {
                Destroy(pLine);
                pLine = null;
            }
        //}
    }

    public void ResetScale()
    {
        elementScale = 1.0f;
        BroadcastMessage("SetElementScale", 1.0f);
        Draw(zIndex);
    }

    protected Vector3 cutTargetPos = Vector3.zero;
    void MoveToPopup(float time)
    {
        //BroadcastMessage("SetElementScale", 1.0f);
        //Draw(zIndex);

        cutTargetPos = Workspace.Instance.GetCurrentActionPosition(gameObject);
        if (time > 0)
        {
            Tweener.CreateNewTween(transform.position, cutTargetPos, time, "easeOutCubic", 0.0f, TweenMoveToCenterS, TweenMoveToCenterU, TweenMoveToCenterC);
        }
        else
        {
            transform.position = cutTargetPos;
            Tweener.StopAndDestroyAllTweens();
            isTweening = false;
        }
    }

    void MoveToCenter(float time)
    {
        //BroadcastMessage("SetElementScale", 1.0f);
        //Draw(zIndex);

        cutTargetPos = Vector3.zero;
        if (time > 0)
        {
            Tweener.CreateNewTween(transform.position, cutTargetPos, time, "easeOutCubic", 0.0f, TweenMoveToCenterS, TweenMoveToCenterU, TweenMoveToCenterC);
        }
        else
        {
            transform.position = cutTargetPos;
            Tweener.StopAndDestroyAllTweens();
            isTweening = false;
        }
    }

    protected Vector3 popupCoord = Vector3.zero;
    void SetCoord(Vector3 coord)
    {
        popupCoord = coord;
    }

    void MoveToCoord(float time)
    {
        //BroadcastMessage("SetElementScale", 1.0f);
        //Draw(zIndex);

        cutTargetPos = popupCoord;
        if (time > 0)
        {
            Tweener.CreateNewTween(transform.position, cutTargetPos, time, "easeOutCubic", 0.0f, TweenMoveToCenterS, TweenMoveToCenterU, TweenMoveToCenterC);
        }
        else
        {
            transform.position = cutTargetPos;
            Tweener.StopAndDestroyAllTweens();
            isTweening = false;
        }
    }
    #endregion

    #region Protected Methods
    #region TweenCallbacks
    protected void TweenScaleS(object v)
    {
        isTweening = true;
    }
    protected void TweenScaleU(object v)
    {
        transform.localScale = (Vector3)v;
    }
    protected void TweenScaleC(object v)
    {
        Tweener.StopAndDestroyAllTweens();
        isTweening = false;
    }
    protected void TweenMoveS(object v)
    {
        isTweening = true;
    }
    protected void TweenMoveU(object v)
    {
        deltaTouch = (Vector3)v;
        OnMouseDrag();
    }
    protected void TweenMoveC(object v)
    {
        Tweener.StopAndDestroyAllTweens();
        isTweening = false;
    }
    protected void TweenMoveToCenterS(object v)
    {
        isTweening = true;
    }
    protected void TweenMoveToCenterU(object v)
    {
        transform.position = (Vector3)v;
    }
    protected void TweenMoveToCenterC(object v)
    {
        transform.position = cutTargetPos;
        Tweener.StopAndDestroyAllTweens();
        isTweening = false;
    }
    #endregion

    protected void InitRectangle(ElementsType type)
    {
        width = 6.0f;
        height = 4.0f;

        GameObject elem = new GameObject("fraction");
        elem.transform.parent = transform;
        elem.transform.position = transform.TransformPoint(Vector3.zero);
        elem.AddComponent<RectangleElement>();
        elem.SendMessage("SetMode", mode);
        elem.SendMessage("SetType", type);
        elem.SendMessage("SetSize", new Vector2(width, height));
        elem.SendMessage("SetColor", color);
        elem.SendMessage("SetElementState", state);
        elem.SendMessage("SetNumerator", numerator);
        elem.SendMessage("SetDenominator", denominator);
        elem.SendMessage("SetPartitions", partitions);
        elem.SendMessage("SetPartNumerator", partNumerator);
        elem.SendMessage("SetPartDenominator", partDenominator);
        elem.SendMessage("Initialize");
        elements.Add(elem);
    }

    protected void InitLine(ElementsType type)
    {
        width = 7.0f;
        height = 0.07f;

        GameObject elem = new GameObject("fraction");
        elem.transform.parent = transform;
        elem.transform.position = transform.TransformPoint(Vector3.zero);
        elem.AddComponent<LineElement>();
        elem.SendMessage("SetMode", mode);
        elem.SendMessage("SetType", type);
        elem.SendMessage("SetSize", new Vector2(width, height));
        elem.SendMessage("SetColor", color);
        elem.SendMessage("SetElementState", state);
        elem.SendMessage("SetNumerator", numerator);
        elem.SendMessage("SetDenominator", denominator);
        elem.SendMessage("SetPartitions", partitions);
        elem.SendMessage("SetPartNumerator", partNumerator);
        elem.SendMessage("SetPartDenominator", partDenominator);
        elem.SendMessage("SetFractionBaseOffset", fractionBaseOffset);
        elem.SendMessage("Initialize");
        elements.Add(elem);
    }

    protected void InitLiquid(ElementsType type)
    {
        width = 5.0f;
        height = 5.0f;

        GameObject elem = new GameObject("fraction");
        elem.transform.parent = transform;
        elem.transform.position = transform.TransformPoint(Vector3.zero);
        elem.AddComponent<LiquidElement>();
        elem.SendMessage("SetMode", mode);
        elem.SendMessage("SetType", type);
        elem.SendMessage("SetSize", new Vector2(width, height));
        elem.SendMessage("SetColor", color);
        elem.SendMessage("SetElementState", state);
        elem.SendMessage("SetNumerator", numerator);
        elem.SendMessage("SetDenominator", denominator);
        elem.SendMessage("SetPartitions", partitions);
        elem.SendMessage("SetPartNumerator", partNumerator);
        elem.SendMessage("SetPartDenominator", partDenominator);
        elem.SendMessage("SetFractionBaseOffset", fractionBaseOffset);
        elem.SendMessage("Initialize");
        elements.Add(elem);
    }    

    protected Vector3 GetSymbolPosition(bool isPartition)
    {
        float baseXPos = (width * 0.5f) * elementScale;
        
        float baseXOffset = 1.0f;
        float symbolOffset = 1.3f;
        float finalX = baseXPos + baseXOffset;

        if (isPartition)
            finalX += symbolOffset;

        return new Vector3(finalX, 0.0f, 0.8f);
    }

    protected void AttachModifierPartition()
    {
        if (null == partMod)
        {
            partMod = new GameObject("partition");
            partMod.transform.parent = transform;

            Vector3 pos = GetSymbolPosition(true);
            partMod.transform.position = transform.TransformPoint(new Vector3(pos.x, pos.y, pos.z));

            partMod.AddComponent<PartitionMCElement>();
            partMod.SendMessage("SetPartitions", partitions);
        }
        AttachButtons(CheckPartition());
        PlaceButtons();

        AttachSymbol(true);
    }

    protected void DetachModifierPartition()
    {
        Destroy(partMod);
        partMod = null;

        DetachButtons();

        DetachSymbol();
        if (isSymbolShown)
            AttachSymbol(true);
    }

    protected void AttachSymbol(bool isNotEditable)
    {
        if (null == symbol)
        {
            symbol = new GameObject("symbol");
            symbol.transform.parent = transform;

            Vector3 pos = GetSymbolPosition(false);
            symbol.transform.position = transform.TransformPoint(new Vector3(pos.x, pos.y, pos.z));

            symbol.AddComponent<SingleFractionMCElement>();
            symbol.SendMessage("SetNumerator", numerator);
            symbol.SendMessage("SetDenominator", denominator);
            symbol.SendMessage("SetPartDenominator", partDenominator);
            symbol.SendMessage("SetPartitions", partitions);
            symbol.SendMessage("SetPartNumerator", partNumerator);
        }
        symbol.SendMessage("ShowFlashNumbers", isNotEditable);
    }

    protected void DetachSymbol()
    {
        Destroy(symbol);
        symbol = null;
    }

    protected void AttachButtons(bool isEnabled)
    {
        Transform parent = null;
        if (mode == InteractionMode.Changing || mode == InteractionMode.Initializing)
        {
            parent = symbol.transform;
        }
        else if (mode == InteractionMode.Partitioning)
        {
            parent = partMod.transform;
        }

        if (null == btnUp)
        {
            btnUp = new GameObject("arrowUp");
            btnUp.transform.parent = parent;

            btnUp.AddComponent<InteractiveMovieClipElement>();
            btnUp.SendMessage("SetupMovieclip", "Flash/i_talk_2_learn.swf:mcChangeValueUpClass");
            btnUp.SendMessage("SetEnabled", isEnabled);
            btnUp.SendMessage("SetMode", mode);
        }

        if (null == btnDown)
        {
            btnDown = new GameObject("arrowDown");
            btnDown.transform.parent = parent;

            btnDown.AddComponent<InteractiveMovieClipElement>();
            btnDown.SendMessage("SetupMovieclip", "Flash/i_talk_2_learn.swf:mcChangeValueDownClass");
            btnDown.SendMessage("SetEnabled", isEnabled);
            btnDown.SendMessage("SetMode", mode);
        }
    }

    protected void DetachButtons()
    {
        Destroy(btnUp);
        btnUp = null;
        Destroy(btnDown);
        btnDown = null;
    }

    protected void AttachModifierSymbol()
    {
        AttachSymbol(true);
        AttachButtons(true);
        PlaceButtons();
    }

    protected void PlaceButtons()
    {
        if (mode == InteractionMode.Changing || mode == InteractionMode.Initializing)
        {
            if (selectedFractionPart == FractionPart.Denominator)
            {
                btnUp.transform.position = symbol.transform.TransformPoint(new Vector3(0.9f, -0.3f, 0.0f));
                btnDown.transform.position = symbol.transform.TransformPoint(new Vector3(0.9f, -0.9f, 0.0f));
                Workspace.Instance.interfaces.SendMessage("ShowSuggestion", "{hint_denominator}");
            }
            if (selectedFractionPart == FractionPart.Numerator)
            {
                btnUp.transform.position = symbol.transform.TransformPoint(new Vector3(0.9f, 0.8f, 0.0f));
                btnDown.transform.position = symbol.transform.TransformPoint(new Vector3(0.9f, 0.2f, 0.0f));
                Workspace.Instance.interfaces.SendMessage("ShowSuggestion", "{hint_numerator}");
            }
        }
        else if (mode == InteractionMode.Partitioning)
        {
            btnUp.transform.position = partMod.transform.TransformPoint(new Vector3(0.9f, 0.3f, 0.0f));
            btnDown.transform.position = partMod.transform.TransformPoint(new Vector3(0.9f, -0.3f, 0.0f));
        }
    }

    protected void DetachModifierSymbol()
    {
        DetachSymbol();
        if (isSymbolShown)
            AttachSymbol(true);

        DetachButtons();
    }

    protected void AttachModifierScale()
    {
        scaleModParent = new GameObject("scale_root");
        scaleModParent.transform.parent = transform;
        scaleModParent.AddComponent<ContainerElement>();

        scaleMod = new GameObject[4];
        for (int i = 0; i < 4; i++)
        {
            scaleMod[i] = new GameObject("square" + i);
            scaleMod[i].transform.parent = scaleModParent.transform;

            scaleMod[i].AddComponent<InteractiveMovieClipElement>();
            scaleMod[i].SendMessage("SetupMovieclip", "Flash/i_talk_2_learn.swf:mcScaleSquareClass");
            scaleMod[i].SendMessage("SetEnabled", true);
            scaleMod[i].SendMessage("SetSize", new Vector2(1.0f, 1.0f));
            scaleMod[i].SendMessage("SetMode", mode);
        }

        PlaceModifierScale();
    }

    protected void SetupLine(ref GameObject go)
    {
        go.transform.parent = scaleLinesParent.transform;
        go.AddComponent<MeshElement>();
        go.SendMessage("Initialize");
        go.SendMessage("SetMode", mode);
        go.SendMessage("SetType", type);
        go.SendMessage("SetColor", Workspace.Instance.green);
        go.SendMessage("SetStrokeDist", 0.2f);
        go.SendMessage("SetStrokeWidth", 0.0f);

    }

    protected void PlaceModifierScale()
    {
        if (null != scaleModParent)
        {
            //float elementScale = 1.0f;
            if (null != scaleLinesParent)
            {
                Destroy(scaleLinesParent);
                scaleLinesParent = null;
                scaleLines = null;
            }

            scaleLinesParent = new GameObject("lines_root");
            scaleLinesParent.AddComponent<ContainerElement>();
            scaleLinesParent.transform.parent = transform;
            scaleLinesParent.transform.position += new Vector3(0.0f, 0.0f, -0.6f);
            scaleLines = new GameObject[4];

            float positionOffset = 0.3f;
            float xPosOffset = bounds.size.x * 0.5f * elementScale + positionOffset;
            float yPosOffset = bounds.size.y * 0.5f * elementScale + positionOffset;

            if (null != scaleMod[0])
            {
                float xPos = bounds.center.x - xPosOffset;
                float yPos = bounds.center.y + yPosOffset;
                float zPos = bounds.center.z;
                scaleMod[0].transform.position = scaleModParent.transform.TransformPoint(new Vector3(xPos, yPos, zPos));

                scaleLines[0] = new GameObject("line0");
                SetupLine(ref scaleLines[0]);
                scaleLines[0].transform.position = scaleLinesParent.transform.TransformPoint(new Vector3(xPos + xPosOffset, yPos, zPos));
                scaleLines[0].SendMessage("SetSize", new Vector2(bounds.size.x * elementScale + positionOffset * 2.0f, 0.05f));
                scaleLines[0].SendMessage("Draw", zIndex);
            }
            if (null != scaleMod[1])
            {
                float xPos = bounds.center.x + xPosOffset;
                float yPos = bounds.center.y + yPosOffset;
                float zPos = bounds.center.z;
                scaleMod[1].transform.position = scaleModParent.transform.TransformPoint(new Vector3(xPos, yPos, zPos));

                scaleLines[1] = new GameObject("line1");
                SetupLine(ref scaleLines[1]);
                scaleLines[1].transform.position = scaleLinesParent.transform.TransformPoint(new Vector3(xPos, yPos - yPosOffset, zPos));
                scaleLines[1].SendMessage("SetSize", new Vector2(0.05f, bounds.size.y * elementScale + positionOffset * 2.0f));
                scaleLines[1].SendMessage("Draw", zIndex);
            }

            if (null != scaleMod[2])
            {
                float xPos = bounds.center.x - xPosOffset;
                float yPos = bounds.center.y - yPosOffset;
                float zPos = bounds.center.z;
                scaleMod[2].transform.position = scaleModParent.transform.TransformPoint(new Vector3(xPos, yPos, zPos));

                scaleLines[2] = new GameObject("line2");
                SetupLine(ref scaleLines[2]);
                scaleLines[2].transform.position = scaleLinesParent.transform.TransformPoint(new Vector3(xPos, yPos + yPosOffset, zPos));
                scaleLines[2].SendMessage("SetSize", new Vector2(0.05f, bounds.size.y * elementScale + positionOffset * 2.0f));
                scaleLines[2].SendMessage("Draw", zIndex);
            }

            if (null != scaleMod[3])
            {
                float xPos = bounds.center.x + xPosOffset;
                float yPos = bounds.center.y - yPosOffset;
                float zPos = bounds.center.z;
                scaleMod[3].transform.position = scaleModParent.transform.TransformPoint(new Vector3(xPos, yPos, zPos));

                scaleLines[3] = new GameObject("line3");
                SetupLine(ref scaleLines[3]);
                scaleLines[3].transform.position = scaleLinesParent.transform.TransformPoint(new Vector3(xPos - xPosOffset, yPos, zPos));
                scaleLines[3].SendMessage("SetSize", new Vector2(bounds.size.x * elementScale + positionOffset * 2.0f, 0.05f));
                scaleLines[3].SendMessage("Draw", zIndex);               
            }
        }
    }

    protected void DetachModifierScale()
    {
        Destroy(scaleModParent);
        scaleModParent = null;
        scaleMod = null;

        Destroy(scaleLinesParent);
        scaleLinesParent = null;
        scaleLines = null;
    }
    #endregion
}
