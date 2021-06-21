
public class DiggingVirtualMapGeneratorBehaviour : VirtualMapGeneratorBehaviour<DiggingVirtualMapGenerator>
{

    // Parameters (copied from the DiggingVirtualMapGenerator)
    public DiggingMapGeneratorAlgorithmChoice algorithmChoice;
    public int directionChangeModifier = 0;
    public int sparsenessModifier = 0;
    public int openDeadEndModifier = 0;

    public bool createRooms = false; 
    public int minRooms = 1;
    public int maxRooms = 1;
    public int minRoomWidth = 2;
    public int maxRoomWidth = 2;
    public int minRoomHeight = 2;
    public int maxRoomHeight = 2;
    public bool forceRoomTransversal;
    public int doorsDensityModifier;


    protected override void InitialiseInstance()
    {
        // We make sure it exists
        if (virtualMapGenerator == null)
        {
            virtualMapGenerator = new DiggingVirtualMapGenerator();
        }

        // TODO: find a way to set them automatically
        virtualMapGenerator.algorithmChoice = algorithmChoice;
        virtualMapGenerator.directionChangeModifier = directionChangeModifier;
        virtualMapGenerator.sparsenessModifier = sparsenessModifier;
        virtualMapGenerator.openDeadEndModifier = openDeadEndModifier;
        virtualMapGenerator.createRooms = createRooms;
        virtualMapGenerator.minRooms = minRooms;
        virtualMapGenerator.maxRooms = maxRooms;
        virtualMapGenerator.minRoomWidth = minRoomWidth;
        virtualMapGenerator.maxRoomWidth = maxRoomWidth;
        virtualMapGenerator.minRoomHeight = minRoomHeight;
        virtualMapGenerator.maxRoomHeight = maxRoomHeight;
        virtualMapGenerator.forceRoomTransversal = forceRoomTransversal;
        virtualMapGenerator.doorsDensityModifier = doorsDensityModifier;

        base.InitialiseInstance();
    }

    override public void ForceCommonSenseOptions()
    {
        base.ForceCommonSenseOptions();
        if (maxRooms < minRooms) maxRooms = minRooms;
        if (maxRoomHeight < minRoomHeight) maxRoomHeight = minRoomHeight;
        if (maxRoomWidth < minRoomWidth) maxRoomWidth = minRoomWidth;
    }

    override public bool HasRooms
    {
        get { return createRooms; }
    }
}