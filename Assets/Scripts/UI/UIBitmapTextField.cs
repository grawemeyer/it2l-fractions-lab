using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections.Generic;
using SBS.Core;

[AddComponentMenu("UI/UIBitmapTextField")]
[ExecuteInEditMode]
public class UIBitmapTextField : MonoBehaviour
{
    #region Public members
    public UIBitmapFont font;

    [Multiline]
    public string text;

    public Material material = null;
    public Color color = Color.white;
    public TextAnchor alignment = TextAnchor.UpperLeft;
    public bool wordWrap = false;
    public bool dynamic = false;

    public float width = 1.0f;
    public float height = 1.0f;
    public Vector2 pivot = Vector2.zero;

    public bool resizeButton = false;

    public int sortingLayerID;
    public int sortingOrder;
    #endregion

    #region Protected members
    protected bool awakeCalled = false;
    protected List<Tuple<int, float>> lineEndIndices = new List<Tuple<int, float>>(16);
    protected List<Image> spriteRenderers = new List<Image>();
    protected Rect localBounds;
    protected Transform spritesRoot = null;
    protected string prevText = string.Empty;

    protected Animation anim;

#if UNITY_EDITOR
    protected Font prevFont = null;
    protected Color prevColor = Color.white;
    protected TextAnchor prevAlignment = TextAnchor.UpperLeft;
    protected bool prevWordWrap = false;
    protected bool prevDynamic = false;
    protected int prevSortingLayerID;
    protected int prevSortingOrder;

    protected bool HasChanged()
    {
        return
            prevFont != font || prevColor != color || prevAlignment != alignment ||
            prevWordWrap != wordWrap || prevDynamic != dynamic ||
            prevSortingLayerID != sortingLayerID || prevSortingOrder != sortingOrder;
    }
#endif
    #endregion

    #region Protected methods
    protected Vector2 GetMultiLineLocalStart(Vector2 textSize, int lineIndex)
    {
        Asserts.Assert(font != null && wordWrap);
        float lineSize = font.lineHeight;
        Vector2 p = -pivot;
        p.x += width * 0.5f;
        p.y += height * 0.5f;
        switch (alignment)
        {
            case TextAnchor.LowerCenter:
                return new Vector2(
                    p.x - lineEndIndices[lineIndex].Second * 0.5f,
                    p.y - height * 0.5f + textSize.y - lineSize * lineIndex);
            case TextAnchor.LowerLeft:
                return new Vector2(
                    p.x - width * 0.5f,
                    p.y - height * 0.5f + textSize.y - lineSize * lineIndex);
            case TextAnchor.LowerRight:
                return new Vector2(
                    p.x + width * 0.5f - lineEndIndices[lineIndex].Second,
                    p.y - height * 0.5f + textSize.y - lineSize * lineIndex);
            case TextAnchor.MiddleCenter:
                return new Vector2(
                    p.x - lineEndIndices[lineIndex].Second * 0.5f,
                    p.y + textSize.y * 0.5f - lineSize * lineIndex);
            case TextAnchor.MiddleLeft:
                return new Vector2(
                    p.x - width * 0.5f,
                    p.y + textSize.y * 0.5f - lineSize * lineIndex);
            case TextAnchor.MiddleRight:
                return new Vector2(
                    p.x + width * 0.5f - lineEndIndices[lineIndex].Second,
                    p.y + textSize.y * 0.5f - lineSize * lineIndex);
            case TextAnchor.UpperCenter:
                return new Vector2(
                    p.x - lineEndIndices[lineIndex].Second * 0.5f,
                    p.y + height * 0.5f - lineSize * lineIndex);
            case TextAnchor.UpperLeft:
                return new Vector2(
                    p.x - width * 0.5f,
                    p.y + height * 0.5f - lineSize * lineIndex);
            case TextAnchor.UpperRight:
                return new Vector2(
                    p.x + width * 0.5f - lineEndIndices[lineIndex].Second,
                    p.y + height * 0.5f - lineSize * lineIndex);
        }
        return pivot;
    }

