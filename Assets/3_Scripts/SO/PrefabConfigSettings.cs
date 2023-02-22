using UnityEditor.Animations;
using UnityEngine;

namespace _3_Scripts.SO
{
    [CreateAssetMenu(fileName = "PrefabConfigSettings", menuName = "PrefabConfigSettings")]
    public class PrefabConfigSettings : ScriptableObject
    {
        public AnimatorController animator;
        public float colliderHeight = 1.1f;
        public float colliderRadius = 0.2f;
    }
}