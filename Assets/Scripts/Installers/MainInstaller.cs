using System.Collections.Generic;
using Bootstrap;
using Game.Controller;
using Game.Model;
using Game.View;
using Logic.Controller;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class MainInstaller : MonoInstaller
    {
        [SerializeField] private List<GameLoadingStep> loadingSteps;
        [SerializeField] private GameObject splashScreen;
        [SerializeField] private HandView handView;
        [SerializeField] private BigRedButtonView bigRedButtonView;
        [SerializeField] private UnattachedCardController unattachedCardController;
        
        public override void InstallBindings()
        {
            foreach (var step in loadingSteps)
                Container.QueueForInject(step);
            
            Container.BindInterfacesAndSelfTo<HandController>().FromNew().AsSingle().WithArguments(handView, bigRedButtonView, unattachedCardController);
            Container.BindInterfacesAndSelfTo<HandModel>().FromNew().AsSingle();
            Container.BindInterfacesAndSelfTo<List<GameLoadingStep>>().FromInstance(loadingSteps);
            Container.BindInterfacesAndSelfTo<GameLoadingService>().FromNew().AsTransient();
            Container.BindInterfacesAndSelfTo<GameBootstrap>().AsSingle().WithArguments(splashScreen);
        }
    }
}