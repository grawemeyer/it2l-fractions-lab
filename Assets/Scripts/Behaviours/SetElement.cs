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
        Heart = 0,
        Moon,
        Star
    }
    #endregion

    #region Public Fields
    public List<int> selectedElement = new List<int>();
    public List<bool> isSelected = new List<bool>();
    public int lastNumerator = 0;
    public int lastDenominator = 0;
    public int lastPartitions = 1;
    public float lastElementScale = 1.0f;
    public float singleWidth;
    #endregion

    #region Protected Fields
    protected GameObject element;
    #endregion

    #region Unity Callbacks
    void Awake()
    {
    }
    #endregion

    #region Public Methods
    public override void SetRoot(GameObject r)
    {
        root = transform.parent.gameObject;
    }

    public override SBSBounds GetBounds()
    {
        if (state == ElementsState.Fraction || state == ElementsState.Result)
        {
            bounds = new SBSBounds(new Vector3(transform.position.x - (width / 2) + (singleWidth / 2), transform.position.y, transform.position.z), new SBSVector3(width, height, 0.0f));
            return bounds;
        }
        else
        {
            bounds.Reset();
            for (int i = 0; i < transform.childCount; i++)
            {
                if (null != transform.GetChild(i).GetComponent<WSElement>())
                    bounds.Encapsulate(transform.GetChild(i).GetComponent<WSElement>().GetBounds());
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

    /*todo*/
    public override bool CheckPartition()
    {
        /*if (partitions > 1)
        {
            if (partNumerator % partitions > 0)
            {
                return false;
            }
            else
            {
                for (int i = 0; i < partNumerator; i += partitions)
                    if (elementsSet[i])
                    {
                        for (int j = 0; j < partitions; j++)
                            if (!elementsSet[i + j])
                                return false;
                    }
            }
        }*/
        return true;
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
        //UpdateSlices();
        root.GetComponent<RootElement>().UpdateGraphics();
    }

    void SetSize(Vector2 size)
    {
        Width = size.x;
        Height = size.y;
    }

    void Initialize()
    {
        lastNumerator = numerator;
        lastDenominator = denominator;
        lastPartitions = partitions;
        element = GameObject.Instantiate(Workspace.Instance.setSource) as GameObject;
        element.name = "elem_0";
        element.transform.parent = transform;
        element.AddComponent<EmptyElement>();
        element.SendMessage("SetMode", mode);
        element.SendMessage("SetType", type);
        element.SendMessage("SetSize", new Vector2(width, height));
        element.renderer.material.color = Workspace.Instance.white;
        singleWidth = element.GetComponent<EmptyElement>().width;
        width = element.GetComponent<EmptyElement>().width;
        element.transform.position = transform.TransformPoint(new Vector3(0, 0, 0));
        if (denominator > 0)
        {
            UpdateDenominator(partDenominator);
            UpdateNumerator(partNumerator);
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

      /*  if (isSelected)
        {
            partNumerator--;
            if (partNumerator < 0)
                partNumerator = 0;
            numerator = partNumerator / partitions;
        }
        else
        {
            partNumerator++;
            numerator = partNumerator / partitions;            
        }*/

        if (partitions == 1)
            lastNumerator = numerator;
        else
            lastNumerator = partNumerator;
        Workspace.Instance.ElementOnFocus.GetComponent<RootElement>().UpdateByChildren();
    }

    void UpdateNumerator(int value)
    {
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

    /*todo*/
    void UpdateNumeratorByPartition(int partition, int lastPartition)
    {
    }

    /*todo*/
    void UpdateDenominatorByPartition(int partition)
    {
    }

    void UpdateDenominator(int value)
    {
        if (value > 0)
        {
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
        Width = singleWidth * value;
        lastDenominator = value;
        UpdateElements(lastDenominator, lastNumerator);
        root.SendMessage("UpdateWidth");
    }

    /*todo*/
    void UpdatePartitions(int value)
    {
    }

    void OnClicked(Vector3 position)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.name.StartsWith("elem"))
            {
                SBSBounds meshBounds = child.GetComponent<EmptyElement>().GetBounds();
                position.z = meshBounds.max.z;

                if (meshBounds.ContainsPointXY(position))
                {
                   /* ClickElement(child.renderer.material.color != Workspace.Instance.white);*/
                    if (!isSelected[i])
                        child.renderer.material.color = color;
                    else
                        child.renderer.material.color = Workspace.Instance.white;
                    ClickElement(i);

                    Draw(zIndex);
                    break;
                }
            }
        }
    }
    #endregion

    #region Protected Methods
    protected void UpdateElements(int den, int num) 
    {
        if (Workspace.Instance.ElementOnFocus.GetComponent<RootElement>().mode != InteractionMode.Initializing) 
        {
            foreach (Transform child in transform)
            {
                 Destroy(child.gameObject);
            }      
       
            for (int k = 0; k < den; k++)
            {
                element = GameObject.Instantiate(Workspace.Instance.setSource) as GameObject;
                element.name = "elem_" + k;
                element.transform.parent = transform;
                element.transform.position = transform.TransformPoint(new Vector3(-((den - 1) * singleWidth) + (k * singleWidth), 0, 0));
                //element.transform.position = transform.TransformPoint(new Vector3(-(k * singleWidth), 0, 0));
                //if (!isSelected[isSelected.Count - 1 - k])
                if (!isSelected[k])
                {
                    element.renderer.material.color = Workspace.Instance.white;
                }
                else
                    element.renderer.material.color = color;
                element.AddComponent<EmptyElement>();
                element.SendMessage("SetMode", mode);
                element.SendMessage("SetType", type);
                element.SendMessage("SetSize", new Vector2(singleWidth, height));
            }
        }
    }
    #endregion
}