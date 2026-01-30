using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water: MonoBehaviour {

    public Vector2 speed;
    public Vector2 secondarySpeed;

    private float offsetX;
    private float offsetY;

    private float secondaryOffsetX;
    private float secondaryOffsetY;
    private new Renderer renderer;

    private void Start()
    {
        renderer = GetComponent<Renderer>();
    }

    void Update () {
        offsetX += Time.deltaTime * speed.x;
        offsetY += Time.deltaTime * speed.y;

        secondaryOffsetX += Time.deltaTime * secondarySpeed.x;
        secondaryOffsetY += Time.deltaTime * secondarySpeed.y;
        renderer.material.mainTextureOffset = new Vector2(offsetX, offsetY);
        renderer.material.SetTextureOffset("_DetailAlbedoMap", new Vector2(secondaryOffsetX, secondaryOffsetY));
	}
}
