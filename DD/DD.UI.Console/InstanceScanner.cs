﻿using StructureMap;

namespace DD.UI.Console;

public class InstanceScanner : Registry
{
    public InstanceScanner()
    {
        Scan(_ =>
        {
            //_.TheCallingAssembly(); // No the implementation of the interface is not in this assembly
            _.AssembliesAndExecutablesFromApplicationBaseDirectory(); // The implementation of the interface is in another assembly in the base directory
            _.WithDefaultConventions(); // Default Convention is that the interface has the same name as the implementation except for the I prefix
        });
    }
}