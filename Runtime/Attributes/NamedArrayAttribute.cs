using UnityEngine;
using System;

namespace Phoder1.Core.Attributes
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