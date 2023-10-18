using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	[NonSerialized]
	public bool ingame;

	public GameObject loadoutUiPrefab;

	public float mouseSensitivity = 0.5f;

	public int victoryPoints = 200;

	private void Awake()
	{
		instance = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Start()
	{
		OnLevelWasLoaded(Application.loadedLevel);
	}

	private void OnLevelWasLoaded(int level)
	{
		if (IngameLevel(level))
		{
			StartGame();
		}
		else
		{
			ingame = false;
		}
	}

	private bool IngameLevel(int level)
	{
		return level > 0;
	}

	private void StartGame()
	{
		ingame = true;
		UnityEngine.Object.Instantiate(loadoutUiPrefab);
		ActorManager.instance.StartGame();
		CoverManager.instance.StartGame();
		DecalManager.instance.StartGame();
	}
}
