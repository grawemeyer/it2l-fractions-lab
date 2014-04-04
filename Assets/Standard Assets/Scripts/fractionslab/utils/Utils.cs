using System;
using System.Collections.Generic;
using UnityEngine;
using SBS.Math;

namespace fractionslab.utils
{
    public enum InteractionMode
    {
        Initializing = 0,
        Moving,
        Changing,
        Partitioning,
        Scaling,
        Freeze,
        Wait
    }

    public enum ActionType
    {
        Join = 0,
        TakingAway,
        Compare
    }

    public enum FractionPart
    {
        Numerator = 0,
        Denominator
    }

    public enum ElementsType
    {
        VRect = 0,
        HRect,
        Line,
        Liquid
    }

    public enum ElementsState
    {
        Fraction = 0,
        Cut,
        Result,
        Improper
    }

    public struct BBExtend
    {
        public float left;
        public float right;
        public float top;
        public float bottom;

        public BBExtend(float l, float t, float r, float b)
        {
            left = l;
            right = r;
            top = t;
            bottom = b;
        }
    }

    public struct Element
    {
        public int numerator;
        public int denominator;
        public int partNumerator;
        public int partDenominator;
        public int partitions;
        public ElementsState state;
        public ElementsType type;
        public SBSVector3 position;
        public Color color;
    }

    public class Tweener
    {
        static protected List<GameObject> tweenGOList = new List<GameObject>();

        protected static iTweenAnimator CreateNewTween()
        {
            GameObject GO = new GameObject("tweenGO");
            iTweenAnimator tween = GO.AddComponent<iTweenAnimator>();

            tweenGOList.Add(GO);
            return tween;
        }

        public static iTweenAnimator CreateNewTween(Vector3 from, Vector3 to, float time, string easeType, float delay, Action<object> onStart, Action<object> onUpdate, Action<object> onComplete, bool reset = false)
        {
            iTweenAnimator tween = CreateNewTween();
            tween.onStart = onStart;
            tween.onUpdate = onUpdate;
            tween.onComplete = onComplete;
            iTween.ValueTo(tween.gameObject, iTween.Hash("from", from, "to", to,
                "time", time, "easeType", easeType, "loopType", "none", "delay", delay,
                "onstart", (Action<object>)tween.onStart, "onupdate", (Action<object>)tween.onUpdate, "oncomplete", (Action<object>)tween.onComplete));

            if (reset)
                tween.onUpdate((object)from);
            return tween;
        }

        public static void StopAndDestroyAllTweens()
        {
            if (tweenGOList == null)
                return;

            iTween.Stop();

            for (int i = tweenGOList.Count - 1; i >= 0; --i)
            {
                GameObject.DestroyImmediate(tweenGOList[i]);
                tweenGOList.RemoveAt(i);
            }
            tweenGOList.Clear();
        }
    }
}
