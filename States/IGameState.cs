using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarDotCom.States
{
    public interface IGameState
    {
        void OnEnter();
        void OnExit();
        void OnTick(float deltaTime);
    }
}
