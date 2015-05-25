using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

	public Texture bgImage;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI(){
		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), bgImage);
		GUI.depth = 0;

		Camera cam = Camera.main;
		cam.Render();
	}

	public void IniciarJogo(){
		Application.LoadLevel ("cena01_2d");
		return;
	}
	
	public void SairDoJogo(){
		Application.Quit ();
		return;
	}
	
}
