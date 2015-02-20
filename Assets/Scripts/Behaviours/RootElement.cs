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
    public GameObject cutRef = null;
    public List<GameObject> elements = new List<GameObject>();
    public BBExtend bbExtends = new BBExtend(0.0f, 0.0f, 0.0f, 0.0f);
    public GameObject symbol_root;
    public GameObject symbol_root_mobile;
    public GameObject partition_root;
    public GameObject partition_root_mobile;
    public GameObject symbol;
    public GameObject partMod;
    public SetElement.Shape shape;
    public bool isSymbolShown = false;
    public GameObject highlight;
    public bool isHighlighted;
    public bool inputByChild = false;
    #endregion

    #region Protected Fields
    protected float ActualfactorScale;
    protected const float deltaDrag = 10.0f;
    protected float lastElementScale = 1.0f;
    protected Vector3 deltaTouch;
    protected Vector3 touchPos;
    protected Vector3 lastStillPosition;
    protected FractionPart selectedFractionPart = FractionPart.Denominator;
    protected GameObject btnUp;
    protected GameObject btnDown;
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

    protected Vector3 initialMouseDownPos = Vector3.zero;
    public bool hasDragged = false;
    public bool isDragged = false;
    protected float doubleTapTimer = -1.0f;
    protected float longTapTimer = -1.0f;

    protected bool partitionActive = false;
    #endregion

    #region Public Getters
    public bool IsScaling
    {
        get
        {
            return isScaling;
        }
    }

    public bool PartitionActive
    {
        get
        {
            return partitionActive;
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
        ActualfactorScale = 1.0f;
    }

    public void OnMouseDown()
    {
        if (Input.touchCount >= 2)
            return;

        if (!inputEnabled)
            return;

#if UNITY_IPHONE || UNITY_ANDROID
        if (Input.touchCount == 1)
            longTapTimer = Time.time;
#endif

        isDragged = true;
        hasDragged = false;
        deltaTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        initialMouseDownPos = Input.mousePosition;
        Workspace.Instance.interfaces.SendMessage("ShowHint", "");

        if (inputByChild)
            return;
    
        if (mode == InteractionMode.Initializing)
        {
            Workspace.Instance.SendMessage("SetFocusOn", gameObject);
            Draw(zIndex);
        }
        else if (mode == InteractionMode.Moving || mode == InteractionMode.Changing)
        {

            Workspace.Instance.SendMessage("CancelOperation");
            Workspace.Instance.SendMessage("SetFocusOn", gameObject);
            if (denominator == 0 && state != ElementsState.Improper)
               BroadcastMessage("SetMode" ,InteractionMode.Changing, SendMessageOptions.DontRequireReceiver);

        }
        else if (mode == InteractionMode.Freeze)
        {
            Workspace.Instance.SendMessage("CancelOperation");
            Workspace.Instance.SendMessage("SetFocusOn", gameObject);
        }
        if (mode != InteractionMode.Wait)
            Workspace.Instance.interfaces.SendMessage("OnElementClicked");

        if (!isSubFraction)
        {
            lastStillPosition = gameObject.transform.position;
        }

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
            case (ElementsType.MoonSet):
                typeString = "MoonSets";
                break;
            case (ElementsType.HeartSet):
                typeString = "HeartSets";
                break;
            case (ElementsType.StarSet):
                typeString = "StarSets";
                break;
        }

        if (state == ElementsState.Cut)
            ExternalEventsManager.Instance.SendMessageToSupport("PressCut", typeString, name, partNumerator + "/" + partDenominator, "(" + transform.position.x + ", " + transform.position.y + ")");
        else
            ExternalEventsManager.Instance.SendMessageToSupport("PressFraction", typeString, name, partNumerator + "/" + partDenominator, "(" + transform.position.x + ", " + transform.position.y + ")");
        
        DestroyFindParentLine();
    }

    public void OnMouseDrag()
    {
        if (Input.touchCount >= 2 && Workspace.Instance.isPinchActive)
            return;
        bool checkButtonOver = GameObject.FindGameObjectWithTag("Interface").GetComponent<InterfaceBehaviour>().isPressingOperation;
        if (!inputEnabled ||
            !isDragged ||
            checkButtonOver ||
            GameObject.FindGameObjectWithTag("Interface").GetComponent<InterfaceBehaviour>().isDragWindow ||
            GameObject.FindGameObjectWithTag("Interface").GetComponent<InterfaceBehaviour>().isZoomActive ||
            GameObject.FindGameObjectWithTag("Interface").GetComponent<InterfaceBehaviour>().isBlockingOperation)
        {
            return;
        }
        float orthoSize = GameObject.FindGameObjectWithTag("Interface").GetComponent<InterfaceBehaviour>().min_orthographicsize;
        float hMargin = orthoSize * Screen.width / Screen.height;
        float vMargin = orthoSize;
        float marginOffset = -1.0f;
        float leftMargin = -(hMargin - marginOffset);
        float rightMargin = hMargin - marginOffset;
        float topMargin = vMargin; // - marginOffset;
        float bottomMargin = -vMargin; // -marginOffset;

        Vector3 clampedMousePos = Input.mousePosition;

        /*   if (Input.mousePosition.x > 220.0f * Screen.width / 800.0f && Input.mousePosition.x < 580.0f * Screen.width / 800.0f)
           {
              // Debug.Log("Input.mousePosition.x " + Input.mousePosition.x);
               clampedMousePos.y = Mathf.Clamp(Input.mousePosition.y, 0.0f, Screen.height - (100.0f * Screen.height / 600.0f));
               /*if (partDenominator == 0)
               {
                   clampedMousePos.y = Mathf.Clamp(Input.mousePosition.y, 0.0f, Screen.height - (230.0f * Screen.height / 600.0f));
               }*/
        //}

        clampedMousePos.y = Mathf.Clamp(Input.mousePosition.y, 0.0f, Screen.height);
        clampedMousePos.x = Mathf.Clamp(Input.mousePosition.x, 0.0f, Screen.width);

        Vector3 currentTouch = Camera.main.ScreenToWorldPoint(clampedMousePos);
        touchPos = currentTouch - deltaTouch;


        if (touchPos.x >= leftMargin && touchPos.x <= rightMargin && touchPos.y <= topMargin && touchPos.y >= bottomMargin)
        {
            transform.position = touchPos;
        }

        Vector3 tmpPos = transform.position;
        tmpPos.z = zIndex;
        transform.position = tmpPos;
        hasDragged = ((Input.mousePosition - initialMouseDownPos).magnitude > deltaDrag);
    }

    void OnMouseOver()
    {
        if (!inputEnabled)
            return;

        if (mode == InteractionMode.Moving && !Input.GetMouseButton(0))
        {
            if (state == ElementsState.Fraction)
                Workspace.Instance.interfaces.SendMessage("ShowHint", "{hint_onshape}");
            else
                Workspace.Instance.interfaces.SendMessage("ShowHint", "{hint_oncut}");
        }

        if (denominator > 0 && Input.GetMouseButtonUp(1) && mode != InteractionMode.Freeze && mode != InteractionMode.Wait && !Input.GetMouseButton(0) && mode != InteractionMode.LookAt)
        {
            RightClick();
        }
    }

    void OnMouseExit()
    {
        if (Input.touchCount >= 2)
            return;
        // Debug.Log("OnMouseExit");
        if (!inputEnabled)
            return;
        if (hasDragged)
            return;
        Workspace.Instance.interfaces.SendMessage("ShowHint", "");
    }

    public void OnMouseUp()
    {
#if UNITY_IPHONE || UNITY_ANDROID
        longTapTimer = -1.0f;
#endif
        isDragged = false;
        if (!inputEnabled)
            return;

        if (mode == InteractionMode.Changing && !hasDragged && !inputByChild)
        {
            BroadcastMessage("OnClicked", Camera.main.ScreenToWorldPoint(Input.mousePosition), SendMessageOptions.DontRequireReceiver);
        }
        if (mode == InteractionMode.Freeze)
        {
            Workspace.Instance.SendMessage("SendBack", gameObject);
        }
        Workspace.Instance.interfaces.SendMessage("OnElementReleased", gameObject);

        //#if UNITY_IPHONE
        if (state == ElementsState.Fraction)
        {
            if (!hasDragged)
            {
                if (!inputByChild)
                {
                    if (doubleTapTimer < 0.0)
                    {
                        doubleTapTimer = Time.time;
                    }
                    else if (Time.time - doubleTapTimer > 0.1f && Time.time - doubleTapTimer < 0.4f)
                    {
                        doubleTapTimer = Time.time;
                        if (denominator > 0 && mode == InteractionMode.Moving && !GameObject.FindGameObjectWithTag("Interface").GetComponent<InterfaceBehaviour>().isMouseOnZoom())
                        {
                            //RightClick();
                            CutFraction();
                        }
                    }
                    else
                    {

                        doubleTapTimer = Time.time;
                    }
                }
            }
            else
            {
                hasDragged = false;
            }

        }
        if (state == ElementsState.Cut)
        {
            Workspace.Instance.SendMessage("CheckCutOverlap", Input.mousePosition);
        }

        if (inputByChild)
        {
            inputByChild = false;
            return;
        }

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
            case (ElementsType.MoonSet):
                typeString = "MoonSets";
                break;
            case (ElementsType.HeartSet):
                typeString = "HeartSets";
                break;
            case (ElementsType.StarSet):
                typeString = "StarSets";
                break;

        }
        if (state == ElementsState.Cut)
            ExternalEventsManager.Instance.SendMessageToSupport("ReleaseCut", typeString, name, partNumerator + "/" + partDenominator, "(" + transform.position.x + ", " + transform.position.y + ")");
        else
            ExternalEventsManager.Instance.SendMessageToSupport("ReleaseFraction", typeString, name, partNumerator + "/" + partDenominator, "(" + transform.position.x + ", " + transform.position.y + ")");

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
                case (ElementsType.MoonSet):
                    typeString = "MoonSets";
                    break;
                case (ElementsType.HeartSet):
                    typeString = "HeartSets";
                    break;
                case (ElementsType.StarSet):
                    typeString = "StarSets";
                    break;
            }
            ExternalEventsManager.Instance.SendMessageToSupport("SecActionFraction", typeString, name, partNumerator + "/" + partDenominator, "(" + transform.position.x + ", " + transform.position.y + ")");
        }

        Workspace.Instance.SendMessage("SetFocusOn", gameObject);
        Workspace.Instance.interfaces.SendMessage("ShowHint", "");
        mode = InteractionMode.Moving;
        BroadcastMessage("SetMode", InteractionMode.Moving);

        if (state == ElementsState.Fraction || state == ElementsState.Result)
            Workspace.Instance.interfaces.SendMessage("OnShowContextMenu", gameObject);
        if (state != ElementsState.Cut)
            Workspace.Instance.interfaces.SendMessage("OnElementReleased", gameObject);
    }
    
    void Update()
    {
        if (mode != lastMode)
        {
            if (mode == InteractionMode.Moving)
            {
                if (!isSubFraction && state == ElementsState.Fraction)
                    AttachSymbol(true);
                if (null == symbol)
                    AttachModifierSymbol();
                DetachModifierScale();
                DetachButtons();
            }
            else if (mode == InteractionMode.Changing || mode == InteractionMode.Initializing)
            {
                AttachModifierSymbol();
            }
            else if (mode == InteractionMode.Partitioning)
            {
                DetachModifierScale();
                DetachButtons();
                AttachModifierPartition();
            }
            else if (mode == InteractionMode.Scaling)
            {
                DetachModifierSymbol();
                DetachButtons();
                //AttachModifierScale();
            }
            else if (mode == InteractionMode.Freeze)
            {
                if (isHighlighted)
                    highlight.gameObject.SetActive(false);
                BroadcastMessage("Freeze", SendMessageOptions.DontRequireReceiver);
            }
            if (lastMode == InteractionMode.Freeze)
            {
                if (isHighlighted)
                    highlight.gameObject.SetActive(true);
                BroadcastMessage("UnFreeze", SendMessageOptions.DontRequireReceiver);
            }

            lastMode = mode;
        }

        if (mode == InteractionMode.Moving && !isSubFraction && state == ElementsState.Fraction)
        {
            if (null == symbol)
                AttachModifierSymbol();
        }
        if (mode == InteractionMode.Initializing)
        {
            Workspace.Instance.SendMessage("SetFocusOn", gameObject);
            Draw(zIndex);
        }

        if (!inputEnabled)
            return;

        CheckScaleTween();
#if UNITY_IPHONE || UNITY_ANDROID
        if (Input.touchCount == 1 &&
        longTapTimer > 0 &&
        Time.time - longTapTimer > 0.6f &&
        !hasDragged &&
        !GameObject.FindGameObjectWithTag("Interface").GetComponent<InterfaceBehaviour>().isDragWindow &&
        !GameObject.FindGameObjectWithTag("Interface").GetComponent<InterfaceBehaviour>().zoom &&
        !GameObject.FindGameObjectWithTag("Interface").GetComponent<InterfaceBehaviour>().isBlockingOperation)
        {

            if (denominator > 0)
                RightClick();
            longTapTimer = -1.0f;
        }
#endif
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
    public override void SetRoot(GameObject r)
    {
        base.SetRoot(r);
        root = r;
    }

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
        if (state == ElementsState.Improper)
        {
            for (int i = 0; i < transform.childCount; i++)
                bounds.Encapsulate(transform.GetChild(i).GetComponent<WSElement>().GetBounds());
        }
        else
        {
            for (int i = 0; i < elements.Count; i++)
            {
                bounds.Encapsulate(elements[i].GetComponent<WSElement>().GetBounds());
            }
        }

        bounds.min -= new SBSVector3(bbExtends.left, bbExtends.bottom, 0.0f);
        bounds.max += new SBSVector3(bbExtends.right, bbExtends.top, 0.0f);

        return bounds;
    }

    public override void UpdateCollider(SBSBounds bounds)
    {

        SBSVector3 pos = transform.position;
        collider.center = ((bounds.max + bounds.min) * 0.5f) - pos;
        collider.center = new Vector3(collider.center.x, collider.center.y, 0.0f);
        if (state == ElementsState.Cut || state == ElementsState.Result)
        {
            collider.size = (bounds.max - bounds.min);
        }
        else
        {
            collider.size = (bounds.max - bounds.min);
        }
    }

    public void UpdateWidth()
    {
        if (isSubFraction /*&& null != transform.GetChild(0).GetComponent<SetElement>()*/)
        {
            if (null != transform.GetChild(0).GetComponent<SetElement>())
                width = transform.GetChild(0).GetComponent<SetElement>().width;
            if (root != null)
                root.SendMessage("UpdateWidth");
        }
        else
        {
            width = GetComponent<BoxCollider>().size.x;
            Workspace.Instance.RepositioningChildren(this.gameObject);
        }

    }

    public override void Draw(int zIndex)
    {
        if (!isSubFraction)
            GameObject.FindGameObjectWithTag("Workspace").GetComponent<Workspace>().DrawCounter();
        base.Draw(zIndex);

        if (mode == InteractionMode.Changing && zIndex > 0)
        {
            //Debug.Log("Draw " + name);
            mode = InteractionMode.Moving;
            BroadcastMessage("SetMode", mode);
        }

        Vector3 pos = transform.position;
        pos.z = zIndex;
        transform.position = pos;

        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).BroadcastMessage("Draw", zIndex, SendMessageOptions.DontRequireReceiver);

        UpdateCollider(GetBounds());
        //if (state == ElementsState.Cut || state == ElementsState.Result)
        if (state == ElementsState.Cut && mode == InteractionMode.Wait)
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
        if(!isSubFraction && isHighlighted && mode != InteractionMode.Freeze)
              InitHighlight(name, true);
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
            if (null != transform.GetChild(i).GetComponent<WSElement>() && !transform.GetChild(i).GetComponent<WSElement>().CheckPartition())
                return false;
        }
        return true;
    }

    public override void IncreaseNumerator()
    {
        //Debug.Log("IncreaseNumerator");
       /* if (!inputEnabled)
            return;*/
        if (!isSubFraction)
        {
            this.partNumerator++;
            if ((float)this.partNumerator / (float)this.partDenominator > Workspace.MAXVALUE)
            {
                this.partNumerator--;
                Workspace.Instance.interfaces.SendMessage("ShowFeedbackPopup", "{rep_greater_" + Workspace.MAXVALUE + "}");
                return;
            }

            bool addedNew = false;
            if (this.partNumerator > this.partDenominator * elements.Count)
            {
                addedNew = true;
                Workspace.Instance.AddFractionsChildren(gameObject);
                //this.partNumerator = this.partDenominator * elements.Count;
            }

            this.numerator = this.partNumerator / this.partitions;
            if (elements.Count > 1)
            {
                if (type == ElementsType.HRect || type == ElementsType.VRect || type == ElementsType.Set || type == ElementsType.HeartSet || type == ElementsType.MoonSet || type == ElementsType.StarSet)
                {
                    if (!addedNew)
                        IncreaseNumeratorInChildren();
                    UpdateByChildren();
                }
                else
                {
                    int lastIndex = elements.Count - 1;
                    int lastNumerator = this.partNumerator - (this.partDenominator * lastIndex);

                    elements[lastIndex].BroadcastMessage("SetNumerator", lastNumerator / this.partitions);
                    elements[lastIndex].BroadcastMessage("SetPartNumerator", lastNumerator);

                    symbol.SendMessage("SetNumerator", this.numerator);
                    symbol.SendMessage("SetPartNumerator", this.partNumerator);
                    ExternalEventsManager.Instance.SendMessageToSupport("FractionChange", "Numerator", root.name, partNumerator);
                }
            }
            else
            {
                root.BroadcastMessage("SetNumerator", this.numerator);
                root.BroadcastMessage("SetPartNumerator", this.partNumerator);
                ExternalEventsManager.Instance.SendMessageToSupport("FractionChange", "Numerator", root.name, partNumerator);
            }
        }
    }

    public override void DecreaseNumerator()
    {
        // Debug.Log("DecreaseNumerator");
        if (!isSubFraction)
        {
            this.partNumerator--;
            if (this.partNumerator < 0)
                this.partNumerator = 0;

            this.numerator = this.partNumerator / this.partitions;

            if (elements.Count > 1)
            {
                if (type == ElementsType.HRect || type == ElementsType.VRect || type == ElementsType.Set || type == ElementsType.HeartSet || type == ElementsType.MoonSet || type == ElementsType.StarSet)
                {
                    DecreaseNumeratorInChildren();
                    UpdateByChildren();
                }
                else
                {
                    int lastIndex = elements.Count - 1;
                    int lastNumerator = this.partNumerator - (this.partDenominator * lastIndex);

                    elements[lastIndex].BroadcastMessage("SetNumerator", lastNumerator / this.partitions);
                    elements[lastIndex].BroadcastMessage("SetPartNumerator", lastNumerator);

                    if (lastNumerator <= 0)
                        Workspace.Instance.RemoveEmptyChildren(gameObject);

                    symbol.SendMessage("SetNumerator", this.numerator);
                    symbol.SendMessage("SetPartNumerator", this.partNumerator);
                    ExternalEventsManager.Instance.SendMessageToSupport("FractionChange", "Numerator", root.name, partNumerator);
                }
            }
            else
            {
                root.BroadcastMessage("SetNumerator", this.numerator);
                root.BroadcastMessage("SetPartNumerator", this.partNumerator);
                ExternalEventsManager.Instance.SendMessageToSupport("FractionChange", "Numerator", root.name, partNumerator);
            }
        }
    }

    public override void IncreaseDenominator()
    {
        if (!isSubFraction)
        {
            this.denominator++;
            this.partDenominator = denominator * partitions;
            root.BroadcastMessage("SetDenominator", this.denominator);
            root.BroadcastMessage("SetPartDenominator", this.partDenominator);

            ExternalEventsManager.Instance.SendMessageToSupport("FractionChange", "Denominator", root.name, partDenominator);

            int repsNum = Mathf.CeilToInt((float)this.partNumerator / (float)(this.partDenominator));

            if (elements.Count > 1)
            {
                int currentNum = this.numerator;
                int currentPartNum = this.partNumerator;
                for (int i = 0; i < elements.Count; i++)
                {
                    int num = Mathf.Min(currentNum, this.denominator);
                    int numPart = Mathf.Min(currentPartNum, this.partDenominator);
                    elements[i].BroadcastMessage("SetNumerator", num);
                    elements[i].BroadcastMessage("SetPartNumerator", numPart);

                    currentNum -= this.denominator;
                    currentPartNum -= this.partDenominator;
                }
                Workspace.Instance.RemoveEmptyChildren(gameObject);
            }

            symbol.SendMessage("SetNumerator", this.numerator);
            symbol.SendMessage("SetPartNumerator", this.partNumerator);
        }
    }

    public override void DecreaseDenominator()
    {
       /* if (!inputEnabled)
            return;*/
        if (!isSubFraction)
        {
            if (this.denominator > 0)
            {
                this.denominator--;
                if (this.denominator < 1)
                    this.denominator = 1;

                if ((float)this.partNumerator / ((float)this.denominator * (float)this.partitions) > Workspace.MAXVALUE)
                {
                    this.denominator++;
                    Workspace.Instance.interfaces.SendMessage("ShowFeedbackPopup", "{rep_greater_" + Workspace.MAXVALUE + "}");
                    return;
                }

                this.partDenominator = denominator * partitions;

                root.BroadcastMessage("SetDenominator", this.denominator);
                root.BroadcastMessage("SetPartDenominator", this.partDenominator);

                int lastIndex = elements.Count - 1;
                int lastNumerator = this.partNumerator - (this.partDenominator * lastIndex);

                if (type == ElementsType.HRect || type == ElementsType.VRect || type == ElementsType.Set || type == ElementsType.HeartSet || type == ElementsType.MoonSet || type == ElementsType.StarSet)
                {
                    int wholes = Mathf.Max(1, Mathf.CeilToInt((float)this.partNumerator / (float)this.partDenominator));
                    lastIndex = wholes - 1;
                    lastNumerator = this.partNumerator - (this.partDenominator * lastIndex);
                    //Debug.Log("Root " + root.name + " this " + gameObject.name + "lastNumerator " + lastNumerator);
                    int newRepNum = 0;
                    if (wholes > elements.Count)
                        newRepNum = wholes - elements.Count;

                    for (int i = 0; i < newRepNum; i++)
                        Workspace.Instance.AddFractionsChildren(gameObject);

                    for (int i = 0; i < elements.Count; i++)
                    {
                        elements[i].BroadcastMessage("SetNumerator", "0");
                        elements[i].BroadcastMessage("SetPartNumerator", "0");
                    }

                    for (int i = 0; i < elements.Count; i++)
                    {
                        if (i < wholes)
                        {
                            if (i < lastIndex)
                            {
                                elements[i].BroadcastMessage("SetNumerator", this.denominator);
                                elements[i].BroadcastMessage("SetPartNumerator", this.partDenominator);
                                // Debug.Log("1root " + gameObject.name + " SetNumerator " + this.denominator + " SetPartNumerator " + this.partDenominator);
                            }
                            else
                            {
                                elements[i].BroadcastMessage("SetNumerator", lastNumerator / this.partitions);
                                elements[i].BroadcastMessage("SetPartNumerator", lastNumerator);
                                //elements[i].BroadcastMessage("Draw", elements[i].transform.position.z);
                                // Debug.Log("2root " + gameObject.name + " SetNumerator " + (lastNumerator / this.partitions) + " SetPartNumerator " + lastNumerator);

                            }
                        }

                    }
                }
                else
                {
                    if (lastNumerator > this.partDenominator)
                    {
                        int newRepNum = lastNumerator / this.partDenominator;
                        for (int i = 0; i < newRepNum; i++)
                            Workspace.Instance.AddFractionsChildren(gameObject);

                        lastIndex = elements.Count - 1;
                        lastNumerator -= (this.partDenominator * newRepNum);
                    }

                    for (int i = 0; i < elements.Count; i++)
                    {
                        if (i < lastIndex)
                        {
                            elements[i].BroadcastMessage("SetNumerator", this.denominator);
                            elements[i].BroadcastMessage("SetPartNumerator", this.partDenominator);
                        }
                        else
                        {
                            elements[i].BroadcastMessage("SetNumerator", lastNumerator / this.partitions);
                            elements[i].BroadcastMessage("SetPartNumerator", lastNumerator);
                        }

                    }
                }
                //Debug.Log("2root " + gameObject.name + " width " + width);
                Workspace.Instance.RemoveEmptyChildren(gameObject);
                ExternalEventsManager.Instance.SendMessageToSupport("FractionChange", "Denominator", root.name, partDenominator);
            }
        }
    }

    public override void IncreasePartitions()
    {
        //Debug.Log("IncreasePartitions");
        if (!isSubFraction)
        {
            this.partitions++;

            this.partNumerator = numerator * partitions;
            this.partDenominator = denominator * partitions;

            root.BroadcastMessage("SetPartitions", this.partitions);

            if (type == ElementsType.Set || type == ElementsType.HeartSet || type == ElementsType.MoonSet || type == ElementsType.StarSet)
            {
                height = (height / (partitions - 1)) * partitions;
                float offset = 0.4f * (root.GetComponent<RootElement>().elements.Count - 1);
                root.BroadcastMessage("SetSize", new Vector2((width - offset) / (root.GetComponent<RootElement>().elements.Count), height));
            }
            ExternalEventsManager.Instance.SendMessageToSupport("FractionChange", "Partitions", root.name, partitions);
        }
    }

    public override void DecreasePartitions()
    {
        if (!isSubFraction)
        {
            if (type == ElementsType.Set || type == ElementsType.HeartSet || type == ElementsType.MoonSet || type == ElementsType.StarSet)
            {
                if (this.partitions > 1)
                {
                    this.height = (this.height / this.partitions) * (this.partitions - 1);
                    float offset = 0.4f * (root.GetComponent<RootElement>().elements.Count - 1);
                    root.BroadcastMessage("SetSize", new Vector2((width - offset) / (root.GetComponent<RootElement>().elements.Count), height));
                }
            }
            this.partitions--;
            if (this.partitions < 1)
                this.partitions = 1;

            this.partNumerator = numerator * partitions;
            this.partDenominator = denominator * partitions;

            root.BroadcastMessage("SetPartitions", this.partitions);

            ExternalEventsManager.Instance.SendMessageToSupport("FractionChange", "Partitions", root.name, partitions);
        }
    }

    public void Cut()
    {
        isSymbolShown = false;
        DetachModifierSymbol();

        if (null != highlight)
            Destroy(highlight);

        for (int i = 0; i < elements.Count; i++)
            elements[i].SendMessage("Cut", SendMessageOptions.DontRequireReceiver);

        switch (type)
        {
            case (ElementsType.HRect):
                ExternalEventsManager.Instance.SendMessageToSupport("CutGenerated", "HRects", name);
                break;
            case (ElementsType.VRect):
                ExternalEventsManager.Instance.SendMessageToSupport("CutGenerated", "VRects", name);
                break;
            case (ElementsType.Liquid):
                ExternalEventsManager.Instance.SendMessageToSupport("CutGenerated", "LiquidMeasures", name);
                break;
            case (ElementsType.Line):
                ExternalEventsManager.Instance.SendMessageToSupport("CutGenerated", "NumberedLines", name);
                break;
            case (ElementsType.HeartSet):
                ExternalEventsManager.Instance.SendMessageToSupport("CutGenerated", "HeartSets", name);
                break;
            case (ElementsType.MoonSet):
                ExternalEventsManager.Instance.SendMessageToSupport("CutGenerated", "MoonSets", name);
                break;
            case (ElementsType.StarSet):
                ExternalEventsManager.Instance.SendMessageToSupport("CutGenerated", "StarSets", name);
                break;
        }
    }

    public void ChangeColor(Color c)
    {
        color = c;
        BroadcastMessage("SetContentColor", color);
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

    #region Functions

    public void InitHighlight(string name, bool isActiveByStudent)
    {
        if (isActiveByStudent)
        {
            if (null == highlight)
                return;
            highlight.transform.parent = Workspace.Instance.transform;
        }
       // Debug.Log("InitHighlight " + isActiveByStudent);
        if (this.isSubFraction) return;
        if (!gameObject.name.Equals(name)) return;
        GameObject GO = this.gameObject;

        SBSBounds b = GO.GetComponent<RootElement>().GetBounds();

        SingleFractionMCElement sfe = gameObject.GetComponentInChildren<SingleFractionMCElement>();
        if (sfe != null)
        {
            // Debug.Log("boxcollider " + );
            // if (null == sfe.gameObject.GetComponent<BoxCollider>())
            //   sfe.gameObject.AddComponent<BoxCollider>();
            //sfe.gameObject.GetComponent<BoxCollider>().size = sfe.GetComponent<RectTransform>().sizeDelta;

            Vector3 sfeSize = sfe.GetComponent<RectTransform>().sizeDelta;
            b.Encapsulate(sfe.transform.position.x + sfeSize.x, sfe.transform.position.y, sfe.transform.position.z);
        }

        PartitionMCElement pe = gameObject.GetComponentInChildren<PartitionMCElement>();
        if (pe != null)
        {
            // if (null == pe.gameObject.GetComponent<BoxCollider>())
            // pe.gameObject.AddComponent<BoxCollider>();
            //pe.gameObject.GetComponent<BoxCollider>().size = pe.GetComponent<RectTransform>().sizeDelta;

            Vector3 peSize = pe.GetComponent<RectTransform>().sizeDelta;
            b.Encapsulate(pe.transform.position.x + 1.0f, pe.transform.position.y, pe.transform.position.z);
        }

        Vector3[] pointList = new Vector3[4];
        float maxX = float.MinValue, maxY = float.MinValue;
        float minX = float.MaxValue, minY = float.MaxValue;

        SBSVector3[] vertices = b.GetVertices();

        for (int i = 0; i < vertices.Length; i++)
        {
            maxX = Mathf.Max(maxX, vertices[i].x);
            minX = Mathf.Min(minX, vertices[i].x);
            maxY = Mathf.Max(maxY, vertices[i].y);
            minY = Mathf.Min(minY, vertices[i].y);
        }

        float offsetX = 0.3f;
        float offsetY = 0.3f;

        pointList[0] = new Vector3(minX - offsetX, maxY + offsetY, 1.0f);   //TopLeft
        pointList[1] = new Vector3(maxX + offsetX, maxY + offsetY, 1.0f);   //TopRight
        pointList[2] = new Vector3(maxX + offsetX, minY - offsetY, 1.0f);   //BottomRight
        pointList[3] = new Vector3(minX - offsetX, minY - offsetY, 1.0f);   //BottmLeft

        float totalWidth = (pointList[0] - pointList[1]).magnitude;
        float totalHeight = (pointList[0] - pointList[3]).magnitude;

        if (!isActiveByStudent)
            GameObject.FindGameObjectWithTag("Interface").GetComponent<InterfaceBehaviour>().RapresentationHighlight(gameObject, new Vector2(totalWidth, totalHeight), b.center);
        else
        {
            float widthOffeset = 40.0f;
            float heightOffset = 0.7f;
            if (type == ElementsType.Liquid)
                widthOffeset = 40.0f;
            if (type == ElementsType.StarSet || type == ElementsType.MoonSet || type == ElementsType.HeartSet)
                heightOffset = 4.2f - totalHeight;
            if (heightOffset < 0.7f)
                heightOffset = 0.7f;
            highlight.SetActive(true);
            highlight.GetComponent<RectTransform>().localScale = Vector3.one * 0.03333334f;
            highlight.GetComponent<RectTransform>().anchoredPosition = (Vector3)b.center;//new Vector2(center.x - m.transform.position.x, 0.0f);
            highlight.GetComponent<RectTransform>().sizeDelta = new Vector2((totalWidth / highlight.GetComponent<RectTransform>().localScale.x) + widthOffeset, (totalHeight + heightOffset) / highlight.GetComponent<RectTransform>().localScale.y);
            highlight.transform.parent = gameObject.transform;
        }
    }

    public void DestroyHighlight()
    {
        GameObject.FindGameObjectWithTag("Interface").SendMessage("HideInterfaceHighlight");
    }
    #endregion

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
            case (ElementsType.Set):
            case (ElementsType.HeartSet):
            case (ElementsType.StarSet):
            case (ElementsType.MoonSet):
                InitSet(type);
                break;

        }
    }

    void SetEnableCollider(bool flag)
    {
        collider.enabled = flag;
    }

    public void ResetLastPosition()
    {
        if (!isSubFraction)
        {
            gameObject.transform.position = lastStillPosition;
            Tweener.StopAndDestroyAllTweens();
            isTweening = false;
            gameObject.transform.localScale = Vector3.one;
            ResetScale();
            if (mode == InteractionMode.Freeze)
            {
                Workspace.Instance.SendMessage("SendBack", gameObject);
            }
            //currentScaleTween = 1.0f;
        }
    }

    /* public void ResetPositionInCenter() 
     {
         gameObject.transform.position = Vector3.zero;
         Tweener.StopAndDestroyAllTweens();
         isTweening = false;
         gameObject.transform.localScale = Vector3.one;
         ResetScale();
     }*/

    void ResetScale()
    {
        tweenScaleQueue.Clear();
        currentScaleTween = 1.0f;
        tweenScaleQueue.Add(1.0f);
    }

    void OnSelectFractionPart(FractionPart part)
    {
        selectedFractionPart = part;
        PlaceButtons((int)part);
    }

    public void OnClickArrowUp(int _selectedFractionPart)
    {
        if (mode == InteractionMode.LookAt)
            return;
        if (mode == InteractionMode.Changing || mode == InteractionMode.Initializing)
        {
            selectedFractionPart = (FractionPart)_selectedFractionPart;
            // Workspace.Instance.interfaces.SendMessage("ShowSuggestion", "");
            if (denominator == 0)
            {
                mode = InteractionMode.Changing;
                Workspace.Instance.interfaces.SendMessage("EnableHUD");
                Workspace.Instance.SendMessage("EnableInput");
            }
            if (_selectedFractionPart == (int)FractionPart.Numerator)
                IncreaseNumerator();
            else if (_selectedFractionPart == (int)FractionPart.Denominator)
            {
                IncreaseDenominator();
            }
            //BroadcastMessage("IncreaseDenominator");
        }
        else if (mode == InteractionMode.Partitioning)
        {
            BroadcastMessage("IncreasePartitions");
        }

        Draw(zIndex);
    }

    public void OnClickArrowDown(int _selectedFractionPart)
    {
        if (mode == InteractionMode.LookAt)
            return;
        if (mode == InteractionMode.Changing || mode == InteractionMode.Initializing)
        {
            selectedFractionPart = (FractionPart)_selectedFractionPart;
            /* if (mode == InteractionMode.Initializing)
                 Workspace.Instance.interfaces.SendMessage("ShowSuggestion", "");*/

            if (_selectedFractionPart == (int)FractionPart.Numerator)
                DecreaseNumerator();
            else if (_selectedFractionPart == (int)FractionPart.Denominator)
                DecreaseDenominator();
        }
        else if (mode == InteractionMode.Partitioning)
        {
            BroadcastMessage("DecreasePartitions");
        }
        Draw(zIndex);
    }

    void ScaleDown()
    {
        tweenScaleQueue.Clear();
        if (currentScaleTween != 0.25f)
            tweenScaleQueue.Add(0.25f);
    }

    void ScaleUp()
    {
        tweenScaleQueue.Clear();
        if (currentScaleTween != 1.0f)
            tweenScaleQueue.Add(1.0f);
    }

    protected float currentScaleTween = 0.0f;
    void CheckScaleTween()
    {
        if (!isTweening && tweenScaleQueue.Count > 0)
        {
            currentScaleTween = tweenScaleQueue[0];
            float scaleTo = currentScaleTween;
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
        isScaling = true;
        initialMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        initialScale = elementScale;

        switch (modfierName)
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
        isScaling = false;
    }

    void CutFraction()
    {
        //Debug.Log("cut fraction " + gameObject.name);
        if (partNumerator > 0)
            Workspace.Instance.SendMessage("CutFraction", gameObject);
    }

    void Copy()
    {
        Workspace.Instance.SendMessage("CreateCopy", gameObject);
    }

    void FindParent()
    {
        /*if (null != parentRef)
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
        }*/
    }

    void DestroyFindParentLine()
    {
        GameObject pLine = GameObject.Find("parent_line");
        if (null != pLine)
        {
            Destroy(pLine);
            pLine = null;
        }
    }

    /*public void ResetScale()
    {
        elementScale = 1.0f;
        BroadcastMessage("SetElementScale", 1.0f);
        Draw(zIndex);
    }*/

    protected Vector3 cutTargetPos = Vector3.zero;
    void MoveToPopup(float time)
    {
        if (!isSubFraction)
        {
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
    }

    void MoveToCenter(float time)
    {
        if (!isSubFraction)
        {
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
    }

    protected Vector3 popupCoord = Vector3.zero;
    void SetCoord(Vector3 coord)
    {
        popupCoord = coord;
    }

    void MoveToCoord(float time)
    {
        //   Debug.Log("MoveToCoord " + time);
        if (!isSubFraction)
        {
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
    }

    void DecreaseRectNum()
    {
        RootElement[] roots = GetComponentsInChildren<RootElement>();
        for (int i = 0; i < roots.Length; i++)
        {
            if (roots[i].gameObject == gameObject)
            {
                continue;
            }
            bool hasRemoved = false;
            Transform fraction = roots[i].transform.GetChild(0);
            if (fraction.GetComponent<SetElement>().numerator > 0 && fraction.GetComponent<SetElement>().partitions == 1)
            {
                fraction.gameObject.SendMessage("DecreaseCutNumerator");
                hasRemoved = true;
                break;
            }
            else if (fraction.GetComponent<SetElement>().partitions > 1 && fraction.GetComponent<SetElement>().partNumerator > 0)
            {
                fraction.gameObject.SendMessage("DecreaseCutNumerator");
                hasRemoved = true;
                break;
            }
            if (hasRemoved)
                break;
        }
    }

    void DecreaseNumR()
    {
        RootElement[] roots = GetComponentsInChildren<RootElement>();
        for (int i = 0; i < roots.Length; i++)
        {
            if (roots[i].gameObject == gameObject)
                continue;

            Transform fraction = roots[i].transform.GetChild(0);

            int num = 0;
            if (type == ElementsType.Line)
                num = fraction.gameObject.GetComponent<LineElement>().partNumerator;
            else if (type == ElementsType.Liquid)
                num = fraction.gameObject.GetComponent<LiquidElement>().partNumerator;
            else if ((type == ElementsType.VRect || type == ElementsType.HRect) && fraction.gameObject.GetComponent<RectangleElement>().partitions > 1)
            {
                num = fraction.gameObject.GetComponent<RectangleElement>().partNumerator;
            }
            else if ((type == ElementsType.VRect || type == ElementsType.HRect) && fraction.gameObject.GetComponent<RectangleElement>().partitions == 1)
            {
                num = fraction.gameObject.GetComponent<RectangleElement>().numerator;
            }
            else if ((type == ElementsType.Set || type == ElementsType.HeartSet || type == ElementsType.StarSet || type == ElementsType.MoonSet) && fraction.gameObject.GetComponent<SetElement>().partitions > 1)
            {
                num = fraction.gameObject.GetComponent<SetElement>().partNumerator;
            }
            else if ((type == ElementsType.Set || type == ElementsType.HeartSet || type == ElementsType.StarSet || type == ElementsType.MoonSet) && fraction.gameObject.GetComponent<SetElement>().partitions == 1)
            {
                num = fraction.gameObject.GetComponent<SetElement>().numerator;
            }

            if (num > 0)
            {
                fraction.gameObject.SendMessage("DecreaseCutNumerator");
                break;
            }
        }
    }

    void DecreaseNum()
    {
        RootElement[] roots = GetComponentsInChildren<RootElement>();
        for (int i = roots.Length - 1; i >= 0; i--)
        {
            if (roots[i].gameObject == gameObject)
                continue;

            Transform fraction = roots[i].transform.GetChild(0);

            int num = 0;
            if (type == ElementsType.Line)
                num = fraction.gameObject.GetComponent<LineElement>().partNumerator;
            else if (type == ElementsType.Liquid)
                num = fraction.gameObject.GetComponent<LiquidElement>().partNumerator;
            else if (type == ElementsType.Set || type == ElementsType.HeartSet || type == ElementsType.StarSet || type == ElementsType.MoonSet)
            {
                num = fraction.gameObject.GetComponent<SetElement>().partNumerator;
            }
            /*else if (type == ElementsType.VRect || type == ElementsType.HRect)
                num = fraction.gameObject.GetComponent<RectangleElement>().partNumerator;*/

            if (num > 0)
            {
                fraction.gameObject.SendMessage("DecreaseCutNumerator");
                break;
            }
        }
    }

    void DecreaseCutNumerator()
    {
        if (!isSubFraction)
        {
            switch (type)
            {

                case (ElementsType.Set):
                case (ElementsType.MoonSet):
                case (ElementsType.HeartSet):
                case (ElementsType.StarSet):
                    DecreaseNumR();
                    //DecreaseRectNum();
                    break;
                case (ElementsType.HRect):
                case (ElementsType.VRect):
                    DecreaseNumR();
                    break;
                case (ElementsType.Line):
                case (ElementsType.Liquid):
                    DecreaseNum();
                    break;
            }
        }
    }

    public void IncreaseNumeratorInChildren()
    {
        for (int i = 0; i < elements.Count; i++)
        {
            RootElement item = elements[i].GetComponent<RootElement>();
            for (int j = 0; j < item.elements.Count; j++)
            {
                if (type == ElementsType.Set || type == ElementsType.HeartSet || type == ElementsType.StarSet || type == ElementsType.MoonSet)
                {
                    SetElement elemSet = item.elements[j].GetComponent<SetElement>();
                    if (elemSet.partNumerator < elemSet.partDenominator)
                    {
                        elemSet.partNumerator++;
                        elemSet.numerator = elemSet.partNumerator / elemSet.partitions;
                        return;
                    }
                }
                else
                {
                    RectangleElement elem = item.elements[j].GetComponent<RectangleElement>();
                    if (elem.partNumerator < elem.partDenominator)
                    {
                        elem.partNumerator++;
                        elem.numerator = elem.partNumerator / elem.partitions;
                        return;
                    }
                }
            }
        }
    }

    public void DecreaseNumeratorInChildren()
    {
        for (int i = elements.Count - 1; i >= 0; i--)
        {
            RootElement item = elements[i].GetComponent<RootElement>();
            for (int j = 0; j < item.elements.Count; j++)
            {
                if (type == ElementsType.Set || type == ElementsType.HeartSet || type == ElementsType.StarSet || type == ElementsType.MoonSet)
                {
                    SetElement elemSet = item.elements[j].GetComponent<SetElement>();
                    if (elemSet.partNumerator <= elemSet.partDenominator)
                    {
                        elemSet.partNumerator--;
                        elemSet.numerator = elemSet.partNumerator / elemSet.partitions;
                        return;
                    }
                }
                else
                {
                    RectangleElement elem = item.elements[j].GetComponent<RectangleElement>();
                    if (elem.partNumerator <= elem.partDenominator)
                    {
                        elem.partNumerator--;
                        elem.numerator = elem.partNumerator / elem.partitions;
                        return;
                    }
                }
            }
        }
    }

    public void UpdateByChildren()
    {
        if (!isSubFraction)
        {
            this.numerator = 0;
            this.partNumerator = 0;
            for (int i = 0; i < elements.Count; i++)
            {
                RootElement item = elements[i].GetComponent<RootElement>();
                item.numerator = 0;
                item.partNumerator = 0;
                for (int j = 0; j < item.elements.Count; j++)
                {
                    if (type == ElementsType.HRect || type == ElementsType.VRect)
                    {
                        RectangleElement elem = item.elements[j].GetComponent<RectangleElement>();
                        item.numerator += elem.numerator;
                        item.partNumerator += elem.partNumerator;
                    }
                    if (type == ElementsType.Set || type == ElementsType.HeartSet || type == ElementsType.StarSet || type == ElementsType.MoonSet)
                    {
                        SetElement elem = item.elements[j].GetComponent<SetElement>();
                        item.numerator += elem.numerator;
                        item.partNumerator += elem.partNumerator;
                    }
                }
                this.numerator += item.numerator;
                this.partNumerator += item.partNumerator;
            }

            symbol.SendMessage("SetNumerator", this.numerator);
            symbol.SendMessage("SetPartNumerator", this.partNumerator);
            ExternalEventsManager.Instance.SendMessageToSupport("FractionChange", "Numerator", root.name, partNumerator);
            Workspace.Instance.RemoveEmptyChildren(gameObject);
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
        currentScaleTween = 0.0f;
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
        //  Debug.Log("TweenMoveToCenterU");
        transform.position = (Vector3)v;
    }
    protected void TweenMoveToCenterC(object v)
    {
        //  Debug.Log("TweenMoveToCenterC");

        transform.position = cutTargetPos;
        Tweener.StopAndDestroyAllTweens();
        isTweening = false;
    }
    #endregion

    protected void InitSet(ElementsType type)
    {
        //Debug.Log("initset");
        float scaleMult = 0.75f;
        width = 1.5f * scaleMult;
        height = 1.0f;

        GameObject elem = new GameObject("fraction");
        elem.transform.parent = transform;
        elem.transform.position = transform.TransformPoint(Vector3.zero);
        elem.AddComponent<SetElement>();
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
        elem.SendMessage("SetRoot", gameObject);
        elem.SendMessage("Initialize");
        elements.Add(elem);
    }

    protected void InitRectangle(ElementsType type)
    {
        ///Just change scaleMult value in order to change the size when creating
        float scaleMult = 0.75f;
        width = 6.0f * scaleMult;
        height = 4.0f * scaleMult;

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
        elem.SendMessage("SetRoot", gameObject);
        elem.SendMessage("Initialize");
        elements.Add(elem);
    }

    protected void InitLine(ElementsType type)
    {
        ///Just change scaleMult value in order to change the size when creating
        float scaleMult = 0.75f;
        width = 7.0f * scaleMult;
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
        elem.SendMessage("SetRoot", gameObject);

        elem.SendMessage("Initialize");
        elements.Add(elem);
    }

    protected void InitLiquid(ElementsType type)
    {
        ///Just change scaleMult value in order to change the size when creating
        float scaleMult = 0.75f;
        width = 5.0f * scaleMult;
        height = 5.0f * scaleMult;

        GameObject elem = new GameObject("fraction");
        elem.transform.parent = transform;
        elem.transform.position = transform.TransformPoint(Vector3.zero);
        LiquidElement comp = elem.AddComponent<LiquidElement>();
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
        elem.SendMessage("SetRoot", gameObject);
        elem.SendMessage("Initialize");
        elements.Add(elem);

        comp.SendMessage("SetElementScale", scaleMult);
        comp.transform.localScale = scaleMult * Vector3.one;
    }

    protected Vector3 GetSymbolPosition(bool isPartition)
    {
        float width = this.width;
        if (null != elements[0].GetComponent<RootElement>())
            width = elements[0].GetComponent<RootElement>().width;

        float baseXPos = (width * 0.5f) * elementScale;
        if (elements[0].GetComponent<RootElement>().type == ElementsType.Set || elements[0].GetComponent<RootElement>().type == ElementsType.HeartSet || elements[0].GetComponent<RootElement>().type == ElementsType.StarSet || elements[0].GetComponent<RootElement>().type == ElementsType.MoonSet)
        {
            baseXPos = 0.75f;
        }
        float baseXOffset = 1.5f;
        float symbolOffset = 2.8f;
        /*
          float baseXOffset = 1.0f;
        float symbolOffset = 1.8f;
         */

        float finalX = baseXPos + baseXOffset;

        if (isPartition)
            finalX += symbolOffset;

        return new Vector3(finalX, 0.0f, 0.0f);
    }

    public Vector3 GetSymbolPosition(bool isPartition, float baseXOffset)
    {
        float width = this.width;
        if (null != elements[0].GetComponent<RootElement>())
            width = elements[0].GetComponent<RootElement>().width;

        float baseXPos = (width * 0.5f) * elementScale;
        if (elements[0].GetComponent<RootElement>().type == ElementsType.Set || elements[0].GetComponent<RootElement>().type == ElementsType.HeartSet || elements[0].GetComponent<RootElement>().type == ElementsType.StarSet || elements[0].GetComponent<RootElement>().type == ElementsType.MoonSet)
        {
            baseXPos = 0.75f;
        }
        float symbolOffset = 2.8f;

        float finalX = baseXPos + (1.5f / baseXOffset);

        if (isPartition)
            finalX += symbolOffset / baseXOffset;

        return new Vector3(finalX, 0.0f, 0.0f);
    }

    protected void AttachModifierPartition()
    {
        if (partitionActive)
        {
            if (null == partMod)
            {
#if !UNITY_IPHONE && !UNITY_ANDROID
                partMod = GameObject.Instantiate(partition_root) as GameObject;
#endif
#if UNITY_IPHONE || UNITY_ANDROID
                partMod = GameObject.Instantiate(partition_root_mobile) as GameObject;
#endif
                partMod.transform.parent = transform;
                Vector3 pos = GetSymbolPosition(true);
                partMod.transform.position = transform.TransformPoint(new Vector3(pos.x, pos.y, pos.z));
                partMod.GetComponent<RectTransform>().localScale = symbol.GetComponent<RectTransform>().localScale;
                partMod.GetComponent<RectTransform>().localPosition = GetSymbolPosition(true, Workspace.Instance.actualFactorScale);

            }
            if (mode != InteractionMode.LookAt)
            {
                AttachButtons(CheckPartition());
                PlaceButtons((int)selectedFractionPart);
            }
            AttachSymbol(true);
        }
        else
        {
            DetachModifierPartition();
        }
        if (!isSubFraction && isHighlighted)
            InitHighlight(name, true);
    }

    protected void SetModifierVisibility(bool vis)
    {
        if (partMod != null)
        {
            partMod.SetActive(vis);
            if (vis)
                partMod.GetComponent<PartitionMCElement>().ShowButtons();
            else
                partMod.GetComponent<PartitionMCElement>().HideButtons();

        }
    }

    protected void DetachModifierPartition()
    {
        //Debug.Log("DetachModifierPartition");
        DestroyImmediate(partMod);
        partMod = null;
        if (!isSubFraction && isHighlighted)
            InitHighlight(name, true);
    }

    protected void AttachSymbol(bool isNotEditable)
    {
        if (null == symbol_root || state == ElementsState.Cut)
            return;
        if (null == symbol)
        {
#if !UNITY_IPHONE && !UNITY_ANDROID
            symbol = GameObject.Instantiate(symbol_root) as GameObject;
#endif
#if UNITY_IPHONE || UNITY_ANDROID
            symbol = GameObject.Instantiate(symbol_root_mobile) as GameObject;
#endif
            symbol.transform.parent = transform;
            initialScaleModifier = symbol.GetComponent<RectTransform>().localScale;
            Vector3 pos = GetSymbolPosition(false);
            symbol.transform.position = transform.TransformPoint(new Vector3(pos.x, pos.y, pos.z));
            symbol.SendMessage("SetNumerator", numerator);
            symbol.SendMessage("SetDenominator", denominator);
            symbol.SendMessage("SetPartDenominator", partDenominator);
            symbol.SendMessage("SetPartitions", partitions);
            symbol.SendMessage("SetPartNumerator", partNumerator);
            float wsFactorScale = Workspace.Instance.actualFactorScale;
            symbol.GetComponent<RectTransform>().localScale = new Vector3(symbol.GetComponent<RectTransform>().localScale.x / wsFactorScale, symbol.GetComponent<RectTransform>().localScale.y / wsFactorScale, symbol.GetComponent<RectTransform>().localScale.z);
            symbol.GetComponent<RectTransform>().localPosition = GetSymbolPosition(false, wsFactorScale);
        }

    }

    protected void DetachSymbol()
    {
        Destroy(symbol);
        symbol = null;
    }

    public void AttachButtons(bool isEnabled)
    {
    
        if (isSubFraction)
            return;
        if (mode == InteractionMode.Changing || mode == InteractionMode.Initializing)
        {
            symbol.GetComponent<SingleFractionMCElement>().ChangeStateButtons(isEnabled);
        }
        else if (mode == InteractionMode.Partitioning)
        {
            partMod.GetComponent<PartitionMCElement>().ChangeStateButtons(isEnabled);
        }
    }

    public void DetachButtons()
    {
        if (null != symbol && mode != InteractionMode.Changing)
            symbol.GetComponent<SingleFractionMCElement>().HideButtons();
        if (null != partMod && mode != InteractionMode.Partitioning)
            partMod.GetComponent<PartitionMCElement>().HideButtons();
        /* Destroy(btnUp);
         btnUp = null;
         Destroy(btnDown);
         btnDown = null;*/
    }

    public void AttachModifierSymbol()
    {
        AttachSymbol(true);
        if (mode == InteractionMode.LookAt)
            return;
        if (null != partMod)
            partMod.GetComponent<PartitionMCElement>().HideButtons();
        AttachButtons(true);
        PlaceButtons((int)selectedFractionPart);
    }

    void SetPartitioning()
    {
        //Debug.Log("setpartitioning");
        partitionActive = !partitionActive;
        // InteractionMode lastmode = mode;
        SetMode(partitionActive ? InteractionMode.Partitioning : InteractionMode.Moving);
        if (!partitionActive)
            DetachModifierPartition();
       /* if (!isSubFraction && isHighlighted)
            InitHighlight(name, true);*/
    
    }

    void SetHighlight()
    {
        if (isHighlighted)
        {
            Destroy(highlight);
            isHighlighted = false;
            ExternalEventsManager.Instance.SendMessageToSupport("DehighlightFraction", name);
        }
        else
        {
            isHighlighted = true;
            ExternalEventsManager.Instance.SendMessageToSupport("HighlightFraction", name);
            if (null == highlight)
                highlight = GameObject.Instantiate(Workspace.Instance.highlight_prefab) as GameObject;
            InitHighlight(name, true);
            Debug.Log("Sethighlight");
        }
    }

    public void PlaceButtons(int _selectedFractionPart)
    {
        if (mode == InteractionMode.LookAt)
            return;
        // Debug.Log("PlaceButton");
        /*  if( btnDown == null || btnUp == null)
              this.AttachButtons(true);*/

        if (mode == InteractionMode.Changing || mode == InteractionMode.Initializing)
        {
            if (_selectedFractionPart == (int)FractionPart.Denominator)
            {
                //  btnUp.transform.position = symbol.transform.TransformPoint(new Vector3(0.9f, -0.3f, 0.0f));
                //  btnDown.transform.position = symbol.transform.TransformPoint(new Vector3(0.9f, -0.9f, 0.0f));
                if (mode != InteractionMode.Initializing)
                    Workspace.Instance.interfaces.SendMessage("ShowSuggestion", "{hint_denominator}");
            }
            if (_selectedFractionPart == (int)FractionPart.Numerator)
            {
                // btnUp.transform.position = symbol.transform.TransformPoint(new Vector3(0.9f, 0.8f, 0.0f));
                // btnDown.transform.position = symbol.transform.TransformPoint(new Vector3(0.9f, 0.2f, 0.0f));
                Workspace.Instance.interfaces.SendMessage("ShowSuggestion", "{hint_numerator}");
            }
        }
        else if (mode == InteractionMode.Partitioning)
        {
            // btnUp.transform.position = partMod.transform.TransformPoint(new Vector3(0.9f, 0.3f, 0.0f));
            // btnDown.transform.position = partMod.transform.TransformPoint(new Vector3(0.9f, -0.3f, 0.0f));
            Workspace.Instance.interfaces.SendMessage("ShowSuggestion", "{hint_partition_num}");
        }
    }

    public void DetachModifierSymbol()
    {
        DetachSymbol();
        if (isSymbolShown)
            AttachSymbol(true);

        DetachButtons();
    }

    protected void AttachModifierScale()
    {

       /* scaleModParent = new GameObject("scale_root");
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

        PlaceModifierScale();*/
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

    public Vector3 initialScaleModifier;
    public void RescaleModifier(float factorScale)
    {
        ActualfactorScale = factorScale;
        if (null != symbol)
        {
            symbol.GetComponent<RectTransform>().localScale = new Vector3(0.03333334f / factorScale, 0.03333334f / factorScale, 1.0f);
            symbol.GetComponent<RectTransform>().localPosition = GetSymbolPosition(false, factorScale);
        }
        if (null != partMod)
        {
            partMod.GetComponent<RectTransform>().localScale = new Vector3(0.03333334f / factorScale, 0.03333334f / factorScale, 1.0f);
            partMod.GetComponent<RectTransform>().localPosition = GetSymbolPosition(true, factorScale);

        }
        Draw(zIndex);
    }
    #endregion
}
