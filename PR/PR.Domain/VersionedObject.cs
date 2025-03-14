﻿using System;

namespace PR.Domain
{
    public abstract class VersionedObject
    {
        public Guid ObjectId { get; set; }
        public DateTime Created { get; set; }
        public DateTime Superseded { get; set; }
    }
}
