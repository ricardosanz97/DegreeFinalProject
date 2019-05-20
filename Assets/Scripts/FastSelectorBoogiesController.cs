using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FastSelectorBoogiesController : MonoBehaviour
{
    #region public
    //public Text currentAmountText;
    public TextMeshProUGUI currentAvailableText;
    public TextMeshProUGUI cleanersAmountText;
    public TextMeshProUGUI collectorsAmountText;
    public TextMeshProUGUI explorersAmountText;
    public Slider cleanersAmountSlider;
    public Slider collectorsAmountSlider;
    public Slider explorersAmountSlider;
    #endregion

    private void GetInitialInformation()
    {
        BoogiesSpawner.CurrentTotalAmount = BoogiesSpawner.CleanersAmount + BoogiesSpawner.ExplorersAmount + BoogiesSpawner.CollectorsAmount + BoogiesSpawner.CurrentBoogiesLeftAmount + BoogiesSpawner.CleanersSpawned + BoogiesSpawner.CollectorsSpawned + BoogiesSpawner.ExplorersSpawned;
        //this.currentAmountText.text = "/ " + BoogiesSpawner.CurrentTotalAmount.ToString();
        this.currentAvailableText.text = BoogiesSpawner.CurrentBoogiesLeftAmount.ToString();

        this.cleanersAmountSlider.value = BoogiesSpawner.CleanersAmount;
        this.cleanersAmountSlider.GetComponent<SliderController>().sliderValue = BoogiesSpawner.CleanersAmount;
        this.cleanersAmountText.text = BoogiesSpawner.CleanersAmount.ToString();

        this.explorersAmountSlider.value = BoogiesSpawner.ExplorersAmount;
        this.explorersAmountSlider.GetComponent<SliderController>().sliderValue = BoogiesSpawner.ExplorersAmount;
        this.explorersAmountText.text = BoogiesSpawner.ExplorersAmount.ToString();

        this.collectorsAmountSlider.value = BoogiesSpawner.CollectorsAmount;
        this.collectorsAmountSlider.GetComponent<SliderController>().sliderValue = BoogiesSpawner.CollectorsAmount;
        this.collectorsAmountText.text = BoogiesSpawner.CollectorsAmount.ToString();
    }

    private void SetInformation()
    {
        BoogiesSpawner.CleanersAmount = (int)this.cleanersAmountSlider.value;
        BoogiesSpawner.ExplorersAmount = (int)this.explorersAmountSlider.value;
        BoogiesSpawner.CollectorsAmount = (int)this.collectorsAmountSlider.value;
        BoogiesSpawner.CurrentBoogiesLeftAmount = (BoogiesSpawner.CurrentTotalAmount - (BoogiesSpawner.CleanersAmount + BoogiesSpawner.CollectorsAmount + BoogiesSpawner.ExplorersAmount + BoogiesSpawner.CleanersSpawned + BoogiesSpawner.ExplorersSpawned + BoogiesSpawner.CollectorsSpawned));
        this.currentAvailableText.text = BoogiesSpawner.CurrentBoogiesLeftAmount.ToString();
    }

    private void Start()
    {
        GetInitialInformation();
    }

    public void UpdateCleanersSlider(int value)
    {
        Debug.Log("pulsamos el botón cleaners con value " + value);
        cleanersAmountSlider.value += value;
    }

    public void UpdateCollectorsSlider(int value)
    {
        collectorsAmountSlider.value += value;
    }

    public void UpdateExplorersSlider(int value)
    {
        explorersAmountSlider.value += value;
    }

    public void UpdateCleanersAmount(int value)
    {
        cleanersAmountSlider.onValueChanged.RemoveAllListeners();
        if (value == 1 && BoogiesSpawner.CurrentBoogiesLeftAmount > 0)
        {
            cleanersAmountSlider.value++;
        }
        else if (value == -1 && BoogiesSpawner.CleanersAmount > 0)
        {
            cleanersAmountSlider.value--;
        }
        cleanersAmountText.text = cleanersAmountSlider.value.ToString();
        SetInformation();
        cleanersAmountSlider.GetComponent<SliderController>().sliderValue = (int)cleanersAmountSlider.value;
        cleanersAmountSlider.GetComponent<SliderController>().AddListener();
    }

    public void UpdateExplorersAmount(int value)
    {
        explorersAmountSlider.onValueChanged.RemoveAllListeners();
        if (value == 1 && BoogiesSpawner.CurrentBoogiesLeftAmount > 0)
        {
            explorersAmountSlider.value++;
        }
        else if (value == -1 && BoogiesSpawner.ExplorersAmount > 0)
        {
            explorersAmountSlider.value--;
        }
        explorersAmountText.text = explorersAmountSlider.value.ToString();
        SetInformation();
        explorersAmountSlider.GetComponent<SliderController>().sliderValue = (int)explorersAmountSlider.value;
        explorersAmountSlider.GetComponent<SliderController>().AddListener();
    }

    public void UpdateCollectorsAmount(int value)
    {
        collectorsAmountSlider.onValueChanged.RemoveAllListeners();
        if (value == 1 && BoogiesSpawner.CurrentBoogiesLeftAmount > 0)
        {
            collectorsAmountSlider.value++;
        }
        else if (value == -1 && BoogiesSpawner.CollectorsAmount > 0)
        {
            collectorsAmountSlider.value--;
        }
        collectorsAmountText.text = collectorsAmountSlider.value.ToString();
        SetInformation();
        collectorsAmountSlider.GetComponent<SliderController>().sliderValue = (int)collectorsAmountSlider.value;
        collectorsAmountSlider.GetComponent<SliderController>().AddListener();
    }


    public void BackButtonPressed(int num)
    {
        if (num == 0)
        {
            //cleaners
            if (FindObjectOfType<BoogieCleaner>() == null)
            {
                Debug.Log("no cleaners in");
                return;
            }
            else
            {
                Debug.Log("cleaners back button pressed = true");
                BoogiesSpawner.CleanersBackButtonPressed = true;
            }
        }
        else if (num == 1)
        {
            //explorers
            if (FindObjectOfType<BoogieExplorer>() == null)
            {
                Debug.Log("no explorers in");
                return;
            }
            else
            {
                Debug.Log("explorers back button pressed = true");
                BoogiesSpawner.ExplorersBackButtonPressed = true;
            }
        }
        else if (num == 2)
        {
            //collectors
            if (FindObjectOfType<BoogieCollector>() == null)
            {
                Debug.Log("no collectors in");
                return;
            }
            else
            {
                Debug.Log("collectors back button pressed = true");
                BoogiesSpawner.CollectorsBackButtonPressed = true;
            }
        }
    }
    
}
