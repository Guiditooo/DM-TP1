using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerX : MonoBehaviourSingleton<GameManagerX>
{
    
    public enum GameMode
    {
        Single,
        Multi
    }

    public GameMode gameMode = GameMode.Single;

    public void IsMultiplayer(bool multiplayer)
    {
        gameMode = multiplayer ? GameMode.Multi : GameMode.Single;
    }

}
