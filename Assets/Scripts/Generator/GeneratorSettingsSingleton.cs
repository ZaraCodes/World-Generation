using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorSettingsSingleton
{
    /// <summary>Private reference to the singleton</summary>
    private static GeneratorSettingsSingleton instance;

    /// <summary>Property that makes sure the instance stays the same</summary>
    public static GeneratorSettingsSingleton Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GeneratorSettingsSingleton();
                instance.useCustomSeed = false;
            }
            return instance;
        }
    }

    /// <summary>the world seed</summary>
    public int seed;

    /// <summary>sets if the generator should use a custom seed</summary>
    public bool useCustomSeed;

    /// <summary>The Generator settings for the world</summary>
    public GeneratorSettings GeneratorSettings;

    /// <summary>Preset index from the settings menu</summary>
    public int SelectedPresetIdx = 0;
}
