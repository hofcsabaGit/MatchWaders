using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

	public Text highscoreText;

	void Awake ()
	{
		highscoreText.text = "Highscore: " + GameManager.Instance.Load ();
		EventBus.AddEventListener (EVENT.BackToMenu, UpdateHighScore);
	}

	void UpdateHighScore (GameObject caller)
	{
		highscoreText.text = "Highscore: " + GameManager.Instance.Load ();
	}

	public void StartGame ()
	{
		EventBus.Broadcast (EVENT.StartGame, gameObject);
	}

	public void QuitGame ()
	{
		EventBus.Broadcast (EVENT.QuitGame, gameObject);
	}

	// for testing
	public void DeleteSave ()
	{
		GameManager.Instance.Delete ();
		highscoreText.text = "Highscore: " + GameManager.Instance.Load ();
	}
}
