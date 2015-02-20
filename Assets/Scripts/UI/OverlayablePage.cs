using UnityEngine;
using System.Collections;

public class OverlayablePage : MonoBehaviour {

    public GameObject topMenuOperation;
    public GameObject trash;
    public GameObject popupCut;

    public bool isTopMenuOpen;

	// Use this for initialization
	void Start () {
        isTopMenuOpen = false;
        SetTopMenuState(isTopMenuOpen);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void SetTopMenuState(bool visibility) 
    {
        topMenuOperation.SetActive(visibility);
        isTopMenuOpen = visibility;
    }
}
