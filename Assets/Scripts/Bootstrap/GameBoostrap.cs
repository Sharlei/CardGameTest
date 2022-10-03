using Bootstrap.Interfaces;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Bootstrap
{
    public class GameBootstrap : IInitializable
    {
        public static bool IsGameReady => isGameReady;
        
        private static bool isGameReady;
        
        private readonly GameObject splashScreen;
        private readonly IGameLoadingService gameLoadingService;
   
        public GameBootstrap(GameObject splashScreen, IGameLoadingService gameLoadingService)
        {
            this.splashScreen = splashScreen;
            this.gameLoadingService = gameLoadingService;
        }

        public void Initialize() => InternalInitialize().Forget();

        private async UniTaskVoid InternalInitialize()
        {
            splashScreen.SetActive(true);
            var loadingProgress = new Subject<float>();
            loadingProgress.OnNext(0);
            
            await gameLoadingService.LoadGame(loadingProgress);
            
            isGameReady = true;
            Object.Destroy(splashScreen);
        }
    }
}
