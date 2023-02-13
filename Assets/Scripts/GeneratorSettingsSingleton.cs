using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorSettingsSingleton
{
    private static GeneratorSettingsSingleton instance;

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

    public int seed;

    public bool useCustomSeed;

    public GeneratorSettings GeneratorSettings;
}
