using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Single authority for a sprite transform's localScale. Other systems (status effects,
/// idle animations, etc.) register named Vector3 modifier factors; this component
/// composites them all via component-wise multiplication against a captured base scale
/// and writes the result to localScale immediately.
///
/// Callers drive animation themselves (e.g. via DOTween.To) and call SetModifier every
/// frame — this component never starts tweens of its own.
/// </summary>
public class SpriteScaleRoot : MonoBehaviour
{

    private Vector3 _baseScale;
    private readonly Dictionary<string, Vector3> _modifiers = new();

    private void Awake()
    {
        _baseScale = transform.localScale;
    }

    /// <summary>
    /// Registers a uniform scale modifier (x and y both set to <paramref name="uniformFactor"/>,
    /// z left at 1). Immediately recomputes and applies localScale.
    /// </summary>
    public void SetModifier(string key, float uniformFactor)
    {
        SetModifier(key, new Vector3(uniformFactor, uniformFactor, 1f));
    }

    /// <summary>
    /// Registers a per-axis scale modifier. Immediately recomputes and applies localScale.
    /// </summary>
    public void SetModifier(string key, Vector3 factor)
    {
        _modifiers[key] = factor;
        Apply();
    }

    /// <summary>
    /// Removes a modifier and immediately recomputes localScale.
    /// </summary>
    public void ClearModifier(string key)
    {
        if (_modifiers.Remove(key))
        {
            Apply();
        }
    }

    private void Apply()
    {
        Vector3 result = _baseScale;
        foreach (Vector3 mod in _modifiers.Values)
        {
            result = Vector3.Scale(result, mod);
        }
        transform.localScale = result;
    }

}
