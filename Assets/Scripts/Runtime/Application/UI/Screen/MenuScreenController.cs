using System;
using System.Collections.Generic;
using System.Threading;
using Application.Services;
using Application.Services.UserData;
using Core;
using Core.Factory;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Application.UI
{
    public class MenuScreenController : BaseController
    {
        private readonly List<LevelButtonView> _levelButtonViews = new List<LevelButtonView>();
        private readonly MenuScreenModel _model;
        private readonly GameObjectFactory _factory;
        private readonly LevelButtonController _levelButtonController;
        private readonly IUiService _uiService;
        private readonly UserDataService _userDataService;

        private MenuScreenView _view;

        public event Action StartGameEvent;
        public event Action SettingsButtonPressEvent;
        public event Action ComingSoonEvent;
        public event Action ExitEvent;

        public MenuScreenController(GameObjectFactory factory, UserDataService userDataService,
            LevelButtonController levelButtonController, IUiService uiService)
        {
            _userDataService = userDataService;
            _uiService = uiService;
            _levelButtonController = levelButtonController;
            _factory = factory;
            _model = new MenuScreenModel(userDataService);
        }

        public override async UniTask Run(CancellationToken cancellationToken)
        {
            await base.Run(cancellationToken);

            _view = _uiService.GetScreen<MenuScreenView>(ConstScreens.MenuScreen);
            await _uiService.ShowScreen(ConstScreens.MenuScreen, cancellationToken);

            _view.Initialize();
            _view.SettingsPressedEvent += HandleSettingsPress;
            _view.ExitPressedEvent += HandleExit;
        }

        public override UniTask Stop()
        {
            ClearViews();

            _uiService.HideScreen(ConstScreens.MenuScreen);
            return base.Stop();
        }

        public async UniTask CreateLevels(CancellationToken cancellationToken)
        {
            await UniTask.Yield();
            _model.LoadLevels();

            for (var level = 0; level < _userDataService.GetUserData().LevelsData.Levels.Count; level++)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var buttonView = await _factory.Create<LevelButtonView>(ConstGame.GoButton);
                buttonView.transform.SetParent(_view.ButtonContainer);
                buttonView.transform.localScale = Vector3.one;
                _levelButtonViews.Add(buttonView);

                var levelModel = _model.GetLevelModel(level);
                _levelButtonController.SetButton(buttonView, levelModel);

                if (!levelModel.IsEndedLevels)
                {
                    buttonView.HandleClickEvent += HandlePlay;
                }
                else
                {
                    buttonView.HandleClickEvent += HandleComingSoon;
                }
            }

            _levelButtonController.InitializeButtons();
        }

        private void ClearViews()
        {
            _view.SettingsPressedEvent -= HandleSettingsPress;
            _view.ExitPressedEvent -= HandleExit;

            if (_levelButtonViews != null)
            {
                foreach (var levelButtonView in _levelButtonViews)
                {
                    levelButtonView.HandleClickEvent -= HandleExit;
                    levelButtonView.HandleClickEvent -= HandleComingSoon;
                }

                _levelButtonViews.Clear();
            }

            _model.ClearLevels();
        }

        private void HandlePlay()
        {
            StartGameEvent?.Invoke();
        }

        private void HandleComingSoon()
        {
            ComingSoonEvent?.Invoke();
        }

        private void HandleExit()
        {
            ExitEvent?.Invoke();
        }

        private void HandleSettingsPress()
        {
            SettingsButtonPressEvent?.Invoke();
        }
    }
}