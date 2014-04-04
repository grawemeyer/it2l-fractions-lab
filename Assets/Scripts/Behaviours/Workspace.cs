using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SBS.Math;
using fractionslab.meshes;
using fractionslab.utils;
using fractionslab.behaviours;

[AddComponentMenu("FractionsLab/WorkspaceBehaviour")]
public class Workspace : MonoBehaviour
{
    #region Singleton
    protected static Workspace instance = null;
    public static Workspace Instance
    {
        get
        {
            if (null == instance)
                instance = GameObject.FindGameObjectWithTag("Workspace").GetComponent<Workspace>();
            return instance;
        }
    }
    #endregion

    #region Static/Const Fields
    public static int elemCounter = 0;
    #endregion

    #region Public Fields
    public GameObject interfaces;
    public GUISkin skin;
    public Color systemColor = new Color(0.2f, 0.2f, 0.2f, 1.0f);
    public Color grey = new Color(0.5490f, 0.5843f, 0.5294f, 1.0f);
    public Color white = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    public Color black = new Color(0.0f, 0.0f, 0.0f, 1.0f);
    public Color green = new Color(0.2431f, 0.4039f, 0.1686f, 1.0f);
    public Color greenResult = new Color(0.3373f, 0.5294f, 0.2392f, 1.0f);
    public Color greyResult = new Color(0.6863f, 0.6588f, 0.5569f, 1.0f);
    public GameObject liquidSource;
    #endregion

    #region Protected Fields
    protected int colorCounter = 0;
    protected SBSBounds bounds;
    protected WSList<GameObject> elements;
    protected List<Color> colorList = new List<Color>();
    [SerializeField]
    protected ActionType currentAction;
    [SerializeField]
    protected GameObject firstCut = null;
    [SerializeField]
    protected GameObject secondCut = null;

    [SerializeField]
    protected bool inputEnabled = true;
    #endregion

    #region Public Get/Set
    public SBSBounds Bounds
    {
        get
        {
            return bounds;
        }
    }

    public GameObject ElementOnFocus
    {
        get
        {
            if (elements.Count > 0)
                return elements[0];
            return null;
        }
    }

    public bool OperationPending
    {
        get
        {
            return (null != firstCut);
        }
    }

    public GameObject FirstCut
    {
        get
        {
            return firstCut;
        }
    }

    public GameObject SecondCut
    {
        get
        {
            return secondCut;
        }
    }
    #endregion

    #region Unity Callbacks
    void Awake()
    {
        elements = new WSList<GameObject>();
        float widthSize = Camera.main.orthographicSize * 2.0f * Camera.main.aspect;
        float heightSize = widthSize / Camera.main.aspect;
        bounds = new SBSBounds(Vector3.zero, Vector3.right * widthSize + Vector3.up * heightSize + Vector3.forward * float.MaxValue);

        colorList.Add(new Color(0.7373f, 0.0078f, 0.0078f, 0.5f));
        colorList.Add(new Color(1.0f, 0.7059f, 0.0f, 0.5f));
        colorList.Add(new Color(0.2431f, 0.6627f, 0.9882f, 0.5f));
        colorList.Add(new Color(0.6588f, 0.4078f, 1.0f, 0.5f));

        //CreatePopup();
        UpdateWS();
    }

    void OnMouseDown()
    {
//#if !UNITY_IPHONE || UNITY_EDITOR
        OnMouseDownEvent();
//#endif
    }

    void OnMouseDownEvent()
    {
        if (!inputEnabled)
            return;

        if (ElementOnFocus != null)
        {
            if (ElementOnFocus.GetComponent<RootElement>().denominator > 0)
            {
                ElementOnFocus.SendMessage("SetMode", InteractionMode.Moving);
                interfaces.SendMessage("ShowSuggestion", "");
                interfaces.SendMessage("ShowHint", "");
                EnableInput();

                if (OperationPending)
                    firstCut = null;
            }
        }
    }

    void Update()
    {
#if UNITY_IPHONE
        /*if (Input.touchCount > 0)
        {
            Touch touch = Input.touches[0];
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            RaycastHit hit ;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider == collider)
                {
                    switch (touch.phase)
                    {
                        case (TouchPhase.Began):
                            OnMouseDownEvent();
                            break;
                        case (TouchPhase.Moved):
                        case (TouchPhase.Stationary):
                        case (TouchPhase.Ended):
                        case (TouchPhase.Canceled):
                            break;
                    }
                }
            }
        }*/
#endif
    }
    #endregion

