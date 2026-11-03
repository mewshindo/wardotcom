using System;
using System.Collections.Generic;
using UnityEngine;

namespace WarDotCom;

public class GunGame : Gamemode
{
    public Action<Gamemode> onGameFinished { get; set; }

    private List<Vector3> _spawnPoints = [];
    public override void StartGame()
    {
        
    }
    public override void OnTick(float dt)
    {
        
    }
    public override void EndGame()
    {
        onGameFinished?.Invoke(this);
    }
}