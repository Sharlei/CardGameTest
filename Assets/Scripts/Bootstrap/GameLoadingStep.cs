using System;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Bootstrap
{
    public abstract class GameLoadingStep : ScriptableObject
    {
        public int StepWeight;
        public LoadingPolicy LoadingPolicy;

        public async UniTask LoadStep(Action<GameLoadingStep> onStepLoaded)
        {
            var stopwatch = Stopwatch.StartNew();
            await LoadStepInternal();
            GameLoadingService.Log($"{GetType().Name} successfully loaded");
            stopwatch.Stop();
            GameLoadingService.Log($"{GetType().Name} Step loading time : {stopwatch.ElapsedMilliseconds}");
            onStepLoaded.Invoke(this);
        }

        protected abstract UniTask LoadStepInternal();

    }
}
