using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorRandomizer : MonoBehaviour
{
    public float changeTime = 2.0f;
    public Color[] colorList;
    private int currentColor = 0;
    public bool randomColors = true;
    public ShapeTracker shapeScript;
    private Coroutine colorChCo;
    public GameObject burstPrefab;
    private Color shapeColor;
    public bool particleBurstOnClick = true;

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
        Color prevColor = shapeScript.shapes[0].GetComponent<Renderer>().material.color;

        float t = 0;

        while (t < 1)
        {
            t += Mathf.Clamp (Time.deltaTime / changeTime, 0, 1);

            Color currColor = Color.Lerp(prevColor, newColor, t);

            SetColor(currColor);

            shapeColor = currColor;
            yield return null;
        }
    }

    private void SetColor(Color newColor)
    {

        for (int i = 0; i < shapeScript.shapes.Length; i++)
        {
            shapeScript.shapes[i].GetComponent<Renderer>().material.color = newColor;
        }
    }

    public void ParticleBurst()
    {
        if (!particleBurstOnClick) return;
        GameObject burst = Instantiate(burstPrefab, this.transform);

        ParticleSystem.MainModule particleSettings = burst.GetComponent<ParticleSystem>().main;
        particleSettings.startColor = new ParticleSystem.MinMaxGradient(shapeColor);
    }

}
