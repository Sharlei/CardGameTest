using UniRx;
using UnityEngine;

namespace Game.Model
{
    public enum CardPropertyType
    {
        Mana,
        Health,
        Damage
    }
    public class CardModel
    {
        public bool IsDead => health.Value < 1;
        public IReactiveProperty<int> Mana => mana;
        public IReactiveProperty<int> Health => health;
        public IReactiveProperty<int> Damage => damage;
        
        public readonly string name;
        public readonly string description;
        public readonly Texture2D image;
        
        private ReactiveProperty<int> mana = new ReactiveProperty<int>();
        private ReactiveProperty<int> health = new ReactiveProperty<int>();
        private ReactiveProperty<int> damage = new ReactiveProperty<int>();
        
        public CardModel(string name, string description, int mana, int health, int damage, Texture2D image)
        {
            this.name = name;
            this.description = description;
            this.image = image;
            
            this.mana.Value = mana;
            this.health.Value = health;
            this.damage.Value = damage;
        }

        public void ChangeProperty(CardPropertyType propertyType, int value)
        {
            switch (propertyType)
            {
                case CardPropertyType.Mana:
                    mana.SetValueAndForceNotify(value);
                    break;
                case CardPropertyType.Health:
                    health.SetValueAndForceNotify(value);
                    break;
                case CardPropertyType.Damage:
                    damage.SetValueAndForceNotify(value);
                    break;
            }
        }
    }
}
