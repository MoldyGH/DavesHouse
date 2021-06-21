using System.Collections.Generic;
using UnityEngine;

public enum BSPSplitDirection
{
    HORIZONTAL,
    VERTICAL,
    MAX
}

public class BSPTree
{
    public BSPTreeNode root;
}

public class BSPTreeNodeBounds
{
    public BSPTreeNodeBounds(int min_x, int min_y, int max_x, int max_y)
    {
        this.min_x = min_x;
        this.min_y = min_y;
        this.max_x = max_x;
        this.max_y = max_y;
    }

    public int min_x;
    public int min_y;
    public int max_x;
    public int max_y;

    public bool Contain(CellLocation l)
    {
        int x = l.x / 2;
        int y = l.y / 2;

        //Debug.Log(x + ", " + y);
        //Debug.Log(this.ToString());

        return min_x <= x && x < max_x && min_y <= y && y < max_y;
    }

    override public string ToString()
    {
        return "X[" + min_x + "," + max_x + "] Y[" + min_y + "," + max_y +"]";
    }
}

public class BSPTreeNode
{
    public BSPTreeNode(int min_x, int min_y, int max_x, int max_y)
    {
        this.areaBounds = new BSPTreeNodeBounds(min_x,min_y,max_x,max_y);
    }

    public BSPTreeNodeBounds areaBounds;
    public BSPTreeNodeBounds roomBounds;
    public BSPSplitDirection splitDirection;

    public BSPTreeNode left;
    public BSPTreeNode right;
}

public class BSPVirtualMapGenerator : VirtualMapGenerator{

    // Parameters
    public int nSplits = 3;
    public float splitRange = 0.3f; // [0,1)  0 splits always in half, going to 1 increases the relative size of the split areas
    public float roomSizeMinRange = 0.4f;  // [0,1] Controls what actual MIN range to use for room generation
    public float roomSizeMaxRange = 0.8f;  // [0,1] Controls what actual MAX range to use for room generation
    public int maxCorridorWidth = 2;   // [1,2]

   // bool ONE_DOOR_PER_CORRIDOR = true;

    override protected void GenerateWithAlgorithm(VirtualMap map, VirtualMap vmapBelow)
    {
        // Start full of rocks
        map.ResetToRocks();
        //return;

        // Pick the cell to start from
        CellLocation starting_location = default(CellLocation);
        if (vmapBelow != null) starting_location = vmapBelow.end;
        else starting_location = CellLocation.INVALID;
        //Debug.Log(starting_location);

        // We start our tree with the full dungeon bounds (only counting FLOOR cells)
        BSPTree tree = new BSPTree();
        tree.root = new BSPTreeNode(0, 0, map.ActualWidth, map.ActualHeight);

        // Pick a random initial direction
        BSPSplitDirection splitDir = (BSPSplitDirection)DungeonGenerator.Random.Instance.Next(0, (int)BSPSplitDirection.MAX - 1);

        // Start the algorithm
        List<BSPTreeNode> leafNodes = new List<BSPTreeNode>();
        SplitNode(tree.root, splitDir, splitRange, nSplits, leafNodes);

        // Create the rooms
        RoomGenerator roomGenerator = new RoomGenerator();
        map.rooms = new List<VirtualRoom>();
        foreach (BSPTreeNode node in leafNodes)
        {
            VirtualRoom room = CreateRoom(map, roomGenerator, node, starting_location);
            if (room != null)
            {
                room.sequentialId = map.rooms.Count;
                map.rooms.Add(room);
            }
        }

        // Create the corridors
        LinkCorridors(tree.root, map, 0);

    }

