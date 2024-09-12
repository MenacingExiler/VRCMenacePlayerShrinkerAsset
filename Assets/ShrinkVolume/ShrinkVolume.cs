
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ShrinkVolume : UdonSharpBehaviour
{
    [Space]
    [Header("How to use this script")]
    [Space]
    [Header("The following script will shrink the player's " + "eye height depending on what variable you set, at length of time you set.")]
    [Space]
    
    
    
    
    
    
    [Header("Shrink time: The time it takes for the player to shrink from start to finish. Eg. 1.0 will take 1 second for player to shrink from current height to target height.")]
    //shrink time
    public float shrinkTime;
    
    [Header("Shrink modes: The script have 2 shrinking modes. Toggle between modes with Fixed Height Mode.")]
    //Shrink mode
    public bool fixedHeightMode;
    [Space]
    [Header("Fixed height mode: Decrease player height by fixed amount. This means all player, no matter how tall they are, will be shrink by the same amount when triggered.")]
    //Fixed height decrease
    public float fixedHeightDecrease;
    [Space]
    [Header("Height mode: Decrease player height by percentage.")]
    [Header("This means player will be shrink by percentage of their current height when triggered. In this mode, there is also minimum and maximum height player can shrink to.")]
    //Height decrease
    public float heightDecrease;
    //Minimum height of player
    public float minHeight;
    //Maximum height of player
    public float maxHeight;
    
    
    //Local player
    private VRCPlayerApi localPlayer;
    //Original player high in metre
    private float originalPlayerHeight;

    private void Start()
    {
        //get local player
        localPlayer = Networking.LocalPlayer;
    }
    
    //Player enter trigger
    private void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        //If player is local player
        if (player.isLocal)
        {
            //Get player eye height with GetAvatarEyeHeightAsMeters
            originalPlayerHeight = player.GetAvatarEyeHeightAsMeters();
            Debug.Log("Player height: " + originalPlayerHeight);

            float newHeight;
            
            //Decrease player height by 20%
            newHeight = originalPlayerHeight * 0.8f;
            //if new high is lower than 27cm, set it to 27cm
            if (newHeight < 0.27f)
            {
                newHeight = 0.27f;
            }
            //if new high is higher than 40cm, set it to 40cm
            if (newHeight > 0.4f)
            {
                newHeight = 0.4f;
            }
            Debug.Log("New height: " + newHeight);

            //Set player eye height to new height using SetAvatarEyeHeightByMeters
            player.SetAvatarEyeHeightByMeters(newHeight);
            
        }
    }
    
    
    //Restore player height
    private void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        //If player is local player
        if (player.isLocal)
        {
            //Set player eye height to original player height using SetAvatarEyeHeightByMeters
            player.SetAvatarEyeHeightByMeters(originalPlayerHeight);
            
            //Reset player height
            Debug.Log("Player height restored");
        }
    }
}
