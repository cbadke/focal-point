﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Lync.Model;

namespace lync2013_plugin
{
    public class StatusUpdater
    {
        private LyncClient _LyncClient = LyncClient.GetClient();
        public void StartSession()
        {
            PublishPersonalNoteAndFreeAvailability("I'm away!", ContactAvailability.DoNotDisturb);
        }
        private void SendPublishRequest(
           Dictionary<PublishableContactInformationType, object> publishData,
            string publishId)
        {
            var publishState = (object)publishId;
            object[] publishAsyncState = { _LyncClient.Self, publishState };
            try
            {
                _LyncClient.Self.BeginPublishContactInformation(
                    publishData,
                    PublicationCallback,
                    publishAsyncState);
            }
            catch (COMException ce)
            {
                //MessageBox.Show("publish COM exception: " + ce.ErrorCode.ToString());
            }
            catch (ArgumentException ae)
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
        private void PublishPersonalNoteAndFreeAvailability(string newNote, ContactAvailability availability)
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
