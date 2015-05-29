using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;
using UnityEngine.UI;

public class IntroController : MonoBehaviour {

    private GameObject optionsContainer;
    private GameObject rankingContainer;
    private List<HighScore> highScores;
    private List<GameObject> highScoreCanvasItem;

    void Start()
    {
        this.highScoreCanvasItem = new List<GameObject>();

        for (int index = 1; index <= 10; index++)
            this.highScoreCanvasItem.Add(GameObject.Find("RankingItem" + index));

        this.highScores = this.GetScores();

        this.optionsContainer = GameObject.Find("OptionMenu");
        this.rankingContainer = GameObject.Find("RankingContainer");

        GameObject.Find("DifficultLevel").GetComponent<Slider>().value = PlayerPrefs.GetInt("Difficult", 2);
        GameObject.Find("PauseCheckEffects").GetComponent<Toggle>().isOn = Convert.ToBoolean(PlayerPrefs.GetInt("PlaySoundEffects", 1));
        GameObject.Find("PauseBackgroundMusic").GetComponent<Toggle>().isOn = Convert.ToBoolean(PlayerPrefs.GetInt("PlayBackgroundMusic", 1));

        this.optionsContainer.SetActive(false);
        this.rankingContainer.SetActive(false);
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

    public void StartGame()
    {
        Application.LoadLevel("cena01_2d");
    }

    #region Ranking

    public void ShowRanking()
    {
        this.rankingContainer.SetActive(true);

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

    public void HideRanking()
    {
        this.rankingContainer.SetActive(false);
    }

    #endregion

    #region Options

    public void ShowOptions()
    {
        this.optionsContainer.SetActive(true);
    }

    public void HideOptions()
    {
        this.optionsContainer.SetActive(false);
    }

    public void PlaySoundEffects()
    {
        int playSoundEffects = Convert.ToInt32(GameObject.Find("PauseCheckEffects").GetComponent<Toggle>().isOn);

        PlayerPrefs.SetInt("PlaySoundEffects", playSoundEffects);
        PlayerPrefs.Save();
    }

    public void PlayBackgroundMusic()
    {
        int playBackgroundMusic = Convert.ToInt32(GameObject.Find("PauseBackgroundMusic").GetComponent<Toggle>().isOn);

        PlayerPrefs.SetInt("PlayBackgroundMusic", playBackgroundMusic);
        PlayerPrefs.Save();
    }

    public void ChangeDifficultLevel()
    {
        int level = (int)GameObject.Find("DifficultLevel").GetComponent<Slider>().value;
        Text difficultText = GameObject.Find("DifficultText").GetComponent<Text>();

        switch (level)
	    {
            case 1:
                difficultText.text = "Fácil";
                break;
            case 2:
                difficultText.text = "Médio";
                break;
            case 3:
                difficultText.text = "Difícil";
                break;
		    default:
                break;
	    }

        PlayerPrefs.SetInt("Difficult", level);
        PlayerPrefs.Save();
    }

    #endregion

    public void ExitGame()
    {
        Application.Quit();
    }
}