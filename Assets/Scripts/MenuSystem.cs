using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuSystem : MonoBehaviour {

	public void PlayAgain() {

		SceneManager.LoadScene ("level");
	}
}
