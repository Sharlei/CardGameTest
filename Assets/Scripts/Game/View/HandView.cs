using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Model;
using UniRx;
using UnityEngine;

namespace Game.View
{
    //todo use objectpool for cards
    public class HandView : MonoBehaviour
    {
        public Subject<CardView> OnCardDragStarted => onCardDragStarted;
        public Subject<CardView> OnCardDragFinished => onCardDragFinished;
        
        [SerializeField] private GameObject cardPrefab;
        
        private List<CardView> cardViews = new List<CardView>();
        private readonly Subject<CardView> onCardDragStarted = new Subject<CardView>();
        private readonly Subject<CardView> onCardDragFinished = new Subject<CardView>();

        public void AddCard(CardView cardView)
        {
            cardViews.Add(cardView);
            cardView.transform.SetParent(transform);
        }
        
        public void CreateCard(CardModel cardModel)
        {
            var card = Instantiate(cardPrefab.gameObject, transform).GetComponent<CardView>();
            cardViews.Add(card);
            card.SetModel(cardModel);
            card.OnDragStarted.AsObservable().Subscribe(card =>
            {
                if (cardViews.Remove(card))
                    onCardDragStarted.OnNext(card);
            }).AddTo(card);
            
            card.OnDragFinished.AsObservable().Subscribe(onCardDragFinished.OnNext).AddTo(card);
        }
        
        public void DeleteCard(CardModel cardModel)
        {
            DeleteWhenNotBlocked(cardModel).Forget();
        }

        private async UniTask DeleteWhenNotBlocked(CardModel cardModel)
        {
            var cardToRemove = cardViews.Find(card => card.Model == cardModel);
            if (cardToRemove == null) return;
            
            await UniTask.WaitUntil(() => !cardToRemove.IsBlocked);
            cardViews.Remove(cardToRemove);
            Destroy(cardToRemove.gameObject);
        }
    }
}
