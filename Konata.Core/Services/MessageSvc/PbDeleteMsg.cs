﻿using System;
using Konata.Events;

namespace Konata.Services.MessageSvc
{
    public class PbDeleteMsg : ServiceRoutine
    {
        public PbDeleteMsg(EventPumper eventPumper)
            : base("MessageSvc.PbDeleteMsg", eventPumper)
        {

        }

        protected override EventParacel OnEvent(EventParacel eventParacel)
        {
            return EventParacel.Reject;
        }
    }
}
