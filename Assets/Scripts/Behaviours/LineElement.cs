using UnityEngine;
using System.Collections.Generic;
using SBS.Math;
using fractionslab;
using fractionslab.behaviours;
using fractionslab.utils;
using fractionslab.meshes;

public class LineElement : WSElement, IWSElement
{
    #region Protected Fields
    protected bool justFreezed = false;

    protected int lastNumerator = 0;
    protected int lastDenominator = 0;
    protected int lastPartitions = 1;
    //protected float width = 0.0f;
    //protected float height = 0.0f;
    protected float strokeWidth = 0.06f;
    protected float lastElementScale = 1.0f;
    protected GameObject root = null;

    protected GameObject bg = null;
    protected GameObject label0 = null;
    protected GameObject label1 = null;

    protected float label0X = -3.01f;
    protected float label1X = 3.95f;
    #endregion

    #region Unity Callbacks
    void Awake()
    {
        root = transform.parent.gameObject;
    }
    #endregion

    #region Public Methods
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
                if (null != transform.GetChild(i).GetComponent<WSElement>() && transform.GetChild(i).name.Equals("numerator_slice"))
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
            UpdateDenominator(denominator);
            UpdateNumerator(numerator);
            lastPartitions = partitions;
        }

        if (partitions == 1)
        {
            if (numerator != lastNumerator)
            {
                UpdateNumerator(numerator);
                lastNumerator = numerator;
            }

            if (denominator != lastDenominator)
            {
                UpdateDenominator(denominator);
                UpdateNumerator(numerator);
                lastDenominator = denominator;
            }
        }
        else
        {
            if (partNumerator != lastNumerator)
            {
                UpdateNumerator(partNumerator);
                lastNumerator = partNumerator;
            }

            if (partDenominator != lastDenominator)
            {
                UpdateDenominator(partDenominator);
                UpdateNumerator(partNumerator);
                lastDenominator = partDenominator;
            }
        }

        if (lastElementScale != elementScale)
        {
            gameObject.transform.localScale = new Vector3(elementScale, elementScale, 1.0f);
            lastElementScale = elementScale;
        }

        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).SendMessage("Draw", zIndex, SendMessageOptions.DontRequireReceiver);
    }

    public override bool CheckPartition()
    {
        return ((partDenominator % partitions == 0) && (partNumerator % partitions == 0));
    }

    public override void IncreaseNumerator()
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
    }

    public override void IncreaseDenominator()
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
    }

    public override void IncreasePartitions()
    {
        this.partitions++;

        this.partNumerator = numerator * partitions;
        this.partDenominator = denominator * partitions;

        root.BroadcastMessage("SetPartitions", this.partitions);

        ExternalEventsManager.Instance.SendMessageToSupport("FractionChange", "Partition", root.name, partitions);
    }

    public override void DecreasePartitions()
    {
        this.partitions--;
        if (this.partitions < 1)
            this.partitions = 1;

        this.partNumerator = numerator * partitions;
        this.partDenominator = denominator * partitions;

        root.BroadcastMessage("SetPartitions", this.partitions);

        ExternalEventsManager.Instance.SendMessageToSupport("FractionChange", "Partition", root.name, partitions);
    }
    #endregion

    #region Messages
    void Cut()
    {
        root.GetComponent<RootElement>().UpdateGraphics();
    }

    void SetSize(Vector2 size)
    {
        Width = size.x;
        Height = size.y;
    }

    void Initialize()
    {
        InitializeBackground();
        InitializeFixedLabels();

        if (denominator > 0)
        {
            UpdateDenominator(partDenominator);
            UpdateNumerator(partNumerator);
        }
    }

    void UpdateNumerator(int value)
    {
        UpdateSlice();
    }

    void UpdateDenominator(int value)
    {
        UpdateTicks();

        if (null == bg)
            InitializeBackground();

        if (null == label0 || null == label1)
            InitializeFixedLabels();

        if (null != label0 || null != label1)
        {
            label0.SetActive(true);
            label1.SetActive(true);
        }
    }

    void OnClicked(Vector3 position)
    {
        Vector3 localPos = root.transform.TransformPoint(position);
        int tickIndex = (partDenominator + 1) - partNumerator;
        if (partNumerator == 0)
            tickIndex = 0;
        float xPos = 0.0f;

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name.Equals("tick" + tickIndex))
            {
                xPos = root.transform.TransformPoint(transform.GetChild(i).transform.position).x;
                break;
            }
        }

        if (localPos.x >= xPos)
            IncreaseNumerator();
        else
            DecreaseNumerator();

        Draw(zIndex);
    }

    void CutFraction()
    {
    }
    #endregion

    #region Protected Methods
    protected void InitializeBackground()
    {
        string backgroundName = "background";
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name.Equals(backgroundName))
            {
                Destroy(transform.GetChild(i).gameObject);
                bg = null;
            }
        }

        Color meshColor = Workspace.Instance.black;
        if (state == ElementsState.Result)
            meshColor = Workspace.Instance.white;

        if (null == bg && state != ElementsState.Cut)
        {
            bg = new GameObject("background");
            bg.transform.parent = transform;
            bg.transform.position = transform.TransformPoint(Vector3.zero);
            bg.AddComponent<MeshElement>();
            bg.SendMessage("Initialize");
            bg.SendMessage("SetMode", mode);
            bg.SendMessage("SetType", type);
            bg.SendMessage("SetSize", new Vector2(width * elementScale, height * elementScale));
            bg.SendMessage("SetColor", meshColor);
            bg.SendMessage("SetStrokeDist", 0.2f);
            bg.SendMessage("SetStrokeWidth", 0.0f);
        }
    }

    protected void InitializeFixedLabels()
    {
        string label0Name = "label0";
        string label1Name = "label1";
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name.Equals(label0Name))
            {
                Destroy(transform.GetChild(i).gameObject);
                label0 = null;
            }
            if (transform.GetChild(i).name.Equals(label1Name))
            {
                Destroy(transform.GetChild(i).gameObject);
                label1 = null;
            }
        }

        Color meshColor = Workspace.Instance.black;
        if (state == ElementsState.Result)
            meshColor = Workspace.Instance.white;

        if (null == label0 && state != ElementsState.Cut)
        {
            label0 = new GameObject("label0");
            label0.transform.parent = transform;

            float offsX = 0.0f,
                  offsY = 0.0f;
            if (Application.platform == RuntimePlatform.OSXWebPlayer)
				offsX = offsY = -1.0f;
#if UNITY_IPHONE
			offsX = offsY = -1.0f;
#endif
            label0.transform.position = transform.TransformPoint(new Vector3(label0X + offsX, 2.12f + offsY, 0.8f));

            label0.AddComponent<SingleLabelMCElement>();
            label0.SendMessage("Initialize");
            label0.SendMessage("SetValue", fractionBaseOffset.ToString());
            label0.SendMessage("SetColor", meshColor);
            label0.SetActive(false);
        }

        if (null == label1 && state != ElementsState.Cut)
        {
            label1 = new GameObject("label1");
            label1.transform.parent = transform;

            float offsX = 0.0f,
                  offsY = 0.0f;
            if (Application.platform == RuntimePlatform.OSXWebPlayer)
				offsX = offsY = -1.0f;
#if UNITY_IPHONE
			offsX = offsY = -1.0f;
#endif
            label1.transform.position = transform.TransformPoint(new Vector3(label1X + offsX, 2.12f + offsY, 0.8f));

            label1.AddComponent<SingleLabelMCElement>();
            label1.SendMessage("Initialize");
            label1.SendMessage("SetValue", (fractionBaseOffset + 1).ToString());
            label1.SendMessage("SetColor", meshColor);
            label1.SetActive(false);
        }
    }

    protected void UpdateTicks()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name.StartsWith("tick"))
                Destroy(transform.GetChild(i).gameObject);
        }

        Color meshColor = Workspace.Instance.black;
        if (state == ElementsState.Result)
            meshColor = Workspace.Instance.white;

        float tickWidth = height;
        if (state == ElementsState.Cut)
            tickWidth *= 0.6f;

        if (partNumerator <= 0 && state == ElementsState.Cut)
            return;

        GameObject zeroTick = new GameObject("tick0");
        zeroTick.transform.parent = transform;
        zeroTick.transform.position = transform.TransformPoint(new Vector3(-width * 0.5f, 0.0f, -0.4f));

        zeroTick.AddComponent<MeshElement>();
        zeroTick.SendMessage("Initialize");
        zeroTick.SendMessage("SetMode", mode);
        zeroTick.SendMessage("SetType", type);
        zeroTick.SendMessage("SetSize", new Vector2(tickWidth * elementScale, 0.6f * elementScale));
        zeroTick.SendMessage("SetColor", meshColor);
        zeroTick.SendMessage("SetStrokeDist", 0.2f);
        zeroTick.SendMessage("SetStrokeWidth", 0.0f);

        float basePosX = width * 0.5f;
        float stepX = width / partDenominator;

        int tickIndex = (partDenominator + 1) - partNumerator;
        for (int i = 0; i < partDenominator; i++)
        {
            if (state != ElementsState.Cut || i >= tickIndex - 1)
            {
                GameObject tick = new GameObject("tick" + (i + 1));
                tick.transform.parent = transform;
                tick.transform.position = transform.TransformPoint(new Vector3(basePosX - (i * stepX), 0.0f, -0.4f));

                tick.AddComponent<MeshElement>();
                tick.SendMessage("Initialize");
                tick.SendMessage("SetMode", mode);
                tick.SendMessage("SetType", type);
                if (i % partitions == 0)
                    tick.SendMessage("SetSize", new Vector2(tickWidth * elementScale, 0.6f * elementScale));
                else
                    tick.SendMessage("SetSize", new Vector2(tickWidth * elementScale, 0.3f * elementScale));

                tick.SendMessage("SetColor", meshColor);
                tick.SendMessage("SetStrokeDist", 0.2f);
                tick.SendMessage("SetStrokeWidth", 0.0f);
            }
        }
    }

    protected void UpdateSlice()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name.StartsWith("numerator"))
                Destroy(transform.GetChild(i).gameObject);
        }

        float value = partNumerator / (float)partDenominator;

        GameObject slice = new GameObject("numerator_slice");
        slice.transform.parent = transform;

        float xPos = -width * 0.5f + (value * width) * 0.5f;
        slice.transform.position = transform.TransformPoint(new Vector3(xPos, 0.0f, -0.2f));
        if (state == ElementsState.Cut)
            slice.transform.position = transform.TransformPoint(new Vector3(xPos, 0.0f, 0.0f));

        Color meshColor = color;
        if (state == ElementsState.Result)
            meshColor = Workspace.Instance.greenResult;

        slice.AddComponent<MeshElement>();
        slice.SendMessage("Initialize");
        slice.SendMessage("SetMode", mode);
        slice.SendMessage("SetType", type);
        slice.SendMessage("SetSize", new Vector2(value * width * elementScale, height * elementScale));
        slice.SendMessage("SetColor", meshColor);
        slice.SendMessage("SetStrokeDist", 0.2f);
        slice.SendMessage("SetStrokeWidth", 0.0f);
    }

    public void DecreaseCutNumerator()
    {
        partNumerator--;
        UpdateSlice();
        UpdateTicks();
        Draw(0);
    }
    #endregion
}