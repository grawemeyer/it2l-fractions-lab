using UnityEngine;
using System.Collections.Generic;
using SBS.Math;
using fractionslab;
using fractionslab.behaviours;
using fractionslab.utils;
using fractionslab.meshes;

public class LiquidElement : WSElement, IWSElement
{
    #region Public Fields
    public GameObject water = null;
    public GameObject bg = null;
    public GameObject leftMarker = null;
    public GameObject rightMarker = null;
    protected GameObject label0 = null;
    #endregion

    #region Protected Fields
    protected int lastNumerator = 0;
    protected int lastDenominator = 0;
    protected int lastPartitions = 1;
    //protected float width = 0.0f;
    //protected float height = 0.0f;
    protected float strokeWidth = 0.06f;
    protected float lastElementScale = 1.0f;
    #endregion

    #region Unity Callbacks
    void Awake()
    {
        //root = transform.parent.gameObject;

            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).name.Equals("background"))
                {
                    bg = transform.GetChild(i).gameObject;
                    bg.renderer.material = new Material(Shader.Find("Diffuse"));
                    UpdateBG();
                    break;
                }
            }
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
            bounds = new SBSBounds(transform.position, new SBSVector3(width, height, 0.0f));
            return bounds;
        }
        else
        {
            bounds.Reset();
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject child = transform.GetChild(i).gameObject;
                if (child.name.Equals("water"))
                {
                    bounds = child.renderer.bounds;
                    break;
                }
            }
            return bounds;
        }
    }

    /*public override void SetColor(Color color)
    {
        bg = null;
        //bg.renderer.material = new Material(Shader.Find("Diffuse"));
        base.SetColor(color);
    }*/

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
    #endregion

    #region Messages
    void Freeze()
    {
        UpdateBG();
        UpdateWater();
        //Debug.Log("FREEZE");
        gameObject.BroadcastMessage("Draw", zIndex , SendMessageOptions.DontRequireReceiver);
    }

    void UnFreeze()
    {
        UpdateBG();
        UpdateWater();
    }

    void Cut()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name.Equals("background"))
            {
                Transform water = transform.GetChild(i).GetChild(0);
                Vector3 pos = new Vector3(water.transform.position.x, water.transform.position.y, 0.12f);
                water.transform.position = pos;
                water.parent = transform;
                Destroy(transform.GetChild(i).gameObject);
            }
        }
        root.GetComponent<RootElement>().UpdateGraphics();
    }

    void SetContentColor(Color c)
    {
        color = c;
        UpdateWater();
        Draw(zIndex);
    }

    void SetSize(Vector2 size)
    {
        Width = size.x;
        Height = size.y;
    }

    void Initialize()
    {
        bg = GameObject.Instantiate(Workspace.Instance.liquidSource) as GameObject;
        bg.name = "background";
        bg.transform.parent = transform;
        bg.transform.position = transform.TransformPoint(Vector3.zero);
        bg.AddComponent<EmptyElement>();
        bg.SendMessage("SetMode", mode);
        bg.SendMessage("SetType", type);
        bg.SendMessage("SetSize", new Vector2(width, height));

        InitializeWater();
        InitializeFixedLabels();
        
        UpdateWater();
        UpdateBG();

        if (denominator > 0)
        {
            UpdateDenominator(partDenominator);
            UpdateNumerator(partNumerator);
        }
    }

    protected void InitializeWater()
    {
        if (state != ElementsState.Cut)
        {
            if (null == bg)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    if (transform.GetChild(i).name.Equals("background"))
                    {
                        bg = transform.GetChild(i).gameObject;
                        bg.renderer.material = new Material(Shader.Find("Diffuse"));
                        break;
                    }
                }
            }

            for (int i = 0; i < bg.transform.childCount; i++)
            {
                if (bg.transform.GetChild(i).name.Equals("water"))
                {
                    water = bg.transform.GetChild(i).gameObject;
                    water.renderer.material = new Material(Shader.Find("Diffuse"));

                    Color meshColor = color;
                    if (state == ElementsState.Result)
                        meshColor = Workspace.Instance.greenResult;

                    water.renderer.material.color = color;

                    for (int j = 0; j < water.transform.childCount; j++)
                    {
                        if (water.transform.GetChild(j).name.Equals("leftMarker"))
                            leftMarker = water.transform.GetChild(j).gameObject;

                        if (water.transform.GetChild(j).name.Equals("rightMarker"))
                            rightMarker = water.transform.GetChild(j).gameObject;
                    }
                }
            }
        }
    }

    protected void InitializeFixedLabels()
    {
        string labelName = "labelmax";
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name.Equals(labelName))
            {
                //label0 = transform.GetChild(i).gameObject;
                Destroy(transform.GetChild(i).gameObject);
                label0 = null;
                break;
            }
        }

        if (null == label0 && state != ElementsState.Cut)
        {
            label0 = new GameObject(labelName);
            label0.transform.parent = transform;

            float offsX = 0.0f,
                  offsY = 0.0f;
            if (Application.platform == RuntimePlatform.OSXWebPlayer)
                offsX = offsY = -1.0f;
#if UNITY_IPHONE  || UNITY_ANDROID
            offsX = offsY = -1.0f;
#endif
            label0.transform.position = transform.TransformPoint(new Vector3(1.0f + offsX, 3.33f + offsY, 0.8f));
            SmallLabelMCElement comp = label0.AddComponent<SmallLabelMCElement>();
            comp.Initialize();

            Color meshColor = Workspace.Instance.green;
            if (state == ElementsState.Result)
                meshColor = Workspace.Instance.white;

            label0.SendMessage("SetValue", "MAX");
            label0.SendMessage("SetColor", meshColor);
            label0.SetActive(false);
        }
    }

    void UpdateNumerator(int value)
    {
        UpdateWater();
    }

    void UpdateDenominator(int value)
    {
        UpdateTicks();

        if (null == water)
            InitializeWater();

        if (null == label0)
            InitializeFixedLabels();

        if (null != label0)
            label0.SetActive(true);
    }

    void OnClicked(Vector3 position)
    {
        Vector3 localPos;

        if (state != ElementsState.Equivalence)
            localPos = root.transform.InverseTransformPoint(position);
        else
            localPos = position;

        Vector3 leftLocalPos = root.transform.InverseTransformPoint(leftMarker.transform.position);
        Vector3 rightLocalPos = root.transform.InverseTransformPoint(rightMarker.transform.position);

        RootElement superRoot = root.transform.parent.GetComponent<RootElement>();

        if (state != ElementsState.Equivalence)
        {
            if (null != superRoot.equivalences && superRoot.equivalences.Count > 0)
            {
                foreach (GameObject eq in superRoot.equivalences)
                {
                    foreach (LiquidElement st in eq.GetComponentsInChildren<LiquidElement>())
                    {
                        if (st.root.name == root.name)
                            st.SendMessage("OnClicked", localPos, SendMessageOptions.DontRequireReceiver);
                    }   
                    //eq.GetComponentInChildren<LiquidElement>().SendMessage("OnClicked", localPos, SendMessageOptions.DontRequireReceiver);
                }
            }
        }

        if (localPos.x > leftLocalPos.x && localPos.x < rightLocalPos.x)
        {
            //Debug.Log("localPos.y " + localPos.y + " leftLocalPos.y " + leftLocalPos.y);
            if (localPos.y > leftLocalPos.y)
            {
                if (partNumerator < partDenominator)
                    superRoot.IncreaseNumerator(); //root.BroadcastMessage("IncreaseNumerator");
            }
            else
            {
                //RootElement superRoot = root.transform.parent.GetComponent<RootElement>();
                int num = superRoot.elements.Count;
                if (partNumerator < partDenominator || superRoot.elements[num - 1] == root)
                    superRoot.DecreaseNumerator(); //root.BroadcastMessage("DecreaseNumerator");
            }
        }
        Draw(zIndex);
    }
    #endregion

    #region Protected Methods
    protected void UpdateTicks()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name.StartsWith("tick"))
                Destroy(transform.GetChild(i).gameObject);
        }

        float basePosX = 1.74f;
        float basePosY = -2.18f;
        float topPosY = 1.85f;
        float brickHeight = topPosY - basePosY;
        float stepY = brickHeight / partDenominator;

        float tickWidth = 0.1f;
        if (state == ElementsState.Cut)
            tickWidth *= 0.6f;

        int tickIndex = (partDenominator + 1) - partNumerator;
        for (int i = 0; i < partDenominator; i++)
        {
            if (state != ElementsState.Cut || i >= tickIndex - 1)
            {
                Color meshColor = Workspace.Instance.green;
                if (state == ElementsState.Result)
                    meshColor = Workspace.Instance.white;

                float w = 0.4f;
                float h = tickWidth;
                if (i % partitions == 0)
                    w = 0.8f;

                GameObject tick = new GameObject("tick" + (i + 1));
                tick.transform.parent = transform;
                tick.transform.position = transform.TransformPoint(new Vector3(basePosX - w * 0.5f, topPosY - (i * stepY), 0.1f));
                tick.transform.localScale = transform.TransformDirection(new Vector3(1.0f, 1.0f, 1.0f));

                MeshElement comp = tick.AddComponent<MeshElement>();
                comp.Initialize();
                tick.SendMessage("SetMode", mode);
                tick.SendMessage("SetType", type);
                tick.SendMessage("SetSize", new Vector2(w, tickWidth));
                tick.SendMessage("SetColor", meshColor);
                tick.SendMessage("SetStrokeDist", 0.2f);
                tick.SendMessage("SetStrokeWidth", 0.0f);
            }
        }
    }

    protected void UpdateWater()
    {
        if (null != water)
        {
            float value = partNumerator / (float)partDenominator;
            if (partDenominator == 0)
                value = 0.0f;
            water.transform.localScale = new Vector3(1.0f, value, 1.0f);

            Color meshColor = color;
            if (mode == InteractionMode.Freeze)
                meshColor += new Color(0.4f, 0.4f, 0.4f);
            else if (state == ElementsState.Result)
                meshColor = Workspace.Instance.greenResult;

            water.renderer.material.color = meshColor;
        }
    }

    protected void UpdateBG()
    {
        if (null != bg)
        {
            Color meshColor = Workspace.Instance.green;
            if (mode == InteractionMode.Freeze)
            {
                meshColor += new Color(0.4f, 0.4f, 0.4f);
            }
            else if (state == ElementsState.Result)
                meshColor = Workspace.Instance.white;

            bg.renderer.material.color = meshColor;
        }
    }

    public void DecreaseCutNumerator()
    {
         partNumerator--;
        UpdateWater();
        UpdateTicks();
        Draw(0);
    }


    public void DecreaseCutNumeratorPartitions()
    {
        if (partitions > 1)
            partNumerator = partNumerator - partitions;
        else
        {
            numerator--;
            UpdateNumerator(numerator);
        }
        UpdateWater();
        UpdateTicks();
        Draw(0);
    }

    #endregion
}