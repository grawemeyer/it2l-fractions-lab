using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using fractionslab.utils;
using fractionslab.behaviours;
using SBS.Math;
using System;

public class InterfaceBehaviour : MonoBehaviour
{
    public const string VER = "0.276";
    public float min_orthographicsize = 10.0f;

    #region Protected Fields
    public bool isStudent = true;
    public bool isLogged = false;
    public bool istaskDefined = false;
    public bool showStartPage = true;
    public string user = "UserProve";
    public string fileAddress;
    public Task task;
    protected Camera mainCamera;
    protected bool inputEnabled = true;
    public LocalizationUtils localizationUtils;
    protected Dictionary<string, GameObject> highlightUIElement = new Dictionary<string, GameObject>();
    protected List<SBSVector3> startingPoints = new List<SBSVector3>();
    protected GameObject bookmark;
    public Texture2D MouseIconNo;
    public Texture2D MouseIconOv;

    //Popup
    protected LinkedList<string> popupsList = new LinkedList<string>();
    public delegate void MenuToolDelegate();

    protected FractionsOperations lastOperation;
    [SerializeField]
    protected bool elementSelected = false;
    protected GameObject dropOverMc;

    protected string suggestionTxt = "";
    protected string lowFeedbackTxt = "";

    public bool isPressingButton = false;
    public bool isPressingOperation = false;
    public bool isDragWindow = false;
    public bool isBlockingOperation = false;
    public bool isZoomActive = false;
    public bool isMenuActive = false;

    protected GameObject elementOnFocus = null;
    protected GameObject actionsPopupBG = null;

    protected bool isOperationShown = false;
    protected bool isSmall = false;
    protected int indexFractionArea;

    protected float toolsMenuButtonsBgWidth = 0;
    protected float actionsMenuButtonsBgWidth = 0;

    protected bool isOnTrash = false;
    protected bool isMenuOut = true;
    protected List<Fraction> fractionOperation2;
    protected List<Fraction> fractionOperation3;

    protected Dictionary<string, Button> buttonDictionary;
    #endregion

    public static Color Green1 = new Color(0.2784f, 0.4510f, 0.1922f, 1.0f);
    public static Color Green2 = new Color(0.3216f, 0.5216f, 0.2235f, 1.0f);
    public static Color Orange = new Color(1.0f, 0.4235f, 0.1490f, 1.0f);
    public static Color ClearGreen = new Color(0.32f, 0.521f, 0.2234f, 1.0f);
    public static Color DarkGreen = new Color(0.106f, 0.20f, 0.06f, 1.0f);
    public static Color Grey = new Color(0.48f, 0.52f, 0.46f, 1.0f);



    public GameObject ui;
    public List<Canvas> canvas;
    public List<GameObject> popups;
    public List<GameObject> subMenu;
    public List<GameObject> placeholders;
    public List<GameObject> zoomElements;
    public GameObject zoomController;
    public GameObject colorMenu;
    public GameObject contextMenu;
    public GameObject contextMenuEquivalence;
    public GameObject actionMenu;

    public List<Button> barTool;
    public GameObject sideBar;
    public List<Button> barOperations;
    public List<Button> barTeacher;

    public GameObject popupmsg;
    public GameObject symbolPrefab;
    public GameObject partitionPrefab;
    public GameObject overlayablePage;
    public Text hintText;
    public GameObject topBottom;
    public GameObject topUp;
    public GameObject trash;
    //public GameObject symbol;
    public GameObject saveAs;
    public GameObject saveNew;
    public GameObject notification;
    public Button taskDescription;
    public GameObject home;
    public GameObject userLabel;
    public GameObject highlight;
    public List<Button> initialConfigurationList;

    protected int indexHighlight;
    protected List<string> nameHighlight;

    protected Stack<GameObject> popupsStack = new Stack<GameObject>();

    #region Protected Struct
    protected struct Fraction
    {
        public float numerator;
        public float denominator;
        public Fraction(float p1, float p2)
        {
            numerator = p1;
            denominator = p2;
        }
    }

    public struct Configuration
    {
        public string name;
        public GameObject operation;
        public bool isActive;
    }


    public enum FractionsOperations
    {
        FIND = 0,
        ADDITION,
        SUBTRACTION
    }
    #endregion

    public bool InputEnabled
    {
        get { return inputEnabled; }
    }

    public Text visitedTask;
    public Text lockedTask;
    public Text newTask;


    #region Unity Callbacks
    void Awake()
    {
        indexFractionArea = -1;
        bookmark = GameObject.FindGameObjectWithTag("Bookmark");
        bookmark.SetActive(false);
        Cursor.SetCursor(MouseIconNo, Vector2.zero, CursorMode.Auto);

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        startingPoints.Add(SBSVector3.zero);
        startingPoints.Add(SBSVector3.left);
        startingPoints.Add(SBSVector3.right);
        startingPoints.Add(SBSVector3.up);
        startingPoints.Add(SBSVector3.down);
        lastOperation = FractionsOperations.SUBTRACTION;
        localizationUtils = GameObject.FindGameObjectWithTag("LevelRoot").GetComponent<LocalizationUtils>();

        Localizations.Instance.mcLanguage = "en";
        //if (Application.srcValue.Contains("language=de"))
        if (ExternalEventsManager.Instance.embeddingVariables.ContainsKey("language"))
        {
            if (ExternalEventsManager.Instance.embeddingVariables["language"].Equals("de"))
                Localizations.Instance.mcLanguage = "de";
            else if (ExternalEventsManager.Instance.embeddingVariables["language"].Equals("es"))
                Localizations.Instance.mcLanguage = "es";
        }

        if (ExternalEventsManager.Instance.embeddingVariables.ContainsKey("username"))
        {
            if ("" != ExternalEventsManager.Instance.embeddingVariables["username"])
            {
                user = ExternalEventsManager.Instance.embeddingVariables["username"];
                isLogged = true;
                isStudent = true;
            }
            else
            {
                isLogged = false;
            }
        }
        if (ExternalEventsManager.Instance.embeddingVariables.ContainsKey("tip"))
        {
            if ("" != ExternalEventsManager.Instance.embeddingVariables["tip"])
            {
                fileAddress = ExternalEventsManager.Instance.embeddingVariables["tip"];
                istaskDefined = true;
            }
            else
            {
                istaskDefined = false;
            }
        }

        if (ExternalEventsManager.Instance.embeddingVariables.ContainsKey("showStartPage"))
        {
            if ("" != ExternalEventsManager.Instance.embeddingVariables["showStartPage"])
            {
                if (ExternalEventsManager.Instance.embeddingVariables["showStartPage"] == "true")
                    showStartPage = true;
                else
                    showStartPage = false;
            }
        }

        /*   mappingInitConfiguration = new List<Configuration>();
           Configuration tmp = new Configuration();
           foreach(Button bt in initialConfigurationList)
           {
               tmp.operation = bt.gameObject;
               tmp.isActive = true;
               mappingInitConfiguration.Add(tmp);       
           }*/


        /*CHEAT*/
        //  symbol.GetComponent<UIButton>().DisableBtn(false);
        saveAs.GetComponent<UIButton>().DisableBtn(false);
        saveNew.GetComponent<UIButton>().DisableBtn(false);

        buttonDictionary = new Dictionary<string, Button>();
        foreach (Button bt in barTool)
        {
            buttonDictionary.Add(bt.name, bt);
        }
        foreach (Button bt in barOperations)
        {
            buttonDictionary.Add(bt.name, bt);
        }
        foreach (Button bt in contextMenu.GetComponentsInChildren<Button>())
        {
            buttonDictionary.Add(bt.name, bt);
        }
        foreach (Button bt in contextMenuEquivalence.GetComponentsInChildren<Button>())
        {
            buttonDictionary.Add(bt.name, bt);
        }
        foreach (Button bt in actionMenu.GetComponentsInChildren<Button>())
        {
            buttonDictionary.Add(bt.name, bt);
        }

        //initialConfigurationList = new Dictionary<string, Button>();
        contextMenu.SetActive(false);
        contextMenuEquivalence.SetActive(false);
        actionMenu.SetActive(false);
        isMenuActive = false;

        Localizations.Instance.initialize();
        Localizations.Instance.listeners.Add(gameObject);
        Localizations.Instance.listeners.Add(localizationUtils.gameObject);

        localizationUtils.AddTranslationText(bookmark.GetComponentInChildren<Text>(), "{designer_mode}");
        localizationUtils.AddTranslationText(visitedTask, "{visited_task}");
        localizationUtils.AddTranslationText(lockedTask, "{locked_task}");
        localizationUtils.AddTranslationText(newTask, "{new_task}");
        initializeHighlightUIElement();
        indexHighlight = 0;
        actionsPopupBG = GameObject.FindGameObjectWithTag("PopupBG");
        actionsPopupBG.SetActive(false);
        
#if UNITY_ANDROID || UNITY_IPHONE
        zoomController.GetComponent<Zoom>().HideUI();
#endif
        //if(isStudent)

        /*CHEAT*/
        /*  isLogged = false;
          istaskDefined = false;*/

    }

    IEnumerator Start()
    {
        Debug.Log("dd " + Debug.isDebugBuild);
        Cursor.SetCursor(MouseIconNo, Vector2.zero, CursorMode.Auto);
       // Debug.Log("isLogged " + isLogged + " showStartPage " + showStartPage + " istaskdefined " + istaskDefined);
        if (!isLogged)
        {
            GoToPage("LoginPage");
        }
        else
        {
            if (showStartPage)
            {
                GoToPage("StartPage");
            }
            else if (istaskDefined)
            {
                // Debug.Log("file address " + fileAddress);
                home.SetActive(showStartPage);
                userLabel.SetActive(showStartPage);
                yield return StartCoroutine(TaskManager.Instance.LoadJson(fileAddress));
                if (TaskManager.Instance.isLoad)
                    LoadWorkspace(TaskManager.Instance.task);
                else
                    NewWorkspace();
            }
            else if (!istaskDefined){
                home.SetActive(showStartPage);
                userLabel.SetActive(showStartPage);
                NewWorkspace();
            }

           
        }
    }

    void initializeHighlightUIElement()
    {
        highlightUIElement.Add("mcHints", hintText.gameObject.transform.parent.gameObject);
        highlightUIElement.Add("mcTrash", trash);
        highlightUIElement.Add("mcRepresentationBar", sideBar);
        highlightUIElement.Add("btLines", barTool[0].gameObject);
        highlightUIElement.Add("btRects", barTool[1].gameObject);
        highlightUIElement.Add("btSets", barTool[2].gameObject);
        highlightUIElement.Add("btLiquid", barTool[3].gameObject);
        highlightUIElement.Add("mcOperationBar", topUp);
        highlightUIElement.Add("home", home);
        highlightUIElement.Add("taskDescription", taskDescription.gameObject);
        highlightUIElement.Add("zoom", zoomController);

        if (Debug.isDebugBuild)
        {
            nameHighlight = new List<string>();
            nameHighlight.Add("mcHints");
            nameHighlight.Add("mcTrash");
            nameHighlight.Add("mcRepresentationBar");
            nameHighlight.Add("btLines");
            nameHighlight.Add("btRects");
            nameHighlight.Add("btSets");
            nameHighlight.Add("btLiquid");
            nameHighlight.Add("mcOperationBar");
            nameHighlight.Add("home");
            nameHighlight.Add("zoom");
            nameHighlight.Add("taskDescription");
        }

    }
    //TODOUI
    public void InterfaceHighlightByName(string name)
    {
        if (highlightUIElement.ContainsKey(name))
        {
            if (name.Equals("mcRepresentationBar"))
            {
                if (isMenuOut)
                    InterfaceHighlight(highlightUIElement[name]);
                else
                {
                    ArrowButtonCallback(true);
                }
            }
            else
                InterfaceHighlight(highlightUIElement[name]);
        }
    }
    //TODOUI

