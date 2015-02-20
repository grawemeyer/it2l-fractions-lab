using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SBS.Math;
using fractionslab;
using fractionslab.meshes;
using fractionslab.utils;

public class SingleFractionMCElement : WSElement, IWSElement
{
    #region Public Fields
    public bool showNumbers = false;
    public GameObject mcObj = null;
    public Text numeratorText;
    public Text denominatorText;
    public GameObject btnsNumerator;
    public GameObject btnsDenominator;
    #endregion

    #region Protected Fields
    protected int lastNumerator = 0;
    protected int lastDenominator = 0;
    protected int lastPartNumerator = 0;
    protected int lastPartDenominator = 0;
    protected float scale = 2.0f;
    //protected float width = 0.0f;
    //protected float height = 4.0f;
    //protected GameObject root = null;
    #endregion

    #region Unity Callbacks
    void Awake()
    {
        float offsX = 0.5f * scale,
                offsY = 0.5f * scale;
        if (Application.platform == RuntimePlatform.OSXWebPlayer)
            offsX = offsY = 0.0f;
#if UNITY_IPHONE  || UNITY_ANDROID
        offsX = offsY = 0.0f;
#endif

    }

    void Start()
    {
        root = transform.parent.gameObject;
        UpdateNumerator();
        UpdateDenominator();
        if (root.GetComponent<RootElement>().mode == InteractionMode.Initializing)
            btnsDenominator.SetActive(true);
    }

    void Update()
    {
        if (numerator != lastNumerator || partNumerator != lastPartNumerator)
        {
            UpdateNumerator();
        }

        if (denominator != lastDenominator || partDenominator != lastPartDenominator)
        {
            UpdateDenominator();
        }
    }

    void OnMouseDown()
    {
        root.GetComponent<RootElement>().inputByChild = true;
        root.GetComponent<RootElement>().OnMouseDown();
    }

    void OnMouseDrag()
    {
        root.GetComponent<RootElement>().OnMouseDrag();
    }

    void OnMouseUp()
    {
        if (root.GetComponent<RootElement>().hasDragged)
            root.GetComponent<RootElement>().OnMouseUp();
        root.GetComponent<RootElement>().inputByChild = false;
    }

    /*  void OnGUI()
      {
          GUI.skin = Workspace.Instance.skin;
          Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
		

          if (root.GetComponent<RootElement>().mode == InteractionMode.Freeze || root.GetComponent<RootElement>().mode == InteractionMode.LookAt)
              return;

          if (!root.GetComponent<RootElement>().inputEnabled)
              return;
         // Debug.Log("singlegui");
          if (GUI.Button(new Rect(screenPos.x - (10 * Screen.width / 800.0f), Screen.height - screenPos.y - (24 * Screen.height / 600.0f), 21.0f * Screen.width / 800.0f, 22.0f * Screen.height / 600.0f), ""))
          {
              if (denominator > 0)
              {
                  Workspace.Instance.SendMessage("SetFocusOn", root);
                  root.SendMessage("SetMode", InteractionMode.Changing);
                  root.SendMessage("OnSelectFractionPart", FractionPart.Numerator);
              }
          }

          if (GUI.Button(new Rect(screenPos.x - (10 * Screen.width / 800.0f), Screen.height - screenPos.y + (4 * Screen.height / 600.0f), 21.0f * Screen.width / 800.0f, 22.0f * Screen.height / 600.0f), ""))
          {
              Workspace.Instance.SendMessage("SetFocusOn", root);
              root.SendMessage("SetMode", InteractionMode.Changing);
              root.SendMessage("OnSelectFractionPart", FractionPart.Denominator);
          }
      }*/
    #endregion

    #region Protected Methods
    protected void UpdateNumerator()
    {
        if (partitions == 1)
        {
            //textnumerator change
            numeratorText.text = numerator.ToString();
            //  mcb.movieClip.getChildByName<TextField>("tfValue1").text = numerator.ToString();
            lastNumerator = numerator;
        }
        //textnumetor change
        numeratorText.text = partNumerator.ToString();
        lastPartNumerator = partNumerator;
    }

