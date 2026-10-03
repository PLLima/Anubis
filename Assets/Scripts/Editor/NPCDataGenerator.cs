#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class NPCDataGenerator {
    [MenuItem("Anubis/Generate NPC Data")]
    public static void GenerateNPCs() {
        
        CreateNPC("Khepri", new string[][] {
            new string[] { "My name was Khepri.", "I sailed from Memphis.", "I traded linen for copper in a society without coins." },
            new string[] { "I was a master of words.", "I convinced people that common lapis amulets were blessed by Ptah." },
            new string[] { "A rival merchant grew tired of my lies.", "He poisoned my beer during a festival.", "I swindled the wrong man." },
        });

        CreateNPC("Nefru", new string[][] {
            new string[] { "My name was Nefru.", "During the flood season, we paid taxes with labor.", "We hauled stones for the temples." },
            new string[] { "I was incredibly strong.", "I hated delays.", "Give me a task, and I just want it done." },
            new string[] { "I ordered my crew to use frayed lifting ropes.", "I wanted to finish early.", "A pillar fell and crushed my chest." }
        });

        CreateNPC("Hori", new string[][] {
            new string[] { "I am Hori of Heliopolis.", "I managed the state granaries.", "I tracked every sack with my reed pen." },
            new string[] { "I kept hidden copies of ledgers.", "I never trusted the honesty of other men." },
            new string[] { "I confronted a corrupt priest alone with my evidence.", "His thugs simply drowned me in a canal." },
        });

        CreateNPC("Menna", new string[][] {
            new string[] { "I am Menna.", "I led trade caravans beyond the southern borders.", "I brought back gold and myrrh." },
            new string[] { "I spoke the tongues of Nubian kings.", "The one who controls the language controls the deal." },
            new string[] { "To save time, I ignored local guides.", "I marched my caravan directly into a massive sandstorm.", "We were buried alive."}
        });

        CreateNPC("Bakenkhonsu", new string[][] {
            new string[] { "I am Bakenkhonsu.", "I commanded the Medjay patrols.", "We guarded the Pharaoh's gold mines." },
            new string[] { "My laws were unforgiving.", "My brother stole a chisel.", "I sent him to the quarries." },
            new string[] { "I refused to retreat from a massive desert raid.", "Orders strictly forbade losing any gold.", "A spear took my throat."}
        });

        CreateNPC("Ipuwer", new string[][] {
            new string[] { "They called me Ipuwer.", "I prepared bodies for eternity.", "I worked in the purification tents of Abydos." },
            new string[] { "I pulled brains through noses.", "I packed bodies with natron salt.", "It is a precise art." },
            new string[] { "I grew too comfortable.", "I forgot to clean my bronze blade.", "I cut my hand on a diseased lung.", "The rot spread to my blood.",}
        });

        CreateNPC("Kiya", new string[][] {
            new string[] { "My name was Kiya.", "I shook the sistrum for the gods.", "My true audience was the royal court." },
            new string[] { "Musicians are invisible.", "I gathered many dark secrets.", "I heard viziers whispering treason." },
            new string[] { "I tried to blackmail a general with my secrets.", "My next cup of lotus wine was poisoned." },
        });

        CreateNPC("Senenmut", new string[][] {
            new string[] { "I am Senenmut.", "I managed the irrigation canals.", "A broken dam meant the entire village would starve." },
            new string[] { "I had to think fast when floods rose.", "I once saved the harvest by building a mud wall in the dark." },
            new string[] { "I refused to let my workers secure a heavy sluice gate.", "I insisted on doing it myself.", "I slipped and drowned in the current." },
        });

        CreateNPC("Ahmose", new string[][] {
            new string[] { "Call me Ahmose.", "I lived in the workers' village.", "I carved royal tombs in the Valley of the Kings." },
            new string[] { "Unofficially, I knew exactly how to bypass the false doors.", "I stole many lapis amulets." },
            new string[] { "I bragged about my thefts to a friend.", "He betrayed me for a reward.", "The guards impaled me." },
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