using UnityEngine;

namespace Util
{
    public class NoteMonoBehavior : MonoBehaviour
    {
        [SerializeField]
        [TextArea(3, 10)]
        private string _devNote;
    }
}