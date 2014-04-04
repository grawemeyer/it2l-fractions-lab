using UnityEngine;
using System.Collections;
using SBS.Math;
using fractionslab.meshes;
using fractionslab.utils;

namespace fractionslab.behaviours
{
    public class EmptyElement : WSElement, IWSElement
    {
        #region Protected Fields
        protected float width;
        protected float height;
        #endregion

        #region Unity Callbacks
        #endregion

        #region Public Methods
        public override SBSBounds GetBounds()
        {
            bounds = new SBSBounds(transform.position, new SBSVector3(width, height, 0.0f));
            return bounds;
        }
        #endregion

        #region Messages
        void SetSize(Vector2 size)
        {
            Width = size.x;
            Height = size.y;
        }
        #endregion
    }
}
