using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	public InputField victoryScoreInput;

	public InputField numberActorsInput;

	public InputField respawnTimeInput;

	public Slider sensitivitySlider;

	public void StartLevel(string levelName)
	{
		SaveGameSettings();
		Application.LoadLevel(levelName);
	}

	private void Start()
	{
		sensitivitySlider.value = PlayerPrefs.GetFloat("mouse sensitivity", 0.5f);
	}

	private void SaveGameSettings()
	{
		int result;
		if (int.TryParse(victoryScoreInput.text, out result))
		{
			GameManager.instance.victoryPoints = result;
		}
		int result2;
		if (int.TryParse(numberActorsInput.text, out result2))
		{
			ActorManager.instance.maxActors = result2;
		}
		int result3;
		if (int.TryParse(respawnTimeInput.text, out result3))
		{
			ActorManager.instance.spawnTime = result3;
		}
		GameManager.instance.mouseSensitivity = sensitivitySlider.value;
		PlayerPrefs.SetFloat("mouse sensitivity", sensitivitySlider.value);
		PlayerPrefs.Save();
	}
}
