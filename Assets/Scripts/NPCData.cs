using UnityEngine;

[CreateAssetMenu(fileName = "NewNPC", menuName = "Anubis/NPC Data")]
public class NPCData : ScriptableObject {
    public string npcName;
    public Sprite npcSprite;
    public DialogueBubble[] dialogueBubbles;

    [Header("Voice Settings (Gibberish)")]
    [Tooltip("Voice configuration for this NPC. You can reuse the same audio clips across NPCs and simply change basePitch!")]
    public VoiceProfile voiceProfile;
}
