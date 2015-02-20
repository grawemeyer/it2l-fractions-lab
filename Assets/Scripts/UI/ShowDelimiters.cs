using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SBS.Math;

public class ShowDelimiters : MonoBehaviour
{

    public GameObject right;
    public GameObject left;
    public GameObject bottom;
    public GameObject top;
    public bool isStartReset;


    protected Vector3 startingCameraPos;
    protected Vector3 finalCameraPos;
    protected bool resetOrtho;
    protected float startTime;
    protected float duration = 0.40f;

    void Awake()
    {
        finalCameraPos = new Vector3(0.0f, 0.0f, -10.0f);
    }

    public void OnEnable()
    {
        right.GetComponent<RectTransform>().localScale = Vector2.one;
        left.GetComponent<RectTransform>().localScale = Vector2.one;
        top.GetComponent<RectTransform>().localScale = Vector2.one;
        bottom.GetComponent<RectTransform>().localScale = Vector2.one;
        if (resetOrtho)
        {
            Camera.main.transform.position = new Vector3(0.0f, 0.0f, -10.0f);
            Camera.main.orthographicSize = Workspace.Instance.halfScreenHeight;
            Camera.main.transform.localScale = Vector3.one;
            resetOrtho = false;
        }
    }

    public void ShowTop(float perc)
    {
        top.GetComponent<RectTransform>().localScale = new Vector2(1.0f, (100 * perc));
    }

    public void ShowLeft(float perc)
    {
        left.GetComponent<RectTransform>().localScale = new Vector2((100 * perc), 1.0f);
    }

    public void ShowBottom(float perc)
    {
        bottom.GetComponent<RectTransform>().localScale = new Vector2(1.0f, (100 * perc));
    }

    public void ShowRight(float percentage)
    {
        right.GetComponent<RectTransform>().localScale = new Vector2(100 * percentage, 1.0f);
    }

    public void ShowAll(float percentage)
    {
        top.GetComponent<RectTransform>().localScale = new Vector2(1.0f, 50 * percentage);
        bottom.GetComponent<RectTransform>().localScale = new Vector2(1.0f, 50 * percentage);
        right.GetComponent<RectTransform>().localScale = new Vector2(50 * percentage, 1.0f);
        left.GetComponent<RectTransform>().localScale = new Vector2(50 * percentage, 1.0f);
    }
    float halfCameraHeight;
    float halfCameraWidth;
    public void Reset(Vector3 _cameraPos, bool _resetOrtho)
    {
        startTime = Time.time;
        startingCameraPos = Camera.main.transform.position;
        finalCameraPos = _cameraPos;
        isStartReset = true;
        resetOrtho = _resetOrtho;

        halfCameraHeight = Camera.main.orthographicSize;
        halfCameraWidth = ((Camera.main.orthographicSize * Screen.width) / Screen.height);

    }
    float zoomOutOffset;
    void Update()
    {
        if (isStartReset)
        {
            zoomOutOffset = (Workspace.Instance.outOffset * Camera.main.orthographicSize) / 10.0f;
            Camera.main.transform.position = Vector3.Lerp(startingCameraPos, finalCameraPos, (Time.time - startTime) / duration);
            ShowRight(Mathf.Clamp(((Camera.main.transform.position.x + halfCameraWidth) - Workspace.Instance.halfScreenWidth) / zoomOutOffset, 0.0f, 1.0f));
            ShowLeft(-Mathf.Clamp(((Camera.main.transform.position.x - halfCameraWidth) + Workspace.Instance.halfScreenWidth) / zoomOutOffset, -1.0f, 0.0f));
            ShowTop(Mathf.Clamp(((Camera.main.transform.position.y + halfCameraHeight) - Workspace.Instance.halfScreenHeight) / zoomOutOffset, 0.0f, 1.0f));
            ShowBottom(-Mathf.Clamp(((Camera.main.transform.position.y - halfCameraHeight) + Workspace.Instance.halfScreenHeight) / zoomOutOffset, -1.0f, 0.0f));
           
            if (resetOrtho)
            {
                Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, Workspace.Instance.halfScreenHeight, (Time.time - startTime) / duration);
                Camera.main.transform.localScale = Vector3.Lerp(Camera.main.transform.localScale, Vector3.one, (Time.time - startTime) / duration);
                if (Camera.main.transform.position == finalCameraPos && Camera.main.orthographicSize == Workspace.Instance.halfScreenHeight && Camera.main.transform.localScale == Vector3.one)
                {
                    isStartReset = false;
                    OnEnable();
                }
            }
            else if (Camera.main.transform.position == finalCameraPos)
            {
                isStartReset = false;
                OnEnable();
            }
        }
    }
}
