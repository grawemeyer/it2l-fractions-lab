using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using fractionslab.behaviours;

public class Zoom : MonoBehaviour
{
    #region Protected field
    protected bool checkPosition = false;
    public bool IsMouseOut = true;
    protected float speedTouch0 = 0.0f;
    protected float speedTouch1 = 0.0f;
    protected float outOffset = 1.0f;
    protected float duration = 0.05f;
    protected float startTime = 0;
    protected float widthSize;
    protected float heightSize;
    protected float scaleFactorH = 1;
    protected float scaleFactorW = 1;
    protected float newOrthoSize = 10.0f;
    protected float orthoWidth;
    protected float orthoHeight;
    protected float initialOrthosize;
    protected float previousTime;
    protected Touch touchZero;
    protected Touch touchOne;
    protected Touch touchZeroPr;
    protected Touch touchOnePr;
    protected Vector2 deltaPosition0 = Vector2.zero;
    protected Vector2 deltaPosition1 = Vector2.zero;
    protected Vector3 initialPosition;
    protected Vector3 finalPosition;
    protected Vector3 initialScale;
    protected Vector3 finalScale;
    protected Vector3 initialSymPos;
    protected Vector3 finalSymPos;
    protected Vector3 initialPartPos;
    protected Vector3 finalPartPos;
    protected Vector3 newPos;
    protected GameObject symbol;
    protected GameObject partition;
    #endregion

    #region Public field
    public float orthoZoomSpeed;
    public float minPinchSpeed = 10.0f;
    public float minXCamera = -6.8f;
    public float maxXCamera = 6.8f;
    public float minYCamera = -9.3f;
    public float maxYCamera = 9.3f;
    public float minCameraSize = 3.0f;
    public float maxCameraSize = 10.0f;
    public Vector2 fromPos;
    public Vector2 toPos;
    public Text percentage;
    public Slider slider;
    public Camera maincamera;
    public Button plus;
    public Button minus;
    public InterfaceBehaviour interfaceB;
    #endregion

    void Start() 
    {
        interfaceB.localizationUtils.AddTranslationText(percentage, "{zoom}");
    }

    void OnEnable()
    {
        orthoZoomSpeed = (maxCameraSize - minCameraSize) / Screen.height;
        slider.value = slider.minValue;
        newOrthoSize = maincamera.orthographicSize;
        newPos = new Vector3();
        initialOrthosize = maincamera.orthographicSize;
        minus.GetComponent<UIButton>().DisableBtn(false);
        fromPos = new Vector3(0.0f, 0.0f);
    }

    public void updateSizeSlider()
    {
        if (IsMouseOut)
        {
            slider.value = (maxCameraSize * 100.0f) / maincamera.orthographicSize;
            return;
        }
        newOrthoSize = Mathf.Clamp((maxCameraSize * 100.0f) / slider.value, minCameraSize, maxCameraSize);
        CheckBound();
    }

