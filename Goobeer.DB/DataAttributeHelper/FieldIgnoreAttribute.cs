using System;

namespace Goobeer.DB.DataAttributeHelper
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FieldIgnoreAttribute : Attribute
    {
    }
}
