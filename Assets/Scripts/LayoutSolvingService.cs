using UnityEngine;

public class LayoutSolvingService
{
    readonly Camera _camera;

    Rect _viewport = new Rect(0, 0, 1, 1);

    public LayoutSolvingService(Camera camera) {
        _camera = camera;
    }

    public bool InViewport(Bounds bounds) {
        var minPoint = _camera.WorldToViewportPoint(bounds.min);
        var maxPoint = _camera.WorldToViewportPoint(bounds.max);

        var insideMin = minPoint.x >= _viewport.xMin && minPoint.y >= _viewport.yMin;
        var insideMax = maxPoint.x <= _viewport.xMax && maxPoint.y <= _viewport.yMax;

        return insideMin && insideMax;
    }
}