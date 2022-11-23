using UnityEngine;

[CreateAssetMenu(fileName = "Achievement_", menuName = "TokenSkin/New Achievement", order = 1)]
public class Achievement : ScriptableObject
{
    public string achievementName;
    public string unlockProgress;
    public string unlockCondition;
    public bool isUnlocked;
    
    
    public Sprite icon;
    public string skinName;
    
    public GameObject tokenPrefab;

    //needs more logic to check the progress and conditions
}