    #region Messages
    GameObject CreateSingleFraction(string name, Element element)
    {
        GameObject root = new GameObject(name);
        root.transform.parent = transform;
        root.transform.position = element.position;
        root.AddComponent<RootElement>();
        root.SendMessage("SetElementState", element.state);
        root.SendMessage("SetNumerator", element.numerator);
        root.SendMessage("SetDenominator", element.denominator);
        root.SendMessage("SetPartitions", element.partitions);
        root.SendMessage("SetPartNumerator", element.partNumerator);
        root.SendMessage("SetPartDenominator", element.partDenominator);

        if (element.denominator == 0)
        {
            root.SendMessage("SetMode", InteractionMode.Initializing);
        }
        else
        {
            //interfaces.SendMessage("EnableHUD");
            root.SendMessage("SetMode", InteractionMode.Moving);
        }

        root.SendMessage("SetColor", element.color);

        return root;
    }

    GameObject CreateVRect(Element element)
    {
        GameObject root = CreateSingleFraction("vrect_" + (++elemCounter), element);

        if (element.partNumerator > element.partDenominator)
        {
            int diff = element.partNumerator - element.partDenominator;

            Element elem1 = element;
            elem1.numerator = elem1.denominator;
            elem1.partNumerator = elem1.partDenominator;

            Element elem2 = element;
            elem2.partNumerator = diff;
            elem2.numerator = elem2.partNumerator / element.partitions;

            float gap = 0.4f;

            GameObject child1 = CreateSingleFraction("vrect_child1", elem1);
            child1.transform.position = element.position - new SBSVector3(3.0f + gap * 0.5f, 0.0f, 0.0f);
            child1.transform.parent = root.transform;
            child1.SendMessage("InitializeAs", element.type);
            child1.SendMessage("SetEnableCollider", false);

            GameObject child2 = CreateSingleFraction("vrect_child2", elem2);
            child2.transform.position = element.position + new SBSVector3(3.0f + gap * 0.5f, 0.0f, 0.0f);
            child2.transform.parent = root.transform;
            child2.SendMessage("InitializeAs", element.type);
            child2.SendMessage("SetEnableCollider", false);

        }
        else
        {
            root.SendMessage("InitializeAs", element.type);
        }

        elements.Push(root);
        UpdateWS();

        ExternalEventsManager.Instance.SendMessageToSupport("FractionGenerated", "VRects", root.name);

        return root;
    }

    GameObject CreateHRect(Element element)
    {
        GameObject root = CreateSingleFraction("vrect_" + (++elemCounter), element);

        if (element.partNumerator > element.partDenominator)
        {
            int diff = element.partNumerator - element.partDenominator;

            Element elem1 = element;
            elem1.numerator = elem1.denominator;
            elem1.partNumerator = elem1.partDenominator;

            Element elem2 = element;
            elem2.partNumerator = diff;
            elem2.numerator = elem2.partNumerator / element.partitions;

            float gap = 0.4f;

            GameObject child1 = CreateSingleFraction("hrect_child1", elem1);
            child1.transform.position = element.position - new SBSVector3(3.0f + gap * 0.5f, 0.0f, 0.0f);
            child1.transform.parent = root.transform;
            child1.SendMessage("InitializeAs", element.type);
            child1.SendMessage("SetEnableCollider", false);

            GameObject child2 = CreateSingleFraction("hrect_child2", elem2);
            child2.transform.position = element.position + new SBSVector3(3.0f + gap * 0.5f, 0.0f, 0.0f);
            child2.transform.parent = root.transform;
            child2.SendMessage("InitializeAs", element.type);
            child2.SendMessage("SetEnableCollider", false);

        }
        else
        {
            root.SendMessage("InitializeAs", element.type);
        }

        elements.Push(root);
        UpdateWS();

        ExternalEventsManager.Instance.SendMessageToSupport("FractionGenerated", "HRects", root.name);

        return root;
    }

