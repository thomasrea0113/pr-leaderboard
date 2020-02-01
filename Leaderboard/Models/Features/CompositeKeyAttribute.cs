using System;

namespace Leaderboard.Models.Features
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class CompositeKeyAttribute : Attribute
    {

    }
}