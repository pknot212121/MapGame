using UnityEngine;
using TMPro;

public class PlayerNicknameDisplayer : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    public void Initialise(string playerNickname, Country country)
    {
        text.text = playerNickname;
        text.color = country.color;
        gameObject.transform.position = ShapeTools.CentroidOfACountry(country);
    }
}