    GameObject CreateNumberedLine(Element element)
    {
        GameObject root = CreateSingleFraction("line_" + (++elemCounter), element);

        if (element.partNumerator > element.partDenominator)
        {
            int diff = element.partNumerator - element.partDenominator;

            Element elem1 = element;
            elem1.numerator = elem1.denominator;
            elem1.partNumerator = elem1.partDenominator;

            Element elem2 = element;
            elem2.partNumerator = diff;
            elem2.numerator = elem2.partNumerator / element.partitions;

            GameObject child1 = CreateSingleFraction("line_child1", elem1);
            child1.transform.position = element.position - new SBSVector3(3.5f, 0.0f, 0.0f);
            child1.transform.parent = root.transform;
            child1.SendMessage("SetFractionBaseOffset", 0);
            child1.SendMessage("SetBBExtends", new BBExtend(0.0f, 1.0f, 0.0f, 1.0f));
            child1.SendMessage("InitializeAs", element.type);
            child1.SendMessage("SetEnableCollider", false);

            GameObject child2 = CreateSingleFraction("line_child2", elem2);
            child2.transform.position = element.position + new SBSVector3(3.5f, 0.0f, 0.0f);
            child2.transform.parent = root.transform;
            child2.SendMessage("SetFractionBaseOffset", 1);
            child2.SendMessage("SetBBExtends", new BBExtend(0.0f, 1.0f, 0.0f, 1.0f));
            child2.SendMessage("InitializeAs", element.type);
            child2.SendMessage("SetEnableCollider", false);

        }
        else
        {
            root.SendMessage("SetFractionBaseOffset", 0);
            root.SendMessage("SetBBExtends", new BBExtend(0.0f, 1.0f, 0.0f, 1.0f));
            root.SendMessage("InitializeAs", element.type);
        }

        elements.Push(root);
        UpdateWS();

        ExternalEventsManager.Instance.SendMessageToSupport("FractionGenerated", "NumberedLines", root.name);

        return root;
    }

    GameObject CreateLiquidMeasures(Element element)
    {
        GameObject root = CreateSingleFraction("liquid_" + (++elemCounter), element);

        if (element.partNumerator > element.partDenominator)
        {
            int diff = element.partNumerator - element.partDenominator;

            Element elem1 = element;
            elem1.numerator = elem1.denominator;
            elem1.partNumerator = elem1.partDenominator;

            Element elem2 = element;
            elem2.partNumerator = diff;
            elem2.numerator = elem2.partNumerator / element.partitions;

            float gap = 0.4f;

            GameObject child1 = CreateSingleFraction("liquid_child1", elem1);
            child1.transform.position = element.position - new SBSVector3(3.0f + gap * 0.5f, 0.0f, 0.0f);
            child1.transform.parent = root.transform;
            child1.SendMessage("InitializeAs", element.type);
            child1.SendMessage("SetEnableCollider", false);

            GameObject child2 = CreateSingleFraction("liquid_child2", elem2);
            child2.transform.position = element.position + new SBSVector3(3.0f + gap * 0.5f, 0.0f, 0.0f);
            child2.transform.parent = root.transform;
            child2.SendMessage("InitializeAs", element.type);
            child2.SendMessage("SetEnableCollider", false);

        }
        else
        {
            root.SendMessage("InitializeAs", element.type);
        }

        elements.Push(root);
        UpdateWS();

        ExternalEventsManager.Instance.SendMessageToSupport("FractionGenerated", "LiquidMeasures", root.name);

        return root;
    }

    /*void CreateLiquidMeasures(Element element)
    {
        GameObject root = new GameObject("liquid_" + (++elemCounter));
        root.transform.parent = transform;
        root.transform.position = element.position;
        root.AddComponent<RootElement>();
        root.SendMessage("SetMode", InteractionMode.Initializing);
        root.SendMessage("SetColor", element.color);
        root.SendMessage("SetElementState", element.state);
        root.SendMessage("InitializeAs", element.type);

        elements.Push(root);
        UpdateWS();

        ExternalEventsManager.Instance.SendBrowserMessage("FractionGenerated", "LiquidMeasure", root.name);
    }*/

    void CreatePopup()
    {
        GameObject slice = new GameObject("popup");
        slice.transform.parent = transform;
        slice.transform.position = Vector3.zero;

        Mesh mesh;
        MeshUtils.CreateRectangle(18.0f, 12.0f, Workspace.Instance.white, out mesh);
        slice.GetComponent<MeshFilter>().mesh = mesh;
        slice.renderer.material.SetColor("_Color", Workspace.Instance.white);
    }

