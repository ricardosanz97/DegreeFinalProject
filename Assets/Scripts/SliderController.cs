using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    
    private Slider slider;
    public int sliderValue;
    public bool canChange = false;
    public BoogieType type;
    FastSelectorBoogiesController selector;
    private void Awake()
    {
        slider = GetComponent<Slider>();
        selector = FindObjectOfType<FastSelectorBoogiesController>();
    }

    private void Start()
    {
        Debug.Log("se activa por primera vez el listener");
        slider.onValueChanged.AddListener(delegate {CheckIfPossible(); });
    }

    public void AddListener()
    {
        slider.onValueChanged.AddListener(delegate { CheckIfPossible(); });
    }

    private void CheckIfPossible()
    {
        if (slider.value < sliderValue)
        {
            slider.value = sliderValue;
            switch (type)
            {
                case BoogieType.Cleaner:
                    selector.UpdateCleanersAmount(-1);
                    break;
                case BoogieType.Explorer:
                    selector.UpdateExplorersAmount(-1);
                    break;
                case BoogieType.Collector:
                    selector.UpdateCollectorsAmount(-1);
                    break;
            }
        }
        else if (slider.value > sliderValue)
        {
            slider.value = sliderValue;
            switch (type)
            {
                case BoogieType.Cleaner:
                    selector.UpdateCleanersAmount(1);
                    break;
                case BoogieType.Explorer:
                    selector.UpdateExplorersAmount(1);
                    break;
                case BoogieType.Collector:
                    selector.UpdateCollectorsAmount(1);
                    break;
            }
        }
    }

}
