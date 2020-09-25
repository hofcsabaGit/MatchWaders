using UnityEngine;

public class Player : MonoBehaviour, IDamageable {
	[SerializeField] private Transform gunEnd;
	[SerializeField] private Transform playerStartPosition;

	[SerializeField] private Bullet playerBullet;
	private float speed;

	void Awake ()
	{
		Init ();
		AddEventListeners ();
	}

	void Init ()
	{
		GameManager.Instance.SetPlayerLives (GameManager.Instance.config.lives);
		gameObject.transform.position = playerStartPosition.position;
		speed = GameManager.Instance.config.playerSpeed;
		playerBullet.Deactivate (gameObject);
		Activate (playerStartPosition.position);
	}

	void AddEventListeners ()
	{
		EventBus.AddEventListener (EVENT.LevelReset, ResetPlayer);
	}

	void RemoveEventListeners ()
	{
		EventBus.RemoveEventListener (EVENT.LevelReset, ResetPlayer);
	}

	void Update ()
	{

		if (!GameManager.Instance) {

			Debug.Log ("GameManager.Instance");
			Debug.Log (GameManager.Instance);

			Debug.Break ();
		}
		if (Input.GetKeyUp (KeyCode.Escape)) {
			if (GameManager.Instance.gameState == States.GameState.InGame) {
				EventBus.Broadcast (EVENT.Paused, gameObject);
			} else {
				EventBus.Broadcast (EVENT.UnPaused, gameObject);
			}
		}
		if (GameManager.Instance.gameState == States.GameState.InGame) {

			if (Input.GetKey (KeyCode.LeftArrow) || Input.GetKey (KeyCode.A)) {
				if (transform.position.x > GameManager.Instance.config.leftBorder)
					transform.position += Vector3.left * speed * Time.deltaTime;
			} else if (Input.GetKey (KeyCode.RightArrow) || Input.GetKey (KeyCode.D)) {
				if (transform.position.x < GameManager.Instance.config.rightBorder)
					transform.position += Vector3.right * speed * Time.deltaTime;
			}

			if (Input.GetKey (KeyCode.Space) || Input.GetKey (KeyCode.LeftControl)) {
				if (GameManager.Instance.config.easyMode) {
					Shoot ();
				} else {
					if (canShoot ()) {
						Shoot ();
					}
				}

			}
		}
	}

	void Shoot ()
	{
		playerBullet.Activate (gunEnd.position);
	}

	// cooldown
	bool canShoot ()
	{
		return !playerBullet.gameObject.activeInHierarchy; //true //for rapid fire
	}

	public void Damage ()
	{
		GameManager.Instance.SetPlayerLives (GameManager.Instance.playerLives - 1);

		if (GameManager.Instance.playerLives <= 0) {
			Kill ();
		} else {
			Activate (playerStartPosition.position);
		}
	}

	public void Kill ()
	{
		// reset game
		Deactivate ();
		EventBus.Broadcast (EVENT.LevelLost, gameObject);
	}

	public void Activate (Vector3 pos)
	{
		gameObject.SetActive (true);
		// todo reset transforms, etc.
		transform.position = pos;
	}

	public void Deactivate ()
	{
		gameObject.SetActive (false);
	}

	void ResetPlayer (GameObject caller)
	{
		Init ();
	}
}
