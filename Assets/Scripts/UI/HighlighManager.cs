using UnityEngine;
using System.Collections;

public class HighlighManager : MonoBehaviour {

    protected RootElement root;

    public void Initialize(RootElement _root, Vector2 size) 
    {
        root = _root;
        BoxCollider bx = GetComponent<BoxCollider>();
        bx.size = size;
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
        //if (root.GetComponent<RootElement>().hasDragged)
        root.GetComponent<RootElement>().OnMouseUp();
        root.GetComponent<RootElement>().inputByChild = false;
    }

    void OnMouseOver()
    {
        root.GetComponent<RootElement>().SendMessage("OnMouseOver", SendMessageOptions.DontRequireReceiver);
    }

}
