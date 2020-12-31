using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeTrackerSprites : MonoBehaviour
{
    public SpriteRenderer[] sprites;
    private GameObject[] shapes;
    public int currentShape = 0;

    private void OnEnable()
    {
        // Set shapes array to be sprites' parent GameObjects so it doesn't need to be set separately
        shapes = new GameObject[sprites.Length];
        for (int i = 0; i < shapes.Length; i++)
        {
            shapes[i] = sprites[i].transform.parent.gameObject;
        }
    }

    public void SwitchShape()
    {

    }
}
