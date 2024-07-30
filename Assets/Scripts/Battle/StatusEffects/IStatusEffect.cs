using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStatusEffect
{

    public string Name { get; }
    public string Description { get; }
    public Sprite Icon { get; }

    /// <summary>
    /// Given a CharacterHandler, inflicts effects onto it (based on the
    /// nature of the status effect) after immediately applied.
    /// </summary>
    public void ApplyEffect(CharacterHandler handler);

    /// <summary>
    /// Given a CharacterHandler, inflicts effects onto it (based on the
    /// nature of the status effect) over one turn.
    /// </summary>
    /// <returns>True if the effect should be removed, or else False.</returns>
    public bool UpdateEffect(CharacterHandler handler);

}
