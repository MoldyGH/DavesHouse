using UnityEngine;
using System.Collections;

// Controls parameters and Unity bindings for virtual map generation
[RequireComponent(typeof(GeneratorBehaviour))]
public abstract class VirtualMapGeneratorBehaviour : MonoBehaviour
{
    public abstract void Initialise();
    public abstract uint InitialiseSeed(bool useSeed, int seed);
    public abstract VirtualMap[] GenerateAllMaps(int width, int height, int nStoreys);

    // Parameters (copied from the VirtualMapGenerator)
    public bool createStartAndEnd = false;
    public bool forceStartAndEndInRooms = false;
    public float minimumDistanceBetweenStartAndEnd = 0;
    public float maximumDistanceBetweenStartAndEnd = 100;

    virtual public void ForceCommonSenseOptions()
    {
    }

    public abstract bool HasRooms
    {
        get;
    }
} 

public abstract class VirtualMapGeneratorBehaviour<T> : VirtualMapGeneratorBehaviour 
    where T : VirtualMapGenerator//,new() <--- won't work correctly
{
    // Bindings
    protected T virtualMapGenerator;

    public override void Initialise()
    {
        InitialiseInstance();
    }

    protected virtual void InitialiseInstance()
    {
        //System.Console.WriteLine("ABSTR");
        // Instantiate here the generator (must be done by derived classes due to new() constraints, ugh)
    
        // TODO: find a way to set them automatically
        virtualMapGenerator.createStartAndEnd = createStartAndEnd;
        virtualMapGenerator.forceStartAndEndInRooms = forceStartAndEndInRooms;
        virtualMapGenerator.minimumDistanceBetweenStartAndEnd = minimumDistanceBetweenStartAndEnd;
        virtualMapGenerator.maximumDistanceBetweenStartAndEnd = maximumDistanceBetweenStartAndEnd;
    }

    
    public T GetGenerator()
    {
        return this.virtualMapGenerator;
    }

    public override void ForceCommonSenseOptions()
    {
        if (!createStartAndEnd) forceStartAndEndInRooms = false;
    }

    public override uint InitialiseSeed(bool useSeed, int seed)
    {
        return virtualMapGenerator.InitialiseSeed(useSeed,seed);
    }

    public override VirtualMap[] GenerateAllMaps(int width, int height, int nStoreys)
    {
        VirtualMap[] virtualMaps = virtualMapGenerator.GenerateAllMaps(width, height, nStoreys);
        return virtualMaps;
    }


}