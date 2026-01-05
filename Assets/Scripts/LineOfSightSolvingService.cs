using DCFApixels;
using UnityEngine;

public class LineOfSightSolvingService
{
    readonly Transform _start;
    readonly Transform _end;
    readonly Color     _debugColor;

    public LineOfSightSolvingService(Transform start, Transform end, Color debugColor) {
        _start      = start;
        _end        = end;
        _debugColor = debugColor;
    }

    public void DrawDebugLine() {
        DebugX.Draw(_debugColor).LineArrow(_start.position, _end.position);
    }

    public float GetPreferAngle(Vector2 center, float radius) {
        var startPosition = (Vector2)_start.position;
        var endPosition   = (Vector2)_end.position;


        var u                 = (endPosition - startPosition).normalized;
        var startToCenter     = center - startPosition;
        var proj              = Vector2.Dot(startToCenter, u);
        var closestPointOnRay = startPosition + (proj <= 0 ? Vector2.zero : u * proj);

        var distCenterToRay = Vector2.Distance(closestPointOnRay, center);
        var isIntersecting  = distCenterToRay <= radius;

        Vector2 bestPoint;

        if (!isIntersecting) {
            var centerToClose = closestPointOnRay - center;
            bestPoint = center + centerToClose.normalized * radius;
        }
        else {
            var L = startPosition - center;

            var a     = 1.0f;
            var b     = 2.0f * Vector2.Dot(L, u);
            var c     = Vector2.Dot(L, L) - (radius * radius);
            var delta = b * b - 4 * a * c;

            if (delta < 0) {
                var centerToClose = closestPointOnRay - center;
                bestPoint = center + centerToClose.normalized * radius;
            }
            else {
                var sqrDelta = Mathf.Sqrt(delta);
                var t1       = (-b - sqrDelta) / 2.0f;
                var t2       = (-b + sqrDelta) / 2.0f;

                var final = 0f;
                if (t1 >= 0) final      = t1;
                else if (t2 >= 0) final = t2;

                bestPoint = startPosition + (u * final);
            }
        }

        var dir    = bestPoint - center;
        var rad    = Mathf.Atan2(dir.y, dir.x);
        var degree = rad * Mathf.Rad2Deg;

        return degree;
    }
}