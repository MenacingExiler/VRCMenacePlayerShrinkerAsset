
using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.Serialization;
using VRC.SDKBase;
using VRC.Udon;

public class ShrinkVolume : UdonSharpBehaviour
{
    [Space]
    [Header("Hover your mouse on variables for more info.")]
    [Space]
    
    [Header("Shrink time")]
    [Tooltip("Shrink time: The time it takes for the player to shrink from start to finish. Eg. 1.0 will take 1 second for player to shrink from current height to target height.")]
    //shrink time
    public float shrinkTime;
    [Space]
    
    
    [Header("Shrink modes: The script have 2 shrinking modes. Toggle between modes with Fixed Height Mode.")]
    
    //Shrink mode
    public bool fixedHeightMode;
    [FormerlySerializedAs("fixedHeightDecrease")]
    [Space]
    [Header("Fixed height mode")]
    [Tooltip("Modify player height by fixed amount. This means all player, no matter how tall they are, will resize by the same amount when triggered.")]
    //Fixed height decrease
    public float fixedHeightModify;
    
    [Header("Height mode")]
    [Tooltip("Change player height by percentage (%) even in negative.")]
    //Height multiplier
    public float heightMultiplier;
    [Tooltip("Minimum height of player. Please keep in mind that VRChat have a minimum of 0.2m. Do not cross this limit! Script will automatically set to minimum if it's less than 0.2m.")]
    //Minimum height of player
    public float minHeight;
    [Tooltip("Maximum height of player. Please keep in mind that VRChat have a maximum of 5m. Do not cross this limit! Script will automatically set to maximum if it's more than 5m.")]
    //Maximum height of player
    public float maxHeight;
    
    
    
    private bool intervalMode = false;
    private bool intervalModeUnshrink = false;
    //Local player
    private VRCPlayerApi localPlayer;
    //Original player high in metre
    private float originalPlayerHeight;

    private void Start()
    {
        //get local player
        localPlayer = Networking.LocalPlayer;
        
        //If minmax is crossed to limit, set it to limit 
        if (minHeight < 0.2f)
        {
            minHeight = 0.2f;
        }
        if (maxHeight > 5f)
        {
            maxHeight = 5f;
        }
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
                targetHeightTemp = fixedHeightModify;
                totalShrinkTime = shrinkTime;
                shrinkTimeElapsed = 0;
            
                //Run shrink lerp
                ShrinkLerp();
            }
            
            //If fixed height mode is true
            if (fixedHeightMode)
            {
                //return if shrink time is >0
                if (shrinkTime > 0)
                {
                    return;
                }
                
                FixedHeightMode();
            }
        }
    }
    
    //Fixed height mode
    public void FixedHeightMode()
    {
        //Decrease player height to fixed height
        localPlayer.SetAvatarEyeHeightByMeters(fixedHeightModify);
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
    
    //Every 0.1 second, shrink player until target height is reached or shrink is interrupted
    public void ShrinkLerp()
    {
        //Return if interval mode is false
        if (!intervalMode)
        {
            return;
        }
        
        //Run shrink lerp again after 0.25 seconds as long as shrink time elapsed is less than total shrink time
        if (shrinkTimeElapsed < totalShrinkTime)
        {
            //Increment shrink time elapsed by 0.1 seconds
            shrinkTimeElapsed += 0.1f;
            
            //Lerp player height from original height to target height
            localPlayer.SetAvatarEyeHeightByMeters(Mathf.Lerp(originHeightTemp, targetHeightTemp, shrinkTimeElapsed / totalShrinkTime));
            
            SendCustomEventDelayedSeconds("ShrinkLerp", 0.1f);
            return;
        }
        intervalMode = false;
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
            //Increment shrink time elapsed by 0.1 seconds
            shrinkTimeElapsed += 0.1f;
            
            //Lerp player height from target height to original height
            localPlayer.SetAvatarEyeHeightByMeters(Mathf.Lerp(originHeightTemp, targetHeightTemp, shrinkTimeElapsed / totalShrinkTime));
            
            SendCustomEventDelayedSeconds("UnshrinkLerp", 0.1f);
        }
        else
        {
            //Set player eye height to original height
            localPlayer.SetAvatarEyeHeightByMeters(originalPlayerHeight);
            intervalModeUnshrink = false;
        }
    }
    
}