    void CreateCopy(GameObject source)
    {
        GameObject root = Instantiate(source) as GameObject;
        string[] splitted = source.name.Split('_');
        root.name = splitted[0] + "_" + (++elemCounter);
        root.transform.parent = transform;
        float xOffset = (root.transform.position.x > 0.0f) ? -1.0f : 1.0f;
        float yOffset = (root.transform.position.y > 0.0f) ? -1.0f : 1.0f;
        root.transform.position += new Vector3(xOffset, yOffset, 0.0f);
        root.BroadcastMessage("SetType", source.GetComponent<RootElement>().type, SendMessageOptions.DontRequireReceiver);
        root.BroadcastMessage("SetColor", GetColor(), SendMessageOptions.DontRequireReceiver);
        root.BroadcastMessage("SetBBExtends", source.GetComponent<RootElement>().bbExtends);
        root.BroadcastMessage("DetachModifierSymbol");

        elements.Push(root);
        UpdateWS();

        ExternalEventsManager.Instance.SendBrowserMessage("FractionCopy", root.name);
    }

    void CutFraction(GameObject source)
    {
        GameObject root = Instantiate(source) as GameObject;
        root.name = source.name + "_cut";
        root.transform.parent = transform;
        float xOffset = (root.transform.position.x > 0.0f) ? -1.0f : 1.0f;
        float yOffset = (root.transform.position.y > 0.0f) ? -1.0f : 1.0f;
        root.transform.position += new Vector3(xOffset, yOffset, 0.0f);
        root.GetComponent<RootElement>().parentRef = source;
        root.BroadcastMessage("SetType", source.GetComponent<RootElement>().type, SendMessageOptions.DontRequireReceiver);
        root.BroadcastMessage("SetColor", source.GetComponent<RootElement>().color, SendMessageOptions.DontRequireReceiver);
        root.BroadcastMessage("SetElementState", ElementsState.Cut, SendMessageOptions.DontRequireReceiver);
        root.BroadcastMessage("SetBBExtends", source.GetComponent<RootElement>().bbExtends);
        root.BroadcastMessage("DetachModifierSymbol");

        root.GetComponent<RootElement>().Cut();

        elements.Push(root);

        source.BroadcastMessage("SetMode", InteractionMode.Freeze);
        UpdateWS();

        ExternalEventsManager.Instance.SendBrowserMessage("FractionCut", root.name);
    }

    GameObject CreateProperFractions(GameObject f1, GameObject f2)
    {
        Element element = new Element();
        element.position = Vector3.zero;
        element.color = Workspace.Instance.greenResult;
        element.type = f1.GetComponent<RootElement>().type;
        element.state = ElementsState.Result;

        if (currentAction == ActionType.Join)
            element.partNumerator = f1.GetComponent<RootElement>().partNumerator + f2.GetComponent<RootElement>().partNumerator;
        else
            element.partNumerator = f1.GetComponent<RootElement>().partNumerator - f2.GetComponent<RootElement>().partNumerator;

        element.partDenominator = f1.GetComponent<RootElement>().partDenominator;
        element.numerator = element.partNumerator / firstCut.GetComponent<RootElement>().partitions;
        element.denominator = f1.GetComponent<RootElement>().denominator;
        element.partitions = f1.GetComponent<RootElement>().partitions;

        GameObject root = null;
        switch(element.type)
        {
            case (ElementsType.HRect):
                root = CreateHRect(element);
                break;
            case (ElementsType.VRect):
                root = CreateVRect(element);
                break;
            case (ElementsType.Line):
                root = CreateNumberedLine(element);
                break;
            case (ElementsType.Liquid):
                root = CreateLiquidMeasures(element);
                break;
        }

        root.SendMessage("SetMode", InteractionMode.Wait);
        f1.GetComponent<RootElement>().parentRef.BroadcastMessage("SetMode", InteractionMode.Moving);
        f2.GetComponent<RootElement>().parentRef.BroadcastMessage("SetMode", InteractionMode.Moving);
        elements.Remove(f1);
        elements.Remove(f2);

        Destroy(firstCut);
        Destroy(secondCut);
        firstCut = null;
        secondCut = null;

        UpdateWS();
        return root;
    }

