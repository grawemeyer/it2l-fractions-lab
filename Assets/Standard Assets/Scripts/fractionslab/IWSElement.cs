using UnityEngine;
using SBS.Math;
using fractionslab.utils;

namespace fractionslab
{
    public interface IWSElement
    {
        void Initialize();
        SBSBounds GetBounds();
        void UpdateCollider(SBSBounds bounds);
        void Draw(int zIndex);
        void SetColor(Color color);
        void SetType(ElementsType type);
        void SetMode(InteractionMode mode);
    }
}
