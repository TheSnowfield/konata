﻿using System;
using System.Text;

using Konata.Core.Packet;
using Konata.Core.EventArgs;
using Konata.Runtime.Base.Event;

namespace Konata.Core.Services.OidbSvc
{
    [SSOService("OidbSvc.0x570_8", "Mute member in the group")]
    public class Oidb0x570_8 : ISSOService
    {
        public bool HandleInComing(SSOMessage ssoMessage, out KonataEventArgs output)
        {
            throw new NotImplementedException();
        }

        public bool HandleOutGoing(KonataEventArgs original, out byte[] message)
        {
            throw new NotImplementedException();
        }
    }
}
