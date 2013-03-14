using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FocalPoint.SDK;
using Microsoft.Lync.Model;

namespace FocalPoint.Lync2013Plugin
{
    public class LyncStatusUpdater : ISessionWatcher
    {
        private int _minutesRemaining = Int32.MinValue;

        private LyncClient _client = null;
        private LyncClient Client
        {
            get
            {
                if (_client == null)
                {
                    try
                    {
                        _client = LyncClient.GetClient();
                    }
                    catch (Exception)
                    {
                        throw new PluginException("Failed to connect to Lync. Please ensure your Lync app is running and signed in.");
                    }
                }
                return _client;
            }
        }

        public string Name { get { return "Lync 2013 Plugin"; } }

        public void Start(ISession session)
        {
            SetAwayMessage(session.EndTime);
        }

        public void Update(ISession session)
        {
            SetAwayMessage(session.EndTime);
        }

        public void Stop()
        {
            _minutesRemaining = Int32.MinValue;
            PublishPersonalNoteAvailability("", ContactAvailability.Free);
        }

        private void SetAwayMessage(DateTime utcEndTime)
        {
            var remaining = (int) Math.Ceiling((utcEndTime - DateTime.UtcNow).TotalMinutes);

            if (remaining != _minutesRemaining)
            {
                _minutesRemaining = remaining;

                PublishPersonalNoteAvailability(
                     String.Format("Pomodoro: {0} minutes left", _minutesRemaining),
                     ContactAvailability.DoNotDisturb);
            }
        }

        private void SendPublishRequest(
           Dictionary<PublishableContactInformationType, object> publishData,
            string publishId)
        {
            var publishState = (object)publishId;
            object[] publishAsyncState = { Client.Self, publishState };
            try
            {
                Client.Self.BeginPublishContactInformation(
                    publishData,
                    PublicationCallback,
                    publishAsyncState);
            }
            catch (COMException)
            {
                _client = null;

                throw new PluginException("Failed to update Lync status. Will try again later.");
            }
            catch (ArgumentException)
            {
                //MessageBox.Show("publish Argument exception: " + ae.Message);
            }
        }
        private void PublicationCallback(IAsyncResult ar)
        {
            if (ar.IsCompleted)
            {
                var asyncState = (object[])ar.AsyncState;
                ((Self)asyncState[0]).EndPublishContactInformation(ar);
            }
        }
        private void PublishPersonalNoteAvailability(string newNote, ContactAvailability availability)
        {
            //Each element of this array must contain a valid enumeration. If null array elements 
            //are passed, an ArgumentException is raised.
            var publishData = new Dictionary<PublishableContactInformationType, object>
                {
                    {PublishableContactInformationType.PersonalNote, newNote},
                    {PublishableContactInformationType.Availability, availability}
                };

            //Helper method is found in the next example.
            SendPublishRequest(publishData, "Personal Note and Availability");
        }
    }
}