    void SplitNode(BSPTreeNode node, BSPSplitDirection splitDir, float splitRange, int recursionLevel, List<BSPTreeNode> leafNodes)
    {
        // Split a node so that we get a new range [min_x,min_y,max_x,max_y] so that a room can be created with these bounds
        // Note that these are bounds on the indices of the virtual map and that they are left-inclusive!
        //Debug.Log("LEVEL " + recursionLevel + " [" + node.areaBounds.min_x + "," + node.areaBounds.max_x + "] [" + node.areaBounds.min_y + "," + node.areaBounds.max_y + "]");
        if (recursionLevel == 0) {
            leafNodes.Add(node);
            return;
        }
        int splitPoint;

        // Pick a random coordinate on that direction
        splitPoint = GetSplitPoint(node.areaBounds, splitDir, splitRange);
        node.splitDirection = splitDir;
        //Debug.Log("ON " + splitDir + " CHOSEN SPLIT: " + splitPoint);

        // Create child nodes from that split point
        switch (splitDir)
        {
            case BSPSplitDirection.VERTICAL:
                node.left = new BSPTreeNode(node.areaBounds.min_x, node.areaBounds.min_y, splitPoint, node.areaBounds.max_y);
                node.right = new BSPTreeNode(splitPoint, node.areaBounds.min_y, node.areaBounds.max_x, node.areaBounds.max_y);
                break;
            case BSPSplitDirection.HORIZONTAL:
                node.left = new BSPTreeNode(node.areaBounds.min_x, node.areaBounds.min_y, node.areaBounds.max_x, splitPoint);
                node.right = new BSPTreeNode(node.areaBounds.min_x, splitPoint, node.areaBounds.max_x, node.areaBounds.max_y);
                break;
        }

        // Next pass: go through child nodes
        recursionLevel--;
        splitDir = (BSPSplitDirection)Mathf.Repeat((int)(splitDir+1), (int)BSPSplitDirection.MAX);
        SplitNode(node.left, splitDir, splitRange, recursionLevel, leafNodes);
        SplitNode(node.right, splitDir, splitRange, recursionLevel, leafNodes);
    }



    private int GetSplitPoint(BSPTreeNodeBounds bounds, BSPSplitDirection splitDir, float splitRange)
    {
        int max = 0;
        int min = 0;
        float delta_high = 0.5f + splitRange / 2;
        float delta_low = 0.5f - splitRange / 2;
        int range;
        switch (splitDir)
        {
            case BSPSplitDirection.VERTICAL:
                range = bounds.max_x - bounds.min_x;
                min = bounds.min_x + (int)(range * delta_low);
                max = bounds.min_x + (int)(range * delta_high);
                break;
            case BSPSplitDirection.HORIZONTAL:
                range = bounds.max_y - bounds.min_y;
                min = bounds.min_y + (int)(range * delta_low);
                max = bounds.min_y + (int)(range * delta_high);
                break;
        }
        if (min == 0) min = 1; // Cannot be too small!

        return DungeonGenerator.Random.Instance.Next(min, max);
    }



    VirtualRoom CreateRoom(VirtualMap map, RoomGenerator roomGenerator, BSPTreeNode node, CellLocation starting_location)
    {
        // Get the maximum bounds
        BSPTreeNodeBounds bounds = node.areaBounds;
        int range_x = bounds.max_x - bounds.min_x;
        int range_y = bounds.max_y - bounds.min_y;

        // Cannot create a room if the area is empty!
        if (range_x == 0 || range_y == 0)
        {
            Debug.LogWarning("Room size too small to be created! " + range_x + "," + range_y);
            return null;
        }

        // Choose a random size for the room
        int min_size_x = Mathf.Max(1, Mathf.FloorToInt(range_x * roomSizeMinRange));
        int max_size_x = Mathf.Max(1, Mathf.CeilToInt(range_x * roomSizeMaxRange));
        int min_size_y = Mathf.Max(1, Mathf.FloorToInt(range_y * roomSizeMinRange));
        int max_size_y = Mathf.Max(1, Mathf.CeilToInt(range_y * roomSizeMaxRange));
         
        // Compute size
        int size_x = DungeonGenerator.Random.Instance.Next(min_size_x, max_size_x);
        int size_y = DungeonGenerator.Random.Instance.Next(min_size_y, max_size_y);
        
        // Compute start
        int start_x, start_y;
        start_x = bounds.min_x + DungeonGenerator.Random.Instance.Next(range_x - size_x);
        start_y = bounds.min_y + DungeonGenerator.Random.Instance.Next(range_y - size_y);

        // If the starting location is inside these bounds, we must force it to create the room on it
        if (starting_location.isValid() && bounds.Contain(starting_location))
        {
            //Debug.Log("YES IN");
            int x = starting_location.x/2;
            int y = starting_location.y/2;

            //Debug.Log("Start bounds: " + (new BSPTreeNodeBounds(start_x, start_y, start_x + size_x, start_y + size_y)).ToString());

            // Make sure the start includes this, or decrease it
            if (x < start_x) start_x = x;   
            if (y < start_y) start_y = y;

            //Debug.Log(y);
            //Debug.Log(start_y + size_y);

            // Make sure the end includes thid, or increase it
            if (x + 1 >= start_x + size_x) size_x = x + 1 - start_x;
            if (y + 1 >= start_y + size_y) size_y = y + 1 - start_y;

            //Debug.Log("End bounds: " + (new BSPTreeNodeBounds(start_x, start_y, start_x + size_x, start_y + size_y)).ToString());
        }

        //Debug.Log("MIN " + min_size_x + " MAX " + max_size_x + " SIZE " + size_x);

        //Debug.Log("SPACE " + " [" + node.areaBounds.min_x + "," + node.areaBounds.max_x + "] [" + node.areaBounds.min_y + "," + node.areaBounds.max_y + "]");

        //Debug.Log("delta_low " + delta_low + " delta_high " + delta_high);

        //Debug.Log("CREATING ROOM: " + start_x + " + " + size_x + "   ||   " + start_y + " + " + size_y);

        node.roomBounds = new BSPTreeNodeBounds(start_x, start_y, start_x + size_x, start_y + size_y);

        // Start and end must be converted to whole-map sizes (not just floors)
        start_x = start_x * 2 + 1;
        start_y = start_y * 2 + 1;

        return roomGenerator.CreateRoom(map, size_x, size_y, start_x, start_y);
    }

