using UnityEngine;
using System.Collections;
using SBS.Math;
using fractionslab.meshes;
using fractionslab.utils;

namespace fractionslab.behaviours
{
    public class MeshElement : WSElement, IWSElement
    {
        //public float width;
        //public float height;

        #region Protected Fields
        protected MeshFilter mf;
        protected Mesh mesh;

        protected GameObject stroke = null;
        protected float strokeDist;
        protected float strokeWidth;

        protected float arrowDirection = 1.0f;
        #endregion

        #region Unity Callbacks
        void Awake()
        {
            /*gameObject.AddComponent<MeshRenderer>();
            mf = gameObject.AddComponent<MeshFilter>();
            bounds = new SBSBounds(transform.position, new SBSVector3(width, height, 0.0f));
            //renderer.material = new Material(Shader.Find("Custom/VertexColor"));
            renderer.material = new Material(Shader.Find("VertexLit"));*/
        }
        #endregion

        #region Public Methods
        public override SBSBounds GetBounds()
        {
            bounds = new SBSBounds(transform.position, new SBSVector3(width, height, 0.0f));
            return bounds;
        }

        public override void Draw(int zIndex)
        {
            Color meshColor = color;
            switch (type)
            {
                case ElementsType.VRect:
                case ElementsType.HRect:
               // case ElementsType.Set:
                    if (mode == InteractionMode.Freeze)
                        meshColor += new Color(0.4f, 0.4f, 0.4f);

                    MeshUtils.CreateRectangle(width, height, meshColor, out mesh);
                    if (null != mf)
                        mf.mesh = mesh;

                    if (null == stroke && strokeWidth > 0)
                    {
                        string strokeName = "stroke";
                        for (int i = 0; i < transform.childCount; i++)
                            if (transform.GetChild(i).name.Equals(strokeName))
                                Destroy(transform.GetChild(i).gameObject);

                        Color strokeColor = new Color(0.1098f, 0.2431f, 0.0353f);
                        if (mode == InteractionMode.Freeze)
                            strokeColor += new Color(0.4f, 0.4f, 0.4f);

                        Vector3[] pointList = {
                                                  new Vector3(-width * 0.5f, -height * 0.5f, -strokeDist),
                                                  new Vector3(-width * 0.5f, height * 0.5f, strokeDist),
                                                  new Vector3(width * 0.5f, height * 0.5f, -strokeDist),
                                                  new Vector3(width * 0.5f, -height * 0.5f, strokeDist)
                                              };
                        GameObject root = new GameObject(strokeName);
                        root.isStatic = true;
                        root.transform.parent = transform;
                        root.transform.localScale = Vector3.one;
                        root.transform.position = transform.TransformPoint(Vector3.zero);
                        MeshLineElement comp = root.AddComponent<MeshLineElement>();
                        comp.lineWidth = strokeWidth;
                        comp.pointsList = pointList;
                        comp.isClosed = true;
                        comp.color = strokeColor;
                        comp.Initialize();
                       // MeshUtils.CreateRectangleStroke(gameObject, strokeDist, strokeWidth, strokeColor, out stroke);


                    }

                    renderer.material.SetColor("_Color", meshColor);
                    break;
                case ElementsType.Set:
                    if (mode == InteractionMode.Freeze)
                        meshColor += new Color(0.4f, 0.4f, 0.4f);

                    /*if (null != mf)
                        mf.mesh = mesh;*/

                    if (null == stroke && strokeWidth > 0)
                    {
                     //   Debug.Log("in if stroke");
                        string strokeName = "stroke";
                        for (int i = 0; i < transform.childCount; i++)
                            if (transform.GetChild(i).name.Equals(strokeName))
                                Destroy(transform.GetChild(i).gameObject);
                        
                        Color strokeColor = new Color(0.1098f, 0.2431f, 0.0353f);
                        if (mode == InteractionMode.Freeze)
                            strokeColor += new Color(0.4f, 0.4f, 0.4f);

                        Vector3[] pointList = {
                                                  new Vector3(-width * 0.5f, -height * 0.5f, -strokeDist),
                                                  new Vector3(-width * 0.5f, height * 0.5f, strokeDist),
                                                  new Vector3(width * 0.5f, height * 0.5f, -strokeDist),
                                                  new Vector3(width * 0.5f, -height * 0.5f, strokeDist)
                                              };
                        GameObject root = new GameObject(strokeName);
                      //  Debug.Log("DRAW STROKE " + root.name);
                        root.isStatic = true;
                        root.transform.parent = transform;
                        root.transform.localScale = Vector3.one;
                        
                        root.transform.position = transform.TransformPoint(Vector3.zero);
                        MeshLineElement comp = root.AddComponent<MeshLineElement>();
                        comp.lineWidth = strokeWidth;
                        comp.pointsList = pointList;
                        comp.isClosed = true;
                        comp.color = strokeColor;
                        comp.Initialize();
                       // MeshUtils.CreateRectangleStroke(gameObject, strokeDist, strokeWidth, strokeColor, out stroke);
                    }
                    renderer.material.SetColor("_Color", meshColor);
                    break;
                case ElementsType.Liquid:
                case ElementsType.Line:
                    if (mode == InteractionMode.Freeze)
                        meshColor += new Color(0.4f, 0.4f, 0.4f);

                    transform.localScale = Vector3.one;
                    MeshUtils.CreateRectangle(width, height, meshColor, out mesh);
                    if (null != mf)
                        mf.mesh = mesh;
                    renderer.material.SetColor("_Color", meshColor);
                    break;

                case ElementsType.Arrow:
                    if (mode == InteractionMode.Freeze)
                        meshColor += new Color(0.4f, 0.4f, 0.4f);
                    
                    Vector3[] arrowPointsList = {
                                                  new Vector3(0.3f * arrowDirection, 0.2f, 0.0f),
                                                  new Vector3(0.0f, 0.0f, 0.0f),
                                                  new Vector3(0.3f* arrowDirection, -0.2f, 0.0f)
                                              };

                    MeshUtils.CreateLine(arrowPointsList, width, meshColor, false, out mesh);
                    if (null != mf)
                        mf.mesh = mesh;
                    renderer.material.SetColor("_Color", meshColor);
                    break;

                case ElementsType.Highlight: 
                    
                    meshColor += new Color(1.0f, 0.0f, 0.0f, 1.0f);

                    MeshUtils.CreateRectangle(width, height, meshColor, out mesh);
                    if (null != mf)
                        mf.mesh = mesh;

                    if (null == stroke && strokeWidth > 0)
                    {
                        string strokeName = "stroke";
                        for (int i = 0; i < transform.childCount; i++)
                            if (transform.GetChild(i).name.Equals(strokeName))
                                Destroy(transform.GetChild(i).gameObject);

                        Color strokeColor = new Color(0.1098f, 0.2431f, 0.0353f);
                        if (mode == InteractionMode.Freeze)
                            strokeColor += new Color(0.4f, 0.4f, 0.4f);

                        Vector3[] pointList = {
                                                  new Vector3(-width * 0.5f, -height * 0.5f, -strokeDist),
                                                  new Vector3(-width * 0.5f, height * 0.5f, strokeDist),
                                                  new Vector3(width * 0.5f, height * 0.5f, -strokeDist),
                                                  new Vector3(width * 0.5f, -height * 0.5f, strokeDist)
                                              };
                        GameObject root = new GameObject(strokeName);
                        root.isStatic = true;
                        root.transform.parent = transform;
                        root.transform.localScale = Vector3.one;
                        root.transform.position = transform.TransformPoint(Vector3.zero);
                        MeshLineElement comp = root.AddComponent<MeshLineElement>();
                        comp.lineWidth = strokeWidth;
                        comp.pointsList = pointList;
                        comp.isClosed = true;
                        comp.color = strokeColor;
                        comp.Initialize();

                        /*MeshUtils.CreateRectangleStroke(gameObject, strokeDist, strokeWidth, strokeColor, out stroke);*/
                    }

                    renderer.material.SetColor("_Color", meshColor);
                    break;
            }
        }
        #endregion

        #region Messages
        public void Initialize()
        {
            gameObject.AddComponent<MeshRenderer>();
            mf = gameObject.AddComponent<MeshFilter>();
            bounds = new SBSBounds(transform.position, new SBSVector3(width, height, 0.0f));
            renderer.material = new Material(Shader.Find("VertexLit"));
        }

        void SetSize(Vector2 size)
        {
            Width = size.x;
            Height = size.y;
        }

        void SetStrokeDist(float dist)
        {
            strokeDist = dist;
        }

        void SetStrokeWidth(float width)
        {
            strokeWidth = width;
        }

        void SetArrowLeft(bool left)
        {
            arrowDirection = left ? 1.0f : -1.0f;
        }
        #endregion
    }
}
