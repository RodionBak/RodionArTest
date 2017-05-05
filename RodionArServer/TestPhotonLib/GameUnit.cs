using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestPhotonLib
{
    public class GameUnit
    {
        public enum BodyState
        {
            Alive, Death
        }

        public string characterName;
        public float health;
        public BodyState bodyState = BodyState.Alive;
    }
}
