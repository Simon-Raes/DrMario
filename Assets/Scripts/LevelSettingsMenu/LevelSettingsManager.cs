using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSettingsManager : MonoBehaviour
{

    [Header("Virus level")]
    [SerializeField]
    private Slider sliderVirusLevel;
    [SerializeField]
    private Text textVirusLevel;

    [Header("Speed")]
    [SerializeField]
    private Toggle toggleLow;
    [SerializeField]
    private Toggle toggleMid;
    [SerializeField]
    private Toggle toggleHi;

    [Header("Other")]
    [SerializeField]
    private Button buttonStart;

    // Use this for initialization
    void Start()
    {
        StateHolder.virusLevel = 0;
        sliderVirusLevel.onValueChanged.AddListener(delegate { ValueChangeCheck(); });

        toggleLow.onValueChanged.AddListener(delegate { OnDifficultyToggled(); });
        toggleMid.onValueChanged.AddListener(delegate { OnDifficultyToggled(); });
        toggleHi.onValueChanged.AddListener(delegate { OnDifficultyToggled(); });

        buttonStart.onClick.AddListener(OnStartClicked);
        // Set initial state
        OnDifficultyToggled();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("return"))
        {
            StartGame();
        }
    }

    // Invoked when the value of the slider changes.
    public void ValueChangeCheck()
    {
        textVirusLevel.text = sliderVirusLevel.value.ToString();

        int virusLevel = (int)sliderVirusLevel.value;
        StateHolder.virusLevel = virusLevel;
    }

    private void OnDifficultyToggled()
    {
        if (toggleLow.isOn)
        {
            StateHolder.difficulty = Difficulty.LOW;
        }
        else if (toggleMid.isOn)
        {
            StateHolder.difficulty = Difficulty.MID;
        }
        else
        {
            StateHolder.difficulty = Difficulty.HI;
        }
    }

    public void OnStartClicked()
    {
        StartGame();
    }

    private void StartGame()
    {
        SceneManager.LoadScene("MainGame");
    }
}
