using UnityEngine;

public class NoteSO : ScriptableObject
{
    [SerializeField]
    [TextArea(3, 10)]
    private string _devNote;
}