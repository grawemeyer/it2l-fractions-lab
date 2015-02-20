using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using fractionslab.meshes;
using fractionslab.utils;
using fractionslab.behaviours;

public class UIButton : MonoBehaviour
{

    public List<Sprite> bgSprite;
    public Image background;
    public List<Image> icons;
    public Text text;
    public bool isMenu;

    protected bool disableByTeacher;


    /* protected override void DoStateTransition(SelectionState state, bool instant)
     {
         foreach(Button bt in gameObject.GetComponentsInChildren<Button>())
         {
             switch (state)
             {
                 case SelectionState.Normal:
                   //  bt.DoStateTransition(SelectionState.Normal, instant);
                    // bt.color = new Color(1.0f, 1.0f,1.0f, 1.0f );
                     //Debug.Log("Normal");
                     break;
                 case SelectionState.Disabled:
                    // bt.color *= this.colors.disabledColor;
                    // bt.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
                  //   bt.DoStateTransition(SelectionState.Disabled, instant);
                     //Debug.Log("Disabled");
                     break;
                 case SelectionState.Highlighted:
                    // bt.color = new Color(0.5f, 0.5f, 0.5f, 0.8f);
                   //  bt.DoStateTransition(SelectionState.Highlighted, instant);
                     Debug.Log("Highlighted");
                     break;
                 case SelectionState.Pressed:
                   //  bt.color = new Color(0.6f, 0.6f, 0.6f, 1.0f);
                   //  bt.DoStateTransition(SelectionState.Pressed, instant);
                     Debug.Log("Pressed");
                     break;
             }
        }
        base.DoStateTransition(state, instant);
     }*/

    /*  public void ChildOver() 
      {
          if (null != background)
              background.sprite = bgSprite[1];
          if (null != icon)
             icon.color = new Color(1f, 0.2f, 0.2f, 0.35f);

      }

      public void ChildPressed()
      {
          if (null != background)
              background.sprite = bgSprite[2];
          if (null != icon)
              icon.color = new Color(0.8f, 0.8f, 0.8f, 0.53f);
      }
      */

    public void EnableBtn(bool enableBy)
    {
        if (enableBy || (!enableBy && !disableByTeacher))
        {
            this.GetComponent<Button>().interactable = true;
            foreach (Image icon in icons)
                icon.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            if (!isMenu)
            {
                this.GetComponent<Button>().image.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
            if (null != text)
                text.color = InterfaceBehaviour.Green1;

            disableByTeacher = false;
        }
    }

    public void DisableBtn(bool disableBy)
    {
        if (disableByTeacher)
            return;
        disableByTeacher = disableBy;
        this.GetComponent<Button>().interactable = false;
        foreach (Image icon in icons)
            icon.color = new Color(0.5f, 0.5f, 0.5f, 0.20f);
        /* ColorBlock cl = new ColorBlock();
 cl = this.GetComponent<Button>().colors;
 cl.disabledColor = new Color(1.0f, 1.0f, 1.0f, 0.17f);
 this.GetComponent<Button>().colors = cl;
 SpriteState st = new SpriteState();
 st = this.GetComponent<Button>().spriteState;*/
        if (!isMenu)
            this.GetComponent<Button>().image.color = new Color(1.0f, 1.0f, 1.0f, 0.17f);
        if (null != text)
            text.color = new Color(0.72f, 0.70f, 0.6f);

    }

    public void ScaleIconDown()
    {
        if (this.GetComponent<Button>().interactable)
            foreach (Image icon in icons)
                icon.GetComponent<RectTransform>().localScale = new Vector3(0.8f, 0.8f, 0.8f);
    }

    public void ScaleIconUp()
    {
        if (this.GetComponent<Button>().interactable)
            foreach (Image icon in icons)
                icon.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    void start()
    {
        disableByTeacher = false;
    }
}