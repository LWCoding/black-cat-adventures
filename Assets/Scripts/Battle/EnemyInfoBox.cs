using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.PlayerSettings;

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
    [SerializeField] private TextMeshPro _enemyDescText;

    private readonly List<GameObject> _instantiatedAttacks = new();

    /// <summary>
    /// Given an Enemy's data, sets the info box's description to
    /// match the enemy and creates an info prefab for each attack.
    /// </summary>
    public void SetInfo(EnemyData enemyData)
    {
        for (int i = _instantiatedAttacks.Count - 1; i >= 0; i--)
        {
            Destroy(_instantiatedAttacks[i]);
        }
        _instantiatedAttacks.Clear();
        // Compile all attacks to load UI
        List<EnemyInfo> infos = new();
        foreach (EnemyAttack attack in enemyData.Attacks)
        {
            infos.Add(new EnemyInfo()
            {
                Name = attack.AttackName,
                Description = attack.AttackDescription,
                InfoSprite = attack.IconSprite
            });
        }
        // Instantiate an instance for each piece of info
        for (int i = 0; i < infos.Count; i++)
        {
            GameObject infoObject = Instantiate(_infoPrefab, _attackContainer, false);
            infoObject.transform.position -= new Vector3(0, 1f * i);
            infoObject.GetComponent<EnemyInfoHandler>().Initialize(infos[i]);
            _instantiatedAttacks.Add(infoObject);
        }
        // Set description text
        _enemyDescText.text = enemyData.EnemyDescription;
    }

}
