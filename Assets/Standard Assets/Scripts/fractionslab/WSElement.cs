using UnityEngine;
using SBS.Math;
using fractionslab.utils;

namespace fractionslab
{
    public class WSElement : MonoBehaviour, IWSElement
    {
        public GameObject root = null;
        public ElementsState state;
        public int numerator = 0;
        public int denominator = 0;
        public int partitions = 1;
        public int partNumerator = 0;
        public int partDenominator = 0;
        public InteractionMode mode;
        public float elementScale = 1.0f;
        public float width = 0.0f;
        public float height = 0.0f;
        public ElementsType type;
        public Color color;
        public int fractionBaseOffset = 0;
        public bool inputEnabled = true;
        public bool isSubFraction = false;
                
        protected int zIndex;
        protected SBSBounds bounds;
        protected BoxCollider collider;
        protected float minScale = 0.5f;
        protected float maxScale = 2.0f;
        protected float initialWidth = 0.0f;
        protected float initialHeight = 0.0f;

        public float Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
                initialWidth = width;
            }
        }

        public float Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
                initialHeight = height;
            }
        }

        public virtual void Initialize()
        {
        }

        public virtual float GetValue()
        {
            return ((float)numerator / (float)denominator);
        }

        public virtual float GetPartValue()
        {
            return ((float)partNumerator / (float)partDenominator);
        }

        public virtual SBSBounds GetBounds()
        {
            return bounds;
        }

        public virtual void UpdateCollider(SBSBounds bounds)
        {
        }

        public virtual void Draw(int zIndex)
        {
            this.zIndex = zIndex;
        }

        public virtual void SetRoot(GameObject r)
        {
        }

        public virtual void SetFractionBaseOffset(int fractionBaseOffset)
        {
            this.fractionBaseOffset = fractionBaseOffset;
        }

        public virtual void SetColor(Color color)
        {
            this.color = color;
        }

        public virtual void SetElementState(ElementsState state)
        {
            this.state = state;
        }

        public virtual void SetType(ElementsType type)
        {
            this.type = type;
        }

        public virtual void SetMode(InteractionMode mode)
        {
            this.mode = mode;
        }

        public virtual void IncreaseNumerator()
        {
        }

        public virtual void DecreaseNumerator()
        {
        }

        public virtual void IncreaseDenominator()
        {
        }

        public virtual void DecreaseDenominator()
        {
        }

        public virtual void IncreasePartitions()
        {
        }

        public virtual void DecreasePartitions()
        {
        }

        public virtual void SetPartNumerator(int num)
        {
            this.partNumerator = num;
        }

        public virtual void SetPartNumerator(string num)
        {
            this.partNumerator = int.Parse(num);
        }

        public virtual void SetPartDenominator(int den)
        {
            this.partDenominator = den;
            //if (this.partNumerator > this.partDenominator && state != ElementsState.Result)
            //    this.partNumerator = this.partDenominator;
        }

        public virtual void SetNumerator(int numerator)
        {
            this.numerator = numerator;
        }

        public virtual void SetNumerator(string numerator)
        {
            this.numerator = int.Parse(numerator);
        }

        public virtual void SetDenominator(int denominator)
        {
            this.denominator = denominator;
            //if (this.numerator > this.denominator && state != ElementsState.Result)
            //    this.numerator = this.denominator;
        }

        public virtual void SetPartitions(int partitions)
        {
            this.partitions = partitions;
            this.partNumerator = numerator * partitions;
            this.partDenominator = denominator * partitions;
        }

        public virtual void EnableInput()
        {
            this.inputEnabled = true;
        }

        public virtual void DisableInput()
        {
            this.inputEnabled = false;
        }

        public virtual bool CheckCut()
        {
            return true;
        }

        public virtual bool CheckPartition()
        {
            return true;
        }

        public virtual void SetElementScale(float scale)
        {
            elementScale = Mathf.Clamp(scale, minScale, maxScale);
            //width = initialWidth * elementScale;
            //height = initialHeight * elementScale;
        }

        public virtual void SetScaleLimits(float min, float max)
        {
            minScale = min;
            maxScale = max;
        }

        public virtual void SetIsSubFraction(bool flag)
        {
            isSubFraction = flag;
        }
    }
}
