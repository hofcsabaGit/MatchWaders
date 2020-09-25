using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {

	[SerializeField] GameObject enemyPrefab;
	private GameObject [,] enemies;
	[SerializeField] private Pool pool;
	private States.EnemyMovementState movementState;
	private bool moveDown;
	private bool moveLeft;
	private bool isGridCreated;
	private List<GameObject> bottomEnemies;

	// we cancel them all eveltually todo remove
	private Coroutine moveEnemiesDown;
	private Coroutine iterateRow;
	private Coroutine requestShoot;

	// local todo move them to property files
	private float variableSpeed;
	private int combos;
	public int totalNumOfEnemies;
	private int lastMovedRow;
	private float rngShootDelay;

	// coming from config
	private int xMax;
	private int yMax;

	private float rightBorder;
	private float leftBorder;

	private float xDistance;
	private float yDistance;

	private float tickTime;
	private float randomShootTimeRange;


	void Awake ()
	{
		Init ();
		AddEventListeners ();

	}

	public void Init ()
	{
		InitProperties ();
		GenerateGrid ();
		UpdateBottomEnemies ();
		//start enemyLogic
		StartEnemyLogic (gameObject);
	}

	void AddEventListeners ()
	{
		EventBus.AddEventListener (EVENT.EnemyDied, EnemyDied);
		EventBus.AddEventListener (EVENT.Paused, StopEnemyLogic);
		EventBus.AddEventListener (EVENT.UnPaused, StartEnemyLogic);
		EventBus.AddEventListener (EVENT.LevelWon, StopEnemyLogic);
		EventBus.AddEventListener (EVENT.LevelLost, StopEnemyLogic);
		EventBus.AddEventListener (EVENT.LevelReset, ResetEnemies);
		EventBus.AddEventListener (EVENT.EnemyDiedProtector, DecreaseEnemyNumber);
	}

	void RemoveEventListeners ()
	{
		EventBus.RemoveEventListener (EVENT.EnemyDied, EnemyDied);
		EventBus.RemoveEventListener (EVENT.Paused, StopEnemyLogic);
		EventBus.RemoveEventListener (EVENT.UnPaused, StartEnemyLogic);
		EventBus.RemoveEventListener (EVENT.LevelWon, StopEnemyLogic);
		EventBus.RemoveEventListener (EVENT.LevelLost, StopEnemyLogic);
		EventBus.RemoveEventListener (EVENT.LevelReset, ResetEnemies);
	}

	void InitProperties ()
	{
		//Grid Setup (should of made a grid level base class for abstraction and inherit from there... next time)
		xMax = GameManager.Instance.config.xMax;
		yMax = GameManager.Instance.config.yMax;

		rightBorder = GameManager.Instance.config.rightBorder;
		leftBorder = GameManager.Instance.config.leftBorder;

		xDistance = GameManager.Instance.config.xDistance;
		yDistance = GameManager.Instance.config.yDistance;

		//Enemy Props
		// Reset movement speed state/flags
		tickTime = GameManager.Instance.config.enemySpeed;
		randomShootTimeRange = GameManager.Instance.config.randomEnemyShootingInterval;
		movementState = States.EnemyMovementState.LeftBorder;
		// todo add horizontal/vertical movement state instead of flags
		// ran out of time
		moveDown = false;
		moveLeft = false;
		combos = 0;
		totalNumOfEnemies = 0;
		variableSpeed = tickTime;
		bottomEnemies = new List<GameObject> ();
		rngShootDelay = UnityEngine.Random.value * randomShootTimeRange;
	}

	// todo create grid level base class and inherit from there
	// ran out of time...
	void GenerateGrid ()
	{
		// im using a flag to minimise code duplication
		if (!isGridCreated) {                  // Only create array on first run of the level		
			enemies = new GameObject [xMax, yMax];
		}
		// init the grid
		for (int i = 0; i < xMax; i++) {
			for (int j = 0; j < yMax; j++) {
				if (!isGridCreated) {  // only instantiate gameobjects on first run of the level
					enemies [i, j] = Instantiate (enemyPrefab, gameObject.transform);
				}
				Enemy enemy = enemies [i, j].GetComponent<Enemy> (); //cache script
				Vector3 enemyPosition = gameObject.transform.position - new Vector3 (-yDistance * i, xDistance * j); ;
				enemy.Activate (enemyPosition);

				States.EnemyType rng;

				// todo set type
				if (GameManager.Instance.config.isRandomColours) {
					rng = (States.EnemyType)(UnityEngine.Random.value * Enum.GetNames (typeof (States.EnemyType)).Length);
				} else {
					rng = States.EnemyType.Green;
				}
				enemy.SetEnemyType (rng);
				enemy.SetGridLocation (i, j);
				totalNumOfEnemies++;
			}
		}
		isGridCreated = true;
		ResetLastRowNumber ();
	}

	IEnumerator RequestShoot ()
	{
		yield return new WaitForSeconds (rngShootDelay);
		rngShootDelay = UnityEngine.Random.value * randomShootTimeRange;

		// pick random enemy from bottom enemy list
		GameObject randomEnemy = PickEnemyFromBottom ();
		// request a bullet from pool
		Bullet requestedBullet = pool.RequestShoot ();
		// if bullet is provided pass it to the enemy and make it shoot
		if (requestedBullet) {
			if (randomEnemy) {
				randomEnemy.GetComponent<Enemy> ().Shoot (requestedBullet);
			}
		}

		requestShoot = StartCoroutine (RequestShoot ());
	}

	GameObject PickEnemyFromBottom ()
	{
		UpdateBottomEnemies ();
		var number = UnityEngine.Random.value * bottomEnemies.Count - 1;
		int rngEnemy = (int)number;
		return bottomEnemies [rngEnemy];
	}

	void MoveEnemies ()
	{
		if (moveDown) {
			moveEnemiesDown = StartCoroutine (MoveEnemiesDown ());
		} else {
			iterateRow = StartCoroutine (IterateRow ());
		}
	}

	// [column, row]
	IEnumerator IterateRow ()       // animate vertically
	{
		for (int i = enemies.GetLength (1) - 1; i >= 0; i--) {
			if (i < lastMovedRow) {
				lastMovedRow = i;
				StartCoroutine (IterateColumn (i));
				yield return new WaitForSeconds (variableSpeed);
			}
		}
		ResetLastRowNumber ();

		MoveEnemies ();
	}

	void ResetLastRowNumber ()
	{
		lastMovedRow = enemies.GetLength (1);
	}

	IEnumerator IterateColumn (int rowNum)  // animate horizontally
	{

		if (!moveLeft) {
			//left
			for (int i = 0; i < enemies.GetLength (0); i++) {
				enemies [i, rowNum].transform.position += new Vector3 (1f, 0f);
				CheckBounds (enemies [i, rowNum]);
			}
		} else {
			//right
			for (int i = enemies.GetLength (0) - 1; i >= 0; i--) {
				enemies [i, rowNum].transform.position += new Vector3 (-1f, 0f);
				CheckBounds (enemies [i, rowNum]);
			}
		}



		yield return new WaitForSeconds (variableSpeed);

	}

	void CheckBounds (GameObject go)
	{
		if (go.activeInHierarchy) {
			if (go.transform.position.x >= rightBorder) { movementState = States.EnemyMovementState.RightBorder; moveDown = true; }
			if (go.transform.position.x <= leftBorder) { movementState = States.EnemyMovementState.LeftBorder; moveDown = true; }
		}
	}

	IEnumerator MoveEnemiesDown ()
	{
		for (int i = enemies.GetLength (1) - 1; i >= 0; i--) {
			if (i < lastMovedRow) {
				lastMovedRow = i;
				for (int j = enemies.GetLength (0) - 1; j >= 0; j--) {
					enemies [j, i].transform.position += new Vector3 (0f, -1f);
				}
				yield return new WaitForSeconds (variableSpeed);
			}
		}
		ResetLastRowNumber ();
		moveDown = false;
		// todo move it to be stateful instead of a flag
		// ran out of time
		moveLeft = !moveLeft;
		MoveEnemies ();
	}

	// callback to check combos, update the number of enemies, speed etc.
	void EnemyDied (GameObject enemy)
	{
		combos++;
		CheckCombos (enemy);
		DecreaseEnemyNumber (gameObject);
		UpdateBottomEnemies ();
	}

	void DecreaseEnemyNumber (GameObject caller)
	{
		totalNumOfEnemies--;
		CheckEnemyNumber ();
	}

	void CheckEnemyNumber ()
	{
		if (totalNumOfEnemies <= 0) {
			EventBus.Broadcast (EVENT.LevelWon, gameObject);
		} else {
			UpdateEnemySpeed ();
		}
	}

	void UpdateEnemySpeed ()
	{
		variableSpeed -= tickTime / enemies.Length;
	}

	void UpdateBottomEnemies ()
	{
		bottomEnemies.Clear ();
		for (int i = 0; i < enemies.GetLength (0); i++) {
			for (int j = enemies.GetLength (1) - 1; j >= 0; j--) {
				if (enemies [i, j].activeInHierarchy) {
					bottomEnemies.Add (enemies [i, j]);
					break;
				} else {
					bottomEnemies.Remove (enemies [i, j]);
				}
			}
		}
	}

	void CheckCombos (GameObject go)
	{
		Enemy enemy = go.GetComponent<Enemy> ();
		Vector2Int killedEnemyPos = enemy.GetGridLocation ();

		Enemy comboEnemy;

		//check left
		if (killedEnemyPos.x > 0) {
			comboEnemy = enemies [killedEnemyPos.x - 1, killedEnemyPos.y].GetComponent<Enemy> ();
			CheckMatchingColors (enemy, comboEnemy);
		}
		//  check right 
		if (enemies.GetLength (0) > killedEnemyPos.x + 1) {
			comboEnemy = enemies [killedEnemyPos.x + 1, killedEnemyPos.y].GetComponent<Enemy> ();
			CheckMatchingColors (enemy, comboEnemy);
		}
		//  check top
		if (0 < killedEnemyPos.y) {
			comboEnemy = enemies [killedEnemyPos.x, killedEnemyPos.y - 1].GetComponent<Enemy> ();
			CheckMatchingColors (enemy, comboEnemy);
		}
		//check bottom / its the top ring probably ? todo refactor
		// 5 total of combo deaths not total death ?
		if (killedEnemyPos.y < enemies.GetLength (1) - 1) {
			comboEnemy = enemies [killedEnemyPos.x, killedEnemyPos.y + 1].GetComponent<Enemy> ();
			CheckMatchingColors (enemy, comboEnemy);
		}

		//  check top right
		/*if (enemies.GetLength (0) > killedEnemyPos.x + 1 && 0 < killedEnemyPos.y) {
			comboEnemy = enemies [killedEnemyPos.x + 1, killedEnemyPos.y - 1].GetComponent<Enemy> ();
			CheckMatchingColors (enemy, comboEnemy);
		}

		//check top left
		if (killedEnemyPos.x > 0 && 0 < killedEnemyPos.y) {
			comboEnemy = enemies [killedEnemyPos.x - 1, killedEnemyPos.y - 1].GetComponent<Enemy> ();
			CheckMatchingColors (enemy, comboEnemy);
		}*/

		AddLevelScore (CalcuateComboScore (combos));
		combos = 0;

	}

	void CheckMatchingColors (Enemy enemy, Enemy comboEnemy)
	{
		if (comboEnemy.GetEnemyType () == enemy.GetEnemyType ()) {
			if (comboEnemy.gameObject.activeInHierarchy) {
				comboEnemy.KillByCombo ();
				DecreaseEnemyNumber (gameObject);
				combos++;
			}
		}
	}

	int CalcuateComboScore (int comboNum)
	{
		int totalPoints = comboNum * Fibonacci (comboNum) * 10;
		//Debug.Log ("Combo Score: " + totalPoints);
		return totalPoints;
	}

	// public for unit test
	public int Fibonacci (int n)
	{
		return FibonacciCalculation.CalculateFibonacci (n);
	}

	void AddLevelScore (int comboScore)
	{
		GameManager.Instance.SetCurrentScore (GameManager.Instance.currentScore + comboScore);


	}

	void ResetEnemies (GameObject caller)
	{
		StopEnemyLogic (gameObject);
		Init ();
	}

	void StartEnemyLogic (GameObject caller)
	{
		MoveEnemies ();
		StartCoroutine (RequestShoot ());
	}

	void StopEnemyLogic (GameObject caller)
	{
		StopAllCoroutines ();
	}

}