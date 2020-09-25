using UnityEngine;

public class Pool : MonoBehaviour {
	public GameObject enemyBullet;
	GameObject [] bulletPool;

	void Awake ()
	{
		Init ();
	}

	void Init ()
	{
		bulletPool = new GameObject [5];

		for (int i = 0; i < bulletPool.GetLength (0); i++) {
			bulletPool [i] = Instantiate (enemyBullet, gameObject.transform);
			bulletPool [i].GetComponent<Bullet> ().SetBulletType (States.BulletType.EnemyBullet);
			bulletPool [i].GetComponent<Bullet> ().Deactivate (gameObject);
		}

	}

	public Bullet RequestShoot ()
	{
		for (int i = 0; i < bulletPool.GetLength (0); i++) {
			if (!bulletPool [i].activeInHierarchy) {
				return bulletPool [i].GetComponent<Bullet> ();
			}
		}
		return null;
	}
}

