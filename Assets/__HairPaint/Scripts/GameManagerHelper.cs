using System;
using UnityEngine;
using UnityEngine.SceneManagement;



public class GameManagerHelper : MonoBehaviour
{
	private static GameManagerHelper instance;
	public static GameManagerHelper Instance { get => instance; set => instance = value; }

	public bool ExecuteGame { get => IsGameStarted && !IsGameFinish; }
	public bool IsGameFinish { get; set; }
	public bool IsGameStarted { get; set; }
	public Action GameStart { get; set; }
	public Action GameWin { get; set; }
	public Action GameFail { get; set; }

	[SerializeField]
	private GameObject winPanel;
	[SerializeField]
	private GameObject failPanel;

	private void Awake()
	{
		if(instance == null)
        {
			instance = this;
        }

		GameStart += Initialize;
		GameWin += Game_Win;
		GameFail += Game_Fail;
	}

	public void Initialize()
	{
		IsGameStarted = true;
		
	}
	private void Game_Win()
	{
		IsGameFinish = true;
		TinySauce.OnGameFinished(true, 100, LevelHelper.Instance.ActiveLevel.ToString());
		winPanel.SetActive(true);
	}
	private void Game_Fail()
	{
		IsGameFinish = true;
		TinySauce.OnGameFinished(false, 50, LevelHelper.Instance.ActiveLevel.ToString());
		failPanel.SetActive(true);

	}

	public void LevelReload()
    {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

}
