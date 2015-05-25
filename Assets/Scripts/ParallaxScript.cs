using UnityEngine;
using System.Collections;

public class ParallaxScript : MonoBehaviour {

    public Renderer background;
    public Renderer foreground;

    private float backgroundSpeed = 0.02f;
    private float foregroundSpeed = 0.06f;

    private bool move = false;

	void Update () {

        if (move)
        {
            float backgroundOffset = Time.timeSinceLevelLoad * backgroundSpeed;
            float foregroundOffset = Time.timeSinceLevelLoad * foregroundSpeed;

            background.material.mainTextureOffset = new Vector2(backgroundOffset, 0);
            foreground.material.mainTextureOffset = new Vector2(foregroundOffset, 0);
        }
	}

    void StartMove()
    {
        this.move = true;
    }

    void StopMove()
    {
        this.move = false;
    }
}
