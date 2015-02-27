using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SBS.Math;
using fractionslab;
using fractionslab.behaviours;
using fractionslab.utils;
using fractionslab.meshes;

public class SetElement : WSElement, IWSElement
{
    #region Internal Data Structures
    public enum Shape
    {
        noShape = -1,
        Heart,
        Moon,
        Star
    }
    #endregion

    #region Public Fields
    public SBSBounds internalBounds;
    public List<int> selectedElement = new List<int>();
    public List<bool> isSelected = new List<bool>();
    public int lastNumerator = 0;
    public int lastDenominator = 0;
    public int lastPartitions = 1;
    public float lastElementScale = 1.0f;
    public float singleWidth;
    public float singleHeight;
    public float maxWidth = 4.5f;
    public float maxHeight = 3.0f;
    public float scalefactorCut = 1.0f;
    public Shape shape;
    protected GameObject prefab;
    #endregion

    #region Protected Fields
    protected GameObject element;

    protected GameObject strokeRect;
    protected Renderer fillRenderer;
    protected Renderer strokeRend;
    protected Color strokeColor = new Color(0.1098f, 0.2431f, 0.0353f);
    #endregion

    #region Unity Callbacks
    void Awake()
    {
        internalBounds = new SBSBounds();
    }
    #endregion

    #region Public Methods
    public override void SetRoot(GameObject r)
    {
        root = transform.parent.gameObject;
    }

    public override SBSBounds GetBounds()
    {
        if (state == ElementsState.Fraction || state == ElementsState.Result || state == ElementsState.Equivalence)
        {
            bounds = new SBSBounds(new Vector3(transform.position.x - ((width) / 2) + ((singleWidth) / 2)-0.20f, transform.position.y, transform.position.z), new SBSVector3(width, height, 0.0f));
            return bounds;
        }
        else if (state == ElementsState.Cut)
        {
            return internalBounds;
        }
        else
        {
            bounds.Reset();
            for (int i = 0; i < transform.childCount; i++)
            {
                if (null != transform.GetChild(i).GetComponent<EmptyElement>())
                    bounds.Encapsulate(transform.GetChild(i).GetComponent<EmptyElement>().GetBounds());
            }
            return bounds;
        }
    }

    public override void Draw(int zIndex)
    {
        base.Draw(zIndex);
        Vector3 pos = transform.position;
        pos.z = zIndex;
        transform.position = pos;

        if (partitions != lastPartitions)
        {
            UpdatePartitions(partitions);
            UpdateDenominatorByPartition(partitions);
            UpdateNumeratorByPartition(partitions, lastPartitions);
            lastPartitions = partitions;
            lastNumerator = partNumerator;
        }

        if (denominator != lastDenominator)
        {
            UpdateDenominator(denominator);
            lastDenominator = denominator;
        }

        if (partitions == 1)
        {
            if (numerator != lastNumerator)
            {
                UpdateNumerator(numerator);
                lastNumerator = numerator;
            }
        }
        else
        {
            if (partNumerator != lastNumerator)
            {
                UpdateNumerator(partNumerator);
                lastNumerator = partNumerator;
            }
        }

        if (lastElementScale != elementScale)
        {
            gameObject.transform.localScale = new Vector3(elementScale, elementScale, 1.0f);
            lastElementScale = elementScale;
        }

        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).SendMessage("Draw", zIndex, SendMessageOptions.DontRequireReceiver);

