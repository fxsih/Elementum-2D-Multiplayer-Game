using UnityEngine;
using TMPro;

public class InfoPages : MonoBehaviour
{
    public GameObject[] pages;

    public GameObject nextButton;
    public GameObject prevButton;

    public TMP_Text pageIndicator;

    int currentPage = 0;

    void Start()
    {
        ShowPage(0);
    }

    public void NextPage()
    {
        if (currentPage < pages.Length - 1)
        {
            currentPage++;
            ShowPage(currentPage);
        }
    }

    public void PrevPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            ShowPage(currentPage);
        }
    }

    void ShowPage(int index)
    {
        currentPage = index;

        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(i == index);
        }

        // 🔥 Button visibility
        prevButton.SetActive(currentPage > 0);
        nextButton.SetActive(currentPage < pages.Length - 1);

        // 🔥 Page indicator (optional)
        if (pageIndicator != null)
        {
            pageIndicator.text = (currentPage + 1) + " / " + pages.Length;
        }
    }
}