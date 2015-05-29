using UnityEngine;
using UnityEngine.UI;

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

public class Haddad2DController : MonoBehaviour
{
    #region Atributos

    public bool dead;
    public float startLife;
    public float currentLife;
    public float movementSpeed;
    public float secondsToStart;

    public Scrollbar lifeBar;
    public AudioSource somAgua;
    public AudioSource somBuraco;

    private uint coins;
    private uint distance;
    
    private bool started;
    private bool newScore;
    private bool scoreAdded;
    private bool playSoundEffects;
    private bool playBackgroundMusic;

    private float ghostTime;
    private float newHighScoreTime;
    private float distanceSecondCount;
    
    private Text coinsCountText;
    private Text distanceCountText;
    private Text secondsToStartText;
    private Text secondsToStartTextShadow;

    private GameObject thief;
    private GameObject gameOverPanel;
    private GameObject highScoreItem;
    private GameObject parallaxCamera;
    private GameObject pauseMenuPanel;
    private GameObject newScoreObject;
    private GameObject rankingContainer;
    private AudioSource backgroundMusic;

    private HighScore currentHighScore;
    private List<HighScore> highScores;
    private List<GameObject> highScoreCanvasItem;

    #endregion

    void Start() {

        //Limpa os highScores atuais
        //PlayerPrefs.SetString("HighScores", string.Empty);
        //PlayerPrefs.Save();
        this.secondsToStart = 4;
        this.startLife = 100f;
        this.movementSpeed = this.SetDifficult();
        this.currentLife = startLife;
        this.highScoreCanvasItem = new List<GameObject>();

        this.playSoundEffects = Convert.ToBoolean(PlayerPrefs.GetInt("PlaySoundEffects", 1));
        this.playBackgroundMusic = Convert.ToBoolean(PlayerPrefs.GetInt("PlayBackgroundMusic", 1));

        this.thief = GameObject.Find("Thief");
        this.newScoreObject = GameObject.Find("NewScore");
        this.gameOverPanel = GameObject.Find("GameOver");
        this.pauseMenuPanel = GameObject.Find("PauseMenu");
        this.parallaxCamera = GameObject.Find("ParallaxCamera");
        this.coinsCountText = GameObject.Find("CoinQuantity").GetComponent<Text>();
        this.distanceCountText = GameObject.Find("DistanceQuantity").GetComponent<Text>();
        this.secondsToStartText = GameObject.Find("SecondsToStartText").GetComponent<Text>();
        this.secondsToStartTextShadow = GameObject.Find("SecondsToStartTextShadow").GetComponent<Text>();
        this.backgroundMusic = GameObject.Find("musica").GetComponent<AudioSource>();

        GameObject.Find("PauseCheckEffects").GetComponent<Toggle>().isOn = this.playSoundEffects;
        GameObject.Find("PauseBackgroundMusic").GetComponent<Toggle>().isOn = this.playBackgroundMusic;

        for (int index = 1; index <= 10; index++)
            this.highScoreCanvasItem.Add(GameObject.Find("RankingItem" + index));

        //Oculta o objeto de novo highScore
        this.newScoreObject.SetActive(false);

        //Oculta o objeto de Game Over
        this.gameOverPanel.SetActive(false);

        //Oculta o objeto de Pause Menu
        this.pauseMenuPanel.SetActive(false);

        //Obtém os scores atuais
        this.highScores = this.GetScores();

        if (this.playBackgroundMusic)
            this.backgroundMusic.Play();
        else
            this.backgroundMusic.Stop();
    }

    float SetDifficult()
    {
        int difficultLevel = PlayerPrefs.GetInt("Difficult", 2);

        switch (difficultLevel)
        {
            case 1:
                return 6.0f;
            case 2:
                return 8.0f;
            case 3:
                return 10.0f;
            default:
                return 8.0f;
        }
    }

    void Update()
    {
        this.secondsToStart -= Time.deltaTime * 1;

        if (this.secondsToStart >= 1)
        {
            secondsToStartText.text = ((int)this.secondsToStart).ToString();
            secondsToStartTextShadow.text = secondsToStartText.text;
        }
        else
        {
            secondsToStartText.text = "P e d a l a ! !";
            secondsToStartTextShadow.text = secondsToStartText.text;
        }
        
        this.UpdateGhostTime();
    }

