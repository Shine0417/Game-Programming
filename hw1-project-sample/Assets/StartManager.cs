using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Text startText;

    [SerializeField] private Text _bestScoreSinceStartText;
    private const string introText = "PRESS SPACE TO START";

    [SerializeField] private AudioSource _startGameAudioSource;

    private float _bestScoreSinceStart;
    private float BestScoreSinceStart
    {
        get => _bestScoreSinceStart;

        set
        {
            _bestScoreSinceStart = value;
            _bestScoreSinceStartText.text = _bestScoreSinceStart.ToString("F2");
        }
    }

    void Start()
    {
        BestScoreSinceStart = PlayerPrefs.GetFloat("score");
        InvokeRepeating("splashText", 0.5f, 0.5f);
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            _startGameAudioSource.Play();
            StartCoroutine(LoadLevelAfterDelay(0.5f));
        }
    }

    IEnumerator LoadLevelAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    void splashText()
    {
        startText.text = (startText.text == "") ? introText : "";
    }
}
