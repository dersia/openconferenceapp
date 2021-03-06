﻿using System;

using Microsoft.Azure.Mobile.Server;


namespace open.conference.app.server.backend.DataObjects
{
    public interface IBaseDataObject
    {
        string Id { get; set; }
    }
    public class BaseDataObject : EntityData
    {
        public BaseDataObject ()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string RemoteId { get; set; }
    }
}
