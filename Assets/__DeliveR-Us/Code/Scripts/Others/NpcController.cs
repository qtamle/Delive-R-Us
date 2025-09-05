using System.Collections.Generic;
using UnityEngine;

public class NpcController : MonoBehaviour
{
    [SerializeField] private GameObject[] _npcs;

    List<int> _doneIndexes = new List<int>();

    private void Start()
    {
        int amtToDeactive = Random.Range(0, _npcs.Length);
        int randomIndex = Random.Range(0, _npcs.Length);

        for (int i = 0; i < amtToDeactive; i++)
        {
            do
            {
                randomIndex = Random.Range(0, _npcs.Length);
            }
            while (_doneIndexes.Contains(randomIndex));

            _doneIndexes.Add(randomIndex);

            Destroy(_npcs[randomIndex].gameObject);
        } 
    }
}
