using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Avatars/Avatar Data")]

public class AvatarData : ScriptableObject
{
    [SerializeField] private ClientData[] _avatars;

    private List<int> _doneIndexes = new List<int>();

    public ClientData GetRandomData
    {
        get
        {
            int randomIndex = Random.Range(0, _avatars.Length);


            do
            {
                randomIndex = Random.Range(0, _avatars.Length);
            }
            while (_doneIndexes.Contains(randomIndex));

            if(_doneIndexes.Count > 3)
                _doneIndexes.RemoveAt(0);

            _doneIndexes.Add(randomIndex);

            return _avatars[randomIndex];
        }
    }
}


[System.Serializable]
public class ClientData
{
    public Sprite icon;
    public GameObject prefab;
}
