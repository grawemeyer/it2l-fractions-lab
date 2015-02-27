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
    public Font fontInUse;
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
    public GameObject setSourceMoon;
    public GameObject setSourceHeart;
    public GameObject setSourceStar;
    public GameObject delimiters;
    public SetElement.Shape lastShape;
    public List<Color> colorList = new List<Color>();
    public bool isPinchActive = false;
    public GameObject symbol_root;
    public GameObject partition_root;
    public GameObject symbol_root_mobile;
    public GameObject partition_root_mobile;
    public GameObject highlight_prefab;
    public float actualFactorScale;

    public bool isAction;
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
    public bool inputEnabled = true;
    protected int operationCounter = 0;
    protected Vector3 initialPos;
    protected float initialOrthoSize;
    protected float prevSize;
    protected Vector3 prevPos;
    protected Vector3 preScale;
    public  float halfScreenWidth;
    public float halfScreenHeight;
    public float outOffset = 3.0f;
    bool isPan = false;
    float percOffset;
    float halfCameraHeight;
    float halfCameraWidth;
    Vector3 percVector;
    Vector3 offset;
    Vector3 newPos;

   // protected Vector3 
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
        isAction = false;
        actualFactorScale = 1.0f;
        elements = new WSList<GameObject>();
        float widthSize = Camera.main.orthographicSize * 2.0f * Camera.main.aspect;
        float heightSize = widthSize / Camera.main.aspect;
       // Debug.Log("***************************************** widthSize " + widthSize + " heightSize " + heightSize);
        bounds = new SBSBounds(Vector3.zero, Vector3.right * widthSize + Vector3.up * heightSize + Vector3.forward * float.MaxValue);

        halfScreenHeight = Camera.main.orthographicSize;
        halfScreenWidth = (halfScreenHeight * Screen.width) / Screen.height;
        outOffset = halfScreenWidth / 4;  //delimiter width is equal to 1/8 of screen width (100 px: 800px)
        
        colorList.Add(new Color(0.7373f, 0.0078f, 0.0078f, 1f));
        colorList.Add(new Color(1.0f, 0.7059f, 0.0f,1f));
        colorList.Add(new Color(0.2431f, 0.6627f, 0.9882f, 1f));
        colorList.Add(new Color(0.6588f, 0.4078f, 1.0f, 1f));

        UpdateWS();
    }

    int index = 0;

    void Update()
    {
        if (isPan) 
        {
            if (percVector.x >= 0)
                delimiters.GetComponent<ShowDelimiters>().ShowRight(percVector.x);
            if(percVector.x <= 0 )
                delimiters.GetComponent<ShowDelimiters>().ShowLeft(-percVector.x);
            if (percVector.y >= 0)
                delimiters.GetComponent<ShowDelimiters>().ShowTop(percVector.y);
            if (percVector.y <= 0)
                delimiters.GetComponent<ShowDelimiters>().ShowBottom(-percVector.y);
            Camera.main.transform.position = newPos;
        }

        if (inputEnabled)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.touchCount > 0)
                DestroyHighlight();
            if((Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)) && interfaces.GetComponent<InterfaceBehaviour>().isMenuActive)
                interfaces.GetComponent<InterfaceBehaviour>().isMenuActive = false;
        }
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.H) && elements.Count > 0)
        {
            Highlight(elements[index++ % elements.Count].gameObject.name);
        }