        UpdateElements(lastDenominator, lastNumerator);
    }

    public override bool CheckPartition()
    {
        {
            if (partitions > 1)
            {
                if (partNumerator % partitions > 0)
                {
                    return false;
                }
                else
                {
                    for (int i = 0; i < partDenominator; i += partitions)
                        if (isSelected[i])
                        {
                            for (int j = 0; j < partitions; j++)
                                if (!isSelected[i + j])
                                    return false;
                        }
                        else
                        {
                            for (int j = 0; j < partitions; j++)
                                if (isSelected[i + j])
                                    return false;

                        }
                }
            }
            return true;
        }
    }

    #endregion

    #region Messages
    void SetContentColor(Color c)
    {
        color = c;
        Draw(zIndex);
    }

    void Cut()
    {
        selectedElement.Sort();
        selectedElement.Reverse();
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name.Equals("background"))
            {
                GameObject bg = transform.GetChild(i).gameObject;
                bg.transform.parent = null;
                Destroy(bg);
            }

            if (transform.GetChild(i).name.Equals("line_root"))
            {
                GameObject bg = transform.GetChild(i).gameObject;
                bg.transform.parent = null;
                Destroy(bg);
            }
        }
        root.GetComponent<RootElement>().UpdateGraphics();
    }

    void SetSize(Vector2 size)
    {
        Width = size.x;
        Height = size.y;
    }

    void SetMode(InteractionMode mode) 
    {
        base.SetMode(mode);
    }

    void UpdateSize()
    {
        Width = root.GetComponent<RootElement>().width;
        Height = root.GetComponent<RootElement>().height;
    }

    void Initialize()
    {
        shape = root.GetComponent<RootElement>().shape;
        lastNumerator = numerator;
        lastDenominator = denominator;
        lastPartitions = partitions;
        prefab = Workspace.Instance.setSourceHeart;
        switch (type)
        {
            case ElementsType.HeartSet:
                prefab = Workspace.Instance.setSourceHeart; 
                break;
            case ElementsType.MoonSet:
                prefab = Workspace.Instance.setSourceMoon;
                break;
            case ElementsType.StarSet:
                prefab = Workspace.Instance.setSourceStar;
                break;
        }

        element = GameObject.Instantiate(prefab) as GameObject;

        element.name = "elem_0";
        element.transform.parent = transform;
        element.AddComponent<EmptyElement>();
        element.BroadcastMessage("SetMode", mode, SendMessageOptions.DontRequireReceiver);
        element.BroadcastMessage("SetType", type, SendMessageOptions.DontRequireReceiver);
        element.BroadcastMessage("SetSize", new Vector2(width, height), SendMessageOptions.DontRequireReceiver);

        for (int i = 0; i < element.transform.childCount; i++)
        {
            if (element.transform.GetChild(i).tag == "filling")
                fillRenderer = element.transform.GetChild(i).renderer;
        }

        foreach (MeshRenderer mr in element.GetComponentsInChildren<MeshRenderer>(true))
        {
            if (mr.gameObject.tag == "stroke")
                mr.gameObject.SetActive(false);
        }

       
        fillRenderer.material.color = Workspace.Instance.white;
        singleWidth = element.GetComponent<EmptyElement>().width;
        singleHeight = element.GetComponent<EmptyElement>().height;
        width = element.GetComponent<EmptyElement>().width;
        element.transform.position = transform.TransformPoint(new Vector3(0, 0, 0));
        InitializeStroke();

        if (denominator > 0)
        {
            UpdateDenominator(partDenominator);
            UpdateNumerator(partNumerator);
            foreach (MeshRenderer mr in element.GetComponentsInChildren<MeshRenderer>(true))
            {
                if (mr.gameObject.tag == "stroke")
                    mr.gameObject.SetActive(true);
            }
        }
    }


    protected void InitializeStroke()
    {
        string strokeName = "strokeRect";
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name.Equals(strokeName))
            {
                Destroy(transform.GetChild(i).gameObject);
                strokeRect = null;
            }
        }
        if (null == strokeRect && state != ElementsState.Cut)
        {
            strokeRect = new GameObject("strokeRect");
            strokeRect.transform.parent = transform;
            strokeRect.transform.position = transform.TransformPoint(Vector3.zero);
            strokeRect.AddComponent<MeshElement>();
            strokeRect.SendMessage("Initialize");
            strokeRect.SendMessage("SetMode", mode);
            strokeRect.SendMessage("SetType", ElementsType.Set);
            strokeRect.SendMessage("SetSize", new Vector2(width * elementScale, height * elementScale));
            strokeRect.SendMessage("SetStrokeDist", 0.2f);
            strokeRect.SendMessage("SetStrokeWidth", 0.03f);
        }
    }


    void ClickElement(int index)
    {
        if (isSelected[index])
        {
            isSelected[index] = false;
            selectedElement.Remove(index);
            partNumerator--;
            if (partNumerator < 0)
                partNumerator = 0;
            numerator = partNumerator / partitions;
        }
        else
        {
            isSelected[index] = true;
            selectedElement.Add(index);
            partNumerator++;
            numerator = partNumerator / partitions;
        }
        if (partitions == 1)
            lastNumerator = numerator;
        else
            lastNumerator = partNumerator;
        root.transform.parent.GetComponent<RootElement>().UpdateByChildren();
    }

    void UpdateNumerator(int value)
    {
       // Debug.Log("UpdateNumerator " + root.transform.parent.name);

        value = Mathf.Clamp(value, 0, isSelected.Count);

        int prevNum = 0;
        for (int i = 0; i < isSelected.Count; i++)
            if (isSelected[i]) prevNum++;

        int diff = value - prevNum;
        if (diff < 0)
        {
            for (int i = 0; i < Mathf.Abs(diff); i++)
            {
                int lastIdx = selectedElement.Count - 1;
                if (lastIdx >= 0.0f)
                {
                    int removeIdx = selectedElement[lastIdx];
                    isSelected[removeIdx] = false;
                    selectedElement.RemoveAt(lastIdx);
                }
            }
        }
        else if (diff > 0)
        {
            for (int i = 0; i < isSelected.Count; i++)
            {
                if (!isSelected[i])
                {
                    isSelected[i] = true;
                    diff--;
                    selectedElement.Add(i);
                }

                if (diff <= 0)
                    break;
            }
        }


        lastNumerator = value;
        UpdateElements(lastDenominator, lastNumerator);
    }

    void UpdateNumeratorByPartition(int partition, int lastPartition)
    {

       /* selectedElement.RemoveAll(item => item % lastPartitions != 0);

        if (lastPartition > 0)
        {
            for (int i = 0; i < selectedElement.Count; i++)
                selectedElement[i] /= lastPartition;
        }

        for (int i = 0; i < selectedElement.Count; i++)
            selectedElement[i] *= partition;

        List<int> tmp = new List<int>(selectedElement);

        for (int i = 0; i < tmp.Count; i++)
            for (int j = 0; j < Mathf.Min(partNumerator - 1, partition - 1); j++)
                selectedElement.Add(tmp[i] + (j + 1));

        for (int i = 0; i < isSelected.Count; i++)
            isSelected[i] = selectedElement.Contains(i);

        if (selectedElement.Count > 0 && selectedElement.Count < partNumerator)
        {
            for (int i = 0; i < isSelected.Count; i++)
                if (!isSelected[i] && selectedElement.Count < partNumerator)
                {
                    isSelected[i] = true;
                    selectedElement.Add(i);
                }
        }*/

        if (state != ElementsState.Equivalence)
            return;

       // selectedElement.RemoveAll(item => item >= numerator);   
        List<int> tmp = new List<int>();

        /*if (lastPartition == 0)
            step = partition;
        else
            step = lastPartition;*/

        for (int i = 0; i < selectedElement.Count; i = i+(lastPartition))
        {
            tmp.Add(selectedElement[i]);
        }

        for (int i = 0; i < tmp.Count; i++)
            tmp[i] /= (lastPartition);

        //RootElement superRoot = root.transform.parent.GetComponent<RootElement>().parentEqRef.GetComponent<RootElement>();

       /* foreach (SetElement SetEle in superRoot.GetComponentsInChildren<SetElement>())
        {
            if (SetEle.root.name == root.name)
            {
                tmp = new List<int>(SetEle.selectedElement);
            }
        }*/

        selectedElement = new List<int>();

        for (int i = 0; i < tmp.Count; i++)
        {
            for (int j = (tmp[i] * partitions); j < ((tmp[i] * partitions) + partition); j++)
            {
                selectedElement.Add(j);
            }
        }

        for (int i = 0; i < isSelected.Count; i++)
            isSelected[i] = selectedElement.Contains(i);

        UpdateElements(lastDenominator, lastNumerator);
    }

    /*todo*/
    void UpdateDenominatorByPartition(int partition)
    {
        isSelected.Clear();
        for (int i = 0; i < denominator; i++)
        {
            for (int j = 0; j < partition; j++)
            {
                isSelected.Add(false);
            }
        }

        for (int i = 0; i < selectedElement.Count; i++)
        {
            if (selectedElement[i] < isSelected.Count)
                isSelected[selectedElement[i]] = true;
        }
        UpdateElements(lastDenominator, lastNumerator);
    }

    public void DecreaseCutNumeratorPartitions()
    {
        if (partitions > 1)
        {
            partNumerator = partNumerator-partitions;
            UpdateNumerator(partNumerator);

        }
        else
        {
            numerator--;
            UpdateNumerator(numerator);
        }
        UpdateElements(denominator, numerator);
        // Draw(0);
    }

    public void DecreaseCutNumerator()
    {


        if (partitions > 1)
        {
            partNumerator--;
            UpdateNumerator(partNumerator);

        }
        else
        {
            numerator--;
            UpdateNumerator(numerator);
        }
        UpdateElements(denominator, numerator);
        // Draw(0);
    }

    void UpdateDenominator(int value)
    {
        if (value > 0)
        {
            foreach (MeshRenderer mr in element.GetComponentsInChildren<MeshRenderer>(true))
            {
                if (mr.gameObject.tag == "stroke")
                    mr.gameObject.SetActive(true);
            }
            int prevDen = isSelected.Count / partitions;
            int diff = (value - prevDen);
            if (diff < 0)
            {
                int startSlice = partitions * (denominator - 1);
           
                for (int j = 0; j < partitions; j++)
                {
                    int lastPos = startSlice + j;
                    if (isSelected[lastPos])
                    {
                        int freeIdx = isSelected.FindIndex(0, p => p.Equals(false));
                        if (freeIdx >= 0)
                            isSelected[freeIdx] = true;
                    }
                    isSelected.RemoveAt(lastPos);
                }
            }
            else
            {
                for (int j = 0; j < partitions; j++)
                {
                    for (int i = 0; i < Mathf.Abs(diff); i++)
                    {
                        isSelected.Add(false);
                    }
                }
            }

            int n = 0;
            selectedElement.Clear();
            for (int k = 0; k < isSelected.Count; k++)
            {
                if (isSelected[k])
                {
                    n++;
                    selectedElement.Add(k);
                }
            }
        }
        else if (value == 0)
        {
            isSelected.Clear();
        }

        width = singleWidth * value;

        lastDenominator = value;
        UpdateElements(lastDenominator, lastNumerator);
        if (null != root.transform.parent.GetComponent<RootElement>())
            root.transform.parent.GetComponent<RootElement>().BroadcastMessage("UpdateWidth");
    }

    /*todo*/
    void UpdatePartitions(int value)
    {
 
    }

    void OnClicked(Vector3 position)
    {
        if (mode == InteractionMode.Initializing)
            return;
        int elementCount = 0;
        Vector3 localPos;
        RootElement superRoot = root.transform.parent.GetComponent<RootElement>();

        if (state != ElementsState.Equivalence)
            localPos = root.transform.InverseTransformPoint(position);
        else
            localPos = position;

        if (state != ElementsState.Equivalence)
        {
            if (null != superRoot.equivalences && superRoot.equivalences.Count > 0)
            {
                foreach (GameObject eq in superRoot.equivalences)
                {
                    foreach (SetElement st in eq.GetComponentsInChildren<SetElement>()) 
                    {
                        if(st.root.name == root.name)
                            st.SendMessage("OnClicked", localPos, SendMessageOptions.DontRequireReceiver);
                    }         
                }
            }
        }

        elementCount = 0;
        //Debug.Log("name "+ root.transform.parent.name  + "gameObject.transform.childCount " + gameObject.transform.childCount);
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.name.StartsWith("elem"))
            {
                SBSBounds meshBounds = child.GetComponent<EmptyElement>().GetBounds();
                meshBounds.max = transform.InverseTransformPoint(meshBounds.max);
                meshBounds.min = transform.InverseTransformPoint(meshBounds.min);
                position.z = meshBounds.max.z;

                if (meshBounds.ContainsPointXY(localPos))
                {
                    if (state != ElementsState.Equivalence || (state == ElementsState.Equivalence && partitions == 1))
                    {
                        //Debug.Log(" i " + i);
                        ClickedOnSingleElement(elementCount, i);
                    }
                    else
                    {
                        for (int j = ((Mathf.FloorToInt((i-1) / partitions)) * partitions)+1; j <= ((Mathf.FloorToInt(i / partitions) * partitions) + partitions); j++)
                        {
                            //Debug.Log("j " + j + " i " + i + " isSelect.count " + isSelected.Count);
                            ClickedOnSingleElement(j, j);
                        }

                    }
                    Draw(zIndex);
                    break;
                }
                elementCount++;
            }
        }
    }

    private void ClickedOnSingleElement(int index, int i)
    {
        Renderer tmpRender = null;
        Transform child = transform.GetChild(i);
       // Debug.Log("ISSELECTED " +isSelected.Count);
        for (int j = 0; j < child.transform.childCount; j++)
        {
            if (child.transform.GetChild(j).tag == "filling")
                tmpRender = child.transform.GetChild(j).renderer;
        }

        if (!isSelected[i-1])
        {
            tmpRender.material.color = color;
        }
        else
            tmpRender.material.color = Workspace.Instance.white;

        ClickElement(i-1);
    }
    #endregion

    #region Protected Methods

    protected void UpdateElements(int den, int num)
    {
        //Debug.Log("UpdateElements " + root.name);

        internalBounds.Reset();
        float scalefactorW = 1;
        float scalefactorH = 1;
        float scalefactor = 1;
        //Debug.Log("UpdateElements before " + mode);
        if (mode != InteractionMode.Initializing)
        {
           // Debug.Log("UpdateElements after" + mode);
            foreach (Transform child in transform)
            {
                if (!child.gameObject.name.StartsWith("strokeRect"))
                    Destroy(child.gameObject);
                if (state == ElementsState.Cut && child.gameObject.name.StartsWith("strokeRect"))
                    Destroy(child.gameObject);
            }
            if (denominator * singleWidth > maxWidth)
            {
                scalefactorW = (singleWidth) / ((denominator * singleWidth) * singleWidth / maxWidth);
                width = maxWidth;
            }
            else
            {
                scalefactorW = 1;
            }

            if (partitions * singleHeight > maxHeight)
            {
                scalefactorH = singleHeight / ((partitions * singleHeight) / maxHeight);
                height = maxHeight;
            }
            else
            {
                scalefactorH = 1;
            }
            scalefactor = Mathf.Min(scalefactorH, scalefactorW);
            height = singleHeight * partitions * scalefactor;

            float startingY = 0;
            int countEle = 0;

            if (partitions > 1)
                startingY = ((height) / 2) - (0.5f * (scalefactor));

            scalefactor *= scalefactorCut;

            for (int k = 0; k < den; k++)
            {
                for (int j = 0; j < partitions; j++)
                {
                    if (null == prefab)
                    {
                        switch (type)
                        {
                            case ElementsType.HeartSet:
                                prefab = Workspace.Instance.setSourceHeart;
                                break;
                            case ElementsType.MoonSet:
                                prefab = Workspace.Instance.setSourceMoon;
                                break;
                            case ElementsType.StarSet:
                                prefab = Workspace.Instance.setSourceStar;
                                break;
                        }
                    }

                    element = GameObject.Instantiate(prefab) as GameObject;
                    element.transform.localScale = element.transform.lossyScale * scalefactor;
                    element.name = "elem_" + (k + (den * j));
                   /* element.AddComponent<Rigidbody>();
                    element.GetComponent<Rigidbody>().useGravity = false;
                    element.gameObject.isStatic = true;*/

                    element.transform.parent = transform;
                    element.transform.position = transform.TransformPoint(new Vector3(-((den - 1) * (singleWidth * scalefactorW)) + (k * (singleWidth * scalefactorW)), startingY - (j * (1.0f * (scalefactor / scalefactorCut))), 0));

                    for (int i = 0; i < element.transform.childCount; i++)
                    {
                        if (element.transform.GetChild(i).tag == "filling")
                            fillRenderer = element.transform.GetChild(i).renderer;
                    }
                    if (!isSelected[countEle++])
                    {
                        if (state != ElementsState.Cut)
                            fillRenderer.material.color = Workspace.Instance.white;
                        else
                        {
                            fillRenderer.material.color = new Color(1f, 1f, 1f, 0.0f);
                        }

                    }
                    else
                    {
                        if (mode == InteractionMode.Freeze)
                            fillRenderer.material.color = color + new Color(0.4f, 0.4f, 0.4f);
                        else
                        {
                            fillRenderer.material.color = color;
                            internalBounds.Encapsulate(new SBSBounds(element.transform.TransformPoint(Vector3.zero), new SBSVector3((1.5f * scalefactor), (1.0f * scalefactor), 0.0f)));
                        }
                    }

                    if (state == ElementsState.Cut)
                    {
                        foreach (MeshRenderer mr in element.GetComponentsInChildren<MeshRenderer>(true))
                        {
                            if (mr.gameObject.tag == "stroke")
                                mr.gameObject.SetActive(false);
                        }
                    }

                    Color strokeC = strokeColor;
                    if (mode == InteractionMode.Freeze)
                        strokeC += new Color(0.4f, 0.4f, 0.4f);

                    foreach (MeshRenderer mr in element.GetComponentsInChildren<MeshRenderer>(true))
                    {
                        if (mr.gameObject.tag == "stroke")
                            mr.material.color = strokeC;
                    }
                    element.AddComponent<EmptyElement>();
                    element.BroadcastMessage("SetMode", mode , SendMessageOptions.DontRequireReceiver);
                    element.BroadcastMessage("SetType", type, SendMessageOptions.DontRequireReceiver);
                    element.BroadcastMessage("SetSize", new Vector2((singleWidth * scalefactor), (height) / partitions), SendMessageOptions.DontRequireReceiver);
                }
            }
        }
        if (state != ElementsState.Cut)
        {
            if (null == strokeRect)
                InitializeStroke();
            strokeRect.SendMessage("SetSize", new Vector2(width * elementScale, height * elementScale));
            strokeRect.transform.position = gameObject.transform.TransformPoint(new Vector3(-(width * (0.5f)), 0.0f, 0.0f));
            strokeRect.transform.position = new Vector3(strokeRect.transform.position.x + (0.55f * scalefactorW), strokeRect.transform.position.y, strokeRect.transform.position.z);
        }
        root.GetComponent<RootElement>().width = width * elementScale;
        root.GetComponent<RootElement>().height = height * elementScale;
        Workspace.Instance.RemoveEmptyChildren(root);
    }
    #endregion
}