using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartScene : MonoBehaviour {

	public void Restart()
	{
		SceneManager.LoadScene("Scenes/SampleScene");
		Time.timeScale = 1.0f;
	}
}
