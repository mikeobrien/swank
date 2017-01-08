using System;
using System.Collections.Generic;

namespace Swank.Specification
{
    public class OrphanedModuleActionException : Exception
    {
        public OrphanedModuleActionException(IEnumerable<string> actions) : base(
            "The following actions are not associated with a module. " +
            "Either assocate them with a module or turn off orphaned " +
            $"action exceptions. {string.Join(", ", actions)}") { }
    }

    public class OrphanedResourceActionException : Exception
    {
        public OrphanedResourceActionException(IEnumerable<string> actions) : base(
            "The following actions are not associated with a resource. " +
            "Either assocate them with a resource or turn off orphaned " +
            $"action exceptions. {string.Join(", ", actions)}") { }
    }
}