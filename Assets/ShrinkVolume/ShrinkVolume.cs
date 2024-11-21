
using System;
using UdonSharp;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using VRC.SDKBase;
using VRC.Udon;

public class ShrinkVolume : UdonSharpBehaviour
{
    [Space]
    [Header("Hover your mouse on variables (when applicable) for more info.")]
    [Space]
    
    public bool enableModifier = false;
    
    //Target walk speed
    public float targetWalkSpeed;
    //Target sprint speed
    public float targetSprintSpeed;
    //Target strafe speed
    public float targetStrafeSpeed;
    //Target jump impulse
    public float targetJumpImpulse;
    //Gravity modifier
    public float gravityModifier;
    //Player originals
    private float originalWalkSpeed;
    private float originalSprintSpeed;
    private float originalStrafeSpeed;
    private float originalJumpImpulse;
    private float originalGravityModifier;
    
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

    [Header("Experimental and debug features")]
    [SerializeField] private bool enableMesh = false;
    public float shrinkRate = 0.1f; //Rate how fast each interval shrink

    [Tooltip("WARNING: May cause lag if set too low. Recommended to keep it at 0.25 or higher.")]
    
    private void Initializing()
    {
        //mesh renderer
        GetComponent<MeshRenderer>().enabled = enableMesh;
        
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
        
        //get player original properties
        originalWalkSpeed = localPlayer.GetWalkSpeed();
        originalSprintSpeed = localPlayer.GetRunSpeed();
        originalStrafeSpeed = localPlayer.GetStrafeSpeed();
        originalJumpImpulse = localPlayer.GetJumpImpulse();
        originalGravityModifier = localPlayer.GetGravityStrength();
    }
    
    //Player enter trigger
    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        //If player is local player
        if (player.isLocal)
        {
            Initializing();
            
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
            
            //return if any of the properties are null
            if (targetWalkSpeed == 0 || targetSprintSpeed == 0 || targetStrafeSpeed == 0 || targetJumpImpulse == 0)
            {
                return;
            }
            
            //return if modifier is disabled
            if (!enableModifier)
            {
                return;
            }
            
            //Get player original properties
            originalWalkSpeed = player.GetWalkSpeed();
            originalSprintSpeed = player.GetRunSpeed();
            originalStrafeSpeed = player.GetStrafeSpeed();
            originalJumpImpulse = player.GetJumpImpulse();
            originalGravityModifier = player.GetGravityStrength();
            //Set player properties
            player.SetWalkSpeed(targetWalkSpeed);
            player.SetRunSpeed(targetSprintSpeed);
            player.SetStrafeSpeed(targetStrafeSpeed);
            player.SetJumpImpulse(targetJumpImpulse);
            player.SetGravityStrength(gravityModifier);
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
            
            //return if any of the properties are null
            if (targetWalkSpeed == 0 || targetSprintSpeed == 0 || targetStrafeSpeed == 0 || targetJumpImpulse == 0)
            {
                return;
            }
            
            //return if modifier is disabled
            if (!enableModifier)
            {
                return;
            }
            
            //restore player properties
            player.SetWalkSpeed(originalWalkSpeed);
            player.SetRunSpeed(originalSprintSpeed);
            player.SetStrafeSpeed(originalStrafeSpeed);
            player.SetJumpImpulse(originalJumpImpulse);
            player.SetGravityStrength(originalGravityModifier);
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
            
            //restore player modifier properties
            player.SetWalkSpeed(originalWalkSpeed);
            player.SetRunSpeed(originalSprintSpeed);
            player.SetStrafeSpeed(originalStrafeSpeed);
            player.SetJumpImpulse(originalJumpImpulse);
            player.SetGravityStrength(originalGravityModifier);
            
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
    
    //Every defined interval rate, shrink player until target height is reached or shrink is interrupted
    public void ShrinkLerp()
    {
        //Return if interval mode is false
        if (!intervalMode)
        {
            return;
        }
        
        //Run shrink lerp again after interval rate as long as shrink time elapsed is less than total shrink time
        if (shrinkTimeElapsed < totalShrinkTime)
        {
            //Increment shrink time elapsed by interval rate
            shrinkTimeElapsed += shrinkRate;
            
            //Lerp player height from original height to target height
            localPlayer.SetAvatarEyeHeightByMeters(Mathf.Lerp(originHeightTemp, targetHeightTemp, shrinkTimeElapsed / totalShrinkTime));
            
            SendCustomEventDelayedSeconds("ShrinkLerp", shrinkRate);
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
        
        //Run unshrink lerp again after interval rate as long as shrink time elapsed is less than total shrink time
        if (shrinkTimeElapsed < totalShrinkTime)
        {
            //Increment shrink time elapsed by interval rate
            shrinkTimeElapsed += shrinkRate;
            
            //Lerp player height from target height to original height
            localPlayer.SetAvatarEyeHeightByMeters(Mathf.Lerp(originHeightTemp, targetHeightTemp, shrinkTimeElapsed / totalShrinkTime));
            
            SendCustomEventDelayedSeconds("UnshrinkLerp", shrinkRate);
        }
        else
        {
            //Set player eye height to original height
            localPlayer.SetAvatarEyeHeightByMeters(originalPlayerHeight);
            intervalModeUnshrink = false;
        }
    }
    
    
}
