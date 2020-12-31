using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowSpin : MonoBehaviour
{
    public Vector2 spinSpeedRange;
    public Vector2 changeSpeedTimeRange;
    public float speedChangeOver = 3.0f;
    private float spinSpeed;
    private float lastChangeTime;
    private float nextChangeTime;
    private bool spinningDir = false;

    private void Start()
    {
        if (Random.value < .5) spinningDir = !spinningDir;
        RandomizeSpin();
    }

    void Update()
    { 
        transform.Rotate(0, 0, spinSpeed * Time.deltaTime);
        if (Time.time > nextChangeTime) RandomizeSpin();
    }

    void RandomizeSpin()
    {
        nextChangeTime = Time.time + 100000.0f;

        float spinSpeedGoal = Random.Range(spinSpeedRange.x, spinSpeedRange.y);

        // Alternate spinning counter or clockwise
        if (spinningDir) spinSpeedGoal *= -1;
        spinningDir = !spinningDir;

        StopAllCoroutines();
        StartCoroutine(ChangeSpeed(spinSpeedGoal));
    }

    IEnumerator ChangeSpeed(float spinSpeedGoal)
    {
        float t = 0;
        float oldSpinSpeed = spinSpeed;

        while (t < 1)
        {
            t += Mathf.Clamp(Time.deltaTime / speedChangeOver, 0, 1);
            float tEased = Mathf.SmoothStep(0, 1, t);
            spinSpeed = Mathf.Lerp(oldSpinSpeed, spinSpeedGoal, tEased);
            yield return null;
        }
        lastChangeTime = Time.time;
        float nextChangeRandom = Random.Range(spinSpeedRange.x, spinSpeedRange.y);
        nextChangeTime = nextChangeRandom + lastChangeTime;
    }

}
