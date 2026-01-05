using System;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;

public class EntryPoint : MonoBehaviour
{
    [SerializeField]
    InfoWindowConfig _infoWindowConfig;

    [SerializeField]
    Camera _camera;

    [SerializeField]
    UserSetup[] _userSetups;

    LayoutSolvingService _layoutSolvingService;

    void Awake() {
        _layoutSolvingService = new LayoutSolvingService(_camera);

        foreach (var setup in _userSetups) {
            setup.Initialize(_infoWindowConfig, _layoutSolvingService);

            Observable
                .EveryUpdate()
                .Subscribe(_ => {
                    setup.LineOfSightSolvingService.DrawDebugLine();
                });
        }
    }

    [Serializable]
    public class UserSetup
    {
        [SerializeField]
        Color _debugColor;

        [SerializeField, InlineEditor]
        InfoWindowController _infoWindowController;

        [SerializeField]
        Transform userPoint;

        [SerializeField]
        Transform gazePoint;

        public LineOfSightSolvingService LineOfSightSolvingService { get; private set; }

        public void Initialize(
            InfoWindowConfig     config,
            LayoutSolvingService layoutSolvingService
        ) {
            LineOfSightSolvingService = new LineOfSightSolvingService(userPoint, gazePoint, _debugColor);
            _infoWindowController.Construct(config, layoutSolvingService, LineOfSightSolvingService, _debugColor);
        }
    }
}