using UnityEngine;
using UnityEngine.UI;

namespace MainMenu.Leaderboard.UI
{
    public class LeaderBoardUI : UICanvas
    {
        [SerializeField] private RectTransform entriesContainer;
        [SerializeField] private GameObject leaderboardEntry;
    
        public Button backBtn;


        private void OnEnable()
        {
          //  SpawnUIEntries(Leaderboard.LeaderboardEntries);
        }
    
        private void OnDisable()
        {
            DestroyEntries(entriesContainer) ;
        }
    
        void SpawnUIEntries(int entries)
        {
            for (int i = 0; i < entries; i++)
            {
                Instantiate(leaderboardEntry, entriesContainer);
            }
        }

        void DestroyEntries(RectTransform entriesContainer)
        {
            foreach (Transform entry in entriesContainer)
            {
                Destroy(entry.gameObject);
            }
        }
    }
}