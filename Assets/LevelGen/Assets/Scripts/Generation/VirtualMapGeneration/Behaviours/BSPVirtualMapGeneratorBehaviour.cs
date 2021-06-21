
public class BSPVirtualMapGeneratorBehaviour : VirtualMapGeneratorBehaviour<BSPVirtualMapGenerator>
{
    // Parameters (copied from the VirtualMapGenerator)
    public int nSplits = 3;
    public float splitRange = 0.3f; // [0,1]  0 splits always in half, going to 1 increases the relative size of the split areas
    public float roomSizeMinRange = 0.4f;  // [0,1] Controls what actual MIN range to use for room generation
    public float roomSizeMaxRange = 0.8f;  // [0,1] Controls what actual MAX range to use for room generation
    public int maxCorridorWidth = 2;   // [1,2]

    protected override void InitialiseInstance()
    {
        if (virtualMapGenerator == null)
        {
            virtualMapGenerator = new BSPVirtualMapGenerator();
        }

        // TODO: find a way to set these automatically!
        virtualMapGenerator.nSplits = nSplits;
        virtualMapGenerator.splitRange = splitRange;
        virtualMapGenerator.roomSizeMinRange = roomSizeMinRange;
        virtualMapGenerator.roomSizeMaxRange = roomSizeMaxRange;
        virtualMapGenerator.maxCorridorWidth = maxCorridorWidth;

        base.InitialiseInstance();
    }

    override public bool HasRooms
    {
        get { return true; }
    }
}