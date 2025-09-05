using System.Collections.Generic;
using UnityEngine;

public class NPCDistanceCuller : MonoBehaviour
{
    public Transform player;
    public float enableDistance = 100f;
    public float disableDistance = 120f;
    public float checkInterval = 0.05f;

    private List<Passersby> npcs;
    private int index;

    void Start()
    {
        if (player == null)
            player = Camera.main.transform;

        npcs = new List<Passersby>(Object.FindObjectsByType<Passersby>(FindObjectsSortMode.None));

        InvokeRepeating(nameof(UpdateOneNPC), 0f, checkInterval);
    }

    void UpdateOneNPC()
    {
        if (npcs == null || npcs.Count == 0) return;

        // Loop until a valid NPC is found or we've tried all
        for (int tries = 0; tries < npcs.Count; tries++)
        {
            index %= npcs.Count;

            var npc = npcs[index];

            // Remove nulls dynamically to keep the list clean
            if (npc == null || npc.gameObject == null)
            {
                npcs.RemoveAt(index);
                continue;
            }

            float dist = Vector3.Distance(player.position, npc.transform.position);

            bool isActive = npc.gameObject.activeSelf;

            if (dist > disableDistance && isActive)
                npc.gameObject.SetActive(false);
            else if (dist < enableDistance && !isActive)
                npc.gameObject.SetActive(true);

            index = (index + 1) % npcs.Count;
            break;
        }
    }
}
