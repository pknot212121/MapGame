using UnityEngine;
using TMPro;

public class PlayerNicknameDisplayer : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    public void Initialise(string playerNickname, Country country)
    {
        text.text = playerNickname;
        text.color = country.color;
        Vector3 v3 = (Vector3)ShapeTools.CentroidOfACountry(country);
        v3.z = -5f;
        gameObject.transform.position = v3;
    }
}
