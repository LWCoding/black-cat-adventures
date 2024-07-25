using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyInfoHandler : MonoBehaviour
{

    [Header("Object Assignments")]
    [SerializeField] private SpriteRenderer _iconRenderer;
    [SerializeField] private TextMeshPro _infoText;

    public void Initialize(EnemyInfo info)
    {
        _iconRenderer.sprite = info.InfoSprite;
        _infoText.text = "<b>" + info.Name + "</b>:\n" + info.Description;
    }

}
