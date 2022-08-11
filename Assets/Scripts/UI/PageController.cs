using UnityEngine;
using UnityEngine.UI;

public class PageController : MonoBehaviour
{
    public float startDelay;

    public Image[] Backgrounds;

    private float StartTimer = 0;
    private bool DelayConcluded = false;

    void Start()
    {
    }

    void Update()
    {
        if (!DelayConcluded)
        {
            StartTimer += Time.deltaTime;
            if (StartTimer > startDelay)
            {
                DelayConcluded = true;
                Backgrounds[0].gameObject.SetActive(true);
            }
        }
    }

    public void GoToPage(int page)
    {
        foreach (Image i in Backgrounds)
        {
            i.gameObject.SetActive(false);
        }
        Backgrounds[page - 1].gameObject.SetActive(true);
    }


}
