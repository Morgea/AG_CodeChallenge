using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorRandomizerSprites : MonoBehaviour
{
    public float changeTime = 2.0f;
    public Color[] colorList;
    private int currentColor = 0;
    public bool randomColors = true;
    public ShapeTrackerSprites shapeScript;
    private Coroutine colorChCo;

    private void Start()
    {
        // If using color list, set all shapes to first color on list
        if (!randomColors) SetColor(colorList[0]);
    }

    public void ChangeColor()
    {
        Color newColor;

        // If creating a completely random color
        if (randomColors)
        {
            // Randomize new color
            float newRed = Random.Range(0.0f, 1.0f);
            float newGreen = Random.Range(0.0f, 1.0f);
            float newBlue = Random.Range(0.0f, 1.0f);

            newColor = new Color(newRed, newGreen, newBlue, 1);
        }
        // If using color list
        else
        {
            currentColor++;
            if (currentColor >= colorList.Length) currentColor = 0;
            newColor = colorList[currentColor];
        }

        if (colorChCo != null) StopCoroutine(colorChCo);
        colorChCo = StartCoroutine(RunColorChange(newColor));
    }

    IEnumerator RunColorChange(Color newColor)
    {
        // Start with current shape color
        Color prevColor = shapeScript.sprites[0].color;

        float t = 0;

        while (t < 1)
        {
            t += Mathf.Clamp (Time.deltaTime / changeTime, 0, 1);

            float currRed = Mathf.Lerp(prevColor.r, newColor.r, t);
            float currGreen = Mathf.Lerp(prevColor.g, newColor.g, t);
            float currBlue = Mathf.Lerp(prevColor.b, newColor.b, t);

            Color currColor = new Color(currRed, currGreen, currBlue, 1);

            SetColor(currColor);

            yield return null;
        }
    }

    private void SetColor(Color newColor)
    {
        for (int i = 0; i < shapeScript.sprites.Length; i++)
        {
            shapeScript.sprites[i].color = newColor;
        }
    }

}
