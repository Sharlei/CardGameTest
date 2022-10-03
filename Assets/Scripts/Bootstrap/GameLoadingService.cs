using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Bootstrap.Interfaces;
using Cysharp.Threading.Tasks;
using UniRx;

namespace Bootstrap
{
    public class GameLoadingService : IGameLoadingService
    {
        private readonly List<GameLoadingStep> _steps;
        private readonly float _loadingWeight = 0;
        private float _currentLoading = 0;
        private Subject<float> _loadingProgress;

        public GameLoadingService(List<GameLoadingStep> steps)
        {
            _steps = steps;
            _loadingWeight = steps.Where(x => x.LoadingPolicy != LoadingPolicy.Forgotten).Sum(x => x.StepWeight);
        }

        public async UniTask<bool> LoadGame(Subject<float> loadingProgress)
        {
            _loadingProgress = loadingProgress;
            var stopwatch = Stopwatch.StartNew();
            
            var asyncSteps = _steps.FindAll(s => s.LoadingPolicy == LoadingPolicy.Async);
            var forgottenSteps = _steps.FindAll(s => s.LoadingPolicy == LoadingPolicy.Forgotten);
            var syncSteps = _steps.FindAll(s => s.LoadingPolicy == LoadingPolicy.Sync);
            
            _steps.Clear();
            _steps.AddRange(asyncSteps);
            _steps.AddRange(forgottenSteps);
            _steps.AddRange(syncSteps);
            
            foreach (var step in _steps)
            {
                switch (step.LoadingPolicy)
                {
                    case LoadingPolicy.Async:
                    case LoadingPolicy.Forgotten:
                        step.LoadStep(OnStepLoaded).Forget();
                        break;
                    case LoadingPolicy.Sync:
                        await step.LoadStep(OnStepLoaded);
                        break;
                }
            }

            await UniTask.WaitUntil(() => _currentLoading / _loadingWeight >= 1);
            stopwatch.Stop();

            return true;
        }

        private void OnStepLoaded(GameLoadingStep step)
        {
            if (step.LoadingPolicy == LoadingPolicy.Forgotten) return;
            
            _currentLoading += step.StepWeight;
            var progressValue = _currentLoading / _loadingWeight;
            _loadingProgress.OnNext(progressValue);
            Log($"Game loading progress : {progressValue.ToString(CultureInfo.CurrentCulture)}");
        }

        public static void Log(string message) => UnityEngine.Debug.Log($"<b><color=green>{message}</color></b>");
    }
}