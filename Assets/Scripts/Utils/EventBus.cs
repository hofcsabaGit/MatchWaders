using System;
using System.Collections.Generic;
using UnityEngine;

// could go to States, ppl. prefer it here
public enum EVENT { EnemyDied, EnemyDiedProtector, LevelReset, NewHighScore, LevelWon, LevelLost, Paused, UnPaused, BackToMenu, StartGame, QuitGame, AudioRefresh };

public static class EventBus {
	private static Dictionary<EVENT, Action<GameObject>> eventTable
		     = new Dictionary<EVENT, Action<GameObject>> ();

	public static void AddEventListener (EVENT evnt, Action<GameObject> action)
	{
		if (!eventTable.ContainsKey (evnt)) eventTable [evnt] = action;
		else eventTable [evnt] += action;
	}

	public static void RemoveEventListener (EVENT evnt, Action<GameObject> action)
	{
		if (eventTable [evnt] != null)
			eventTable [evnt] -= action;
		if (eventTable [evnt] == null)
			eventTable.Remove (evnt);
	}

	public static void Broadcast (EVENT evnt, GameObject self)
	{
		if (eventTable [evnt] != null) eventTable [evnt] (self);
	}

	public static void FlushEventListeners ()
	{
		eventTable = new Dictionary<EVENT, Action<GameObject>> ();
	}

}
