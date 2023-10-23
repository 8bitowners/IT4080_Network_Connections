using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpFireRate :  BasePowerUp

{
    public float betterTimeBetweenBullets = .2f; 

    protected override bool ApplyToPlayer(Player thePickerUpper) {
        
        //This line doesn't seem to be working?
        if(thePickerUpper.bulletSpawner.timeBetweenBullets <= betterTimeBetweenBullets) {
            //NetworkHelper.Log("we have recognized that you already shoot fast");

            //This branch is being hit and won't stop the player from picking up power ups despite the return statement. 
            return false; 
        } else {
            thePickerUpper.bulletSpawner.timeBetweenBullets = betterTimeBetweenBullets; 
            return true; 
        }
    }
}
