using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarDotCom.States
{
    public class PlayingState : IGameState
    {
        private readonly GameManager _gameManager;

        public Gamemode Gamemode { get; private set; }

        private float _timeRemaining;

        public PlayingState(GameManager gameManager, Gamemode gamemode, float timeRemaining)
        {
            _gameManager = gameManager;
            Gamemode = gamemode;
            _timeRemaining = timeRemaining;
        }

        public void OnEnter()
        {
            Gamemode.onGameFinished += HandleGameFinished;
        }

        public void OnExit()
        {
            
        }

        public void OnTick(float deltaTime)
        {
            
        }

        private void HandleGameFinished(Gamemode gamemode)
        {
            throw new NotImplementedException();
        }


    }
}
