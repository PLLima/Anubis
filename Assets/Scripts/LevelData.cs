using UnityEngine;

[CreateAssetMenu(fileName = "NewLevel", menuName = "Anubis/Level Data")]
public class LevelData : ScriptableObject {
    public NPCData[] npcsInLevel;
    
    [Header("Winning Condition")]
    [Tooltip("The NPC that yields a victory if chosen.")]
    public NPCData correctNPC;
    
    [Header("Anubis Deliberation")]
    [TextArea(2, 5)]
    public string[] deliberationDialogue;
}
