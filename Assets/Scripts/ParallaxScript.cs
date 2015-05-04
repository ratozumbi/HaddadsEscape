using UnityEngine;
using System.Collections;

public class ParallaxScript : MonoBehaviour {

    public Renderer background;
    public Renderer foreground;

    public float backgroundSpeed = 0.02f;
    public float foregroundSpeed = 0.06f;

	void Update () {

        float backgroundOffset = Time.timeSinceLevelLoad * backgroundSpeed;
        float foregroundOffset = Time.timeSinceLevelLoad * foregroundSpeed;

        background.material.mainTextureOffset = new Vector2(backgroundOffset, 0);
        foreground.material.mainTextureOffset = new Vector2(foregroundOffset, 0);
	}
}
