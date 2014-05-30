using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SBS.Math;
using fractionslab;
using fractionslab.behaviours;
using fractionslab.utils;
using fractionslab.meshes;

public class RectangleElement : WSElement, IWSElement
{
    #region Internal Data Structures
    public enum Direction
    {
        Vertical = 0,
        Horizontal
    }
    #endregion

    #region Public Fields
    public List<bool> slices = new List<bool>();
    public List<int> selectedSlices = new List<int>();
    #endregion

    #region Protected Fields
    public int lastNumerator = 0;
    public int lastDenominator = 0;
    public int lastPartitions = 1;
    public float lastElementScale = 1.0f;
    protected float strokeWidth = 0.1f;
    #endregion

    #region Unity Callbacks
    void Awake()
    {
        //root = transform.parent.gameObject;
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
            bounds = new SBSBounds(transform.position, new SBSVector3(width, height, 0.0f));
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

        if (state == ElementsState.Fraction || state == ElementsState.Result)
            UpdateSlices();

        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).SendMessage("Draw", zIndex, SendMessageOptions.DontRequireReceiver);
    }

    public override bool CheckPartition()
    {
        if (partitions > 1)
        {
            if (partNumerator % partitions > 0)
            {
                return false;
            }
            else
            {
                for (int i = 0; i < partNumerator; i += partitions)
                    if (slices[i])
                    {
                        for (int j = 0; j < partitions; j++)
                            if (!slices[i + j])
                                return false;
                    }
            }
        }
        return true;
    }

    /*public override void IncreaseNumerator()
    {
        if (partitions == 1)
        {
            this.numerator++;
            if (this.numerator > this.denominator)
                this.numerator = this.denominator;

            this.partNumerator = this.numerator * partitions;

        }
        else
        {
            this.partNumerator++;
            if (this.partNumerator > this.partDenominator)
                this.partNumerator = this.partDenominator;

            this.numerator = this.partNumerator / this.partitions;
        }

        root.BroadcastMessage("SetNumerator", this.numerator);
        root.BroadcastMessage("SetPartNumerator", this.partNumerator);

        ExternalEventsManager.Instance.SendMessageToSupport("FractionChange", "Numerator", root.name, partNumerator);
    }

    public override void DecreaseNumerator()
    {
        if (partitions == 1)
        {
            this.numerator--;
            if (this.numerator < 0)
                this.numerator = 0;
            this.partNumerator = this.numerator * partitions;
        }
        else
        {
            this.partNumerator--;
            if (this.partNumerator < 0)
                this.partNumerator = 0;

            this.numerator = this.partNumerator / this.partitions;
        }

        root.BroadcastMessage("SetNumerator", this.numerator);
        root.BroadcastMessage("SetPartNumerator", this.partNumerator);

        ExternalEventsManager.Instance.SendMessageToSupport("FractionChange", "Numerator", root.name, partNumerator);
    }*/

    /*public override void IncreasePartitions()
    {
        this.partitions++;

        this.partNumerator = numerator * partitions;
        this.partDenominator = denominator * partitions;

        root.BroadcastMessage("SetPartitions", this.partitions);

        ExternalEventsManager.Instance.SendMessageToSupport("FractionChange", "Partitions", root.name, partitions);
    }

    public override void DecreasePartitions()
    {
        this.partitions--;
        if (this.partitions < 1)
            this.partitions = 1;

        this.partNumerator = numerator * partitions;
        this.partDenominator = denominator * partitions;

        root.BroadcastMessage("SetPartitions", this.partitions);

        ExternalEventsManager.Instance.SendMessageToSupport("FractionChange", "Partitions", root.name, partitions);
    }*/

    /*public override void IncreaseDenominator()
    {
        this.denominator++;
        this.partDenominator = denominator * partitions;

        root.BroadcastMessage("SetDenominator", this.denominator);
        root.BroadcastMessage("SetPartDenominator", this.partDenominator);

        ExternalEventsManager.Instance.SendMessageToSupport("FractionChange", "Denominator", root.name, partDenominator);
    }

    public override void DecreaseDenominator()
    {
        if (this.denominator > 0)
        {
            this.denominator--;
            if (this.denominator < 1)
                this.denominator = 1;

            if (this.denominator < this.numerator)
                this.numerator = this.denominator;

            this.partDenominator = denominator * partitions;

            if (partDenominator < partNumerator)
                partNumerator = partDenominator;

            root.BroadcastMessage("SetDenominator", this.denominator);
            root.BroadcastMessage("SetNumerator", this.numerator);

            root.BroadcastMessage("SetPartDenominator", this.partDenominator);
            root.BroadcastMessage("SetPartNumerator", this.partNumerator);

            ExternalEventsManager.Instance.SendMessageToSupport("FractionChange", "Denominator", root.name, partDenominator);
        }
    }*/
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
        UpdateSlices();
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

        GameObject bg = new GameObject("background");
        bg.transform.parent = transform;
        bg.transform.position = transform.TransformPoint(Vector3.zero);
        bg.AddComponent<MeshElement>();
        bg.SendMessage("Initialize", mode);
        bg.SendMessage("SetMode", mode);
        bg.SendMessage("SetElementState", state);
        bg.SendMessage("SetType", type);
        bg.SendMessage("SetSize", new Vector2(width, height));
        bg.SendMessage("SetColor", Workspace.Instance.white);
        bg.SendMessage("SetStrokeDist", 0.2f);
        bg.SendMessage("SetStrokeWidth", 0.02f);

        if (denominator > 0)
        {
            UpdateDenominatorByPartition(partitions);
            UpdateNumerator(numerator);
            UpdatePartitions(partitions);
            UpdateNumeratorByPartition(partitions, 0);

            UpdateSliceStructure();
            UpdateSlices();
        }
    }

    void ClickSlice(int index)
    {
        if (slices[index])
        {
            slices[index] = false;
            selectedSlices.Remove(index);
            //Workspace.Instance.ElementOnFocus.GetComponent<RootElement>().DecreaseNumerator();
            partNumerator--;
            if (partNumerator < 0)
                partNumerator = 0;
            numerator = partNumerator / partitions;
        }
        else
        {
            slices[index] = true;
            selectedSlices.Add(index);
            //Workspace.Instance.ElementOnFocus.GetComponent<RootElement>().IncreaseNumerator();
            partNumerator++;
            numerator = partNumerator / partitions;            
        }

        if (partitions == 1)
            lastNumerator = numerator;
        else
            lastNumerator = partNumerator;
        UpdateSlices();
        Workspace.Instance.ElementOnFocus.GetComponent<RootElement>().UpdateByChildren();
    }

    void UpdateNumerator(int value)
    {
        value = Mathf.Clamp(value, 0, slices.Count);

        int prevNum = 0;
        for (int i = 0; i < slices.Count; i++)
            if (slices[i]) prevNum++;

        int diff = value - prevNum;
        if (diff < 0)
        {
            for (int i = 0; i < Mathf.Abs(diff); i++)
            {
                int lastIdx = selectedSlices.Count - 1;
                if (lastIdx >= 0.0f)
                {
                    int removeIdx = selectedSlices[lastIdx];
                    slices[removeIdx] = false;
                    selectedSlices.RemoveAt(lastIdx);
                }
            }
        }
        else if (diff > 0)
        {
            for (int i = 0; i < slices.Count; i++)
            {
                if (!slices[i])
                {
                    slices[i] = true;
                    diff--;
                    selectedSlices.Add(i);
                }

                if (diff <= 0)
                    break;
            }
        }

        UpdateSlices();
    }

    void UpdateNumeratorByPartition(int partition, int lastPartition)
    {
        selectedSlices.RemoveAll(item => item % lastPartitions != 0);

        if (lastPartition > 0)
        {
            for (int i = 0; i < selectedSlices.Count; i++)
                selectedSlices[i] /= lastPartition;
        }

        for (int i = 0; i < selectedSlices.Count; i++)
            selectedSlices[i] *= partition;

        List<int> tmp = new List<int>(selectedSlices);

        for (int i = 0; i < tmp.Count; i++)
            for (int j = 0; j < Mathf.Min(partNumerator - 1, partition - 1); j++)
                selectedSlices.Add(tmp[i] + (j + 1));

        for (int i = 0; i < slices.Count; i++)
            slices[i] = selectedSlices.Contains(i);

        //hack
        if (selectedSlices.Count > 0 && selectedSlices.Count < partNumerator)
        {
            Debug.Log(root.name + ", " + selectedSlices.Count + " < " + partNumerator);
            for (int i = 0; i < slices.Count; i++)
                if (!slices[i] && selectedSlices.Count < partNumerator)
                {
                    slices[i] = true;
                    selectedSlices.Add(i);
                }
        }

        UpdateSlices();

        /*
        List<int> tmp = new List<int>(selectedSlices);
        selectedSlices.Clear();
        for (int j = 0; j < partition; j++)
        {
            int n = Mathf.Min(numerator, denominator);
            for (int i = 0; i < n; i++)
            {
                slices[tmp[i] + j * denominator] = true;
                selectedSlices.Add(tmp[i] + j * denominator);
            }
        }

        UpdateSlices();
        */
    }

    void UpdateDenominatorByPartition(int partition)
    {
        slices.Clear();
        for (int i = 0; i < denominator; i++)
        {
            for (int j = 0; j < partition; j++)
            {
                slices.Add(false);
            }
        }

        for (int i = 0; i < selectedSlices.Count; i++)
        {
            if (selectedSlices[i] < slices.Count)
                slices[selectedSlices[i]] = true;
        }

        UpdateSlices();
    }

    void UpdateDenominator(int value)
    {
        if (value > 0)
        {
            int prevDen = slices.Count / partitions;
            int diff = (value - prevDen);
            if (diff < 0)
            {
                int startSlice = partitions * (denominator - 1);
                for (int j = 0; j < partitions; j++)
                {
                    int lastPos = startSlice + j;
                    if (slices[lastPos])
                    {
                        int freeIdx = slices.FindIndex(0, p => p.Equals(false));
                        if (freeIdx >= 0)
                            slices[freeIdx] = true;
                    }
                    slices.RemoveAt(lastPos);
                }
                /*for (int j = partitions - 1; j >= 0; j--)
                {
                    for (int i = 0; i < Mathf.Abs(diff); i++)
                    {
                        int lastPos = (denominator + 1) * (j + 1) - 1;
                        if (slices[lastPos])
                        {
                            int freeIdx = slices.FindIndex(j * (denominator + 1), denominator + 1, p => p.Equals(false));
                            if (freeIdx >= 0)
                                slices[freeIdx] = true;
                        }
                        slices.RemoveAt(lastPos);
                    }
                }*/
            }
            else
            {
                for (int j = 0; j < partitions; j++)
                {
                    for (int i = 0; i < Mathf.Abs(diff); i++)
                    {
                        slices.Add(false);
                        //slices.Insert(denominator * (j + 1) - 1, false);
                    }
                }
            }

            int n = 0;
            selectedSlices.Clear();
            for (int k = 0; k < slices.Count; k++)
            {
                if (slices[k])
                {
                    n++;
                    selectedSlices.Add(k);
                }
            }
            /*
            //hack
            if (this.numerator == this.denominator)
            {
                this.partNumerator = this.numerator * partitions;
                n = this.partNumerator;
            }

            root.BroadcastMessage("SetPartNumerator", n);
            //root.BroadcastMessage("SetPartDenominator", partDenominator);
            */
        }
        else if (value == 0)
        {
            slices.Clear();
        }

        UpdateSlices();
    }

    void UpdatePartitions(int value)
    {
        selectedSlices.Sort();
        UpdateSlices();
    }

    void OnClicked(Vector3 position)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.name.StartsWith("slice"))
            {
                SBSBounds meshBounds = child.GetComponent<MeshElement>().GetBounds();
                position.z = meshBounds.max.z;
                if (meshBounds.ContainsPointXY(position))
                {
                    string n = child.name;
                    int sliceIndex = int.Parse(n.Substring(n.Length - (n.Length - 5), (n.Length - 5)));
                    ClickSlice(sliceIndex);
                    Draw(zIndex);
                    break;
                }
            }
        }
    }
    #endregion

    #region Protected Methods
    protected void UpdateSlices()
    {
        int sliceCounter = 0;
        float basePosX = 0.0f;
        float basePosY = 0.0f;
        float baseSizeX = 0.0f;
        float baseSizeY = 0.0f;
        float sliceWidth = 0.0f;
        float sliceHeight = 0.0f;
        int sliceCount = slices.Count / partitions;

        float elementScale = root.GetComponent<RootElement>().elementScale;

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name.StartsWith("slice"))
                Destroy(transform.GetChild(i).gameObject);
        }

        if (type == ElementsType.VRect)
        {
            sliceWidth = width * elementScale / (float)sliceCount;
            basePosX = -width * 0.5f + sliceWidth / elementScale * 0.5f;
            baseSizeX = sliceWidth / elementScale;

            sliceHeight = height * elementScale / (float)partitions;
            basePosY = height * 0.5f - sliceHeight / elementScale * 0.5f;
            baseSizeY = sliceHeight / elementScale;
        }
        else if (type == ElementsType.HRect)
        {
            sliceWidth = width * elementScale / (float)partitions;
            basePosX = -width * 0.5f + sliceWidth / elementScale * 0.5f;
            baseSizeX = sliceWidth / elementScale;

            sliceHeight = height * elementScale / (float)sliceCount;
            basePosY = height * 0.5f - sliceHeight / elementScale * 0.5f;
            baseSizeY = sliceHeight / elementScale;
        }

        for (int p = 0; p < partitions; p++) 
        {
            for (int i = 0; i < sliceCount; i++) 
            {
                //int sliceIndex = (p * sliceCount + i);
                int sliceIndex = (i * partitions + p);
                if (slices[sliceIndex] || state != ElementsState.Cut)
                {
                    GameObject slice = new GameObject("slice" + sliceIndex);
                    slice.transform.parent = transform;
                    if (type == ElementsType.VRect)
                        slice.transform.position = transform.TransformPoint(new Vector3(basePosX + baseSizeX * i, basePosY - baseSizeY * p, -0.2f));
                    else if (type == ElementsType.HRect)
                        slice.transform.position = transform.TransformPoint(new Vector3(basePosX + baseSizeX * p, basePosY - baseSizeY * i, -0.2f));

                    slice.AddComponent<MeshElement>();
                    slice.SendMessage("Initialize", mode);
                    slice.SendMessage("SetMode", mode);
                    slice.SendMessage("SetElementState", state);
                    slice.SendMessage("SetType", type);


                    if (state == ElementsState.Cut)
                        slice.SendMessage("SetSize", new Vector2(sliceWidth - 0.1f, sliceHeight - 0.1f));
                    else
                        slice.SendMessage("SetSize", new Vector2(sliceWidth, sliceHeight));

                    Color emptySliceColor = Workspace.Instance.white;
                    if (state == ElementsState.Result)
                        emptySliceColor = Workspace.Instance.greyResult;

                    slice.SendMessage("SetColor", slices[sliceIndex] ? color : emptySliceColor);
                    slice.SendMessage("SetStrokeDist", 0.2f);
                    
                    if (state == ElementsState.Cut)
                        slice.SendMessage("SetStrokeWidth", 0.0f);
                    else
                        slice.SendMessage("SetStrokeWidth", 0.0f); //strokeWidth);

                    sliceCounter++;
                }
            }
        }

        if (state != ElementsState.Cut)
            UpdateSlicesLines();
    }

    void UpdateSlicesLines()
    {
        //float elementScale = 1.0f;
        string rootName = "line_root";

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name.StartsWith(rootName))
                Destroy(transform.GetChild(i).gameObject);
        }

        if (denominator > 0)
        {
            Color strokeColor = new Color(0.1098f, 0.2431f, 0.0353f);
            if (mode == InteractionMode.Freeze)
                strokeColor += new Color(0.4f, 0.4f, 0.4f);
            else if (state == ElementsState.Result)
                strokeColor = Workspace.Instance.white;

            GameObject lineRoot = new GameObject(rootName);
            lineRoot.transform.parent = transform;
            lineRoot.transform.position = transform.TransformPoint(Vector3.zero);

            if (type == ElementsType.VRect)
            {
                float baseX = elementScale * (-width * 0.5f);
                float stepX = elementScale * (width / (float)denominator);
                float baseY = elementScale * (-height * 0.5f);
                float stepY = elementScale * (height / (float)partitions);

                /* top line */
                Vector3[] topList = new Vector3[2];
                topList[0] = new Vector3(baseX - strokeWidth * elementScale * 0.5f, -baseY, 0.0f);
                topList[1] = new Vector3(-baseX + strokeWidth * elementScale * 0.5f, -baseY, 0.0f);

                GameObject lineTop = new GameObject("line_t");
                lineTop.transform.parent = lineRoot.transform;
                lineTop.transform.position = lineRoot.transform.TransformPoint(Vector3.zero);
                lineTop.transform.position += new Vector3(0.0f, 0.0f, -0.4f);
                MeshLineElement compTop = lineTop.AddComponent<MeshLineElement>();
                compTop.lineWidth = strokeWidth * elementScale;
                compTop.pointsList = topList;
                compTop.isClosed = false;
                compTop.color = strokeColor;
                compTop.Initialize();

                /* bottom line */
                Vector3[] bottomList = new Vector3[2];
                bottomList[0] = new Vector3(baseX - strokeWidth * elementScale * 0.5f, baseY, 0.0f);
                bottomList[1] = new Vector3(-baseX + strokeWidth * elementScale * 0.5f, baseY, 0.0f);

                GameObject lineBottom = new GameObject("line_b");
                lineBottom.transform.parent = lineRoot.transform;
                lineBottom.transform.position = lineRoot.transform.TransformPoint(Vector3.zero);
                lineBottom.transform.position += new Vector3(0.0f, 0.0f, -0.4f);
                MeshLineElement compBottom = lineBottom.AddComponent<MeshLineElement>();
                compBottom.lineWidth = strokeWidth * elementScale;
                compBottom.pointsList = bottomList;
                compBottom.isClosed = false;
                compBottom.color = strokeColor;
                compBottom.Initialize();

                for (int i = 0; i < denominator + 1; i++)
                {
                    Vector3[] pointList = new Vector3[2];
                    pointList[0] = new Vector3(baseX + stepX * i, -baseY, 0.0f);
                    pointList[1] = new Vector3(baseX + stepX * i, baseY, 0.0f);

                    GameObject line = new GameObject("line_" + i);
                    line.transform.parent = lineRoot.transform;
                    line.transform.position = lineRoot.transform.TransformPoint(Vector3.zero);
                    line.transform.position += new Vector3(0.0f, 0.0f, -0.4f);
                    MeshLineElement comp = line.AddComponent<MeshLineElement>();
                    comp.lineWidth = strokeWidth * elementScale;
                    comp.pointsList = pointList;
                    comp.isClosed = false;
                    comp.color = strokeColor;
                    comp.Initialize();
                }

                for (int i = 0; i < partitions - 1; i++)
                {
                    Vector3[] pointList = new Vector3[2];
                    pointList[0] = new Vector3(-baseX, baseY + stepY * (i + 1), 0.0f);
                    pointList[1] = new Vector3(baseX, baseY + stepY * (i + 1), 0.0f);

                    GameObject line = new GameObject("pline_" + i);
                    line.transform.parent = lineRoot.transform;
                    line.transform.position = lineRoot.transform.TransformPoint(Vector3.zero);
                    line.transform.position += new Vector3(0.0f, 0.0f, -0.4f);
                    MeshLineElement comp = line.AddComponent<MeshLineElement>();
                    comp.lineWidth = strokeWidth * 0.3f * elementScale;
                    comp.pointsList = pointList;
                    comp.isClosed = false;
                    comp.color = strokeColor;
                    comp.Initialize();
                }
            }

            if (type == ElementsType.HRect)
            {
                float baseX = elementScale * (-width * 0.5f);
                float stepX = elementScale * (width / (float)partitions);
                float baseY = elementScale * (-height * 0.5f);
                float stepY = elementScale * (height / (float)denominator);

                /* top line */
                Vector3[] topList = new Vector3[2];
                topList[0] = new Vector3(-baseX, -baseY + strokeWidth * elementScale * 0.5f, 0.0f);
                topList[1] = new Vector3(-baseX, baseY - strokeWidth * elementScale * 0.5f, 0.0f);

                GameObject lineTop = new GameObject("line_t");
                lineTop.transform.parent = lineRoot.transform;
                lineTop.transform.position = lineRoot.transform.TransformPoint(Vector3.zero);
                lineTop.transform.position += new Vector3(0.0f, 0.0f, -0.4f);
                MeshLineElement compTop = lineTop.AddComponent<MeshLineElement>();
                compTop.lineWidth = strokeWidth * elementScale;
                compTop.pointsList = topList;
                compTop.isClosed = false;
                compTop.color = strokeColor;
                compTop.Initialize();

                /* bottom line */
                Vector3[] bottomList = new Vector3[2];
                bottomList[0] = new Vector3(baseX, -baseY + strokeWidth * elementScale * 0.5f, 0.0f);
                bottomList[1] = new Vector3(baseX, baseY - strokeWidth * elementScale * 0.5f, 0.0f);

                GameObject lineBottom = new GameObject("line_b");
                lineBottom.transform.parent = lineRoot.transform;
                lineBottom.transform.position = lineRoot.transform.TransformPoint(Vector3.zero);
                lineBottom.transform.position += new Vector3(0.0f, 0.0f, -0.4f);
                MeshLineElement compBottom = lineBottom.AddComponent<MeshLineElement>();
                compBottom.lineWidth = strokeWidth * elementScale;
                compBottom.pointsList = bottomList;
                compBottom.isClosed = false;
                compBottom.color = strokeColor;
                compBottom.Initialize();

                for (int i = 0; i < partitions - 1; i++)
                {
                    Vector3[] pointList = new Vector3[2];
                    pointList[0] = new Vector3(baseX + stepX * (i + 1), -baseY, 0.0f);
                    pointList[1] = new Vector3(baseX + stepX * (i + 1), baseY, 0.0f);

                    GameObject line = new GameObject("pline_" + i);
                    line.transform.parent = lineRoot.transform;
                    line.transform.position = lineRoot.transform.TransformPoint(Vector3.zero);
                    line.transform.position += new Vector3(0.0f, 0.0f, -0.4f);
                    MeshLineElement comp = line.AddComponent<MeshLineElement>();
                    comp.lineWidth = strokeWidth * 0.3f * elementScale;
                    comp.pointsList = pointList;
                    comp.isClosed = false;
                    comp.color = strokeColor;
                    comp.Initialize();
                }

                for (int i = 0; i < denominator + 1; i++)
                {
                    Vector3[] pointList = new Vector3[2];
                    pointList[0] = new Vector3(-baseX, baseY + stepY * i, 0.0f);
                    pointList[1] = new Vector3(baseX, baseY + stepY * i, 0.0f);

                    GameObject line = new GameObject("line_" + i);
                    line.transform.parent = lineRoot.transform;
                    line.transform.position = lineRoot.transform.TransformPoint(Vector3.zero);
                    line.transform.position += new Vector3(0.0f, 0.0f, -0.4f);
                    MeshLineElement comp = line.AddComponent<MeshLineElement>();
                    comp.lineWidth = strokeWidth * elementScale;
                    comp.pointsList = pointList;
                    comp.isClosed = false;
                    comp.color = strokeColor;
                    comp.Initialize();
                }
            }
        }
    }

    void UpdateSliceStructure()
    {
        int diff = partNumerator - (numerator * partitions);

        if (diff > 0)
        {
            for (int i = 0; i < slices.Count; i++)
            {
                if (!slices[i] && diff > 0)
                {
                    diff--;
                    slices[i] = true;
                    selectedSlices.Add(i);
                }

                if (diff == 0)
                    break;
            }
        }
    }

    protected IEnumerator UpdateSliceLines()
    {
        yield return new WaitForEndOfFrame();
        string rootName = "line_root";
        for (int i = 0; i < transform.childCount; i++)
            if (transform.GetChild(i).name.StartsWith(rootName))
                Destroy(transform.GetChild(i).gameObject);

        GameObject lineRoot = new GameObject(rootName);
        lineRoot.transform.parent = transform;

        Vector3[] pointList = {
                                  transform.position - new Vector3(-width * 0.5f, -height * 0.5f, -0.2f),
                                  transform.position - new Vector3(-width * 0.5f, height * 0.5f, -0.2f),
                                  transform.position - new Vector3(width * 0.5f, height * 0.5f, -0.2f),
                                  transform.position - new Vector3(width * 0.5f, -height * 0.5f, -0.2f)
                              };
        GameObject root = new GameObject("thin_stroke");
        root.transform.parent = lineRoot.transform;
        root.transform.position = Vector3.zero;
        MeshLineElement comp = root.AddComponent<MeshLineElement>();
        comp.lineWidth = 0.01f;
        comp.pointsList = pointList;
        comp.isClosed = true;
        comp.color = Workspace.Instance.systemColor;
        comp.Initialize();
    }
    #endregion
}