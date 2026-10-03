#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class NPCDataGenerator {
    [MenuItem("Anubis/Generate NPC Data")]
    public static void GenerateNPCs() {
        CreateNPC("Khepri", new string[][] {
            new string[] { "My name was Khepri.", "I came from Memphis." },
            new string[] { "I spent most of my life traveling from city to city.", "I was a merchant of fabrics and jewelry." },
            new string[] { "A good sale was never only about the product.", "I always tried to understand what people wanted to hear." },
            new string[] { "Sometimes I told customers an item was rare, even when I had many more.", "People liked to believe they had gotten a special deal." }
        });

        CreateNPC("Nefru", new string[][] {
            new string[] { "My name was Nefru.", "I came from Thebes." },
            new string[] { "I worked building temples from a young age.", "I could carry stones that many people could not lift alone." },
            new string[] { "Once, I worked for three days with almost no rest.", "I was never very good at talking to people." },
            new string[] { "I preferred to be given a task and simply complete it.", "When there was a problem, I tried to solve it with my own hands." }
        });

        CreateNPC("Hori", new string[][] {
            new string[] { "My name was Hori.", "I was born in Heliopolis." },
            new string[] { "I worked as a scribe for a local administrator.", "I could read, write, and calculate very well." },
            new string[] { "I kept copies of everything I wrote.", "I never completely trusted other people's memories." },
            new string[] { "I once discovered a tax collector stealing grain because the records did not match.", "I preferred to observe carefully before accusing someone." }
        });

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("NPC Data successfully generated in Assets/NPC!");
    }

    private static void CreateNPC(string npcName, string[][] dialogues) {
        NPCData npc = ScriptableObject.CreateInstance<NPCData>();
        npc.npcName = npcName;
        npc.dialogueBubbles = new DialogueBubble[dialogues.Length];

        for (int i = 0; i < dialogues.Length; i++) {
            npc.dialogueBubbles[i] = new DialogueBubble { snippets = dialogues[i] };
        }

        string path = $"Assets/NPC/{npcName}.asset";
        
        if (!AssetDatabase.IsValidFolder("Assets/NPC")) {
            AssetDatabase.CreateFolder("Assets", "NPC");
        }
        
        NPCData existing = AssetDatabase.LoadAssetAtPath<NPCData>(path);
        if (existing != null) {
            EditorUtility.CopySerialized(npc, existing);
        } else {
            AssetDatabase.CreateAsset(npc, path);
        }
    }
}
#endif
