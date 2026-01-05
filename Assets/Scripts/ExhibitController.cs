using UnityEngine;

public class ExhibitController : MonoBehaviour
{
    [SerializeField]
    public new Collider2D collider;

    [SerializeField, Layer]
    int obstacle;

    public Vector2 Center => collider.bounds.center;


    int _defaultLayer;

    public void SetObstacle(bool isSet) {
        if (isSet) {
            _defaultLayer    = gameObject.layer;
            gameObject.layer = obstacle;
        }
        else {
            gameObject.layer = _defaultLayer;
        }
    }
}