    GameObject CreateImproperFractions(GameObject f1, GameObject f2)
    {
        GameObject root = new GameObject("result_" + (++elemCounter));
        root.transform.parent = transform;
        root.transform.position = Vector3.zero;
        RootElement rootComp = root.AddComponent<RootElement>();
        root.SendMessage("SetMode", InteractionMode.Wait);
        root.SendMessage("SetColor", grey);
        root.SendMessage("SetElementState", ElementsState.Improper);

        f1.GetComponent<RootElement>().parentRef.BroadcastMessage("SetMode", InteractionMode.Moving);
        f2.GetComponent<RootElement>().parentRef.BroadcastMessage("SetMode", InteractionMode.Moving);
        f1.SendMessage("SetMode", InteractionMode.Moving);
        f2.SendMessage("SetMode", InteractionMode.Moving);

        f1.transform.parent = root.transform;
        f2.transform.parent = root.transform;

        Vector3 f1Pos = new Vector3(f1.transform.position.x, f1.transform.position.y, -0.2f);
        Vector3 f2Pos = new Vector3(f2.transform.position.x, f2.transform.position.y, -0.2f);
        f1.transform.position = f1Pos;
        f2.transform.position = f2Pos;

        elements.Remove(f1);
        elements.Remove(f2);

        f1.SendMessage("SetEnableCollider", false);
        f2.SendMessage("SetEnableCollider", false);

        elements.Push(root);
        UpdateWS();

        switch (rootComp.type)
        {
            case (ElementsType.HRect):
                ExternalEventsManager.Instance.SendMessageToSupport("ImproperGenerated", "HRects", rootComp.name);
                break;
            case (ElementsType.VRect):
                ExternalEventsManager.Instance.SendMessageToSupport("ImproperGenerated", "HRects", rootComp.name);
                break;
            case (ElementsType.Liquid):
                ExternalEventsManager.Instance.SendMessageToSupport("ImproperGenerated", "LiquidMeasures", rootComp.name);
                break;
            case (ElementsType.Line):
                ExternalEventsManager.Instance.SendMessageToSupport("ImproperGenerated", "NumberedLines", rootComp.name);
                break;
        }

        firstCut = null;
        secondCut = null;

        return root;
    }

    void SetFocusOn(GameObject elem)
    {
        if (ElementOnFocus.GetComponent<RootElement>().denominator > 0 || ElementOnFocus.GetComponent<RootElement>().state == ElementsState.Improper)
        {
            elements.Pull(elem);
            UpdateWS();
        }
    }

    void DeleteElement(GameObject element)
    {
        if (null != element.GetComponent<RootElement>().parentRef)
            element.GetComponent<RootElement>().parentRef.BroadcastMessage("SetMode", InteractionMode.Moving);

        Tweener.StopAndDestroyAllTweens();
        Destroy(element);
        elements.Remove(element);
        UpdateWS();
    }

    void EnableInput()
    {
        inputEnabled = true;
        for (int i = 0; i < elements.Count; i++)
            elements[i].BroadcastMessage("EnableInput", SendMessageOptions.DontRequireReceiver);
    }

    void DisableInput()
    {
        inputEnabled = false;
        for (int i = 0; i < elements.Count; i++)
            elements[i].BroadcastMessage("DisableInput", SendMessageOptions.DontRequireReceiver);
    }

    void SetCurrenAction(ActionType action)
    {
        currentAction = action;
    }

    void SetFirstOperationCut(GameObject fc)
    {
        firstCut = fc;
    }

    void SetSecondOperationCut(GameObject sc)
    {
        secondCut = sc;
    }
    #endregion

    #region Actions Management
    #region Messages
    void ResetAction()
    {
        if (null != firstCut)
        {
            firstCut.SendMessage("SetMode", InteractionMode.Moving);
            firstCut = null;
        }

        if (null != secondCut)
        {
            secondCut.SendMessage("SetMode", InteractionMode.Moving);
            secondCut = null;
        }
    }

    void StartCurrentAction()
    {
        resultObject = null;
        RootElement firstCutRoot = firstCut.GetComponent<RootElement>();
        RootElement secondCutRoot = secondCut.GetComponent<RootElement>();

        bool result = CheckActionValidity(firstCutRoot, secondCutRoot);
        if (result)
        {
            if (CheckDenominator(firstCutRoot, secondCutRoot))
            {
                firstCutRoot.ResetScale();
                secondCutRoot.ResetScale();

                switch (currentAction)
                {
                    case (ActionType.Join):
                        StartCoroutine(MakeProperJoin());
                        break;
                    case (ActionType.Compare):
                        StartCoroutine(MakeProperCompare());
                        break;
                    case (ActionType.TakingAway):
                        StartCoroutine(MakeProperTakingAway());
                        break;
                }
            }
            else
            {
                StartCoroutine(MakeImproprer());
            }
        }
        else
        {
            interfaces.SendMessage("ShowFeedbackPopup", "{action_not_valid}");
            switch (currentAction)
            {
                case (ActionType.Join):
                    ExternalEventsManager.Instance.SendMessageToSupport("JoinError", firstCut.name, secondCut.name);
                    break;
                case (ActionType.Compare):
                    ExternalEventsManager.Instance.SendMessageToSupport("CompareError", firstCut.name, secondCut.name);
                    break;
                case (ActionType.TakingAway):
                    ExternalEventsManager.Instance.SendMessageToSupport("TakingAwayError", firstCut.name, secondCut.name);
                    break;
            }
        }
    }

