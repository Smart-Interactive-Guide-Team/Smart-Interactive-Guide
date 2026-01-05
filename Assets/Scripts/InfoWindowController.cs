using System;
using System.Linq;
using DCFApixels;
using MoreLinq;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class InfoWindowController : MonoBehaviour
{
    [SerializeField]
    Collider2D _collider;

    readonly Collider2D[] _overlapResult = new Collider2D[3];

    InfoWindowConfig  _config;
    ExhibitController _focus;

    [ShowInInspector, ReadOnly]
    Guid _guid = Guid.NewGuid();

    LayoutSolvingService      _layoutSolvingService;
    LineOfSightSolvingService _lineOfSightSolvingService;
    Vector2                   _velocity;

    public void Construct(
        InfoWindowConfig          config,
        LayoutSolvingService      layoutSolvingService,
        LineOfSightSolvingService lineOfSightSolvingService,
        Color                     debugColor
    ) {
        _config                     = config;
        _layoutSolvingService       = layoutSolvingService;
        _lineOfSightSolvingService  = lineOfSightSolvingService;
        GetComponent<Image>().color = debugColor;

        Observable
            .EveryUpdate(UnityFrameProvider.FixedUpdate)
            .Subscribe(_ => {
                if (!_focus) {
                    return;
                }

                var bestAngle = DetectBestAngle();
                UpdateWindowPosition(bestAngle);
            })
            .AddTo(this);

        Observable
            .EveryUpdate()
            .Subscribe(_ => {
                if (!_focus) return;

                DebugX.Draw(debugColor).WidthLine(transform.position, _focus.Center, 0.1f);
            });
    }

    [Button]
    public void SetFocus(ExhibitController target) {
        _focus?.SetObstacle(false);

        _focus = target;
        _focus.SetObstacle(true);
    }

    void UpdateWindowPosition(float bestAngle) {
        var offset   = AngleToVector(bestAngle) * _config.orbitRadius;
        var position = (Vector2)_focus.transform.position + offset;
        transform.position =
            Vector2.SmoothDamp(
                transform.position,
                position,
                ref _velocity,
                _config.moveSmoothTime
            );
    }

    float DetectBestAngle() {
        var preferAngle = _lineOfSightSolvingService.GetPreferAngle(_focus.Center, _config.orbitRadius);

        for (var deltaAngle = 0; deltaAngle < 180; deltaAngle += 15) {
            var testAngles = new[] { preferAngle - deltaAngle, preferAngle + deltaAngle };
            foreach (var angle in testAngles) {
                var offset = AngleToVector(angle) * _config.orbitRadius;
                var center = (Vector2)_focus.transform.position + offset;
                var size   = _collider.bounds.size;
                var bounds = new Bounds(center, size);

                var any        = OverlapAnyValidObstacle(bounds);
                var inViewport = _layoutSolvingService.InViewport(bounds);

                DrawDebugCube(any, inViewport, bounds);

                var notValid = any || !inViewport;

                if (notValid) {
                    continue;
                }

                return angle;
            }
        }

        Debug.LogWarning("No best position");

        return preferAngle;
    }

    static void DrawDebugCube(bool any, bool inViewport, Bounds bounds) {
        var debugColor = Color.green;
        if (any) {
            debugColor = Color.red;
        }

        if (!inViewport) {
            debugColor = Color.blue;
        }

        DebugX
            .Draw(debugColor)
            .WireCube(bounds.center, Quaternion.identity, bounds.size);
    }

    bool OverlapAnyValidObstacle(Bounds bounds) {
        Array.Clear(_overlapResult, 0, 3);
        Physics2D
            .OverlapBoxNonAlloc(
                bounds.center,
                bounds.size,
                0,
                _overlapResult,
                _config.obstacleLayer
            );

        var any =
            _overlapResult
                .Where(c => {
                    if (!c) return false;

                    var notSelf      = c != _collider;
                    var notSelfFocus = c != _focus.collider;

                    return notSelf && notSelfFocus;
                })
                .Any();

        return any;
    }

    Vector2 AngleToVector(float angle) {
        var rad    = angle * Mathf.Deg2Rad;
        var vector = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

        return vector;
    }
}