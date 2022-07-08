using UnityEngine;

namespace Pluggables
{
    [CreateAssetMenu(fileName = "PlugMidAffector", menuName = "ScriptableObjects/PluggableMidAffector", order = 1)]
    public class MidAffectorSuper : PluggablesSO
    {
        public MidAffectorType type;
    }
}