using System.Collections.Generic;
using Game.Model;
using Game.View;
using Logic.Controller;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Controller
{
    public class UnattachedCardController : MonoBehaviour
    {
        [SerializeField] private RectTransform tableViewRectTransform;

        private HandModel handModel;
        private List<CardView> freeCards = new List<CardView>();

        [Inject]
        private void Construct(HandController handController, HandModel handModel)
        {
            this.handModel = handModel;

            handController.OnCardDragStarted.AsObservable().Subscribe(TryDetachCard).AddTo(this);
            handController.OnCardDragFinished.AsObservable().Subscribe(TryAttachCard).AddTo(this);
        }

        public bool TryGetFreeCard(CardModel cardModel, out CardView cardView)
        { 
            cardView = freeCards.Find(card => card.Model == cardModel);
            return cardView != null;
        }
        
        private void TryDetachCard(CardView cardView)
        {
            if (!handModel.Cards.Contains(cardView.Model)) return;
                
            cardView.transform.SetParent(transform);
            handModel.RemoveCard(cardView.Model);
            freeCards.Add(cardView);
        }
        
        private void TryAttachCard(CardView cardView)
        {
            Vector2 localMousePosition = tableViewRectTransform.InverseTransformPoint(Input.mousePosition);
            if (tableViewRectTransform.rect.Contains(localMousePosition))
            {
                cardView.transform.SetParent(tableViewRectTransform);
                cardView.SetDraggable(false);//todo when TableView class will be created, then it will be its responsibility
                freeCards.Remove(cardView);
            }
            else
            {
                handModel.AddCard(cardView.Model);
            }
        }
    }
}
