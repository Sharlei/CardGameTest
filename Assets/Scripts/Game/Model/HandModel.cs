using System;
using System.Linq;
using Settings;
using UniRx;
using Zenject;
using Random = UnityEngine.Random;

namespace Game.Model
{
    public class HandModel
    {
        public IReactiveCollection<CardModel> Cards => cards;

        private GameSettings gameSettings;
        private readonly ReactiveCollection<CardModel> cards = new ReactiveCollection<CardModel>();

        [Inject]
        private void Construct(GameSettings gameSettings)
        {
            this.gameSettings = gameSettings;
        }
        
        public void AddCard(CardModel cardModel) => cards.Add(cardModel);

        public void RemoveCard(CardModel cardModel) => cards.Remove(cardModel);

        public void ChangeCardValuesRandomly()
        {
            for (var i = 0; i < cards.Count; i++)
            {
                var cardModel = cards[i];
                var cardProperty =  (CardPropertyType)Random.Range(0, Enum.GetValues(typeof(CardPropertyType)).Length);
                cardModel.ChangeProperty(cardProperty, Random.Range(gameSettings.minRandomValue, gameSettings.maxRandomValue +1));
                if (cardModel.IsDead)
                {
                    RemoveCard(cardModel);
                }
            }
        }
    }
}
