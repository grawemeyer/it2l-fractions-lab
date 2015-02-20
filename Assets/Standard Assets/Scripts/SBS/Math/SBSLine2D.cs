using System;
using UnityEngine;

namespace SBS.Math
{
    public struct SBSLine2D
    {
        public float A;
        public float B;
        public float C;

        public SBSLine2D(SBSVector3 p1, SBSVector3 p2)
        {
			A = p2.y - p1.y;
			B = p1.x - p2.x;
			C = A * p1.x + B * p1.y;
        }

        public bool Intersect2D(SBSLine2D line, out SBSVector3 point)
        {
            float det = this.A * line.B - line.A * this.B;
            point = SBSVector3.zero;

            if (det == 0)
                return false;

            point.x = (line.B * this.C - this.B * line.C) / det;
            point.y = (this.A * line.C - line.A * this.C) / det;
            return true;
        }

		override public string ToString()
		{
			return A +"x + " + B + "y = " + C;
		}
	}
}
