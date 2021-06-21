using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


// A group of interconnected cell locations
/*[System.Serializable]
public class ConnectedArea
{
    [SerializeField]
    private List<CellLocation> locations;

    [SerializeField]
    private int index;

    public ConnectedArea(int index)
    {
        this.index = index;
        locations = new List<CellLocation>();
    }

    public void AddLocation(CellLocation l){
        locations.Add(l);
    }
}*/

// This determines the overall shape of the map
[System.Serializable]
public class VirtualMap 
{
    [SerializeField]
    private int width;	// The map Width
    [SerializeField]
    private int height;	// The map Height
    [SerializeField]
	private VirtualCellGrid cells;	// The cells in this map

    public List<VirtualRoom> rooms;
	public int storey_number;
	
	public enum DirectionType {North, East, South, West, None };	// Directions
	public int nDirections{get; private set;}
	public DirectionType[] directions;

	public readonly List<CellLocation> visitedCells = new List<CellLocation>();				// Visited cells
	public readonly List<CellLocation> visitedAndBlockedCells = new List<CellLocation>(); 	// Visited and blocked (in all 4 directions) cells
	public readonly List<CellLocation> floorCells = new List<CellLocation>();				// Walkable cells
	public readonly List<CellLocation> borderCells = new List<CellLocation>(); 				// Walls, limits and passage cells
    public readonly List<CellLocation> noneCells = new List<CellLocation>();				// Parts between walls (where columns may be placed)
    public List<CellLocation> roomCells = new List<CellLocation>();
	
	public CellLocation start	=		new CellLocation(-1,-1);
	public CellLocation end		=		new CellLocation(-1,-1);
	public CellLocation root	=		new CellLocation(-1,-1);	// Cell used for distance computations

    //public List<ConnectedArea> connectedAreas;

    public static bool ONE_DOOR_PER_CORRIDOR = true;    // For width > 1 corridors

    public struct Dimensions
    {
        public int left;
        public int right;
        public int bottom;
        public int top;
        public int width_x;
        public int width_y;
        public int storey; // Written by the specific virtual map it belongs to
    }
	

	// Constructor
	public VirtualMap (int width, int height, int storey_number = 0)
	{
		this.width = 2*width +1;
		this.height = 2*height +1;
		this.storey_number = storey_number;
		cells = new VirtualCellGrid(this.width, this.height);
		SetupDirections(new DirectionType[4]{DirectionType.North,DirectionType.East,DirectionType.South,DirectionType.West});
	}
	public int Width        // This considers all map cells (floors and walls)
	{
		get {return width;}
	}
	public int Height
	{
		get {return height;}
	}
	public int ActualWidth  // TODO: change ActualWidth to FloorWidth (it considers only the floors)
	{
		get {return (width-1)/2;}
	}
	public int ActualHeight
	{
		get {return (height-1)/2;}
	}
	
	private void SetupDirections(DirectionType[] _directions){
		this.directions = _directions;
		this.nDirections = this.directions.Length;
	}

	public DirectionType GetDirectionClockwise(DirectionType _dir, int delta){
        int max = nDirections;
        if (delta < 0) delta += max;
        int next = (int)_dir + delta;
        return this.directions[(int)Mathf.Repeat(next, max)];  
	}

	public DirectionType GetDirectionOpposite(DirectionType _dir){
		return this.GetDirectionClockwise(_dir, this.nDirections/2);
	}
	
