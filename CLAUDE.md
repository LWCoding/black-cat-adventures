# Black Cat Adventures

## Naming conventions

### `*Manager` classes must be singletons

Any MonoBehaviour named `<Something>Manager` must derive from `Singleton<T>` ([Assets/Scripts/Utility/Singleton.cs](Assets/Scripts/Utility/Singleton.cs)) and expose itself via the inherited static `Instance` property, instead of hand-rolling singleton boilerplate.

```csharp
public class FooManager : Singleton<FooManager>
{
    protected override void Awake()
    {
        base.Awake();
        // ... rest of your setup
    }
}
```

Only override `Awake()` if you have extra setup to do; otherwise leave it out entirely and the base class handles it.

**Exception:** plain static classes with no MonoBehaviour/scene presence (e.g. `GameManager`, `SaveManager`) are not subject to this rule — they hold no per-instance state and aren't attached to a GameObject, so the singleton pattern doesn't apply to them.

## Animations

### Prefer DOTween over manual coroutines

DOTween (`Assets/Plugins/Demigiant/DOTween/`) is installed project-wide. Use it for any positional, rotational, scale, color, or fade animation instead of hand-rolling `IEnumerator` / `Vector3.Lerp` coroutines.

```csharp
// Preferred
transform.DOMove(target, 0.25f).SetEase(Ease.InBack).OnComplete(() => Destroy(gameObject));

// Avoid
StartCoroutine(MoveCoroutine(target, 0.25f));
```

Use `SetDelay`, `SetEase`, `OnComplete`, and `DOSequence`/`Sequence.Join`/`Sequence.Append` for sequencing. Fall back to a coroutine only when the animation logic genuinely cannot be expressed as a tween (e.g. physics-driven or data-dependent per-frame decisions).
