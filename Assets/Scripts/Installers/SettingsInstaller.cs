using Settings;
using UnityEngine;
using Zenject;

namespace Installers
{
    [CreateAssetMenu(fileName = "SettingsInstaller", menuName = "Installers/SettingsInstaller")]
    public class SettingsInstaller : ScriptableObjectInstaller<SettingsInstaller>
    {
        public GenerationSettings generationSettings;
        public GameSettings gameSettings;
        
        public override void InstallBindings()
        {
            Container.Bind<GenerationSettings>().FromInstance(generationSettings).AsSingle();
            Container.Bind<GameSettings>().FromInstance(gameSettings).AsSingle();
        }
    }
}