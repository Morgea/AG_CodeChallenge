using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ColorPatterns
{
    public string gradientName;
    public Color Sky;
    public Color Horizon;
    public Color Ground;
    public float lastsFor = 5.0f;
}

public class BackgroundGradients : MonoBehaviour
{
    public ColorPatterns[] colorSets;
    public float changeTime = 15.0f;
    private float nextChange;
    public GameObject skyRend;
    public GameObject groundRend;
    private int onColorSet = 0;
    private Material skyMat;
    private Material groundMat;

    private void Start()
    {
        skyMat = skyMat = skyRend.GetComponent<UnityEngine.UI.Image>().material;
        groundMat = groundRend.GetComponent<UnityEngine.UI.Image>().material;

        nextChange = Time.time + colorSets[onColorSet].lastsFor;

        skyMat.SetColor("_Color2", colorSets[onColorSet].Sky);
        skyMat.SetColor("_Color", colorSets[onColorSet].Horizon);
        groundMat.SetColor("_Color2", colorSets[onColorSet].Horizon);
        groundMat.SetColor("_Color", colorSets[onColorSet].Ground);
    }

    private void Update()
    {
        if (Time.time > nextChange) DoChange();
    }

    private void DoChange()
    {
        onColorSet += 1;
        if (onColorSet >= colorSets.Length) onColorSet = 0;

        nextChange = Time.time + 1000000.0f;
        StopAllCoroutines();
        StartCoroutine(ChangeBackground());
    }

    IEnumerator ChangeBackground()
    {
        Color startSkyCol = skyMat.GetColor("_Color2");
        Color startHorCol = skyMat.GetColor("_Color");
        Color startGroCol = groundMat.GetColor("_Color");
        Color endSkyCol = colorSets[onColorSet].Sky;
        Color endHorCol = colorSets[onColorSet].Horizon;
        Color endGroCol = colorSets[onColorSet].Ground;

        float t = 0;

        while (t < 1)
        {
            t += Mathf.Clamp(Time.deltaTime / changeTime, 0, 1);
            float tEased = Mathf.SmoothStep(0, 1, t);

            Color newSkyCol = Color.Lerp(startSkyCol, endSkyCol, tEased);
            Color newHorCol = Color.Lerp(startHorCol, endHorCol, tEased);
            Color newGroCol = Color.Lerp(startGroCol, endGroCol, tEased);

            skyMat.SetColor("_Color2", newSkyCol);
            skyMat.SetColor("_Color", newHorCol);
            groundMat.SetColor("_Color2", newHorCol);
            groundMat.SetColor("_Color", newGroCol);

            yield return null;
        }
        nextChange = Time.time + colorSets[onColorSet].lastsFor;
    }

}
