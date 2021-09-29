﻿using System;

namespace EventBus.Messages.Events
{
    public class IntegrationsBaseEvent
    {
        public IntegrationsBaseEvent()
        {
            Id = new Guid();
            CreateDate = DateTime.UtcNow;
        }

        public IntegrationsBaseEvent(Guid id, DateTime createDate)
        {
            Id = id;
            CreateDate = createDate;
        }

        public Guid Id { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
