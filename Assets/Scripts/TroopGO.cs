using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class TroopGO : MonoBehaviour
{
    public Troop data;
    public SpriteRenderer[] sr = new SpriteRenderer[4];
    
    void Start()
    {
        
    }

    public void Initialise(Troop data)
    {
        this.data = data;
        transform.position = ShapeTools.CentroidOfAPolygon(data.province.points);
        Revisualise();
    }

    public void Revisualise()
    {
        var top4 = data.numbers.Where(x => x.Value > 0).OrderByDescending(x => x.Value).Take(4).ToList();
        int sum = data.numbers.Values.Sum();
        int icons;
        if(sum < 25)
        {
            icons = 1;
            sr[0].gameObject.SetActive(true);
            sr[0].gameObject.transform.localPosition = new Vector3(0, 0, 0);
            sr[1].gameObject.SetActive(false);
            sr[2].gameObject.SetActive(false);
            sr[3].gameObject.SetActive(false);
        }
        else if(sum < 100)
        {
            icons = 2;
            sr[0].gameObject.SetActive(true);
            sr[0].gameObject.transform.localPosition = new Vector3(-0.4f, 0, 0);
            sr[1].gameObject.SetActive(true);
            sr[1].gameObject.transform.localPosition = new Vector3(0.4f, 0, 0);
            sr[2].gameObject.SetActive(false);
            sr[3].gameObject.SetActive(false);
        }
        else if(sum < 500)
        {
            icons = 3;
            sr[0].gameObject.SetActive(true);
            sr[0].gameObject.transform.localPosition = new Vector3(0, 0.3f, 0);
            sr[1].gameObject.SetActive(true);
            sr[1].gameObject.transform.localPosition = new Vector3(-0.4f, -0.3f, 0);
            sr[2].gameObject.SetActive(true);
            sr[2].gameObject.transform.localPosition = new Vector3(0.4f, -0.3f, 0);
            sr[3].gameObject.SetActive(false);
        }
        else
        {
            icons = 4;
            sr[0].gameObject.SetActive(true);
            sr[0].gameObject.transform.localPosition = new Vector3(-0.4f, 0.4f, 0);
            sr[1].gameObject.SetActive(true);
            sr[1].gameObject.transform.localPosition = new Vector3(0.4f, 0.4f, 0);
            sr[2].gameObject.SetActive(true);
            sr[2].gameObject.transform.localPosition = new Vector3(-0.4f, -0.4f, 0);
            sr[3].gameObject.SetActive(true);
            sr[3].gameObject.transform.localPosition = new Vector3(0.4f, -0.4f, 0);
        }

        int iconIndex = 0;

        foreach (var item in top4)
        {
            if (iconIndex >= sr.Length) break;
            float percentage = (float)item.Value / sum;
            Sprite icon = Resources.Load<Sprite>("TroopIcons/" + item.Key.iconName);
            if(icons == 1) // Tu po prostu ustawiamy pierwszego najliczniejszego
            {
                sr[0].sprite = icon;
                break;
            }
            else if(icons == 2)
            {
                if(iconIndex <= 1) // Dostaje wolne miejsce jeśli jakieś jest
                {
                    sr[iconIndex++].sprite = icon;
                }
                else break;
                if(percentage > 0.75f) // Jeśli stanowi ponad 75% wszystkich dostaje też drugie
                {
                    sr[iconIndex++].sprite = icon;
                    break;
                }
            }
            else if(icons == 3)
            {
                if(iconIndex <= 2) // Dostaje wolne miejsce jeśli jakieś jest
                {
                    sr[iconIndex++].sprite = icon;
                }
                else break;
                if(percentage > 0.50f) // Jeśli stanowi ponad 50% wszystkich dostaje też drugie
                {
                    sr[iconIndex++].sprite = icon;
                }
                if(percentage > 0.85f) // Jeśli stanowi ponad 85% wszystkich dostaje też trzecie
                {
                    sr[iconIndex++].sprite = icon;
                    break;
                }
            }
            else if(icons == 4)
            {
                if(iconIndex <= 3) // Dostaje wolne miejsce jeśli jakieś jest
                {
                    sr[iconIndex++].sprite = icon;
                }
                else break;
                if(percentage > 0.45f) // Jeśli stanowi ponad 45% wszystkich dostaje też drugie
                {
                    sr[iconIndex++].sprite = icon;
                }
                if(percentage > 0.70f) // Jeśli stanowi ponad 70% wszystkich dostaje też trzecie
                {
                    sr[iconIndex++].sprite = icon;
                }
                if(percentage > 0.90f) // Jeśli stanowi ponad 90% wszystkich dostaje też czwarte
                {
                    sr[iconIndex++].sprite = icon;
                    break;
                }
            }
        }
    }
}