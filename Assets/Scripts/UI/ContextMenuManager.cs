using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ContextMenuManager : MonoBehaviour {

    public Text change_color;
    public Text copy;
    public Text partition;
    public Text highlight;
    public InterfaceBehaviour interfaceB;

    void Start()
    {
      /*  if(change_color.gameObject.GetComponents<UIButton>())
        change_color.color = InterfaceBehaviour.Green1;
        copy.color = InterfaceBehaviour.Green1;
        partition.color = InterfaceBehaviour.Green1;
        highlight.color = InterfaceBehaviour.Green1;*/
        interfaceB.localizationUtils.AddTranslationText(change_color, "{change_colour}");
        interfaceB.localizationUtils.AddTranslationText(copy, "{copy}");
        interfaceB.localizationUtils.AddTranslationText(partition, "{show_partition}");
        interfaceB.localizationUtils.AddTranslationText(highlight, "{show_highlight}");
    }

    public void ChangeTextPartition(bool isShow) 
    {
        if (isShow)
            interfaceB.localizationUtils.AddTranslationText(partition, "{hide_partition}");
        else
            interfaceB.localizationUtils.AddTranslationText(partition, "{show_partition}");
       
    }

    public void ChangeTextHighlight(bool isShow)
    {
        if (isShow)
            interfaceB.localizationUtils.AddTranslationText(highlight, "{hide_highlight}");
        else
            interfaceB.localizationUtils.AddTranslationText(highlight, "{show_highlight}");
    }

}
