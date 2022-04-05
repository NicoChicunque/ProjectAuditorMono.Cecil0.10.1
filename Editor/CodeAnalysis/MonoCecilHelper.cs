using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace Unity.ProjectAuditor.Editor.CodeAnalysis
{
    internal static class MonoCecilHelper
    {
        // Some sequence points do not map to any source document line. Such sequence points have a line number value equal to this constant (0xfeefee)
        public const int HiddenLine = 16707566;

        // for reference https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/MonoCecil/MonoCecilHelper.cs
        public static IEnumerable<TypeDefinition> AggregateAllTypeDefinitions(IEnumerable<TypeDefinition> types)
        {
            var typeDefs = types.ToList();
            foreach (var typeDefinition in types)
                if (typeDefinition.HasNestedTypes)
                    typeDefs.AddRange(AggregateAllTypeDefinitions(typeDefinition.NestedTypes));
            return typeDefs;
        }
    }
}