    protected void UpdateDenominator()
    {
        if (partitions == 1)
        {
            //textnumetor change
            denominatorText.text = denominator.ToString();
            lastDenominator = denominator;
        }
        //textnumetor change
        denominatorText.text = partDenominator.ToString();
        lastPartDenominator = partDenominator;
    }

    public void OnSelectNumerator()
    {
        if (root.GetComponent<RootElement>().mode == InteractionMode.Freeze || root.GetComponent<RootElement>().mode == InteractionMode.LookAt || root.GetComponent<RootElement>().mode == InteractionMode.Initializing)
            return;

        if (!root.GetComponent<RootElement>().inputEnabled)
            return;
        if (denominator > 0)
        {
            Workspace.Instance.SendMessage("SetFocusOn", root);
            root.BroadcastMessage("SetMode", InteractionMode.Changing, SendMessageOptions.DontRequireReceiver);
            root.SendMessage("OnSelectFractionPart", FractionPart.Numerator);
        }
        btnsDenominator.SetActive(false);
        btnsNumerator.SetActive(true);
        root.GetComponent<RootElement>().PlaceButtons(0);
    }

    public void OnSelectDenominator()
    {
      
        if (denominator == 0)
            return;
        if (root.GetComponent<RootElement>().mode == InteractionMode.Freeze || root.GetComponent<RootElement>().mode == InteractionMode.LookAt)
            return;

        if (!root.GetComponent<RootElement>().inputEnabled)
            return;
        root.GetComponent<RootElement>().PlaceButtons(1);
        btnsDenominator.SetActive(true);
        btnsNumerator.SetActive(false);
        Workspace.Instance.SendMessage("SetFocusOn", root);
        root.BroadcastMessage("SetMode", InteractionMode.Changing, SendMessageOptions.DontRequireReceiver);
        root.SendMessage("OnSelectFractionPart", FractionPart.Denominator);
    }

    public void HideButtons()
    {
        btnsDenominator.SetActive(false);
        btnsNumerator.SetActive(false);
    }

    public void ChangeStateButtons(bool isEnable)
    {
        foreach (Button bt in btnsDenominator.GetComponentsInChildren<Button>())
        {
            bt.interactable = isEnable;
            if (null != bt.gameObject.GetComponent<UIButton>())
            {
                if (!isEnable)
                    bt.gameObject.GetComponent<UIButton>().DisableBtn(false);
                else
                    bt.gameObject.GetComponent<UIButton>().EnableBtn(false);

            }
        }
        foreach (Button bt in btnsNumerator.GetComponentsInChildren<Button>())
        {
            bt.interactable = isEnable;
            if (null != bt.gameObject.GetComponent<UIButton>())
            {
                if (!isEnable)
                    bt.gameObject.GetComponent<UIButton>().DisableBtn(false);
                else
                    bt.gameObject.GetComponent<UIButton>().EnableBtn(false);

            }
        }
    }
    #endregion

    #region Public Methods

    public void OnClickArrowUp(int part)
    {
        root.BroadcastMessage("SetMode", InteractionMode.Changing, SendMessageOptions.DontRequireReceiver);
        root.GetComponent<RootElement>().OnClickArrowUp(part);

    }

    public void OnClickArrowDown(int part)
    {
        root.BroadcastMessage("SetMode", InteractionMode.Changing, SendMessageOptions.DontRequireReceiver);
        root.GetComponent<RootElement>().OnClickArrowDown(part);
    }

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Draw(int zIndex)
    {
        //Update();
    }

    public override SBSBounds GetBounds()
    {
        bounds.Reset();
        return bounds;
    }
    #endregion

    #region Messages
    void SetSize(Vector2 size)
    {
    }

    void ShowFlashNumbers(bool flag)
    { }

    void UpdatePartitionDenominator(int value)
    {
        partDenominator = value;
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
        // Debug.Log("hit : " + hits.Count + " " + gos);
    }
    #endregion
}