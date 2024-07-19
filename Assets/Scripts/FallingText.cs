using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class FallingText : MonoBehaviour
{

    [Header("Object Assignments")]
    [SerializeField] private TextMeshPro _text;

    public void Initialize(string text, Color textColor)
    {
        _text.text = text;
        _text.color = new Color(textColor.r, textColor.g, textColor.b, 1);
        StartCoroutine(MoveDownCoroutine());
        StartCoroutine(FadeTextCoroutine());
        Destroy(this, 1.5f);  // Destroy after 1.5 second
    }

    /// <summary>
    /// Make this sprite move down after bouncing up,
    /// with a randomized x-velocity over 1.5 second.
    /// </summary>
    private IEnumerator MoveDownCoroutine()
    {
        float currTime = 0f;
        float timeToWait = 1.5f;
        float xVel = Random.Range(-1.2f, 1.2f);
        float yVelStageOne = 5f;
        float yVelStageTwo = -2.5f;
        while (currTime < timeToWait)
        {
            currTime += Time.deltaTime;
            if (currTime < 0.4f)
            {
                transform.Translate(xVel * Time.deltaTime, yVelStageOne * (2 * currTime / timeToWait) * Time.deltaTime, 0);
            }
            else
            {
                transform.Translate(xVel * 0.5f * Time.deltaTime, yVelStageTwo * (currTime / timeToWait) * Time.deltaTime, 0);
            }
            yield return null;
        }
    }

    /// <summary>
    /// Make this sprite fade away over 1.5 second.
    /// </summary>
    private IEnumerator FadeTextCoroutine()
    {
        float currTime = 0f;
        float timeToWait = 1.5f;
        Color startingColor = _text.color;
        Color endColor = new(startingColor.r, startingColor.g, startingColor.b, 0);
        while (currTime < timeToWait)
        {
            currTime += Time.deltaTime;
            _text.color = Color.Lerp(startingColor, endColor, currTime / timeToWait);
            yield return null;
        }
        _text.color = endColor;
    }

}
