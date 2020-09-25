using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
	public AudioSource jukebox;
	public AudioSource sfx;

	public AudioClip bgmBase;
	public AudioClip bgmEasy;
	public AudioClip win;
	public AudioClip lose;
	public AudioClip highscore;
	public AudioClip startVO;

	void Awake ()
	{
		AddEventListeners (gameObject);
		StartMusic ();
	}

	public void AddEventListeners (GameObject caller)
	{
		EventBus.AddEventListener (EVENT.StartGame, StartGame);
		EventBus.AddEventListener (EVENT.LevelWon, LevelWon);
		EventBus.AddEventListener (EVENT.LevelLost, LevelLost);
		EventBus.AddEventListener (EVENT.NewHighScore, NewHighScore);
	}

	void StartMusic ()
	{
		if (GameManager.Instance.config.easyMode) {
			jukebox.clip = bgmEasy;
		} else {
			jukebox.clip = bgmBase;
		}
		jukebox.Play (0);
	}

	void StartGame (GameObject caller)
	{
		sfx.clip = startVO;
		sfx.Play (0);
	}

	void LevelWon (GameObject caller)
	{
		sfx.clip = win;
		sfx.Play (0);
	}

	void LevelLost (GameObject caller)
	{
		sfx.clip = lose;
		sfx.Play (0);
	}

	void NewHighScore (GameObject caller)
	{
		Debug.Log ("Play New High Score!");
		StartCoroutine ("PlayHihgscoreAfterWin");
	}

	IEnumerator PlayHihgscoreAfterWin ()
	{


		yield return new WaitForSeconds (2f);
		sfx.clip = highscore;
		sfx.Play (0);
	}
}
