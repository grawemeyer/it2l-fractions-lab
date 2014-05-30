using UnityEngine;
using System.Collections;
using SBS.Math;
using fractionslab.meshes;
using fractionslab.utils;

namespace fractionslab.behaviours
{
    public class MeshLineElement : WSElement, IWSElement
    {
        #region Protected Fields
        public float lineWidth;
        public Vector3[] pointsList;
        public bool isClosed;
        #endregion

        #region Protected Fields
        protected MeshFilter mf;
        protected Mesh mesh;
        #endregion

        #region Unity Callbacks
        #endregion

        #region Public Methods
        public override SBSBounds GetBounds()
        {
            bounds = new SBSBounds(transform.position, new SBSVector3(0.0f, 0.0f, 0.0f));
            return bounds;
        }

        public override void Draw(int zIndex)
        {
            MeshUtils.CreateLine(pointsList, lineWidth, color, isClosed, out mesh);
            if (null != mf)
                mf.mesh = mesh;
            renderer.material.SetColor("_Color", color);
        }
        #endregion

        #region Messages
        public void Initialize()
        {
            gameObject.AddComponent<MeshRenderer>();
            mf = gameObject.AddComponent<MeshFilter>();
            renderer.material = new Material(Shader.Find("VertexLit"));
            Draw(0);
        }
        #endregion
    }
}