    public void InterfaceHighlight(GameObject m)
    {
        //DestroyImmediate(GameObject.FindGameObjectWithTag("Highlight"));
        //HideInterfaceHighlight();
        highlight.SetActive(true);
        highlight.transform.parent = m.transform;
        highlight.transform.SetAsLastSibling();
        highlight.GetComponent<RectTransform>().localScale = Vector3.one;
        highlight.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        if (null != m.GetComponent<RectTransform>())
            highlight.GetComponent<RectTransform>().sizeDelta = new Vector2(m.GetComponent<RectTransform>().sizeDelta.x+10.0f, m.GetComponent<RectTransform>().sizeDelta.y+10.0f);
        else
            highlight.GetComponent<RectTransform>().sizeDelta = new Vector2(m.GetComponent<RootElement>().width+10f, m.GetComponent<RootElement>().height+10f);
    }

    public void RapresentationHighlight(GameObject m, Vector2 size, Vector3 center)
    {
        //DestroyImmediate(GameObject.FindGameObjectWithTag("Highlight"));
        //HideInterfaceHighlight();
        //float xOffset = 0.0f;
        float widthOffeset = 40.0f;
        float heightOffset = 0.7f;
        if (m.GetComponent<RootElement>().type == ElementsType.Liquid)
            widthOffeset = 40.0f;
        if (m.GetComponent<RootElement>().type == ElementsType.StarSet || m.GetComponent<RootElement>().type == ElementsType.MoonSet || m.GetComponent<RootElement>().type == ElementsType.HeartSet)
            heightOffset = 4.2f - size.y;
        if (heightOffset < 0.7f)
            heightOffset = 0.7f;
        highlight.SetActive(true);
        highlight.transform.parent = Workspace.Instance.gameObject.transform;
        highlight.GetComponent<RectTransform>().localScale = Vector3.one * 0.03333334f;
        //highlight.transform.SetAsLastSibling();
        //highlight.GetComponent<RectTransform>().localScale = Vector3.one;
        //center.x += 0.3f; 
        highlight.GetComponent<RectTransform>().anchoredPosition = center;//new Vector2(center.x - m.transform.position.x, 0.0f);
        highlight.GetComponent<RectTransform>().sizeDelta = new Vector2((size.x / highlight.GetComponent<RectTransform>().localScale.x) + widthOffeset, (size.y + heightOffset) / highlight.GetComponent<RectTransform>().localScale.y);
    }

    public void HideInterfaceHighlight()
    {
        if (null != highlight)
        {
            highlight.transform.SetParent(null);
            highlight.SetActive(false);
        }
    }


    void Update()
    {

        //Debug.Log("inputEnabled " + inputEnabled);
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Z))
            ShowHighFeedback("PROVA");
