using UnityEngine;

[CreateAssetMenu(fileName = "NewLevel", menuName = "Anubis/Level Data")]
public class LevelData : ScriptableObject {
    public NPCData[] npcsInLevel;
    [Header("Anubis Startup Concepts")]
    [Tooltip("Possible startups Anubis wants to create for this level. One is chosen at random.")]
    public LevelScenario[] scenarios;
}

[System.Serializable]
public struct LevelScenario
{
    [TextArea(2, 5)]
    [Tooltip("What Anubis says during deliberation for this startup idea.")]
    public string[] deliberationDialogue;

    [Tooltip("The NPC that yields a victory if chosen for this startup.")]
    public NPCData correctNPC;
}
