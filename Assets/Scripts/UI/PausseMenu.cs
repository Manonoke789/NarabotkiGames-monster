using UnityEngine;
using UnityEngine.UI;

public class PausseMenu : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;

    [SerializeField] private PlayerController player;
    [SerializeField] private EnemyController enemy;
    [SerializeField] private CombatSystem combatSystem;

    private PlayerController _playerController;
    private EnemyController _enemyController;

    /// <summary>
    /// This method is used to pause the game when the pause menu is active.
    /// </summary>
    private void Update()
    {
        // If the pause menu is active
        if (gameObject.activeSelf)
        {
            // Pause the game
            Time.timeScale = 0;
        }
    }

    private void OnEnable()
    {
        playButton.onClick.AddListener(PlayButtonDown);
        quitButton.onClick.AddListener(QuitButtonDown);
    }
    private void OnDisable()
    {
        playButton.onClick.RemoveListener(PlayButtonDown);
        quitButton.onClick.RemoveListener(QuitButtonDown);
    }

    private void PlayButtonDown()
    {
        _playerController =  Instantiate(player, player.transform.position, player.transform.rotation);
        _enemyController = Instantiate(enemy, enemy.transform.position, enemy.transform.rotation);

        gameObject.SetActive(false);
        Time.timeScale = 1;
        combatSystem.playerController = _playerController;
        combatSystem.enemyController = _enemyController;

        combatSystem.HelthBar();
    }

    private void QuitButtonDown()
    {
        Application.Quit();
    }
}
