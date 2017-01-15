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

    // [Header("Speed")]
    

    // Use this for initialization
    void Start()
    {
        StateHolder.virusLevel = 0;
        sliderVirusLevel.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("return"))
        {
            print("yes!");
            SceneManager.LoadScene("MainGame");
        }
    }

    // Invoked when the value of the slider changes.
    public void ValueChangeCheck()
    {
        textVirusLevel.text = sliderVirusLevel.value.ToString();

        int virusLevel = (int) sliderVirusLevel.value;
        StateHolder.virusLevel = virusLevel;
    }
}
