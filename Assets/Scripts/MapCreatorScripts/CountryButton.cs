using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountryButton : MonoBehaviour
{
    public Country country;
    [SerializeField] TMP_Text buttonText;

    public void Initialise(Country country)
    {
        this.country = country;
        buttonText.text = country.name;
    }

    public void Click()
    {
        MapCreatorController.me.EnterCountryAdjustment(country);
    }
}
