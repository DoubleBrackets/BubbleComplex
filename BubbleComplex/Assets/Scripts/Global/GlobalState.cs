using UnityEngine;

namespace Global
{
    [CreateAssetMenu(fileName = "GlobalStateSO")]
    public class GlobalState : NoteSO
    {
        [field: SerializeField]
        public Vector2 PlayerPos { get; set; }
    }
}