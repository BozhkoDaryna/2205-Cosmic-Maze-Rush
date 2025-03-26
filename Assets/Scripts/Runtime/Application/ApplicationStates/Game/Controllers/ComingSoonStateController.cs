using System.Threading;
using Application.Game;
using Application.UI;
using Core;
using Core.StateMachine;
using Core.UI;
using Cysharp.Threading.Tasks;

namespace Application.RocketPigs
{
    public class ComingSoonStateController : StateController
    {
        private readonly IUiService _uiService;

        private ComingSoonPopup _comingSoonPopup;

        public ComingSoonStateController(IUiService uiService, ILogger logger) : base(logger)
        {
            _uiService = uiService;
        }

        public override UniTask Enter(CancellationToken cancellationToken = default)
        {
            _comingSoonPopup = _uiService.GetPopup<ComingSoonPopup>(ConstPopups.ComingSoonPopup);
            _comingSoonPopup.Show(default, cancellationToken);

            if (_comingSoonPopup != null)
                _comingSoonPopup.CancelledEvent += GoToMenuController;

            return UniTask.CompletedTask;
        }

        public override UniTask Exit()
        {
            _comingSoonPopup.CancelledEvent -= GoToMenuController;
            return base.Exit();
        }

        private void GoToMenuController()
        {
            GoTo<MenuStateController>();
        }
    }
}