#endif
    }
   

    void OnMouseDown()
    {
        if (!inputEnabled || Input.touchCount == 2 || isPinchActive || interfaces.GetComponent<InterfaceBehaviour>().isMenuActive)
            return;
       // Debug.Log("OnMouseDown " + interfaces.GetComponent<InterfaceBehaviour>().isMenuActive);
        initialPos = Input.mousePosition;

        if (ElementOnFocus != null)
        {
            if (ElementOnFocus.GetComponent<RootElement>().denominator > 0)
            {
                //if (ElementOnFocus.GetComponent<RootElement>().state != ElementsState.Equivalence)
                    ElementOnFocus.BroadcastMessage("SetMode", InteractionMode.Moving, SendMessageOptions.DontRequireReceiver);
                interfaces.SendMessage("ShowSuggestion", "");
                interfaces.SendMessage("ShowHint", "");
                EnableInput();
                if (OperationPending)
                    firstCut = null;
            }
        }
    }

    float zoomOutOffset;
    void OnMouseDrag()
    {
        if (!inputEnabled || interfaces.GetComponent<InterfaceBehaviour>().isMenuActive)
            return;

        if (Input.touchCount == 2 || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Stationary) || !inputEnabled)
            return;
        if (interfaces.GetComponent<InterfaceBehaviour>().isZoomActive ||
            interfaces.GetComponent<InterfaceBehaviour>().isDragWindow ||
            interfaces.GetComponent<InterfaceBehaviour>().isPressingButton ||
            !interfaces.GetComponent<InterfaceBehaviour>().InputEnabled||
            interfaces.GetComponent<InterfaceBehaviour>().isBlockingOperation)
            return;

        halfCameraHeight = Camera.main.orthographicSize;
        halfCameraWidth = ((Camera.main.orthographicSize * Screen.width) / Screen.height);
        offset = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.ScreenToWorldPoint(initialPos));

        if (offset == Vector3.zero)
            return;

        if (!isPan)
        {
            ExternalEventsManager.Instance.SendMessageToSupport("BeginPan", "(" + Camera.main.transform.position.x + ", " + Camera.main.transform.position.y + ")");
            isPan = true;
        }

       // float mouseCursorSpeed = Input.GetAxis("Mouse X") / Time.deltaTime;
       // Debug.Log("mouse speed " + mouseCursorSpeed);
        newPos = Camera.main.transform.position - offset;
        zoomOutOffset = (outOffset * Camera.main.orthographicSize) / 10.0f;
        //Debug.Log("zoomOutOffset " + zoomOutOffset);
        percVector = Vector3.zero;
        percVector.x = Mathf.Clamp(((newPos.x + halfCameraWidth) - halfScreenWidth) / zoomOutOffset, 0.0f, 1.0f);
        percVector.x += Mathf.Clamp(((newPos.x - halfCameraWidth) + halfScreenWidth) / zoomOutOffset, -1.0f, 0.0f);
        percVector.y = Mathf.Clamp(((newPos.y + halfCameraHeight) - halfScreenHeight) / zoomOutOffset, 0.0f, 1.0f);
        percVector.y += Mathf.Clamp(((newPos.y - halfCameraHeight) + halfScreenHeight) / zoomOutOffset, -1.0f, 0.0f);

        newPos = Camera.main.transform.position - (offset * (1 - percVector.magnitude));
        
        percVector = Vector3.zero;
        percVector.x = Mathf.Clamp(((newPos.x + halfCameraWidth) - halfScreenWidth) / zoomOutOffset, 0.0f, 1.0f);
        percVector.x += Mathf.Clamp(((newPos.x - halfCameraWidth) + halfScreenWidth) / zoomOutOffset, -1.0f, 0.0f);
        percVector.y = Mathf.Clamp(((newPos.y + halfCameraHeight) - halfScreenHeight) / zoomOutOffset, 0.0f, 1.0f);
        percVector.y += Mathf.Clamp(((newPos.y - halfCameraHeight) + halfScreenHeight) / zoomOutOffset, -1.0f, 0.0f);

        initialPos = Input.mousePosition;
    }

    void OnMouseUp() 
    {
        if (isPan)
        {    
            isPan = false;

            halfCameraHeight = Camera.main.orthographicSize;
            halfCameraWidth = ((Camera.main.orthographicSize * Screen.width) / Screen.height);

            if ((Camera.main.transform.position.x + halfCameraWidth) > halfScreenWidth)
                newPos.x = halfScreenWidth - halfCameraWidth;
            if ((Camera.main.transform.position.x - halfCameraWidth) < -halfScreenWidth)
                newPos.x = -halfScreenWidth + halfCameraWidth;
            if ((Camera.main.transform.position.y + halfCameraHeight) > halfScreenHeight)
               newPos.y = halfScreenHeight - halfCameraHeight;
            if ((Camera.main.transform.position.y - halfCameraHeight) < -halfScreenHeight)
                newPos.y = -halfScreenHeight + halfCameraHeight;

            delimiters.GetComponent<ShowDelimiters>().Reset(newPos, false);

            ExternalEventsManager.Instance.SendMessageToSupport("EndPan", "(" + Camera.main.transform.position.x + ", " + Camera.main.transform.position.y + ")");
        }
    }
    #endregion

    #region Internal Utilities
    GameObject CreateSingleFraction(string name, Element element, bool isSubFraction)
    {
        GameObject root = new GameObject(name);
        root.transform.parent = transform;
        root.transform.position = new Vector3(element.position.x + Camera.main.transform.position.x, element.position.y + Camera.main.transform.position.y, element.position.z);
        root.AddComponent<RootElement>();
        root.SendMessage("SetType", element.type);
        root.SendMessage("SetElementState", element.state);
        root.SendMessage("SetNumerator", element.numerator);
        root.SendMessage("SetDenominator", element.denominator);
        root.SendMessage("SetPartitions", element.partitions);
        root.SendMessage("SetPartNumerator", element.partNumerator);
        root.SendMessage("SetPartDenominator", element.partDenominator);
        root.SendMessage("SetIsSubFraction", isSubFraction);
       // root.SendMessage("SetIsSubFraction", isSubFraction);

        if (element.denominator == 0 && !isSubFraction)
            root.BroadcastMessage("SetMode", InteractionMode.Initializing);
        else
            root.BroadcastMessage("SetMode", InteractionMode.Moving);

        root.SendMessage("SetColor", element.color);

        return root;
    }

    public void CleanElements() 
    {
        for (int i = 0; i < elements.Count; i++)
            DeleteElement(elements[i]);
        if(null != ElementOnFocus)
            DeleteElement(ElementOnFocus);
        for (int i = 0; i < transform.childCount; i++) 
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        elements.Clear();
    }

    GameObject CreateContainer(GameObject f1, GameObject f2)
    {
        //Debug.Log("CreateContainer " +  f1.GetComponent<RootElement>().shape);
        Element element = new Element();
        element.position = Vector3.zero;
        element.color = f1.GetComponent<RootElement>().color; //Workspace.Instance.greenResult;
        element.type = f1.GetComponent<RootElement>().type;
        element.state = ElementsState.Fraction; //ElementsState.Result;
        element.partNumerator = 0;
     //   Debug.Log("f1 partden " + f1.GetComponent<RootElement>().partDenominator + " f2 part den " + f2.GetComponent<RootElement>().partDenominator);
        element.partDenominator = Mathf.Min(f1.GetComponent<RootElement>().partDenominator, f2.GetComponent<RootElement>().partDenominator);
        element.partitions = Mathf.Min(f1.GetComponent<RootElement>().partitions, f2.GetComponent<RootElement>().partitions);
        element.numerator = 0;
        element.denominator =  element.partDenominator / element.partitions;
       
        element.shape = (int)f1.GetComponent<RootElement>().shape;
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
                //root.GetComponent<RootElement>().shape = f1.GetComponent<RootElement>().shape;
                //root.GetComponent<RootElement>().BroadcastMessage("SetSize", new Vector2(Mathf.Max(f1.GetComponent<RootElement>().width, f2.GetComponent<RootElement>().width), Mathf.Max(f1.GetComponent<RootElement>().height, f2.GetComponent<RootElement>().height)));
                //root.GetComponent<RootElement>().UpdateGraphics();
                break;
            case (ElementsType.HeartSet):
                root = CreateHeartSet(element);
                break;
            case (ElementsType.StarSet):
                root = CreateStarSet(element);
                break;
            case (ElementsType.MoonSet):
                root = CreateMoonSet(element);
                break;
            case (ElementsType.Line):
                root = CreateNumberedLine(element);
                break;
            case (ElementsType.Liquid):
                root = CreateLiquidMeasures(element);
                break;
        }

        root.SendMessage("SetMode", InteractionMode.Wait, SendMessageOptions.DontRequireReceiver);
        return root;
    }

    GameObject CreateProperFractions(GameObject f1, GameObject f2)
    {
        Element element = new Element();
        element.position = Vector3.zero;
        element.color = f1.GetComponent<RootElement>().color; //Workspace.Instance.greenResult;
        element.type = f1.GetComponent<RootElement>().type;
        element.state = ElementsState.Fraction; //ElementsState.Result;
        element.shape = (int)f1.GetComponent<RootElement>().shape;
        //Debug.Log("f1 partnun " + f1.GetComponent<RootElement>().partNumerator + " f2 partnumerator " + f2.GetComponent<RootElement>().partNumerator);
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
               // root.GetComponent<RootElement>().BroadcastMessage("SetSize", new Vector2(Mathf.Max(f1.GetComponent<RootElement>().width, f2.GetComponent<RootElement>().width), Mathf.Max(f1.GetComponent<RootElement>().height, f2.GetComponent<RootElement>().height)));
                //root.GetComponent<RootElement>().UpdateGraphics();
                break;
            case (ElementsType.HeartSet):
                root = CreateHeartSet(element);
                break;
            case (ElementsType.StarSet):
                root = CreateStarSet(element);
                break;
            case (ElementsType.MoonSet):
                root = CreateMoonSet(element);
                break;
            case (ElementsType.Line):
                root = CreateNumberedLine(element);
                break;
            case (ElementsType.Liquid):
                root = CreateLiquidMeasures(element);
                break;
        }
        root.SendMessage("SetMode", InteractionMode.Wait, SendMessageOptions.DontRequireReceiver);

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
        //Debug.Log("CreateImproperFractions");
        GameObject root = new GameObject("result_" + (++elemCounter));
        root.transform.parent = transform;
        root.transform.position = Vector3.zero;
        RootElement rootComp = root.AddComponent<RootElement>();
        root.SendMessage("SetMode", InteractionMode.Wait, SendMessageOptions.DontRequireReceiver);
        root.SendMessage("SetColor", grey);
        root.SendMessage("SetType", f1.GetComponent<RootElement>().type);
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
       /* if (null != firstCut.GetComponentsInChildren<SetElement>())
        {
            foreach (SetElement set in firstCut.GetComponentsInChildren<SetElement>())
            {
                set.scalefactorCut = scaleFactor;
            }
            foreach (SetElement set in secondCut.GetComponentsInChildren<SetElement>())
            {
                set.scalefactorCut = scaleFactor;
            }
        }*/
        //f1.GetComponent<RootElement>().parentRef.BroadcastMessage("SetMode", InteractionMode.Moving);
        //f2.GetComponent<RootElement>().parentRef.BroadcastMessage("SetMode", InteractionMode.Moving);
        f1.BroadcastMessage("SetMode", InteractionMode.Moving, SendMessageOptions.DontRequireReceiver);
        f2.BroadcastMessage("SetMode", InteractionMode.Moving, SendMessageOptions.DontRequireReceiver);

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
                ExternalEventsManager.Instance.SendMessageToSupport("ImproperGenerated", "VRects", rootComp.name);
                break;
            case (ElementsType.Liquid):
                ExternalEventsManager.Instance.SendMessageToSupport("ImproperGenerated", "LiquidMeasures", rootComp.name);
                break;
            case (ElementsType.Line):
                ExternalEventsManager.Instance.SendMessageToSupport("ImproperGenerated", "NumberedLines", rootComp.name);
                break;
            case (ElementsType.Set):
                ExternalEventsManager.Instance.SendMessageToSupport("ImproperGenerated", "Sets", rootComp.name);
                break;
            case (ElementsType.HeartSet):
                ExternalEventsManager.Instance.SendMessageToSupport("ImproperGenerated", "HeartSets", rootComp.name);
                break;
            case (ElementsType.MoonSet):
                ExternalEventsManager.Instance.SendMessageToSupport("ImproperGenerated", "MoonSets", rootComp.name);
                break;
            case (ElementsType.StarSet):
                ExternalEventsManager.Instance.SendMessageToSupport("ImproperGenerated", "StarSets", rootComp.name);
                break;
        }

        firstCut = null;
        secondCut = null;

        return root;
    }
    #endregion

    #region Messages

    void SetElementVisibility(bool visibility) 
    {
        foreach (GameObject el in elements) 
        {
            if (null != ElementOnFocus && el != ElementOnFocus)
                el.SetActive(visibility);
        }   
    }

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
        //Debug.Log("wholes " + wholes);
        for (int i = 0; i < wholes; i++)
        {
            Element elem = element;
            if (i == wholes - 1)
            {
                //Debug.Log("diff " + diff);
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
            if (elem.type == ElementsType.HeartSet || elem.type == ElementsType.MoonSet || elem.type == ElementsType.StarSet)
            {
                child.SendMessage("SetFractionBaseOffset", i);
                root.BroadcastMessage("SetBBExtends", new BBExtend(0.0f, 0.5f, 0.0f, 0.5f));
            }

            //work on set
            /*if (element.type == ElementsType.Set)
            {
                //Debug.Log("element shape " + element.shape);
                if (element.shape != -1)
                {
                    child.GetComponent<RootElement>().shape = (SetElement.Shape)element.shape;
                    root.GetComponent<RootElement>().shape = (SetElement.Shape)element.shape;
                    //Debug.Log("-1 NAME " + root.name + " SHAPE " + (SetElement.Shape)element.shape);
                }
                else
                {
                    child.GetComponent<RootElement>().shape = lastShape;
                    root.GetComponent<RootElement>().shape = lastShape;
                    //Debug.Log("ROOT NAME " + root.name + " SHAPE " + lastShape);
                }   
            }*/
           // Debug.Log("MODE " + root.GetComponent<RootElement>().mode);
            child.BroadcastMessage("SetMode", root.GetComponent<RootElement>().mode);
            child.SendMessage("InitializeAs", element.type);
            child.SendMessage("SetEnableCollider", false);
            child.transform.position = root.transform.TransformPoint((wholes - 1 - i) * (new SBSVector3(-(child.GetComponent<RootElement>().width + gap), 0.0f, 0.0f)));
            //Debug.Log("child name " + child.name + " position  " + child.transform.position);
            root.GetComponent<RootElement>().elements.Add(child);
            //Debug.Log("elements of " + root.name + " " + root.GetComponent<RootElement>().elements.Count);
            root.GetComponent<RootElement>().symbol_root = symbol_root;
            root.GetComponent<RootElement>().symbol_root_mobile = symbol_root_mobile;
            root.GetComponent<RootElement>().partition_root = partition_root;
            root.GetComponent<RootElement>().partition_root_mobile = partition_root_mobile; 

            totalHeight = child.GetComponent<RootElement>().height;
            totalWidth += child.GetComponent<RootElement>().width;
        }

        root.GetComponent<RootElement>().height = totalHeight;
        root.GetComponent<RootElement>().width = totalWidth;
        
        
        root.transform.position += new Vector3(totalWidth * 0.25f, 0.0f, 0.0f);
        RepositioningChildren(root);
    }

    public void AddFractionsChildren(GameObject root)
    {
        string label = root.name.Split('_')[0];
        float totalWidth = 0.0f;
        float totalHeight = 0.0f;
        
        RootElement r = root.GetComponent<RootElement>();
        //Debug.Log("1) root "+ root.name+" height  " + r.height + " width " + r.width);
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
            case (ElementsType.HeartSet):
            case (ElementsType.StarSet):
            case (ElementsType.MoonSet):
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
        if(elem.partitions > 1)
            elem.partNumerator = elem.partitions;
        else
            elem.partNumerator = 1;

        elem.numerator = elem.partNumerator / element.partitions;
       // Debug.Log("Father " + gameObject.name + " child " + (i + 1) + "numerator " + elem.numerator);
        GameObject child = CreateSingleFraction(label + "_child" + (i + 1), elem, true);
        child.transform.parent = root.transform;
        //work on set
      /*  if (element.type == ElementsType.Set )
        {      
            child.GetComponent<RootElement>().shape = root.GetComponent<RootElement>().shape ;
        }*/
        if (elem.type == ElementsType.Line)
        {
            child.SendMessage("SetFractionBaseOffset", i);
            child.SendMessage("SetBBExtends", new BBExtend(0.0f, 1.0f, 0.0f, 1.0f));
        }
        if (elem.type == ElementsType.HeartSet || elem.type == ElementsType.MoonSet || elem.type == ElementsType.StarSet)
        {
            child.SendMessage("SetFractionBaseOffset", i);
            child.BroadcastMessage("SetBBExtends", new BBExtend(0.0f, 0.5f, 0.0f, 0.5f));
        }
        child.SendMessage("InitializeAs", element.type);
        child.SendMessage("SetEnableCollider", false);
        //Debug.Log("1) child height  " + child.GetComponent<RootElement>().height + " width " + child.GetComponent<RootElement>().width);

		child.GetComponent<RootElement>().BroadcastMessage("SetSize", new Vector2(child.GetComponent<RootElement>().width, r.height)); 
        totalHeight = child.GetComponent<RootElement>().height;
        totalWidth += child.GetComponent<RootElement>().width;

        child.transform.position = root.transform.TransformPoint((wholes - 1 - i) * (new SBSVector3(-(child.GetComponent<RootElement>().width + gap), 0.0f, 0.0f)));
        //Debug.Log(child.name + " Position1 " + child.transform.position);
        root.GetComponent<RootElement>().elements.Add(child);

        root.GetComponent<RootElement>().height = totalHeight;
        root.GetComponent<RootElement>().width = totalWidth;

        //root.transform.position += new Vector3(totalWidth * 0.25f, 0.0f, 0.0f);

        RepositioningChildren(root);
    }

    public void AddResultFractionsChildren(GameObject root)
    {
        string label = root.name.Split('_')[0];
        float totalWidth = 0.0f;
        float totalHeight = 0.0f;

        RootElement r = root.GetComponent<RootElement>();
        //Debug.Log("1) root "+ root.name+" height  " + r.height + " width " + r.width);
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
            case (ElementsType.HeartSet):
            case (ElementsType.StarSet):
            case (ElementsType.MoonSet):
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
        // Debug.Log("Father " + gameObject.name + " child " + (i + 1) + "numerator " + elem.numerator);
        GameObject child = CreateSingleFraction(label + "_child" + (i + 1), elem, true);
        child.transform.parent = root.transform;
        //work on set
        /*  if (element.type == ElementsType.Set )
          {      
              child.GetComponent<RootElement>().shape = root.GetComponent<RootElement>().shape ;
          }*/
        if (elem.type == ElementsType.Line)
        {
            child.SendMessage("SetFractionBaseOffset", i);
            child.SendMessage("SetBBExtends", new BBExtend(0.0f, 1.0f, 0.0f, 1.0f));
        }
        if (elem.type == ElementsType.HeartSet || elem.type == ElementsType.MoonSet || elem.type == ElementsType.StarSet)
        {
            child.SendMessage("SetFractionBaseOffset", i);
            child.BroadcastMessage("SetBBExtends", new BBExtend(0.0f, 0.5f, 0.0f, 0.5f));
        }
        child.SendMessage("InitializeAs", element.type);
        child.SendMessage("SetEnableCollider", false);
        //Debug.Log("1) child height  " + child.GetComponent<RootElement>().height + " width " + child.GetComponent<RootElement>().width);

        child.GetComponent<RootElement>().BroadcastMessage("SetSize", new Vector2(child.GetComponent<RootElement>().width, r.height));
        totalHeight = child.GetComponent<RootElement>().height;
        totalWidth += child.GetComponent<RootElement>().width;

        child.transform.position = root.transform.TransformPoint((wholes - 1 - i) * (new SBSVector3(-(child.GetComponent<RootElement>().width + gap), 0.0f, 0.0f)));
        //Debug.Log(child.name + " Position1 " + child.transform.position);
        root.GetComponent<RootElement>().elements.Add(child);

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
           
            if (root.GetComponent<RootElement>().type == ElementsType.MoonSet || root.GetComponent<RootElement>().type == ElementsType.HeartSet || root.GetComponent<RootElement>().type == ElementsType.StarSet)
                root.BroadcastMessage("UpdateWidth" , SendMessageOptions.DontRequireReceiver);
           
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
            case (ElementsType.HeartSet):
            case (ElementsType.StarSet):
            case (ElementsType.MoonSet):
                gap = 0.4f;
                break;
            case (ElementsType.Liquid):
                gap = 0.6f;
                break;
        }

        int wholes = element.elements.Count;

       // Debug.Log("whole " + wholes );
        totalWidth += gap * (wholes - 1);

        for (int i = 0; i < wholes; i++)
        {
            GameObject child = element.elements[i];
            child.transform.position = root.transform.TransformPoint((wholes - 1 - i) * (new SBSVector3(-(child.GetComponent<RootElement>().width + gap), 0.0f, 0.0f)));
            //Debug.Log(child.name + " Position2 " + child.transform.position);
            totalHeight = child.GetComponent<RootElement>().height;
            totalWidth += child.GetComponent<RootElement>().width;
        }

       // Debug.Log("totalt width " + totalWidth);
        root.GetComponent<RootElement>().height = totalHeight;

        root.GetComponent<RootElement>().width = totalWidth;

        root.BroadcastMessage("UpdateArrowsState", SendMessageOptions.DontRequireReceiver);
        if (!OperationPending)
            UpdateWS();
    }

    void CreateFraction(Element element) 
    {
        switch (element.type)
        {
            case ElementsType.VRect:
                CreateVRect(element);
                break;
            case ElementsType.HRect:
                CreateHRect(element);
                break;
            case ElementsType.Line:
                CreateNumberedLine(element);
                break;
            case ElementsType.HeartSet:
                CreateHeartSet(element);
                break;
            case ElementsType.MoonSet:
                CreateMoonSet(element);
                break;
            case ElementsType.StarSet:
                CreateStarSet(element);
                break;
            case ElementsType.Liquid:
                CreateLiquidMeasures(element);
                break;

        }

    }

    GameObject CreateSet(Element element) 
    {
       // Debug.Log("createSet");
        string label = "set";
        Debug.Log("CreateSet " + element.numerator + " " + element.denominator + " " + element.partitions + " " + element.partNumerator + " " + element.partDenominator);
        GameObject root = CreateSingleFraction(label + "_" + (++elemCounter), element, false);
        CreateFractionsChildren(element, root, 0.4f, label);
        elements.Push(root);
        UpdateWS();
        root.BroadcastMessage("SetRoot", root);
        ExternalEventsManager.Instance.SendMessageToSupport("FractionGenerated", "Set", root.name);
        return root;
    }

    GameObject CreateHeartSet(Element element)
    {
       // Debug.Log("createheartset");
        string label = "heartset";
        GameObject root = CreateSingleFraction(label + "_" + (++elemCounter), element, false);
        CreateFractionsChildren(element, root, 0.4f, label);
        elements.Push(root);
        UpdateWS();
        root.BroadcastMessage("SetRoot", root);
        ExternalEventsManager.Instance.SendMessageToSupport("FractionGenerated", "HeartSets", root.name);
        return root;
    }

    GameObject CreateStarSet(Element element)
    {
        string label = "starset";
        //Debug.Log("CreateSet " + element.shape);
        GameObject root = CreateSingleFraction(label + "_" + (++elemCounter), element, false);
        CreateFractionsChildren(element, root, 0.4f, label);
        elements.Push(root);
        UpdateWS();
        root.BroadcastMessage("SetRoot", root);
        ExternalEventsManager.Instance.SendMessageToSupport("FractionGenerated", "StarSets", root.name);
        return root;
    }

    GameObject CreateMoonSet(Element element)
    {
        string label = "moonset";
        //Debug.Log("CreateSet " + element.shape);
        GameObject root = CreateSingleFraction(label + "_" + (++elemCounter), element, false);
        root.AddComponent<Rigidbody>();
        root.GetComponent<Rigidbody>().useGravity = false;
        CreateFractionsChildren(element, root, 0.4f, label);
        elements.Push(root);
        UpdateWS();
        root.BroadcastMessage("SetRoot", root);
        ExternalEventsManager.Instance.SendMessageToSupport("FractionGenerated", "MoonSets", root.name);
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
        Debug.Log("CreateNumberedLine");
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
            if (root.transform.GetChild(i).name.Equals("Partition(Clone)"))
                Destroy(root.transform.GetChild(i).gameObject);

        if (root.GetComponent<RootElement>().isHighlighted)
        {
            root.GetComponent<RootElement>().isHighlighted = false;
            Destroy(root.GetComponent<RootElement>().highlight);
            
        }
        float xOffset = (root.transform.position.x > 0.0f) ? -1.0f : 1.0f;
        float yOffset = (root.transform.position.y > 0.0f) ? -1.0f : 1.0f;
        root.transform.position += new Vector3(xOffset, yOffset, 0.0f);
        root.BroadcastMessage("SetType", source.GetComponent<RootElement>().type, SendMessageOptions.DontRequireReceiver);
        //Debug.Log("before color counter"+ colorCounter);
        root.BroadcastMessage("SetColor", GetColor(), SendMessageOptions.DontRequireReceiver);
        //Debug.Log("after color counter"+ colorCounter);
        root.BroadcastMessage("SetBBExtends", source.GetComponent<RootElement>().bbExtends);
        root.GetComponent<RootElement>().equivalences = null;
        root.GetComponent<RootElement>().UpdateGraphics();  
        elements.Push(root);
        UpdateWS();

        ExternalEventsManager.Instance.SendMessageToSupport("FractionCopy", root.name);
    }


    public GameObject CreateEquivalence(GameObject source)
    {
        GameObject root = Instantiate(source) as GameObject;
        string[] splitted = source.name.Split('_');
        elemCounter++;
        root.name = source.name + "_eq" + (root.GetComponent<RootElement>().equivalences.Count + 1);
        root.transform.parent = transform;
        
        if (root.GetComponent<RootElement>().isHighlighted)
        {
            root.GetComponent<RootElement>().isHighlighted = false;
            Destroy(root.GetComponent<RootElement>().highlight);
        }

        float xOffset = 0.0f;
        float yOffset = (root.transform.position.y > 0.0f) ? (-4.0f + Random.Range(-0.5f, 0.5f)) : (4.0f + Random.Range(-0.5f, 0.5f));
        root.transform.position += new Vector3(xOffset, yOffset, 0.0f);
        root.BroadcastMessage("SetType", source.GetComponent<RootElement>().type, SendMessageOptions.DontRequireReceiver);
        //Debug.Log("before color counter"+ colorCounter);
        root.BroadcastMessage("SetColor", source.GetComponent<RootElement>().color, SendMessageOptions.DontRequireReceiver);
        //Debug.Log("after color counter"+ colorCounter);
        root.BroadcastMessage("SetBBExtends", source.GetComponent<RootElement>().bbExtends);
        UpdateWS();
        root.GetComponent<RootElement>().UpdateGraphics();
        elements.Push(root);
        ExternalEventsManager.Instance.SendMessageToSupport("EquivalenceGenerated", root.name);
        root.BroadcastMessage("SetElementState", ElementsState.Equivalence, SendMessageOptions.DontRequireReceiver);
        root.GetComponent<RootElement>().SendMessage("SetPartitioning", SendMessageOptions.DontRequireReceiver);
        return root;
    }


    void CutFraction(GameObject source)
    {
       // Debug.Log("source name " + source.name);
        if (source.GetComponent<RootElement>().state == ElementsState.Cut)
            return;
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
        if (source.GetComponent<RootElement>().state == ElementsState.Equivalence)
            root.GetComponent<RootElement>().isFatherEquivalent = true;
        else
            root.GetComponent<RootElement>().isFatherEquivalent = false;

        source.BroadcastMessage("SetMode", InteractionMode.Freeze);
        UpdateWS();

        ExternalEventsManager.Instance.SendMessageToSupport("FractionCut", root.name);
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

        if (null != element.GetComponent<RootElement>().parentEqRef && element.GetComponent<RootElement>().state == ElementsState.Equivalence)
            element.GetComponent<RootElement>().parentEqRef.BroadcastMessage("DeleteEquivalence", element);

        if (element.GetComponent<RootElement>().state != ElementsState.Equivalence && null != element.GetComponent<RootElement>().equivalences && element.GetComponent<RootElement>().equivalences.Count > 0 && element.GetComponent<RootElement>().state != ElementsState.Cut)
        {
            foreach (GameObject eq in element.GetComponent<RootElement>().equivalences) 
            {
                eq.GetComponent<RootElement>().parentEqRef = null;
            }
        }

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


    public void RescaleFractionModifier(float factorScale) 
    {
       // Debug.Log("rescalefraction");
        actualFactorScale = factorScale;
        for (int i = 0; i < elements.Count; i++) 
        {
            elements[i].GetComponent<RootElement>().RescaleModifier(factorScale);         
        }
       
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
            firstCut.BroadcastMessage("SetMode", InteractionMode.Moving, SendMessageOptions.DontRequireReceiver);
            firstCut = null;
        }

        if (null != secondCut)
        {
            secondCut.BroadcastMessage("SetMode", InteractionMode.Moving, SendMessageOptions.DontRequireReceiver);
            secondCut = null;
        }
    }

    void StartCurrentAction()
    {
        isAction = true;
        prevSize = Camera.main.orthographicSize;
        prevPos = Camera.main.transform.position;
        preScale = Camera.main.transform.localScale;
        // Debug.Log("S prevsize " + prevSize + " prevPs " + prevPos + " prevscale " + preScale);

        Camera.main.orthographicSize = 10.0f;
        Camera.main.transform.position = new Vector3(0.0f, 0.0f, -10.0f);
        Camera.main.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

       // Debug.Log("is input " + inputEnabled);
        //Debug.Log("START CURRENT ACTION");
      
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
                    Camera.main.orthographicSize = prevSize;
                    Camera.main.transform.position = prevPos;
                    Camera.main.transform.localScale = preScale;
                    isAction = false;
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

    bool isEquivalent = false;
    void TerminateCurrentAction()
    {
        //Debug.Log("T prevsize " + prevSize + " prevPs " + prevPos + " prevscale " + preScale );
        Camera.main.orthographicSize = prevSize;
        Camera.main.transform.position = prevPos;
        Camera.main.transform.localScale = preScale;

       // Debug.Log("C size " + Camera.main.orthographicSize + " prevPs " + Camera.main.transform.position + " prevscale " + Camera.main.transform.localScale);

        if (null != firstCut && null != secondCut)
        {
            RootElement firstCutRoot = firstCut.GetComponent<RootElement>();
            RootElement secondCutRoot = secondCut.GetComponent<RootElement>();
            //Debug.Log("state first " + firstCut.GetComponent<RootElement>().state + " second state " + secondCut.GetComponent<RootElement>().state);
            if (firstCut.GetComponent<RootElement>().isFatherEquivalent && secondCut.GetComponent<RootElement>().isFatherEquivalent)
                isEquivalent = true; 
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
        Vector3 containerPos = Camera.main.transform.position + new Vector3(-containerBB.center.x, 0.0f, 0.0f) + new Vector3(0.0f, -(containerBB.size.y + 0.1f), 0.0f);
        containerPos.z = 0.0f;
        resultObject.transform.position = containerPos;
       // resultObject.transform.position = Camera.main.ScreenToWorldPoint(Camera.main.transform.position);
        if (resultObject.GetComponent<RootElement>().state != ElementsState.Improper)
            resultObject.SendMessage("AttachSymbol", true);

        resultObject.SendMessage("SetMode", InteractionMode.Moving, SendMessageOptions.DontRequireReceiver);
        
        if (null != containerObject)
        {
            elements.Remove(containerObject);
            Destroy(containerObject);
            containerObject = null;
        }

        if (isEquivalent && resultObject.GetComponent<RootElement>().type != ElementsType.HeartSet && resultObject.GetComponent<RootElement>().type != ElementsType.MoonSet && resultObject.GetComponent<RootElement>().type != ElementsType.StarSet)
        {
            GameObject tmp = CreateEquivalence(resultObject);
            tmp.GetComponent<RootElement>().parentEqRef = null;
            DeleteElement(resultObject);
        }

        UpdateWS();
        isAction = false;
    }
    #endregion

    #region Coroutines
    IEnumerator MakeProperJoin()
    {
        //Debug.Log("MakeProperJoin");

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
            if (null != firstCut.GetComponentsInChildren<SetElement>())
            {
                foreach (SetElement set in firstCut.GetComponentsInChildren<SetElement>())
                {
                    set.scalefactorCut = scaleFactor;
                }
                foreach (SetElement set in secondCut.GetComponentsInChildren<SetElement>())
                {
                    set.scalefactorCut = scaleFactor;
                }
            }

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
       
       // containerObject.GetComponent<RootElement>().BroadcastMessage("SetSize", new Vector2(firstCut.GetComponent<RootElement>().width, firstCut.GetComponent<RootElement>().height));
        BoxCollider containerBB = containerObject.GetComponent<BoxCollider>();
        float offset = -0.50f;
        if (firstCut.GetComponent<RootElement>().type == ElementsType.Set || firstCut.GetComponent<RootElement>().type == ElementsType.HeartSet || firstCut.GetComponent<RootElement>().type == ElementsType.StarSet || firstCut.GetComponent<RootElement>().type == ElementsType.MoonSet)
            offset = -containerBB.size.y*0.5f;

        Vector3 containerPos = Vector3.zero + new Vector3(-containerBB.center.x - 1.2f, 0.0f, 0.0f) + new Vector3(0.0f, -(containerBB.size.y + offset), 0.0f);
        containerPos = Vector3.zero + new Vector3(-containerBB.center.x, 0.0f, 0.0f) + new Vector3(0.0f, -(containerBB.size.y + offset), 0.0f);
        containerObject.transform.position = containerPos;
        containerObject.SendMessage("AttachSymbol", true);
        containerObject.SendMessage("DisableInput");
        
        yield return new WaitForSeconds(0.2f);

        int numerator = firstCut.GetComponent<RootElement>().partNumerator;
       // if(firstCut.GetComponent<RootElement>().partitions > 1 && secondCut.GetComponent<RootElement>().partitions > 1)
          //  numerator = firstCut.GetComponent<RootElement>().numerator;
        for (int i = 0; i < numerator; i++)
        {
            yield return new WaitForSeconds(1.0f);

            firstCut.SendMessage("DecreaseCutNumerator");
            containerObject.GetComponent<RootElement>().IncreaseResultNumerator();
            //containerObject.GetComponent<RootElement>().IncreaseNumerator();
            containerObject.GetComponent<RootElement>().UpdateGraphics();
            yield return new WaitForEndOfFrame();
            containerPos = Vector3.zero + new Vector3(-containerBB.center.x, 0.0f, 0.0f) + new Vector3(0.0f, -(containerBB.size.y + offset), 0.0f);
            containerObject.transform.position = containerPos;
        }
       
        numerator = secondCut.GetComponent<RootElement>().partNumerator;
      //  if (firstCut.GetComponent<RootElement>().partitions > 1 && secondCut.GetComponent<RootElement>().partitions > 1)
         //   numerator = secondCut.GetComponent<RootElement>().numerator;

        for (int i = 0; i < numerator; i++)
        {
            yield return new WaitForSeconds(1.0f);
            secondCut.SendMessage("DecreaseCutNumerator");
            containerObject.GetComponent<RootElement>().IncreaseResultNumerator();
            containerObject.GetComponent<RootElement>().UpdateGraphics();
            yield return new WaitForEndOfFrame();
            containerPos = Vector3.zero + new Vector3(-containerBB.center.x, 0.0f, 0.0f) + new Vector3(0.0f, -(containerBB.size.y + offset), 0.0f);
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

        if (firstCut.GetComponent<RootElement>().isFatherEquivalent && secondCut.GetComponent<RootElement>().isFatherEquivalent)
            isEquivalent = true; 

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
            if (null != firstCut.GetComponentsInChildren<SetElement>())
            {
                foreach (SetElement set in firstCut.GetComponentsInChildren<SetElement>())
                {
                    set.scalefactorCut = scaleFactor;
                }
                foreach (SetElement set in secondCut.GetComponentsInChildren<SetElement>())
                {
                    set.scalefactorCut = scaleFactor;
                }
            }
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
        //if (secondCut.GetComponent<RootElement>().partitions > 1)
         //   numerator = secondCut.GetComponent<RootElement>().numerator;
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
        float offset = -0.50f;
        if (firstCut.GetComponent<RootElement>().type == ElementsType.Set || firstCut.GetComponent<RootElement>().type == ElementsType.HeartSet || firstCut.GetComponent<RootElement>().type == ElementsType.StarSet || firstCut.GetComponent<RootElement>().type == ElementsType.MoonSet)
            offset = -containerBB.size.y * 0.5f;
        Vector3 containerPos = Vector3.zero + new Vector3(-containerBB.center.x, 0.0f, 0.0f) + new Vector3(0.0f, -(containerBB.size.y + offset), 0.0f);
        containerObject.transform.position = containerPos;
        containerObject.SendMessage("AttachSymbol", true);
        containerObject.SendMessage("DisableInput");

        yield return new WaitForSeconds(0.2f);

        numerator = firstCut.GetComponent<RootElement>().partNumerator - secondCut.GetComponent<RootElement>().partNumerator;

        for (int i = 0; i < numerator; i++)
        {
            yield return new WaitForSeconds(1.0f);
            firstCut.SendMessage("DecreaseCutNumerator");
            containerObject.GetComponent<RootElement>().IncreaseResultNumerator();
            containerObject.GetComponent<RootElement>().UpdateGraphics();
            yield return new WaitForEndOfFrame();
            containerPos = Vector3.zero + new Vector3(-containerBB.center.x, 0.0f, 0.0f) + new Vector3(0.0f, -(containerBB.size.y + offset), 0.0f);
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

        if (firstCut.GetComponent<RootElement>().isFatherEquivalent && secondCut.GetComponent<RootElement>().isFatherEquivalent)
            isEquivalent = true; 

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
            if (null != firstCut.GetComponentsInChildren<SetElement>())
            {
                foreach (SetElement set in firstCut.GetComponentsInChildren<SetElement>())
                {
                    set.scalefactorCut = scaleFactor;
                }
                foreach (SetElement set in secondCut.GetComponentsInChildren<SetElement>())
                {
                    set.scalefactorCut = scaleFactor;
                }
            }
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
        bool checkRepresentations = ((firstCutRoot.type == secondCutRoot.type)
            || ((firstCutRoot.type == ElementsType.HRect) && (secondCutRoot.type == ElementsType.VRect))
            || ((firstCutRoot.type == ElementsType.VRect) && (secondCutRoot.type == ElementsType.HRect)));
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
        //Debug.Log("updatews");
        for (int i = 0; i < elements.Count; i++)
        {
            if (elements[i].GetComponent<RootElement>().mode != InteractionMode.Wait)
                elements[i].SendMessage("Draw", i);

            if (i > 0)
            {
                if (elements[i].GetComponent<RootElement>().mode != InteractionMode.Freeze)
                {
                    if ((null != elements[i].GetComponent<RootElement>().parentEqRef && elements[i].GetComponent<RootElement>().parentEqRef.GetComponent<RootElement>().mode != InteractionMode.Changing) || null == elements[i].GetComponent<RootElement>().parentEqRef)
                    {
                            elements[i].SendMessage("SetMode", InteractionMode.Moving, SendMessageOptions.DontRequireReceiver);
                        //Debug.Log("UpdateWs");
                    }
                    //elements[i].BroadcastMessage("UpdateWidth");
                }

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
        foreach (RootElement root in gameObject.GetComponentsInChildren<RootElement>())
            root.InitHighlight(name, false);
            //gameObject.BroadcastMessage("InitHighlight", name, SendMessageOptions.DontRequireReceiver);
    }

    public void DestroyHighlight()
    {
        interfaces.SendMessage("HideInterfaceHighlight");
       // Destroy(GameObject.FindGameObjectWithTag("Highlight"));
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
