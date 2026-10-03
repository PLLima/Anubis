using UnityEngine;

[CreateAssetMenu(fileName = "NewLevel", menuName = "Anubis/Level Data")]
public class LevelData : ScriptableObject {
    public NPCData[] npcsInLevel;
}
