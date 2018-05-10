using Age.Phases;
using System;

namespace Age.Core
{
    class Trigger
    {
        public Func<LevelPhase, bool> StateTrigger;
        public Action<Session> OnComplete;
    }
}