using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using fractionslab.utils;
using fractionslab.behaviours;
using SBS.Math;
using pumpkin.display;
using pumpkin.events;
using pumpkin.text;
using System;

public class InterfaceBehaviour : MonoBehaviour
{
    public const string VER = "0.162";

    #region Protected Fields
    protected string SWFUtilsPath = "Flash/Utils.swf:";
    protected string SWFPath = "Flash/i_talk_2_learn.swf:";
    protected string SWFPathBottom = "Flash/i_talk_2_learn_bottom.swf:";
    protected Stage stage = null;
    protected Camera mainCamera;
    protected bool inputEnabled = true;
    protected LocalizationUtils localizationUtils;

    protected Dictionary<string, MovieClip> highlightMovieclips = new Dictionary<string, MovieClip>();
    
    protected List<SBSVector3> startingPoints = new List<SBSVector3>();

    protected MovieClip mcStartPage;
    protected MovieClip mcIngame;
    protected MovieClip mcIngameBottom;
    protected MovieClip mcMouseIcon;

    //Popup
    protected LinkedList<string> popupsList = new LinkedList<string>();
    protected MovieClip mcPopup;
    protected MovieClip btPopupBack;
    protected MovieClip mcFeedbackPopup;
    protected MovieClip btFeedbackClose;

    //HUD Menus
    //protected string[] menuTexts = { "{change_fraction}", "{change_size}", "{change_colour}", "{copy}", "{cut}", "{show_hide_symbol}", "{partition}", "{join}", "{taking_away}", "{compare}", "{find_parent}" };
    protected string[] menuTexts = { "{change_colour}", "{copy}", "{partition}", "{join}", "{taking_away}" }; 
    protected MovieClip mcHints;
    protected MovieClip mcTrash;
    protected MovieClip mcMenuAction;
    protected MovieClip mcBarTools;
    protected MovieClip mcMenuTools;
    protected MovieClip mcSingleFraction;
    protected MovieClip mcTop;
    protected MovieClip mcTopBottom;
    protected MovieClip mcTopBG;
    protected MovieClip mcOperation;
    protected MovieClip mcFeedback;
    protected MovieClip mcResultOperator;
    protected MovieClip mcActionsPopupButton;
    protected MovieClip mcColorPalette;
    protected List<ToolsBarData> toolsBarData;

    protected List<MenuToolsData> menuToolsData;
    protected List<MenuToolsData> menuActionsData;
    public delegate void MenuToolDelegate( );

    protected FractionsOperations lastOperation;
    [SerializeField]
    protected bool elementSelected = false;
    protected MovieClip dropOverMc;
    protected MovieClip lastDropOverMc;

    protected string suggestionTxt = "";
    protected string lowFeedbackTxt = "";

    protected bool isPressingButton = false;

    protected GameObject elementOnFocus = null;
    protected GameObject actionsPopupBG = null;

    protected bool isOperationShown = false;
	protected bool isSmall = false;

    protected float toolsMenuButtonsBgWidth = 0;
    protected float actionsMenuButtonsBgWidth = 0;

    protected bool isOnTrash = false;
    #endregion

    public static Color Green1 = new Color(0.2784f, 0.4510f, 0.1922f);
    public static Color Green2 = new Color(0.3216f, 0.5216f, 0.2235f);
    public static Color Orange = new Color(1.0f, 0.4235f, 0.1490f);

    #region Protected Struct
    protected struct ToolsBarData
    {
        public string toolName;
        public MovieClip button;
        public MovieClip subMenu;
    }

    protected struct MenuToolsData
    {
        public string toolName;
        public MovieClip button;
        public MenuToolDelegate callback;
    }

    protected enum FractionsOperations
    {
        ADDITION = 0,
        SUBTRACTION,
        FIND
    }
    #endregion
    
    public bool InputEnabled
    {
        get { return inputEnabled; }
    }

    #region Unity Callbacks
	/*void Awake()
	{
		MovieClipOverlayCameraBehaviour.overlayCameraName = "UICamera";
		stage = MovieClipOverlayCameraBehaviour.instance.stage;
		stage.scaleX = Screen.width / 800.0f;
		stage.scaleY = Screen.height / 600.0f;
	}*/

    void Start()
	{
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        startingPoints.Add(SBSVector3.zero);
        startingPoints.Add(SBSVector3.left);
        startingPoints.Add(SBSVector3.right);
        startingPoints.Add(SBSVector3.up);
        startingPoints.Add(SBSVector3.down);

        localizationUtils = GameObject.FindGameObjectWithTag("LevelRoot").GetComponent<LocalizationUtils>();

        Localizations.Instance.mcLanguage = "en";
        //if (Application.srcValue.Contains("language=de"))
        if (ExternalEventsManager.Instance.embeddingVariables.ContainsKey("language"))
            if (ExternalEventsManager.Instance.embeddingVariables["language"].Equals("de"))
                Localizations.Instance.mcLanguage = "de";

        Localizations.Instance.initialize();
        Localizations.Instance.listeners.Add(gameObject);
        Localizations.Instance.listeners.Add(localizationUtils.gameObject);

        actionsPopupBG = GameObject.FindGameObjectWithTag("PopupBG");
        actionsPopupBG.SetActive(false);

        InitializeStartPage();
    }

    MovieClip mcHighlight = null;
    void InterfaceHighlightByName(string name)
    {
        if (highlightMovieclips.ContainsKey(name))
        {
            if (name.Equals("mcRepresentationBar"))
            {
                if (isMenuOut)
                    InterfaceHighlight(highlightMovieclips[name]);
                else
                {
                    ArrowButtonCallback(true);
                }
            }
            else
                InterfaceHighlight(highlightMovieclips[name]);
        }
    }

    void InterfaceHighlight(MovieClip m)
    {
        DestroyImmediate(GameObject.FindGameObjectWithTag("Highlight"));
        DestroyInterfaceHighlight();

        mcHighlight = new MovieClip(SWFPath + "mcHighlightClass");
        mcIngame.getChildByName<MovieClip>("mcHighlightHolder").addChild(mcHighlight);
        
        MovieClip top, bottom, left, right, topLeft, topRight, bottomLeft, bottomRight;
        top = mcHighlight.getChildByName<MovieClip>("mcTop");
        bottom = mcHighlight.getChildByName<MovieClip>("mcBottom");
        left = mcHighlight.getChildByName<MovieClip>("mcLeft");
        right = mcHighlight.getChildByName<MovieClip>("mcRight");
        topLeft = mcHighlight.getChildByName<MovieClip>("mcTopLeft");
        topRight = mcHighlight.getChildByName<MovieClip>("mcTopRight");
        bottomLeft = mcHighlight.getChildByName<MovieClip>("mcBottomLeft");
        bottomRight = mcHighlight.getChildByName<MovieClip>("mcBottomRight");

        pumpkin.geom.Rectangle mRect = m.getBounds(m);
        Vector2 mCoords = m.localToGlobal(Vector2.zero);

        mcHighlight.x = mCoords.x;
        mcHighlight.y = mCoords.y;
        
        top.scaleX = 1.0f;
        top.scaleX = mRect.width * 0.5f;
        top.y = -mRect.height * 0.5f;
        bottom.scaleX = 1.0f;
        bottom.scaleX = mRect.width * 0.5f;
        bottom.y = mRect.height * 0.5f;
        left.scaleY = 1.0f;
        left.scaleY = mRect.height * 0.5f;
        left.x = -mRect.width * 0.5f;
        right.scaleY = 1.0f;
        right.scaleY = mRect.height * 0.5f;
        right.x = m.width * 0.5f;

        topLeft.y = topRight.y = top.y;
        topLeft.x = bottomLeft.x = left.x;
        bottomLeft.y = bottomRight.y = bottom.y;
        topRight.x = right.x;
        bottomRight.x = right.x + bottomRight.width;
        FixUVs(top);
        FixUVs(bottom);
        FixUVs(left);
        FixUVs(right);
        FixUVs(topLeft);
        FixUVs(topRight);
        FixUVs(bottomLeft);
        FixUVs(bottomRight);
    }

    void DestroyInterfaceHighlight()
    {
        if (mcHighlight != null)
        {
            mcIngame.getChildByName<MovieClip>("mcHighlightHolder").removeChild(mcHighlight);
            mcHighlight = null;
        }
    }