    void TerminateCurrentAction()
    {
        if (null != firstCut && null != secondCut)
        {
            RootElement firstCutRoot = firstCut.GetComponent<RootElement>();
            RootElement secondCutRoot = secondCut.GetComponent<RootElement>();

            if (firstCutRoot.type == ElementsType.Liquid)
            {
                Vector3 basePos = new Vector3(0.0f, -firstCut.GetComponent<RootElement>().height * 0.5f, 0.0f);
                firstCut.SendMessage("SetCoord", basePos);
                firstCut.SendMessage("MoveToCoord", -1.0f);
                secondCut.SendMessage("SetCoord", basePos + new Vector3(0.0f, firstCut.GetComponent<RootElement>().height * 0.5f + secondCut.GetComponent<RootElement>().height * 0.5f + 0.2f, 0.0f));
                secondCut.SendMessage("MoveToCoord", -1.0f);
            }
            else
            {
                firstCut.SendMessage("MoveToPopup", -1.0f);
                secondCut.SendMessage("MoveToPopup", -1.0f);
            }

            StopAllCoroutines();
            if (resultObject == null)
            {
                if(CheckDenominator(firstCutRoot, secondCutRoot))
                    resultObject = CreateProperFractions(firstCut, secondCut);
                else
                    resultObject = CreateImproperFractions(firstCut, secondCut);
            }
        }

        resultObject.SendMessage("SetMode", InteractionMode.Moving);
    }
    #endregion

    #region Coroutines
    IEnumerator MakeProperJoin()
    {
        ExternalEventsManager.Instance.SendMessageToSupport("ProperJoin", firstCut.name, secondCut.name);
        interfaces.SendMessage("ShowActionPopup");

        switch (firstCut.GetComponent<RootElement>().type)
        {
            case (ElementsType.HRect):
            case (ElementsType.VRect):
            case (ElementsType.Line):
                firstCut.SendMessage("MoveToPopup", 1.0f);
                yield return new WaitForSeconds(1.2f);
                secondCut.SendMessage("MoveToPopup", 1.0f);
                yield return new WaitForSeconds(2.2f);
                break;
            case (ElementsType.Liquid):
                Vector3 basePos = new Vector3(0.0f, -firstCut.GetComponent<RootElement>().height * 0.5f, 0.0f);
                firstCut.SendMessage("SetCoord", basePos);
                firstCut.SendMessage("MoveToCoord", 1.0f);
                yield return new WaitForSeconds(1.2f);
                secondCut.SendMessage("SetCoord", basePos + new Vector3(0.0f, firstCut.GetComponent<RootElement>().height * 0.5f + secondCut.GetComponent<RootElement>().height * 0.5f, 0.0f));
                secondCut.SendMessage("MoveToCoord", 1.0f);
                yield return new WaitForSeconds(2.2f);
                break;
        }

        resultObject = CreateProperFractions(firstCut, secondCut);
        SetFocusOn(resultObject);
    }

    IEnumerator MakeProperCompare()
    {
        ExternalEventsManager.Instance.SendMessageToSupport("ProperCompare", firstCut.name, secondCut.name);
        interfaces.SendMessage("ShowActionPopup");

        switch (firstCut.GetComponent<RootElement>().type)
        {
            case (ElementsType.Line):
                firstCut.SendMessage("SetCoord", Vector3.zero);
                firstCut.SendMessage("MoveToCoord", 1.0f);
                yield return new WaitForSeconds(1.2f);

                float diff = (firstCut.GetComponent<RootElement>().width - secondCut.GetComponent<RootElement>().width) * 0.5f;
                secondCut.SendMessage("SetCoord", Vector3.zero + new Vector3(diff, 0.0f, -0.2f));
                secondCut.SendMessage("MoveToCoord", 1.0f);

                yield return new WaitForSeconds(2.2f);

                int count = secondCut.GetComponent<RootElement>().partNumerator;
                for (int i = 0; i < count; i++)
                {
                    firstCut.BroadcastMessage("DecreaseCutNumerator");
                    secondCut.BroadcastMessage("DecreaseCutNumerator");
                }
                break;
            case (ElementsType.Liquid):
                firstCut.SendMessage("SetCoord", Vector3.zero);
                firstCut.SendMessage("MoveToCoord", 1.0f);
                yield return new WaitForSeconds(1.2f);

                diff = (firstCut.GetComponent<RootElement>().height - secondCut.GetComponent<RootElement>().height) * 0.5f;
                secondCut.SendMessage("SetCoord", Vector3.zero + new Vector3(0.0f, diff, -0.2f));
                secondCut.SendMessage("MoveToCoord", 1.0f);

                yield return new WaitForSeconds(2.2f);

                count = secondCut.GetComponent<RootElement>().partNumerator;
                for (int i = 0; i < count; i++)
                {
                    firstCut.BroadcastMessage("DecreaseCutNumerator");
                    secondCut.BroadcastMessage("DecreaseCutNumerator");
                }
                break;
        }
        yield return new WaitForSeconds(2.0f);

        resultObject = CreateProperFractions(firstCut, secondCut);
        SetFocusOn(resultObject);
    }

