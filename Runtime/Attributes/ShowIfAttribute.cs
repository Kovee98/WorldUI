using UnityEngine;
using System;

namespace WorldUI {
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class ShowIfAttribute : PropertyAttribute {
        public string memberName;
        public object expectedValue;

        public ShowIfAttribute (string name, bool equals = true) {
            memberName = name;
            expectedValue = equals;
        }

        public ShowIfAttribute (string name, object equals) {
            memberName = name;
            expectedValue = equals;
        }
    }
}
