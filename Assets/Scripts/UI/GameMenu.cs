using UnityEngine;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour {
	public Text livesText;
	public Text scoreText;

	public GameObject HUD;
	public GameObject pausedMenu;
	public GameObject winMenu;
	public GameObject newHighscoreNotification;
	public GameObject loseMenu;

	public Text winScoreText;
	public Text winHighScoreText;

	public Text loseScoreText;
	public Text loseHighScoreText;


	// next level and restart button
	public void RestartLevelButtons ()
	{
		EventBus.Broadcast (EVENT.LevelReset, gameObject);
	}
	// Unpause button
	public void ContinueButton ()
	{
		EventBus.Broadcast (EVENT.UnPaused, gameObject);
	}

	public void BackToMenuButton ()
	{
		EventBus.Broadcast (EVENT.BackToMenu, gameObject);
	}
}
