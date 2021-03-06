﻿using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

/// <summary>
/// Keeps track of common global variables that objects need access to
/// </summary>
public static class Global
{   
    #region global variables
    private static GameObject mainCamera; //Main Camera object in scene
    private static GameObject player; //player object

    public static bool paused = false; //true when game is paused and nothing is moving
    public static bool muted = false;

    public const int pixelsPerUnit = 8; //number of pixels displayed in each unity unit
    public const float buffer = 0.01f; //collision buffer 

    public static Vector2 startPosition = Vector2.one;

    #endregion

    #region global accesors

    /// <summary>
    /// accessor for the mainCamera
    /// </summary>
    public static GameObject MainCamera
    {
        get
        {
            //find the mainCamera if not assigned
            if (mainCamera == null)
            {
                mainCamera = GameObject.FindObjectOfType<Camera>().gameObject;
                Assert.IsNotNull(mainCamera, "Scene needs a Camera");
            }
            return mainCamera;
        }
        set
        {
            //make sure new mainCamera object has a camera attached
            if (value.GetComponent<Camera>() != null) { mainCamera = value; }
        }
    }

    /// <summary>
    /// accessor for player gameObject
    /// </summary>
    public static GameObject Player
    {
        get
        {
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player");
                Assert.IsNotNull(player, "Need to tag a object \"Player\"");
            }
            return player;
        }
    }

    /// <summary>
    /// accessor for player transform
    /// </summary>
    public static  Transform PlayerTransform{ get { return Player.transform; } }
    #endregion 
}
