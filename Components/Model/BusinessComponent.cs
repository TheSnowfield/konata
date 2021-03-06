﻿using System;
using System.Text;
using System.Threading.Tasks;

using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Entity;
using Konata.Core.Message;
using Konata.Core.Attributes;

namespace Konata.Core.Components.Model
{
    [Component("BusinessComponent", "Konata Business Component")]
    internal class BusinessComponent : InternalComponent
    {
        public string TAG = "BusinessComponent";

        private OnlineStatusEvent.Type _onlineType;
        private TaskCompletionSource<WtLoginEvent> _userOperation;

        public BusinessComponent()
        {
            _onlineType = OnlineStatusEvent.Type.Offline;
        }

        public async Task<bool> Login()
        {
            if (_onlineType == OnlineStatusEvent.Type.Offline)
            {
                if (!await SocketComponent.Connect(true))
                {
                    return false;
                }

                var wtStatus = await WtLogin();
                {
                    while (true)
                    {
                        switch (wtStatus.EventType)
                        {
                            case WtLoginEvent.Type.OK:

                                // Set online
                                var online = await SetClientOnineType(OnlineStatusEvent.Type.Online);
                                {
                                    _onlineType = online.EventType;

                                    // Bot online
                                    if (online.EventType == OnlineStatusEvent.Type.Online)
                                    {
                                        PostEventToEntity(online);
                                        return true;
                                    }
                                    else
                                    {
                                        SocketComponent.DisConnect("Wtlogin failed.");
                                        return false;
                                    }
                                }

                            case WtLoginEvent.Type.CheckSMS:
                            case WtLoginEvent.Type.CheckSlider:
                                PostEventToEntity(wtStatus);
                                wtStatus = await WtCheckUserOperation();
                                break;

                            case WtLoginEvent.Type.RefreshSMS:
                                wtStatus = await WtRefreshSMSCode();
                                break;

                            case WtLoginEvent.Type.CheckDevLock:
                            //wtStatus = await WtValidateDeviceLock();
                            //break;

                            case WtLoginEvent.Type.LoginDenied:
                            case WtLoginEvent.Type.InvalidSmsCode:
                            case WtLoginEvent.Type.InvalidLoginEnvironment:
                            case WtLoginEvent.Type.InvalidUinOrPassword:
                                PostEventToEntity(wtStatus);
                                SocketComponent.DisConnect("Wtlogin failed.");
                                return false;

                            default:
                            case WtLoginEvent.Type.NotImplemented:
                                SocketComponent.DisConnect("Wtlogin failed.");
                                LogW(TAG, "Login fail. Unsupported wtlogin event type received.");
                                return false;
                        }
                    }
                }

                LogW(TAG, "You're here? What the happened?");
                return false;
            }

            LogW(TAG, "Calling Login method again while online.");
            return false;
        }

        public Task<bool> Logout()
        {
            // <TODO>
            return Task.FromResult(false);
        }

        public void SubmitSMSCode(string code)
            => _userOperation.SetResult(new WtLoginEvent
            { EventType = WtLoginEvent.Type.CheckSMS, CaptchaResult = code });

        public void SubmitSliderTicket(string ticket)
            => _userOperation.SetResult(new WtLoginEvent
            { EventType = WtLoginEvent.Type.CheckSlider, CaptchaResult = ticket });

        internal async Task<WtLoginEvent> WtLogin()
            => (WtLoginEvent)await PostEvent<PacketComponent>
            (new WtLoginEvent { EventType = WtLoginEvent.Type.Tgtgt });

        internal async Task<WtLoginEvent> WtRefreshSMSCode()
            => (WtLoginEvent)await PostEvent<PacketComponent>
            (new WtLoginEvent { EventType = WtLoginEvent.Type.RefreshSMS });

        internal async Task<WtLoginEvent> WtValidateDeviceLock()
            => (WtLoginEvent)await PostEvent<PacketComponent>
            (new WtLoginEvent { EventType = WtLoginEvent.Type.CheckDevLock });

        internal async Task<WtLoginEvent> WtCheckUserOperation()
            => (WtLoginEvent)await PostEvent<PacketComponent>
            (await WaitForUserOperation());

        internal async Task<OnlineStatusEvent> SetClientOnineType(OnlineStatusEvent.Type onlineType)
            => (OnlineStatusEvent)await PostEvent<PacketComponent>
            (new OnlineStatusEvent { EventType = onlineType });

