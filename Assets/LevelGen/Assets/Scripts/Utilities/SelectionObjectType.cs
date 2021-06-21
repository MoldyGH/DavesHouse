using UnityEngine;
using System.Collections;

public enum SelectionObjectType{
	All,
	
	Walls,
	Rocks,
	Floors,
	Columns,
	Ceilings,
	Doors,
	
	Rooms,
	Corridors,
	
	Walkable,
	Unwalkable,

	RoomFloors,
	CorridorFloors,
	
	RoomColumns,
	CorridorColumns,
	InsideRoomColumns,
	PassageColumns,
	
	RoomCeilings,
	CorridorCeilings,
	
	RoomWalls,
	CorridorWalls,
	
	Perimeter,
	Ladders,
}

