using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary> Manages the state of the whole application </summary>
public class GameManager : MonoBehaviour {
	[Header ("Variables")]
	[SerializeField] private string menuScene;
	[SerializeField] private string gameScene;
	[SerializeField]
	private Config _config;

	public Config config { get { return _config; } }

	public States.GameState gameState;

	public int highScore { get; private set; }
	public int currentScore { get; private set; }
	public int playerLives { get; private set; }

	[Header ("GUI")]
	[SerializeField] private GameMenu gameMenu;

	// singleton pattern
	private static GameManager _instance;

	public static GameManager Instance { get { return _instance; } }

	public AudioManager audioManager;

	// set first in script execution order (for config to populate)
	void Awake ()
	{
		if (_instance != null && _instance != this) {
			Destroy (this.gameObject);
		} else {
			_instance = this;
			DontDestroyOnLoad (this.gameObject);
			Application.targetFrameRate = 60;
			Init ();
		}
	}

	void Init ()
	{
		//todo load save if any default to 0
		highScore = Load ();
		gameState = States.GameState.MainMenu;
		//for menu
		AddEventListeners ();
	}

	void AddEventListeners ()
	{
		EventBus.AddEventListener (EVENT.LevelWon, LevelWon);
		EventBus.AddEventListener (EVENT.LevelLost, LevelLost);
		EventBus.AddEventListener (EVENT.LevelReset, LevelReset);
		EventBus.AddEventListener (EVENT.Paused, LevelPaused);
		EventBus.AddEventListener (EVENT.UnPaused, LevelUnPaused);
		EventBus.AddEventListener (EVENT.BackToMenu, BackToMenu);
		EventBus.AddEventListener (EVENT.StartGame, Play);
		EventBus.AddEventListener (EVENT.QuitGame, Quit);
	}
	void RemoveEventListeners ()
	{
		EventBus.RemoveEventListener (EVENT.LevelWon, LevelWon);
		EventBus.RemoveEventListener (EVENT.LevelLost, LevelLost);
		EventBus.RemoveEventListener (EVENT.LevelReset, LevelReset);
		EventBus.RemoveEventListener (EVENT.Paused, LevelPaused);
		EventBus.RemoveEventListener (EVENT.UnPaused, LevelUnPaused);
		EventBus.RemoveEventListener (EVENT.BackToMenu, BackToMenu);
		EventBus.RemoveEventListener (EVENT.StartGame, Play);
		EventBus.RemoveEventListener (EVENT.QuitGame, Quit);
	}

	public void Play (GameObject caller)
	{
		StartCoroutine (LoadScene (gameScene));
		gameState = States.GameState.InGame;
	}

	public void Quit (GameObject caller)
	{
		Application.Quit ();
	}

	private IEnumerator LoadScene (string sceneName)
	{
		//TODO Clean Up listeners from earlier playtrough

		//Ran out of time had no time to do it properly per class
		EventBus.FlushEventListeners ();
		//for gameplay
		AddEventListeners ();
		// TODO Audio hack
		audioManager.AddEventListeners (gameObject);

		Debug.Log ("Loading game!");
		yield return new WaitForSeconds (.4f);
		AsyncOperation operation = SceneManager.LoadSceneAsync (sceneName, LoadSceneMode.Single);

		while (!operation.isDone) {
			yield return null;
		}


		// do it only after loading is complete
		if (gameState == States.GameState.InGame) {
			gameMenu.HUD.SetActive (true);
		}

		if (gameState == States.GameState.MainMenu) {
			gameMenu.HUD.SetActive (false);
		}
	}

	public void LevelWon (GameObject caller)
	{
		// do we have a new highscore?
		if (currentScore > highScore) {
			SetNewHighScore (currentScore);
			EventBus.Broadcast (EVENT.NewHighScore, gameObject);
		}
		gameState = States.GameState.WinGame;
		gameMenu.winHighScoreText.text = "HighScore:\n" + highScore;
		gameMenu.winMenu.SetActive (true);
	}

	public void LevelLost (GameObject caller)
	{
		gameState = States.GameState.LoseGame;
		gameMenu.livesText.text = "Game Over!";
		// todo load highscore data
		gameMenu.loseHighScoreText.text = "HighScore:\n" + highScore;
		gameMenu.loseMenu.SetActive (true);
	}

	//call after next level button, restart button
	public void LevelReset (GameObject caller)
	{
		SetCurrentScore (0);
		gameState = States.GameState.InGame;
		gameMenu.newHighscoreNotification.SetActive (false);
		gameMenu.winMenu.SetActive (false);
		gameMenu.loseMenu.SetActive (false);
	}

	public void LevelPaused (GameObject caller)
	{
		gameState = States.GameState.Paused;
		gameMenu.pausedMenu.SetActive (true);
	}

	public void LevelUnPaused (GameObject caller)
	{
		gameState = States.GameState.InGame;
		gameMenu.pausedMenu.SetActive (false);
	}

	public void BackToMenu (GameObject caller)
	{
		SetCurrentScore (0);
		gameState = States.GameState.MainMenu;
		gameMenu.newHighscoreNotification.SetActive (false);
		gameMenu.winMenu.SetActive (false);
		gameMenu.loseMenu.SetActive (false);
		gameMenu.pausedMenu.SetActive (false);
		StartCoroutine (LoadScene (menuScene));
	}

	public void SetPlayerLives (int lives)
	{
		playerLives = lives;
		gameMenu.livesText.text = "Lives: " + playerLives;
	}

	public void SetCurrentScore (int levelScore)
	{
		currentScore = levelScore;
		// todo move out to function
		// ran out of time...
		gameMenu.scoreText.text = "" + currentScore;
		gameMenu.winScoreText.text = "Score:\n" + currentScore;
		gameMenu.loseScoreText.text = "Score:\n" + currentScore;
	}

	public void SetNewHighScore (int newHighScore)
	{
		gameMenu.newHighscoreNotification.SetActive (true);
		highScore = newHighScore;
		// todo save highscore
		// ran out of time, weak implementation...
		Save (highScore);
	}

	void Save (int score)
	{
		SaveData data = new SaveData ();
		data.highscore = score;

		string jsonData = JsonUtility.ToJson (data, true);
		File.WriteAllText (Application.persistentDataPath + "/highscore.sav", jsonData);
	}

	public int Load ()
	{
		if (File.Exists (Application.persistentDataPath + "/highscore.sav")) {
			SaveData data = JsonUtility.FromJson<SaveData> (File.ReadAllText (Application.persistentDataPath + "/highscore.sav"));
			return data.highscore;
		} else {
			return 0;
		}
	}
	// for testing
	public void Delete ()
	{
		if (File.Exists (Application.persistentDataPath + "/highscore.sav")) {
			File.Delete (Application.persistentDataPath + "/highscore.sav");
		}
		highScore = 0;
		Save (highScore);
	}
}

//todo  Properly create entities, with dependency injection

/*	No time, hotlinking and config then :( Ignore!
 *	
public class Factory : MonoBehaviour {

	GameManager gameManager;
	Config config;

	Model model;

	void Awake () {
		Init ();
	}

	void Init () {
		model = new Model ();
		CreateGameManager (model, config);
		CreateEnemyManager (model, config);
		CreatePlayer (model, new PlayerProperties(config));
		CreatePlayerBullet (model, new PlayerBulletProperties(config));
		CreateProtections (model, new ProtectionProperties(config));
		Crea
	}
}

//for dynamic data
public class Model() {
	int highscore;
	int lives;

	Model(){

	}

}
*/