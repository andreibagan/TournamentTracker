using System;

namespace TournamentTracker.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyAttribute : Attribute
    {
        public string PropertyName { get; private set; }

        public PropertyAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}
