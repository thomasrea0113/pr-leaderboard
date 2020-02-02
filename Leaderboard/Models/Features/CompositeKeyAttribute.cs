using System;

namespace Leaderboard.Models.Features
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CompositeKeyAttribute : Attribute
    {

    }
}