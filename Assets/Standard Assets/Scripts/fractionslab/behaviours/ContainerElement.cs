using UnityEngine;
using System.Collections;
using SBS.Math;
using fractionslab.meshes;
using fractionslab.utils;

namespace fractionslab.behaviours
{
    public class ContainerElement : WSElement, IWSElement
    {
        #region Protected Fields
        #endregion

        #region Unity Callbacks
        #endregion

        #region Public Methods
        public override SBSBounds GetBounds()
        {
            bounds.Reset();
            return bounds;
        }

        public override void Draw(int zIndex)
        {

            base.Draw(zIndex);
            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).SendMessage("Draw", zIndex, SendMessageOptions.DontRequireReceiver);
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
