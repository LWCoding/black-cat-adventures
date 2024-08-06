using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelHandler : MonoBehaviour
{

    [Header("Object Assignments")]
    [SerializeField] private TextMeshPro _levelText;
    [Header("Level Properties")]
    public int LevelNumber = 0;
    public string LevelName;

    private void Awake()
    {
        _levelText.text = "Level " + LevelNumber.ToString();
    }

    public void OnMouseDown()
    {
        SceneManager.LoadScene(LevelName);
    }

}
