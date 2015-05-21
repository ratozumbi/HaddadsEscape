using UnityEngine;
using System.Collections;

public class Haddad2DController : MonoBehaviour {
   
    public bool dead;
    public float startLife;
    public float ghostTime;
    public float currentLife = 0f;
    public float movementSpeed = 10.0f;

	public static float direction;

	public AudioSource somAgua;
	public AudioSource somBuraco;

    public Texture2D lifeBarEmpty;
    public Texture2D lifeBarFull;

    private float lifeBarCurrentPosition = 0;

	public static void ChangeDirection(){
		direction = direction >= 0 ? -1:1;
	}

    void Start() {
        this.startLife = 100f;
        this.currentLife = startLife;
    }

    void OnGUI() 
    {
        GUI.BeginGroup(new Rect(25, 25, 300, 30));
        GUI.Box(new Rect(0, 0, 300, 30), lifeBarEmpty, new GUIStyle() { stretchHeight = true, stretchWidth = true });

        GUI.BeginGroup(new Rect(0, 0, 250 * lifeBarCurrentPosition, 30));
        GUI.Box(new Rect(0, 0, 250, 30), lifeBarFull, new GUIStyle() { stretchHeight = true, stretchWidth = true });
        GUI.EndGroup();

        GUI.EndGroup();
        
        this.GameOver();
    }

    void Update()
    {
        this.UpdateLifeBar();

        if (this.ghostTime > 0)
        {
            this.ghostTime -= Time.deltaTime;
            this.gameObject.layer = LayerMask.NameToLayer("GhostCharacter");
            this.GetComponent<Renderer>().enabled = !this.GetComponent<Renderer>().enabled;
        }

        if (ghostTime < 0)
        {
            ghostTime = 0;
            this.GetComponent<Renderer>().enabled = true;
            this.gameObject.layer = LayerMask.NameToLayer("Character");
        }
    }

	void FixedUpdate () {

        if (!dead)
        {
			if (Input.GetAxisRaw("Vertical") > 0 && direction < 0 ||
			    Input.GetAxisRaw("Vertical") < 0 && direction > 0) ChangeDirection() ;
            Rigidbody2D rigidBody2D = this.transform.GetComponent<Rigidbody2D>();

            Vector2 newVelocity = rigidBody2D.velocity;
            newVelocity.x = this.movementSpeed;
			newVelocity.y = 0;
            rigidBody2D.velocity = newVelocity;

            if (direction > 0)
            {
                this.transform.position = new Vector3(this.transform.position.x, -1.75f);
            }
            else if (direction < 0)
            {
                this.transform.position = new Vector3(this.transform.position.x, -2.88f);
            }

			if (Input.GetAxisRaw("Cancel") > 0) Application.LoadLevel ("menu");
        }
	}

    void OnTriggerEnter2D(Collider2D collider)
    {
        float value = 0;

        switch (collider.tag)
        {
            case "Obstaculo":
                value = -10;
			    this.somBuraco.Play();
                this.ghostTime = 1;
                break;
            case "Water":
                value = 10;
                Destroy(collider.gameObject);
			    this.somAgua.Play();
                break;
            default:
                value = 0;
                break;
        }

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
