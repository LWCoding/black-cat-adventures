using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Poison", menuName = "Status Effects/Poison")]
public class Poison : ScriptableObject, IStatusEffect
{

    [SerializeField] private string _name;
    [SerializeField] private string _description;
    [SerializeField] private Sprite _icon;

    public string Name => _name;
    public string Description => _description;
    public Sprite Icon => _icon;

    public void ApplyEffect(CharacterHandler handler)
    {
        throw new System.NotImplementedException();
    }

    public bool UpdateEffect(CharacterHandler handler)
    {
        throw new System.NotImplementedException();
    }

}
