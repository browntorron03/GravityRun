using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager gm;

    public enum gameStates { Playing, Death, GameOver, BeatLevel };
    public gameStates gameState = gameStates.Playing;

    public Transform Player;
    private Vector3 playerSpawnLocation;

    [HideInInspector]
    public int currentGold;
    private int currentScore;
    public GameObject trophy; // Assign in the Inspector or use for logic if needed

    public TMP_Text goldText;
    public GameObject winText;
    public GameObject GameOverText;
    public GameObject PlayAgainButton;
    public GameObject NextLevelButton;
    public bool canBeatLevel = false;

    public AudioClip backgroundSFX;
    public AudioClip gameOverSFX;
    public AudioClip beatLevelSFX;
    public GameObject PauseCanvas;
    public bool pauseGame = false;

    private AudioSource audioSource;
    public Camera cam;

    void Start()
    {
        gm = this;
        pauseGame = false;
        SetUpDefaults();
    }

    void Update()
    {
        
    }

    void SetUpDefaults()
    {
        winText.SetActive(false);
        PlayAgainButton.SetActive(false);
        NextLevelButton.SetActive(false);

        if (Player != null)
        {
            playerSpawnLocation = Player.position;
        }
        else
        {
            Debug.LogError("Player not assigned in GameManager!");
        }
    }

    void ResetGame()
    {
        currentScore = 0;
        gameState = gameStates.Playing;
        pauseGame = false;

        SetUpDefaults();

        if (audioSource != null)
        {
            audioSource.Stop();
            playAudioRepeat(backgroundSFX);
        }

        goldText.text = "Score: " + currentScore;
    }
    public void playAgain()
    {
        ResetGame();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void nextLevel()
    {
        ResetGame();
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("No more levels available!");
        }
    }

    public void BeatLevel()
    {
            winText.SetActive(true);
            NextLevelButton.SetActive(true);
            PlayAgainButton.SetActive(true);
            //RestartGameButton.SetActive(true);
            audioSource.Stop();
            playAudioOneTime(beatLevelSFX);
            //destroyAllEnemy();
            pauseGame = true;
    }

    public void CollectTrophy()
    {
            if (trophy != null)
            {
                Destroy(trophy); // Optional: visually remove trophy
                canBeatLevel = true; 
                pauseGame = true;
                BeatLevel(); // This already triggers the win UI/audio
            }

           
    }

    public void restartGame()
    {
        SceneManager.LoadScene("Level 1");
    }


    public void AddGold(int goldToAdd)
    {
        currentGold += goldToAdd;
        currentScore = currentGold * 100;
        goldText.text = "Score: " + currentScore;
    }

    void playAudioRepeat(AudioClip clip)
    {
        audioSource = cam.GetComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.Play();
    }

    void playAudioOneTime(AudioClip clip)
    {
        AudioSource.PlayClipAtPoint(clip, cam.transform.position);
    }
}
