#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class NPCDataGenerator {
    [MenuItem("Anubis/Generate NPC Data")]
    public static void GenerateNPCs() {
        
        CreateNPC("Khepri", new string[][] {
            new string[] { "My name was Khepri.", "I sailed from Memphis, trading linen for copper in a society without coins." },
            new string[] { "I was a master of words, convincing people that common lapis amulets were blessed by Ptah." },
            new string[] { "But a rival merchant grew tired of my lies and poisoned my beer during a festival." },
            new string[] { "I swindled the wrong man. I make bitter enemies because I always prioritize my own profit over ethics." }
        });

        CreateNPC("Nefru", new string[][] {
            new string[] { "My name was Nefru.", "During the flood season, we paid taxes by hauling stones for the temples." },
            new string[] { "I was incredibly strong, but I hated delays. Give me a task, and I just want it done." },
            new string[] { "I ordered my crew to use frayed lifting ropes to finish early. A pillar fell and crushed my chest." },
            new string[] { "I am reckless. I value finishing the job quickly over planning and safety." }
        });

        CreateNPC("Hori", new string[][] {
            new string[] { "I am Hori of Heliopolis.", "I managed the state granaries, tracking every sack with my reed pen." },
            new string[] { "I kept hidden copies of ledgers because I never trusted the honesty of other men." },
            new string[] { "I confronted a corrupt priest alone with my evidence. His thugs simply drowned me in a canal." },
            new string[] { "I blindly follow the rules, but I lack strategic sense. I act without thinking of survival." }
        });

        CreateNPC("Menna", new string[][] {
            new string[] { "I am Menna.", "I led trade caravans beyond the southern borders to bring back gold and myrrh." },
            new string[] { "I spoke the tongues of Nubian kings. The one who controls the language controls the deal." },
            new string[] { "To save time, I ignored local guides and marched my caravan directly into a massive sandstorm." },
            new string[] { "We were buried alive. I am arrogant and refuse to listen to advice from those beneath me." }
        });

        CreateNPC("Bakenkhonsu", new string[][] {
            new string[] { "I am Bakenkhonsu.", "I commanded the Medjay patrols guarding the Pharaoh's gold mines." },
            new string[] { "My laws were unforgiving. When my brother stole a chisel, I sent him to the quarries." },
            new string[] { "I refused to retreat from a massive desert raid because orders strictly forbade losing any gold." },
            new string[] { "A spear took my throat. I am completely inflexible and will throw away lives for a rule." }
        });

        CreateNPC("Ipuwer", new string[][] {
            new string[] { "They called me Ipuwer.", "I prepared bodies for eternity in the purification tents of Abydos." },
            new string[] { "I pulled brains through noses and packed bodies with natron salt. It is a precise art." },
            new string[] { "But I grew too comfortable. I forgot to clean my bronze blade and cut my hand on a diseased lung." },
            new string[] { "The rot spread to my blood. I become dangerously careless when tasks feel too routine." }
        });

        CreateNPC("Kiya", new string[][] {
            new string[] { "My name was Kiya.", "I shook the sistrum for the gods, but my true audience was the royal court." },
            new string[] { "Musicians are invisible. I gathered many dark secrets whispered between viziers." },
            new string[] { "I tried to blackmail a general with what I knew. My next cup of lotus wine was poisoned." },
            new string[] { "I am too greedy with information. I overplay my hand and become a liability." }
        });

        CreateNPC("Senenmut", new string[][] {
            new string[] { "I am Senenmut.", "I managed the irrigation canals. A broken dam meant the entire village would starve." },
            new string[] { "I had to think fast when floods rose. I once saved the harvest by building a mud wall in the dark." },
            new string[] { "But I refused to let my workers secure a heavy sluice gate, insisting on doing it myself." },
            new string[] { "I slipped and drowned in the current. I micromanage and refuse to trust others with the real work." }
        });

        CreateNPC("Ahmose", new string[][] {
            new string[] { "Call me Ahmose.", "I lived in the workers' village, carving royal tombs in the Valley of the Kings." },
            new string[] { "Unofficially, I knew exactly how to bypass the false doors, and I stole many lapis amulets." },
            new string[] { "I bragged about my thefts to a friend, who betrayed me for a reward. The guards impaled me." },
            new string[] { "I am fundamentally dishonest. I will steal from my employer and I choose my allies poorly." }
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