    void Update()
    {
        if (mcMouseIcon != null)
        {
#if !UNITY_IPHONE
            mcMouseIcon.x = Input.mousePosition.x;
            mcMouseIcon.y = Screen.height - Input.mousePosition.y;
#endif
        }

        if (Input.GetMouseButtonDown(0) && !isPressingButton)
		{
			if (mcStartPage != null && mcStartPage.visible)
			{
				return;
			}
			else
			{
				mcMouseIcon.gotoAndStop(1);
				OnHideContextMenu();
				OnHideActionsMenu();
				OnHideSubMenus();
                if (!actionsPopupBG.activeSelf)
                    Workspace.Instance.SendMessage("ResetAction");
			}
        }

        if (elementSelected && mcTopBottom.visible)
        {
            dropOverMc = null;
            if (null != mcOperation.getChildByName<MovieClip>("Fraction1Area"))
                CheckDragDrop(mcOperation.getChildByName<MovieClip>("Fraction1Area"));

            CheckDragDrop(mcOperation.getChildByName<MovieClip>("Fraction2Area"));
            CheckDragDrop(mcOperation.getChildByName<MovieClip>("ResultArea"));
        }

        UpdateSidebarTween();

#if UNITY_IPHONE
		bool isOver = false;
		if (elementSelected && mcTopBottom.visible)
		{
			MovieClip area1 = mcOperation.getChildByName<MovieClip>("Fraction1Area");
			MovieClip area2 = mcOperation.getChildByName<MovieClip>("Fraction2Area");
			MovieClip area3 = mcOperation.getChildByName<MovieClip>("ResultArea");

			if (area1 != null)
				isOver = isOver || IsOverHUDElement(area1);
			isOver = isOver || IsOverHUDElement(area2) || IsOverHUDElement(area3);
		}

		if (mcTrash != null)
		{
			if (IsOverHUDElement(mcTrash) || isOver)
			{
				if(!isSmall && elementSelected)
				{
					isSmall = true;
					Workspace.Instance.ElementOnFocus.SendMessage("ScaleDown");
				}
			}
			else
			{
				if(isSmall && elementSelected)
				{
					isSmall = false;
					Workspace.Instance.ElementOnFocus.SendMessage("ScaleUp");
				}
			}
		}
#endif

    }

    void OnGUI()
    {
        GUI.skin = Workspace.Instance.skin;
        string ver = VER;
#if UNITY_IPHONE
        ver += " mobile";
#endif
        GUI.Label(new Rect(10, Screen.height - 20.0f, 100, 20), "Ver " + ver);
    }
    #endregion

    #region Functionalities
    void CreateHRect()
    {
        CreateHRect(0, 0, 1, 0, 0);
    }