#endif
        if (Input.GetKeyDown(KeyCode.G) && Debug.isDebugBuild)
        {
            InterfaceHighlightByName(nameHighlight[(indexHighlight++) % nameHighlight.Count]);
            //Debug.Log("hightlight " + nameHighlight[indexHighlight%nameHighlight.Count] + " index " + indexHighlight);
        }
        //#endif
        if (null != Workspace.Instance.ElementOnFocus)
        {
            if ((Input.mousePosition.x > Screen.width || Input.mousePosition.x < 0) || (Input.mousePosition.y > Screen.height || Input.mousePosition.y < 0))
            {
                if (null != topBottom)
                {
                    if (!topBottom.GetComponent<TopBottom>().isClose)
                    {
                        if (indexFractionArea == -1)
                        {
                            //Debug.Log("ismouseout");
                            isMouseOut = true;
                        }
                    }
                }
            }
            else
            {
                isMouseOut = false;
            }
        }
        if ((Input.GetMouseButtonDown(0) || Input.touchCount > 1) && !isPressingButton)
        {
            OnHideContextMenu();
            OnHideActionsMenu();
            OnHideSubMenus();
           
            if (!actionsPopupBG.activeSelf)
                Workspace.Instance.SendMessage("ResetAction");
          /*  if (null != Workspace.Instance.ElementOnFocus && Workspace.Instance.ElementOnFocus.GetComponent<RootElement>().mode == InteractionMode.LookAt)
                Camera.main.GetComponent<LookAtFraction>().ResetLookAt();*/
           // isMenuActive = false;
        }
        UpdateSidebarTween();


        /*#if UNITY_IPHONE || UNITY_ANDROID
                bool isOver = false;
                if (elementSelected && !topBottom.GetComponent<TopBottom>().isClose)
                {
                    if (area1 != null)
                        isOver = isOver || IsOverHUDElement(area1);
                    isOver = isOver || IsOverHUDElement(area2) || IsOverHUDElement(area3);
                }

                if (trash != null)
                {
                    if(IsOverHUDElement(trash) || isOver)
                    {
                        if(!isSmall && elementSelected)
                        {
                            Debug.Log("scaledown trash");
                            isSmall = true;
                            isOnTrash = true;
                            trash.GetComponent<RectTransform>().localScale = new Vector3(0.8f, 0.8f, 0.8f);
                            Workspace.Instance.ElementOnFocus.SendMessage("ScaleDown");
                        }
                    }
                    else
                    {
                        if(isSmall && elementSelected)
                        {
                            Debug.Log("scaledown trash");
                            isSmall = false;
                            isOnTrash = false;
                            trash.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
                            Workspace.Instance.ElementOnFocus.SendMessage("ScaleUp");
                        }
                    }
                }
        #endif*/

    }

    //OK
    void OnGUI()
    {
        GUI.skin = Workspace.Instance.skin;
        string ver = VER;
#if UNITY_IPHONE  || UNITY_ANDROID
        ver += " mobile";
#elif UNITY_WEBGL
        ver += " webGL preview";
#endif
        GUI.Label(new Rect(4.0f, /*Screen.height -*/ 2.0f, 100, 20), "Ver " + ver);
    
    }

    #endregion

    #region Functionalities
    //OK
    public void SetIsStudent()
    {
        isStudent = !isStudent;
        if (isStudent)
        {
            bookmark.SetActive(false);
        }
        else
        {
            bookmark.SetActive(true);
        }
    }

    #region CreateElement
    void CreateSet()
    {
        CreateSet(0, 0, 1, 0, 0);
    }

    void CreateSet(int numerator, int denominator, int partitions, int partNumerator, int partDenominator)
    {

        //Debug.Log("infacebehaviour CreateSet");
        int r = UnityEngine.Random.Range(0, 10000) % startingPoints.Count;
        Element element = new Element();
        element.position = startingPoints[r];
        element.color = Workspace.Instance.GetColor();
        element.type = ElementsType.Set;
        element.state = ElementsState.Fraction;
        element.numerator = numerator;
        element.denominator = denominator;
        element.partitions = partitions;
        element.partNumerator = partNumerator;
        element.partDenominator = partDenominator;
        element.shape = -1;
        Workspace.Instance.SendMessage("CreateSet", element);

    }


    void CreateHeartSet()
    {
        CreateHeartSet(0, 0, 1, 0, 0);
    }

    void CreateHeartSet(int numerator, int denominator, int partitions, int partNumerator, int partDenominator)
    {
        int r = UnityEngine.Random.Range(0, 10000) % startingPoints.Count;
        Element element = new Element();
        element.position = startingPoints[r];
        element.color = Workspace.Instance.GetColor();
        element.type = ElementsType.HeartSet;
        element.state = ElementsState.Fraction;
        element.numerator = numerator;
        element.denominator = denominator;
        element.partitions = partitions;
        element.partNumerator = partNumerator;
        element.partDenominator = partDenominator;
        element.shape = -1;
        Workspace.Instance.SendMessage("CreateHeartSet", element);

    }

    void CreateMoonSet()
    {
        CreateMoonSet(0, 0, 1, 0, 0);
    }

    void CreateMoonSet(int numerator, int denominator, int partitions, int partNumerator, int partDenominator)
    {
        //Debug.Log("infacebehaviour CreateSet");
        int r = UnityEngine.Random.Range(0, 10000) % startingPoints.Count;
        Element element = new Element();
        element.position = startingPoints[r];
        element.color = Workspace.Instance.GetColor();
        element.type = ElementsType.MoonSet;
        element.state = ElementsState.Fraction;
        element.numerator = numerator;
        element.denominator = denominator;
        element.partitions = partitions;
        element.partNumerator = partNumerator;
        element.partDenominator = partDenominator;
        element.shape = -1;
        Workspace.Instance.SendMessage("CreateMoonSet", element);

    }

    void CreateStarSet()
    {
        CreateStarSet(0, 0, 1, 0, 0);
    }

    void CreateStarSet(int numerator, int denominator, int partitions, int partNumerator, int partDenominator)
    {
        //Debug.Log("infacebehaviour CreateSet");
        int r = UnityEngine.Random.Range(0, 10000) % startingPoints.Count;
        Element element = new Element();
        element.position = startingPoints[r];
        element.color = Workspace.Instance.GetColor();
        element.type = ElementsType.StarSet;
        element.state = ElementsState.Fraction;
        element.numerator = numerator;
        element.denominator = denominator;
        element.partitions = partitions;
        element.partNumerator = partNumerator;
        element.partDenominator = partDenominator;
        element.shape = -1;
        Workspace.Instance.SendMessage("CreateStarSet", element);

    }



    void CreateHRect()
    {
        CreateHRect(0, 0, 1, 0, 0);
    }

    void CreateHRect(int numerator, int denominator, int partitions, int partNumerator, int partDenominator)
    {
        int r = UnityEngine.Random.Range(0, 10000) % startingPoints.Count;
        Element element = new Element();
        element.position = startingPoints[r];
        //element.position = Camera.main.transform.position + startingPoints[r];
        element.color = Workspace.Instance.GetColor();
        element.type = ElementsType.HRect;
        element.state = ElementsState.Fraction;
        element.numerator = numerator;
        element.denominator = denominator;
        element.partitions = partitions;
        element.partNumerator = partNumerator;
        element.partDenominator = partDenominator;
        Workspace.Instance.SendMessage("CreateHRect", element);
    }

    void CreateVRect()
    {
        CreateVRect(0, 0, 1, 0, 0);
    }

    void CreateVRect(int numerator, int denominator, int partitions, int partNumerator, int partDenominator)
    {
        int r = UnityEngine.Random.Range(0, 10000) % startingPoints.Count;
        Element element = new Element();
        element.position = startingPoints[r];
        element.color = Workspace.Instance.GetColor();
        element.type = ElementsType.VRect;
        element.state = ElementsState.Fraction;
        element.numerator = numerator;
        element.denominator = denominator;
        element.partitions = partitions;
        element.partNumerator = partNumerator;
        element.partDenominator = partDenominator;
        Workspace.Instance.SendMessage("CreateVRect", element);
    }

    void CreateNumberedLine()
    {
        CreateNumberedLine(0, 0, 1, 0, 0);
    }

    void CreateNumberedLine(int numerator, int denominator, int partitions, int partNumerator, int partDenominator)
    {
        //Debug.Log("CreateNumberedLine");
        int r = UnityEngine.Random.Range(0, 10000) % startingPoints.Count;
        Element element = new Element();
        element.position = startingPoints[r];
        element.color = Workspace.Instance.GetColor();
        element.type = ElementsType.Line;
        element.state = ElementsState.Fraction;
        element.numerator = numerator;
        element.denominator = denominator;
        element.partitions = partitions;
        element.partNumerator = partNumerator;
        element.partDenominator = partDenominator;
        Workspace.Instance.SendMessage("CreateNumberedLine", element);
    }

    void CreateLiquidMeasures()
    {
        CreateLiquidMeasures(0, 0, 1, 0, 0);
    }

    void CreateLiquidMeasures(int numerator, int denominator, int partitions, int partNumerator, int partDenominator)
    {
        int r = UnityEngine.Random.Range(0, 10000) % startingPoints.Count;
        Element element = new Element();
        element.position = startingPoints[r];
        element.color = Workspace.Instance.GetColor();
        element.type = ElementsType.Liquid;
        element.state = ElementsState.Fraction;
        element.numerator = numerator;
        element.denominator = denominator;
        element.partitions = partitions;
        element.partNumerator = partNumerator;
        element.partDenominator = partDenominator;
        Workspace.Instance.SendMessage("CreateLiquidMeasures", element);
    }

    #endregion

    //OK
    void ShowSuggestion(string text)
    {
        if (lowFeedbackTxt.Length == 0)
        {
#if !UNITY_IPHONE && !UNITY_ANDROID
            suggestionTxt = text;
            hintText.color = Green1;
            localizationUtils.AddTranslationText(hintText, text);
#endif
        }
    }

    //OK
    void ShowHint(string text)
    {
        //Debug.Log("ShowHint " + text);
        if (lowFeedbackTxt.Length == 0 && suggestionTxt.Length == 0)
        {
#if !UNITY_IPHONE && !UNITY_ANDROID
            hintText.color = Green2;
            localizationUtils.AddTranslationText(hintText, text);
#endif
        }
    }

    //OK
    void ShowLowFeedback(string text)
    {
        ShowSuggestion("");
        lowFeedbackTxt = text;
        hintText.color = Orange;
        hintText.text = lowFeedbackTxt;

    }

    void InitializeFeedbackPopup()
    { }

    
    public void RemoveFeedbackPopup()
    {
        Workspace.Instance.SendMessage("ResetAction");
        Workspace.Instance.SendMessage("EnableInput");
    }

    void ShowHighFeedback(string text)
    {
        Workspace.Instance.SendMessage("DisableInput");
        ShowLowFeedback("");
        GameObject popupFeedback = GameObject.Instantiate(popupmsg) as GameObject;
        popupFeedback.transform.parent = ui.transform;
        PushPopup(popupFeedback);
        popupFeedback.GetComponent<UIPopup>().setText(text);
        popupFeedback.GetComponent<UIPopup>().SetColor(Orange);
        popupFeedback.GetComponent<UIPopup>().isHighFeedback = true;

    }

    void ShowFeedbackPopup(string text)
    {
        Workspace.Instance.SendMessage("DisableInput");
        GameObject popupFeedback = GameObject.Instantiate(popupmsg) as GameObject;
        popupFeedback.GetComponent<UIPopup>().isHighFeedback = false;
        popupFeedback.transform.SetParent(ui.transform);
        PushPopup(popupFeedback);
        localizationUtils.AddTranslationText(popupFeedback.GetComponent<UIPopup>().text, text);
        popupFeedback.GetComponent<UIPopup>().SetColor(Green1);
    }

    bool IsOverHUDElement(GameObject element)
    {

        Vector2 elementPosition = new Vector2(element.transform.position.x, element.transform.position.y);
        Vector2 elementSize = new Vector2(element.GetComponent<RectTransform>().sizeDelta.x, element.GetComponent<RectTransform>().sizeDelta.y);
        Vector3 mousePosition = Input.mousePosition;
        bool heigthCheck = mousePosition.y > Screen.height - elementPosition.y - elementSize.y * 0.5f && mousePosition.y < Screen.height - elementPosition.y + elementSize.y * 0.5f;
        bool lateralCheck = mousePosition.x > elementPosition.x - elementSize.x * 0.5f && mousePosition.x < elementPosition.x + elementSize.x * 0.5f;
        return lateralCheck && heigthCheck;
    }



    //OK
    public void CheckFractionOperation()
    {
        float fraction1 = 0.0f,
              fraction2 = 0.0f,
              result = 0.0f;

        float num = 0, den = 0;
        bool fraction1Ready = false,
             fraction2Ready = false,
             resultReady = false;
        Fraction currFraction;

        if (lastOperation == FractionsOperations.FIND)
        {
            if (null == fractionOperation2)
                return;
            currFraction = fractionOperation2[0];
            num = currFraction.numerator;
            den = currFraction.denominator;
            if (den != 0.0f)
            {
                fraction1 = (float)num / (float)den;
                fraction1Ready = true;
            }

            fraction2 = 0.0f;
            fraction2Ready = true;

            currFraction = fractionOperation2[1];
            num = currFraction.numerator;
            den = currFraction.denominator;
            if (den != 0)
            {
                result = (float)num / (float)den;
                resultReady = true;
            }
        }
        else
        {
            if (null == fractionOperation3)
                return;
            currFraction = fractionOperation3[0];
            num = currFraction.numerator;
            den = currFraction.denominator;
            if (den != 0.0f)
            {
                fraction1 = (float)num / (float)den;
                fraction1Ready = true;
            }

            currFraction = fractionOperation3[1];
            num = currFraction.numerator;
            den = currFraction.denominator;
            if (den != 0.0f)
            {
                fraction2 = (float)num / (float)den;
                fraction2Ready = true;
            }

            currFraction = fractionOperation3[2];
            num = currFraction.numerator;
            den = currFraction.denominator;
            if (den != 0)
            {
                result = (float)num / (float)den;
                resultReady = true;
            }

        }

        float totFraction = fraction1 + fraction2;
        if (lastOperation == FractionsOperations.SUBTRACTION)
            totFraction = fraction1 - fraction2;

        if (fraction1Ready && fraction2Ready && resultReady)
        {
            string strResult = "=";
            bool correct = Mathf.Abs(totFraction - result) < 0.000001f;
            if (correct)
            {
                strResult = "=";
            }
            else if (totFraction > result)
            {
                strResult = ">";
            }
            else
            {
                strResult = "<";
            }
            topBottom.GetComponent<TopBottom>().setResult(strResult);

            switch (lastOperation)
            {
                case (FractionsOperations.ADDITION):
                    ExternalEventsManager.Instance.SendMessageToSupport("OperationResult", "Sum", strResult);
                    break;
                case (FractionsOperations.SUBTRACTION):
                    ExternalEventsManager.Instance.SendMessageToSupport("OperationResult", "Substraction", strResult);
                    break;
                case (FractionsOperations.FIND):
                    ExternalEventsManager.Instance.SendMessageToSupport("OperationResult", "Equivalence", strResult);
                    break;
            }
        }
    }
    #endregion

    #region HUD Functions

    //OK
    void EnableHUD()
    {
        inputEnabled = true;
        //if (isOperationShown)
        //    ShowOperationMenu(lastOperation);
        EnableBarOperations();
        EnableBarTools();
        EnableZoom();
        if (home.activeSelf)
            home.GetComponent<UIButton>().EnableBtn(false);
        if (!isStudent)
            EnableBarTeacher();
        taskDescription.gameObject.GetComponent<UIButton>().EnableBtn(false);
        ShowSuggestion("");
        Workspace.Instance.SendMessage("EnableInput");
    }

    //OK
    void DisableHUD()
    {
        inputEnabled = false;
        DisableBarOperations();
        DisableBarTools();
        DisableZoom();
        if(home.activeSelf)
            home.GetComponent<UIButton>().DisableBtn(false);
        if (!isStudent)
            DisableBarTeacher();
        taskDescription.gameObject.GetComponent<UIButton>().DisableBtn(false);
        Cursor.SetCursor(MouseIconNo, Vector2.zero, CursorMode.Auto);
    }

    //NOT IMPLEMENTED YET
    /*  #region Multilanguage
      #region Multilanguage Members

      protected MovieClip mcLangFlags = null;
      protected MovieClip mcLanguageSelectorModule = null;
      protected MovieClip mcFlagMenuAnim = null;
      protected MovieClip mcFlagMenu = null;
      protected MovieClip mcFlagPopup = null;
      protected MovieClip[] mcLang = new MovieClip[13];

      public LangSelectorType langSelType = LangSelectorType.mono;
      public bool disableInputForLang = false;
      public bool firstTimeInStartPage = true;
      public int languageEventsCounter = 0;

      public enum LangSelectorType
      {
          eng = 0,
          mono,
          menu,
          popup
      }

      #endregion

      #region Multilanguage Functions
      void InitMultilinguageInterface()
      {
          bool validLanguage = !Localizations.Instance.CheckValidLanguage(Localizations.Instance.mcLanguage).Equals("en");

          switch (langSelType)
          {
              case LangSelectorType.mono:
                  break;
              case LangSelectorType.menu:
                  break;
              case LangSelectorType.popup:
                  break;
              case LangSelectorType.eng:
                  break;
          }
      }

      void MultilanguageExec()
      {
          #region Multilanguage Exec
          if (Input.GetMouseButtonDown(0) && langSelType == LangSelectorType.menu && !NotLangSelected() && !mcLangFlags.getChildByName<MovieClip>("mcFlagOver").visible)
          {
              mcFlagMenuAnim.gotoAndStop("end");
              mcFlagMenuAnim.visible = false;
          }
          #endregion
      }

      bool NotLangSelected()
      {
          bool selected = false;

          for (int i = 0; i < mcLang.Length; i++)
          {
              if (mcLang[i].getChildByName<MovieClip>("mcFlagOver").visible)
                  selected = true;
          }
          return selected;
      }
      #endregion
      #endregion*/

    #region Top Functions

    bool isMouseOut = false;

    public void OnTopEnter(int index)
    {
        GameObject elementOnFocus = Workspace.Instance.ElementOnFocus;
        if (elementSelected && Workspace.Instance.ElementOnFocus.GetComponent<RootElement>().hasDragged)
        {
            indexFractionArea = index;
            if (elementOnFocus.GetComponent<RootElement>().partDenominator > 0 && elementOnFocus.GetComponent<RootElement>().mode != InteractionMode.Freeze && elementOnFocus.GetComponent<RootElement>().state != ElementsState.Cut)
                elementOnFocus.SendMessage("ScaleDown");
        }
        ShowHint("{hint_drag_eq}");
    }

    public void OnTopLeave()
    {
        Debug.Log("onTopLeave");
        indexFractionArea = -1;
        GameObject elementOnFocus = Workspace.Instance.ElementOnFocus;
        if (elementSelected && Workspace.Instance.ElementOnFocus.GetComponent<RootElement>().hasDragged)
        {
            if (elementOnFocus.GetComponent<RootElement>().partDenominator > 0 && elementOnFocus.GetComponent<RootElement>().mode != InteractionMode.Freeze && elementOnFocus.GetComponent<RootElement>().state != ElementsState.Cut)
                elementOnFocus.SendMessage("ScaleUp");
        }
        ShowHint("");
    }
    #endregion

    #region Trash Bin Functions

    //OK
    public void OnTrashEnter()
    {
        if (null != Workspace.Instance.ElementOnFocus && Workspace.Instance.ElementOnFocus.GetComponent<RootElement>().mode == InteractionMode.LookAt)
            return;
        isOnTrash = true;
        if (elementSelected && Workspace.Instance.ElementOnFocus.GetComponent<RootElement>().hasDragged)
        {
            trash.GetComponent<RectTransform>().localScale = new Vector3(1.2f, 1.2f, 1.2f);
            Workspace.Instance.ElementOnFocus.SendMessage("ScaleDown");
        }
        ShowHint("{hint_trash}");
    }

    //OK
    public void OnTrashLeave()
    {
        if (null != Workspace.Instance.ElementOnFocus && Workspace.Instance.ElementOnFocus.GetComponent<RootElement>().mode == InteractionMode.LookAt)
            return;
        isOnTrash = false;

        if (elementSelected && Workspace.Instance.ElementOnFocus.GetComponent<RootElement>().hasDragged)
        {
            trash.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
            Workspace.Instance.ElementOnFocus.SendMessage("ScaleUp");
        }
        ShowHint("");
    }
    #endregion

    #region Menu Tools Functions

    //OK
    void JoinCuts()
    {
        actionMenu.SetActive(false);      
        Workspace.Instance.SendMessage("EnableInput");
        ShowSuggestion("{hint_join}");
        Workspace.Instance.SendMessage("SetCurrenAction", ActionType.Join);
        Workspace.Instance.SendMessage("StartCurrentAction");
        isMenuActive = false;
    }

    //OK
    void TakingAwayCuts()
    {
        actionMenu.SetActive(false);
        Workspace.Instance.SendMessage("EnableInput");
        ShowSuggestion("{hint_taking_away}");
        Workspace.Instance.SendMessage("SetCurrenAction", ActionType.TakingAway);
        Workspace.Instance.SendMessage("StartCurrentAction");

        isMenuActive = false;
    }

    //CANCELLARE
    void ChangeSize()
    {
        //mcMenuTools.visible = false;
        OnHideContextMenu();
      //  contextMenu.SetActive(false);
      //  Workspace.Instance.SendMessage("EnableInput");
        Workspace.Instance.ElementOnFocus.SendMessage("SetMode", InteractionMode.Scaling);
       
    }

    void ChangeColor()
    {
        //contextMenu.SetActive(false);
        OnHideContextMenu();
        colorMenu.SetActive(true);
        Workspace.Instance.SendMessage("DisableInput");
        DisableHUD();
    }

    //OK
    void Copy()
    {
        Workspace.Instance.interfaces.SendMessage("ShowSuggestion", "");
        OnHideContextMenu();
        // contextMenu.SetActive(false);
        // Workspace.Instance.SendMessage("EnableInput");
        Workspace.Instance.ElementOnFocus.SendMessage("Copy");
    }

    //OK
    void CutFraction()
    {
        OnHideContextMenu();
        //contextMenu.SetActive(false);
        Workspace.Instance.ElementOnFocus.SendMessage("CutFraction");
       // Workspace.Instance.SendMessage("EnableInput");
    }

    //OK
    void FindEquivalence()
    {
        OnHideContextMenu();
        //contextMenu.SetActive(false);
        //Workspace.Instance.ElementOnFocus.SendMessage("SetMode", InteractionMode.Partitioning);
        Workspace.Instance.ElementOnFocus.SendMessage("CreateEquivalence");
       // Workspace.Instance.SendMessage("EnableInput");
    }

    void FindParent() 
    {
        Workspace.Instance.ElementOnFocus.SendMessage("FindParent");
    }

    void Highlight()
    {
        OnHideContextMenu();
       // Debug.Log("INTERFACE HIGHTLUIGHT");
       // contextMenu.SetActive(false);
        Workspace.Instance.ElementOnFocus.SendMessage("SetHighlight");
       // Workspace.Instance.SendMessage("EnableInput");
    }

    public bool isMouseOnZoom() 
    {
        return !zoomController.GetComponent<Zoom>().IsMouseOut;
    }

    #endregion

    #region Tool Bar Functions

    #region CloseSideBar
    //OK
    public void ArrowButtonCallback(bool highlight)
    {
        StartCoroutine(MoveBarTools(highlight));
    }
    //OK
    IEnumerator MoveBarTools(bool highlight)
    {
        isMenuOut = !isMenuOut;

        Button btnCloseBarTool = barTool[barTool.Count - 1];
        if (isMenuOut)
            btnCloseBarTool.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0.0f, 0.0f, 180.0f);
        else
            btnCloseBarTool.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

        Vector3 start = Vector3.zero;
        Vector3 end = Vector3.zero;
        start.x = sideBar.GetComponent<RectTransform>().localPosition.x;
        if (!isMenuOut)
        {
            ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "HideRepresentaionToolbar");
            end.x = start.x + 105.0f;
            //end.x = 848.0f;
            //end.x = 174.0f;
            // Debug.Log("!isMenuOut end.x " + end.x);
        }
        else
        {
            ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "ShowRepresentaionToolbar");
            end.x = start.x - 105.0f;
            //end.x = 748.0f;
            // end.x = 74.0f;
            //Debug.Log("isMenuOut end.x " + end.x);

        }

        //Tweener.CreateNewTween(start, end, 0.5f, "easeOutCubic", 0.0f, TweenBarToolsS, TweenBarToolsU, TweenBarToolsC);*/
        StartSidebarTween(start.x, end.x, 0.2f);
        yield return new WaitForSeconds(0.3f);
        if (highlight)
            InterfaceHighlightByName("mcRepresentationBar");
    }

    float timer = -1.0f;
    float duration = 0.5f;
    float startX = 0.0f;
    float endX = 0.0f;
    //OK
    protected void StartSidebarTween(float sx, float ex, float time)
    {
        timer = Time.time;
        duration = time;
        startX = sx;
        endX = ex;
    }
    //OK
    protected void UpdateSidebarTween()
    {
        if (timer > 0)
        {
            float cTime = Time.time;
            float elapsed = cTime - timer;
            float factor = elapsed / duration;
            if (factor >= 1.0f)
            {
                factor = 1.0f;
                timer = -1.0f;
            }

            float spaceDiff = endX - startX;
            sideBar.GetComponent<RectTransform>().localPosition = new Vector3(startX + spaceDiff * factor, sideBar.GetComponent<RectTransform>().localPosition.y, sideBar.GetComponent<RectTransform>().localPosition.z);
        }
    }

    #endregion

    //OK
    void OpenToolSubmenu(string name)
    {
        foreach (GameObject sm in subMenu)
        {
            if (sm.name == name)
            {
                //Debug.Log("sm " + sm.name );
                sm.SetActive(true);
                break;
            }

        }
    }
    //OK
    void CloseToolSubmenu()
    {
        foreach (GameObject sub in subMenu)
        {
            sub.SetActive(false);
        }
    }

    //OK
    void EnableBarTools()
    {
        foreach (Button bt in barTool)
        {
            if (bt.name != "btnClose")
                bt.GetComponent<UIButton>().EnableBtn(false);
            else
                bt.interactable = true;
        }
    }

    //OK
    void DisableBarTools()
    {
        foreach (Button bt in barTool)
        {
            if (bt.name != "btnClose")
                bt.GetComponent<UIButton>().DisableBtn(false);
            else
                bt.interactable = false;
        }
    }

    void EnableZoom()
    {

        foreach (GameObject st in zoomElements)
        {
            if (null != st.GetComponent<UIButton>())
                st.GetComponent<UIButton>().EnableBtn(false);
            else
                st.GetComponent<Selectable>().interactable = true;
        }
        zoomController.GetComponent<Zoom>().CheckBound();
    }

    void DisableZoom()
    {
        foreach (GameObject st in zoomElements)
        {
            if (null != st.GetComponent<UIButton>())
                st.GetComponent<UIButton>().DisableBtn(false);
            else
                st.GetComponent<Selectable>().interactable = false;
        }
    }
    #endregion

    #region Operations Functions
    //OK

    public void SetOperation(FractionsOperations currOperation)
    {
        bool checkDifferentOperation = (lastOperation != currOperation);
        if (checkDifferentOperation)
        {
            lastOperation = currOperation;
        }
    }

    public void InitializeOperationMenu()
    {
        // Debug.Log("InitializeOperationMenu");
        // fractionOperation.Clear();
        fractionOperation3 = new List<Fraction>(3);
        fractionOperation3.Add(new Fraction());
        fractionOperation3.Add(new Fraction());
        fractionOperation3.Add(new Fraction());
        // fractionOperation.Clear();
        fractionOperation2 = new List<Fraction>(2);
        fractionOperation2.Add(new Fraction());
        fractionOperation2.Add(new Fraction());

    }
    public void ShowOperationMenu(FractionsOperations currOperation)
    {
        /* bool checkDifferentOperation = (lastOperation != currOperation);
         if (checkDifferentOperation)
         {
             if (currOperation != FractionsOperations.FIND)
             {
                // fractionOperation.Clear();
                 fractionOperation = new List<Fraction>(3);
                 fractionOperation.Add(new Fraction());
                 fractionOperation.Add(new Fraction());
                 fractionOperation.Add(new Fraction());
             }
             else
             {
                // fractionOperation.Clear();
                 fractionOperation = new List<Fraction>(2);
                 fractionOperation.Add(new Fraction());
                 fractionOperation.Add(new Fraction());
             }
             CheckFractionOperation();
            // topBottom.GetComponent<TopBottom>().OpenTopBottom((int)currOperation);
         }*/
        /*  else
          {
             // fractionOperation.Clear();
              //topBottom.GetComponent<TopBottom>().CloseTopBottom();
          }*/
        lastOperation = currOperation;
    }
    //OK
    void HideOperationMenu()
    {
        topBottom.GetComponent<TopBottom>().CloseTopBottom();
    }

    //OK
    void SendDropMessage(int index, RootElement root)
    {
        if (index != -1)
        {
            string str1 = "Sum";
            switch (lastOperation)
            {
                case (FractionsOperations.ADDITION):
                    str1 = "Sum";
                    break;
                case (FractionsOperations.SUBTRACTION):
                    str1 = "Sub";
                    break;
                case (FractionsOperations.FIND):
                    str1 = "Eq";
                    break;
            }

            string str2 = "Op1";
            switch (index)
            {
                case 0:
                    str2 = "Op1";
                    break;
                case 1:
                    str2 = "Op2";
                    /*if (lastOperation == FractionsOperations.FIND)
                        str2 = "Op1";*/
                    break;
                case 2:
                    str2 = "Res";
                   /* if (lastOperation == FractionsOperations.FIND)
                        str2 = "Op2";*/
                    break;
            }
            ExternalEventsManager.Instance.SendMessageToSupport("FractionPlaced", str1 + str2, root.name, root.partNumerator + "/" + root.partDenominator);
        }
    }

    //OK
    void SetFractionValue(int index, float numerator, float denominator)
    {
        if (index != -1)
        {
            Fraction fr = new Fraction();
            fr.numerator = numerator;
            fr.denominator = denominator;
            if (lastOperation == FractionsOperations.FIND)
                fractionOperation2[index] = fr;
            else
                fractionOperation3[index] = fr;
            topBottom.GetComponent<TopBottom>().setFractionValue(numerator, denominator, index);
            CheckFractionOperation();
        }
    }

    //OK
    void EnableBarOperations()
    {
        foreach (Button bt in barOperations)
        {
            bt.GetComponent<UIButton>().EnableBtn(false);
            if (bt.gameObject.name == "operationsSet")
                bt.GetComponent<ButtonsSetManager>().lockInteraction = false;
            // bt.interactable = true;
        }
    }

    //OK
    void DisableBarOperations()
    {
        foreach (Button bt in barOperations)
        {
            bt.GetComponent<UIButton>().DisableBtn(false);
            if (bt.gameObject.name == "operationsSet")
                bt.GetComponent<ButtonsSetManager>().lockInteraction = true;
            // bt.interactable = false;
        }
    }

    //OK
    void DisableBarTeacher()
    {
        foreach (Button bt in barTeacher)
        {
            bt.GetComponent<UIButton>().DisableBtn(false);
        }
    }
    //OK
    void EnableBarTeacher()
    {
        foreach (Button bt in barTeacher)
        {
            bt.GetComponent<UIButton>().EnableBtn(false);
        }
    }
    #endregion

    #region Buttons Callbacks


    public void OnBtnsPress(Button button)
    {
        if (!button.interactable)
            return;
        Cursor.SetCursor(MouseIconNo, Vector2.zero, CursorMode.Auto);
        switch (button.name)
        {
            case "btnB5":
            case "btnB1":
            case "btnB2":
            case "btnB3":
            case "btnHeart":
            case "btnMoon":
            case "btnStar":
            case "btnB4":
            case "btnHRect":
            case "btnVRect":
            case "btChangeFraction":
            case "btChangeSize":
            case "ChangeCol":
            case "Copy":
            case "btPopupBack":
            case "btCloseHighFeedback":
            case "Add":
            case "Subtract":
            case "btCompare":
            case "btnClose":
            case "btClosePopupColors":
            case "btnRed":
            case "btnOrange":
            case "btnBlue":
            case "btnPurple":
            case "TaskDescriptor":
            case "ToolEditor":
            case "SaveNew":
            case "SaveAs":
            case "Home":
            case "Highlight":
            case "HighlightEqui":
                //Debug.Log("OnBtnsPress btSets");
                isPressingButton = true;
                if (null != Workspace.Instance.ElementOnFocus && Workspace.Instance.ElementOnFocus.GetComponent<RootElement>().mode != InteractionMode.Freeze)
                    Workspace.Instance.ElementOnFocus.GetComponent<RootElement>().mode = InteractionMode.Moving;
                break;
            case "btnPlus":
            case "btnMinus":
            case "btnSearch":
            case "btnLab":
                isPressingButton = true;
                isPressingOperation = true;
                if (null != Workspace.Instance.ElementOnFocus  && Workspace.Instance.ElementOnFocus.GetComponent<RootElement>().mode != InteractionMode.Freeze)
                    Workspace.Instance.ElementOnFocus.GetComponent<RootElement>().mode = InteractionMode.Moving;
                Workspace.Instance.CheckOverlapActionMenu();
                break;
            case "ShowPartition":
                CheckNotificationWarning();
                isPressingButton = true;
                break;
            case "LookAt":
                isPressingButton = true;
                break;
            case "FindParent":
            case "FindEquivalence":
                isPressingButton = true;
                break;
        }
    }

    public void OnBtnsPress(string button)
    {
        switch (button)
        {
            case "btLines":
            case "btNumbers":
            case "btPies":
            case "btSets":
            case "btHearts":
            case "btMoons":
            case "btStars":
            case "btContainers":
            case "btHRect":
            case "btVRect":
            case "btChangeFraction":
            case "btChangeSize":
            case "btChangeColor":
            case "btCopy":
            case "btPopupBack":
            case "btCloseHighFeedback":
            case "btJoin":
            case "btTakeAway":
            case "btCompare":
            case "mcBgSideArrow":
            case "btClosePopupColors":
            case "btColorShape1":
            case "btColorShape2":
            case "btColorShape3":
            case "btColorShape4":
            case "btTaskDescriptor":
            case "btToolEditor":
            case "btSave":
            case "btSaveAs":
            case "btnHome":
            case "oper2":
            case "oper3":
                //Debug.Log("OnBtnsPress btSets");
                isPressingButton = true;
                if (null != Workspace.Instance.ElementOnFocus && Workspace.Instance.ElementOnFocus.GetComponent<RootElement>().mode != InteractionMode.Freeze)
                    Workspace.Instance.ElementOnFocus.GetComponent<RootElement>().mode = InteractionMode.Moving;
                break;
            case "mcFractionAdd":
            case "mcFractionSub":
            case "mcFractionSearch":
            case "btnLab":
                isPressingButton = true;
                isPressingOperation = true;
                if (null != Workspace.Instance.ElementOnFocus && Workspace.Instance.ElementOnFocus.GetComponent<RootElement>().mode != InteractionMode.Freeze)
                    Workspace.Instance.ElementOnFocus.GetComponent<RootElement>().mode = InteractionMode.Moving;
                Workspace.Instance.CheckOverlapActionMenu();
                break;
            case "btPartition":
                CheckNotificationWarning();
                isPressingButton = true;
                break;
            case "btLookAt":
                isPressingButton = true;
                break;
        }
    }

    public void OnBtnsClick(string button)
    {
        if (!isPressingButton)
            return;
        CloseToolSubmenu();
        OnHideContextMenu();
        switch (button)
        {
            case "btLines":
                ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "NumberedLines");
                CreateNumberedLine();
                ShowSuggestion("{hint_denominator_first}");
                DisableHUD();
                break;
            case "btNumbers":
                break;
            case "btSets":
                // Debug.Log("OnBtnsClick btSets");
                OpenToolSubmenu("BgSub_set");
                break;
            case "btPies":
                OpenToolSubmenu("BgSub_rect");
                break;
            case "btContainers":
                ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "LiquidMeasures");
                CreateLiquidMeasures();
                ShowSuggestion("{hint_denominator_first}");
                DisableHUD();
                break;
            case "btCircles":
                break;
            case "btHRect":
                ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "Shape/HRects");
                CreateHRect();
                ShowSuggestion("{hint_denominator_first}");
                DisableHUD();
                break;
            case "btVRect":
                //Debug.Log("btVRect");
                ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "Shape/VRects");
                CreateVRect();
                ShowSuggestion("{hint_denominator_first}");
                DisableHUD();
                break;
            case "btHearts":
                ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "Set/Hearts");
                Workspace.Instance.lastShape = SetElement.Shape.Heart;
                // CreateSet();
                CreateHeartSet();
                ShowSuggestion("{hint_denominator_first}");
                DisableHUD();
                break;
            case "btStars":
                Workspace.Instance.lastShape = SetElement.Shape.Star;
                ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "Set/Stars");
                //CreateSet();
                CreateStarSet();
                ShowSuggestion("{hint_denominator_first}");
                DisableHUD();
                break;
            case "btMoons":
                Workspace.Instance.lastShape = SetElement.Shape.Moon;
                ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "Set/Moons");
                //CreateSet();
                CreateMoonSet();
                ShowSuggestion("{hint_denominator_first}");
                DisableHUD();
                break;
            case "btnLab":
                if (null != Workspace.Instance.ElementOnFocus && Workspace.Instance.ElementOnFocus.GetComponent<RootElement>().mode != InteractionMode.Freeze)
                    Workspace.Instance.ElementOnFocus.GetComponentInChildren<RootElement>().mode = InteractionMode.Moving;
                // ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "Lab");
                Debug.Log("brnlab");
                break;
            case "oper2":
                if (null != Workspace.Instance.ElementOnFocus && Workspace.Instance.ElementOnFocus.GetComponent<RootElement>().mode != InteractionMode.Freeze)
                    Workspace.Instance.ElementOnFocus.GetComponentInChildren<RootElement>().mode = InteractionMode.Moving;
                // ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "2OperandTab");
                break;
            case "oper3":
                if (null != Workspace.Instance.ElementOnFocus && Workspace.Instance.ElementOnFocus.GetComponent<RootElement>().mode != InteractionMode.Freeze)
                    Workspace.Instance.ElementOnFocus.GetComponentInChildren<RootElement>().mode = InteractionMode.Moving;
                // ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "3OperandTab");
                break;

            /*  case "mcFractionSub":
                  if (null != Workspace.Instance.ElementOnFocus && Workspace.Instance.ElementOnFocus.GetComponent<RootElement>().mode != InteractionMode.Freeze)
                      Workspace.Instance.ElementOnFocus.GetComponentInChildren<RootElement>().mode = InteractionMode.Moving;
                  //ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "Substraction");
                  ShowOperationMenu(FractionsOperations.SUBTRACTION);
                 //placeholders[1].SetActive(!placeholders[1].activeSelf);
                 // placeholders[0].SetActive(false);
                 // placeholders[2].SetActive(false);
                  break;
              case "mcFractionSearch":
                  if (null != Workspace.Instance.ElementOnFocus && Workspace.Instance.ElementOnFocus.GetComponent<RootElement>().mode != InteractionMode.Freeze)
                      Workspace.Instance.ElementOnFocus.GetComponentInChildren<RootElement>().mode = InteractionMode.Moving;
                 // ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "Equivalence");
                  ShowOperationMenu(FractionsOperations.FIND);
                 // placeholders[2].SetActive(!placeholders[2].activeSelf);
                 // placeholders[0].SetActive(false);
                 // placeholders[1].SetActive(false);
                  break;*/
            case "btChangeColor":
                ChangeColor();
                ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "Tool/ChangeColour");
                CheckNotificationWarning();
                break;
            case "btCopy":
                ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "Tool/Copy");
                Copy();
                CheckNotificationWarning();
                break;
            case "btPartition":
                ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "Tool/ShowHidePartition");
                FindEquivalence();
                CheckNotificationWarning();
                break;
            case "btFindEquivalence":
                ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "Tool/FindEquivalence");
                FindEquivalence();
                CheckNotificationWarning();
                break;
            case "btFindParent":
                ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "Tool/FindParent");
                FindParent();
                CheckNotificationWarning();
                break;
            case "highlight":
                ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "Tool/Highlight");
                Highlight();
                CheckNotificationWarning();
                break;
            case "btPopupBack":
                RemovePopup();
                break;
            /*case "btCloseHighFeedback":
                ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "CloseFeedbackPopup");
                RemoveFeedbackPopup();
                break;*/
            case "btJoin":
                ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "Action/Sum");
                //MenuActionsCallback(button);
                JoinCuts();
                break;
            case "btTakeAway":
                ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "Action/Subtraction");
                TakingAwayCuts();
                break;
            /*  case "btCompare":
                  ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "Action/Compare");
                  //MenuActionsCallback(button);
                  break;*/
            case "mcBgSideArrow":
                ArrowButtonCallback(false);
                CloseToolSubmenu();
                break;
            case "btClosePopupColors":
                colorMenu.SetActive(false);
                Workspace.Instance.SendMessage("EnableInput");
                EnableHUD();
                break;
            case "btColorShape1":
                colorMenu.SetActive(false);
                ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "Tool/ChangeColour");
                Workspace.Instance.ElementOnFocus.SendMessage("ChangeColor", Workspace.Instance.colorList[0]);
                Workspace.Instance.SendMessage("EnableInput");
                EnableHUD();
                break;
            case "btColorShape2":
                colorMenu.SetActive(false);
                ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "Tool/ChangeColour");
                Workspace.Instance.ElementOnFocus.SendMessage("ChangeColor", Workspace.Instance.colorList[1]);
                Workspace.Instance.SendMessage("EnableInput");
                EnableHUD();
                break;
            case "btColorShape3":
                colorMenu.SetActive(false);
                ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "Tool/ChangeColour");
                Workspace.Instance.ElementOnFocus.SendMessage("ChangeColor", Workspace.Instance.colorList[2]);
                Workspace.Instance.SendMessage("EnableInput");
                EnableHUD();
                break;
            case "btColorShape4":
                colorMenu.SetActive(false);
                ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "Tool/ChangeColour");
                Workspace.Instance.ElementOnFocus.SendMessage("ChangeColor", Workspace.Instance.colorList[3]);
                Workspace.Instance.SendMessage("EnableInput");
                EnableHUD();
                break;
            /*case "btLookAt":
                //startLookAt();
                break;*/
            case "TaskDescriptor":
                ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "OpenTaskDescription");
                break;
            case "home":
                ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "Home");
                break;
            /*  case "plusZoom":
                  ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "ZoomIn");
                  break;
              case "minusZoom":
                  ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "ZoomOut");
                  break;
              case "actionOk":
                  ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "ActionOK");
                  break;*/
        }
        Cursor.SetCursor(MouseIconNo, Vector2.zero, CursorMode.Auto);
        isPressingButton = false;
        isPressingOperation = false;
    }

 /*   void startLookAt()
    {
        if (!inputEnabled || Workspace.Instance.ElementOnFocus.GetComponent<RootElement>().mode == InteractionMode.LookAt || Workspace.Instance.ElementOnFocus.GetComponent<RootElement>().mode == InteractionMode.Initializing || Workspace.Instance.ElementOnFocus.GetComponent<RootElement>().mode == InteractionMode.Wait || Workspace.Instance.ElementOnFocus.GetComponent<RootElement>().mode == InteractionMode.Freeze || Workspace.Instance.ElementOnFocus.GetComponent<RootElement>().state == ElementsState.Cut)
            return;
        Camera.main.GetComponent<LookAtFraction>().StartLookAt();
    }*/

    //OK
    public void OnBtnEnter(string button)
    {
        Cursor.SetCursor(MouseIconOv, Vector2.zero, CursorMode.Auto);
        switch (button)
        {
            case ("btPies"):
                ShowHint("{hint_shape_menu}");
                break;
            case ("btLines"):
                ShowHint("{hint_create_lines}");
                break;
            case ("btContainers"):
                ShowHint("{hint_create_liquid}");
                break;
            case ("btVRect"):
                ShowHint("{hint_create_horizontal}");
                break;
            case ("btHRect"):
                ShowHint("{hint_create_vertical}");
                break;
            case ("btSets"):
                ShowHint("{hint_set_menu}");
                break;
            case "btHearts":
                ShowHint("{hint_create_heart}");
                break;
            case "btMoons":
                ShowHint("{hint_create_moon}");
                break;
            case "btStars":
                ShowHint("{hint_create_star}");
                break;
            case "btnLab":
                if (topBottom.GetComponent<TopBottom>().isClose)
                    ShowHint("{hint_open_lab}");
                else
                    ShowHint("{hint_close_lab}");
                break;
            /*case ("mcFractionSearch"):
                ShowHint("{hint_find_eq}");
                break;
            case ("mcFractionAdd"):
                ShowHint("{hint_find_sum}");
                break;
            case ("mcFractionSub"):
                ShowHint("{hint_find_sub}");
                break;*/
            case ("btChangeFraction"):
                ShowHint("{hint_change_fraction}");
                break;
            case ("btShow"):
                ShowHint("{hint_showhide}");
                break;
            case ("btChangeSize"):
                ShowHint("{hint_scale_fraction}");
                break;
            case "btChangeColor":
                ShowHint("{hint_change_colour}");
                break;
            case "btPartition":
                ShowHint("{hint_partition}");
                CheckNotificationWarning();
                break;
            case "btFindEquivalence":
                ShowHint("{hint_find_equivalence}");
                CheckNotificationWarning();
                break;
            case "btFindParent":
                ShowHint("{hint_find_parent}");
                CheckNotificationWarning();
                break;
            case "btCut":
                ShowHint("{hint_cut}");
                break;
            case "btCopy":
                ShowHint("{hint_copy}");
                break;
            case "highlight":
                ShowHint("{hint_highlight}");
                break;
            case "btJoin":
                ShowHint("{hint_join}");
                break;
            case "btTakeAway":
                ShowHint("{hint_taking_away}");
                break;
            case "btCompare":
                ShowHint("{hint_compare}");
                break;
            case "btFind":
                ShowHint("{hint_find_parent}");
                break;
            case "mcBgSideArrow":
                if (isMenuOut)
                    ShowHint("{hint_hide_side_menu}");
                else
                    ShowHint("{hint_show_side_menu}");
                break;
        }
    }

    //OK
    public void OnBtnExit()
    {
        Cursor.SetCursor(MouseIconNo, Vector2.zero, CursorMode.Auto);
        ShowSuggestion("");
        ShowHint("");
        isPressingButton = false;
    }


    #endregion //Ingame Buttons Callback




    //DA RIVEDERE
    #region Popup Functions
    public enum PopupType
    {
        PAUSE = 0
    }

    void InitializePopup()
    {
        /* mcPopup = mcIngame.getChildByName<MovieClip>("mcPopup");
         mcPopup.visible = false;
         mcPopup.x = Screen.width * 0.5f;
         mcPopup.y = Screen.height * 0.5f;

         btPopupBack = mcPopup.getChildByName<MovieClip>("btPopupBack");*/
        //   SetupButton(btPopupBack);      
    }

    public void ShowPopup(PopupType type)
    {
        LinkedListNode<string> node = popupsList.Find(type.ToString());
        if (node != null)
        {
            popupsList.Remove(node);
            popupsList.AddLast(node);

            this.SetupNextPopup(type.ToString(), false);
        }
        else
        {
            this.SetupNextPopup(type.ToString(), true);
        }

        Workspace.Instance.SendMessage("DisableInput");
    }

    protected void SetupNextPopup(string name, bool firstTime)
    {
        if (firstTime)
        {
            popupsList.AddLast(name);
        }

        /* mcPopup.visible = true;

         TextField popupTextField = mcPopup.getChildByName<TextField>("tfPopupLabel");
         PopupType type = (PopupType)System.Enum.Parse(typeof(PopupType), name);
         switch (type)
         {
             case PopupType.PAUSE:
                 popupTextField.text = Localizations.Instance.getString("popup_pause_text");
                 break;
         }*/
    }

    public string RemovePopup()
    {
        if (0 == popupsList.Count)
            return null;

        string popupName = popupsList.Last.Value;
        popupsList.RemoveLast();
        // mcPopup.visible = false;

        if (popupsList.Count > 0)
            this.SetupNextPopup(popupsList.Last.Value, false);
        else
            Workspace.Instance.SendMessage("EnableInput");

        return popupName;
    }

    #endregion

    #endregion

    #region Messages


    //OK
    void LookAtDisabled()
    {
        CloseToolSubmenu();
        colorMenu.SetActive(false);
        HideOperationMenu();
        if (isMenuOut)
            ArrowButtonCallback(false);
        DisableHUD();
    }

    //OK
    void LookAtEnabled()
    {
        if (!isMenuOut)
            ArrowButtonCallback(false);
        EnableHUD();
    }

    //OK
    void OnElementClicked()
    {
        elementSelected = true;
    }


    //OK
    void OnElementReleased(GameObject element)
    {
        // Debug.Log("ON ELEMENT RELEASED  y " + Input.mousePosition.y + " x " + Input.mousePosition.x);
        RootElement selElement = element.GetComponentInChildren<RootElement>();
        selElement.SendMessage("SetType", element.GetComponent<RootElement>().type);
        if (isOnTrash)
        {
            if (null != Workspace.Instance.ElementOnFocus && Workspace.Instance.ElementOnFocus.GetComponent<RootElement>().mode == InteractionMode.LookAt)
                return;
            //if (selElement.state == ElementsState.Improper || selElement.denominator != 0 && selElement.mode == InteractionMode.Moving)
            {
                if (selElement.denominator == 0)
                    EnableHUD();

                Workspace.Instance.SendMessage("DeleteElement", element);

                string eventName = "FractionTrashed";
                switch (selElement.state)
                {
                    case (ElementsState.Cut):
                        eventName = "CutTrashed";
                        break;
                    case (ElementsState.Result):
                    case (ElementsState.Fraction):
                        eventName = "FractionTrashed";
                        break;
                    case (ElementsState.Improper):
                        eventName = "ImproperTrashed";
                        break;
                    case (ElementsState.Equivalence):
                        eventName = "EquivalenceTrashed";
                        break;
                }

                string typeString = "HRects";
                switch (selElement.type)
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
                    case (ElementsType.Set):
                        typeString = "Set";
                        break;
                    case (ElementsType.HeartSet):
                        typeString = "HeartSets";
                        break;
                    case (ElementsType.MoonSet):
                        typeString = "MoonSets";
                        break;
                    case (ElementsType.StarSet):
                        typeString = "StarSets";
                        break;
                }

                string value = selElement.partNumerator + "/" + selElement.partDenominator;
                ExternalEventsManager.Instance.SendMessageToSupport(eventName, typeString, selElement.name, value);
            }
            trash.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
        elementSelected = false;
        if (indexFractionArea != -1)
        {
            if (selElement.denominator != 0 && (selElement.mode == InteractionMode.Moving || selElement.mode == InteractionMode.Changing) && (selElement.state == ElementsState.Fraction || selElement.state == ElementsState.Result || selElement.state == ElementsState.Equivalence))
            {
                // Debug.Log("elementRelease indexFraciontArea " + indexFractionArea + "numerator" + selElement.partNumerator);
                SetFractionValue(indexFractionArea, selElement.partNumerator, selElement.partDenominator);
                SendDropMessage(indexFractionArea, selElement);
            }
            element.SendMessage("ResetLastPosition");
        }
        if (isMouseOut)
        {
            //Debug.Log("OnElementRelease isMouseOut");
            element.SendMessage("ResetLastPosition");
        }

        if (!topBottom.GetComponent<TopBottom>().isClose)
        {
            if (Input.mousePosition.y > Screen.height - (100.0f * Screen.height / 600.0f) && (Input.mousePosition.x > 200.0f * Screen.width / 800.0f && Input.mousePosition.x < 580.0f * Screen.width / 800.0f))
            {
                element.SendMessage("ResetLastPosition");
            }
            if (indexFractionArea != -1)
            {
                element.SendMessage("ResetLastPosition");
                //element.transform.position = Vector3.zero;
            }
        }
        else
        {
            if (Input.mousePosition.y > Screen.height - (100.0f * Screen.height / 600.0f) && (Input.mousePosition.x > 250.0f * Screen.width / 800.0f && Input.mousePosition.x < 550.0f * Screen.width / 800.0f))
            {
                element.SendMessage("ResetLastPosition");
            }

        }

        dropOverMc = null;
    }

    //OK
    void CheckNotificationWarning()
    {
        if (elementOnFocus.GetComponent<RootElement>().state == ElementsState.Result)
        {
            if (null != notification)
                notification.SetActive(false);
        }
        else
        {
            bool partitionEnabled = elementOnFocus.GetComponent<RootElement>().CheckPartition();
            if (partitionEnabled)
            {
                if (null != notification)
                    notification.SetActive(false);
            }
            else
            {
                if (null != notification)
                    notification.SetActive(true);
            }
        }
    }

    //OK
    void OnShowContextMenu(GameObject element)
    {
        isMenuActive = true;
        CloseToolSubmenu();
        GameObject menu;
        if (element.GetComponent<RootElement>().state == ElementsState.Equivalence)
            menu = contextMenuEquivalence; 
        else
            menu = contextMenu;

        menu.SetActive(true);
        Vector3 menuPos = Vector3.zero;
        GameObject back = null;
        for (int i = 0; i < menu.transform.childCount; i++)
        {
            if (menu.transform.GetChild(i).name == "Back")
            {
                menuPos = menu.transform.GetChild(i).GetComponent<RectTransform>().position;
                back = menu.transform.GetChild(i).gameObject;
            }
        }

         //Debug.Log("onshowcontext");

        if (null == back)
            return;
        elementOnFocus = element;     
        float lossy = canvas[0].GetComponent<RectTransform>().lossyScale.x;
        Vector3 halfScreen = canvas[0].GetComponent<RectTransform>().sizeDelta * 0.5f;
        Vector3 elementPos = RectTransformUtility.WorldToScreenPoint(mainCamera, element.transform.position);

        
        elementPos -= halfScreen;   //hack: canvas space has the origin in the middle of the Screen
        elementPos /= lossy;
        elementPos += halfScreen;

        float wGap = back.GetComponent<RectTransform>().sizeDelta.x * 0.5f;

        if (elementPos.x > halfScreen.x)
            menuPos.x = elementPos.x - wGap; //Input.mousePosition.x - wGap;//menuPos.x = elementPos.x / (Screen.width / 800.0f) - wGap;
        else
            menuPos.x = elementPos.x + wGap; //Input.mousePosition.x + wGap;// menuPos.x = elementPos.x / (Screen.width / 800.0f) + wGap;

        float hGap = back.GetComponent<RectTransform>().sizeDelta.y * 0.5f;

        if (elementPos.y > halfScreen.y)
            menuPos.y = elementPos.y - hGap; // Input.mousePosition.y - hGap;//menuPos.y = 600.0f - (elementPos.y / (Screen.height / 600.0f) - hGap);
        else
            menuPos.y = elementPos.y + hGap; //Input.mousePosition.y + hGap;//menuPos.y = 600.0f - (elementPos.y / (Screen.height / 600.0f) + hGap);

        back.GetComponent<RectTransform>().anchoredPosition = menuPos;

        menu.GetComponent<ContextMenuManager>().ChangeTextHighlight(element.GetComponent<RootElement>().isHighlighted);
       /* if (element.GetComponent<RootElement>().type == ElementsType.HeartSet || element.GetComponent<RootElement>().type == ElementsType.StarSet || element.GetComponent<RootElement>().type == ElementsType.MoonSet && element.GetComponent<RootElement>().state != ElementsState.Equivalence)
            menu.GetComponent<ContextMenuManager>().DisableFindEquivalence(true);
        else if( element.GetComponent<RootElement>().state != ElementsState.Equivalence)
            menu.GetComponent<ContextMenuManager>().DisableFindEquivalence(false);*/

            
        if (element.GetComponent<RootElement>().state == ElementsState.Equivalence)
            menu.GetComponent<ContextMenuManager>().DisableFindParent(element.GetComponent<RootElement>().parentEqRef == null);

        //menu.GetComponent<ContextMenuManager>().ChangeTextPartition(element.GetComponent<RootElement>().PartitionActive);

        CheckNotificationWarning();
        Workspace.Instance.SendMessage("DisableInput");

       
    }

    //OK
    void OnShowActionsMenu(GameObject element)
    {
        isMenuActive = true;
        elementOnFocus = element;
        //Vector3 elementPos = mainCamera.WorldToScreenPoint(element.transform.position);
        GameObject back = null;
        Vector3 menuPos = Vector3.zero;
        for (int i = 0; i < actionMenu.transform.childCount; i++)
        {
            if (actionMenu.transform.GetChild(i).name == "Back")
                back = actionMenu.transform.GetChild(i).gameObject;
        }
        float lossy = canvas[0].GetComponent<RectTransform>().lossyScale.x;
        Vector3 elementPos = RectTransformUtility.WorldToScreenPoint(mainCamera, element.transform.position);
        Vector3 halfScreen = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0.0f);
        elementPos -= halfScreen;   //hack: canvas space has the origin in the middle of the Screen
        elementPos /= lossy;
        elementPos += halfScreen;
        //Debug.Log(Input.mousePosition + "ele " + elementPos);
        float wGap = 0.0f; // mcMenuAction.width * 0.5f;
        if (elementPos.x > Screen.width * 0.5f)
            menuPos.x = elementPos.x - wGap;
        else
            menuPos.x = elementPos.x + wGap;
        //menupos.x = Mathf.Clamp(menupos.x, back.GetComponent<RectTransform>().sizeDelta.x * 0.5f, Screen.width - back.GetComponent<RectTransform>().sizeDelta.x * 0.5f);
        // menupos.x = Mathf.Clamp(menupos.x, -(Screen.width/2) + (back.GetComponent<RectTransform>().sizeDelta.x * 0.5f), (Screen.width/2) -( back.GetComponent<RectTransform>().sizeDelta.x * 0.5f));

        float hGap = back.GetComponent<RectTransform>().sizeDelta.y * 0.5f;
        if (elementPos.y > Screen.height * 0.5f)
            menuPos.y = elementPos.y - hGap;
        else
            menuPos.y = elementPos.y + hGap;
        // menupos.y = Mathf.Clamp(menupos.y, -240.0f, 240.0f);
        back.GetComponent<RectTransform>().position = menuPos;
        actionMenu.SetActive(true);
        Workspace.Instance.SendMessage("DisableInput");
    }

    //OK
    void OnHideContextMenu()
    {
        if (contextMenu.activeSelf)
        {
            contextMenu.SetActive(false);
            Workspace.Instance.SendMessage("EnableInput");
        }
        if (contextMenuEquivalence.activeSelf) 
        {
            contextMenuEquivalence.SetActive(false);
            Workspace.Instance.SendMessage("EnableInput");
        }
        //isMenuActive = false;
        Cursor.SetCursor(MouseIconNo, Vector2.zero, CursorMode.Auto);
    }

    //OK
    void OnHideActionsMenu()
    {   
        if (actionMenu.activeSelf)
        {
            //if (!IsOverHUDElement(mcMenuAction))
            //{
            actionMenu.SetActive(false);
            Workspace.Instance.SendMessage("EnableInput");
            // }
        }
        //isMenuActive = false;
        Cursor.SetCursor(MouseIconNo, Vector2.zero, CursorMode.Auto);
    }
    //OK
    void OnHideSubMenus()
    {
        foreach (GameObject sm in subMenu)
        {
            if (!IsOverHUDElement(sm))
            {
                sm.SetActive(false);
            }
        }
        Cursor.SetCursor(MouseIconNo, Vector2.zero, CursorMode.Auto);
    }

    //TODOUI
    void ShowActionPopup()
    {
        inputEnabled = false;
        DisableHUD();
        Workspace.Instance.SendMessage("DisableInput");
        elementSelected = false;
        // if (actionsPopupBG != null)
        //   actionsPopupBG.SetActive(true);
        PushPopup(actionsPopupBG);
    }

    //OK
    public void HideActionPopup()
    {
        ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "ActionOK");
        inputEnabled = true;
        elementSelected = false;
        PopPopup();
        Workspace.Instance.SendMessage("TerminateCurrentAction");
        Workspace.Instance.SendMessage("EnableInput");
        EnableHUD();
        Cursor.SetCursor(MouseIconNo, Vector2.zero, CursorMode.Auto);
    }
    #endregion

    #region Popups
    public GameObject FrontPopup
    {
        get
        {
            if (popupsStack.Count > 0)
                return popupsStack.Peek();
            else
                return null;
        }
    }

    public void AddPopup(GameObject popup)
    {
        popups.Add(popup);
    }

    public void RemovePopup(GameObject popup)
    {
        this.RemovePopupFromStack(popup);

        popups.Remove(popup);
    }

    public bool IsPopupInStack(GameObject popup)
    {
        return popupsStack.Contains(popup);
    }

    public bool IsPopupInStack(string name)
    {
        foreach (GameObject popup in popups)
        {
            if (popup.name.Equals(name))
                return this.IsPopupInStack(popup);
        }
        return false;
    }

    public void PushPopup(GameObject popup, bool deactivatePrevious = true)
    {
        if (popupsStack.Count > 0 && deactivatePrevious)
            popupsStack.Peek().gameObject.SetActive(false);

        popupsStack.Push(popup);

        /* if (popup.pausesGame)
             TimeManager.Instance.MasterSource.Pause();*/

        popup.gameObject.SetActive(true);
    }

    public void PushPopup(string name, bool deactivatePrevious = true)
    {
        Workspace.Instance.SendMessage("DisableInput");
        //inputEnabled = false;
        foreach (GameObject popup in popups)
        {
            if (popup.name.Equals(name))
            {
                this.PushPopup(popup, deactivatePrevious);
                break;
            }
        }
    }

    public void EnqueuePopup(GameObject popup)
    {
        if (0 == popupsStack.Count)
        {
            this.PushPopup(popup);
            return;
        }

        GameObject[] stack = popupsStack.ToArray();

        popupsStack.Clear();
        popupsStack.Push(popup);

        for (int i = stack.Length - 1; i >= 0; --i)
            popupsStack.Push(stack[i]);
    }

    public void EnqueuePopup(string name)
    {
        foreach (GameObject popup in popups)
        {
            if (popup.name.Equals(name))
            {
                this.EnqueuePopup(popup);
                break;
            }
        }
    }

    public void PopPopup()
    {

        if (popupsStack.Count > 0)
        {
            GameObject popup = popupsStack.Peek();
            if (popup.GetComponent<UIPopup>().isHighFeedback)
            {
                //ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "CloseFeedbackPopup");
                RemoveFeedbackPopup();
            }
            popup.gameObject.SetActive(false);
            popupsStack.Pop();
            if (popup.GetComponent<UIPopup>().destroyOnClose)
                Destroy(popup);

            /* if (popup.pausesGame)
                 TimeManager.Instance.MasterSource.Resume();
             popup.onRemoveFromStack.Invoke(popup);*/

            if (popupsStack.Count > 0)
                popupsStack.Peek().gameObject.SetActive(true);

            //return true;
        }
        Workspace.Instance.SendMessage("EnableInput");
        inputEnabled = true;
        //return false;
    }

    public bool RemovePopupFromStack(GameObject popup)
    {
        GameObject[] stack = popupsStack.ToArray();
        int index = Array.IndexOf<GameObject>(stack, popup);
        if (index >= 0)
        {
            popupsStack.Clear();
            for (int i = stack.Length - 1; i >= 0; --i)
            {
                if (i == index)
                    continue;

                popupsStack.Push(stack[i]);
            }

            popup.gameObject.SetActive(false);
            if (0 == index && popupsStack.Count > 0)
                popupsStack.Peek().gameObject.SetActive(true);

            return true;
        }
        return false;
    }

    public bool RemovePopupFromStack(string name)
    {
        foreach (GameObject popup in popups)
        {
            if (popup.name.Equals(name))
                return this.RemovePopupFromStack(popup);
        }
        return false;
    }

    public bool BringPopupToFront(GameObject popup)
    {
        GameObject[] stack = popupsStack.ToArray();
        int index = Array.IndexOf<GameObject>(stack, popup);
        if (index >= 0)
        {
            if (0 == index)
                return true;

            popupsStack.Peek().gameObject.SetActive(false);

            popupsStack.Clear();
            for (int i = stack.Length - 1; i >= 0; --i)
            {
                if (i == index)
                    continue;

                popupsStack.Push(stack[i]);
            }

            popupsStack.Push(popup);
            popup.gameObject.SetActive(true);

            return true;
        }
        return false;
    }

    public bool BringPopupToFront(string name)
    {
        foreach (GameObject popup in popups)
        {
            if (popup.name.Equals(name))
                return this.BringPopupToFront(popup);
        }
        return false;
    }

    #endregion

    #region Canvas

    public void GoToPage(string pagename)
    {
        foreach (Canvas cv in canvas)
        {
            if (cv.name != pagename)
                cv.gameObject.SetActive(false);
            else
            {
                cv.gameObject.SetActive(true);
                cv.BroadcastMessage("Initialize", SendMessageOptions.DontRequireReceiver);
            }
        }
        Cursor.SetCursor(MouseIconNo, Vector2.zero, CursorMode.Auto);
    }

    public void activeWindow(string window)
    {

    }

    #endregion

    #region Login

    public bool CheckLogin(string username, string password)
    {
        if (UserDataControl(username, password))
        {
            user = username;
            GoToPage("StartPage");
            return true;
        }
        else
            return false;
    }

    bool UserDataControl(string username, string password)
    {
        //CHEAT
        Debug.Log("is a debug build?" + Debug.isDebugBuild);

#if UNITY_EDITOR
        return true;
#endif
        if(Debug.isDebugBuild)
            return true;

        //chiamata esterna al controllo di username e password
        if (username.ToUpper() == "student01".ToUpper() && password == "12345" && isStudent)
            return true;
        else if (username.ToUpper() == "teacher01".ToUpper() && password == "12345" && !isStudent)
            return true;
        return false;

    }
    public void Logout()
    {
        PopPopup();
        user = "";
        isStudent = true;
        bookmark.SetActive(false);
        GoToPage("LoginPage");
    }

    public void Register()
    {
        PushPopup("PopupRegister", true);
        Cursor.SetCursor(MouseIconNo, Vector2.zero, CursorMode.Auto);
    }

    public bool CheckRegister(string username, string password)
    {
        if ((username.ToUpper() == "student01".ToUpper() && isStudent) || (username.ToUpper() == "teacher01".ToUpper() && !isStudent))
            return false;
        user = username;
        GoToPage("StartPage");
        PopPopup();
        return true;
    }

    public void NewWorkspace()
    {
        if (isStudent)
            taskDescription.gameObject.SetActive(false);
        else
            taskDescription.gameObject.SetActive(true);

        if (!isMenuOut)
        {
            barTool[barTool.Count - 1].GetComponent<RectTransform>().localRotation = Quaternion.Euler(0.0f, 0.0f, 180.0f);
            sideBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(338.0f, sideBar.GetComponent<RectTransform>().anchoredPosition.y);
            isMenuOut = true;
        }
        EnableHUD();
        overlayablePage.SetActive(true);
        GoToPage("HUDPage");
        Cursor.SetCursor(MouseIconNo, Vector2.zero, CursorMode.Auto);
        istaskDefined = false;
    }

    public void CleanWorkspace()
    {
        PopPopup();
        if (IsPopupInStack("TaskDescriptionWindowStudent"))
            PopPopup();
        if (null != Workspace.Instance.ElementOnFocus && Workspace.Instance.ElementOnFocus.GetComponent<RootElement>().mode == InteractionMode.LookAt)
        {
            Camera.main.GetComponent<LookAtFraction>().RapidResetLookAt();
        }
        foreach (GameObject popup in popups)
        {
            if (popup.name == "ToolCustumizationWindow")
            {
                popup.GetComponent<ToolCustomizationWindow>().CleanStateToggle();
            }
        }
        Camera.main.orthographicSize = 10.0f;
        Camera.main.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        Camera.main.transform.position = new Vector3(0.0f, 0.0f, -10.0f);
        highlight.transform.parent = null;
        Workspace.Instance.SendMessage("EnableInput");
        Workspace.Instance.CleanElements();
        HideOperationMenu();
        OnHideActionsMenu();
        OnHideContextMenu();
        OnHideSubMenus();
        EnableButtons();
        task.title = "";
        task.description = "";
        task.number = 0;
        task.taskState = TaskState.New;
        Cursor.SetCursor(MouseIconNo, Vector2.zero, CursorMode.Auto);
        istaskDefined = false;
        if (!topBottom.GetComponent<TopBottom>().isClose)
        {
            topBottom.GetComponent<TopBottom>().ChangeStatePanel();
            barOperations[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, 254.0f);
        }
        GoToPage("StartPage");
        Workspace.Instance.actualFactorScale = 1.0f;
    }

    public void taskRedirect(string filename) 
    {
        StartCoroutine(ChooseTask(filename));
    }

    public IEnumerator ChooseTask(string filename)
    {
       // Debug.Log("ChooseTask");
        //cheat!!!!
        yield return StartCoroutine(TaskManager.Instance.LoadJson("http://172.19.6.254/italk2learn/tip/Task01.tip"));
       // Debug.Log("ChooseTask after");

        if (TaskManager.Instance.isLoad)
            LoadWorkspace(TaskManager.Instance.task);
        else
            NewWorkspace();
    }

    public void LoadWorkspace(Task _task)
    {
        // Debug.Log("NewWorkspace");

        istaskDefined = true;
        PopPopup();
        task = _task;

        //fake task
        /*  Element element = new Element();

          element.position = Vector3.zero;
          element.color = Workspace.Instance.colorList[1];
          element.type = ElementsType.HRect;
          element.state = ElementsState.Fraction;
          element.numerator = 2;
          element.denominator = 3;
          element.partitions = 1;
          element.partNumerator = 2;
          element.partDenominator = 3;
          Workspace.Instance.SendMessage("CreateHRect", element);

          element.position = new Vector3(-3.0f, -4.0f, 0.0f);
          element.color = Workspace.Instance.colorList[0];
          element.type = ElementsType.Liquid;
          element.state = ElementsState.Fraction;
          element.numerator = 1;
          element.denominator = 3;
          element.partitions = 2;
          element.partNumerator = 2;
          element.partDenominator = 6;
          Workspace.Instance.SendMessage("CreateLiquidMeasures", element);

          //CreateHRect();
          */

        //todo da aggiungere chiamata per il caricamento del task
        foreach (Element el in TaskManager.Instance.elements)
        {
            Workspace.Instance.SendMessage("CreateFraction", el);
        }

        GoToPage("HUDPage");
        EnableHUD();

        if (!isMenuOut)
        {
            sideBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(338.0f, sideBar.GetComponent<RectTransform>().anchoredPosition.y);
            isMenuOut = true;
        }
        if (TaskManager.Instance.popupDescriptionIsActive)
        {
            taskDescription.GetComponent<UIButton>().DisableBtn(false);
            LoadTaskDescription();
        }
        else
        {
            taskDescription.gameObject.SetActive(TaskManager.Instance.taskDescriptionIsActive);
        }
       
        overlayablePage.SetActive(true);
       
        Cursor.SetCursor(MouseIconNo, Vector2.zero, CursorMode.Auto);
    }

    public void LoadTaskList()
    {
        PushPopup("PopupLoad", true);
        Cursor.SetCursor(MouseIconNo, Vector2.zero, CursorMode.Auto);
    }

    public void LoadTaskDescription()
    {
        foreach (GameObject popup in popups)
        {
            if (isStudent && popup.name == "TaskDescriptionWindowStudent")
            {
                PushPopup("TaskDescriptionWindowStudent", true);
                popup.GetComponent<PopupTaskDescriptor>().initialize(task.title, task.description, isStudent);
                popup.SetActive(true);
                inputEnabled = true;
                Workspace.Instance.inputEnabled = true;
                Workspace.Instance.SendMessage("EnableInput");
            }
            if (!isStudent && popup.name == "TaskDescriptionWindowTeacher")
            {
                PushPopup("TaskDescriptionWindowTeacher", true);
                popup.SetActive(true);
                if (istaskDefined)
                    popup.GetComponent<PopupTaskDescriptor>().initialize(task.title, task.description, isStudent);
                else
                    popup.GetComponent<PopupTaskDescriptor>().initialize("", "", isStudent);
                popup.SetActive(true);


            }
        }       
        taskDescription.gameObject.SetActive(true);
        taskDescription.GetComponent<UIButton>().DisableBtn(false);

    }

    public void CustomizationToolTeacher(UIButton bt)
    {
        bt.DisableBtn(false);
        PushPopup("ToolCustumizationWindow");
        Cursor.SetCursor(MouseIconNo, Vector2.zero, CursorMode.Auto);
    }

    public void SaveTask()
    {
        Debug.Log("SAVE TASK");
        ShowHint("Task saved");
    }

    public void SaveAsTask(UIButton btn)
    {
        btn.DisableBtn(false);
        foreach (GameObject popup in popups)
        {
            if (!isStudent && popup.name == "SaveAsWindow")
            {
                popup.SetActive(true);

            }
        }
        Cursor.SetCursor(MouseIconNo, Vector2.zero, CursorMode.Auto);
    }
    
    public void ChangeStateButton(string btn, bool isActive)
    {

        Button tmp;
        if (buttonDictionary.TryGetValue(btn, out tmp))
        {
            if (!isActive)
                tmp.GetComponent<UIButton>().DisableBtn(true);
            else
                tmp.GetComponent<UIButton>().EnableBtn(true);
        }

    }

    public void ChangeStateButton(int index, bool isActive)
    {
        //Debug.Log("change state button " + ((configurationName)index).ToString() + " " + isActive +" index " + index );
        if(index < initialConfigurationList.Count-1)
            TaskManager.Instance.initialConfiguration[((configurationName)index).ToString()] = isActive;
        if (!isActive)
        {
            initialConfigurationList[index].GetComponent<UIButton>().DisableBtn(true);
        }
        else
        {
            initialConfigurationList[index].GetComponent<UIButton>().EnableBtn(true);
        }
    }

    public void EnableButtons()
    {
        foreach (KeyValuePair<string, Button> entry in buttonDictionary)
        {
            if (entry.Key != "btnClose")
                entry.Value.GetComponent<UIButton>().EnableBtn(true);
        }
    }

    public void EnableButton(UIButton btn)
    {
        btn.EnableBtn(false);
    }

    public void ChangeTitleTask(InputField txt)
    {
        task.title = txt.text;
    }

    public void ChangeTaskDescription(string _title, string _desc)
    {
        task.title = _title;
        task.description = _desc;
        istaskDefined = true;
    }

    public void ConfirmOperation(string operation)
    {
        if (Workspace.Instance.isAction)
            return;
        PushPopup("PopupConfirm");
        foreach (GameObject popup in popups)
        {
            if (popup.name.Equals("PopupConfirm"))
            {
                popup.GetComponent<PopupConfirm>().Initialize(operation);
                break;
            }
        }
    }

    public void EnablePlaceHolder(int index)
    {
        placeholders[index].SetActive(true);
        placeholders[1 - index].SetActive(false);
    }

    public void DisablePlaceHolders()
    {
        placeholders[0].SetActive(false);
        placeholders[1].SetActive(false);
    }

    public void CheckMultiRaycastCollision()
    {
        PointerEventData pe = new PointerEventData(EventSystem.current);
        pe.position = Input.mousePosition;
        List<RaycastResult> hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pe, hits);

        string gos = "GOs: ";
        foreach (RaycastResult h in hits)
        {
            GameObject g = h.gameObject;
            gos += g + "  ";
        }
        //Debug.Log("hit : " + hits.Count + " " + gos);
    }

    #endregion

}
