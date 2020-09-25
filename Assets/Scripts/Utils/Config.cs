using UnityEngine;

[CreateAssetMenu (fileName = "ConfigData", menuName = "MatchShooter/Config", order = 1)]
public class Config : ScriptableObject {


	[Header ("Player")]
	public float playerSpeed = 0.5f;
	public int lives = 3;

	[Header ("Protection")]
	public int protectionLives = 5;

	[Header ("Enemy")]
	public float enemySpeed = 0.3f;
	public float randomEnemyShootingInterval = 5.0f;

	[Header ("Bullet")]
	public float bulletSpeed = 50.0f;

	[Header ("Grid")]
	public int xMax = 16;
	public int yMax = 5;

	public float rightBorder = 13f;
	public float leftBorder = -13f;

	public float xDistance = 1.5f;
	public float yDistance = 1.5f;

	public bool isRandomColours = false;
	public bool easyMode = false;           // :)
}