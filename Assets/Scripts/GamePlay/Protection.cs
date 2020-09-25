using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Protection : MonoBehaviour, IDamageable {
	[SerializeField] private SpriteRenderer sprite;
	private int lives;

	void Awake ()
	{
		Init ();
		AddEventListeners ();
	}

	void Init ()
	{
		lives = GameManager.Instance.config.protectionLives;
		Color tmp = sprite.color;
		tmp.a = 1.0f;
		sprite.color = tmp;
		Activate ();
	}

	void AddEventListeners ()
	{
		EventBus.AddEventListener (EVENT.LevelReset, ResetProtection);
	}

	void RemoveEventListeners ()
	{
		EventBus.RemoveEventListener (EVENT.LevelReset, ResetProtection);
	}

	public void Damage ()
	{
		lives--;
		if (lives <= 0) {
			Kill ();
		}

		Color tmp = sprite.color;
		tmp.a = sprite.color.a / 2;
		sprite.color = tmp;
	}

	void Kill ()
	{
		// reset game
		Deactivate ();
	}

	public void Activate ()
	{
		gameObject.SetActive (true);
	}

	public void Deactivate ()
	{
		gameObject.SetActive (false);
	}

	void ResetProtection (GameObject caller)
	{
		Init ();
	}
}
