using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SBS.Core;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class UIBitmapFont : ScriptableObject
{
    #region Consts
    protected static char[] lineEndingChars = { '\n', '\t', ' ', '.', ',', ';', ':', '-' };
    #endregion

    #region Serializable data
    [Serializable]
#if UNITY_4_5
    public struct Character
#else
    public class Character
#endif
    {
        public int code;
        public Sprite sprite;
        public Vector2 offset;
        public float width;
        public float height;
        public float xAdvance;
    }

    [Serializable]
#if UNITY_4_5
    public struct Kerning
#else
    public class Kerning
#endif
    {
        public int code0;
        public int code1;
        public float kerning;
    }
    #endregion

    #region Public members
    public Character[] characters;
    public Kerning[] kernings;
    public float lineHeight;
    public float extraSpacing;
    #endregion

    #region Protected members
    protected Dictionary<int, Character> charsDict = null;
    protected Dictionary<int, Kerning[]> kernHash = null;
    protected List<float> charsSize = null;
    #endregion

    #region Protected methods
    protected void Initialize()
    {
        if (charsDict != null)
            return;

        charsDict = new Dictionary<int, Character>();
        kernHash = new Dictionary<int, Kerning[]>();
        charsSize = new List<float>(16);

        foreach (Character c in characters)
            charsDict.Add(c.code, c);

        foreach (Kerning k in kernings)
        {
            int hash = k.code0 + k.code1;
            Kerning[] kernArray;

            if (kernHash.TryGetValue(hash, out kernArray))
            {
                int len = kernArray.Length;
                Array.Resize<Kerning>(ref kernArray, len + 1);
                kernArray[len] = k;
            }
            else
            {
                kernArray = new Kerning[1];
                kernArray[0] = k;

                kernHash.Add(hash, kernArray);
            }
        }
    }
    #endregion

    #region Public methods
    public bool HasCharacter(char c)
    {
        this.Initialize();
        return charsDict.ContainsKey(c);
    }

    public bool TryGetCharacter(char c, out Character data)
    {
        this.Initialize();
        return charsDict.TryGetValue(c, out data);
    }

    public Vector2 GetMultilineTextSize(string text, float maxWidth, ref List<Tuple<int, float>> lineEndIndices)
    {
        this.Initialize();

        Vector2 size = new Vector2(0.0f, lineHeight);

        int last = text.Length - 1;
        Character charDesc;
        Kerning[] kernArray;
        float maxLineSize = 0.0f;
        char c0, c1;

        charsSize.Clear();
        lineEndIndices.Clear();

        for (int i = 0; i <= last; ++i)
        {
            c0 = text[i];

            if (charsDict.TryGetValue(c0, out charDesc))
            {
                float kerning = 0.0f;
                if (i < last)
                {
                    c1 = text[i + 1];
                    if (kernHash.TryGetValue((int)c0 + (int)c1, out kernArray))
                    {
                        foreach (Kerning k in kernArray)
                        {
                            if (c0 == k.code0 && c1 == k.code1)
                            {
                                kerning = k.kerning;
                                break;
                            }
                        }
                    }
                }

                size.x += (-charDesc.offset.x + charDesc.xAdvance + extraSpacing + kerning);
            }

            charsSize.Add(size.x);

            if (size.x > maxWidth || '\n' == c0)
            {
                int lastLineEndIndex = lineEndIndices.Count > 0 ? lineEndIndices[lineEndIndices.Count - 1].First : 0;
                int newLineEndIndex = text.LastIndexOfAny(lineEndingChars, i, i - lastLineEndIndex);

#if UNITY_EDITOR
                int count = 0;
                while (newLineEndIndex >= 0 && charsSize[newLineEndIndex] > maxWidth && count++ < 100)
#else
                while (newLineEndIndex >= 0 && charsSize[newLineEndIndex] > maxWidth)
#endif
                {
#if UNITY_EDITOR
                    Debug.Log("newLineEndIndex: " + newLineEndIndex + ", lastLineEndIndex: " + lastLineEndIndex);
#endif
                    newLineEndIndex = text.LastIndexOfAny(lineEndingChars, newLineEndIndex - 1, newLineEndIndex - 1 - lastLineEndIndex);
                }
                if (-1 == newLineEndIndex)
                {
                    maxLineSize = Mathf.Max(maxLineSize, size.x);
                }
                else
                {
                    float sizeOffset = charsSize[newLineEndIndex];

                    bool retOnSpace = (' ' == text[newLineEndIndex]);
                    ++newLineEndIndex;
                    if (' ' == text[newLineEndIndex])
                    {
                        ++newLineEndIndex;
                        retOnSpace = true;
                    }

                    lineEndIndices.Add(new Tuple<int, float>(newLineEndIndex, (retOnSpace ? charsSize[Mathf.Max(0, newLineEndIndex - 1)] : sizeOffset)));

                    ++newLineEndIndex;
                    for (; newLineEndIndex < i; ++newLineEndIndex)
                        charsSize[newLineEndIndex] -= sizeOffset;

                    size.x -= sizeOffset;
                    size.y += lineHeight;
                }
            }
        }

        lineEndIndices.Add(new Tuple<int, float>(last + 1, size.x));
        size.x = Mathf.Max(maxLineSize, maxWidth);

        return size;
    }

    public Vector2 GetTextSize(string text)
    {
        this.Initialize();

        Vector2 size = new Vector2(0.0f, lineHeight);

        int last = text.Length - 1;
        Character charDesc;
        Kerning[] kernArray;
        char c0, c1;

        for (int i = 0; i <= last; ++i)
        {
            c0 = text[i];
            if (charsDict.TryGetValue(c0, out charDesc))
            {
                float kerning = 0.0f;
                if (i < last)
                {
                    c1 = text[i + 1];
                    if (kernHash.TryGetValue((int)c0 + (int)c1, out kernArray))
                    {
                        foreach (Kerning k in kernArray)
                        {
                            if (c0 == k.code0 && c1 == k.code1)
                            {
                                kerning = k.kerning;
                                break;
                            }
                        }
                    }
                }

                size.x += (-charDesc.offset.x + charDesc.xAdvance + extraSpacing + kerning);
            }
        }

        return size;
    }

    public Image DrawCharacter(char c0, Color color, ref Vector2 position, ref Rect bounds, Transform root, Image spriteRenderer, int layer, float scaleFactor)
    {
        this.Initialize();
        Character charDesc;
        if (charsDict.TryGetValue(c0, out charDesc))
        {
            Rect charRect = new Rect(position.x, position.y, charDesc.width*scaleFactor, charDesc.height*scaleFactor);

            if (null == spriteRenderer)
            {
                spriteRenderer = new GameObject("UIBitmapFontCharacter", typeof(Image)).GetComponent<Image>();
                //spriteRenderer.gameObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector | HideFlags.DontSave;
                spriteRenderer.gameObject.layer = layer;
            }

            Vector2 spriteOffset = (charDesc.sprite.bounds.size * 0.5f) * scaleFactor;
            spriteOffset.y = -spriteOffset.y;

            spriteRenderer.sprite = charDesc.sprite;
            spriteRenderer.color = color;
            spriteRenderer.GetComponent<RectTransform>().sizeDelta = new Vector2((50.0f) * scaleFactor, (60.0f) * scaleFactor);
            spriteRenderer.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.0f);
            spriteRenderer.transform.parent = root;
            spriteRenderer.transform.localPosition = position - charDesc.offset*scaleFactor + spriteOffset;
            spriteRenderer.transform.localRotation = Quaternion.identity;
            spriteRenderer.transform.localScale = Vector3.one;

            position.x += (charDesc.xAdvance + extraSpacing)*scaleFactor;

            bounds.xMin = Mathf.Min(bounds.xMin, charRect.xMin);
            bounds.yMin = Mathf.Min(bounds.yMin, charRect.yMin);
            bounds.xMax = Mathf.Max(bounds.xMax, charRect.xMax);
            bounds.yMax = Mathf.Max(bounds.yMax, charRect.yMax);

            return spriteRenderer;
        }
        else
        {
            return null;
        }
    }

    public Image DrawCharacterWithKerning(char c0, char c1, Color color, ref Vector2 position, ref Rect bounds, Transform root, Image spriteRenderer, int layer, float scaleFactor)
    {
        this.Initialize();

        Character charDesc;
        if (charsDict.TryGetValue(c0, out charDesc))
        {
            Rect charRect = new Rect(position.x, position.y, charDesc.width*scaleFactor, charDesc.height*scaleFactor);
            if (null == spriteRenderer)
            {
                spriteRenderer = new GameObject("UIBitmapFontCharacter", typeof(Image)).GetComponent<Image>();
                //spriteRenderer.gameObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector | HideFlags.DontSave;
                spriteRenderer.gameObject.layer = layer;
            }

            Vector2 spriteOffset = (charDesc.sprite.bounds.size * 0.5f) * scaleFactor;
            spriteOffset.y = -spriteOffset.y;

            spriteRenderer.sprite = charDesc.sprite;
            spriteRenderer.color = color;
            spriteRenderer.GetComponent<RectTransform>().sizeDelta = new Vector2((50.0f) * scaleFactor, (60.0f) * scaleFactor);
            spriteRenderer.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.0f);
            spriteRenderer.transform.parent = root;
            spriteRenderer.transform.localPosition = position - charDesc.offset + spriteOffset;
            spriteRenderer.transform.localRotation = Quaternion.identity;
            spriteRenderer.transform.localScale = Vector3.one;

            float kerning = 0.0f;
            Kerning[] kernArray;
            if (kernHash.TryGetValue((int)c0 + (int)c1, out kernArray))
            {
                foreach (Kerning k in kernArray)
                {
                    if (c0 == k.code0 && c1 == k.code1)
                    {
                        kerning = k.kerning;
                        break;
                    }
                }
            }

            position.x += (charDesc.xAdvance + extraSpacing + kerning)*scaleFactor;

            bounds.xMin = Mathf.Min(bounds.xMin, charRect.xMin);
            bounds.yMin = Mathf.Min(bounds.yMin, charRect.yMin);
            bounds.xMax = Mathf.Max(bounds.xMax, charRect.xMax);
            bounds.yMax = Mathf.Max(bounds.yMax, charRect.yMax);

            return spriteRenderer;
        }
        else
        {
            return null;
        }
    }
    #endregion

    #region Unity callbacks
    #endregion

#if UNITY_EDITOR
    [MenuItem("UI/Create Void Font")]
    public static void CreateNew()
    {
        //Debug.Log("Create new font");
        UIBitmapFont data = ScriptableObject.CreateInstance<UIBitmapFont>();
        AssetDatabase.CreateAsset(data, "Assets/NewFont.asset");
        AssetDatabase.SaveAssets();
    }
#endif
}
