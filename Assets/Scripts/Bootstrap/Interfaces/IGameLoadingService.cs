using Cysharp.Threading.Tasks;
using UniRx;

namespace Bootstrap.Interfaces
{
    public interface IGameLoadingService
    {
        UniTask<bool> LoadGame(Subject<float> loadingProgress);
    }
}
