﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour {

	public float fallSpeed = 1.0f;

	public int currentLevel = 0;
	private int numOfRowsCleared = 0;

	public static bool startingAtLevelZero;
	public static int startingLevel;

	public static int gridWidth = 5;
	public static int gridHeight = 15;

	public static Transform[ , , ] grid = new Transform[gridWidth, gridHeight, gridWidth];

	public int scoreOneLine = 100;
	public int scoreTwoLine = 300;
	public int scoreThreeLine = 500;

	private int numberOfRowsThisTurn = 0;

	public Text hud_score;
	public Text hud_level;
	public Text hud_rows;

	public static int currentScore = 0;

	private int startingHighScore;
	private int startingHighScore2;
	private int startingHighScore3;


	public AudioClip clearRowSound;

	private AudioSource audioSource;


	private GameObject nextTetromino;
	private GameObject previewTetromino;
	private string previewTetrominoName;

	private bool gameStarted = false;

	private Vector3 initTetrominoPosition = new Vector3 (2.0f, 15.0f, 2.0f);
	private Vector3 previewTetrominoPosition = new Vector3 (-4.0f, -1.0f, 5.0f);

	public GameObject liveTetromino;

	// Use this for initialization
	void Start () {

		currentScore = 0;

		UpdateLevel ();
		UpdateSpeed ();

		SpawnNextTetronimo ();

		audioSource = GetComponent<AudioSource> ();

		startingHighScore = PlayerPrefs.GetInt ("HighScore"); 
		startingHighScore2 = PlayerPrefs.GetInt ("HighScore2");
		startingHighScore3 = PlayerPrefs.GetInt ("HighScore3");
	}
	
	// Update is called once per frame
	void Update () {

		UpdateScore ();

		UpdateUI ();

		UpdateLevel ();

		UpdateSpeed ();
	}

	void UpdateLevel () {

		currentLevel = startingLevel + numOfRowsCleared / 10;

//		Debug.Log ("current level" + currentLevel);
	}

	void UpdateSpeed () {

		fallSpeed = 1.0f - ((float)currentLevel * 0.1f);

//		Debug.Log ("fall speed in game" + fallSpeed);
	}

	public void UpdateUI () {

		hud_score.text = currentScore.ToString ();
		hud_level.text = currentLevel.ToString ();
		hud_rows.text = numOfRowsCleared.ToString ();
	}

	public void UpdateScore () {

		if (numberOfRowsThisTurn > 0) {

			if (numberOfRowsThisTurn == 1) {

				ClearedOneRow ();
				
			} else if (numberOfRowsThisTurn == 2) {

				ClearedTwoRows ();
				
			} else if (numberOfRowsThisTurn == 3) {

				ClearedThreeRows ();
				
			}
				
			numberOfRowsThisTurn = 0;

			PlayClearRowAudio ();
		}
	}

	public void UpdateHighScore () {

		if (currentScore > startingHighScore) {

			PlayerPrefs.SetInt ("HighScore3", startingHighScore2);
			PlayerPrefs.SetInt ("HighScore2", startingHighScore);
			PlayerPrefs.SetInt ("HighScore", currentScore);

		} else if (currentScore > startingHighScore2) {

			PlayerPrefs.SetInt ("HighScore3", startingHighScore2);
			PlayerPrefs.SetInt ("HighScore2", currentScore);

		} else if (currentScore > startingHighScore3) {

			PlayerPrefs.SetInt ("HighScore3", currentScore);
		}
	}

	public void ClearedOneRow () {

		currentScore += scoreOneLine + (currentLevel * 30);

		numOfRowsCleared++;
	}

	public void ClearedTwoRows () {

		currentScore += scoreTwoLine + (currentLevel * 40);

		numOfRowsCleared += 2;
	}

	public void ClearedThreeRows () {

		currentScore += scoreThreeLine + (currentLevel * 50);

		numOfRowsCleared += 3;
	}

	public void PlayClearRowAudio () {

		audioSource.PlayOneShot (clearRowSound);
	}

	public void DeleteRow () {

		for (int y = 0; y < gridHeight; y++) {

			if (IsFullRowAt (y)) {

				DeleteMinoAt (y);

				MoveAllRowDown (y + 1);

				y--;
			}
		}
	}

	public bool IsFullRowAt (int y) {

		for (int x = 0; x < gridWidth; x++) {
			for (int z = 0; z < gridWidth; z++) {

				if (grid [x, y, z] == null) {
					return false;
				}
			}
		}

		// found one row full
		numberOfRowsThisTurn++;

		return true;
	}

	public void DeleteMinoAt (int y) {

		for (int x = 0; x < gridWidth; x++) {
			for (int z = 0; z < gridWidth; z++) {

				Destroy (grid [x, y, z].gameObject);

				grid [x, y, z] = null;
			}
		}
	}

	public void MoveAllRowDown (int y) {

		for (int i = y; i < gridHeight; i++) {

			MoveRowDown (i);
		}
	}
		
	public void MoveRowDown(int y) {

		for (int x = 0; x < gridWidth; x++) {
			for (int z = 0; z < gridWidth; z++) {

				if (grid [x, y, z] != null) {

					grid [x, y - 1, z] = grid [x, y, z];

					grid [x, y, z] = null;

					grid [x, y - 1, z].position += new Vector3 (0, -1, 0);
				}
			}
		}
	}

	public void SpawnNextTetronimo () {

		if (!gameStarted) {

			gameStarted = true;

			string nextTetroName = GetRandomTetromino ();
			string nextTetroPath = "Prefabs/Tetromino_" + nextTetroName;
			string nextTetroShadowPath = "Prefabs/Shadow_" + nextTetroName;

			nextTetromino = (GameObject)Instantiate (Resources.Load (nextTetroPath, typeof(GameObject)), initTetrominoPosition, Quaternion.identity);

			GameObject nextTetroShadow = (GameObject) Instantiate (Resources.Load(nextTetroShadowPath, typeof(GameObject)), initTetrominoPosition, Quaternion.identity);

			//- preview tetromino
			previewTetrominoName = GetRandomTetromino ();
			string previewTetroPath = "Prefabs/Tetromino_" + previewTetrominoName;

			previewTetromino = (GameObject)Instantiate (Resources.Load (previewTetroPath, typeof(GameObject)), previewTetrominoPosition, Quaternion.identity);
			previewTetromino.GetComponent<Tetromino>().enabled = false;

		} else {

			previewTetromino.transform.localPosition = initTetrominoPosition;
			nextTetromino = previewTetromino;
			nextTetromino.GetComponent<Tetromino> ().enabled = true;

			string nextTetroShadowPath = "Prefabs/Shadow_" + previewTetrominoName;
			GameObject nextTetroShadow = (GameObject) Instantiate (Resources.Load(nextTetroShadowPath, typeof(GameObject)), initTetrominoPosition, Quaternion.identity);


			//- preview tetromino
			previewTetrominoName = GetRandomTetromino ();
			string previewTetroPath = "Prefabs/Tetromino_" + previewTetrominoName;

			previewTetromino = (GameObject)Instantiate (Resources.Load (previewTetroPath, typeof(GameObject)), previewTetrominoPosition, Quaternion.identity);
			previewTetromino.GetComponent<Tetromino>().enabled = false;

		}

		liveTetromino = nextTetromino;


	}

	public bool CheckIsAboveGrid(Tetromino tetromino) {

		foreach (Transform mino in tetromino.transform) {

			Vector3 pos = Round (mino.position);

			if (pos.y > gridHeight - 1) {
				return true;
			}
		}

		return false;
	}
		
	public void UpdateGrid (Tetromino tetromino) {

		for (int y = 0; y < gridHeight; y++) {
			for (int x = 0; x < gridWidth; x++) {
				for (int z = 0; z < gridWidth; z++) {

					if (grid [x, y, z] != null) {

						if (grid [x, y, z].parent == tetromino.transform) {
							
							grid [x, y, z] = null;
						}
					}
				}
			}
		}

		foreach (Transform mino in tetromino.transform) {

			Vector3 pos = Round (mino.position);

			if (pos.y < gridHeight) {
				grid [(int) pos.x, (int) pos.y, (int) pos.z] = mino;
			}
		}

	}
		
	public Transform GetTransformAtGridPosition (Vector3 pos) {

		if (pos.y > gridHeight - 1) {

			return null;

		} else {

			return grid[(int) pos.x, (int) pos.y, (int) pos.z];
		}
	}


	public bool CheckIsInsideGrid(Vector3 pos) {
		return (int) pos.x >= 0 && (int) pos.x < gridWidth && (int) pos.z >= 0 && (int) pos.z < gridWidth && (int) pos.y >= 0;
	}

	public Vector3 Round (Vector3 pos) {
		return new Vector3 (Mathf.Round (pos.x), Mathf.Round (pos.y), Mathf.Round (pos.z));
	}

	string GetRandomTetromino () {
		int randomTetromino = Random.Range (1, 8);

		string randomTetrominoName = "";

		switch (randomTetromino) {

		case 1:
			randomTetrominoName = "2";
			break;
		case 2:
			randomTetrominoName = "3";
			break;
		case 3:
			randomTetrominoName = "Square";
			break;
		case 4:
			randomTetrominoName = "L";
			break;
		case 5:
			randomTetrominoName = "T";
			break;
		case 6:
			randomTetrominoName = "claw";
			break;
		case 7:
			randomTetrominoName = "Z";
			break;
		}

		return randomTetrominoName;
	}


	public void GameOver() {

		UpdateHighScore ();

		SceneManager.LoadScene ("GameOver");
	}
}
