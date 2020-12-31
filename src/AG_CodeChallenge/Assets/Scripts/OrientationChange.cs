using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientationChange : MonoBehaviour
{
    public Camera mainCam;
    public Vector3 portrait_Pos = new Vector3(0, 3, -10);
    public float portrait_Size = 7;
    public Vector3 landscape_Pos = new Vector3(0, 0, -10);
    public float landscape_Size = 5;
    private bool switching_Portrait = false;
    private bool switching_Landscape = false;

    void Update()
    {
        // Portrait orientation
        if ((Screen.height > Screen.width) && (!switching_Portrait))
        {
            switching_Portrait = true;
            switching_Landscape = false;
            StopAllCoroutines();
            StartCoroutine(SwitchOrientation(portrait_Size, portrait_Pos));
        }
        // Landscape orientation
        else if ((Screen.width > Screen.height) && (!switching_Landscape))
        {
            switching_Landscape = true;
            switching_Portrait = false;
            StopAllCoroutines();
            StartCoroutine(SwitchOrientation(landscape_Size, landscape_Pos));
        }
    }

    IEnumerator SwitchOrientation(float target_Size, Vector3 target_Pos)
    {
        float t = 0;
        Vector3 start_Pos = mainCam.transform.localPosition;
        float start_Size = mainCam.orthographicSize;

        while (t < 1)
        {
            t += Mathf.Clamp (Time.deltaTime / 2.0f, 0, 1);
            float tEased = Mathf.SmoothStep(0, 1, t);

            mainCam.transform.localPosition = Vector3.Lerp(start_Pos, target_Pos, tEased);
            mainCam.orthographicSize = Mathf.Lerp(start_Size, target_Size, tEased);

            yield return null;
        }
    }

}
