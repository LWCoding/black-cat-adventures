using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct EnemyInfo
{
    public Sprite InfoSprite;
    public string Name;
    public string Description;
}

public class EnemyInfoBox : MonoBehaviour
{

    [Header("Prefab Assignments")]
    [SerializeField] private GameObject _infoPrefab;
    [Header("Object Assignments")]
    [SerializeField] private Transform _attackContainer;

    private readonly List<GameObject> _instantiatedAttacks = new();

    /// <summary>
    /// Given an EnemyInfo struct (defined above), refreshes the info
    /// box and creates an object for each piece of information.
    /// </summary>
    public void SetInfo(List<EnemyInfo> infos)
    {
        _instantiatedAttacks.Clear();
        // Instantiate an instance for each piece of info
        for (int i = 0; i < infos.Count; i++)
        {
            GameObject infoObject = Instantiate(_infoPrefab, _attackContainer, false);
            infoObject.transform.position -= new Vector3(0, 1f * i);
            infoObject.GetComponent<EnemyInfoHandler>().Initialize(infos[i]);
            _instantiatedAttacks.Add(infoObject);
        }
    }

}