    void LinkCorridors(BSPTreeNode node, VirtualMap map, int recursionLevel, bool isLeft = false)
    {
        //Debug.Log("Linking at level " + recursionLevel);
        // We link corridors recursively
        if (node.left != null && node.right != null)
        {
            // First we recurse to the children
            LinkCorridors(node.left, map, recursionLevel+1, isLeft:true);
            LinkCorridors(node.right, map, recursionLevel + 1, isLeft: false);

            // We get the left, we get the right, and we link them
            // We do this for non-leaf nodes
            if(node.left.roomBounds != null && node.right.roomBounds != null)
            {
                // DEBUG: Only connect at higher recursion level
                if (recursionLevel >= 0)    
                { 
                    if (node.splitDirection == BSPSplitDirection.HORIZONTAL)
                    {
                        CreateVerticalCorridor(map, node.left.roomBounds, node.right.roomBounds);
                    }
                    else
                    {
                        CreateHorizontalCorridor(map, node.left.roomBounds, node.right.roomBounds);
                    }
                }

                //if (isLeft)
                //return;
                //return;
           
                // Now that we linked the children, we update the bounds of this node, which had no roomBounds before
                BSPTreeNodeBounds lb = node.left.roomBounds;
                BSPTreeNodeBounds rb = node.right.roomBounds;

                // We always get the more central one, to make sure we can connect easily
                // Central one: has midpoint closest to the middle of the map
                int mid_x_map = map.ActualWidth / 2;
                int mid_y_map = map.ActualHeight / 2;
                int mid_x_lb = (lb.min_x+lb.max_x)/2; 
                int mid_y_lb = (lb.min_y+lb.max_y)/2;
                int mid_x_rb = (rb.min_x + rb.max_x) / 2;
                int mid_y_rb = (rb.min_y + rb.max_y) / 2;

                //Debug.Log("MID_MAP " + mid_x_map);
                //Debug.Log("mid_x_lb " + mid_x_lb);
                //Debug.Log("mid_x_rb " + mid_x_rb);

                // Check the more central one
                int dist_lb = Mathf.Abs(mid_x_lb - mid_x_map) + Mathf.Abs(mid_y_lb - mid_y_map);
                int dist_rb = Mathf.Abs(mid_x_rb - mid_x_map) + Mathf.Abs(mid_y_rb - mid_y_map);

                //Debug.Log("RECURSION " + recursionLevel);
                //Debug.Log("THIS: " + node.areaBounds);// + "    CONNECTED INSIDE: " + node.roomBounds);
                //Debug.Log("Distance of L is " + dist_lb + " L: " + lb);
                //Debug.Log("Distance of R is " + dist_rb + " R: " + rb);
                //Debug.Log("CLOSEST L? " + (dist_lb <= dist_rb));
                if (dist_lb <= dist_rb)
                {
                    node.roomBounds =  new BSPTreeNodeBounds(lb.min_x, lb.min_y, lb.max_x, lb.max_y);
                }
                else
                {
                    node.roomBounds = new BSPTreeNodeBounds(rb.min_x, rb.min_y, rb.max_x, rb.max_y);
                }

                // We use all the range (WRONG! may not connect!)
                /*
                node.roomBounds = new BSPTreeNodeBounds(Mathf.Min(lb.min_x, rb.min_x),
                                        Mathf.Min(lb.min_y, rb.min_y),
                                        Mathf.Max(lb.max_x, rb.max_x),
                                        Mathf.Max(lb.max_y, rb.max_y)
                                        );*/
                // We connect left or right, depending on where we are (WRONG)
                /*
                if (!isLeft)
                {
                    node.roomBounds =  new BSPTreeNodeBounds(lb.min_x, lb.min_y, lb.max_x, lb.max_y);
                }
                else
                {
                    node.roomBounds = new BSPTreeNodeBounds(rb.min_x, rb.min_y, rb.max_x, rb.max_y);
                }*/
                //node.roomBounds = new BSPTreeNodeBounds(lb.min_x, lb.min_y, lb.max_x, lb.max_y);

                
            }


        }

    }