        public async Task<GroupKickMemberEvent> GroupKickMember(uint groupUin, uint memberUin, bool preventRequest)
            => (GroupKickMemberEvent)await PostEvent<PacketComponent>
                (new GroupKickMemberEvent
                {
                    GroupUin = groupUin,
                    MemberUin = memberUin,
                    ToggleType = preventRequest
                });

        public async Task<GroupMuteMemberEvent> GroupMuteMember(uint groupUin, uint memberUin, uint timeSeconds)
            => (GroupMuteMemberEvent)await PostEvent<PacketComponent>
                (new GroupMuteMemberEvent
                {
                    GroupUin = groupUin,
                    MemberUin = memberUin,
                    TimeSeconds = timeSeconds
                });

        public async Task<GroupPromoteAdminEvent> GroupPromoteAdmin(uint groupUin, uint memberUin, bool toggleAdmin)
            => (GroupPromoteAdminEvent)await PostEvent<PacketComponent>
                (new GroupPromoteAdminEvent
                {
                    GroupUin = groupUin,
                    MemberUin = memberUin,
                    ToggleType = toggleAdmin
                });

        internal async void ConfirmReadGroupMessage(GroupMessageEvent groupMessage)
            => await PostEvent<PacketComponent>
                (new GroupMessageReadEvent
                {
                    GroupUin = groupMessage.GroupUin,
                    RequestId = groupMessage.MessageId,
                    SessionSequence = groupMessage.SessionSequence,
                });

        internal async void PrivateMessagePulldown()
            => await PostEvent<PacketComponent>(new PrivateMessagePullEvent
            {
                SyncCookie = GetComponent<ConfigComponent>().KeyStore.Account.SyncCookie
            });

        internal void ConfirmPrivateMessage(PrivateMessageEvent privateMessage)
            => GetComponent<ConfigComponent>().SyncCookie(privateMessage.SyncCookie);

        private async Task<WtLoginEvent> WaitForUserOperation()
        {
            _userOperation = new TaskCompletionSource<WtLoginEvent>();
            return await _userOperation.Task;
        }

        public async Task<GroupMessageEvent> SendGroupMessage(uint groupUin, MessageChain message)
          => (GroupMessageEvent)await PostEvent<PacketComponent>
            (new GroupMessageEvent
            {
                GroupUin = groupUin,
                Message = message
            });

        public async Task<PrivateMessageEvent> SendPrivateMessage(uint friendUin, MessageChain message)
            => (PrivateMessageEvent)await PostEvent<PacketComponent>
            (new PrivateMessageEvent
            {
                FriendUin = friendUin,
                Message = message
            });

        public OnlineStatusEvent.Type GetOnlineStatus()
            => _onlineType;

        public Task<bool> SetOnlineStatus(OnlineStatusEvent.Type status)
        {
            if (_onlineType == status)
            {
                return Task.FromResult(true);
            }

            switch (_onlineType)
            {
                // Not supported yet
                case OnlineStatusEvent.Type.Online:
                case OnlineStatusEvent.Type.Leave:
                case OnlineStatusEvent.Type.Busy:
                case OnlineStatusEvent.Type.Hidden:
                case OnlineStatusEvent.Type.QMe:
                case OnlineStatusEvent.Type.DoNotDistrub:
                    return Task.FromResult(false);

                // Login
                case OnlineStatusEvent.Type.Offline:
                    return Login();
            }

            return Task.FromResult(false);
        }

        internal override void EventHandler(KonataTask task)
        {
            switch (task.EventPayload)
            {
                // Receive online status from server
                case OnlineStatusEvent onlineStatusEvent:
                    _onlineType = onlineStatusEvent.EventType;
                    break;

                // Confirm with server about we have read group message
                case GroupMessageEvent groupMessageEvent:
                    ConfirmReadGroupMessage(groupMessageEvent);
                    goto default;

                // Pull the private message when notified
                case PrivateMessageNotifyEvent _:
                    PrivateMessagePulldown();
                    break;

                // Confirm with server about we have read private message
                case PrivateMessageEvent privateMessageEvent:
                    ConfirmPrivateMessage(privateMessageEvent);
                    goto default;

                // Pass messages to upstream
                default:
                    PostEventToEntity(task.EventPayload);
                    break;
            }
        }
    }
}
