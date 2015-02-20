using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ActionMenuManager : MonoBehaviour {

    public Text add;
    public Text subtract;
    public InterfaceBehaviour interfaceB;

	void Start () {
        interfaceB.localizationUtils.AddTranslationText(add, "{join}");
        interfaceB.localizationUtils.AddTranslationText(subtract, "{taking_away}");

	}
	
}
