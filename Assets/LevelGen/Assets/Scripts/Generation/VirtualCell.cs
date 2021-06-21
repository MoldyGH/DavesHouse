using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[System.Serializable]
public class VirtualCellGrid : IEnumerable<VirtualCell>
{
    [System.Serializable]
    private class VirtualCellArray
    {
        [SerializeField]
        private VirtualCell[] array;

        public VirtualCellArray(int size)
        {
            array = new VirtualCell[size];
        }

        public VirtualCell this[int i]
        {
            get
            {
                if (i >= this.array.Length)Debug.Log(i + " " + this.array.Length);
                return array[i];
            }
            set
            {
                array[i] = value;
            }
        }

    }

    [SerializeField]
    private VirtualCellArray[] grid;
    private int size_x;
    private int size_y;
    
	public VirtualCellGrid(int size_x, int size_y)
	{
        this.size_x = size_x;
        this.size_y = size_y;
        this.grid = new VirtualCellArray[size_x];
        for (int i = 0; i < size_x; i++) this.grid[i] = new VirtualCellArray(size_y);
	}

    public VirtualCell this[int i, int j]
    {
        get
        {
            return grid[i][j];
        }
        set 
        {
            grid[i][j] = value;
        }
    }

    public IEnumerator<VirtualCell> GetEnumerator() {
        for (int i = 0; i < size_x; i++)
        {
            for (int j = 0; j < size_y; j++)
            {
                yield return grid[i][j];
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
}

// An instance inside a cell
[System.Serializable]
public class CellInstance{
	public VirtualCell.CellType type;
	public VirtualMap.DirectionType dir;

	public CellInstance (VirtualCell.CellType type, VirtualMap.DirectionType dir)
	{
		this.type = type;
		this.dir = dir;
	}
	public static CellInstance Copy(CellInstance other){
		CellInstance new_ci = new CellInstance(other.type,other.dir);
		return new_ci;
	}

}

[System.Serializable]
public class VirtualCell
{
    public int connectedAreaIndex;    // All cells with the same index are from the same connected area
	public bool visited;    // Used by algorithms
	public CellLocation location;

	// Variables for the graph representation
	public int distance_from_root;
	//public int index_from_root;

    // A graph representation useful for connected cells checking
    // This is used for navigation, door creation, and distance computations
    [SerializeField]
	public List<CellLocation> connectedCells; 

	public enum CellType
	{
		// Passage cells
		EmptyPassage,	// Empty is a passage!
		Border,
		CorridorWall,
		RoomWall,
		Door,
		RoomDoor,
		PerimeterWall,

		// Floor cells
		None,
		Rock,	// Non-passable
		CorridorFloor,
		RoomFloor,

		// Advanced tiles: floors
		CorridorFloorU,
		CorridorFloorI,
		CorridorFloorL,
		CorridorFloorT,
		CorridorFloorX,
		RoomFloorInside,
		RoomFloorBorder,
		RoomFloorCorner,

		// Advanced tiles: columns
		CorridorWallO, // No neighbours
		CorridorWallU, 
		CorridorWallI,
		CorridorWallL,
		CorridorWallT,
		CorridorWallX,
		RoomWallU,
		RoomWallO, // No neighbours
		RoomWallI,
		RoomWallL,
		RoomWallT,
		RoomWallX,

		// Ceiling
		CorridorCeiling,
		RoomCeiling,
		
		// Column cells
		CorridorColumn,
		RoomColumn,
		InsideRoomColumn,
		PassageColumn,
		PerimeterColumn,

		// Multi-Storey
		Ladder,
		Ladder2,

		// Fake 3D
		Fake3D_Corridor_WallAbove,
		Fake3D_Corridor_WallFront,
		Fake3D_Room_WallAbove,
		Fake3D_Room_WallFront,

		DoorVertical,
		//		DoorHorizontalTop,
		DoorHorizontalBottom,
		
	};

    [SerializeField]
	private List<CellInstance> instances;	// Instances of this cell. The first one is the main.

	public VirtualCell (bool visited, CellLocation location)
	{
		this.visited = visited;
		this.location = location;
		//this.index_from_root = -1;
        instances = new List<CellInstance>();
        connectedCells = new List<CellLocation>();
	}
	public VirtualCell (CellLocation location):this(false,location){	
	}

	public static VirtualCell Copy(VirtualCell input_cell){
		VirtualCell copied_cell = new VirtualCell(input_cell.location);
		foreach(CellInstance c in input_cell.instances) copied_cell.AddCellInstance(CellInstance.Copy(c));
        copied_cell.connectedCells = input_cell.connectedCells; // They have the same connected cells TODO: maybe instead copy them alltogether?
        copied_cell.distance_from_root = input_cell.distance_from_root;
		return copied_cell;
	}
	
	public CellType Type { 
		get {
            //Debug.Log(this.instances);
			if (this.instances.Count == 0) return CellType.None;	// TODO: Check that this is correct! It may happen that the type is removed by the stairs. If this happens, then the cell is basically empty, hence why it should be a None.
			//Debug.Log(this.instances.Count);
			return this.instances[0].type; } 
		set { 
			if (this.instances.Count == 0) this.instances.Add (new CellInstance(value,VirtualMap.DirectionType.None));
			else this.instances[0].type = value; 
		}
	}
	
	public VirtualMap.DirectionType Orientation {
		get { return this.instances[0].dir;}
		set { this.instances[0].dir = value;}
	}

	public void AddCellInstance(CellInstance ci){
		if (!this.instances.Contains(ci)) this.instances.Add(ci);
	}
	public void AddCellInstance(VirtualCell.CellType type, VirtualMap.DirectionType dir){
		// TODO: Should we check if we add two times the same type?
		this.instances.Add(new CellInstance(type,dir));
	}

	public void RemoveCellInstance(CellInstance ci){
		if (this.instances.Contains(ci)) {
			this.instances.Remove(ci);
		}
	}
	
	public void RemoveCellInstancesOfTypesInSelection(SelectionObjectType sop){
		foreach(VirtualCell.CellType t in SelectionManager.GetCellTypes(sop)){
			List<CellInstance> tmp_list = new List<CellInstance>(this.instances);
			foreach(CellInstance ci in tmp_list){
				if (ci.type == t) RemoveCellInstance(ci);
			}
		}
	}

	public List<CellInstance> GetCellInstances(){
		return instances;
	}

	/*public bool IsRootOfGraph (){
		return index_from_root == distance_from_root;
	}*/

	// Returns true if this cell represent a tile (floor or none)
	public bool IsTile(){
		return this.location.x % 2 == this.location.y % 2;
	}
	// True if this cell represents a border
	public bool IsBorder(){
		return !IsTile();
	}

	public bool IsRoom(){
		return IsRoomFloor () || IsRoomColumn () || IsInsideRoomColumn () || IsRoomWall ()  || IsPassageColumn () || IsRoomDirectionalWall();
	}

	public bool IsFloor(){
		return IsFloor (Type);
	}
	public bool IsWall(){
		return Type == CellType.CorridorWall || Type==CellType.RoomWall || Type==CellType.PerimeterWall
			|| Type == CellType.Fake3D_Corridor_WallAbove
			|| Type == CellType.Fake3D_Corridor_WallFront
			|| Type == CellType.Fake3D_Room_WallAbove
			|| Type == CellType.Fake3D_Room_WallFront
				|| IsDirectionalWall();	
	}
	public bool IsColumn(){
		return Type == CellType.CorridorColumn || Type == CellType.PassageColumn || Type == CellType.InsideRoomColumn || Type == CellType.RoomColumn || Type == CellType.PerimeterColumn;
	}
	public bool IsInsideRoomColumn(){
		return Type == CellType.InsideRoomColumn;
	}

	public bool IsCorridorDirectionalWall(){
		return Type == CellType.CorridorWallI || Type ==CellType.CorridorWallU || Type ==CellType.CorridorWallL || Type ==CellType.CorridorWallO || Type ==CellType.CorridorWallT || Type ==CellType.CorridorWallX;
	}
	public bool IsRoomDirectionalWall(){
		return Type == CellType.RoomWallI || Type == CellType.RoomWallU || Type == CellType.RoomWallL || Type ==CellType.RoomWallO || Type ==CellType.RoomWallT || Type ==CellType.RoomWallX;
	}
	public bool IsDirectionalWall(){ 
		return IsCorridorDirectionalWall() || IsRoomDirectionalWall();
	}

	public bool IsCorridorWall(){
		return Type == CellType.CorridorWall;	
	}
	public bool IsCorridorFloor(){
		return IsCorridorFloor(Type);
	}
	public bool IsCorridorColumn(){
		return Type == CellType.CorridorColumn;
	}

	public bool IsRock(){
		return Type == CellType.Rock;	
	}
	public bool IsEmpty(){
		return Type == CellType.EmptyPassage;	
	}
	public bool IsDoor(){
        return IsDoor(Type);
	}
	public bool IsRoomFloor(){
		return IsRoomFloor(Type);
	}
	public bool IsRoomWall(){
		return Type == CellType.RoomWall;	
	}
	public bool IsRoomColumn(){
		return Type == CellType.RoomColumn;
	}
	public bool IsPassageColumn(){
		return Type == CellType.PassageColumn;
	}

	public bool IsNone(){
		return Type == CellType.None;	
	}


	public static bool IsFloor(CellType type){
		return IsCorridorFloor(type) || IsRoomFloor(type);
	}
	public static bool IsCorridorFloor(CellType type){
		return type == CellType.CorridorFloor || type == CellType.CorridorFloorU ||  type == CellType.CorridorFloorI ||type == CellType.CorridorFloorL ||type == CellType.CorridorFloorT ||type == CellType.CorridorFloorX;	
	}
	public static bool IsRoomFloor(CellType type){
		return type == CellType.RoomFloor || type == CellType.RoomFloorBorder
            || type == CellType.RoomFloorCorner || type == CellType.RoomFloorInside;	
	}
    public static bool IsDoor(CellType type)
    {
        return type == CellType.Door || type == CellType.RoomDoor || type == CellType.DoorVertical || type == CellType.DoorHorizontalBottom;	 // || Type == CellType.DoorHorizontalTop
    }


	public static bool IsDynamic(CellType type){
		return type == CellType.Door || type == CellType.RoomDoor ;	
	}

	public bool IsHorizontal(){
		return this.Orientation == VirtualMap.DirectionType.East || this.Orientation == VirtualMap.DirectionType.West;
	}

    
    public override string ToString()
    {
        return this.location + " - " + this.Type;
    }
}



[System.Serializable]
public struct CellLocation
{
    public static CellLocation INVALID = new CellLocation(-1,-1);

	public int x;
	public int y;
	
	public CellLocation (int x, int y)
	{
		this.x = x;
		this.y = y;
	}
	
	override public string ToString(){
		return "("+this.x+","+this.y+")";	
	}
	
	public static bool operator ==(CellLocation a, CellLocation b)
	{
		return a.x == b.x && a.y == b.y;	
	}
	
	public static bool operator !=(CellLocation a, CellLocation b){
		return !(a == b);	
	}
	
	public override bool Equals(System.Object obj)
    {
        // If parameter is null, return false.
        if (obj == null)
        {
            return false;
        }

        // If parameter cannot be cast to CellLocation, return false.
		CellLocation p;
		if (obj is CellLocation){
        	p = (CellLocation)obj;
		} else {
            return false;
        }

        // Return true if the fields match.
        return this == p;
    }

    public bool Equals(CellLocation p)
    {
        // If parameter is null, return false.
        if ((object)p == null)
        {
            return false;
        }

        // Return true if the fields match.
        return this == p;
    }

    public override int GetHashCode()
    {
        return x ^ y;
    }
	
	public bool isValid(){
		return this.x != -1 && this.y != -1;	
	}

}

public struct MetricLocation
{
	public float x;
	public float y;
	public int storey;
	
	public MetricLocation (float x, float y, int storey)
	{
		this.x = x;
		this.y = y;
		this.storey = storey;
	}
}
