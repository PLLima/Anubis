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
                "I was a sailor from Memphis.",
                "Trade was how I made my living!",
                "I worked with linen and copper everyday."
            },
            new string[]
            {
                "Words were my greatest weapon.",
                "I always knew what people wanted to hear.",
                "I consider myself a great salesman.",
                "I sometimes sold phony amulets."
            },
            new string[]
            {s
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
                "I hauled stones for the temples.",
                "Physical labor was part of my everyday!."
            },
            new string[]
            {
                "Strength was never a problem for me.",
                "I could not stand delays.",
                "I was a master at construction!"
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
                "Still, I loved labouring!",
                "I was foolishly brave and reckless!"
            }
        });

        CreateNPC("Hori", new string[][]
        {
            new string[]
            {
                "I managed the state granaries.",
                "Every sack passed through my records."
            },
            new string[]
            {
                "I kept hidden copies of the ledgers.",
                "All of my days were spent writing and transcribing.",
                "I became a master at remembering and recording."
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
                "Well, I led trade caravans south.",
                "We brought back gold and myrrh. I handled luxury goods, you see."
            },
            new string[]
            {
                "Why, I learned the tongues of Nubian kings.",
                "Language gave me an advantage and I was a good talker.",
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
                "In life, I prepared the dead for eternity...",
                "I handled corpses all day..."
            },
            new string[]
            {
                "I pulled brains out through the nose...",
                "I became completely used to dead bodies...",
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
                "I loved playing the sistrum at court!",
                "Most people barely noticed musicians, but I was different!"
            },
            new string[]
            {
                "People felt totally at ease with me!.",
                "Viziers whispered secrets nearby.",
                "I always remembered the nastiest and juiciest they said."
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
                "I managed the irrigation canals.",
                "A broken dam could starve a village, so I had a lot of responsibility."
            },
            new string[]
            {
                "Floods forced me to think fast.",
                "I wasn't scared of any Nile creatures.",
                "I was a skilled and efficient worker."
            },
            new string[]
            {
                "A heavy sluice gate needed securing.",
                "My workers offered to help.",
                "I refused, since I believed myself strong.",
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
                "I carved royal tombs.",
                "I lived in the workers' village, hehe. But I wanted more."
            },
            new string[]
            {
                "I knew where the false doors and hidden paths were.",
                "I had sticky fingers, hehe.",
                "I had eyes for gold and jewelry."
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
                "Loyalty mattered little to me, hehe.",
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
            Sprite savedSprite = existing.npcSprite;
            VoiceProfile savedVoice = existing.voiceProfile;
            EditorUtility.CopySerialized(npc, existing);
            existing.npcSprite = savedSprite;
            if (savedVoice != null) existing.voiceProfile = savedVoice;
            EditorUtility.SetDirty(existing);
        }
        else
        {
            AssetDatabase.CreateAsset(npc, path);
        }
    }
}
#endif