    protected Vector2 GetLocalStart(Vector2 textSize)
    {
        Asserts.Assert(font != null && !wordWrap);
        Vector2 p = -pivot;
        p.x += width * 0.5f;
        p.y += height * 0.5f;
        switch (alignment)
        {
            case TextAnchor.LowerCenter:
                return new Vector2(
                    p.x - textSize.x * 0.5f,
                    p.y - height * 0.5f + textSize.y);
            case TextAnchor.LowerLeft:
                return new Vector2(
                    p.x - width * 0.5f,
                    p.y - height * 0.5f + textSize.y);
            case TextAnchor.LowerRight:
                return new Vector2(
                    p.x + width * 0.5f - textSize.x,
                    p.y - height * 0.5f + textSize.y);
            case TextAnchor.MiddleCenter:
                return new Vector2(
                    p.x - textSize.x * 0.5f,
                    p.y + textSize.y * 0.5f);
            case TextAnchor.MiddleLeft:
                return new Vector2(
                    p.x - width * 0.5f,
                    p.y + textSize.y * 0.5f);
            case TextAnchor.MiddleRight:
                return new Vector2(
                    p.x + width * 0.5f - textSize.x,
                    p.y + textSize.y * 0.5f);
            case TextAnchor.UpperCenter:
                return new Vector2(
                    p.x - textSize.x * 0.5f,
                    p.y + height * 0.5f);
            case TextAnchor.UpperLeft:
                return new Vector2(
                    p.x - width * 0.5f,
                    p.y + height * 0.5f);
            case TextAnchor.UpperRight:
                return new Vector2(
                    p.x + width * 0.5f - textSize.x,
                    p.y + height * 0.5f);
        }
        return pivot;
    }

    protected void RenderText()
    {
        if (null == font)
            return;
        if (null == spritesRoot)
        {
            GameObject rootGO = new GameObject(name + "_text");
            //rootGO.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector | HideFlags.DontSave;
            rootGO.transform.parent = this.transform;
            spritesRoot = rootGO.transform;
        }

        localBounds.xMin =  float.MaxValue;
        localBounds.yMin =  float.MaxValue;
        localBounds.xMax = -float.MaxValue;
        localBounds.yMax = -float.MaxValue;

        Vector2 textSize = wordWrap ?
            font.GetMultilineTextSize(text, width, ref lineEndIndices) :
            font.GetTextSize(text);

        int textLen       = text.Length,
            textLast      = textLen - 1,
            counter       = 0,
            spriteCounter = 0,
            lineCounter   = 0;
        float scaleFactor = 1;

        if ((textLen * 50) > width) 
        {
            scaleFactor = width / (textLen * 50);
            //Debug.Log("SCALE FACTOR " + scaleFactor + "WIDTH" + width);
        }
        Vector2 position = wordWrap ? this.GetMultiLineLocalStart(textSize*scaleFactor, lineCounter) : this.GetLocalStart(textSize*scaleFactor);

        for (; counter < textLen; ++counter)
        {
            if (wordWrap && counter == lineEndIndices[lineCounter].First)
            {
                ++lineCounter;
                position = this.GetMultiLineLocalStart(textSize*scaleFactor, lineCounter);
            }

            if (spriteCounter >= spriteRenderers.Count)
            {
                Image sprite;
                if (counter < textLast)
                    sprite = font.DrawCharacterWithKerning(text[counter], text[counter + 1], color, ref position, ref localBounds, spritesRoot, null, gameObject.layer, scaleFactor);
                else
                    sprite = font.DrawCharacter(text[counter], color, ref position, ref localBounds, spritesRoot, null, gameObject.layer, scaleFactor);

                if (sprite != null)
                {
                   /* if (material != null)
                        sprite.sharedMaterial = material;*/
                    /*sprite.sortingLayerID = sortingLayerID;
                    sprite.sortingOrder = sortingOrder;*/

                    sprite.gameObject.SetActive(true);
                    spriteRenderers.Add(sprite);

                    ++spriteCounter;
                }
            }
            else
            {
                Image sprite = spriteRenderers[spriteCounter];
                if (counter < textLast)
                    sprite = font.DrawCharacterWithKerning(text[counter], text[counter + 1], color, ref position, ref localBounds, spritesRoot, sprite, gameObject.layer, scaleFactor);
                else
                    sprite = font.DrawCharacter(text[counter], color, ref position, ref localBounds, spritesRoot, sprite, gameObject.layer, scaleFactor);

                if (sprite != null)
                {
                   /* if (material != null)
                        sprite.sharedMaterial = material;*/

                    /*sprite.sortingLayerID = sortingLayerID;
                    sprite.sortingOrder = sortingOrder;*/
                    sprite.gameObject.SetActive(true);
                    ++spriteCounter;
                }
            }
        }

        for (int i = counter, c = spriteRenderers.Count; i < c; ++i)
            spriteRenderers[i].gameObject.SetActive(false);
        prevText = text;
    }
    #endregion

