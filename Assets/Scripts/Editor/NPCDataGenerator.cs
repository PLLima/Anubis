#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class NPCDataGenerator
{
    [MenuItem("Anubis/Generate NPC Data")]
    public static void GenerateNPCs()
    {
        CreateNPC("Khepri", new string[][]
        {
            new string[]
            {
                "My name was Khepri.",
                "I sailed from Memphis.",
                "Trade was how I made my living.",
                "Linen and copper were common goods."
            },
            new string[]
            {
                "Words were my greatest weapon.",
                "I knew what people wanted to hear.",
                "I sold ordinary lapis amulets.",
                "I claimed they were blessed by Ptah."
            },
            new string[]
            {
                "A rival merchant grew tired of my lies.",
                "He poisoned my beer during a festival.",
                "I finally swindled the wrong man."
            },
            new string[]
            {
                "Profit mattered more than honesty.",
                "That earned me many enemies.",
                "Ethics rarely stopped a good deal."
            }
        });

        CreateNPC("Nefru", new string[][]
        {
            new string[]
            {
                "My name was Nefru.",
                "I hauled stones for the temples.",
                "Labor was how many of us paid taxes."
            },
            new string[]
            {
                "Strength was never a problem for me.",
                "I could not stand delays.",
                "I wanted every task finished quickly."
            },
            new string[]
            {
                "My crew used frayed lifting ropes.",
                "I refused to replace them.",
                "New ropes would have slowed us down.",
                "A pillar fell and crushed me."
            },
            new string[]
            {
                "Speed mattered more than planning.",
                "I often ignored safety.",
                "Impatience was my greatest weakness."
            }
        });

        CreateNPC("Hori", new string[][]
        {
            new string[]
            {
                "I am Hori of Heliopolis.",
                "I managed the state granaries.",
                "Every sack passed through my records."
            },
            new string[]
            {
                "I kept hidden copies of the ledgers.",
                "I did not trust other people's memories.",
                "Written evidence felt safer."
            },
            new string[]
            {
                "My records exposed a corrupt priest.",
                "I confronted him alone.",
                "His thugs drowned me in a canal."
            },
            new string[]
            {
                "Rules guided almost everything I did.",
                "Proof mattered more than instinct.",
                "Strategy was never my strength.",
                "Being right mattered more than survival."
            }
        });

        CreateNPC("Menna", new string[][]
        {
            new string[]
            {
                "I am Menna.",
                "I led trade caravans south.",
                "We brought back gold and myrrh."
            },
            new string[]
            {
                "I learned the tongues of Nubian kings.",
                "Language gave me an advantage.",
                "Control the talk, control the deal."
            },
            new string[]
            {
                "I wanted to save time.",
                "The local guides warned me.",
                "I ignored them.",
                "A sandstorm buried my caravan."
            },
            new string[]
            {
                "Advice rarely impressed me.",
                "I looked down on local guides.",
                "I trusted my own judgment too much."
            }
        });

        CreateNPC("Bakenkhonsu", new string[][]
        {
            new string[]
            {
                "I am Bakenkhonsu.",
                "I commanded the Medjay patrols.",
                "We guarded the Pharaoh's gold mines."
            },
            new string[]
            {
                "My laws left little room for mercy.",
                "My brother once stole a chisel.",
                "I sent him to the quarries."
            },
            new string[]
            {
                "Raiders attacked us in the desert.",
                "Retreat could have saved us.",
                "Orders forbade losing any gold.",
                "So I held the position."
            },
            new string[]
            {
                "Rules mattered more than circumstances.",
                "Once ordered, I refused to bend.",
                "Even lives could be sacrificed."
            }
        });

        CreateNPC("Ipuwer", new string[][]
        {
            new string[]
            {
                "They called me Ipuwer.",
                "I prepared the dead for eternity.",
                "I worked in Abydos."
            },
            new string[]
            {
                "Brains came out through the nose.",
                "Bodies were packed with natron.",
                "The work demanded precision."
            },
            new string[]
            {
                "Years of repetition made me careless.",
                "I forgot to clean my bronze blade.",
                "I cut my hand on a diseased lung.",
                "The infection killed me."
            },
            new string[]
            {
                "Difficult work kept me alert.",
                "Routine work made me careless.",
                "Familiarity made me overconfident."
            }
        });

        CreateNPC("Kiya", new string[][]
        {
            new string[]
            {
                "My name was Kiya.",
                "I played the sistrum at court.",
                "Most people barely noticed musicians."
            },
            new string[]
            {
                "That made people careless around me.",
                "Viziers whispered secrets nearby.",
                "I remembered what they said."
            },
            new string[]
            {
                "I tried to blackmail a general.",
                "I thought his secrets gave me power.",
                "My wine was poisoned soon after."
            },
            new string[]
            {
                "Information felt like leverage.",
                "I pushed people too far.",
                "Knowing too much made me dangerous."
            }
        });

        CreateNPC("Senenmut", new string[][]
        {
            new string[]
            {
                "I am Senenmut.",
                "I managed the irrigation canals.",
                "A broken dam could starve a village."
            },
            new string[]
            {
                "Floods forced me to think fast.",
                "I once saved a harvest at night.",
                "A mud wall stopped the water."
            },
            new string[]
            {
                "A heavy sluice gate needed securing.",
                "My workers offered to help.",
                "I refused.",
                "The current pulled me under."
            },
            new string[]
            {
                "I struggled to trust others.",
                "I wanted control over every detail.",
                "Doing everything myself killed me."
            }
        });

        CreateNPC("Ahmose", new string[][]
        {
            new string[]
            {
                "Call me Ahmose.",
                "I carved royal tombs.",
                "I lived in the workers' village."
            },
            new string[]
            {
                "I knew where the false doors were.",
                "I knew the hidden paths.",
                "I stole lapis amulets."
            },
            new string[]
            {
                "Stealing did not expose me.",
                "Bragging did.",
                "A friend betrayed me for a reward.",
                "The guards impaled me."
            },
            new string[]
            {
                "Loyalty mattered little to me.",
                "I trusted the wrong people.",
                "Dishonesty came naturally."
            }
        });

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("NPC Data successfully generated in Assets/NPC!");
    }

    private static void CreateNPC(string npcName, string[][] dialogues)
    {
        NPCData npc = ScriptableObject.CreateInstance<NPCData>();
        npc.npcName = npcName;
        npc.dialogueBubbles = new DialogueBubble[dialogues.Length];

        for (int i = 0; i < dialogues.Length; i++)
        {
            npc.dialogueBubbles[i] = new DialogueBubble
            {
                snippets = dialogues[i]
            };
        }

        string path = $"Assets/NPC/{npcName}.asset";

        if (!AssetDatabase.IsValidFolder("Assets/NPC"))
        {
            AssetDatabase.CreateFolder("Assets", "NPC");
        }

        NPCData existing = AssetDatabase.LoadAssetAtPath<NPCData>(path);

        if (existing != null)
        {
            EditorUtility.CopySerialized(npc, existing);
            EditorUtility.SetDirty(existing);
        }
        else
        {
            AssetDatabase.CreateAsset(npc, path);
        }
    }
}
#endif