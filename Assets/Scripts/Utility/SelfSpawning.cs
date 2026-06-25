using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Marks a <see cref="MonoBehaviour"/> singleton as self-spawning: an instance
/// will be created automatically at the specified load phase if none is already
/// present in the scene.
///
/// Two spawning strategies are supported:
/// <list type="bullet">
///   <item>
///     <term>Bare GameObject (default)</term>
///     <description>
///       A new <see cref="GameObject"/> named after the type is created and the
///       component is added to it. Optionally call
///       <see cref="DontDestroyOnLoad"/> on it by setting
///       <see cref="SelfSpawningAttribute.KeepAcrossScenes"/> to <c>true</c>
///       (not needed when the class already derives from
///       <see cref="PersistentSingleton{T}"/>).
///     </description>
///   </item>
///   <item>
///     <term>Resources prefab</term>
///     <description>
///       When <see cref="SelfSpawningAttribute.ResourcePath"/> is set the
///       prefab is loaded from <c>Resources</c> and instantiated. This is the
///       right strategy when the component needs serialized references
///       (e.g. child canvas objects).
///     </description>
///   </item>
/// </list>
///
/// Usage example:
/// <code>
/// [SelfSpawning(RuntimeInitializeLoadType.BeforeSceneLoad)]
/// public class AudioManager : PersistentSingleton&lt;AudioManager&gt; { ... }
///
/// [SelfSpawning(RuntimeInitializeLoadType.AfterSceneLoad, ResourcePath = "PauseMenu")]
/// public class PauseMenuManager : PersistentSingleton&lt;PauseMenuManager&gt; { ... }
/// </code>
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class SelfSpawningAttribute : Attribute
{
    /// <summary>When to spawn the instance.</summary>
    public RuntimeInitializeLoadType LoadType { get; }

    /// <summary>
    /// Path inside a <c>Resources</c> folder to a prefab that will be
    /// instantiated. When <c>null</c> (the default) a bare
    /// <see cref="GameObject"/> is created instead.
    /// </summary>
    public string ResourcePath { get; set; }

    /// <summary>
    /// When <c>true</c> the spawned <see cref="GameObject"/> is marked
    /// <see cref="UnityEngine.Object.DontDestroyOnLoad"/>. Defaults to
    /// <c>false</c>. Classes that derive from <see cref="PersistentSingleton{T}"/>
    /// do not need this — the base class already handles persistence.
    /// </summary>
    public bool KeepAcrossScenes { get; set; }

    public SelfSpawningAttribute(
        RuntimeInitializeLoadType loadType = RuntimeInitializeLoadType.BeforeSceneLoad)
    {
        LoadType = loadType;
    }
}

/// <summary>
/// Reflection-driven bootstrap that discovers every <see cref="MonoBehaviour"/>
/// decorated with <see cref="SelfSpawningAttribute"/> and spawns an instance at
/// the requested load phase if none is already in the scene.
///
/// <para>
/// This class exists because <c>[RuntimeInitializeOnLoadMethod]</c> cannot be
/// placed on generic types, so the logic cannot live directly on
/// <see cref="Singleton{T}"/> or <see cref="PersistentSingleton{T}"/>.
/// </para>
/// </summary>
internal static class SelfSpawningBootstrap
{
    private static List<(Type type, SelfSpawningAttribute attr)> _registry;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void SpawnBeforeSceneLoad() =>
        SpawnAll(RuntimeInitializeLoadType.BeforeSceneLoad);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void SpawnAfterSceneLoad() =>
        SpawnAll(RuntimeInitializeLoadType.AfterSceneLoad);

    private static void SpawnAll(RuntimeInitializeLoadType loadType)
    {
        foreach (var (type, attr) in GetRegistry())
        {
            if (attr.LoadType != loadType) { continue; }
            if (UnityEngine.Object.FindAnyObjectByType(type) != null) { continue; }

            if (attr.ResourcePath != null)
            {
                SpawnFromResources(type, attr.ResourcePath, attr.KeepAcrossScenes);
            }
            else
            {
                SpawnBareGameObject(type, attr.KeepAcrossScenes);
            }
        }
    }

    private static void SpawnBareGameObject(Type type, bool keepAcrossScenes)
    {
        var go = new GameObject(type.Name);
        go.AddComponent(type);
        if (keepAcrossScenes) { UnityEngine.Object.DontDestroyOnLoad(go); }
    }

    private static void SpawnFromResources(Type type, string resourcePath, bool keepAcrossScenes)
    {
        var prefab = Resources.Load<GameObject>(resourcePath);
        if (prefab == null)
        {
            Debug.LogError(
                $"[SelfSpawning] Prefab not found at Resources path \"{resourcePath}\" " +
                $"for type {type.Name}. The instance was not created.");
            return;
        }
        var go = UnityEngine.Object.Instantiate(prefab);
        if (keepAcrossScenes) { UnityEngine.Object.DontDestroyOnLoad(go); }
    }

    private static List<(Type, SelfSpawningAttribute)> GetRegistry()
    {
        if (_registry != null) { return _registry; }

        _registry = new List<(Type, SelfSpawningAttribute)>();
        var monoBehaviourType = typeof(MonoBehaviour);

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types;
            }

            foreach (var type in types)
            {
                if (type == null) { continue; }
                if (!monoBehaviourType.IsAssignableFrom(type)) { continue; }
                if (type.IsAbstract) { continue; }

                var attr = type.GetCustomAttribute<SelfSpawningAttribute>(inherit: false);
                if (attr != null)
                {
                    _registry.Add((type, attr));
                }
            }
        }

        return _registry;
    }
}