    IEnumerator MakeProperTakingAway()
    {
        ExternalEventsManager.Instance.SendMessageToSupport("ProperTakingAway", firstCut.name, secondCut.name);
        interfaces.SendMessage("ShowActionPopup");

        switch (firstCut.GetComponent<RootElement>().type)
        {
            case (ElementsType.HRect):
            case (ElementsType.VRect):
                firstCut.SendMessage("MoveToPopup", 1.0f);
                yield return new WaitForSeconds(1.2f);
                secondCut.SendMessage("MoveToPopup", 1.0f);
                yield return new WaitForSeconds(2.2f);
                yield return StartCoroutine(RectangleTakingAway());
                break;
            case (ElementsType.Line):
                firstCut.SendMessage("SetCoord", Vector3.zero - new Vector3(0.0f, 0.5f, 0.0f));
                firstCut.SendMessage("MoveToCoord", 1.0f);
                yield return new WaitForSeconds(1.2f);

                float diff = (firstCut.GetComponent<RootElement>().width - secondCut.GetComponent<RootElement>().width) * 0.5f;
                secondCut.SendMessage("SetCoord", Vector3.zero + new Vector3(diff, 0.5f, 0.0f));
                secondCut.SendMessage("MoveToCoord", 1.0f);

                yield return new WaitForSeconds(2.2f);
                yield return StartCoroutine(TakingAway());
                break;
            case (ElementsType.Liquid):
                diff = (firstCut.GetComponent<RootElement>().height - secondCut.GetComponent<RootElement>().height) * 0.5f;
                float hOffset = firstCut.GetComponent<RootElement>().width * 1.1f * 0.5f;

                firstCut.SendMessage("SetCoord", Vector3.zero + new Vector3(-hOffset, 0.0f, -0.2f));
                firstCut.SendMessage("MoveToCoord", 1.0f);
                yield return new WaitForSeconds(1.2f);

                secondCut.SendMessage("SetCoord", Vector3.zero + new Vector3(hOffset, -diff, -0.2f));
                secondCut.SendMessage("MoveToCoord", 1.0f);

                yield return new WaitForSeconds(2.2f);
                yield return StartCoroutine(TakingAway());
                break;
        }
        yield return new WaitForSeconds(1.0f);

        resultObject = CreateProperFractions(firstCut, secondCut);
        SetFocusOn(resultObject);
    }

