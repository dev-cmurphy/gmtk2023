using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.GameData
{
    [CreateAssetMenu(fileName ="boss_settings", menuName = "GMTK/Boss/Settings", order = -1)]
    public class BossMovementSettings : ScriptableObject
    {
        public float Speed;
    }
}