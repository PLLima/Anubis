using UnityEngine;

[CreateAssetMenu(fileName = "NewNPC", menuName = "Anubis/NPC Data")]
public class NPCData : ScriptableObject {
    public string npcName;
    public Sprite npcSprite;
    public DialogueBubble[] dialogueBubbles;
}
