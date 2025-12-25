using Sirenix.OdinInspector;
using UnityEngine;

public class EntryPoint : MonoBehaviour
{
    [SerializeField]
    InfoWindowConfig _infoWindowConfig;

    [SerializeField]
    Camera _camera;

    [SerializeField, InlineEditor]
    InfoWindowController[] _infoWindowControllers;


    LayoutSolvingService _layoutSolvingService;

    void Awake() {
        _layoutSolvingService = new LayoutSolvingService(_camera);

        foreach (var controller in _infoWindowControllers) {
            controller.Construct(_infoWindowConfig, _layoutSolvingService);
        }
    }
}