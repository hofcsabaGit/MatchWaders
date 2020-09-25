using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Enemy : MonoBehaviour, IDamageable {

	public GameObject explosion;
	[SerializeField] private Transform gunEnd;
	[SerializeField] private States.EnemyType enemyType;
	[SerializeField] private SpriteRenderer sprite;
	[SerializeField] private Vector2Int gridLocation;

	//KVP for States and colors
	private Dictionary<States.EnemyType, Color> colorTable
		= new Dictionary<States.EnemyType, Color> ();

	// todo test remove
	public Text debugText;

	void Awake ()
	{
		Init ();
	}

	void Init ()
	{
		InitColorKVP ();
	}

	void InitColorKVP ()
	{
		colorTable.Add (States.EnemyType.Red, Color.red);
		colorTable.Add (States.EnemyType.Green, Color.green);
		colorTable.Add (States.EnemyType.Blue, Color.blue);
		colorTable.Add (States.EnemyType.Yellow, Color.yellow);
	}

	public void SetEnemyType (States.EnemyType type)
	{
		enemyType = type;
		sprite.color = colorTable [enemyType];
	}

	public States.EnemyType GetEnemyType ()
	{
		return enemyType;
	}

	public void SetGridLocation (int x, int y)
	{
		gridLocation = new Vector2Int (x, y);
	}

	public Vector2Int GetGridLocation ()
	{
		return gridLocation;
	}

	public void Shoot (Bullet bullet)
	{
		bullet.Activate (gunEnd.position);
	}

	public void Damage ()
	{
		Kill ();
	}

	void Kill ()
	{
		// fire event
		EventBus.Broadcast (EVENT.EnemyDied, gameObject);
		Deactivate ();
	}

	public void KillByCombo ()
	{
		// reset game
		Deactivate ();
	}

	void OnCollisionEnter2D (Collision2D collision)
	{
		//cache
		GameObject go = collision.gameObject;

		if (go.tag == "Player" || go.tag == "Protection") {

			if (go.tag == "Player") {
				go.GetComponent<Player> ().Kill ();
			} else {
				go.GetComponent<IDamageable> ().Damage ();
				// todo decrease the total enemy number when hit by protector but dont add score
				EventBus.Broadcast (EVENT.EnemyDiedProtector, gameObject);
			}
			Deactivate ();
		}
	}

	public void Activate (Vector3 pos)
	{
		gameObject.SetActive (true);
		transform.position = pos;
	}

	public void Deactivate ()
	{
		Instantiate (explosion).transform.position = transform.position;
		gameObject.SetActive (false);
	}
}
