using UnityEngine;
using System.Collections;

public class Haddad2DController : MonoBehaviour {
   
    public bool dead;
    public float startLife;
    public float currentLife = 0f;
    public float movementSpeed = 10.0f;

	public AudioSource somAgua;
	public AudioSource somBuraco;

    public Texture2D lifeBarEmpty;
    public Texture2D lifeBarFull;

    private Vector2 lifeBarSize;
    private Vector2 lifeBarPosition;
    private float lifeBarCurrentPosition = 0;

    void Start() {
        this.startLife = 100f;
        this.currentLife = startLife;
        this.lifeBarSize = new Vector2(300, 30);
        this.lifeBarPosition = new Vector2(25, 25);
    }

    void OnGUI() 
    {
        GUI.BeginGroup(new Rect(lifeBarPosition.x, lifeBarPosition.y, lifeBarSize.x, lifeBarSize.y));
        GUI.Box(new Rect(0, 0, lifeBarSize.x, lifeBarSize.y), lifeBarEmpty, new GUIStyle() { stretchHeight = true, stretchWidth = true });

        GUI.BeginGroup(new Rect(0, 0, lifeBarSize.x * lifeBarCurrentPosition, lifeBarSize.y));
        GUI.Box(new Rect(0, 0, lifeBarSize.x, lifeBarSize.y), lifeBarFull, new GUIStyle() { stretchHeight = true, stretchWidth = true });
        GUI.EndGroup();

        GUI.EndGroup();
        
        this.GameOver();
    }

	void FixedUpdate () {

        this.UpdateLifeBar();

        if (!dead)
        {
            float direction = Input.GetAxisRaw("Vertical");
            Rigidbody2D rigidBody2D = this.transform.GetComponent<Rigidbody2D>();

            Vector2 newVelocity = rigidBody2D.velocity;
            newVelocity.x = this.movementSpeed;
            rigidBody2D.velocity = newVelocity;

            if (direction > 0)
            {
                this.transform.position = new Vector3(this.transform.position.x, -1.75f);
            }
            else if (direction < 0)
            {
                this.transform.position = new Vector3(this.transform.position.x, -2.88f);
            }

			if (Input.GetAxisRaw("Cancel") >0) Application.LoadLevel ("menu");
        }
	}

    void OnTriggerEnter2D(Collider2D collider)
    {
        float value = 0;

        switch (collider.tag)
        {
            case "Obstaculo":
                value = -10;
			somBuraco.Play();
                break;
            case "Water":
                value = 10;
                Destroy(collider.gameObject);
			somAgua.Play();
                break;
            default:
                value = 0;
                break;
        }

        Debug.Log(string.Format("Antes - Current: {0}; Value: {1}", this.currentLife, value));

        if (value > 0)
        {
            if (this.currentLife + value > this.startLife)
                this.currentLife = this.startLife;
            else
                this.currentLife += value;
        }
        else
        {
            if(this.currentLife > 0)
                this.currentLife += value;
        }

        Debug.Log(string.Format("Depois - Current: {0}; Value: {1}", this.currentLife, value));
    }

    void UpdateLifeBar()
    {
        this.currentLife = currentLife - Time.deltaTime * 2;
        this.lifeBarCurrentPosition = this.currentLife / this.startLife;

        if (this.lifeBarCurrentPosition <= 0f)
            this.dead = true;
    }

    void GameOver() {
        
        if (this.dead)
        {
			GameObject.Find ("musica").GetComponent<AudioSource>().Stop();
            Rect buttonRect = new Rect(Screen.width * 0.20f, Screen.height * 0.45f, Screen.width * 0.60f, Screen.height * 0.1f);

            if (GUI.Button(buttonRect, "Game Over! Clique aqui para iniciar novamente!"))
                Application.LoadLevel(Application.loadedLevelName);
        }
    }
}
