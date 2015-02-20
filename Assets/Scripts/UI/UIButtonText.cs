using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIButtonText : MonoBehaviour {

    public Text text;

    public void ChildPressed() 
    {
        text.gameObject.GetComponent<RectTransform>().localScale = new Vector3(0.8f, 0.8f, 0.8f);
    }

    public void ChildUp()
    {
        text.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

}
