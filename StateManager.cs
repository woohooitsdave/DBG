using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TKSprites
{
    public enum GameState { STOPPED = 0, RUNNING, PAUSED, DIALOG };

    public class StateManager
    {
        public GameState mainstate = GameState.RUNNING;

        public byte currDialog = 0;
        
    }
}
