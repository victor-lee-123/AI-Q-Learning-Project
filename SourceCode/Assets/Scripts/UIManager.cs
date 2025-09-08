/*
    File Name: UIManager.cs
 
    Authors: Dylan LAU (d.lau), Joel TEO (t.joel), Victor LEE (lee.v)

    File Description:
    This script manages the in-game UI for tweaking training parameters.
    It links UI elements like sliders and input fields to the public
    variables in the TrainingManager, allowing for real-time adjustments
    to the simulation's learning process.
*/
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TrainingManager trainingManager;

    [Header("Parameter Sliders")]
    public Slider learningRateSlider;
    public Slider discountFactorSlider;
    public Slider explorationDecaySlider;

    [Header("Parameter Text")]
    public TextMeshProUGUI learningRateText;
    public TextMeshProUGUI discountFactorText;
    public TextMeshProUGUI explorationDecayText;

    [Header("Reward Input Fields")]
    public TMP_InputField touchPlayerInput;
    public TMP_InputField wallCollisionInput;
    public TMP_InputField moveCloserInput;
    public TMP_InputField moveAwayInput;
    public TMP_InputField timePenaltyInput;

    void Start()
    {
        if (trainingManager == null)
        {
            trainingManager = FindObjectOfType<TrainingManager>();
            if (trainingManager == null)
            {
                Debug.LogError("UIManager could not find TrainingManager!");
                return;
            }
        }
        InitializeUI();
        AddListeners();
    }

    // Set the initial values of the UI from the TrainingManager
    void InitializeUI()
    {
        // Sliders
        learningRateSlider.value = trainingManager.learningRate;
        discountFactorSlider.value = trainingManager.discountFactor;
        explorationDecaySlider.value = trainingManager.explorationDecay;

        // Slider Text
        learningRateText.text = $"Learning Rate: {trainingManager.learningRate:F2}";
        discountFactorText.text = $"Discount Factor: {trainingManager.discountFactor:F2}";
        explorationDecayText.text = $"Exploration Decay: {trainingManager.explorationDecay:F3}";

        // Input Fields
        touchPlayerInput.text = trainingManager.reward_touchPlayer.ToString();
        wallCollisionInput.text = trainingManager.reward_wallCollision.ToString();
        moveCloserInput.text = trainingManager.reward_moveCloser.ToString();
        moveAwayInput.text = trainingManager.reward_moveAway.ToString();
        timePenaltyInput.text = trainingManager.reward_timePenalty.ToString();
    }

    // Tell the UI elements to call a function when their value changes
    void AddListeners()
    {
        // Sliders
        learningRateSlider.onValueChanged.AddListener(OnLearningRateChanged);
        discountFactorSlider.onValueChanged.AddListener(OnDiscountFactorChanged);
        explorationDecaySlider.onValueChanged.AddListener(OnExplorationDecayChanged);

        // Input Fields
        touchPlayerInput.onEndEdit.AddListener(OnTouchPlayerRewardChanged);
        wallCollisionInput.onEndEdit.AddListener(OnWallCollisionRewardChanged);
        moveCloserInput.onEndEdit.AddListener(OnMoveCloserRewardChanged);
        moveAwayInput.onEndEdit.AddListener(OnMoveAwayRewardChanged);
        timePenaltyInput.onEndEdit.AddListener(OnTimePenaltyRewardChanged);
    }

    // --- Slider Update Functions ---
    public void OnLearningRateChanged(float value)
    {
        trainingManager.learningRate = value;
        learningRateText.text = $"Learning Rate: {value:F2}";
    }

    public void OnDiscountFactorChanged(float value)
    {
        trainingManager.discountFactor = value;
        discountFactorText.text = $"Discount Factor: {value:F2}";
    }

    public void OnExplorationDecayChanged(float value)
    {
        trainingManager.explorationDecay = value;
        explorationDecayText.text = $"Exploration Decay: {value:F3}";
    }

    // --- Input Field Update Functions ---
    public void OnTouchPlayerRewardChanged(string value)
    {
        if (float.TryParse(value, out float result))
            trainingManager.reward_touchPlayer = result;
    }
    public void OnWallCollisionRewardChanged(string value)
    {
        if (float.TryParse(value, out float result))
            trainingManager.reward_wallCollision = result;
    }
    public void OnMoveCloserRewardChanged(string value)
    {
        if (float.TryParse(value, out float result))
            trainingManager.reward_moveCloser = result;
    }
    public void OnMoveAwayRewardChanged(string value)
    {
        if (float.TryParse(value, out float result))
            trainingManager.reward_moveAway = result;
    }
    public void OnTimePenaltyRewardChanged(string value)
    {
        if (float.TryParse(value, out float result))
            trainingManager.reward_timePenalty = result;
    }
}