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

        //int iconIndex = 0;
        Debug.Log(icons);
        /*Debug.Log(iconIndex);

        foreach (var item in top4)
        {
            
        }*/
    }
}