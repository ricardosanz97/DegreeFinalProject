using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FastSelectorBoogiesController : MonoBehaviour
{
    #region public
    public Text currentAmountText;
    public Text currentAvailableText;
    public Text currentElixirText;
    public Text cleanersAmountText;
    public Text wrestlersAmountText;
    public Text collectorsAmountText;
    public Text explorersAmountText;
    public Slider cleanersAmountSlider;
    public Slider wrestlersAmountSlider;
    public Slider collectorsAmountSlider;
    public Slider explorersAmountSlider;
    #endregion
    #region private
    private int currentTotalAmount;
    private int currentBoogiesLeftAmount;
    private int currentElixir;
    private int cleanersAmount;
    private int wrestlersAmount;
    private int explorersAmount;
    private int collectorsAmount;
    #endregion

    private void GetInformation()
    {
        currentTotalAmount = BoogiesSpawner.CurrentTotalAmount;
        currentBoogiesLeftAmount = BoogiesSpawner.CurrentBoogiesLeftAmount;
        currentElixir = BoogiesSpawner.CurrentElixir;
        cleanersAmount = BoogiesSpawner.CleanersAmount;
        wrestlersAmount = BoogiesSpawner.WrestlersAmount;
        explorersAmount = BoogiesSpawner.ExplorersAmount;
        collectorsAmount = BoogiesSpawner.CollectorsAmount;
    }

    private void SetInformation()
    {
        BoogiesSpawner.CurrentBoogiesLeftAmount = currentBoogiesLeftAmount;
        BoogiesSpawner.CleanersAmount = cleanersAmount;
        BoogiesSpawner.WrestlersAmount = wrestlersAmount;
        BoogiesSpawner.ExplorersAmount = explorersAmount;
        BoogiesSpawner.CollectorsAmount = collectorsAmount;
    }

    private void Start()
    {
        GetInformation();
        OnUpdateUI();
    }

    private void OnUpdateUI()
    {
        currentAvailableText.text = currentBoogiesLeftAmount.ToString() + " /";

        currentAmountText.text = currentTotalAmount.ToString();

        currentElixirText.text = currentElixir.ToString();

        cleanersAmountSlider.value = cleanersAmount;
        cleanersAmountSlider.maxValue = currentTotalAmount;
        cleanersAmountText.text = cleanersAmount.ToString();

        wrestlersAmountSlider.value = wrestlersAmount;
        wrestlersAmountSlider.maxValue = currentTotalAmount;
        wrestlersAmountText.text = wrestlersAmount.ToString();

        explorersAmountSlider.value = explorersAmount;
        explorersAmountSlider.maxValue = currentTotalAmount;
        explorersAmountText.text = explorersAmount.ToString();

        collectorsAmountSlider.value = collectorsAmount;
        collectorsAmountSlider.maxValue = currentTotalAmount;
        collectorsAmountText.text = collectorsAmount.ToString();
    }

    public void UpdateCleanersAmount(int value)
    {
        if (value == 1 && currentBoogiesLeftAmount > 0)
        {
            cleanersAmount++;
        }
        else if (value == -1 && cleanersAmount > 0)
        {
            cleanersAmount--;
        }
        currentBoogiesLeftAmount = (currentTotalAmount - (cleanersAmount + wrestlersAmount + collectorsAmount + explorersAmount));
        SetInformation();
    }

    public void UpdateWrestlersAmount(int value)
    {
        if (value == 1 && currentBoogiesLeftAmount > 0)
        {
            wrestlersAmount++;
        }
        else if (value == -1 && wrestlersAmount > 0)
        {
            wrestlersAmount--;
        }
        currentBoogiesLeftAmount = (currentTotalAmount - (cleanersAmount + wrestlersAmount + collectorsAmount + explorersAmount));
        SetInformation();
    }

    public void UpdateExplorersAmount(int value)
    {
        if (value == 1 && currentBoogiesLeftAmount > 0)
        {
            explorersAmount++;
        }
        else if (value == -1 && explorersAmount > 0)
        {
            explorersAmount--;
        }
        currentBoogiesLeftAmount = (currentTotalAmount - (cleanersAmount + wrestlersAmount + collectorsAmount + explorersAmount));
        SetInformation();
    }

    public void UpdateCollectorsAmount(int value)
    {
        if (value == 1 && currentBoogiesLeftAmount > 0)
        {
            collectorsAmount++;
        }
        else if (value == -1 && collectorsAmount > 0)
        {
            collectorsAmount--;
        }
        currentBoogiesLeftAmount = (currentTotalAmount - (cleanersAmount + wrestlersAmount + collectorsAmount + explorersAmount));
        SetInformation();
    }
    
    private void Update()
    {
        GetInformation();
        OnUpdateUI();
    }
    
}
