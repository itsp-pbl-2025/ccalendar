using System;
using Domain.Enum;

namespace Domain.Entity
{
    public record HistoryContainer(HistoryType Type, string Data, CCDateTime UpdatedAt)
    {
        public HistoryType Type { get; } = Type;

        public string Data { get; } = Data;
        public CCDateTime UpdatedAt { get; } = UpdatedAt;
    }
}