	void FixedUpdate () 
    {
        if (secondsToStart < 0)
        {
            if (!started)
            {
                started = true;
                secondsToStartText.gameObject.SetActive(false);
                secondsToStartTextShadow.gameObject.SetActive(false);

                this.parallaxCamera.SendMessage("StartMove");
                this.transform.GetComponent<GeneratorController>().started = true;
                this.GetComponent<Animator>().SetBool("Running", true);
                this.thief.GetComponent<Animator>().SetBool("Running", true);
            }

            if (!dead)
            {
                this.UpdateLifeBar();
                this.UpdateNewScore();
                this.UpdateCoinsCount();
                this.UpdateDistanceCount();

                Rigidbody2D rigidBody2D = this.transform.GetComponent<Rigidbody2D>();
                Vector2 newVelocity = rigidBody2D.velocity;
                newVelocity.x = this.movementSpeed;
                rigidBody2D.velocity = newVelocity;

                float direction = Input.GetAxisRaw("Vertical");

                if (direction > 0)
                {
                    this.transform.position = new Vector3(this.transform.position.x, -0.83f);
                    this.thief.transform.position = new Vector3(this.thief.transform.position.x, -0.83f);
                }
                else if (direction < 0)
                {
                    this.transform.position = new Vector3(this.transform.position.x, -1.98f);
                    this.thief.transform.position = new Vector3(this.thief.transform.position.x, -1.91f);
                }
            }
            else
                this.UpdateGameOver();
        }
	}

    /// <summary>
    /// Atualiza a barra de vida do personagem.
    /// </summary>
    void UpdateLifeBar()
    {
        this.currentLife = currentLife - Time.deltaTime * 2;
        this.lifeBar.size = this.currentLife / 100f;

        if (this.currentLife <= 0f)
            this.dead = true;
    }

    /// <summary>
    /// Exibe uma mensagem de novo highscore quando o jogo atual
    /// ultrapassar o score mais alto até então
    /// </summary>
    void UpdateNewScore()
    {
        if (this.highScores.Count > 0 && this.distance > this.highScores[0].Distance && !newScore)
        {
            this.newScore = true;
            this.newHighScoreTime = 3;
        }

        if (this.newHighScoreTime > 0)
        {
            this.newScoreObject.SetActive(true);
            this.newHighScoreTime -= Time.deltaTime;
        }

        if (this.newHighScoreTime < 0)
        {
            this.newHighScoreTime = 0;
            this.newScoreObject.SetActive(false);
        }
    }

    /// <summary>
    /// Atualiza o contador de moedas
    /// </summary>
    void UpdateCoinsCount()
    {
        if (!dead)
            this.coinsCountText.text = this.coins.ToString();
    }

    /// <summary>
    /// Atualiza a distância percorrida e o contador de distância.
    /// </summary>
    void UpdateDistanceCount()
    {
        if (!dead)
        {
            distanceSecondCount += Time.deltaTime * 1000;

            if (distanceSecondCount >= 200)
            {
                distance += 1;
                distanceSecondCount = 0;
            }

            this.distanceCountText.text = this.distance.ToString();
        }
    }

    /// <summary>
    /// Atualiza o status do personagem ao receber dano 
    /// para ficar imune por um segundo e piscando.
    /// </summary>
    void UpdateGhostTime()
    {
        if (this.ghostTime > 0)
        {
            this.ghostTime -= Time.deltaTime;
            this.gameObject.layer = LayerMask.NameToLayer("GhostCharacter");
            this.GetComponent<Renderer>().enabled = !this.GetComponent<Renderer>().enabled;
        }

        if (ghostTime < 0 || this.dead)
        {
            ghostTime = 0;
            this.GetComponent<Renderer>().enabled = true;
            this.gameObject.layer = LayerMask.NameToLayer("Character");
        }
    }

    /// <summary>
    /// Fim de jogo - Para todas as animações, movimentos e música,
    /// atualiza o ranking com as informações da partida, exibe o 
    /// ranking já atualizado e opção de iniciar um novo jogo.
    /// </summary>
    void UpdateGameOver()
    {
        //Para o personagem
        Rigidbody2D rigidBody2D = this.transform.GetComponent<Rigidbody2D>();
        rigidBody2D.velocity = new Vector2(0, 0);
        this.GetComponent<Animator>().SetBool("Running", false);

        //Para o bandidinho
        this.thief.GetComponent<Animator>().SetBool("Running", false);

        //Para a música de fundo
        if(this.backgroundMusic.isPlaying)
            this.backgroundMusic.Stop();

        this.parallaxCamera.SendMessage("StopMove");

        //Armazena o score do personagem
        if (!scoreAdded)
        {
            scoreAdded = true;
            this.AddScore();
        }

        //Monta o ranking atualizado
        this.ShowScores();

        //Exibe todas as informações de fim de jogo
        this.gameOverPanel.SetActive(true);
    }

