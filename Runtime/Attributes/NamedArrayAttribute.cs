using System;
using UnityEngine;

namespace UniKit.Attributes
{
    public class NamedArrayAttribute : PropertyAttribute
    {
        private readonly string _elementName;
        public string ElementName => _elementName;

        public NamedArrayAttribute(string elementName = "Name")
        {
            _elementName = elementName ?? throw new ArgumentNullException(nameof(elementName));
        }
    }
}