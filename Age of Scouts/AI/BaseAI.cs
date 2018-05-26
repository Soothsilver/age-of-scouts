using Age.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Age.AI
{
    abstract class BaseAI
    {
        protected Troop Self;
        protected BaseAI(Troop self)
        {
            Self = self;
        }
        public abstract void Update(Session session);
    }
}
