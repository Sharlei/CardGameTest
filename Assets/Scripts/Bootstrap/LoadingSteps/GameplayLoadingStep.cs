using Cysharp.Threading.Tasks;
using Logic.Controller;
using UnityEngine;
using Zenject;


namespace Bootstrap.LoadingSteps
{
    [CreateAssetMenu(menuName = "Game/Loading/Gameplay Step")]
    public class GameplayLoadingStep : GameLoadingStep
    {
        private HandController handController;
        
        [Inject]
        private void Construct(HandController handController)
        {
            this.handController = handController;
        }
        
        protected override async UniTask LoadStepInternal()
        {
            await handController.InflateHand();
        }
    }
}