    void CreateHRect(int numerator, int denominator, int partitions, int partNumerator, int partDenominator)
    {
        int r = UnityEngine.Random.Range(0, 10000) % startingPoints.Count;
        Element element = new Element();
        element.position = startingPoints[r];
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

    void ShowSuggestion(string text)
    {
        if (lowFeedbackTxt.Length == 0)
        {
            suggestionTxt = text;
            mcHints.getChildByName<TextField>("tfLabel").colorTransform = Green1;
            localizationUtils.AddTranslationText(mcHints.getChildByName<TextField>("tfLabel"), text);
        }
    }

    void ShowHint(string text)
    {
        if (lowFeedbackTxt.Length == 0 && suggestionTxt.Length == 0)
        {
#if !UNITY_IPHONE
            mcHints.getChildByName<TextField>("tfLabel").colorTransform = Green2;
            localizationUtils.AddTranslationText(mcHints.getChildByName<TextField>("tfLabel"), text);
#endif
        }
    }

    void ShowLowFeedback(string text)
    {
        ShowSuggestion("");
        lowFeedbackTxt = text;
        mcHints.getChildByName<TextField>("tfLabel").colorTransform = Orange;
        mcHints.getChildByName<TextField>("tfLabel").text = lowFeedbackTxt;
    }

    void InitializeFeedbackPopup()
    {
        mcFeedbackPopup = mcIngame.getChildByName<MovieClip>("mcHighFeedback");

        //mcFeedbackPopup.scaleX = Screen.width / 800.0f;
        //mcFeedbackPopup.scaleY = Screen.height / 600.0f;

        mcFeedbackPopup.visible = false;
        btFeedbackClose = mcFeedbackPopup.getChildByName<MovieClip>("btCloseHighFeedback");
        localizationUtils.AddTranslationText(btFeedbackClose.getChildByName<TextField>("tfClose"), "{ok}");
        SetupButton(btFeedbackClose);
    }

    void RemoveFeedbackPopup()
    {
        Workspace.Instance.SendMessage("ResetAction");
        mcFeedbackPopup.visible = false;
        Workspace.Instance.SendMessage("EnableInput");
    }

    void ShowHighFeedback(string text)
    {
        ShowLowFeedback("");
        TextField popupTextField = mcFeedbackPopup.getChildByName<TextField>("tfPopupLabel");
        popupTextField.colorTransform = Orange;
        popupTextField.text = text;
        mcFeedbackPopup.visible = true;
        Workspace.Instance.SendMessage("DisableInput");
    }

    void ShowFeedbackPopup(string text)
    {
        Workspace.Instance.SendMessage("DisableInput");
        TextField popupTextField = mcFeedbackPopup.getChildByName<TextField>("tfPopupLabel");
        popupTextField.colorTransform = Green1;
        localizationUtils.AddTranslationText(popupTextField, text);
        mcFeedbackPopup.visible = true;
        Workspace.Instance.SendMessage("DisableInput");
    }


    bool IsOverHUDElement(MovieClip element)
    {
		Vector2 elementPosition = new Vector2(element.x, element.y) * (Screen.width / 800.0f);
		Vector2 elementSize = new Vector2(element.width, element.height) * (Screen.height / 600.0f);
        Vector3 mousePosition = Input.mousePosition;
		//bool heigthCheck = mousePosition.y < Screen.height - elementPosition.y && mousePosition.y > Screen.height - elementPosition.y - element.height;
        elementPosition = element.localToGlobal(Vector2.zero);
        //bool heigthCheck = mousePosition.y > Screen.height - elementPosition.y - element.height * 0.5f && mousePosition.y < Screen.height - elementPosition.y + element.height * 0.5f;
		//bool lateralCheck = mousePosition.x > elementPosition.x - element.width * 0.5f && mousePosition.x < elementPosition.x + element.width * 0.5f;
		bool heigthCheck = mousePosition.y > Screen.height - elementPosition.y - elementSize.y * 0.5f && mousePosition.y < Screen.height - elementPosition.y + elementSize.y * 0.5f;
		bool lateralCheck = mousePosition.x > elementPosition.x - elementSize.x * 0.5f && mousePosition.x < elementPosition.x + elementSize.x * 0.5f;

        return lateralCheck && heigthCheck;
    }

    bool DragDropCheck(MovieClip element)
    {
        return IsOverHUDElement(element);
    }

    void CheckDragDrop(MovieClip element)
    {
        if (DragDropCheck(element))
            dropOverMc = element;
    }

    void CheckFractionOperation( )
    {
        float fraction1 = 0.0f,
              fraction2 = 0.0f,
              result = 0.0f;

        int num = 0, den = 0;
        bool fraction1Ready = false,
             fraction2Ready = false,
             resultReady = false;

        string numString = "";
        string denString = "";

        MovieClip currFraction = mcOperation.getChildByName<MovieClip>("mcFirstOperator");
        if (currFraction != null)
        {
            numString = currFraction.getChildByName<TextField>("tfNumerator").text;
            denString = currFraction.getChildByName<TextField>("tfDenominator").text;

            if (denString.Length > 0)
                num = int.Parse(numString);
            else
                num = 0;
            if (denString.Length > 0)
                den = int.Parse(denString);
            else
                den = 0;

            if (den != 0)
            {
                fraction1 = (float)num / (float)den;
                fraction1Ready = true;
            }
        }
        else
            fraction1Ready = true;

        currFraction = mcOperation.getChildByName<MovieClip>("mcSecondOperator");
        numString = currFraction.getChildByName<TextField>("tfNumerator").text;
        denString = currFraction.getChildByName<TextField>("tfDenominator").text;

        if (denString.Length > 0)
            num = int.Parse(numString);
        else
            num = 0;
        if (denString.Length > 0)
            den = int.Parse(denString);
        else
            den = 0;

        if (den != 0)
        {
            fraction2 = (float)num / (float)den;
            fraction2Ready = true;
        }

        currFraction = mcOperation.getChildByName<MovieClip>("mcResult");
        numString = currFraction.getChildByName<TextField>("tfNumerator").text;
        denString = currFraction.getChildByName<TextField>("tfDenominator").text;

        if (denString.Length > 0)
            num = int.Parse(numString);
        else
            num = 0;
        if (denString.Length > 0)
            den = int.Parse(denString);
        else
            den = 0;

        if (den != 0)
        {
            result = (float)num / (float)den;
            resultReady = true;
        }

        float totFraction = fraction1 + fraction2;
        if (lastOperation == FractionsOperations.SUBTRACTION)
            totFraction = fraction1 - fraction2;

        if(fraction1Ready && fraction2Ready && resultReady)
        {
            string strResult = "=";
            bool correct = Mathf.Abs(totFraction - result) < 0.000001f;
            if (correct)
            {
                strResult = "=";
                mcResultOperator.gotoAndStop("equal");
            }
            else if (totFraction > result)
            {
                strResult = ">";
                mcResultOperator.gotoAndStop("major");
            }
            else
            {
                strResult = "<";
                mcResultOperator.gotoAndStop("minor");
            }

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
    void InitializeStartPage()
	{
		MovieClipOverlayCameraBehaviour.overlayCameraName = "UICamera";
		stage = MovieClipOverlayCameraBehaviour.instance.stage;

        bool showHud = false;
        if (ExternalEventsManager.Instance.embeddingVariables.ContainsKey("showStartPage"))
            if (ExternalEventsManager.Instance.embeddingVariables["showStartPage"].Equals("false"))
                showHud = true;

        if (showHud)
        {
            InitializeHUD();
        }
        else
        {
            mcStartPage = new MovieClip(SWFPath + "mcStartPageClass");

            localizationUtils.AddTranslationButton(mcStartPage.getChildByName<MovieClip>("btStart"), "{start}", LocalizedButton.Alignment.center, Color.white);

            mcMouseIcon = mcStartPage.getChildByName<MovieClip>("mcCursor");
            mcMouseIcon.scaleX = mcMouseIcon.scaleY = 0.5f;
            mcMouseIcon.gotoAndStop(1);
            mcMouseIcon.mouseEnabled = false;
#if !UNITY_IPHONE
            Screen.showCursor = false;
#else
            mcMouseIcon.visible = false;
#endif

            mcStartPage.scaleX = Screen.width / 800.0f;
            mcStartPage.scaleY = Screen.height / 600.0f;
            stage.addChild(mcStartPage);
        }

        //localization
        //Localizations.Instance.mcLanguage = "de";
        //Localizations.Instance.changeLanguage("de");
        #region Multilanguage Init
        InitMultilinguageInterface();
        #endregion
    }

    void DestroyStartPage()
    {
        stage.removeChild(mcStartPage);
        mcStartPage = null;
    }

    void InitializeHUD()
    {
        mcIngame = new MovieClip(SWFPath + "mcIngameClass");
		mcIngame.name = "mcIngame";

        mcIngameBottom = new MovieClip(SWFPathBottom + "mcIngameBottomClass");
        MovieClipOverlayCameraBehaviour.overlayCameraName = "UICamera";
		stage = MovieClipOverlayCameraBehaviour.instance.stage;
		stage.scaleX = Screen.width / 800.0f;
		stage.scaleY = Screen.height / 600.0f;

        mcMouseIcon = mcIngame.getChildByName<MovieClip>("mcCursor");
        mcMouseIcon.scaleX = mcMouseIcon.scaleY = 0.5f;
        mcMouseIcon.mouseEnabled = false;
        mcMouseIcon.gotoAndStop(1);
#if !UNITY_IPHONE
        Screen.showCursor = false;
#else
        mcMouseIcon.visible = false;
#endif

        InitializePopup();
        InitializeFeedbackPopup();

        mcHints = mcIngame.getChildByName<MovieClip>("mcHints");
        mcHints.getChildByName<TextField>("tfLabel").text = "";
        highlightMovieclips.Add("mcHints", mcHints);

        mcTrash = mcIngameBottom.getChildByName<MovieClip>("mcTrash");
        mcTrash.gotoAndStop("up");
        mcTrash.addEventListener(MouseEvent.MOUSE_ENTER, OnTrashEnter);
        mcTrash.addEventListener(MouseEvent.MOUSE_LEAVE, OnTrashLeave);
        highlightMovieclips.Add("mcTrash", mcTrash.getChildByName<MovieClip>("mcTrashBin"));

        mcActionsPopupButton = new MovieClip(SWFPath + "btStartClass");
        mcActionsPopupButton.name = "btOk";
        mcActionsPopupButton.gotoAndStop("up");
        localizationUtils.AddTranslationButton(mcActionsPopupButton, "{ok}", LocalizedButton.Alignment.center, Color.white);
		mcActionsPopupButton.x = 406.0f; //Screen.width * 0.5f;
		mcActionsPopupButton.y = 500.0f; //Screen.height - (600.0f - 490.0f);
        stage.addChild(mcActionsPopupButton);
        mcActionsPopupButton.visible = false;
        //highlightMovieclips.Add(mcActionsPopupButton.name, mcActionsPopupButton);

        //Menu Actions-----------
        mcMenuAction = mcIngame.getChildByName<MovieClip>("mcMenuAction");
        mcMenuAction.visible = false;
        InitializeMenuActions(mcMenuAction);
        //highlightMovieclips.Add(mcMenuAction.name, mcMenuAction);

        //Tools Side Bar-----------
        mcBarTools = mcIngame.getChildByName<MovieClip>("mcSideButton");
        InitializeBarTools(mcBarTools);
        highlightMovieclips.Add("mcRepresentationBar", mcBarTools.getChildByName<MovieClip>("mcSideBg"));

        //Menu Tools-----------
        mcMenuTools = mcIngame.getChildByName<MovieClip>("mcMenuTools");
        mcMenuTools.visible = false;
        InitializeMenuTools(mcMenuTools);
        //highlightMovieclips.Add(mcMenuTools.name, mcMenuTools);

        //Color Palette
        mcColorPalette = mcIngame.getChildByName<MovieClip>("mcColorPalette");
        mcColorPalette.visible = false;
        InitializeColorPalette(mcColorPalette);
        //highlightMovieclips.Add(mcColorPalette.name, mcColorPalette);

        //Fraction HUD-----------
        mcSingleFraction = mcIngame.getChildByName<MovieClip>("mcSingleFraction");
        mcSingleFraction.visible = false;
        mcIngame.getChildByName<MovieClip>("mcArrowUp").visible = false;
        mcIngame.getChildByName<MovieClip>("mcArrowDown").visible = false;

        //Operations HUD-----------
        InitializeBarOperations( );
        
        bool multiRes = false;
        if (multiRes)
        {
            //stage.addChild(mcTrash);
            mcHints.x = Screen.width - (800.0f - 500.0f);
            mcHints.y = Screen.height - (600.0f - 576.0f);
            stage.addChild(mcHints);

            mcTop.x = Screen.width * 0.5f;
            stage.addChild(mcTop);

            mcBarTools.x = Screen.width - (800.0f - 748.0f);
            mcBarTools.y = Screen.height * 0.5f;
            stage.addChild(mcBarTools);

            stage.addChild(mcMenuAction);
            stage.addChild(mcMenuTools);

            //stage.addChild(mcActionsPopupButton);
            stage.addChild(mcPopup);
            stage.addChild(mcFeedbackPopup);            
            
            stage.addChild(mcMouseIcon);
        }
        else
            stage.addChild(mcIngame);

        //localization
        //Localizations.Instance.changeLanguage("de");
        #region Multilanguage Init
        InitMultilinguageInterface();
        #endregion
    }

    void EnableHUD()
    {
        //if (isOperationShown)
        //    ShowOperationMenu(lastOperation);

        EnableBarOperations();
        EnableBarTools();
        ShowSuggestion("");
        Workspace.Instance.SendMessage("EnableInput");
    }

    void DisableHUD()
    {
        //isOperationShown = mcTopBottom.visible;
        //HideOperationMenu();
        DisableBarOperations();
        DisableBarTools();
    }
    #region Multilanguage
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
    #endregion

    #region Top Functions
    void OnTopEnter(CEvent evt)
    {
#if !UNITY_IPHONE
        if (elementSelected)
        {
            if (Workspace.Instance.ElementOnFocus.GetComponent<RootElement>().partDenominator > 0)
                Workspace.Instance.ElementOnFocus.SendMessage("ScaleDown");
        }
        ShowHint("{hint_drag_eq}");
#endif
    }

    void OnTopLeave(CEvent evt)
	{
#if !UNITY_IPHONE
        if (elementSelected)
        {
            if (Workspace.Instance.ElementOnFocus.GetComponent<RootElement>().partDenominator > 0)
                Workspace.Instance.ElementOnFocus.SendMessage("ScaleUp");
        }
		ShowHint("");
#endif
    }
    #endregion

    #region Trash Bin Functions
    void OnTrashEnter(CEvent evt)
	{
#if !UNITY_IPHONE
        isOnTrash = true;
        MovieClip mc = evt.currentTarget as MovieClip;
        if (elementSelected)
        {
            mc.gotoAndStop("ov");
            Workspace.Instance.ElementOnFocus.SendMessage("ScaleDown");
        }
        ShowHint("{hint_trash}");
#endif
    }

    void OnTrashLeave(CEvent evt)
    {
#if !UNITY_IPHONE
        isOnTrash = false;
        MovieClip mc = evt.currentTarget as MovieClip;
        if (elementSelected)
        {
            mc.gotoAndStop("up");
            Workspace.Instance.ElementOnFocus.SendMessage("ScaleUp");
        }
        ShowHint("");
#endif
    }
    #endregion

    #region Menu Tools Functions
    float TextboxWidth(MovieClip exampleButton, int startRange, int endRange)
    {
        float w = 0;
        for (int i = startRange; i <= endRange; i++)
        {
            string translatedStr = Localizations.Instance.replaceText(menuTexts[i]);
            TextField txtField = exampleButton.getChildByName<TextField>("tfLabel");
            txtField.text = translatedStr;
            float strLength = 0.0f;
            for (int k = 0; k < translatedStr.Length; k++)
                strLength = strLength + txtField.getGlyph(translatedStr[k]).charWidth + txtField.textFormat.letterSpacing;

            w = Mathf.Max(w, strLength);
        }
        return w;
    }

    protected List<GraphicsDrawOP> drawOps = new List<GraphicsDrawOP>();
    void FixHorizontalUVs(pumpkin.display.Sprite mc)
    {
        if (null == mc)
            return;

        foreach (GraphicsDrawOP drawOp in mc.graphics.drawOPs)
        {
            if (drawOps.IndexOf(drawOp) < 0)
            {
                Vector2 halfTexel = drawOp.material.mainTexture.texelSize * 0.5f;
                Rect r = drawOp.drawSrcRect;
                r.xMin += halfTexel.x;
                r.xMax -= halfTexel.x;
                //r.yMin += halfTexel.y;
                //r.yMax -= halfTexel.y;
                drawOp.drawSrcRect = r;
                drawOps.Add(drawOp);
            }
        }

        for (int i = 0; i < mc.numChildren; ++i)
            FixHorizontalUVs(mc.getChildAt<pumpkin.display.Sprite>(i));
    }

    void FixUVs(pumpkin.display.Sprite mc)
    {
        if (null == mc)
            return;

        foreach (GraphicsDrawOP drawOp in mc.graphics.drawOPs)
        {
            if (drawOps.IndexOf(drawOp) < 0)
            {
                Vector2 halfTexel = drawOp.material.mainTexture.texelSize * 0.5f;
                Rect r = drawOp.drawSrcRect;
                r.xMin += halfTexel.x;
                r.xMax -= halfTexel.x;
                r.yMin += halfTexel.y;
                r.yMax -= halfTexel.y;
                drawOp.drawSrcRect = r;
                drawOps.Add(drawOp);
            }
        }

        for (int i = 0; i < mc.numChildren; ++i)
            FixUVs(mc.getChildAt<pumpkin.display.Sprite>(i));
    }

    void SetMenuElementBgWidth(MovieClip bg, TextField txtField, float width)
    {
        SetBgWidth(bg, width);

        txtField.width = width;
    }

    void SetBgWidth(MovieClip bg, float width)
    {
        MovieClip centerMc = bg.getChildByName<MovieClip>("mcCenter");
        MovieClip rightMc = bg.getChildByName<MovieClip>("mcRight");
        MovieClip leftMc = bg.getChildByName<MovieClip>("mcLeft");
        float leftX = leftMc.x;
        float centerX = centerMc.x;
        float rightX = rightMc.x;

        width = Mathf.CeilToInt(width);

        centerMc.scaleX = 1.0f;
        centerMc.scaleX = width * 0.5f;

        leftMc.x = leftX;
        centerMc.x = leftMc.x - 1;
        rightMc.x = centerMc.x + centerMc.width - 1;

        FixHorizontalUVs(centerMc);
    }

    void InitializeColorPalette(MovieClip colorPalette)
    {
        SetupButton(colorPalette.getChildByName<MovieClip>("btClosePopupColors"));
        SetupButton(colorPalette.getChildByName<MovieClip>("btColorShape1"));
        SetupButton(colorPalette.getChildByName<MovieClip>("btColorShape2"));
        SetupButton(colorPalette.getChildByName<MovieClip>("btColorShape3"));
        SetupButton(colorPalette.getChildByName<MovieClip>("btColorShape4"));
    }

    void InitializeMenuActions(MovieClip menuActions)
    {
        menuActionsData = new List<MenuToolsData>();

        actionsMenuButtonsBgWidth = 0.0f;
        actionsMenuButtonsBgWidth = TextboxWidth(menuActions.getChildByName<MovieClip>("btJoin"), 3, 4);

        /*InitializeSingleMenuAction("Join", menuActions.getChildByName<MovieClip>("btJoin"), JoinCuts, menuTexts[7], actionsMenuButtonsBgWidth);
        InitializeSingleMenuAction("TakingAway", menuActions.getChildByName<MovieClip>("btTakeAway"), TakingAwayCuts, menuTexts[8], actionsMenuButtonsBgWidth);
        InitializeSingleMenuAction("Compare", menuActions.getChildByName<MovieClip>("btCompare"), CompareCuts, menuTexts[9], actionsMenuButtonsBgWidth);
        InitializeSingleMenuAction("FindParent", menuActions.getChildByName<MovieClip>("btFind"), FindParent, menuTexts[10], actionsMenuButtonsBgWidth);*/
        InitializeSingleMenuAction("Join", menuActions.getChildByName<MovieClip>("btJoin"), JoinCuts, menuTexts[3], actionsMenuButtonsBgWidth);
        InitializeSingleMenuAction("TakingAway", menuActions.getChildByName<MovieClip>("btTakeAway"), TakingAwayCuts, menuTexts[4], actionsMenuButtonsBgWidth);

        SetBgWidth(menuActions.getChildByName<MovieClip>("mcBg"), actionsMenuButtonsBgWidth);
    }

    void InitializeMenuTools(MovieClip menuTools)
    {
        menuToolsData = new List<MenuToolsData>();

        toolsMenuButtonsBgWidth = 0.0f;
        toolsMenuButtonsBgWidth = TextboxWidth(menuTools.getChildByName<MovieClip>("btChangeColor"), 0, 2);

        //InitializeSingleMenuTool("ChangeFraction", menuTools.getChildByName<MovieClip>("btChangeFraction"), ChangeFraction, menuTexts[0], toolsMenuButtonsBgWidth);
        //InitializeSingleMenuTool("ChangeSize", menuTools.getChildByName<MovieClip>("btChangeSize"), ChangeSize, menuTexts[1], toolsMenuButtonsBgWidth);
        InitializeSingleMenuTool("ChangeColor", menuTools.getChildByName<MovieClip>("btChangeColor"), ChangeColor, menuTexts[0], toolsMenuButtonsBgWidth);
        InitializeSingleMenuTool("Copy", menuTools.getChildByName<MovieClip>("btCopy"), Copy, menuTexts[1], toolsMenuButtonsBgWidth);
        //InitializeSingleMenuTool("Cut", menuTools.getChildByName<MovieClip>("btCut"), CutFraction, menuTexts[4], toolsMenuButtonsBgWidth);
        //InitializeSingleMenuTool("Show", menuTools.getChildByName<MovieClip>("btShow"), ShowHideSymbol, menuTexts[5], toolsMenuButtonsBgWidth);
        InitializeSingleMenuTool("Partition", menuTools.getChildByName<MovieClip>("btPartition"), Partition, menuTexts[2], toolsMenuButtonsBgWidth);

        SetBgWidth(menuTools.getChildByName<MovieClip>("mcBg"), toolsMenuButtonsBgWidth);
        //DisableButton(menuTools.getChildByName<MovieClip>("btChangeColor"), menuTexts[2]);
        //DisableButton(menuTools.getChildByName<MovieClip>("btChangeSize"), menuTexts[1]);
    }

    void InitializeSingleMenuAction(string name, MovieClip button, MenuToolDelegate buttonDelegate, string text, float width)
    {
        MenuToolsData data = new MenuToolsData();
        localizationUtils.AddTranslationText(button.getChildByName<TextField>("tfLabel"), text);
        button.getChildByName<TextField>("tfLabel").colorTransform = Green1;
        data.toolName = name;
        data.button = button;
        data.callback = buttonDelegate;
        menuActionsData.Add(data);
        SetupButton(button);
        SetMenuElementBgWidth(button.getChildByName<MovieClip>("mcBg").getChildByName<MovieClip>("mcBg"), button.getChildByName<TextField>("tfLabel"), width);
    }

    void InitializeSingleMenuTool(string name, MovieClip button, MenuToolDelegate buttonDelegate, string text, float width)
    {
        MenuToolsData data = new MenuToolsData();
        SetMenuElementBgWidth(button.getChildByName<MovieClip>("mcBg"), button.getChildByName<TextField>("tfLabel"), width);
        button.getChildByName<TextField>("tfLabel").colorTransform = Green1;
        localizationUtils.AddTranslationText(button.getChildByName<TextField>("tfLabel"), text);
        data.toolName = name;
        data.button = button;
        data.callback = buttonDelegate;
        menuToolsData.Add(data);
        SetupButton(button);
    }

    void MenuActionsCallback(string buttonName)
    {
        for (int i = 0; i < menuActionsData.Count; ++i)
        {
            if (menuActionsData[i].button.name == buttonName)
            {
                MenuToolDelegate buttonDelegate = menuActionsData[i].callback;
                if (buttonDelegate != null)
                    buttonDelegate();
                break;
            }
        }
    }

    void MenuToolsCallback(string buttonName)
	{
		Debug.Log("MenuToolsCallback " + buttonName);
        for (int i = 0; i < menuToolsData.Count; ++i)
        {
            if (menuToolsData[i].button.name == buttonName)
            {
                MenuToolDelegate buttonDelegate = menuToolsData[i].callback;
                if (buttonDelegate != null)
				{
					Debug.Log(buttonName + ": " + buttonDelegate.ToString());
                    buttonDelegate();
				}
                break;
            }
        }
    }

    void JoinCuts()
    {
        mcMenuAction.visible = false;
        Workspace.Instance.SendMessage("EnableInput");
        ShowSuggestion("{hint_join2}");
        //Workspace.Instance.ElementOnFocus.SendMessage("SetMode", InteractionMode.Wait);

        Workspace.Instance.SendMessage("SetCurrenAction", ActionType.Join);
        Workspace.Instance.SendMessage("StartCurrentAction");
        //Workspace.Instance.SendMessage("SetFirstOperationCut", Workspace.Instance.ElementOnFocus);
    }

    void TakingAwayCuts()
    {
        mcMenuAction.visible = false;
        Workspace.Instance.SendMessage("EnableInput");
        ShowSuggestion("{hint_taking_away2}");
        //Workspace.Instance.ElementOnFocus.SendMessage("SetMode", InteractionMode.Wait);

        Workspace.Instance.SendMessage("SetCurrenAction", ActionType.TakingAway);
        Workspace.Instance.SendMessage("StartCurrentAction");
        //Workspace.Instance.SendMessage("SetFirstOperationCut", Workspace.Instance.ElementOnFocus);
    }

    /*void CompareCuts()
    {
        mcMenuAction.visible = false;
        Workspace.Instance.SendMessage("EnableInput");
        ShowSuggestion("{hint_compare}");
        Workspace.Instance.ElementOnFocus.SendMessage("SetMode", InteractionMode.Wait);

        Workspace.Instance.SendMessage("SetCurrenAction", ActionType.Compare);
        Workspace.Instance.SendMessage("SetFirstOperationCut", Workspace.Instance.ElementOnFocus);
    }

    void FindParent()
    {
        mcMenuAction.visible = false;
        Workspace.Instance.ElementOnFocus.SendMessage("FindParent");
        Workspace.Instance.SendMessage("EnableInput");
    }*/

    void ChangeFraction()
    {
        mcMenuTools.visible = false;
        Workspace.Instance.ElementOnFocus.SendMessage("SetMode", InteractionMode.Changing);
        Workspace.Instance.SendMessage("EnableInput");
    }

    void ChangeSize()
    {
        mcMenuTools.visible = false;
        Workspace.Instance.ElementOnFocus.SendMessage("SetMode", InteractionMode.Scaling);
        Workspace.Instance.SendMessage("EnableInput");
    }

    void ChangeColor()
    {
        mcMenuTools.visible = false;
        mcColorPalette.visible = true;
        DisableHUD();
    }

    void Copy()
    {
        mcMenuTools.visible = false;
        Workspace.Instance.ElementOnFocus.SendMessage("Copy");
        Workspace.Instance.SendMessage("EnableInput");
    }

    void CutFraction()
    {
        mcMenuTools.visible = false;
        Workspace.Instance.ElementOnFocus.SendMessage("CutFraction");
        Workspace.Instance.SendMessage("EnableInput");
    }

    void Partition()
    {
        mcMenuTools.visible = false;
        //Workspace.Instance.ElementOnFocus.SendMessage("SetMode", InteractionMode.Partitioning);
        Workspace.Instance.ElementOnFocus.SendMessage("SetPartitioning");
        Workspace.Instance.SendMessage("EnableInput");
    }

    /*void ShowHideSymbol()
    {
        mcMenuTools.visible = false;
        Workspace.Instance.ElementOnFocus.SendMessage("ToggleSymbol");
        Workspace.Instance.SendMessage("EnableInput");
    }*/
    #endregion

    #region Tool Bar Functions
    void InitializeBarTools(MovieClip barTools)
    {
        SetButtonHider();
        toolsBarData = new List<ToolsBarData>();

        InitializeSingleTool("Numbers", barTools.getChildByName<MovieClip>("btNumbers"), null);
        InitializeSingleTool("Lines", barTools.getChildByName<MovieClip>("btLines"), null);
        InitializeSingleTool("Pies", barTools.getChildByName<MovieClip>("btPies"), barTools.getChildByName<MovieClip>("mcSubMenu1"));
        InitializeSingleTool("Sets", barTools.getChildByName<MovieClip>("btSets"), barTools.getChildByName<MovieClip>("mcSubMenu2"));
        InitializeSingleTool("Containers", barTools.getChildByName<MovieClip>("btContainers"), null);

        highlightMovieclips.Add("btSymbol", barTools.getChildByName<MovieClip>("btNumbers"));
        highlightMovieclips.Add("btLines", barTools.getChildByName<MovieClip>("btLines"));
        highlightMovieclips.Add("btRects", barTools.getChildByName<MovieClip>("btPies"));
        highlightMovieclips.Add("btSets", barTools.getChildByName<MovieClip>("btSets"));
        highlightMovieclips.Add("btLiquid", barTools.getChildByName<MovieClip>("btContainers"));

        //FLAG: disable representation buttons
        //DisableButton(barTools.getChildByName<MovieClip>("btLines"));
        //DisableButton(barTools.getChildByName<MovieClip>("btContainers"));
        DisableButton(barTools.getChildByName<MovieClip>("btNumbers"));
        DisableButton(barTools.getChildByName<MovieClip>("btSets"));
    }

    bool isMenuOut = true;
    MovieClip arrowBt = null;
    MovieClip mcArrowSide = null;

    void SetButtonHider()
    {
        mcArrowSide = mcBarTools.getChildByName<MovieClip>("mcSideBg").getChildByName<MovieClip>("mcArrowSide");

        if (isMenuOut)
            mcArrowSide.gotoAndStop(2);
        else
            mcArrowSide.gotoAndStop(1);

        arrowBt = mcArrowSide.getChildByName<MovieClip>("mcBgSideArrow");
        SetupButton(arrowBt);
    }

    void ArrowButtonCallback(bool highlight)
    {
        StartCoroutine(MoveBarTools(highlight));
    }

    IEnumerator MoveBarTools(bool highlight)
    {
        isMenuOut = !isMenuOut;

        if (isMenuOut)
            mcArrowSide.gotoAndStop(2);
        else
            mcArrowSide.gotoAndStop(1);

        Vector3 start = Vector3.zero;
        Vector3 end = Vector3.zero;
        start.x =  mcBarTools.x;

        if (!isMenuOut)
        {
            ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "HideRepresentaionToolbar");
            end.x = start.x + 100.0f;
        }
        else
        {
            ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "ShowRepresentaionToolbar");
            end.x = start.x - 100.0f;
        }

        //Tweener.CreateNewTween(start, end, 0.5f, "easeOutCubic", 0.0f, TweenBarToolsS, TweenBarToolsU, TweenBarToolsC);*/
        StartSidebarTween(mcBarTools.x, end.x, 0.2f);
        yield return new WaitForSeconds(0.3f);
        if (highlight)
            InterfaceHighlightByName("mcRepresentationBar");
    }

    float timer = -1.0f;
    float duration = 0.5f;
    float startX = 0.0f;
    float endX = 0.0f;

    protected void StartSidebarTween(float sx, float ex, float time)
    {
        timer = Time.time;
        duration = time;
        startX = sx;
        endX = ex;
    }

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
            mcBarTools.x = startX + spaceDiff * factor;
        }
    }

    /*protected void TweenBarToolsS(object v)
    {
    }
    protected void TweenBarToolsU(object v)
    {
        mcBarTools.x = ((Vector3)v).x;
    }
    protected void TweenBarToolsC(object v)
    {
        Tweener.StopAndDestroyAllTweens();
    }*/

    void InitializeSingleTool(string name, MovieClip button, MovieClip subMenu)
    {
        ToolsBarData data = new ToolsBarData();
        data.toolName = name;
        data.button = button;
        data.subMenu = subMenu;
        if (data.subMenu != null)
        {
            data.subMenu.visible = false;
            for (int y = 0; y < data.subMenu.numChildren; ++y)
            {
                if (data.subMenu.getChildAt(y) is MovieClip)
                {
                    SetupButton(data.subMenu.getChildAt(y) as MovieClip);

                    //Pelle: cheat
                    if ((data.subMenu.getChildAt(y) as MovieClip).name=="btCircles")
                        DisableButton(data.subMenu.getChildAt(y) as MovieClip);
                }
            }
        }
        toolsBarData.Add(data);
        SetupButton(button);

    }

    void OpenToolSubmenu(string name)
    {
        for (int i = 0; i < toolsBarData.Count; ++i)
        {
            if (toolsBarData[i].button.name == name)
            {
                MovieClip subMenu = toolsBarData[i].subMenu;
                if(subMenu!=null)
                    subMenu.visible = !subMenu.visible;
                break;
            }
        }
    }

    void CloseToolSubmenu()
    {
        for (int i = 0; i < toolsBarData.Count; i++)
        {
            MovieClip subMenu = toolsBarData[i].subMenu;
            if (subMenu != null)
                subMenu.visible = false;
        }
    }
    
    void EnableBarTools()
    {
        SetButtonHider();
        InitializeSingleTool("Pies", mcBarTools.getChildByName<MovieClip>("btPies"), mcBarTools.getChildByName<MovieClip>("mcSubMenu1"));
        //InitializeSingleTool("Sets", mcBarTools.getChildByName<MovieClip>("btSets"), mcBarTools.getChildByName<MovieClip>("mcSubMenu2"));
        InitializeSingleTool("Lines", mcBarTools.getChildByName<MovieClip>("btLines"), null);
        InitializeSingleTool("Containers", mcBarTools.getChildByName<MovieClip>("btContainers"), null);
    }

    void DisableBarTools()
    {
        DisableButton(mcBarTools.getChildByName<MovieClip>("btPies"));
        DisableButton(mcBarTools.getChildByName<MovieClip>("btNumbers"));
        DisableButton(mcBarTools.getChildByName<MovieClip>("btLines"));
        DisableButton(mcBarTools.getChildByName<MovieClip>("btSets"));
        DisableButton(mcBarTools.getChildByName<MovieClip>("btContainers"));
        DisableButton(arrowBt);
    }
    #endregion

    #region Operations Functions
    void InitializeBarOperations( )
    {
        mcTop = mcIngame.getChildByName<MovieClip>("mcTop");
        SetupButton(mcTop.getChildByName<MovieClip>("mcFractionAdd"));
        SetupButton(mcTop.getChildByName<MovieClip>("mcFractionSub"));
        SetupButton(mcTop.getChildByName<MovieClip>("mcFractionSearch"));

        highlightMovieclips.Add("mcOperationBar", mcTop);
        highlightMovieclips.Add("btAdd", mcTop.getChildByName<MovieClip>("mcFractionAdd"));
        highlightMovieclips.Add("btSub", mcTop.getChildByName<MovieClip>("mcFractionSub"));
        highlightMovieclips.Add("btEqual", mcTop.getChildByName<MovieClip>("mcFractionSearch"));

        //Pelle: cheat
        //DisableButton(mcTop.getChildByName<MovieClip>("mcFractionAdd"));
        //DisableButton(mcTop.getChildByName<MovieClip>("mcFractionSub"));

        mcTopBottom = mcIngameBottom.getChildByName<MovieClip>("mcTop");
        mcTopBottom.visible = false;
        //mcTopBottom.x = Screen.width * 0.5f;

        mcTopBG = mcTopBottom.getChildByName<MovieClip>("mcTopBg");
        mcTopBG.addEventListener(MouseEvent.MOUSE_ENTER, OnTopEnter);
        mcTopBG.addEventListener(MouseEvent.MOUSE_LEAVE, OnTopLeave);
        mcOperation = mcTopBottom.getChildByName<MovieClip>("mcOperation");

        GameObject bottomUI = new GameObject("UICameraBottom", typeof(Camera));
        bottomUI.camera.orthographic = true;
        bottomUI.camera.clearFlags = CameraClearFlags.SolidColor;
        bottomUI.camera.backgroundColor = Color.white;
        MovieClipOverlayCameraBehaviour bMc = bottomUI.AddComponent<MovieClipOverlayCameraBehaviour>();
        bottomUI.camera.CopyFrom(MovieClipOverlayCameraBehaviour.instance.camera);
        bottomUI.camera.pixelRect = new Rect(0, 0, Screen.width, Screen.height);
        bottomUI.camera.clearFlags = CameraClearFlags.SolidColor;
        bottomUI.camera.backgroundColor = new Color(0.898f, 0.866f, 0.77f);
        bottomUI.camera.depth = -2;
        bMc.postRender = true;

        bottomUI.GetComponent<MovieClipOverlayCameraBehaviour>().stage.addChild(mcTopBottom);
        bottomUI.GetComponent<MovieClipOverlayCameraBehaviour>().stage.addChild(mcTrash);
		bottomUI.GetComponent<MovieClipOverlayCameraBehaviour>().postRender = false;
		bottomUI.GetComponent<MovieClipOverlayCameraBehaviour>().syncStageScalePerFrame = false;
		bottomUI.GetComponent<MovieClipOverlayCameraBehaviour>().stage.scaleX = Screen.width / 800.0f;
		bottomUI.GetComponent<MovieClipOverlayCameraBehaviour>().stage.scaleY = Screen.height / 600.0f;

        mcOperation.getChildByName<MovieClip>("mcFirstOperator").getChildByName<TextField>("tfNumerator").colorTransform = Green1;
        mcOperation.getChildByName<MovieClip>("mcFirstOperator").getChildByName<TextField>("tfDenominator").colorTransform = Green1;
        mcOperation.getChildByName<MovieClip>("mcSecondOperator").getChildByName<TextField>("tfNumerator").colorTransform = Green1;
        mcOperation.getChildByName<MovieClip>("mcSecondOperator").getChildByName<TextField>("tfDenominator").colorTransform = Green1;
        mcOperation.getChildByName<MovieClip>("mcResult").getChildByName<TextField>("tfNumerator").colorTransform = Green1;
        mcOperation.getChildByName<MovieClip>("mcResult").getChildByName<TextField>("tfDenominator").colorTransform = Green1;

        //highlightMovieclips.Add("mcFirstOperator", mcOperation.getChildByName<MovieClip>("mcFirstOperator"));
        //highlightMovieclips.Add("mcSecondOperator", mcOperation.getChildByName<MovieClip>("mcSecondOperator"));
        //highlightMovieclips.Add("mcResult", mcOperation.getChildByName<MovieClip>("mcResult"));
    }

    void ShowOperationMenu(FractionsOperations currOperation)
    {
        bool checkDifferentOperation = (lastOperation != currOperation);
        if (!mcTopBottom.visible || checkDifferentOperation)
        {
            mcTopBottom.visible = true;

            switch (currOperation)
            {
                case FractionsOperations.ADDITION: mcOperation.gotoAndStop(1); break;
                case FractionsOperations.SUBTRACTION: mcOperation.gotoAndStop(2); break;
                case FractionsOperations.FIND: mcOperation.gotoAndStop(3); break;
            }

            mcFeedback = mcOperation.getChildByName<MovieClip>("mcFeedback");
            mcFeedback.visible = false;
            mcResultOperator = mcOperation.getChildByName<MovieClip>("mcResultOperator");
            mcResultOperator.gotoAndStop("question");
        }
        else
        {
            /*SetFractionValue(mcOperation.getChildByName<MovieClip>("mcFirstOperator"), 0.0f, 0.0f);
            SetFractionValue(mcOperation.getChildByName<MovieClip>("mcSecondOperator"), 0.0f, 0.0f);
            SetFractionValue(mcOperation.getChildByName<MovieClip>("mcResult"), 0.0f, 0.0f);
            mcFeedback.visible = false;*/
            mcTopBottom.visible = false;
        }
        mcFeedback.visible = false;
        mcResultOperator.gotoAndStop("question");
        /*SetFractionValue(mcOperation.getChildByName<MovieClip>("mcFirstOperator"), 0.0f, 0.0f);
        SetFractionValue(mcOperation.getChildByName<MovieClip>("mcSecondOperator"), 0.0f, 0.0f);
        SetFractionValue(mcOperation.getChildByName<MovieClip>("mcResult"), 0.0f, 0.0f);*/

        lastOperation = currOperation;
        //CheckFractionOperation();
    }

    void HideOperationMenu()
    {
        mcResultOperator = mcOperation.getChildByName<MovieClip>("mcResultOperator");
        Debug.Log("mcOperation: " + mcOperation);
        mcResultOperator.gotoAndStop("question");
        mcFeedback = mcOperation.getChildByName<MovieClip>("mcFeedback");
        mcFeedback.visible = false;
        mcTopBottom.visible = false;
    }

    void ResetOperationMenu()
    {
        mcFeedback.visible = false;
        SetFractionValue(mcOperation.getChildByName<MovieClip>("mcFirstOperator"), 0.0f, 0.0f);
        SetFractionValue(mcOperation.getChildByName<MovieClip>("mcSecondOperator"), 0.0f, 0.0f);
        SetFractionValue(mcOperation.getChildByName<MovieClip>("mcResult"), 0.0f, 0.0f);
    }

    void SendDropMessage(MovieClip mc, RootElement root)
    {
        if (mc != null)
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
            switch (mc.name)
            {
                case ("Fraction1Area"):
                    str2 = "Op1";
                    break;
                case ("Fraction2Area"):
                    str2 = "Op2";
                    if (lastOperation == FractionsOperations.FIND)
                        str2 = "Op1";
                    break;
                case ("ResultArea"):
                    str2 = "Res";
                    if (lastOperation == FractionsOperations.FIND)
                        str2 = "Op2";
                    break;
            }

            ExternalEventsManager.Instance.SendMessageToSupport("FractionPlaced", str1 + str2, root.name, root.partNumerator + "/" + root.partDenominator);
        }
    }

    void SetFractionValue(MovieClip mc, float numerator, float denominator)
    {
        if (mc != null)
        {
            if (mc.name == "Fraction1Area") mc = mcOperation.getChildByName<MovieClip>("mcFirstOperator");
            else if (mc.name == "Fraction2Area") mc = mcOperation.getChildByName<MovieClip>("mcSecondOperator");
            else if (mc.name == "ResultArea") mc = mcOperation.getChildByName<MovieClip>("mcResult");
            mc.getChildByName<TextField>("tfNumerator").text = (denominator == 0.0f) ? string.Empty : numerator.ToString();
            mc.getChildByName<TextField>("tfDenominator").text = (denominator == 0.0f) ? string.Empty : denominator.ToString();
            CheckFractionOperation();
        }
    }

    void EnableBarOperations()
    {
        SetupButton(mcTop.getChildByName<MovieClip>("mcFractionSearch"));
        SetupButton(mcTop.getChildByName<MovieClip>("mcFractionAdd"));
        SetupButton(mcTop.getChildByName<MovieClip>("mcFractionSub"));
    }

    void DisableBarOperations()
    {
        DisableButton(mcTop.getChildByName<MovieClip>("mcFractionSearch"));
        DisableButton(mcTop.getChildByName<MovieClip>("mcFractionAdd"));
        DisableButton(mcTop.getChildByName<MovieClip>("mcFractionSub"));
    }
    #endregion

    #region Buttons Functions
    public void SetupButton(MovieClip mc)
    {
        mc.gotoAndStop("up");
        mc.addEventListener(MouseEvent.MOUSE_DOWN, OnBtnsPress);
        mc.addEventListener(MouseEvent.MOUSE_UP, OnBtnsClick);
        mc.addEventListener(MouseEvent.MOUSE_ENTER, OnBtnsEnter);
        mc.addEventListener(MouseEvent.MOUSE_LEAVE, OnBtnsLeave);
    }

    public void DisableButton(MovieClip mc, string text=null)
    {
        mc.removeEventListener(MouseEvent.MOUSE_DOWN, OnBtnsPress);
        mc.removeEventListener(MouseEvent.MOUSE_UP, OnBtnsClick);
        mc.removeEventListener(MouseEvent.MOUSE_ENTER, OnBtnsEnter);
        mc.removeEventListener(MouseEvent.MOUSE_LEAVE, OnBtnsLeave);
        mc.gotoAndStop("gh");

        if (text != null)
            localizationUtils.AddTranslationText(mc.getChildByName<TextField>("tfLabel"), text);
    }

    public void RemoveAllEventFromButton(MovieClip m)
    {
        m.removeAllEventListeners(MouseEvent.MOUSE_DOWN);
        m.removeAllEventListeners(MouseEvent.MOUSE_UP);
        m.removeAllEventListeners(MouseEvent.MOUSE_ENTER);
        m.removeAllEventListeners(MouseEvent.MOUSE_LEAVE);
    }
    #endregion

    #region Buttons Callbacks
    void OnBtnsPress(CEvent evt)
    {
        MovieClip mc = evt.currentTarget as MovieClip;

        if (mc.currentLabel == "gh") return;
		isPressingButton = false;

        mc.gotoAndStop("dn");
        switch (mc.name)
        {
            case "btLines":
            case "btNumbers":
            case "btPies":
            case "btSets":
            case "btContainers":
            case "btCircles":
            case "btHRect":
            case "btVRect":
            case "mcFractionAdd":
            case "mcFractionSub":
            case "mcFractionSearch":
            case "btChangeFraction":
            case "btChangeSize":
            case "btChangeColor":
            case "btCopy":
            case "btCut":
            case "btShow":
            case "btPopupBack":
            case "btCloseHighFeedback":
            case "btJoin":
            case "btTakeAway":
            case "btCompare":
            case "btFind":
            case "mcBgSideArrow":
            case "btClosePopupColors":
            case "btColorShape1":
            case "btColorShape2":
            case "btColorShape3":
            case "btColorShape4":
                isPressingButton = true;
                break;
            case "btPartition":
                CheckNotificationWarning();
                isPressingButton = true;
                break;
        }
    }

    void OnBtnsClick(CEvent evt)
    {
        MovieClip mc = evt.currentTarget as MovieClip;

        if (mc.currentLabel == "gh") return;

        if (!isPressingButton)
			return;
        CloseToolSubmenu();

        mc.gotoAndStop("dn");
        switch (mc.name)
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
                OpenToolSubmenu(mc.name);
                break;
            case "btPies":
                OpenToolSubmenu(mc.name);
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
                CreateVRect();
                //OpenToolSubmenu("btPies");
                ShowSuggestion("{hint_denominator_first}");
                DisableHUD();
                break;
            case "btVRect":
                ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "Shape/VRects");
                CreateHRect();
                //OpenToolSubmenu("btPies");
                ShowSuggestion("{hint_denominator_first}");
                DisableHUD();
                break;
            case "btHearts":
                ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "Sets/Hearts");
                //CreateHRect();
                OpenToolSubmenu("btSets");
                ShowSuggestion("{hint_denominator_first}");
                DisableHUD();
                break;
            case "btStars":
                ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "Sets/Stars");
                //CreateHRect();
                OpenToolSubmenu("btSets");
                ShowSuggestion("{hint_denominator_first}");
                DisableHUD();
                break;
            case "btMoons":
                ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "Sets/Moons");
                //CreateHRect();
                OpenToolSubmenu("btSets");
                ShowSuggestion("{hint_denominator_first}");
                DisableHUD();
                break;
            case "mcFractionAdd":
                ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "Sum");
                ShowOperationMenu(FractionsOperations.ADDITION);
                ResetOperationMenu();
                break;
            case "mcFractionSub":
                ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "Substraction");
                ShowOperationMenu(FractionsOperations.SUBTRACTION);
                ResetOperationMenu();
                break;
            case "mcFractionSearch":
                ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "Equivalence");
                ShowOperationMenu(FractionsOperations.FIND);
                ResetOperationMenu();
                break;
            case "btChangeFraction":
                ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "Tool/ChangeFraction");
                MenuToolsCallback(mc.name);
                CheckNotificationWarning();
                break;
            case "btChangeSize":
                MenuToolsCallback(mc.name);
                CheckNotificationWarning();
                break;
            case "btChangeColor":
                MenuToolsCallback(mc.name);
                CheckNotificationWarning();
                break;
            case "btCopy":
                ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "Tool/Copy");
                MenuToolsCallback(mc.name);
                CheckNotificationWarning();
                break;
            case "btCut":
                ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "Tool/Use");
                MenuToolsCallback(mc.name);
                CheckNotificationWarning();
                break;
            case "btShow":
                ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "Tool/ShowHideSymbol");
                MenuToolsCallback(mc.name);
                CheckNotificationWarning();
                break;
            case "btPartition":
                ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "Tool/ShowHidePartition");
                MenuToolsCallback(mc.name);
                CheckNotificationWarning();
                break;
            case "btPopupBack":
                RemovePopup();
            break;
            case "btCloseHighFeedback":
                ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "CloseFeedbackPopup");
                RemoveFeedbackPopup();
                break;
            case "btJoin":
                ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "Action/Sum");
                //mcMouseIcon.gotoAndStop(3);
                MenuActionsCallback(mc.name);
                break;
            case "btTakeAway":
                ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "Action/Subtraction");
                //mcMouseIcon.gotoAndStop(4);
                MenuActionsCallback(mc.name);
                break;
            case "btCompare":
                ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "Action/Compare");
                //mcMouseIcon.gotoAndStop(5);
                MenuActionsCallback(mc.name);
                break;
            case "btFind":
                ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "Action/FindParent");
                MenuActionsCallback(mc.name);
                break;
            case "mcBgSideArrow":
                ArrowButtonCallback(false);
                CloseToolSubmenu();
                break;
            case "btClosePopupColors":
                mcColorPalette.visible = false;
                Workspace.Instance.SendMessage("EnableInput");
                EnableHUD();
                break;
            case "btColorShape1":
                mcColorPalette.visible = false;
                ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "Tool/ChangeColour");
                Workspace.Instance.ElementOnFocus.SendMessage("ChangeColor", Workspace.Instance.colorList[0]);
                Workspace.Instance.SendMessage("EnableInput");
                EnableHUD();
                break;
            case "btColorShape2":
                mcColorPalette.visible = false;
                ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "Tool/ChangeColour");
                Workspace.Instance.ElementOnFocus.SendMessage("ChangeColor", Workspace.Instance.colorList[1]);
                Workspace.Instance.SendMessage("EnableInput");
                EnableHUD();
                break;
            case "btColorShape3":
                mcColorPalette.visible = false;
                ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "Tool/ChangeColour");
                Workspace.Instance.ElementOnFocus.SendMessage("ChangeColor", Workspace.Instance.colorList[2]);
                Workspace.Instance.SendMessage("EnableInput");
                EnableHUD();
                break;
            case "btColorShape4":
                mcColorPalette.visible = false;
                ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "Tool/ChangeColour");
                Workspace.Instance.ElementOnFocus.SendMessage("ChangeColor", Workspace.Instance.colorList[3]);
                Workspace.Instance.SendMessage("EnableInput");
                EnableHUD();
                break;
		}

		isPressingButton = false;
    }

    void OnBtnsEnter(CEvent evt)
    {
        MovieClip mc = evt.currentTarget as MovieClip;
        //mc.gotoAndStop("ov");
        mcMouseIcon.gotoAndStop(2);

        switch (mc.name)
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
            case ("mcFractionSearch"):
                ShowHint("{hint_find_eq}");
                break;
            case ("mcFractionAdd"):
                ShowHint("{hint_find_sum}");
                break;
            case ("mcFractionSub"):
                ShowHint("{hint_find_sub}");
                break;
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
            case "btCut":
                ShowHint("{hint_cut}");
                break;
            case "btCopy":
                ShowHint("{hint_copy}");
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

    void OnBtnsLeave(CEvent evt)
    {
        MovieClip mc = evt.currentTarget as MovieClip;
        mc.gotoAndStop("up");
        if (mcMouseIcon.currentFrame == 2)
            mcMouseIcon.gotoAndStop(1);
        ShowHint("");

        switch (mc.name)
        {
            case "btPartition":
                CheckNotificationWarning();
                break;
        }
    }

    #endregion //Ingame Buttons Callback

    #region Localized buttons
    public void OnTranslationBtnsEnter(LocalizedButton bt)
    {
    }

    public void OnTranslationBtnsLeave(LocalizedButton bt)
    {
    }

    public void OnTranslationBtnsClick(LocalizedButton bt)
    {
        switch (bt.mc.name)
        { 
            case "btStart":                
                DestroyStartPage();
                InitializeHUD();
                break;
            case "btOk":
                HideActionPopup();
                break;
        }
    }
    #endregion

    #region Popup Functions
    public enum PopupType
    {
        PAUSE = 0
    }

    void InitializePopup()
    {
        mcPopup = mcIngame.getChildByName<MovieClip>("mcPopup");
        mcPopup.visible = false;
        mcPopup.x = Screen.width * 0.5f;
        mcPopup.y = Screen.height * 0.5f;

        btPopupBack = mcPopup.getChildByName<MovieClip>("btPopupBack");
        SetupButton(btPopupBack);      
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

        mcPopup.visible = true;

        TextField popupTextField = mcPopup.getChildByName<TextField>("tfPopupLabel");
        PopupType type = (PopupType)System.Enum.Parse(typeof(PopupType), name);
        switch (type)
        {
            case PopupType.PAUSE:
                popupTextField.text = Localizations.Instance.getString("popup_pause_text");
                break;
        }
    }


    public string RemovePopup()
    {
        if (0 == popupsList.Count)
            return null;

        string popupName = popupsList.Last.Value;
        popupsList.RemoveLast();
        mcPopup.visible = false;

        if (popupsList.Count > 0)
            this.SetupNextPopup(popupsList.Last.Value, false);
        else
            Workspace.Instance.SendMessage("EnableInput");

        return popupName;
    }

    #endregion

    #endregion

    #region Messages
    void OnElementClicked()
    {
        elementSelected = true;
    }

    void OnElementReleased(GameObject element)
    {
        GameObject elementOnFocus = Workspace.Instance.ElementOnFocus;
        RootElement selElement = elementOnFocus.GetComponentInChildren<RootElement>();

		if (IsOverHUDElement(mcTrash) && isOnTrash)
        {
            //if (selElement.state == ElementsState.Improper || selElement.denominator != 0 && selElement.mode == InteractionMode.Moving)
            {
                if (selElement.denominator == 0)
                    EnableHUD();

                Workspace.Instance.SendMessage("DeleteElement", element);

                string eventName = "FractionTrashed";
                switch(selElement.state)
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
                }

                string value = selElement.partNumerator + "/" + selElement.partDenominator;
                ExternalEventsManager.Instance.SendMessageToSupport(eventName, typeString, selElement.name, value);  
            }

            mcTrash.gotoAndStop("up");
        }
        elementSelected = false;

        if (mcTopBottom.visible)
        {
            dropOverMc = null;
            if (null != mcOperation.getChildByName<MovieClip>("Fraction1Area"))
                CheckDragDrop(mcOperation.getChildByName<MovieClip>("Fraction1Area"));

            CheckDragDrop(mcOperation.getChildByName<MovieClip>("Fraction2Area"));
            CheckDragDrop(mcOperation.getChildByName<MovieClip>("ResultArea"));
        }

        if (dropOverMc != null)
        {
            if (selElement.denominator != 0 && selElement.mode == InteractionMode.Moving && (selElement.state == ElementsState.Fraction || selElement.state == ElementsState.Result))
            {
                SetFractionValue(dropOverMc, selElement.partNumerator, selElement.partDenominator);
                SendDropMessage(dropOverMc, selElement);
                //elementOnFocus.SendMessage("ResetLastPosition");
            }
            elementOnFocus.SendMessage("ResetLastPosition");
            //Workspace.Instance.ElementOnFocus.SendMessage("ScaleUp");
        }
        dropOverMc = null;
    }

    void CheckNotificationWarning()
    {
        MovieClip notification = mcMenuTools.getChildByName<MovieClip>("btPartition").getChildByName<MovieClip>("mcNotification");
        if (elementOnFocus.GetComponent<RootElement>().state == ElementsState.Result)
        {
            if (null != notification)
                notification.visible = false;
        }
        else
        {
            bool partitionEnabled = elementOnFocus.GetComponent<RootElement>().CheckPartition();
            if (partitionEnabled)
            {
                if (null != notification)
                    notification.visible = false;
            }
            else
            {
                if (null != notification)
                    notification.visible = true;
            }
        }
    }

    void CheckResultFraction()
    {
        //DisableButton(mcMenuTools.getChildByName<MovieClip>("btChangeFraction"), menuTexts[0]);
        //DisableButton(mcMenuTools.getChildByName<MovieClip>("btChangeSize"), menuTexts[1]);
        DisableButton(mcMenuTools.getChildByName<MovieClip>("btChangeColor"), menuTexts[0]);
        DisableButton(mcMenuTools.getChildByName<MovieClip>("btCopy"), menuTexts[1]);
        //DisableButton(mcMenuTools.getChildByName<MovieClip>("btCut"), menuTexts[4]);
        DisableButton(mcMenuTools.getChildByName<MovieClip>("btPartition"), menuTexts[2]);
    }

    /*void CheckCutEnabled()
    {
        MovieClip notification = mcMenuTools.getChildByName<MovieClip>("btCut");
        bool cutEnabled = elementOnFocus.GetComponent<RootElement>().CheckCut();
        if (cutEnabled)
            InitializeSingleMenuTool("Cut", mcMenuTools.getChildByName<MovieClip>("btCut"), CutFraction, menuTexts[4], toolsMenuButtonsBgWidth);
        else
            DisableButton(mcMenuTools.getChildByName<MovieClip>("btCut"), menuTexts[4]);
    }*/

    void OnShowContextMenu(GameObject element)
    {
        InitializeMenuTools(mcMenuTools);
        elementOnFocus = element;

		Vector3 elementPos = mainCamera.WorldToScreenPoint(element.transform.position);
        float wGap = mcMenuTools.width * 0.5f;
		if (elementPos.x > Screen.width * 0.5f)
            mcMenuTools.x = elementPos.x / (Screen.width / 800.0f) - wGap;
		else
            mcMenuTools.x = elementPos.x / (Screen.width / 800.0f) + wGap;

        float hGap = mcMenuTools.height * 0.5f;
        if (elementPos.y > Screen.height * 0.5f)
            mcMenuTools.y = 600.0f - (elementPos.y / (Screen.height / 600.0f) - hGap);
        else
            mcMenuTools.y = 600.0f - (elementPos.y / (Screen.height / 600.0f) + hGap);

		mcMenuTools.visible = true;
        CheckNotificationWarning();
        Workspace.Instance.SendMessage("DisableInput");
    }

    void OnShowActionsMenu(GameObject element)
    {
        elementOnFocus = element;

        Vector3 elementPos = mainCamera.WorldToScreenPoint(element.transform.position);
        float wGap = 0.0f; // mcMenuAction.width * 0.5f;
        if (elementPos.x > Screen.width * 0.5f)
            mcMenuAction.x = Input.mousePosition.x / (Screen.width / 800.0f) - wGap;
        else
            mcMenuAction.x = Input.mousePosition.x / (Screen.width / 800.0f) + wGap;

        mcMenuAction.x = Mathf.Clamp(mcMenuAction.x, mcMenuAction.width * 0.5f, Screen.width - mcMenuAction.width * 0.5f);

        float hGap = mcMenuAction.height * 0.5f;
        if (elementPos.y > Screen.height * 0.5f)
            mcMenuAction.y = 600.0f - (elementPos.y / (Screen.height / 600.0f) - hGap);
        else
            mcMenuAction.y = 600.0f - (elementPos.y / (Screen.height / 600.0f) + hGap);
        mcMenuAction.visible = true;
        Workspace.Instance.SendMessage("DisableInput");
    }

    void OnHideContextMenu()
    {
        if (mcMenuTools.visible)
        {
            if (!IsOverHUDElement(mcMenuTools))
            {
                mcMenuTools.visible = false;
                Workspace.Instance.SendMessage("EnableInput");
            }
        }

    }

    void OnHideActionsMenu()
    {
        if (mcMenuAction.visible)
        {
            if (!IsOverHUDElement(mcMenuAction))
            {
                mcMenuAction.visible = false;
                //InitializeSingleMenuAction("FindParent", mcMenuAction.getChildByName<MovieClip>("btCompare"), CompareCuts, menuTexts[9], actionsMenuButtonsBgWidth);
                Workspace.Instance.SendMessage("EnableInput");
            }
        }

    }

    void OnHideSubMenus()
    {
        for (int i = 0; i < toolsBarData.Count; ++i)
        {
            MovieClip subMenu = toolsBarData[i].subMenu;
            if (subMenu != null && subMenu.visible)
            {
                if (!IsOverHUDElement(subMenu))
                {
                    subMenu.visible = false;
                }
            }
        }
    }

    void ShowActionPopup()
    {
        DisableHUD();
        Workspace.Instance.SendMessage("DisableInput");
        elementSelected = false;
        if (actionsPopupBG != null)
            actionsPopupBG.SetActive(true);

        if (null != mcActionsPopupButton)
            mcActionsPopupButton.visible = true;
    }

    void HideActionPopup()
    {
        elementSelected = false;
        if (actionsPopupBG != null)
            actionsPopupBG.SetActive(false);

        if (null != mcActionsPopupButton)
            mcActionsPopupButton.visible = false;

        Workspace.Instance.SendMessage("TerminateCurrentAction");
        Workspace.Instance.SendMessage("EnableInput");
        EnableHUD();
    }
    #endregion

}
