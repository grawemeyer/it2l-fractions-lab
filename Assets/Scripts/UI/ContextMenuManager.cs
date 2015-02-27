using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ContextMenuManager : MonoBehaviour {

    public Text change_color;
    public Text copy;
    public Text findEquivalence;
    public Text findParent;
    public UIButton findParentBtn;
    public UIButton findEquivalenceBtn;
    public Text highlight;
    public InterfaceBehaviour interfaceB;
    public bool isEquivalence;
   
    void Start()
    {
       // if(change_color.gameObject.GetComponents<UIButton>())
      /*  change_color.color = InterfaceBehaviour.Green1;
        copy.color = InterfaceBehaviour.Green1;
        if(null != findParent)
            findParent.color = InterfaceBehaviour.Green1;
        if (null != findEquivalence)
            findEquivalence.color = InterfaceBehaviour.Green1;
        highlight.color = InterfaceBehaviour.Green1;*/

        if (isEquivalence) 
        {
            interfaceB.localizationUtils.AddTranslationText(findParent, "{findParent}");
        }
        else
        {
            interfaceB.localizationUtils.AddTranslationText(change_color, "{change_colour}");
            interfaceB.localizationUtils.AddTranslationText(copy, "{copy}");
            interfaceB.localizationUtils.AddTranslationText(findEquivalence, "{findEquivalence}");
        }
        interfaceB.localizationUtils.AddTranslationText(highlight, "{show_highlight}");
    }

   /* public void ChangeTextPartition(bool isShow) 
    {
        if (isShow)
            interfaceB.localizationUtils.AddTranslationText(findEquivalence, "{hide_partition}");
        else
            interfaceB.localizationUtils.AddTranslationText(findEquivalence, "{show_partition}");
       
    }*/

    public void ChangeTextHighlight(bool isShow)
    {
        if (isShow)
            interfaceB.localizationUtils.AddTranslationText(highlight, "{hide_highlight}");
        else
            interfaceB.localizationUtils.AddTranslationText(highlight, "{show_highlight}");
    }

    public void DisableFindParent(bool isFatherTrashed) 
    {
        if(isFatherTrashed)
            findParentBtn.DisableBtn(false);
        else
            findParentBtn.EnableBtn(false);

    }

    public void DisableFindEquivalence(bool disable)
    {
        if (disable)
        {
            findEquivalenceBtn.DisableBtn(false);
        }
        else
        {
            findEquivalenceBtn.EnableBtn(false);
            findEquivalence.color = new Color(0.17f, 0.32f, 0.09f);
        }

    }

}
