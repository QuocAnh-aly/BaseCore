using System;

namespace BaseCore.Common
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }

        public DateTime CreatedDateTime { get; set; } = DateTime.Now;

        public string CreatedUser { get; set; }
    }
}