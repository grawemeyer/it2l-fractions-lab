using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PostitButton : MonoBehaviour {

    public List<Sprite> bgSpriteStudent;
    public List<Sprite> bgSpriteTeacher;
    public List<Sprite> iconsStudent;
    public List<Sprite> iconsTeacher;
    public Image icon;
    public Image pin;
    public Text title;
    public Text desc;

    public InterfaceBehaviour interfaceB;

    void Start() 
    {
       // interfaceB = GameObject.FindGameObjectWithTag("Interface").GetComponent<InterfaceBehaviour>();
    }


    public void Initialize() 
    {
        SpriteState st = new SpriteState();
        if (interfaceB.isStudent) 
        {
            st.highlightedSprite = bgSpriteStudent[0];
            st.pressedSprite = bgSpriteStudent[1];
            this.GetComponent<Button>().spriteState = st;
            icon.sprite = iconsStudent[0];
            title.color = InterfaceBehaviour.ClearGreen;
            desc.color = InterfaceBehaviour.ClearGreen;
        }
        else 
        {
            st.highlightedSprite = bgSpriteTeacher[0];
            st.pressedSprite = bgSpriteTeacher[1];
            this.GetComponent<Button>().spriteState = st;
            icon.sprite = iconsTeacher[0];
            title.color = InterfaceBehaviour.Orange;
            desc.color = InterfaceBehaviour.Orange;
        }
        pin.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
        icon.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
        title.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
        desc.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    public void ChildUp()
    {
        
        if (interfaceB.isStudent)
        {
            icon.sprite = iconsStudent[0];
            title.color = InterfaceBehaviour.ClearGreen;
            desc.color = InterfaceBehaviour.ClearGreen;
           
        }
        else
        {
            icon.sprite = iconsTeacher[0];
            title.color = InterfaceBehaviour.Orange;
            desc.color = InterfaceBehaviour.Orange;
        }
        pin.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
        icon.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
        title.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
        desc.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    public void ChildOver() 
      {
          if (interfaceB.isStudent)
          {
              icon.sprite = iconsStudent[1];
          }
          else
          {
              icon.sprite = iconsTeacher[1];
          }
          title.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
          desc.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

      }

      public void ChildPressed()
      {
          if (interfaceB.isStudent)
          {
              icon.sprite = iconsStudent[2];
          }
          else
          {
              icon.sprite = iconsTeacher[2];
          }
          pin.GetComponent<RectTransform>().localScale = new Vector3(0.8f, 0.8f, 0.8f);
          icon.GetComponent<RectTransform>().localScale = new Vector3(0.8f, 0.8f, 0.8f);
          title.GetComponent<RectTransform>().localScale = new Vector3(0.8f, 0.8f, 0.8f);
          desc.GetComponent<RectTransform>().localScale = new Vector3(0.8f, 0.8f, 0.8f);
      }

    

    public void EnableBtn()
    {
        Initialize();
    }

    public void DisableBtn()
    {
        if (interfaceB.isStudent)
        {
            icon.sprite = iconsStudent[3];
        }
        else
        {
            icon.sprite = iconsTeacher[3];
        }

    }

}
