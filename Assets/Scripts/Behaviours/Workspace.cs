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
    public const int MAXVALUE = 3;
    public const float POPUPSIZE = 15.0f;
    public static int elemCounter = 0;
    public const int MAXDRAWCOUNT = 20;
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
    public GameObject setSource;
    public List<Color> colorList = new List<Color>();
    #endregion

    #region Protected Fields
    protected int colorCounter = 0;
    protected SBSBounds bounds;
    protected WSList<GameObject> elements;
    [SerializeField]
    protected ActionType currentAction;
    [SerializeField]
    protected GameObject firstCut = null;
    [SerializeField]
    protected GameObject secondCut = null;
    protected GameObject containerObject = null;
    [SerializeField]
    protected bool inputEnabled = true;
    protected int operationCounter = 0;
    
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

        UpdateWS();
    }

    void Update()
    {
        if (inputEnabled)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                DestroyHighlight();
        }
       if (Input.GetKeyDown(KeyCode.G))
        {
            Resources.UnloadUnusedAssets();

        }

        /*if (Input.GetKeyDown(KeyCode.Q))
        {
            if (null != ElementOnFocus)
                Highlight(ElementOnFocus.name);
        }

        if (Input.GetKeyDown(KeyCode.R))
            Highlight("mcTrash");

        if (Input.GetKeyDown(KeyCode.W))
            Highlight("mcRepresentationBar");

        if (Input.GetKeyDown(KeyCode.E))
            Highlight("mcHints");*/
    }

    void OnMouseDown()
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
    #endregion

    #region Internal Utilities
    GameObject CreateSingleFraction(string name, Element element, bool isSubFraction)
    {
        GameObject root = new GameObject(name);
        root.transform.parent = transform;
        root.transform.position = element.position;
        root.AddComponent<RootElement>();
        root.SendMessage("SetType", element.type);
        root.SendMessage("SetElementState", element.state);
        root.SendMessage("SetNumerator", element.numerator);
        root.SendMessage("SetDenominator", element.denominator);
        root.SendMessage("SetPartitions", element.partitions);
        root.SendMessage("SetPartNumerator", element.partNumerator);
        root.SendMessage("SetPartDenominator", element.partDenominator);
        root.SendMessage("SetIsSubFraction", isSubFraction);

        if (element.denominator == 0 && !isSubFraction)
            root.SendMessage("SetMode", InteractionMode.Initializing);
        else
            root.SendMessage("SetMode", InteractionMode.Moving);

        root.SendMessage("SetColor", element.color);

        return root;
    }

    GameObject CreateContainer(GameObject f1, GameObject f2)
    {
       // Debug.Log("CreateContainer");
        Element element = new Element();
        element.position = Vector3.zero;
        element.color = f1.GetComponent<RootElement>().color; //Workspace.Instance.greenResult;
        element.type = f1.GetComponent<RootElement>().type;
        element.state = ElementsState.Fraction; //ElementsState.Result;
        element.partNumerator = 0;
        element.partDenominator = f1.GetComponent<RootElement>().partDenominator;
        element.numerator = element.partNumerator / firstCut.GetComponent<RootElement>().partitions;
        element.denominator = f1.GetComponent<RootElement>().denominator;
        element.partitions = f1.GetComponent<RootElement>().partitions;

        GameObject root = null;
        switch (element.type)
        {
            case (ElementsType.HRect):
                root = CreateHRect(element);
                break;
            case (ElementsType.VRect):
                root = CreateVRect(element);
                break;
            case (ElementsType.Set):
                root = CreateSet(element);
                break;
            case (ElementsType.Line):
                root = CreateNumberedLine(element);
                break;
            case (ElementsType.Liquid):
                root = CreateLiquidMeasures(element);
                break;
        }

        root.SendMessage("SetMode", InteractionMode.Wait);
        return root;
    }

    GameObject CreateProperFractions(GameObject f1, GameObject f2)
    {
        Element element = new Element();
        element.position = Vector3.zero;
        element.color = f1.GetComponent<RootElement>().color; //Workspace.Instance.greenResult;
        element.type = f1.GetComponent<RootElement>().type;
        element.state = ElementsState.Fraction; //ElementsState.Result;

        if (currentAction == ActionType.Join)
            element.partNumerator = f1.GetComponent<RootElement>().partNumerator + f2.GetComponent<RootElement>().partNumerator;
        else
            element.partNumerator = f1.GetComponent<RootElement>().partNumerator - f2.GetComponent<RootElement>().partNumerator;

        element.partDenominator = f1.GetComponent<RootElement>().partDenominator;
        element.numerator = element.partNumerator / firstCut.GetComponent<RootElement>().partitions;
        element.denominator = f1.GetComponent<RootElement>().denominator;
        element.partitions = f1.GetComponent<RootElement>().partitions;

        GameObject root = null;
        switch (element.type)
        {
            case (ElementsType.HRect):
                root = CreateHRect(element);
                break;
            case (ElementsType.VRect):
                root = CreateVRect(element);
                break;
            case (ElementsType.Set):
                root = CreateSet(element);
                break;
            case (ElementsType.Line):
                root = CreateNumberedLine(element);
                break;
            case (ElementsType.Liquid):
                root = CreateLiquidMeasures(element);
                break;
        }
        root.SendMessage("SetMode", InteractionMode.Wait);

        if (f1.GetComponent<RootElement>().parentRef != null)
        {
            f1.GetComponent<RootElement>().parentRef.BroadcastMessage("SetMode", InteractionMode.Moving);
            f1.GetComponent<RootElement>().parentRef.GetComponent<RootElement>().cutRef = null;
            f1.GetComponent<RootElement>().parentRef = null;
        }

        if (f2.GetComponent<RootElement>().parentRef != null)
        {
            f2.GetComponent<RootElement>().parentRef.BroadcastMessage("SetMode", InteractionMode.Moving);
            f2.GetComponent<RootElement>().parentRef.GetComponent<RootElement>().cutRef = null;
            f2.GetComponent<RootElement>().parentRef = null;
        }

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
        root.SendMessage("UpdateGraphics");
        root.SendMessage("DisableInput");

        if (firstCut.GetComponent<RootElement>().parentRef != null)
        {
            firstCut.GetComponent<RootElement>().parentRef.BroadcastMessage("SetMode", InteractionMode.Moving);
            firstCut.GetComponent<RootElement>().parentRef.GetComponent<RootElement>().cutRef = null;
            firstCut.GetComponent<RootElement>().parentRef = null;
        }

        if (secondCut.GetComponent<RootElement>().parentRef != null)
        {
            secondCut.GetComponent<RootElement>().parentRef.BroadcastMessage("SetMode", InteractionMode.Moving);
            secondCut.GetComponent<RootElement>().parentRef.GetComponent<RootElement>().cutRef = null;
            secondCut.GetComponent<RootElement>().parentRef = null;
        }

        //f1.GetComponent<RootElement>().parentRef.BroadcastMessage("SetMode", InteractionMode.Moving);
        //f2.GetComponent<RootElement>().parentRef.BroadcastMessage("SetMode", InteractionMode.Moving);
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
    #endregion

    #region Messages

    public void DrawCounter() 
    {
        operationCounter++;
      //  Debug.Log("*************************Increase Counter " + operationCounter);
        if (operationCounter >= MAXDRAWCOUNT) 
        {
          //  Debug.Log("********************************Free Memory " + operationCounter);
            Resources.UnloadUnusedAssets();  // free memory
            operationCounter = 0;
        }
    }

    public void CheckOverlapActionMenu() 
    {
        Camera camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        Vector3 fractionPosition;
        foreach (GameObject e in elements)
        {
            fractionPosition = camera.WorldToScreenPoint(e.transform.position);
            if (fractionPosition.x > 205 && fractionPosition.x < 595 && fractionPosition.y > 407 && fractionPosition.y < 600) 
            {
                e.transform.position = new Vector3(e.transform.position.x, e.transform.position.y-7.0f, e.transform.position.z); 
            }
        }
    }


    void CreateFractionsChildren(Element element, GameObject root, float gap, string label)
    {
        //Debug.Log("createfractionchildren");
        float totalWidth = 0.0f;
        float totalHeight = 0.0f;

        int wholes = Mathf.Max(1, Mathf.CeilToInt((float)element.partNumerator / (float)element.partDenominator));
        
        int diff = element.partNumerator - (element.partDenominator * Mathf.Max(wholes - 1, 1));
        if (wholes == 1)
            diff = element.partNumerator;

        totalWidth += gap * (wholes - 1);

        for (int i = 0; i < wholes; i++)
        {
            Element elem = element;
            if (i == wholes - 1)
            {
                elem.partNumerator = diff;
                elem.numerator = elem.partNumerator / element.partitions;
            }
            else
            {
                elem.numerator = elem.denominator;
                elem.partNumerator = elem.partDenominator;
            }

            GameObject child = CreateSingleFraction(label +"_child" + (i + 1), elem, true);
            child.transform.parent = root.transform;
            if (elem.type == ElementsType.Line)
            {
                child.SendMessage("SetFractionBaseOffset", i);
                root.BroadcastMessage("SetBBExtends", new BBExtend(0.0f, 1.0f, 0.0f, 1.0f));
            }
            child.SendMessage("InitializeAs", element.type);
            child.SendMessage("SetEnableCollider", false);
            child.transform.position = root.transform.TransformPoint((wholes - 1 - i) * (new SBSVector3(-(child.GetComponent<RootElement>().width + gap), 0.0f, 0.0f)));

            root.GetComponent<RootElement>().elements.Add(child);

            totalHeight = child.GetComponent<RootElement>().height;
            totalWidth += child.GetComponent<RootElement>().width;
        }

        root.GetComponent<RootElement>().height = totalHeight;
        root.GetComponent<RootElement>().width = totalWidth;

        root.transform.position += new Vector3(totalWidth * 0.25f, 0.0f, 0.0f);
    }

    public void AddFractionsChildren(GameObject root)
    {
        string label = root.name.Split('_')[0];
        float totalWidth = 0.0f;
        float totalHeight = 0.0f;
        
        RootElement r = root.GetComponent<RootElement>();
        Element element = new Element();        
        element.numerator = r.numerator;
        element.denominator = r.denominator;
        element.partNumerator = r.partNumerator;
        element.partDenominator = r.partDenominator;
        element.partitions = r.partitions;
        element.state = r.state;
        element.type = r.type;
        element.position = Vector3.zero;
        element.color = r.color;

        float gap = 0.0f;
        switch (element.type)
        {
            case (ElementsType.HRect):
            case (ElementsType.VRect):
            case (ElementsType.Set):
                gap = 0.4f;
                break;
            case (ElementsType.Liquid):
                gap = 0.6f;
                break;
        }

        //int diff = element.partNumerator - element.partDenominator;

        int wholes = r.elements.Count + 1; // Mathf.Max(1, Mathf.CeilToInt((float)element.partNumerator / (float)element.partDenominator));

        totalWidth += gap * (wholes - 1);

        int i = wholes - 1;

        Element elem = element;
        elem.partNumerator = 1;
        elem.numerator = elem.partNumerator / element.partitions;

        GameObject child = CreateSingleFraction(label + "_child" + (i + 1), elem, true);
        child.transform.parent = root.transform;
        if (elem.type == ElementsType.Line)
        {
            child.SendMessage("SetFractionBaseOffset", i);
            child.SendMessage("SetBBExtends", new BBExtend(0.0f, 1.0f, 0.0f, 1.0f));
        }
        child.SendMessage("InitializeAs", element.type);
        child.SendMessage("SetEnableCollider", false);
           
        child.transform.position = root.transform.TransformPoint((wholes - 1 - i) * (new SBSVector3(-(child.GetComponent<RootElement>().width + gap), 0.0f, 0.0f)));

        root.GetComponent<RootElement>().elements.Add(child);

        totalHeight = child.GetComponent<RootElement>().height;
        totalWidth += child.GetComponent<RootElement>().width;

        root.GetComponent<RootElement>().height = totalHeight;
        root.GetComponent<RootElement>().width = totalWidth;

        //root.transform.position += new Vector3(totalWidth * 0.25f, 0.0f, 0.0f);

        RepositioningChildren(root);
    }

    public void RemoveEmptyChildren(GameObject root)
    {
        //Debug.Log("RemoveEmptyChildren");

        if (root.GetComponent<RootElement>().elements.Count > 1)
        {
            int elemCount = root.GetComponent<RootElement>().elements.Count;
            for (int i = 0; i < elemCount; i++)
            {
                GameObject child = root.GetComponent<RootElement>().elements[i];
                if (child.GetComponent<RootElement>().partNumerator <= 0)
                {
                    Destroy(child);
                    child = null;
                    root.GetComponent<RootElement>().elements[i] = null;
                }
            }
            root.GetComponent<RootElement>().elements.RemoveAll(item => item == null);
            RepositioningChildren(root);
        }
    }

    public void RepositioningChildren(GameObject root)
    {
        float totalWidth = 0.0f;
        float totalHeight = 0.0f;

        RootElement element = root.GetComponent<RootElement>();

        float gap = 0.0f;
        switch (element.type)
        {
            case (ElementsType.HRect):
            case (ElementsType.VRect):
            case (ElementsType.Set):
                gap = 0.4f;
                break;
            case (ElementsType.Liquid):
                gap = 0.6f;
                break;
        }

        int wholes = element.elements.Count;

        totalWidth += gap * (wholes - 1);

        for (int i = 0; i < wholes; i++)
        {
            GameObject child = element.elements[i];
            child.transform.position = root.transform.TransformPoint((wholes - 1 - i) * (new SBSVector3(-(child.GetComponent<RootElement>().width + gap), 0.0f, 0.0f)));
            totalHeight = child.GetComponent<RootElement>().height;
            totalWidth += child.GetComponent<RootElement>().width;
        }

        root.GetComponent<RootElement>().height = totalHeight;
        root.GetComponent<RootElement>().width = totalWidth;

        root.BroadcastMessage("UpdateArrowsState", SendMessageOptions.DontRequireReceiver);
        if (!OperationPending)
            UpdateWS();
    }

    GameObject CreateSet(Element element) 
    {
        string label = "set";
        GameObject root = CreateSingleFraction(label + "_" + (++elemCounter), element, false);
        CreateFractionsChildren(element, root, 0.4f, label);
        elements.Push(root);
        UpdateWS();
        root.BroadcastMessage("SetRoot", root);
        ExternalEventsManager.Instance.SendMessageToSupport("FractionGenerated", "Set", root.name);
        return root;
    }

    GameObject CreateVRect(Element element)
    {
        string label = "vrect";
        GameObject root = CreateSingleFraction(label + "_" + (++elemCounter), element, false);
        CreateFractionsChildren(element, root, 0.4f, label);
        elements.Push(root);
        UpdateWS();
        root.BroadcastMessage("SetRoot", root);
        ExternalEventsManager.Instance.SendMessageToSupport("FractionGenerated", "VRects", root.name);
        return root;
    }

    GameObject CreateHRect(Element element)
    {
        string label = "hrect";
        GameObject root = CreateSingleFraction(label + "_" + (++elemCounter), element, false);
        CreateFractionsChildren(element, root, 0.4f, label);
        elements.Push(root);
        UpdateWS();
        root.BroadcastMessage("SetRoot", root);
        ExternalEventsManager.Instance.SendMessageToSupport("FractionGenerated", "HRects", root.name);
        return root;
    }

    GameObject CreateNumberedLine(Element element)
    {
        string label = "line";
        GameObject root = CreateSingleFraction(label + "_" + (++elemCounter), element, false);
        CreateFractionsChildren(element, root, 0.0f, label);
        elements.Push(root);
        UpdateWS();
        root.BroadcastMessage("SetRoot", root);
        ExternalEventsManager.Instance.SendMessageToSupport("FractionGenerated", "NumberedLines", root.name);
        return root;
    }

    GameObject CreateLiquidMeasures(Element element)
    {
        string label = "liquid";
        GameObject root = CreateSingleFraction(label + "_" + (++elemCounter), element, false);
        CreateFractionsChildren(element, root, 0.6f, label);
        elements.Push(root);
        UpdateWS();
        root.BroadcastMessage("SetRoot", root);
        ExternalEventsManager.Instance.SendMessageToSupport("FractionGenerated", "LiquidMeasures", root.name);
        return root;
    }

    /*void CreatePopup()
    {
        GameObject slice = new GameObject("popup");
        slice.transform.parent = transform;
        slice.transform.position = Vector3.zero;

        Mesh mesh;
        MeshUtils.CreateRectangle(18.0f, 12.0f, Workspace.Instance.white, out mesh);
        slice.GetComponent<MeshFilter>().mesh = mesh;
        slice.renderer.material.SetColor("_Color", Workspace.Instance.white);
    }*/

    void CreateCopy(GameObject source)
    {
        GameObject root = Instantiate(source) as GameObject;
        string[] splitted = source.name.Split('_');
        root.name = splitted[0] + "_" + (++elemCounter);
        root.transform.parent = transform;

        for (int i = 0; i < root.transform.childCount; i++)
            if (root.transform.GetChild(i).name.Equals("partition"))
                Destroy(root.transform.GetChild(i).gameObject);
        
        float xOffset = (root.transform.position.x > 0.0f) ? -1.0f : 1.0f;
        float yOffset = (root.transform.position.y > 0.0f) ? -1.0f : 1.0f;
        root.transform.position += new Vector3(xOffset, yOffset, 0.0f);
        root.BroadcastMessage("SetType", source.GetComponent<RootElement>().type, SendMessageOptions.DontRequireReceiver);
        Debug.Log("before color counter"+ colorCounter);
        root.BroadcastMessage("SetColor", GetColor(), SendMessageOptions.DontRequireReceiver);
        Debug.Log("after color counter"+ colorCounter);
        root.BroadcastMessage("SetBBExtends", source.GetComponent<RootElement>().bbExtends);
        root.GetComponent<RootElement>().UpdateGraphics();  
        elements.Push(root);
        UpdateWS();

        ExternalEventsManager.Instance.SendBrowserMessage("FractionCopy", root.name);
    }

    void CutFraction(GameObject source)
    {
        GameObject root = Instantiate(source) as GameObject;
        source.GetComponent<RootElement>().cutRef = root;
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
        root.BroadcastMessage("DetachModifierPartition");

        root.GetComponent<RootElement>().Cut();

        elements.Push(root);

        source.BroadcastMessage("SetMode", InteractionMode.Freeze);
        UpdateWS();

        ExternalEventsManager.Instance.SendBrowserMessage("FractionCut", root.name);
    }

    void SetFocusOn(GameObject elem)
    {
        if (ElementOnFocus.GetComponent<RootElement>().denominator > 0 || ElementOnFocus.GetComponent<RootElement>().state == ElementsState.Improper)
        {
            elements.Pull(elem);
            UpdateWS();
        }
    }

    void SendBack(GameObject elem)
    {
        if (ElementOnFocus.GetComponent<RootElement>().denominator > 0 || ElementOnFocus.GetComponent<RootElement>().state == ElementsState.Improper)
        {
            elements.SendBack(elem);
            UpdateWS();
        }
    }

    void DeleteElement(GameObject element)
    {
        if (null != element.GetComponent<RootElement>().parentRef)
            element.GetComponent<RootElement>().parentRef.BroadcastMessage("SetMode", InteractionMode.Moving);

        if (null != element.GetComponent<RootElement>().cutRef)
            element.GetComponent<RootElement>().cutRef.GetComponent<RootElement>().parentRef = null;

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

    void CheckCutOverlap(Vector3 mousePos)
    {
        GameObject tmp = null;
        for (int i = 0; i < elements.Count; i++)
        {
            RootElement ec = elements[i].GetComponent<RootElement>();
            if (ec.state == ElementsState.Cut && elements[i] != ElementOnFocus)
            {
                if (ec.GetBounds().ContainsPointXY(Camera.main.ScreenToWorldPoint(mousePos)))
                {
                    tmp = elements[i];
                    break;
                }
            }
        }

        if (tmp != null)
        {
            interfaces.SendMessage("OnShowActionsMenu", ElementOnFocus);
            //ElementOnFocus.GetComponent<RootElement>().mode = InteractionMode.Wait;
            //tmp.GetComponent<RootElement>().mode = InteractionMode.Wait;
            Workspace.Instance.SendMessage("SetFirstOperationCut", ElementOnFocus);
            Workspace.Instance.SendMessage("SetSecondOperationCut", tmp);
        }
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

        firstCutRoot.mode = InteractionMode.Wait;
        secondCutRoot.mode = InteractionMode.Wait;

        if (firstCutRoot.GetPartValue() < secondCutRoot.GetPartValue())
        {
            GameObject tmp = firstCut;
            firstCut = secondCut;
            secondCut = tmp;
            firstCutRoot = firstCut.GetComponent<RootElement>();
            secondCutRoot = secondCut.GetComponent<RootElement>();
        }

        bool result = CheckActionValidity(firstCutRoot, secondCutRoot);
        if (result)
        {
            if (CheckDenominator(firstCutRoot, secondCutRoot))
            {
                if (CheckValueGreaterThen(firstCutRoot, secondCutRoot, MAXVALUE))
                {
                    //firstCutRoot.ResetScale();
                    //secondCutRoot.ResetScale();

                    switch (currentAction)
                    {
                        case (ActionType.Join):
                            StartCoroutine(MakeProperJoin());
                            break;
                        /*case (ActionType.Compare):
                            StartCoroutine(MakeProperCompare());
                            break;*/
                        case (ActionType.TakingAway):
                            StartCoroutine(MakeProperTakingAway());
                            break;
                    }                    
                }
                else
                {
                    interfaces.SendMessage("ShowFeedbackPopup", "{rep_greater_" + MAXVALUE + "}");
                    switch (currentAction)
                    {
                        case (ActionType.Join):
                            ExternalEventsManager.Instance.SendMessageToSupport("ActionEvent", "SumError", firstCut.name, secondCut.name);
                            break;
                        case (ActionType.TakingAway):
                            ExternalEventsManager.Instance.SendMessageToSupport("ActionEvent", "SubtractionError", firstCut.name, secondCut.name);
                            break;
                    }
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
                    ExternalEventsManager.Instance.SendMessageToSupport("ActionEvent", "SumError", firstCut.name, secondCut.name);
                    break;
                case (ActionType.TakingAway):
                    ExternalEventsManager.Instance.SendMessageToSupport("ActionEvent", "SubtractionError", firstCut.name, secondCut.name);
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

            firstCut.SendMessage("MoveToCoord", -1.0f);
            secondCut.SendMessage("MoveToCoord", -1.0f);

            StopAllCoroutines();
            if (resultObject == null)
            {
                if(CheckDenominator(firstCutRoot, secondCutRoot))
                    resultObject = CreateProperFractions(firstCut, secondCut);
                else
                    resultObject = CreateImproperFractions(firstCut, secondCut);
            }
        }

        BoxCollider containerBB = resultObject.GetComponent<BoxCollider>();
        Vector3 containerPos = Vector3.zero + new Vector3(-containerBB.center.x, 0.0f, 0.0f) + new Vector3(0.0f, -(containerBB.size.y + 0.1f), 0.0f);
        resultObject.transform.position = containerPos;
        if (resultObject.GetComponent<RootElement>().state != ElementsState.Improper)
            resultObject.SendMessage("AttachSymbol", true);

        resultObject.SendMessage("SetMode", InteractionMode.Moving);

        if (null != containerObject)
        {
            elements.Remove(containerObject);
            Destroy(containerObject);
            containerObject = null;
        }
        UpdateWS();
    }
    #endregion

    #region Coroutines
    IEnumerator MakeProperJoin()
    {
        ExternalEventsManager.Instance.SendMessageToSupport("ActionEvent", "ProperSum", firstCut.name, secondCut.name);
        interfaces.SendMessage("ShowActionPopup");

        float gap = 0.1f;
        if (firstCut.GetComponent<RootElement>().type == ElementsType.Line)
            gap = 0.0f;

        BoxCollider firstBB = firstCut.GetComponent<BoxCollider>();
        BoxCollider secondBB = secondCut.GetComponent<BoxCollider>();
        

        float totalWidth = firstBB.size.x + secondBB.size.x + gap;
        float scaleFactor = 1.0f;
        if (totalWidth > POPUPSIZE)
        {
            scaleFactor = POPUPSIZE / totalWidth;
            firstCut.transform.localScale = scaleFactor * Vector3.one;
            secondCut.transform.localScale = scaleFactor * Vector3.one;
        }
        float diff = totalWidth * 0.5f - firstBB.size.x;

        Vector3 firstPos = Vector3.zero + new Vector3(-firstBB.center.x * scaleFactor, 0.0f, 0.0f)
            + new Vector3(-(firstBB.size.x * scaleFactor * 0.5f + gap * 0.5f * scaleFactor) - diff * scaleFactor, firstBB.size.y * 0.5f * scaleFactor + gap * scaleFactor, 0.0f);
        Vector3 secondPos = Vector3.zero + new Vector3(-secondBB.center.x * scaleFactor, 0.0f, 0.0f)
            + new Vector3(secondBB.size.x * scaleFactor * 0.5f + gap * 0.5f * scaleFactor - diff * scaleFactor, firstBB.size.y * 0.5f * scaleFactor + gap * scaleFactor, 0.0f);

        /*float totalWidth = firstBB.size.x + secondBB.size.x + gap;
        float diff = totalWidth * 0.5f - firstBB.size.x;

        Vector3 firstPos = Vector3.zero + new Vector3(-firstBB.center.x, 0.0f, 0.0f) + new Vector3(-(firstBB.size.x * 0.5f + gap * 0.5f) - diff, firstBB.size.y * 0.5f + gap, 0.0f);
        Vector3 secondPos = Vector3.zero + new Vector3(-secondBB.center.x, 0.0f, 0.0f) + new Vector3(secondBB.size.x * 0.5f + gap * 0.5f - diff, firstBB.size.y * 0.5f + gap, 0.0f);*/

        firstCut.SendMessage("SetCoord", firstPos);
        secondCut.SendMessage("SetCoord", secondPos);

        firstCut.SendMessage("MoveToCoord", 1.0f);

        yield return new WaitForSeconds(1.2f);
        
        secondCut.SendMessage("MoveToCoord", 1.0f);

        yield return new WaitForSeconds(1.2f);
        
        containerObject = CreateContainer(firstCut, secondCut);
        BoxCollider containerBB = containerObject.GetComponent<BoxCollider>();
        Vector3 containerPos = Vector3.zero + new Vector3(-containerBB.center.x, 0.0f, 0.0f) + new Vector3(0.0f, -(containerBB.size.y + 0.06f), 0.0f);
        containerObject.transform.position = containerPos;
        containerObject.SendMessage("AttachSymbol", true);
        containerObject.SendMessage("DisableInput");

        yield return new WaitForSeconds(0.2f);

        int numerator = firstCut.GetComponent<RootElement>().partNumerator;
        for (int i = 0; i < numerator; i++)
        {
            yield return new WaitForSeconds(1.0f);
            firstCut.SendMessage("DecreaseCutNumerator");
            containerObject.GetComponent<RootElement>().IncreaseNumerator();
            containerObject.GetComponent<RootElement>().UpdateGraphics();
            yield return new WaitForEndOfFrame();
            containerPos = Vector3.zero + new Vector3(-containerBB.center.x, 0.0f, 0.0f) + new Vector3(0.0f, -(containerBB.size.y + 0.06f), 0.0f);
            containerObject.transform.position = containerPos;
        }

        numerator = secondCut.GetComponent<RootElement>().partNumerator;
        for (int i = 0; i < numerator; i++)
        {
            yield return new WaitForSeconds(1.0f);
            secondCut.SendMessage("DecreaseCutNumerator");
            containerObject.GetComponent<RootElement>().IncreaseNumerator();
            containerObject.GetComponent<RootElement>().UpdateGraphics();
            yield return new WaitForEndOfFrame();
            containerPos = Vector3.zero + new Vector3(-containerBB.center.x, 0.0f, 0.0f) + new Vector3(0.0f, -(containerBB.size.y + 0.06f), 0.0f);
            containerObject.transform.position = containerPos;
        }

        resultObject = containerObject;
        containerObject = null;
        SetFocusOn(resultObject);

        if (firstCut.GetComponent<RootElement>().parentRef != null)
        {
            firstCut.GetComponent<RootElement>().parentRef.BroadcastMessage("SetMode", InteractionMode.Moving);
            firstCut.GetComponent<RootElement>().parentRef.GetComponent<RootElement>().cutRef = null;
            firstCut.GetComponent<RootElement>().parentRef = null;
        }

        if (secondCut.GetComponent<RootElement>().parentRef != null)
        {
            secondCut.GetComponent<RootElement>().parentRef.BroadcastMessage("SetMode", InteractionMode.Moving);
            secondCut.GetComponent<RootElement>().parentRef.GetComponent<RootElement>().cutRef = null;
            secondCut.GetComponent<RootElement>().parentRef = null;
        }

        elements.Remove(firstCut);
        elements.Remove(secondCut);

        Destroy(firstCut);
        Destroy(secondCut);
        firstCut = null;
        secondCut = null;

        UpdateWS();
    }

    IEnumerator MakeProperTakingAway()
    {
        ExternalEventsManager.Instance.SendMessageToSupport("ActionEvent", "ProperSubtraction", firstCut.name, secondCut.name);
        interfaces.SendMessage("ShowActionPopup");

        float gap = 0.1f;
        if (firstCut.GetComponent<RootElement>().type == ElementsType.Line)
            gap = 0.0f;

        BoxCollider firstBB = firstCut.GetComponent<BoxCollider>();
        BoxCollider secondBB = secondCut.GetComponent<BoxCollider>();

        float totalWidth = firstBB.size.x + secondBB.size.x + gap;
        float scaleFactor = 1.0f;
        if (totalWidth > POPUPSIZE)
        {
            scaleFactor = POPUPSIZE / totalWidth;
            firstCut.transform.localScale = scaleFactor * Vector3.one;
            secondCut.transform.localScale = scaleFactor * Vector3.one;
        }
        float diff = totalWidth * 0.5f - firstBB.size.x;

        Vector3 firstPos = Vector3.zero + new Vector3(-firstBB.center.x * scaleFactor, 0.0f, 0.0f)
            + new Vector3(-(firstBB.size.x * scaleFactor * 0.5f + gap * 0.5f * scaleFactor) - diff * scaleFactor, firstBB.size.y * 0.5f * scaleFactor + gap * scaleFactor, 0.0f);
        Vector3 secondPos = Vector3.zero + new Vector3(-secondBB.center.x * scaleFactor, 0.0f, 0.0f)
            + new Vector3(secondBB.size.x * scaleFactor * 0.5f + gap * 0.5f * scaleFactor - diff * scaleFactor, firstBB.size.y * 0.5f * scaleFactor + gap * scaleFactor, 0.0f);

        /*float totalWidth = firstBB.size.x + secondBB.size.x + gap;
        float diff = totalWidth * 0.5f - firstBB.size.x;

        Vector3 firstPos = Vector3.zero + new Vector3(-firstBB.center.x, 0.0f, 0.0f) + new Vector3(-(firstBB.size.x * 0.5f + gap * 0.5f) - diff, firstBB.size.y * 0.5f + gap, 0.0f);
        Vector3 secondPos = Vector3.zero + new Vector3(-secondBB.center.x, 0.0f, 0.0f) + new Vector3(secondBB.size.x * 0.5f + gap * 0.5f - diff, firstBB.size.y * 0.5f + gap, 0.0f);*/
        firstCut.SendMessage("SetCoord", firstPos);
        secondCut.SendMessage("SetCoord", secondPos);

        firstCut.SendMessage("MoveToCoord", 1.0f);

        yield return new WaitForSeconds(1.2f);

        secondCut.SendMessage("MoveToCoord", 1.0f);

        yield return new WaitForSeconds(1.2f);

        int numerator = secondCut.GetComponent<RootElement>().partNumerator;
        for (int i = 0; i < numerator; i++)
        {
            yield return new WaitForSeconds(1.0f);
            firstCut.SendMessage("DecreaseCutNumerator");
            secondCut.SendMessage("DecreaseCutNumerator");
            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(1.2f);

        containerObject = CreateContainer(firstCut, secondCut);
        BoxCollider containerBB = containerObject.GetComponent<BoxCollider>();
        Vector3 containerPos = Vector3.zero + new Vector3(-containerBB.center.x, 0.0f, 0.0f) + new Vector3(0.0f, -(containerBB.size.y + 0.06f), 0.0f);
        containerObject.transform.position = containerPos;
        containerObject.SendMessage("AttachSymbol", true);
        containerObject.SendMessage("DisableInput");

        yield return new WaitForSeconds(0.2f);

        numerator = firstCut.GetComponent<RootElement>().partNumerator - secondCut.GetComponent<RootElement>().partNumerator;
        for (int i = 0; i < numerator; i++)
        {
            yield return new WaitForSeconds(1.0f);
            firstCut.SendMessage("DecreaseCutNumerator");
            containerObject.GetComponent<RootElement>().IncreaseNumerator();
            containerObject.GetComponent<RootElement>().UpdateGraphics();
            yield return new WaitForEndOfFrame();
            containerPos = Vector3.zero + new Vector3(-containerBB.center.x, 0.0f, 0.0f) + new Vector3(0.0f, -(containerBB.size.y + 0.06f), 0.0f);
            containerObject.transform.position = containerPos;
            yield return new WaitForSeconds(0.2f);
        }

        resultObject = containerObject;
        containerObject = null;
        SetFocusOn(resultObject);

        if (firstCut.GetComponent<RootElement>().parentRef != null)
        {
            firstCut.GetComponent<RootElement>().parentRef.BroadcastMessage("SetMode", InteractionMode.Moving);
            firstCut.GetComponent<RootElement>().parentRef.GetComponent<RootElement>().cutRef = null;
            firstCut.GetComponent<RootElement>().parentRef = null;
        }

        if (secondCut.GetComponent<RootElement>().parentRef != null)
        {
            secondCut.GetComponent<RootElement>().parentRef.BroadcastMessage("SetMode", InteractionMode.Moving);
            secondCut.GetComponent<RootElement>().parentRef.GetComponent<RootElement>().cutRef = null;
            secondCut.GetComponent<RootElement>().parentRef = null;
        }

        elements.Remove(firstCut);
        elements.Remove(secondCut);

        Destroy(firstCut);
        Destroy(secondCut);
        firstCut = null;
        secondCut = null;

        UpdateWS();
    }

    /*IEnumerator MakeProperCompare()
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
    }*/

    IEnumerator MakeProperTakingAwayOLD()
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
                ExternalEventsManager.Instance.SendMessageToSupport("ActionEvent", "ImproperSum", firstCut.name, secondCut.name);
                break;
            case (ActionType.TakingAway):
                ExternalEventsManager.Instance.SendMessageToSupport("ActionEvent", "ImproperSubtraction", firstCut.name, secondCut.name);
                break;
        }
        
        interfaces.SendMessage("ShowActionPopup");

        float gap = 0.1f;
        if (firstCut.GetComponent<RootElement>().type == ElementsType.Line)
            gap = 0.0f;

        BoxCollider firstBB = firstCut.GetComponent<BoxCollider>();
        BoxCollider secondBB = secondCut.GetComponent<BoxCollider>();

        float totalWidth = firstBB.size.x + secondBB.size.x + gap;
        float scaleFactor = 1.0f;
        if (totalWidth > POPUPSIZE)
        {
            scaleFactor = POPUPSIZE / totalWidth;
            firstCut.transform.localScale = scaleFactor * Vector3.one;
            secondCut.transform.localScale = scaleFactor * Vector3.one;
        }
        float diff = totalWidth * 0.5f - firstBB.size.x;

        Vector3 firstPos = Vector3.zero + new Vector3(-firstBB.center.x * scaleFactor, 0.0f, 0.0f)
            + new Vector3(-(firstBB.size.x * scaleFactor * 0.5f + gap * 0.5f * scaleFactor) - diff * scaleFactor, firstBB.size.y * 0.5f * scaleFactor + gap * scaleFactor, 0.0f);
        Vector3 secondPos = Vector3.zero + new Vector3(-secondBB.center.x * scaleFactor, 0.0f, 0.0f)
            + new Vector3(secondBB.size.x * scaleFactor * 0.5f + gap * 0.5f * scaleFactor - diff * scaleFactor, firstBB.size.y * 0.5f * scaleFactor + gap * scaleFactor, 0.0f);

        firstCut.SendMessage("SetCoord", firstPos);
        secondCut.SendMessage("SetCoord", secondPos);

        firstCut.SendMessage("MoveToCoord", 1.0f);

        yield return new WaitForSeconds(1.2f);

        secondCut.SendMessage("MoveToCoord", 1.0f);

        yield return new WaitForSeconds(1.0f);

        resultObject = CreateImproperFractions(firstCut, secondCut);
        SetFocusOn(resultObject);
        UpdateWS();
    }
    #endregion

    #region Protected Members
    protected GameObject resultObject = null;

    protected bool CheckActionValidity(RootElement firstCutRoot, RootElement secondCutRoot)
    {
        bool checkRepresentations = (firstCutRoot.type == secondCutRoot.type)
            || ((firstCutRoot.type == ElementsType.HRect) && (secondCutRoot.type == ElementsType.VRect))
            || ((firstCutRoot.type == ElementsType.VRect) && (secondCutRoot.type == ElementsType.HRect));

        return checkRepresentations;
    }

    protected bool CheckValueGreaterThen(RootElement firstCutRoot, RootElement secondCutRoot, int value)
    {
        if (currentAction == ActionType.Join)
            return (((float)(firstCutRoot.partNumerator + secondCutRoot.partNumerator) / (float)firstCutRoot.partDenominator) <= value);

        return (((float)(firstCutRoot.partNumerator - secondCutRoot.partNumerator) / (float)firstCutRoot.partDenominator) <= value);
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
            if (elements[i].GetComponent<RootElement>().mode != InteractionMode.Wait)
                elements[i].SendMessage("Draw", i);

            if (i > 0)
            {
                if (elements[i].GetComponent<RootElement>().mode != InteractionMode.Freeze)
                    elements[i].SendMessage("SetMode", InteractionMode.Moving);

                if (elements[0].GetComponent<RootElement>().denominator == 0 && elements[0].GetComponent<RootElement>().state != ElementsState.Improper)
                    elements[i].SendMessage("DisableInput");
                else
                {
                    if (inputEnabled)
                        elements[i].SendMessage("EnableInput");
                }
            }
        }

        Vector3 center = GetComponent<BoxCollider>().center;
        center.z = 1.0f + elements.Count;
        GetComponent<BoxCollider>().center = center;
    }
    #endregion

    #region Public Methods
    public void Highlight(string name)
    {
        interfaces.SendMessage("InterfaceHighlightByName", name);

        if(transform.childCount > 0)
            gameObject.BroadcastMessage("InitHighlight", name, SendMessageOptions.DontRequireReceiver);
    }

    public void DestroyHighlight()
    {
        interfaces.SendMessage("DestroyInterfaceHighlight");
        Destroy(GameObject.FindGameObjectWithTag("Highlight"));
    }

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
        BoxCollider bb = cutRoot.GetComponent<BoxCollider>();

        float gap = 0.1f;
        if (cutRoot.type == ElementsType.Line)
            gap = 0.0f;

        if (isFirst)
            return (Vector3.zero - new Vector3(bb.size.x * 0.5f + gap * 0.5f, 0.0f, 0.0f) - bb.center);
        else
            return (Vector3.zero + new Vector3(bb.size.x * 0.5f + gap * 0.5f, 0.0f, 0.0f) - bb.center);
    }

    public void CancelOperation()
    {
        firstCut = null;
    }
    #endregion
}
