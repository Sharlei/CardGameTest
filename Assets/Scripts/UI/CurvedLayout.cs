using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public enum MoveType
    {
        Hand, Table
    }
    [AddComponentMenu("Layout/Curved Layout")]
    public class CurvedLayout : LayoutGroup
    {
        
        public Vector3 curveOffset;
        public Vector3 itemsAxis;
        public float rotationRange;
        public float itemSize;
        public float centerPoint = 0.5f;
        public float itemsMoveSpeed;
        public MoveType moveType;
        
        private readonly List<Vector3> desiredPositionsForChildren = new List<Vector3>();
        private readonly List<Vector3> desiredRotationsForChildren = new List<Vector3>();

        protected override void OnEnable()
        {
            base.OnEnable();
            CalculatePosition();
        }

        public override void SetLayoutHorizontal() { }
        public override void SetLayoutVertical() { }

        public override void CalculateLayoutInputVertical()
        {
            CalculatePosition();
        }

        public override void CalculateLayoutInputHorizontal()
        {
            CalculatePosition();
            if (Application.isPlaying)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    var child = transform.GetChild(i);
                    if (moveType == MoveType.Hand)
                    {
                        child.DOLocalMove(desiredPositionsForChildren[i], itemsMoveSpeed);
                        child.DOLocalRotate(desiredRotationsForChildren[i], itemsMoveSpeed);
                    }
                    else
                    {
                        child.DOLocalMove(desiredPositionsForChildren[i], itemsMoveSpeed).SetEase(Ease.OutBounce);
                        child.DOLocalRotate(desiredRotationsForChildren[i], itemsMoveSpeed).SetEase(Ease.OutBounce);
                    }
                }
            }
        }
        
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            
            if (Application.isPlaying) return;
            
            CalculatePosition();
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                child.localPosition = desiredPositionsForChildren[i];
                child.localRotation = Quaternion.Euler(desiredRotationsForChildren[i]);
            }
        }
#endif

        void CalculatePosition()
        {
            m_Tracker.Clear();
            var childCount = transform.childCount;

            if (childCount < desiredPositionsForChildren.Count)
            {
                desiredPositionsForChildren.RemoveRange(childCount, desiredPositionsForChildren.Count - childCount);
                desiredRotationsForChildren.RemoveRange(childCount, desiredRotationsForChildren.Count - childCount);
            }

            if (childCount == 0)
                return;

            Vector2 pivot = new Vector2(((int)childAlignment % 3), ((int)childAlignment / 3) * 0.5f);

            Vector3 lastPos = new Vector3(
                rectTransform.sizeDelta.x / 2 - childCount * itemSize / 2,
                GetStartOffset(1, GetTotalPreferredSize(1)),
                0f
            );

            float interpolationValue = 0;
            float step = 1f / childCount;
            float startRotation = -rotationRange / 2;
            float rotationPerStep = rotationRange / childCount;

            var distanceBetweenItems = itemsAxis.normalized * itemSize;

            for (int i = 0; i < childCount; i++)
            {
                RectTransform child = (RectTransform)transform.GetChild(i);
                if (child != null)
                {
                    m_Tracker.Add(this, child,
                        DrivenTransformProperties.Anchors |
                        DrivenTransformProperties.AnchoredPosition |
                        DrivenTransformProperties.Pivot);
                    Vector3 vPos = lastPos + distanceBetweenItems;

                    if (i >= desiredPositionsForChildren.Count)
                    {
                        desiredPositionsForChildren.Add(Vector3.zero);
                        desiredRotationsForChildren.Add(Vector3.zero);
                    }

                    desiredPositionsForChildren[i] = lastPos = vPos + (interpolationValue - centerPoint) * curveOffset;
                    desiredRotationsForChildren[i] = new Vector3(0, 0, -(startRotation + (i + 1) * rotationPerStep));
                  
                    child.pivot = pivot;
                    child.anchorMin = child.anchorMax = new Vector2(0.5f, 0.5f);
                    
                    interpolationValue += step;
                }
            }
        }
    }
}