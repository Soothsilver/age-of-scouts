using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Age.Core;

namespace Age.AI
{
    class DoNothingAI : BaseAI
    {
        public DoNothingAI(Troop self) : base(self)
        {
        }

        public override void Update(Session session)
        {
            // Do nothing.
        }
    }
}
