using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SaveAsWindow : MonoBehaviour {

    public Text explanation;
    public InputField title;
    public InterfaceBehaviour interfaceB;

	// Use this for initialization
	void OnEnable () {
       
        if(null != interfaceB.task.title)
            title.text = interfaceB.task.title;
        interfaceB.localizationUtils.AddTranslationText(explanation, "{save_as_explanation}");    
	}

    public void UpdateTitle()
    {
        title.text = interfaceB.task.title;
    }
	
}
