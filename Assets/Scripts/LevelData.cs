using UnityEngine;

[CreateAssetMenu(fileName = "NewLevel", menuName = "Anubis/Level Data")]
public class LevelData : ScriptableObject {
    public NPCData[] npcsInLevel;
    
    [Header("Anubis Deliberation")]
    [TextArea(2, 5)]
    public string[] deliberationDialogue;
}
