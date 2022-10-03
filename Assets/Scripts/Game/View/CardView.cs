using System;
using System.Threading;
using DG.Tweening;
using Game.Model;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.View
{
    public class CardView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        //todo send event that will be caught by UnattachedCardController instead of using chain of Subjects
        public Subject<CardView> OnDragStarted => onDragStarted;
        public Subject<CardView> OnDragFinished => onDragFinished;
        public bool IsBlocked => isBlocked;
        public RectTransform RectTransform => transform as RectTransform;
        public CardModel Model => cardModel;
        
        //todo better to take counter logic out into separate View class to make CardView not responsible for counter animation
        [SerializeField] private int counterTimeInMilliseconds;
        [SerializeField] private Vector3 counterScale;

        [SerializeField] private Image imageView;
        [SerializeField] private TextMeshProUGUI nameView;
        [SerializeField] private TextMeshProUGUI descriptionView;
        [SerializeField] private TextMeshProUGUI manaView;
        [SerializeField] private TextMeshProUGUI healthView;
        [SerializeField] private TextMeshProUGUI damageView;
        [SerializeField] private Image cardShiningView;
        [SerializeField] private Color shineColor;
        
        private CardModel cardModel;
        private RectTransform parentCanvasRectTransform;
        
        private CancellationTokenSource manaCounterCancellationToken;
        private CancellationTokenSource healthCounterCancellationToken;
        private CancellationTokenSource damageCounterCancellationToken;
        private CompositeDisposable disposables;
        
        private bool isBlocked;
        private readonly Subject<CardView> onDragStarted = new Subject<CardView>();
        private readonly Subject<CardView> onDragFinished = new Subject<CardView>();
        private bool draggable = true;

        private void Awake()
        {
            parentCanvasRectTransform = GetComponentInParent<Canvas>().transform as RectTransform;
        }

        public void SetDraggable(bool draggable) => this.draggable = draggable;
        
        public void SetModel(CardModel cardModel)
        {
            disposables?.Dispose();
            
            disposables = new CompositeDisposable();
            disposables.AddTo(this);
            
            this.cardModel = cardModel;
            
            imageView.sprite = Sprite.Create(cardModel.image, new Rect(0, 0, cardModel.image.width, cardModel.image.height), new Vector2(.5f, .5f));
            nameView.text = cardModel.name;
            descriptionView.text = cardModel.description;
            manaView.text = cardModel.Mana.Value.ToString();
            healthView.text = cardModel.Health.Value.ToString();
            damageView.text = cardModel.Damage.Value.ToString();

            BindViewToModel();
        }

        private void BindViewToModel()
        {
            cardModel.Mana.Subscribe(newMana =>
            {
                manaCounterCancellationToken?.Cancel();
                manaCounterCancellationToken = new CancellationTokenSource();
                manaCounterCancellationToken.AddTo(this);
                ChangeCardIntValue(manaView, newMana, manaCounterCancellationToken.Token);
            }).AddTo(disposables);
            
            cardModel.Health.Subscribe(newHealth =>
            {
                healthCounterCancellationToken?.Cancel();
                healthCounterCancellationToken = new CancellationTokenSource();
                healthCounterCancellationToken.AddTo(this);
                ChangeCardIntValue(healthView, newHealth, healthCounterCancellationToken.Token);
            }).AddTo(disposables);
            
            cardModel.Damage.Subscribe(newDamage =>
            {
                damageCounterCancellationToken?.Cancel();
                damageCounterCancellationToken = new CancellationTokenSource();
                damageCounterCancellationToken.AddTo(this);
                ChangeCardIntValue(damageView, newDamage, damageCounterCancellationToken.Token);
            }).AddTo(disposables);
        }

        private void ChangeCardIntValue(TextMeshProUGUI textViewWithIntValue, int newValue, CancellationToken ct)
        {
            var oldVal = int.Parse(textViewWithIntValue.text);
            var difference = Mathf.Abs(oldVal - newValue);
         
            if (difference == 0) return;
            
            var timeForOneCount = counterTimeInMilliseconds / difference;
            ChangeCounter(textViewWithIntValue, oldVal, newValue, timeForOneCount);
        }

        private void ChangeCounter(TextMeshProUGUI textViewWithIntValue, int oldVal, int newValue, Single timeForOneCount)
        {
            isBlocked = true;
            DOTween.Sequence()
                .SetId(textViewWithIntValue)
                .OnStart(() =>
                {
                    oldVal = Mathf.RoundToInt(Mathf.MoveTowards(oldVal, newValue, 1));
                    textViewWithIntValue.text = oldVal.ToString();
                })
                .Append(textViewWithIntValue.transform.DOPunchScale(counterScale, timeForOneCount * 0.001f, 0, 0.1f))
                .OnKill(() =>
                {
                    textViewWithIntValue.transform.localScale = Vector3.one;
                    if (oldVal != newValue)
                    {
                        ChangeCounter(textViewWithIntValue, oldVal, newValue, timeForOneCount);
                    }
                    else
                    {
                        isBlocked = false;
                    }
                });
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (isBlocked || !draggable) return;

            DOTween.Kill(transform);
            cardShiningView.DOColor(shineColor, .2f);
            onDragStarted.OnNext(this);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (isBlocked || !draggable) return;

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(parentCanvasRectTransform, eventData.position, eventData.pressEventCamera, out var globalMousePos))
            {
                RectTransform.position = globalMousePos + new Vector3(RectTransform.sizeDelta.x / 2,RectTransform.sizeDelta.y / 2);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (isBlocked || !draggable) return;
            
            cardShiningView.DOColor(Color.black, .2f);
            onDragFinished.OnNext(this);
        }
    }
}