    #region Public methods
	//----------------------------------------------//
    public void UpdateBitmapText()
    {
#if UNITY_EDITOR
        Profiler.BeginSample("UpdateBitmapText");

        if (!Application.isPlaying && (null == spritesRoot || this.HasChanged()))
            this.RenderText();

        if (Application.isPlaying && null == spritesRoot)
        {
            this.RenderText();

            if (spritesRoot != null)
                spritesRoot.gameObject.SetActive(this.enabled);
        }

        if (null == spritesRoot)
        {
            Profiler.EndSample();
            return;
        }
#else
        if (null == spritesRoot)
        {
            this.RenderText();

            if (spritesRoot != null)
                spritesRoot.gameObject.SetActive(this.enabled);
        }

        if (null == spritesRoot || !dynamic)
            return;
#endif
#if UNITY_EDITOR || !UNITY_METRO
        if (!text.Equals(prevText, StringComparison.InvariantCulture) ||
#else
        if (!text.Equals(prevText, StringComparison.Ordinal) ||
#endif
            (anim != null && anim.isPlaying))
        {
            this.RenderText();
        }

        if (transform.hasChanged)
        {
            spritesRoot.position = transform.position;
            spritesRoot.rotation = transform.rotation;
            spritesRoot.localScale = transform.lossyScale;

            transform.hasChanged = false;
        }

#if UNITY_EDITOR
        Profiler.EndSample();
#endif
    }
    #endregion

    #region Unity callbacks
    void Awake()
    {
#if UNITY_EDITOR
      /*  if (null == Manager<UIManager>.Get())
            return;*/
#endif

        Transform t = transform;
        anim = null;
        while (null == anim)
        {
            anim = t.gameObject.GetComponent<Animation>();
            t = t.parent;
            if (null == t)
                break;
        }

        this.RenderText();

        if (spritesRoot != null)
            spritesRoot.gameObject.SetActive(this.enabled);

        awakeCalled = true;
    }

    void OnEnable()
    {
#if UNITY_EDITOR
       /* if (null == Manager<UIManager>.Get())
            return;*/

        if (!Application.isPlaying && (null == spritesRoot || this.HasChanged()))
            this.RenderText();
#endif

        if (spritesRoot != null)
            spritesRoot.gameObject.SetActive(true);
        else
            this.RenderText();

       // Manager<UIManager>.Get().AddBitmapTextField(this);
    }

    void OnDisable()
    {
#if UNITY_EDITOR
      /*  if (null == Manager<UIManager>.Get())
            return;*/
#endif

       /* if (Manager<UIManager>.Get() != null)
            Manager<UIManager>.Get().RemoveBitmapTextField(this);*/

        if (spritesRoot != null)
            spritesRoot.gameObject.SetActive(false);
    }
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        Gizmos.DrawCube(transform.TransformPoint(-(Vector3)pivot + new Vector3(width * 0.5f, height * 0.5f)), new Vector3(width, height));
    }

    void OnDrawGizmosSelected()
    { }
#endif
    void OnDestroy()
    {
#if UNITY_EDITOR
        if (null == spritesRoot)
            return;
#endif

        GameObject.DestroyImmediate(spritesRoot.gameObject);
        spritesRoot = null;
    }
    #endregion
}
