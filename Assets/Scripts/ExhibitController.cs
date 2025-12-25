using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class ExhibitController : MonoBehaviour
    {
        [SerializeField]
        public Collider2D _collider;

        [SerializeField]
        LayerMask obstacle;

        public void SetObstacle() {
            gameObject.layer = obstacle;
        }
    }
}