    /// <summary>
    /// Verifica as colisões do personagem com os objetos,
    /// esse método é responsável por aumentar o diminuir 
    /// o atributo de controle da quantidade de vida do personagem
    /// </summary>
    /// <param name="collider"></param>
    void OnTriggerEnter2D(Collider2D collider)
    {
        float value = 0;

        switch (collider.tag)
        {
            case "Moeda":
                this.CollectCoin(collider);
                break;
            case "Obstaculo":
                value = -10;
			    if(this.playSoundEffects) this.somBuraco.Play();
                this.ghostTime = 1;
                break;
            case "Water":
                value = 10;
                Destroy(collider.gameObject);
			    if(this.playSoundEffects) this.somAgua.Play();
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

    /// <summary>
    /// Atualiza a quantidade de moedas obtidas
    /// </summary>
    /// <param name="coinCollider"></param>
    void CollectCoin(Collider2D coinCollider)
    {
        coins++;
        Destroy(coinCollider.gameObject);
    }

    /// <summary>
    /// Adiciona o novo score na lista e salva a configuração
    /// </summary>
    void AddScore()
    {
        this.highScores.Add(new HighScore()
        {
            Coins = this.coins,
            Date = DateTime.Now,
            Distance = this.distance
        });

        this.highScores.Sort(new HighScoreComparer());
        
        //Somente armazena os 10 melhores resultados
        if(this.highScores.Count > 10)
            this.highScores = this.highScores.GetRange(0, 10);

        for (int index = 1; index <= this.highScores.Count; index++)
            this.highScores[index - 1].Position = index;

        MemoryStream stream = new MemoryStream();
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        binaryFormatter.Serialize(stream, this.highScores);
        
        PlayerPrefs.SetString("HighScores", Convert.ToBase64String(stream.ToArray()));
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Obtém a lista de scores salva e retorna uma lista com todos eles
    /// </summary>
    /// <returns></returns>
    List<HighScore> GetScores()
    {
        string highScoresText;
        List<HighScore> highScores = new List<HighScore>();

        highScoresText = PlayerPrefs.GetString("HighScores", string.Empty);
        
        if (!string.IsNullOrEmpty(highScoresText))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            highScores = (List<HighScore>)binaryFormatter.Deserialize(new MemoryStream(Convert.FromBase64String(highScoresText)));
        }

        return highScores;
    }

    void ShowScores()
    {
        for (int index = 0; index < 10; index++)
        {
            if (index < this.highScores.Count)
            {
                foreach (Text text in this.highScoreCanvasItem[index].GetComponentsInChildren<Text>())
                {
                    if (text.name == "PositionText")
                        text.text = this.highScores[index].Position.ToString();
                    else if (text.name == "DistanceText")
                        text.text = this.highScores[index].Distance.ToString();
                    else if (text.name == "CoinsText")
                        text.text = this.highScores[index].Coins.ToString();
                    else
                        text.text = this.highScores[index].Date.ToString("dd/MM/yyyy HH:mm");
                }
            }
            else
                this.highScoreCanvasItem[index].GetComponentsInChildren<Text>().All(p => { p.text = string.Empty; return true; });
        }
    }

    public void Pause()
    {
        if (!this.dead)
        {
            if (Time.timeScale == 0)
            {
                Time.timeScale = 1;
                this.pauseMenuPanel.SetActive(false);
            }
            else
            {
                Time.timeScale = 0;
                this.pauseMenuPanel.SetActive(true);
            }
        }
    }

    public void Restart()
    {
        Time.timeScale = 1;
        Application.LoadLevel("cena01_2d");
    }

    public void Exit()
    {
        Time.timeScale = 1;
        Application.LoadLevel("Intro");
    }

    public void SoundEffects()
    {
        this.playSoundEffects = GameObject.Find("PauseCheckEffects").GetComponent<Toggle>().isOn;
        PlayerPrefs.SetInt("PlaySoundEffects", Convert.ToInt32(this.playSoundEffects));
    }

    public void BackgroundMusic()
    {
        this.playBackgroundMusic = GameObject.Find("PauseBackgroundMusic").GetComponent<Toggle>().isOn;
        PlayerPrefs.SetInt("PlayBackgroundMusic", Convert.ToInt32(this.playBackgroundMusic));

        if (this.playBackgroundMusic) 
            this.backgroundMusic.Play();
        else
            this.backgroundMusic.Stop();
    }
}

[Serializable]
public class HighScore
{
    public int Position { get; set; }
    public uint Coins { get; set; }
    public uint Distance { get; set; }
    public DateTime Date { get; set; }
}

public class HighScoreComparer :
    IComparer<HighScore>
{
    public int Compare(HighScore x, HighScore y)
    {
        return y.Distance.CompareTo(x.Distance);
    }
}