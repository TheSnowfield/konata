﻿using System;

using Konata.Core.Events;
using Konata.Core.Packets;
using Konata.Core.Attributes;

namespace Konata.Core.Services.MessageSvc
{
    [Service("MessageSvc.PbDeleteMsg", "Delete message")]
    public class PbDeleteMsg : IService
    {
        public bool Parse(SSOFrame input, BotKeyStore signInfo, out ProtocolEvent output)
        {
            throw new NotImplementedException();
        }

        public bool Build(Sequence sequence, ProtocolEvent input,
            BotKeyStore signInfo, BotDevice device, out int newSequence, out byte[] output)
        {
            throw new NotImplementedException();
        }
    }
}