    public void updateSizeButton(float step)
    {
        /*if (IsMouseOut)
        {
            slider.value = (maxCameraSize * 100.0f) / maincamera.orthographicSize;
            return;
        }*/
        if(step < 0)
            ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "ZoomOut");
        else
            ExternalEventsManager.Instance.SendMessageToSupport("ClickButton", "ZoomIn");
        previousTime = Time.time;
        slider.value += step;
        newOrthoSize = Mathf.Clamp((maxCameraSize * 100.0f) / slider.value, minCameraSize, maxCameraSize);
        CheckBound();
    }

    public void updateSizeCont(float step)
    {
        if (Time.time - previousTime > 0.5f)
            updateSizeButton(step);
    }

    public void OnMouseDown()
    {
        GameObject.FindGameObjectWithTag("Interface").GetComponent<InterfaceBehaviour>().isZoomActive = true;
    }

    public void PointerExit()
    {
        IsMouseOut = true;   
    }

    public void PointerEnter()
    {
        IsMouseOut = false;
    }

    public void SendExternalMsg(bool isDown)
    {
        if (isDown)
        {
            ExternalEventsManager.Instance.SendMessageToSupport("Slider", "BeginSlide", slider.value);

        }
        else if (GameObject.FindGameObjectWithTag("Interface").GetComponent<InterfaceBehaviour>().isZoomActive)
        {
            ExternalEventsManager.Instance.SendMessageToSupport("Slider", "EndSlide", slider.value);
        }
    }

    public void CheckBound()
    {
        if (slider.value == slider.minValue)
        {
            if(minus.GetComponent<Button>().IsInteractable())
                minus.GetComponent<UIButton>().DisableBtn(false);
        }
        else
        {
           // if (minus.GetComponent<Button>().IsInteractable())
                minus.GetComponent<UIButton>().EnableBtn(false);
        }
        if (slider.value == slider.maxValue)
        {
            if (plus.GetComponent<Button>().IsInteractable())
                plus.GetComponent<UIButton>().DisableBtn(false);
        }
        else
        {
           // if (plus.GetComponent<Button>().IsInteractable())
                plus.GetComponent<UIButton>().EnableBtn(false);
        }
    }

    void Update()
    {

        if (Workspace.Instance.isAction)
        {
            return;
        }
#if UNITY_IPHONE || UNITY_ANDROID
        if (Input.touchCount == 2 && GameObject.FindGameObjectWithTag("Interface").GetComponent<InterfaceBehaviour>().InputEnabled && Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(1).phase == TouchPhase.Moved)
        {
            Workspace.Instance.isPinchActive = true;
            touchZero = Input.touches[0];
            touchOne = Input.touches[1];
            if (!checkPosition)
            {
                toPos = Vector2.zero;
                toPos = (touchOne.position + touchZero.position) * 0.5f;
                toPos = maincamera.ScreenToWorldPoint(toPos);
                touchZeroPr = Input.touches[0];
                touchOnePr = Input.touches[1];
                checkPosition = true;
                ExternalEventsManager.Instance.SendMessageToSupport("BeginPinch", slider.value, "(" + toPos.x + ", " + toPos.y + ")");
            }

            Vector2 touchZeroPrevPos = touchZeroPr.position;
            Vector2 touchOnePrevPos = touchOnePr.position;
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
            touchZeroPr = Input.touches[0];
            touchOnePr = Input.touches[1];
            speedTouch0 = Input.GetTouch(0).deltaPosition.magnitude / Input.GetTouch(0).deltaTime;
            speedTouch1 = Input.GetTouch(1).deltaPosition.magnitude / Input.GetTouch(1).deltaTime;
            if (speedTouch0 >= minPinchSpeed && speedTouch1 >= minPinchSpeed)
            {
                newOrthoSize = Mathf.Clamp((maincamera.orthographicSize + deltaMagnitudeDiff * orthoZoomSpeed), minCameraSize, (maxCameraSize + outOffset));
                slider.value = (maxCameraSize * 100.0f) / newOrthoSize;
            }
        }
#endif
        if (maincamera.orthographicSize != newOrthoSize && GameObject.FindGameObjectWithTag("Interface").GetComponent<InterfaceBehaviour>().InputEnabled)
        {
            float prevOrtho = maincamera.orthographicSize;
            maincamera.orthographicSize = newOrthoSize;
            maincamera.transform.localScale = new Vector3(newOrthoSize / initialOrthosize, newOrthoSize / initialOrthosize, 1);
            Workspace.Instance.RescaleFractionModifier(slider.value / 100);
            if (Input.touchCount == 2 && Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(1).phase == TouchPhase.Moved)
            {
                deltaPosition0 = (toPos - new Vector2(maincamera.transform.position.x, maincamera.transform.position.y));
                deltaPosition1 = deltaPosition0 * (newOrthoSize / prevOrtho);
                newPos = deltaPosition0 - deltaPosition1;
                newPos += maincamera.transform.position;
                newPos.z = -10;
            }
            else
            {
                newPos = new Vector3(maincamera.transform.position.x, maincamera.transform.position.y, maincamera.transform.position.z);
            }
            orthoHeight = maincamera.orthographicSize;
            orthoWidth = ((maincamera.orthographicSize * Screen.width) / Screen.height);
            if ((newPos.x + orthoWidth) > (initialOrthosize * Screen.width) / Screen.height)
                newPos.x = ((initialOrthosize * Screen.width) / Screen.height) - orthoWidth;
            else if ((newPos.x - orthoWidth) < -((initialOrthosize * Screen.width) / Screen.height))
                newPos.x = -((initialOrthosize * Screen.width) / Screen.height) + orthoWidth;
            if ((newPos.y + orthoHeight) > initialOrthosize)
                newPos.y = (initialOrthosize - orthoHeight);
            if (((newPos.y - orthoHeight) < -initialOrthosize))
                newPos.y = -initialOrthosize + orthoHeight;
            if (Input.touchCount == 2)
            {
                if (maincamera.orthographicSize >= maxCameraSize && maincamera.orthographicSize <= (maxCameraSize + outOffset))
                {
                    Workspace.Instance.delimiters.GetComponent<ShowDelimiters>().ShowAll((maincamera.orthographicSize - maxCameraSize) / outOffset);
                    newPos = new Vector3(0.0f, 0.0f, -10.0f);
                }
            }
            maincamera.transform.position = newPos;
        }
#if !UNITY_IPHONE && !UNITY_ANDROID
        if (Input.GetMouseButtonUp(0))
#else
        if (Input.touchCount == 0)
#endif
        {
            if (Workspace.Instance.isPinchActive)
            {
                ExternalEventsManager.Instance.SendMessageToSupport("EndPinch", slider.value, "(" + toPos.x + ", " + toPos.y + ")");
                Workspace.Instance.isPinchActive = false;
                if (maincamera.orthographicSize >= maxCameraSize)
                {
                    Workspace.Instance.delimiters.GetComponent<ShowDelimiters>().Reset(new Vector3(0.0f, 0.0f, -10.0f), true);
                }
            }

            GameObject.FindGameObjectWithTag("Interface").GetComponent<InterfaceBehaviour>().isZoomActive = false;
            checkPosition = false;
            deltaPosition0 = Vector2.zero;
            deltaPosition1 = Vector2.zero;

            newOrthoSize = maincamera.orthographicSize;
            touchZero = new Touch();
            touchOne = new Touch();
           // Workspace.Instance.RescaleFractionModifier(slider.value / 100);
        }
        //CheckBound();
    }
}