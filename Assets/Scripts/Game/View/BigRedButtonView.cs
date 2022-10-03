using UniRx;
using UnityEngine;

namespace Game.View
{
    public class BigRedButtonView : MonoBehaviour
    {
        public Subject<Unit> OnClicked => onClicked;

        private Subject<Unit> onClicked = new Subject<Unit>();
            
        public void OnClick()
        {
            onClicked.OnNext(Unit.Default);
        }
    }
}
