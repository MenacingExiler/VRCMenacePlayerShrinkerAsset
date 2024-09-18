
using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.Serialization;
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
    [Header("Fixed height mode: Decrease player height by fixed amount.")]
    [Header("This means all player, no matter how tall they are, will be shrink by the same amount when triggered.")]
    //Fixed height decrease
    public float fixedHeightDecrease;
    [FormerlySerializedAs("heightDecrease")]
    [Space]
    [Header("Height mode: Change player height by percentage (%) even in negative.")]
    //Height multiplier
    public float heightMultiplier;
    //Minimum height of player
    public float minHeight;
    //Maximum height of player
    public float maxHeight;


    
    private bool intervalMode = false;
    private bool intervalModeUnshrink = false;
    //Local player
    private VRCPlayerApi localPlayer;
    //Original player high in metre
    [SerializeField] private float originalPlayerHeight;

    private void Start()
    {
        //get local player
        localPlayer = Networking.LocalPlayer;
    }
    
    //Player enter trigger
    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        //If player is local player
        if (player.isLocal)
        {
            //Get player original height
            originalPlayerHeight = player.GetAvatarEyeHeightAsMeters();
            
            //if shrink time is >0, switch to interval mode
            if (shrinkTime > 0)
            {
                intervalMode = true;
                //Set all temp for interval mode
                originHeightTemp = player.GetAvatarEyeHeightAsMeters();
                targetHeightTemp = fixedHeightDecrease;
                totalShrinkTime = shrinkTime;
                shrinkTimeElapsed = 0;
            
                //Run shrink lerp
                ShrinkLerp();
            }
            
            //If fixed height mode is true
            if (fixedHeightMode)
            {
                FixedHeightMode();
            }
            else
            {
                HeightMode();
            }
        }
    }
    
    //Fixed height mode
    public void FixedHeightMode()
    {
        //Decrease player height to fixed height
        localPlayer.SetAvatarEyeHeightByMeters(fixedHeightDecrease);
    }
    
    //Height mode
    public void HeightMode()
    {
        //Convert percentage to decimal
        float heightMultiplierDecimal = heightMultiplier / 100;
        
        //Calculate player height
        float newHeight = originalPlayerHeight * heightMultiplierDecimal;
        //
    }
    
    //Player changes avatar
    private void OnPlayerAvatarChanged(VRCPlayerApi player)
    {
        //disable interval mode
        intervalMode = false;
        
        //If player is local player
        if (player.isLocal)
        {
            //Get player eye height with GetAvatarEyeHeightAsMeters
            originalPlayerHeight = player.GetAvatarEyeHeightAsMeters();
            Debug.Log("Player height: " + originalPlayerHeight);
            
            //If fixed height mode is true
            if (fixedHeightMode)
            {
                FixedHeightMode();
            }
            else
            {
                HeightMode();
            }
        }
    }
    
    
    //Restore player height
    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        //If player is local player
        if (player.isLocal)
        {
            //disable interval mode
            intervalMode = false;
        
            //if shrink time is >0, switch to interval mode unshrink
            if (shrinkTime > 0)
            {
                intervalModeUnshrink = true;
                //Set all temp for interval mode
                originHeightTemp = player.GetAvatarEyeHeightAsMeters();
                targetHeightTemp = originalPlayerHeight;
                totalShrinkTime = shrinkTime;
                shrinkTimeElapsed = 0;
            
                //Run unshrink lerp
                UnshrinkLerp();
            }
            else
            {
                //Set player eye height to original height
                localPlayer.SetAvatarEyeHeightByMeters(originalPlayerHeight);
            }
            
            //Reset player height
            Debug.Log("Player height restored");
        }
    }
    
    //Origin height
    float originHeightTemp;
    //Target height
    float targetHeightTemp;
    //Total shrink time
    float totalShrinkTime;
    //Shrink time elapsed
    float shrinkTimeElapsed;
    
    //Every quarter of a second, shrink player until target height is reached or shrink is interrupted
    public void ShrinkLerp()
    {
        //Return if interval mode is false
        if (!intervalMode)
        {
            intervalMode = false;
            return;
        }
        
        //Run shrink lerp again after 0.25 seconds as long as shrink time elapsed is less than total shrink time
        if (shrinkTimeElapsed < totalShrinkTime)
        {
            //Increment shrink time elapsed by 0.25 seconds
            shrinkTimeElapsed += 0.25f;
            
            //Lerp player height from original height to target height
            localPlayer.SetAvatarEyeHeightByMeters(Mathf.Lerp(originHeightTemp, targetHeightTemp, shrinkTimeElapsed / totalShrinkTime));
            
            SendCustomEventDelayedSeconds("ShrinkLerp", 0.25f);
        }
        else
        {
            //Set player eye height to target height
            intervalMode = false;
        }
    }
    
    //vice versa of shrink lerp
    public void UnshrinkLerp()
    {
        //Return if interval mode unshrink is false
        if (!intervalModeUnshrink)
        {
            return;
        }
        
        //Run unshrink lerp again after 0.25 seconds as long as shrink time elapsed is less than total shrink time
        if (shrinkTimeElapsed < totalShrinkTime)
        {
            //Increment shrink time elapsed by 0.25 seconds
            shrinkTimeElapsed += 0.25f;
            
            //Lerp player height from target height to original height
            localPlayer.SetAvatarEyeHeightByMeters(Mathf.Lerp(targetHeightTemp, originHeightTemp, shrinkTimeElapsed / totalShrinkTime));
            
            SendCustomEventDelayedSeconds("UnshrinkLerp", 0.25f);
        }
        else
        {
            //Set player eye height to original height
            localPlayer.SetAvatarEyeHeightByMeters(originHeightTemp);
            intervalModeUnshrink = false;
        }
    }
    
}