	public void ResetToWalls()
	{	
		// Initialise the virtual map with interleaved cells floors and walls, with None cells between walls
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				CellLocation location = new CellLocation(i, j);
				SetCell(i,j,new VirtualCell(false,location));
				if(i%2==0)
				{
					if(j%2==0){
						GetCell(i,j).Type=VirtualCell.CellType.None;
						noneCells.Add(location);
					}
					else
					{
						GetCell(i,j).Type=VirtualCell.CellType.CorridorWall;
						borderCells.Add(location);
					}
				}
				else
				{
					if(j%2==0)
					{	
						GetCell(i,j).Type=VirtualCell.CellType.CorridorWall;
						borderCells.Add(location);
					}
					else
					{
						GetCell(i,j).Type=VirtualCell.CellType.CorridorFloor;
						floorCells.Add(location);
					}
				}
			}
		}
	}

    public void ResetToRocks(){
        // Initialise the virtual map with interleaved rock cells and empty passages (no walls), with None cells between empty passages
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                CellLocation location = new CellLocation(i, j);
                SetCell(i, j, new VirtualCell(false, location));
                if (i % 2 == 0)
                {
                    if (j % 2 == 0)
                    {
                        GetCell(i, j).Type = VirtualCell.CellType.None;
                        noneCells.Add(location);
                    }
                    else
                    {
                        GetCell(i, j).Type = VirtualCell.CellType.EmptyPassage;
                        borderCells.Add(location);
                    }
                }
                else
                {
                    if (j % 2 == 0)
                    {
                        GetCell(i, j).Type = VirtualCell.CellType.EmptyPassage;
                        borderCells.Add(location);
                    }
                    else
                    {
                        GetCell(i, j).Type = VirtualCell.CellType.Rock;
                        floorCells.Add(location);
                    }
                }
            }
        }
    }


	public CellLocation[] GetAllNeighbours(CellLocation location){
		CellLocation[] neighs = new CellLocation[4];
		neighs[0] = GetNeighbourCellLocation(location,DirectionType.North);
		neighs[1] = GetNeighbourCellLocation(location,DirectionType.East);
		neighs[2] = GetNeighbourCellLocation(location,DirectionType.South);
		neighs[3] = GetNeighbourCellLocation(location,DirectionType.West);
		return neighs;
	}
	
	
	public CellLocation[] GetAllSameNeighbours(CellLocation location){
		CellLocation[] neighs = new CellLocation[4];
		neighs[0] = GetNeighbourCellLocationOfSameType(location,DirectionType.North);
		neighs[1] = GetNeighbourCellLocationOfSameType(location,DirectionType.East);
		neighs[2] = GetNeighbourCellLocationOfSameType(location,DirectionType.South);
		neighs[3] = GetNeighbourCellLocationOfSameType(location,DirectionType.West);
		return neighs;
	}
	
	// Return the next virtual cell in the given direction (for floors, these are walls. for walls, these are floors)
	public CellLocation GetNeighbourCellLocation(CellLocation location, DirectionType direction){
		return GetNeighbourCellLocationAtStep(location,direction,1);
	}
	
	// Return the next virtual cell of the same type in the given direction (i.e. floor for floor, wall for wall)
	public CellLocation GetNeighbourCellLocationOfSameType(CellLocation location, DirectionType direction){
		return GetNeighbourCellLocationAtStep(location,direction,2);
	}

	public CellLocation GetNeighbourCellLocationAtStep(CellLocation location, DirectionType direction, int step){
		switch(direction)
		{
		case DirectionType.South:
			return new CellLocation(location.x, location.y - step);
		case DirectionType.West:
			return new CellLocation(location.x -step, location.y);
		case DirectionType.North:
			return new CellLocation(location.x, location.y + step);
		case DirectionType.East:
			return new CellLocation(location.x +step, location.y);
		default:
			throw new InvalidOperationException();
		}
	}

	
    public bool LocationIsOutsideBounds(CellLocation location)
    {
        return ((location.x < 0) || (location.x >= Width) || (location.y < 0) || (location.y >= Height));
    }
	
	public bool LocationIsInPerimeter(CellLocation location){
		return ((location.x == 0) || (location.x == Width-1) || (location.y == 0) || (location.y == Height-1));
	}

	// Mark this cell as visited TODO: This is used only by some generation algorithms, so it should not be here!
	public void FlagCellAsVisited(CellLocation location)
    {
        if (LocationIsOutsideBounds(location)) throw new ArgumentException("Location is outside of Map bounds", "starting_location");
        if (this.GetCell(location.x, location.y).visited) throw new ArgumentException("Location is already visited", "starting_location");

        this.GetCell(location.x, location.y).visited = true;
        visitedCells.Add(location);
    }
	//add a room cell to the map
	public void AddRoomCell(CellLocation l)
	{
		roomCells.Add(l);
        this.GetCell(l.x, l.y).Type = VirtualCell.CellType.RoomFloor;
	}
    public bool HasRooms()
    {
        return this.rooms != null && this.rooms.Count > 0; 
    }

    public VirtualRoom GetRoom(int index)
    {
        return this.rooms[index];
    }
	
	// Did we process all the cells in the map yet?
	public bool AllCellsVisited
    {
        get { return visitedCells.Count == ((Width -1)/2)*((Height-1)/2); }
    }
	
	// Pick a random cell in the map and mark it as Visited
	public CellLocation PickRandomUnvisitedLocation(){
		List<CellLocation> locations = new List<CellLocation>(floorCells);
		foreach (CellLocation l in visitedCells) locations.Remove(l);
		
		int index= DungeonGenerator.Random.Instance.Next(0,locations.Count-1);
		CellLocation location = locations[index];

		return location;
	}

	public void MarkAsVisited(CellLocation l){
		VirtualCell cell = GetCell(l);
		cell.visited = true;
		visitedCells.Add(l);
	}

	// Does this cell have any neighbour in a certain direction?
	public bool HasAdjacentCellInDirection (CellLocation location, DirectionType direction)
	{
		if (LocationIsOutsideBounds(location)) return false;
		CellLocation l = GetNeighbourCellLocationOfSameType(location, direction);
		return !LocationIsOutsideBounds(l);
	}
	// Is this cell's neighbour marked as Visited?
	public bool AdjacentCellInDirectionIsVisited(CellLocation location, DirectionType direction)
    {
        if (HasAdjacentCellInDirection(location, direction))
		{
	        switch(direction)
	        {
	            case DirectionType.South:
                    return this.GetCell(location.x, location.y - 2).visited;
	            case DirectionType.West:
                    return this.GetCell(location.x - 2, location.y).visited;
	            case DirectionType.North:
                    return this.GetCell(location.x, location.y + 2).visited;
	            case DirectionType.East:
                    return this.GetCell(location.x + 2, location.y).visited;
	            default:
	                throw new InvalidOperationException();
	        }
		}
		
		return false;
    }
	// Get a random visited cell
	public CellLocation GetRandomVisitedCell(CellLocation location)
    {
		List<CellLocation> tempCells = new List<CellLocation>(visitedCells);
		
		//tempCells.Remove(starting_location);
		
		// NOTE: when does visistedAndBlockedCells get populated???
		foreach (CellLocation l in visitedAndBlockedCells)
		{
			tempCells.Remove(l);
		}
	    foreach (CellLocation l in roomCells)
		{
			tempCells.Remove(l);
		}
		
        if (tempCells.Count == 0) return new CellLocation(-1,-1);
		
        int index = DungeonGenerator.Random.Instance.Next(0, tempCells.Count -1);

        return tempCells[index];   
    }
	
	// Create a corridor between one cell and another, digging the passage in-between.
    // This will make sure that the starting floor and the end floor become corridor floors, if they are ROCKS or NONEs
	public CellLocation CreateCorridor(CellLocation zero_starting_location, VirtualMap.DirectionType direction, int corridor_width = 1, bool makeDoor = false)
    {
        // NOTE: we do not drill room walls (unless we make doors)
        VirtualCell cell;
        CellLocation target_location = default(CellLocation);

        // CHECK whether there is enough corridor space (NOTE: this only works for corridor_width=2)
        while(LocationIsOutsideBounds(GetNeighbourCellLocationAtStep(zero_starting_location, GetDirectionClockwise(direction, 1), (2 * (corridor_width-1)))))
        {
            Debug.LogWarning("Decreasing corridor width to due to out of bounds problems!");
            corridor_width--;
        }

        for (int cw = 0; cw < corridor_width; cw++)
        {
            CellLocation starting_location = GetNeighbourCellLocationAtStep(zero_starting_location, GetDirectionClockwise(direction, 1), 2 * cw);

            // Starting
            cell = this.GetCell(starting_location);
            if (cell.IsRock() || cell.IsNone()) cell.Type = VirtualCell.CellType.CorridorFloor;

            // Target
            target_location = GetTargetLocation(starting_location, direction);
            cell = this.GetCell(target_location);
            //if (cell.IsFloor()) break; // NO! Already !
            if (cell.IsRock() || cell.IsNone()) cell.Type = VirtualCell.CellType.CorridorFloor;
            

            //		cell.Orientation = direction;	// This is removed, because the directional tiles will take care of this instead

            // Connection passage
            CellLocation connection_location = GetNeighbourCellLocation(starting_location, direction);
            cell = this.GetCell(connection_location);
            if (cell.Type == VirtualCell.CellType.CorridorWall)
            {
                cell.Type = VirtualCell.CellType.EmptyPassage;
                ConnectCells(starting_location, target_location);
            }
            else if(cell.Type == VirtualCell.CellType.RoomWall && makeDoor && CheckDoorCreation(cw,corridor_width))
            {
                cell.Type = VirtualCell.CellType.Door;
                cell.Orientation = direction;   // Force orientation for doors, or it will crash
                ConnectCells(starting_location, target_location);
            }
            //		cell.Orientation = direction;		// Setting this would just change randomly the direction of some walls in tilemaps. This is removed for now!

            // We also create walls around the digging, UNLESS there are already floors there (or out of bounds) (as we already have the walls)
            VirtualMap.DirectionType clockwise_dir = GetDirectionClockwise(direction, 1);
            VirtualMap.DirectionType counter_clockwise_dir = GetDirectionClockwise(direction, -1);
            CellLocation neigh_same_loc;

            // Sides back
            if (!IsInRoom(starting_location))
            {
                if (cw == corridor_width-1)
                {
                    neigh_same_loc = this.GetNeighbourCellLocationOfSameType(starting_location, clockwise_dir);
                    if (LocationIsOutsideBounds(neigh_same_loc) ||
                        this.GetCell(neigh_same_loc).Type != VirtualCell.CellType.CorridorFloor)
                    {
                        VirtualCell other_cell = this.GetCell(this.GetNeighbourCellLocation(starting_location, clockwise_dir));
                        if (other_cell.Type == VirtualCell.CellType.EmptyPassage) other_cell.Type = VirtualCell.CellType.CorridorWall;
                    }
                }

                if (cw == 0)
                {
                    neigh_same_loc = this.GetNeighbourCellLocationOfSameType(starting_location, counter_clockwise_dir);
                    if (LocationIsOutsideBounds(neigh_same_loc) ||
                        this.GetCell(neigh_same_loc).Type != VirtualCell.CellType.CorridorFloor)
                    {
                        VirtualCell other_cell = this.GetCell(this.GetNeighbourCellLocation(starting_location, counter_clockwise_dir));
                        if (other_cell.Type == VirtualCell.CellType.EmptyPassage) other_cell.Type = VirtualCell.CellType.CorridorWall;
                    }
                }
            }

            // Sides front
            if (!IsInRoom(target_location))
            {
                if (cw == corridor_width - 1)
                {
                    neigh_same_loc = this.GetNeighbourCellLocationOfSameType(target_location, clockwise_dir);
                    if (LocationIsOutsideBounds(neigh_same_loc) ||
                        this.GetCell(neigh_same_loc).Type != VirtualCell.CellType.CorridorFloor)
                    {
                        VirtualCell other_cell = this.GetCell(this.GetNeighbourCellLocation(target_location, clockwise_dir));
                        if (other_cell.Type == VirtualCell.CellType.EmptyPassage) other_cell.Type = VirtualCell.CellType.CorridorWall;
                    }
                }

                if (cw == 0)
                {
                    neigh_same_loc = this.GetNeighbourCellLocationOfSameType(target_location, counter_clockwise_dir);
                    if (LocationIsOutsideBounds(neigh_same_loc) ||
                        this.GetCell(neigh_same_loc).Type != VirtualCell.CellType.CorridorFloor)
                    {
                        VirtualCell other_cell = this.GetCell(this.GetNeighbourCellLocation(target_location, counter_clockwise_dir));
                        if (other_cell.Type == VirtualCell.CellType.EmptyPassage) other_cell.Type = VirtualCell.CellType.CorridorWall;
                    }
                }
            }

            // Back
            if (!IsInRoom(starting_location))
            {
                neigh_same_loc = this.GetNeighbourCellLocationOfSameType(starting_location, GetDirectionOpposite(direction));
                if (LocationIsOutsideBounds(neigh_same_loc) ||
                    this.GetCell(neigh_same_loc).Type != VirtualCell.CellType.CorridorFloor)
                {
                    VirtualCell other_cell = this.GetCell(this.GetNeighbourCellLocation(starting_location, GetDirectionOpposite(direction)));
                    if (other_cell.Type == VirtualCell.CellType.EmptyPassage) other_cell.Type = VirtualCell.CellType.CorridorWall;
                }
            }

            // Front
            if (!IsInRoom(target_location))
            {
                neigh_same_loc = this.GetNeighbourCellLocationOfSameType(target_location, direction);
                if (LocationIsOutsideBounds(neigh_same_loc) ||
                    this.GetCell(neigh_same_loc).Type != VirtualCell.CellType.CorridorFloor)
                {
                    VirtualCell other_cell = this.GetCell(this.GetNeighbourCellLocation(target_location, direction));
                    if (other_cell.Type == VirtualCell.CellType.EmptyPassage) other_cell.Type = VirtualCell.CellType.CorridorWall;
                }
            }

        }


		return target_location;
    }

    private bool CheckDoorCreation(int count, int range)
    {
        bool createDoor = true;
        if (ONE_DOOR_PER_CORRIDOR)
        {
            //Debug.Log(count + " " + range / 2);
            if (count != range / 2) createDoor = false;
        }
        return createDoor;
    }

	// Get target starting_location from a valid cell/direction
	public CellLocation GetTargetLocation(CellLocation location, VirtualMap.DirectionType direction)
    {
        if (!HasAdjacentCellInDirection(location, direction)) 
			return new CellLocation(-1,-1);
		else
			return GetNeighbourCellLocationOfSameType(location, direction);
        
    }
	
	// A dead end cell has one and only one direction free for walking (i.e. there is no wall there -> empty)
	public bool IsDeadEnd(CellLocation l, bool alsoConsiderDoors = false)
	{
		int emptyCount = 0;
		
		CellLocation[] locs = GetAllNeighbours(l);
		foreach(CellLocation n_l in locs){
            VirtualCell.CellType type = this.GetCell(n_l.x,n_l.y).Type;
			if ( type == VirtualCell.CellType.EmptyPassage
                || (alsoConsiderDoors &&  VirtualCell.IsDoor(type))) {
				emptyCount++;
				//Debug.Log("For loc " + l + " neigh " + n_l + " is empty!");	
			}
		}
		
		return emptyCount == 1;	
	}
	
	// A rock is an unreachable place surrounded by walls
	public bool IsSurroundedByWalls(CellLocation l) 
	{
		int emptyCount = 0;
		
		CellLocation[] locs = GetAllNeighbours(l);
		foreach(CellLocation n_l in locs){
			if (this.GetCell(n_l.x,n_l.y).Type == VirtualCell.CellType.EmptyPassage) {
				emptyCount++;
//				Debug.Log("For loc " + l + " neigh " + n_l + " is empty!");	
			}
		}

        if (emptyCount == 0 && this.GetCell(l.x, l.y).connectedCells.Count > 0)
        {
//			Debug.Log ("Not a rock, just an isolated floor cell!");
			return false;
		}
		return emptyCount == 0;
	}

	// TODO: remove those
	public bool IsFloor(CellLocation l){
		return this.GetCell(l).IsFloor();
	}
	
	public bool IsRoomFloor(CellLocation l){
		return this.GetCell(l).Type == VirtualCell.CellType.RoomFloor;	
	}

	public bool CellsAreInTheSameRoom(CellLocation l1, CellLocation l2){
		VirtualRoom room1 = null, room2 = null;
		foreach(VirtualRoom room in this.rooms){
			if (room.containsLocation(l1)) {
				room1 = room;
//				Debug.Log ("ROOM 1: " + room1);
				break;
			}
		}
		foreach(VirtualRoom room in this.rooms){
			if (room.containsLocation(l2)) {
				room2 = room;
//				Debug.Log ("ROOM 2: " + room2);
				break;
			}
		}

//		if (room1 == room2) Debug.Log ("SAME ROOM!");
//		else Debug.Log ("NOT SAME!");

		return room1 == room2;
	}

	
	public bool HasAdjacentFloor(CellLocation l){
		CellLocation[] locs = GetAllSameNeighbours(l);
		foreach(CellLocation n_l in locs){
//			Debug.Log(n_l);
			if (!LocationIsOutsideBounds(n_l) && GetCell(n_l).IsFloor()) {
				return true;
			} 
		} 
		return false;
	}

	public bool HasAdjacentDoor(CellLocation l){
		foreach(CellLocation n_l in GetAllNeighbours(l)){
			if (!LocationIsOutsideBounds(n_l) && GetCell(n_l).IsDoor()) {
				return true;
			} 
		} 
		return false;
	}

	
	
	public VirtualCell GetCell(CellLocation l){
		return this.cells[l.x,l.y];	
	}
	public VirtualCell GetCell(int x, int y){
		return this.cells[x,y];
	}
    public void SetCell(int x, int y, VirtualCell cell)
    {
        this.cells[x, y] = cell;
    }

    public IEnumerable<VirtualCell> GetAllCells()
    {   
        for (int i=0; i<Width; i++){
            for(int j=0; j<Height; j++){
                yield return this.cells[i, j];
            }
        }
    }

	
	public IEnumerable<CellLocation> DeadEndCellLocations
    {
        get
		{
			//NOTE: This creates an enumerator, so that if a starting_location becomes a dead end during the following removal it will be updated automatically (if following the order of the grid!)
			foreach(CellLocation l in floorCells)
                if (IsDeadEnd(l)) {
					//Debug.Log("Location " + l + " is a dead end!");
					yield return new CellLocation(l.x, l.y);
				}
        }
    }


    public CellLocation GetWalkableLocation()
    {
        foreach (CellLocation cl in WalkableLocations)
        {
            if (GetCell(cl).IsFloor()) return cl;
        }
        return default(CellLocation);
    }
	
	public IEnumerable<CellLocation> WalkableLocations
    {
        get
		{
            foreach(CellLocation l in floorCells)	// Floor cells may also be rocks!
                if (cells[l.x,l.y].IsFloor()) yield return new CellLocation(l.x, l.y);
        }
    }
	public IEnumerable<CellLocation> RoomWalkableLocations {
		get
		{
			foreach(CellLocation l in roomCells) yield return new CellLocation(l.x, l.y);
		}
	}
	
	public DirectionType CalculateDeadEndCorridorDirection(CellLocation location)
    {
	    if (!IsDeadEnd(location)) throw new InvalidOperationException();
	
	    if (this.cells[location.x, location.y-1].Type == VirtualCell.CellType.EmptyPassage) return DirectionType.South;
	    if (this.cells[location.x, location.y+1].Type == VirtualCell.CellType.EmptyPassage) return DirectionType.North;
	    if (this.cells[location.x-1, location.y].Type == VirtualCell.CellType.EmptyPassage) return DirectionType.West;
	    if (this.cells[location.x+1, location.y].Type == VirtualCell.CellType.EmptyPassage) return DirectionType.East;
	
	    throw new InvalidOperationException();
   }
	public void CreateWall(CellLocation location, DirectionType direction)
    {
        CellLocation connection = GetNeighbourCellLocation(location, direction);
		
		if(!(this.cells[connection.x,connection.y].Type==VirtualCell.CellType.RoomWall))
		 this.cells[connection.x,connection.y].Type=VirtualCell.CellType.CorridorWall;
				
		// Remove the connection
		CellLocation target = GetTargetLocation(location, direction);
        DisconnectCells(location, target);
    }
	//is the cell in that direction a Rock?
	public bool AdjacentCellInDirectionIsRock(CellLocation location, DirectionType direction)
    {
        if (HasAdjacentCellInDirection(location, direction))
		{
            CellLocation l = GetNeighbourCellLocationOfSameType(location, direction);
			return IsSurroundedByWalls(l);
		}
		return true;
    }

	// Check if two locations are the same
	public bool CompareLocations (CellLocation location1, CellLocation location2)
	{
		return (location1.x == location2.x && location1.y == location2.y);
	}

	
	// Can we put a door around this cell?
    public bool IsDoorable(CellLocation l)
	{
//			Debug.Log("Is " + l + " doorable?");
			// Cannot already have a door here
//			if ((!LocationIsOutsideBounds(new Location(l.x-1,l.y)) && this.cells[l.x-1,l.y].Type == VirtualCell.CellType.Door) || 
//			(!LocationIsOutsideBounds(new Location(l.x+1,l.y)) && this.cells[l.x+1,l.y].Type == VirtualCell.CellType.Door)  || 
//			(!LocationIsOutsideBounds(new Location(l.x,l.y-1)) &&this.cells[l.x,l.y-1].Type == VirtualCell.CellType.Door)  || 
//			(!LocationIsOutsideBounds(new Location(l.x,l.y+1)) &&this.cells[l.x,l.y+1].Type == VirtualCell.CellType.Door) ) {
//				Debug.Log("No connections to floors here!");
//				return false;
//			}
//			else
//			{
				// Not a rock
//				Debug.Log("Is dead end: " + isDeadEnd(l));
//				Debug.Log("Is there a rock? " + isRock(l));
			return !IsSurroundedByWalls(l);
//			}
            
	}

	public bool IsInRoom(CellLocation l){
		if (rooms == null || rooms.Count == 0) return false;
		foreach(VirtualRoom room in rooms){
			if (room.IsInRoom(l)){
				return true;
			}
		}
		return false;
	}
	
	public bool IsOnRoomBorder(CellLocation l){
		if (rooms == null || rooms.Count == 0) return false;
		foreach(VirtualRoom room in rooms){
			if (room.IsInBorder(l)){
				return true;
			}
		}
		return false;
	}


    // Returns true if this 'none' (i.e. column) cell can be removed from the map (i.e. not shown)
    public bool IsColumnRemovable(CellLocation l, bool drawCorners = true, bool createColumnsInRooms = false)
    {
        VirtualCell cell = this.GetCell(l);
        if (cell.IsNone())  // Should be performed only on a NONE
        {
            int validNeigh = 0;
            int wallCount = 0;
            int emptyCount = 0;

            CellLocation n;
            foreach (DirectionType dir in directions)
            {
                n = GetNeighbourCellLocation(l, dir);
                if (!LocationIsOutsideBounds(n))
                {
                    validNeigh++;
                    VirtualCell neigh_cell = GetCell(n);
                    //Debug.Log(neigh_cell.Type);
                    if (neigh_cell.IsEmpty() || neigh_cell.IsRock()) emptyCount++;
                    else
                    {
                        wallCount++;

                        // HACK: we need to do this or the columns won't know if the walls have been made removable during this step!
                        if (IsPassageRemovable(n)) 
                        {
                            wallCount--;
                            emptyCount++;
                        }
                    }
                    //Debug.Log("Neigh " + n + " is " + GetCell(n).Type);
                }
            }

            //Debug.Log("Cell " + l + " W " + wallCount + " ,  E " + emptyCount + " ,  V " + validNeigh);
            if (!drawCorners)
            {
                // We do not draw corners of rooms (corner = two walls and adjacent)
                if (wallCount == 2)
                {
                    // We check whether the two walls are adjacent (also doors)
                    // At least one neigh wall need not be removable as well for this to be a corner (and not an isolated None cell)
                    bool lastWasWall = false;
                    foreach (DirectionType dir in directions)
                    {
                        n = GetNeighbourCellLocation(l, dir);
                        //Debug.Log(n);
                        if (!LocationIsOutsideBounds(n) && (GetCell(n).IsWall() || GetCell(n).IsDoor())) {
                            //Debug.Log("WALL!");
                            if (lastWasWall) return true;   // Adjacent!
                            lastWasWall = true;
                        } else lastWasWall = false;
                    }
                }
            }


            // HACK: check if we are in a room (as in the standardmap a roomcolumn is in fact surrounded by empties!)
            if (IsInRoom(l) && createColumnsInRooms)
            {
                return false;
            }

            return wallCount == 0;
		}
		return false;
    }

	// Returns true if this passage cell can be removed from the map (i.e. not shown)
	public bool IsPassageRemovable(CellLocation l)
	{
		VirtualCell cell = this.GetCell(l);
        if (cell.IsWall() || cell.IsEmpty())
        {
            // We count how many valid neighs are floors, and how many are nones
			int validNeigh = 0;
			int floorCount = 0;
			int noneCount = 0;
			
			CellLocation n;
			foreach(DirectionType dir in directions){
                n = GetNeighbourCellLocation(l, dir);
				if(!LocationIsOutsideBounds(n)){
					validNeigh++;
					VirtualCell neigh_cell = GetCell(n);
                    if (neigh_cell.IsFloor()) floorCount++;
                    else if (neigh_cell.IsNone() || neigh_cell.IsRock()) noneCount++; 
				}
			}
            //Debug.Log(l + " Valid neighs: " + validNeigh + " floorCount: " + floorCount + " noneCount: " + noneCount);
            return floorCount == 0;
		}
		return false;
	}
	public MetricLocation GetActualLocation(CellLocation l, int storey)
	{
		return new MetricLocation(l.x/2.0f,l.y/2.0f, storey);
	}




	/****************************
	 *  Connection and distance
	 ***************************/

	private void ResetCellsAsUnvisited(){
		foreach (VirtualCell c in this.cells) {
			c.visited = false;
			c.distance_from_root = 10000; // Initial large distance
		}
	}

	public void ComputeCellDistances(CellLocation startCellLocation = default(CellLocation)){
        // Computes all cell distances from startCellLocation
        // Now uses the connectedCells list to compute, way faster!
		this.ResetCellsAsUnvisited ();
		List<CellLocation> unvisited_locations = new List<CellLocation>(this.WalkableLocations);
		CellLocation currentLocation = startCellLocation;
		if (startCellLocation == default(CellLocation)) currentLocation = unvisited_locations[0];
		GetCell (currentLocation).distance_from_root = 0;
		this.root = currentLocation;	// We set this as the root

        // This goes inside all the connections and computes distances
        // Note that this requires all cells to be connected (should ALWAYS be the case with our deungeons!)
        ComputeCellDistanceRecursive(GetCell(currentLocation), unvisited_locations, 0);
        //PrintDistances();
    }

    private void ComputeCellDistanceRecursive(VirtualCell input_cell, List<CellLocation> unvisited_locations, int current_distance)
    {
        //Debug.Log("Check cell " + input_cell.location + " dist " + current_distance);
        input_cell.visited = true;
        current_distance += 1;

        foreach (CellLocation nl in input_cell.connectedCells)
        {
            VirtualCell nc = this.GetCell(nl);
            //Debug.Log("NEIGH LOC" + nl);
            int last_distance = nc.distance_from_root;
            nc.distance_from_root = (current_distance <= last_distance) ? current_distance : last_distance;
            //Debug.Log("SET DIST " + nc.distance_from_root);
            if (!nc.visited)
            {
                ComputeCellDistanceRecursive(nc, unvisited_locations, current_distance);
            }
        }
    }
        /*

		while (true) {
//			Debug.Log ("CHECKING LOC" + currentLocation);
			VirtualCell current_cell = GetCell (currentLocation);
			//CellLocation[] neighbour_locations = this.GetAllSameNeighbours (currentLocation);
			current_distance = current_cell.distance_from_root + 1;
//			Debug.Log ("NEW DIST: " + current_distance);
			foreach (CellLocation nl in current_cell.connectedCells) {
				//if (LocationIsOutsideBounds(nl)) continue;
				//if (!CanConnectToNeighbour(currentLocation,nl)) continue;
				VirtualCell nc = this.GetCell (nl);
//				Debug.Log ("NEIGH LOC" + nl);
				if (!nc.visited) {
					int last_distance = nc.distance_from_root;
					nc.distance_from_root = (current_distance <= last_distance) ? current_distance : last_distance;
//					Debug.Log ("SET DIST " + nc.distance_from_root);
				}
			}
			current_cell.visited = true;
			unvisited_locations.Remove (currentLocation);
			if (unvisited_locations.Count == 0) break;	// We finished!

			// Choose the next cell to use
			int min_distance = 10000;
			foreach (CellLocation cl in unvisited_locations) {
				if (GetCell (cl).distance_from_root < min_distance) {
					currentLocation = cl;
					min_distance = GetCell (cl).distance_from_root;
				}
			}
			it++;
			if (it>100) break;
		}
//		PrintDistances ();
	}*/

	public int GetMaximumDistance(){
		int max_distance = 0;
		foreach (CellLocation cl in WalkableLocations) {
			int walk_distance = GetWalkDistance(root,cl);
			if (walk_distance > max_distance) max_distance = walk_distance;
		}
		return max_distance;
	}

	public int GetWalkDistance(CellLocation s, CellLocation e){
		// TODO: this works by walking to the root and back!! Doens't consider other paths! Not good!
		return Mathf.Abs( GetCell (s).distance_from_root + GetCell (e).distance_from_root);
	}

	public bool CanConnectToNeighbour(CellLocation a, CellLocation b){
		// Only if there is not a wall in-between.
		// Works only for neighbours!
		DirectionType direction = GetDirectionBetweenNeighbours (a, b);
//		Debug.Log ("Direction between " + a + " and " + b + " is " + direction);
        CellLocation passage_cell_location = GetNeighbourCellLocation(a, direction);
//		Debug.Log ("PASSAGE: " + passage_cell_location);
		bool canConnect = !GetCell(passage_cell_location).IsWall();
//		Debug.Log ("CAN CONNECT? " + canConnect);
		return canConnect;
	}

	public DirectionType GetDirectionBetweenNeighbours(CellLocation a, CellLocation b){
		// Works only for neighbours
		DirectionType type = DirectionType.None;
		if (a.x < b.x) type = DirectionType.East;
		else if (a.x > b.x) type = DirectionType.West;
		else if (a.y > b.y) type = DirectionType.South;
		else if (a.y < b.y) type = DirectionType.North;
		return type;
	}

	public CellLocation GetCellLocationInLimits(IEnumerable<CellLocation> iterable_locations, float min_wanted_distance, float max_wanted_distance){
        //Debug.Log("Min : " + min_wanted_distance + " MAX " + max_wanted_distance);

        // We make them ints, since the distances are integers
        min_wanted_distance = Mathf.FloorToInt(min_wanted_distance);
        max_wanted_distance = Mathf.CeilToInt(max_wanted_distance);

        //Debug.Log("Min : " + min_wanted_distance + " MAX " + max_wanted_distance);

        float min_distance_from_good_distance = 10000;
		CellLocation start = this.root;
		CellLocation current_chosen_location = default(CellLocation);
		foreach (CellLocation cl in iterable_locations) {
			int current_distance = GetWalkDistance(start,cl);
			//Debug.Log ("MIN: " + min_wanted_distance + "   MAX: " + max_wanted_distance);
            //Debug.Log ("current new max: " + current_distance);
			if (current_distance >= min_wanted_distance && current_distance <= max_wanted_distance) {
                //Debug.Log ("FOUND!");
				return cl;	// Found it!
            }
            else
            {
                float to_min = Mathf.Abs(min_wanted_distance - current_distance);
                float to_max = Mathf.Abs(current_distance - max_wanted_distance);
                float minimum = Mathf.Min(to_min, to_max);
                if (minimum < min_distance_from_good_distance)
                {
                    //Debug.Log("New minimum: " + minimum + " at distance " + current_distance);
                    min_distance_from_good_distance = minimum;
                    current_chosen_location = cl;
                }
            }
		}
		// Couldn't find that distance!
		Debug.LogWarning("Couldn't find a cell distant " + min_wanted_distance + " to " + max_wanted_distance + " from the starting cell! Returning closer cell to these limits.");
		return current_chosen_location;
	}


    /********************
     * Cell Connection
     ********************/

	public void ConnectCells(CellLocation s, CellLocation e)
	{
		//Debug.Log("Connecting cell " + s + " and " + e);
		this.GetCell(s).connectedCells.Add(e);
        this.GetCell(e).connectedCells.Add(s);
	}
	public void DisconnectCells(CellLocation s, CellLocation e)
    {
        //Debug.Log("Disconnecting cell " + s + " and " + e);
        this.GetCell(s).connectedCells.Remove(e);
        this.GetCell(e).connectedCells.Remove(s);
	}

    public bool ExistsPathBetweenLocations(CellLocation s, CellLocation e)
    {
        List<CellLocation> visited = new List<CellLocation>();
        return ExistsPathBetweenLocationsRecursive(s, e, visited);
    }

    private bool ExistsPathBetweenLocationsRecursive(CellLocation s, CellLocation e, List<CellLocation> visited)
    {
        //Debug.Log("Visiting " + s);
        visited.Add(s);
        VirtualCell s_cell = GetCell(s);
        foreach(CellLocation c in s_cell.connectedCells){
            if (c == e)
            {
                //Debug.Log("Found the end!");
                return true; // Found this!
            }
            else
            {
                if (!visited.Contains(c))
                {
                    bool foundEnd = ExistsPathBetweenLocationsRecursive(c, e, visited);
                    //if (foundEnd) Debug.Log("Found the end in child!");
                    if (foundEnd) return true; // Found connected!
                }
            }
        }
        return false; // Not here!
    }

    /********************
     * Miscellaneous
     ********************/

    private int Min(int a, int b)
    {
        return (a <= b) ? a : b;
    }

    public void Print(){
		Debug.Log(width + "X" + height);
		string s ="";
		for (int i = 0; i < this.width; i++){
			for (int j = 0; j < this.height; j++){	
				if (GetCell(i,j).IsFloor()){
					s+="o";
				} else if (GetCell(i,j).IsWall()){
					s+="|";	
				} else {
					s+="x";
				}
			}
			s +="\n";
		}
		Debug.Log(s);
	}

	public void PrintDistances(){
		// Prints the grid with distances on the walkable tiles
		Debug.Log(width + "X" + height);
		string s ="";
		for (int i = 0; i < this.width; i++){
			for (int j = 0; j < this.height; j++){	
				if (GetCell(i,j).IsFloor()){
					s += GetCell(i,j).distance_from_root;
				} else if (GetCell(i,j).IsRock()){
					s += "X";
				}
//				} else if (GetCell(i,j).IsWall()){
//					s+="|";	
//				} else {
//					s+="x";
//				}
			}
			s +="\n";
		}
		Debug.Log(s);
	}


    /*******************
     * Various info
     *******************/


    public List<VirtualRoom.Dimensions> GetRoomDimensions()
    {
        List<VirtualRoom.Dimensions> dimensions = new List<VirtualRoom.Dimensions>();
        // Returns a structure with the dimensions of all the rooms
        //Debug.Log(rooms);
        foreach (VirtualRoom room in rooms)
        {
            VirtualRoom.Dimensions d = room.GetDimensions();
            d.storey = this.storey_number;
            dimensions.Add(d);
        }
        return dimensions;
    }


    public VirtualMap.Dimensions GetMapDimensions()
    {
        //Print();

        // Gets the map dimensions by checking what are the actual width and height of the map.
        int actual_width = 0;
        int actual_height = 0;

        int left_border = width;
        bool left_found = false;
        int right_border = 0;
        bool right_found = false;

        for (int i = 0; i < this.width; i++)
        {
            // For each column, we seek whether we find a cell. This will be the left border.
            if(!left_found){
                for (int j = 0; j < this.height; j++)
                {
                    if (!cells[i, j].IsNone())
                    {
                        left_border = i;
                        //Debug.Log("L: " + left_border);
                        left_found = true;
                        break;  // FOUND!
                    }
                }
            }

            // We do the same for the right border
            if (!right_found)
            {
                for (int j = 0; j < this.height; j++)
                {
                    if (!cells[this.width - 1 - i, j].IsNone())
                    {
                        right_border = this.width - i;
                        //Debug.Log("R: " + right_border);
                        right_found = true;
                        break;  // FOUND!
                    }
                }
            }

            if (left_found && right_found)
            {
                actual_width = right_border-left_border;
                break; 
            }
        }

        // We do the same for the top/bottom border
        int top_border = 0;
        bool top_found = false;
        int bottom_border = height;
        bool bottom_found = false;
        for (int i = 0; i < this.height; i++)
        {
            if (!bottom_found)
            {
                for (int j = 0; j < this.width; j++)
                {
                    if (!cells[j,i].IsNone())
                    {
                        bottom_border = i;
                        //Debug.Log("B: " + bottom_border);
                        bottom_found = true;
                        break;  // FOUND!
                    }
                }
            }

            if (!top_found)
            {
                for (int j = 0; j < this.height; j++)
                {
                    if (!cells[j, this.height - 1 - i].IsNone())
                    {
                        top_border = this.height - i;
                        //Debug.Log("T: " + top_border);
                        top_found = true;
                        break;  // FOUND!
                    }
                }
            }

            if (top_found && bottom_found)
            {
                actual_height = top_border - bottom_border;
                break;
            }
        }

        VirtualMap.Dimensions dim = new VirtualMap.Dimensions();
        dim.left = left_border;
        dim.right = right_border;
        dim.top = top_border;
        dim.bottom = bottom_border;
        dim.width_x = actual_width;
        dim.width_y = actual_height;
        dim.storey = storey_number;
        return dim;
    }
}

