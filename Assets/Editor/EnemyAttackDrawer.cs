using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom property drawer for EnemyAttack.
/// - Draws a titled box around each attack for clear list separation.
/// - Projectile Sprite and Projectile Speed appear directly under AnimType,
///   visible only when AnimType == PROJECTILE.
/// </summary>
[CustomPropertyDrawer(typeof(EnemyAttack))]
public class EnemyAttackDrawer : PropertyDrawer
{

    private const int PROJECTILE_ANIM_INDEX = 3; // AttackAnimation.PROJECTILE
    private const float PADDING = 4f;
    private const float HEADER_HEIGHT = 20f;

    // Explicit draw order. Projectile fields are injected after AnimType in OnGUI/GetPropertyHeight.
    private static readonly string[] FieldOrder =
    {
        "IconSprite",
        "AttackName",
        "_attackDescription",
        "Damage",
        "AnimType",
        // ProjectileSprite + ProjectileSpeed inserted here when PROJECTILE
        "InflictedStatuses",
        "BoardEffects",
        "AvoidIfSelfHasStatus",
        "AvoidIfTargetHasStatus",
    };

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Draw background box
        GUI.Box(position, GUIContent.none, EditorStyles.helpBox);

        float x = position.x + PADDING;
        float width = position.width - PADDING * 2f;
        float y = position.y + PADDING;
        float spacing = EditorGUIUtility.standardVerticalSpacing;

        // Header label — show attack name if filled in, fall back to element index from label
        SerializedProperty nameProp = property.FindPropertyRelative("AttackName");
        string headerText = !string.IsNullOrWhiteSpace(nameProp.stringValue)
            ? nameProp.stringValue
            : label.text;
        EditorGUI.LabelField(new Rect(x, y, width, HEADER_HEIGHT), headerText, EditorStyles.boldLabel);
        y += HEADER_HEIGHT + spacing;

        // Separator line
        EditorGUI.DrawRect(new Rect(x, y, width, 1f), new Color(0.4f, 0.4f, 0.4f, 0.6f));
        y += 1f + spacing;

        SerializedProperty animTypeProp = property.FindPropertyRelative("AnimType");
        bool isProjectile = animTypeProp.enumValueIndex == PROJECTILE_ANIM_INDEX;

        foreach (string fieldName in FieldOrder)
        {
            SerializedProperty prop = property.FindPropertyRelative(fieldName);
            if (prop == null) { continue; }

            float fieldH = EditorGUI.GetPropertyHeight(prop, true);
            EditorGUI.PropertyField(new Rect(x, y, width, fieldH), prop, true);
            y += fieldH + spacing;

            // Inject projectile fields directly after AnimType
            if (fieldName == "AnimType" && isProjectile)
            {
                DrawProjectileField(property, "ProjectileSprite", ref y, x, width, spacing);
                DrawProjectileField(property, "ProjectileSpeed", ref y, x, width, spacing);
            }
        }

        EditorGUI.EndProperty();
    }

    private static void DrawProjectileField(SerializedProperty parent, string name, ref float y, float x, float width, float spacing)
    {
        SerializedProperty prop = parent.FindPropertyRelative(name);
        if (prop == null) { return; }
        float h = EditorGUI.GetPropertyHeight(prop, true);
        EditorGUI.PropertyField(new Rect(x, y, width, h), prop, true);
        y += h + spacing;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float spacing = EditorGUIUtility.standardVerticalSpacing;
        SerializedProperty animTypeProp = property.FindPropertyRelative("AnimType");
        bool isProjectile = animTypeProp.enumValueIndex == PROJECTILE_ANIM_INDEX;

        float total = PADDING                   // top padding
                    + HEADER_HEIGHT + spacing   // header
                    + 1f + spacing;             // separator line

        foreach (string fieldName in FieldOrder)
        {
            SerializedProperty prop = property.FindPropertyRelative(fieldName);
            if (prop == null) { continue; }
            total += EditorGUI.GetPropertyHeight(prop, true) + spacing;

            if (fieldName == "AnimType" && isProjectile)
            {
                total += FieldHeight(property, "ProjectileSprite", spacing);
                total += FieldHeight(property, "ProjectileSpeed", spacing);
            }
        }

        total += PADDING; // bottom padding
        return total;
    }

    private static float FieldHeight(SerializedProperty parent, string name, float spacing)
    {
        SerializedProperty prop = parent.FindPropertyRelative(name);
        return prop != null ? EditorGUI.GetPropertyHeight(prop, true) + spacing : 0f;
    }

}
