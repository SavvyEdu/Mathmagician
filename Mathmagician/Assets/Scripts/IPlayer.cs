﻿using UnityEngine;

public interface IPlayer{

    /// <summary>
    /// defines properties and methods related to player  
    /// </summary>

    Vector2 ReturnPosition { get; set; } //set reset postion for checkpoints
    Animator Animator { get; }

    //player states
    bool InFallZone { set; }
    bool IsReturning { get; }
    bool TouchingWater{ get; }
    bool CanSwim { set; }
    //bool Frozen { get; set; }

    //movement speeds
    float MoveSpeed { get; set; }
    float JumpSpeed { get; set; }

    /// <summary>
    /// occurs when the player falls off the map
    /// </summary>
    void OnPlayerFall();
}