    private void CreateHorizontalCorridor(VirtualMap map, BSPTreeNodeBounds roomBoundsLeft, BSPTreeNodeBounds roomBoundsRight)
    {
        BSPTreeNodeBounds higherRoomBounds = null, lowerRoomBounds = null;
        int v_start = 0;
        bool createLShape = false;
        int higher_min_v = Mathf.Max(roomBoundsLeft.min_y, roomBoundsRight.min_y);
        int lower_max_v = Mathf.Min(roomBoundsLeft.max_y, roomBoundsRight.max_y);
        int overlapping_v_range = lower_max_v - higher_min_v;

        if (overlapping_v_range > 0)
        {
            // They overlap: we choose randomly the start where the sides overlap
            v_start = DungeonGenerator.Random.Instance.Next(
                higher_min_v,
                lower_max_v - 1);
        }
        else
        {
            // They do not overlap! We'll need a L-shaped corridor!
            // We do so starting from one room, then going OVER the other, and then going down
            //Debug.Log("L");
            createLShape = true;

            // We now need to check which is the higher one
            if (roomBoundsLeft.max_y > roomBoundsRight.max_y)
            {
                // L higher than R
                higherRoomBounds = roomBoundsLeft;
                lowerRoomBounds = roomBoundsRight;
            }
            else
            {
                // R higher than L
                higherRoomBounds = roomBoundsRight;
                lowerRoomBounds = roomBoundsLeft;
            }

            // The y_start comes from the higher one 
            v_start = DungeonGenerator.Random.Instance.Next(
                higherRoomBounds.min_y,
                higherRoomBounds.max_y-1);
        }

        // We create a corridor from west to east
        int corridorWidth = 1;
        if (!createLShape)
        {
            corridorWidth = Mathf.Min(overlapping_v_range, maxCorridorWidth);
            if (corridorWidth > 1 && overlapping_v_range > 0)
            {
                //Debug.Log("Start: " + v_start);
                int overflow = higher_min_v - (v_start - (corridorWidth - 1)); //(v_start - (corridorWidth - 1)) +  (higher_min_v - 1);
                v_start += overflow;
                //Debug.Log("Overflow: " + overflow);
                //Debug.Log("End: " + v_start);
            }
        }

        for (int x = roomBoundsLeft.max_x - 1; x < roomBoundsRight.min_x; x++)
        {
            CellLocation l = new CellLocation(x * 2 + 1, v_start * 2 + 1);
            //Debug.Log("EAST: " + l);

            // If already entering a corridor, we stop here!
            //CellLocation target_l = map.GetNeighbourCellLocationOfSameType(l,VirtualMap.DirectionType.East);
            //if (map.GetCell(target_l).Type == VirtualCell.CellType.CorridorFloor) return;
            //CellLocation target_l = map.GetNeighbourCellLocationOfSameType(l, VirtualMap.DirectionType.East);
            //bool makeDoor = !(map.GetCell(target_l).Type == VirtualCell.CellType.CorridorFloor);

            map.CreateCorridor(l, VirtualMap.DirectionType.East, corridorWidth, makeDoor: true);

        }

        if (createLShape)
        {
            // We also create the S-N corridor
            for (int y = lowerRoomBounds.max_y - 1; y < v_start; y++)
            {
                int x_start = 0;
                if (lowerRoomBounds == roomBoundsRight) x_start = roomBoundsRight.min_x;
                else x_start = roomBoundsLeft.max_x - 1;
                CellLocation l = new CellLocation(x_start * 2 + 1, y * 2 + 1);

                // If already entering a corridor, we stop here!
                //CellLocation target_l = map.GetNeighbourCellLocationOfSameType(l, VirtualMap.DirectionType.East);
                //if (map.GetCell(target_l).Type == VirtualCell.CellType.CorridorFloor) return;
                // If already entering a corridor, we stop here!
                //CellLocation target_l = map.GetNeighbourCellLocationOfSameType(l, VirtualMap.DirectionType.North);
                //bool makeDoor = !(map.GetCell(target_l).Type == VirtualCell.CellType.CorridorFloor);

                map.CreateCorridor(l, VirtualMap.DirectionType.North, makeDoor: true);
            }
        }

    }

