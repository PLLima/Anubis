using UnityEngine;

[System.Serializable]
public class DialogueBubble {
    [Tooltip("Each string is a clickable snippet")]
    [TextArea(2, 3)]
    public string[] snippets; 
}

[CreateAssetMenu(fileName = "NewNPC", menuName = "Anubis/NPC Data")]
public class NPCData : ScriptableObject {
    public string npcName;
    public Sprite npcSprite;
    public DialogueBubble[] dialogueBubbles;
}

[CreateAssetMenu(fileName = "NewLevel", menuName = "Anubis/Level Data")]
public class LevelData : ScriptableObject {
    public NPCData[] npcsInLevel;
}
