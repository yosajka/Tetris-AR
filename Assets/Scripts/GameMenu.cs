﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour {

	public Text levelText;
	public Text highScoreText;
	public Text highScoreText2;
	public Text highScoreText3;


	// Use this for initialization
	void Start () {

		if (highScoreText != null) {

			highScoreText.text = PlayerPrefs.GetInt ("HighScore").ToString ();
			highScoreText2.text = PlayerPrefs.GetInt ("HighScore2").ToString ();
			highScoreText3.text = PlayerPrefs.GetInt ("HighScore3").ToString ();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void PlayGame () {

		SceneManager.LoadScene ("testing");
	}

	public void PlayGameARmode () {
		
		SceneManager.LoadScene ("level");
	}

	public void LevelChanged (float value) {

		Game.startingLevel = (int)value;
		levelText.text = value.ToString ();
	}

	public void PlayAgain() {

		SceneManager.LoadScene ("testing");
	}

	public void BackToGameMenu () {

		SceneManager.LoadScene ("GameMenu");
	}
}