    private void CreateVerticalCorridor(VirtualMap map, BSPTreeNodeBounds roomBoundsBottom, BSPTreeNodeBounds roomBoundsTop)
    {
        BSPTreeNodeBounds higherRoomBounds = null, lowerRoomBounds = null;
        int v_start = 0;
        bool createLShape = false;
        int higher_min_v = Mathf.Max(roomBoundsBottom.min_x, roomBoundsTop.min_x);
        int lower_max_v = Mathf.Min(roomBoundsBottom.max_x, roomBoundsTop.max_x);
        int overlapping_v_range = lower_max_v - higher_min_v;

        if (overlapping_v_range > 0)
        {
            // They overlap: we choose randomly the start where the sides overlap
            v_start = DungeonGenerator.Random.Instance.Next(higher_min_v, lower_max_v - 1);
        }
        else
        {
            // They do not overlap! We'll need a L-shaped corridor!
            // We do so starting from one room, then going OVER the other, and then going down
            //Debug.Log("L");
            createLShape = true;

            // We now need to check which is the higher one
            if (roomBoundsBottom.max_x > roomBoundsTop.max_x)
            {
                // B higher than T
                higherRoomBounds = roomBoundsBottom;
                lowerRoomBounds = roomBoundsTop;
            }
            else
            {
                // T higher than B
                higherRoomBounds = roomBoundsTop;
                lowerRoomBounds = roomBoundsBottom;
            }

            // The v_start comes from the higher one 
            v_start = DungeonGenerator.Random.Instance.Next(
                higherRoomBounds.min_x,
                higherRoomBounds.max_x - 1);
        }


        // We create a corridor from south to north
        int corridorWidth = 1;
        if (!createLShape)
        {
            corridorWidth = Mathf.Min(overlapping_v_range, maxCorridorWidth);
            if (corridorWidth > 1 && overlapping_v_range > 0)
            {
                int overflow = v_start + (corridorWidth - 1) - (lower_max_v - 1);
                v_start -= overflow;
                //Debug.Log(overflow);
            }
        }

        for (int y = roomBoundsBottom.max_y - 1; y < roomBoundsTop.min_y; y++)
        {
            CellLocation l = new CellLocation(v_start * 2 + 1, y * 2 + 1);

            // If already entering a corridor, we stop here!
            /*CellLocation target_l =  map.GetNeighbourCellLocationOfSameType(l, VirtualMap.DirectionType.North);
            bool makeDoor = !(map.GetCell(target_l).Type == VirtualCell.CellType.CorridorFloor);
            makeDoor = true;*/

            map.CreateCorridor(l, VirtualMap.DirectionType.North, corridorWidth, makeDoor: true);
        }

        if (createLShape)
        {
            // We also create the W-E corridor
            for (int x = lowerRoomBounds.max_x - 1; x < v_start; x++)
            {
                int y_start = 0;
                if (lowerRoomBounds == roomBoundsTop) y_start = roomBoundsTop.min_y;
                else y_start = roomBoundsBottom.max_y - 1;
                CellLocation l = new CellLocation(x * 2 + 1, y_start * 2 + 1);

                // If already entering a corridor, we stop here!
                //CellLocation target_l = map.GetNeighbourCellLocationOfSameType(l, VirtualMap.DirectionType.East);
                //bool makeDoor = !(map.GetCell(target_l).Type == VirtualCell.CellType.CorridorFloor);
                //CellLocation target_l = map.GetNeighbourCellLocationOfSameType(l, VirtualMap.DirectionType.East);
                //if (map.GetCell(target_l).Type == VirtualCell.CellType.CorridorFloor) return;

                map.CreateCorridor(l, VirtualMap.DirectionType.East, makeDoor: true);
            }
        }
    }


}
