using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MainMenu.Searching.UI
{
    public class SearchingUI : UICanvas
    {
        public Button cancelBtn;
        public TextMeshProUGUI searchingText;


        IEnumerator Searching()
        {
            searchingText.text = "searching for a game";
            yield return new WaitForSeconds(2);
            // await Task.Delay(TimeSpan.FromSeconds(2));

            StartCoroutine(nameof(MatchFound));
        }

        IEnumerator MatchFound()
        {
            searchingText.text = "match found";
            yield return new WaitForSeconds(1);
            // await Task.Delay(TimeSpan.FromSeconds(1));


            StartCoroutine(nameof(Joining));
        }

        IEnumerator Joining()
        {
            searchingText.text = "joining";
            yield return new WaitForSeconds(1);
            // await Task.Delay(TimeSpan.FromSeconds(1));
            JoinMatch();
        }

        void JoinMatch()
        {
            SceneManager.LoadScene("Game");
        }
    }
}