    IEnumerator RectangleTakingAway()
    {
        for (int i = 0; i < secondCut.GetComponent<RootElement>().partNumerator; i++)
        {
            firstCut.transform.GetChild(0).GetChild(i).gameObject.SetActive(false);
            secondCut.transform.GetChild(0).GetChild(i).gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator TakingAway()
    {
        int count = secondCut.GetComponent<RootElement>().partNumerator;
        for (int i = 0; i < count; i++)
        {
            firstCut.BroadcastMessage("DecreaseCutNumerator");
            secondCut.BroadcastMessage("DecreaseCutNumerator");
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator MakeImproprer()
    {
        switch (currentAction)
        {
            case (ActionType.Join):
                ExternalEventsManager.Instance.SendMessageToSupport("ImproperJoin", firstCut.name, secondCut.name);
                break;
            case (ActionType.Compare):
                ExternalEventsManager.Instance.SendMessageToSupport("ImproperCompare", firstCut.name, secondCut.name);
                break;
            case (ActionType.TakingAway):
                ExternalEventsManager.Instance.SendMessageToSupport("ImproperTakingAway", firstCut.name, secondCut.name);
                break;
        }

        interfaces.SendMessage("ShowActionPopup");
        if (firstCut.GetComponent<RootElement>().type != ElementsType.Liquid)
        {
            firstCut.SendMessage("MoveToPopup", 1.0f);
            yield return new WaitForSeconds(1.2f);
            secondCut.SendMessage("MoveToPopup", 1.0f);
            yield return new WaitForSeconds(1.2f);
        }
        else
        {
            Vector3 basePos = new Vector3(0.0f, -firstCut.GetComponent<RootElement>().height * 0.5f, 0.0f);
            firstCut.SendMessage("SetCoord", basePos);
            firstCut.SendMessage("MoveToCoord", 1.0f);
            yield return new WaitForSeconds(1.2f);
            secondCut.SendMessage("SetCoord", basePos + new Vector3(0.0f, firstCut.GetComponent<RootElement>().height * 0.5f + secondCut.GetComponent<RootElement>().height * 0.5f + 0.2f, 0.0f));
            secondCut.SendMessage("MoveToCoord", 1.0f);
            yield return new WaitForSeconds(1.2f);
        }

        resultObject = CreateImproperFractions(firstCut, secondCut);
        SetFocusOn(resultObject);
    }
    #endregion

    #region Protected Members
    protected GameObject resultObject = null;

    protected bool CheckActionValidity(RootElement firstCutRoot, RootElement secondCutRoot)
    {
        bool checkRepresentations = (firstCutRoot.type == secondCutRoot.type)
            || ((firstCutRoot.type == ElementsType.HRect) && (secondCutRoot.type == ElementsType.VRect))
            || ((firstCutRoot.type == ElementsType.VRect) && (secondCutRoot.type == ElementsType.HRect));

        bool checkValue = true;
        if (currentAction != ActionType.Join)
            checkValue = firstCutRoot.GetPartValue() >= secondCutRoot.GetPartValue();

        return checkRepresentations && checkValue;
    }

    protected bool CheckDenominator(RootElement firstCutRoot, RootElement secondCutRoot)
    {
        return (firstCutRoot.partDenominator == secondCutRoot.partDenominator);
    }
    #endregion
    #endregion

    #region Protected Methods
    protected void UpdateWS()
    {
        for (int i = 0; i < elements.Count; i++)
        {
            elements[i].SendMessage("Draw", i);
            if (i > 0)
            {
                if (elements[i].GetComponent<RootElement>().mode != InteractionMode.Freeze)
                    elements[i].SendMessage("SetMode", InteractionMode.Moving);

                if (elements[0].GetComponent<RootElement>().denominator == 0 && elements[0].GetComponent<RootElement>().state != ElementsState.Improper)
                    elements[i].SendMessage("DisableInput");
                else
                    elements[i].SendMessage("EnableInput");
            }
        }

        Vector3 center = GetComponent<BoxCollider>().center;
        center.z = 1.0f + elements.Count;
        GetComponent<BoxCollider>().center = center;
    }
    #endregion

    #region Public Methods
    public Color GetColor()
    {
        if (colorCounter >= colorList.Count)
            colorCounter = 0;
        return colorList[colorCounter++];
    }

    public Vector3 GetCurrentActionPosition(GameObject go)
    {
        bool isFirst = (go == firstCut);
        RootElement cutRoot = (isFirst) ? firstCut.GetComponent<RootElement>() : secondCut.GetComponent<RootElement>();

        if (cutRoot.type == ElementsType.HRect || cutRoot.type == ElementsType.VRect || cutRoot.type == ElementsType.Liquid)
        {
            float gap = 0.1f;
            if (isFirst)
                return (Vector3.zero - new Vector3(cutRoot.width * 0.5f + gap * 0.5f, 0.0f, 0.0f));
            else
                return (Vector3.zero + new Vector3(cutRoot.width * 0.5f + gap * 0.5f, 0.0f, 0.0f));
        }
        else if (cutRoot.type == ElementsType.Line)
        {
            float gap = 0.0f;
            if (isFirst)
                return (Vector3.zero - new Vector3(cutRoot.width * 0.5f + gap * 0.5f, 0.0f, 0.0f));
            else
                return (Vector3.zero + new Vector3(cutRoot.width * 0.5f + gap * 0.5f, 0.0f, 0.0f));
        }

        return Vector3.zero;
    }

    public void CancelOperation()
    {
        firstCut = null;
    }
    #endregion
}
