using UnityEngine;

[System.Serializable]
public class DialogueBubble {
    [Tooltip("Each string is a clickable snippet")]
    [TextArea(2, 3)]
    public string[] snippets; 
}
