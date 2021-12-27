using System;

namespace Ludaludaed.KECS.Unity {
    [AttributeUsage(AttributeTargets.Struct)]
    public class AutoRegistrationComponentAttribute : Attribute {
        public AutoRegistrationComponentAttribute() { }
    }
}