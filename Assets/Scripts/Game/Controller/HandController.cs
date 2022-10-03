using System;
using Cysharp.Threading.Tasks;
using Game.Controller;
using Game.Model;
using Game.View;
using Settings;
using UniRx;
using Zenject;
using Random = UnityEngine.Random;

namespace Logic.Controller
{
    public class HandController : IInitializable, IDisposable
    {
        public Subject<CardView> OnCardDragStarted => handView.OnCardDragStarted;
        public Subject<CardView> OnCardDragFinished => handView.OnCardDragFinished;
        
        private readonly HandModel handModel;
        private readonly HandView handView;
        private readonly BigRedButtonView bigRedButtonView;
        private readonly GenerationSettings generationSettings;
        private readonly CompositeDisposable disposables = new();
        private readonly UnattachedCardController unattachedCardController;
        [Inject]
        public HandController(GenerationSettings generationSettings, 
            HandView handView,
            BigRedButtonView bigRedButtonView, 
            HandModel handModel,
            UnattachedCardController unattachedCardController)
        {
            this.generationSettings = generationSettings;
            this.handView = handView;
            this.bigRedButtonView = bigRedButtonView;
            this.handModel = handModel;
            this.unattachedCardController = unattachedCardController;
        }
        
        public async UniTask InflateHand()
        {
            for (int i = 0; i < generationSettings.maxCardsCount; i++)
            {
                var imageTexture = await ImageDownloader.DownloadImageTexture(generationSettings.ImageUri);
                
                handModel.AddCard(new CardModel(string.Format(generationSettings.defaultName, i),
                    generationSettings.defaultDescription, 
                    Random.Range(generationSettings.minValue, generationSettings.maxValue + 1),
                    Random.Range(generationSettings.minValue, generationSettings.maxValue + 1),
                    Random.Range(generationSettings.minValue, generationSettings.maxValue + 1), 
                    imageTexture));
            }
        }
        
        
        public void Initialize()
        {
            handModel.Cards.ObserveAdd().Subscribe(OnCardAdded).AddTo(disposables);
            handModel.Cards.ObserveRemove().Subscribe(OnCardRemoved).AddTo(disposables);
            bigRedButtonView.OnClicked.Subscribe(_ => MakeSomethingCards()).AddTo(disposables);
        }

        private void MakeSomethingCards()
        {
            handModel.ChangeCardValuesRandomly();
        }

        private void OnCardRemoved(CollectionRemoveEvent<CardModel> removeEvent)
        {
            handView.DeleteCard(removeEvent.Value);
        }

        private void OnCardAdded(CollectionAddEvent<CardModel> addEvent)
        {
            if (unattachedCardController.TryGetFreeCard(addEvent.Value, out var cardView))
            {
                handView.AddCard(cardView);
                cardView.SetDraggable(true);
            }
            else
            {
                handView.CreateCard(addEvent.Value);
            }
        }

        public void Dispose()
        {
            disposables?.Dispose();
        }
        
    }
}
