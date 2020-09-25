using UnityEngine;

[RequireComponent (typeof (AudioSource))]
public class Bullet : MonoBehaviour {
	// todo subclass
	// ran out of time
	[SerializeField] States.BulletType bulletType;
	public GameObject explosion;
	float speed;
	Rigidbody2D rb;
	Vector2 shootForce;

	void Awake ()
	{
		Init ();
		AddEventListeners ();
	}

	void Init ()
	{
		rb = GetComponent<Rigidbody2D> ();
		speed = GameManager.Instance.config.bulletSpeed;
	}

	void AddEventListeners ()
	{
		EventBus.AddEventListener (EVENT.LevelReset, ResetBullet);
		EventBus.AddEventListener (EVENT.LevelWon, Deactivate);
		EventBus.AddEventListener (EVENT.LevelLost, Deactivate);
		EventBus.AddEventListener (EVENT.Paused, Deactivate);
		EventBus.AddEventListener (EVENT.UnPaused, ResetBullet);
	}

	void RemoveEventListeners ()
	{
		EventBus.RemoveEventListener (EVENT.LevelReset, ResetBullet);
		EventBus.RemoveEventListener (EVENT.LevelWon, Deactivate);
		EventBus.RemoveEventListener (EVENT.LevelLost, Deactivate);
		EventBus.RemoveEventListener (EVENT.Paused, Deactivate);
		EventBus.RemoveEventListener (EVENT.UnPaused, ResetBullet);
	}

	void Update ()
	{
		if (gameObject.activeInHierarchy) {
			if (bulletType == States.BulletType.PlayerBullet) {
				shootForce = new Vector2 (0f, speed);
				rb.AddForce (shootForce, ForceMode2D.Impulse);
			} else
			if (bulletType == States.BulletType.EnemyBullet) {
				shootForce = new Vector2 (0f, -speed);
				rb.AddForce (shootForce, ForceMode2D.Impulse);
			}
		}
	}

	void OnCollisionEnter2D (Collision2D collision)
	{
		//cache
		GameObject go = collision.gameObject;
		if (bulletType == States.BulletType.PlayerBullet) {

			if (go.tag == "Enemy") {
				go.GetComponent<IDamageable> ().Damage ();
			}

		} else
		if (bulletType == States.BulletType.EnemyBullet) {
			if (go.tag == "Player" || go.tag == "Protection") {
				go.GetComponent<IDamageable> ().Damage ();
				Instantiate (explosion).transform.position = transform.position;

			}
		}
		if (!GameManager.Instance.config.easyMode)
			Deactivate (gameObject);
	}

	public void SetBulletType (States.BulletType type)
	{
		bulletType = type;
	}

	public void Activate (Vector3 pos)
	{
		gameObject.SetActive (true);
		transform.position = pos;
		// play sound
		AudioSource audio = GetComponent<AudioSource> ();
		audio.Play ();

	}

	public void Deactivate (GameObject caller)
	{
		gameObject.SetActive (false);
	}

	void ResetBullet (GameObject caller)
	{
		Init ();
	}
}
