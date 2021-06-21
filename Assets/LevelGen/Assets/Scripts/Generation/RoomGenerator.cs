using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomGenerator
{
    // properties
    public int doorsDensityModifier;

    // Internal variables
    List<CellLocation> usedLocations;

    public RoomGenerator(){
		usedLocations = new List<CellLocation>();
    }

    public VirtualRoom CreateRoom(VirtualMap map, int width, int height, int start_x, int start_y)
    {
        VirtualRoom r = new VirtualRoom(width, height, new CellLocation(start_x, start_y));
        return CreateRoom(map, width, height, r);
    }
    public VirtualRoom CreateRoom(VirtualMap map, int width, int height, VirtualRoom r){
        // Check if we have enough space for a room
		if (r.leftCorner == new CellLocation(-1,-1)) return null;
        else {
            //Debug.Log ("Creating a room: w:"+width +" H:"+height +" starting_location:"+r.leftCorner);
			for (int i = 0; i < r.Width; i++) {
				for (int j = 0; j < r.Height; j++) {
					CellLocation l = new CellLocation (r.leftCorner.x + 2 * i, r.leftCorner.y + 2 * j);
                    // We create the room starting from the lower left floor

                    //Debug.Log("LOC: " + l);

					// Do not add to this room's cells if it already belongs to another room
					if (usedLocations.Contains(l)) {
//							Debug.Log (l + " already belongs to a room!");
						continue;
					}
						
					VirtualCell passageCell;

					// Set top passage to empty
					passageCell = map.GetCell(l.x, l.y+1);
					if (!passageCell.IsRoomWall()){	// May belong to another room already
						passageCell.Type = VirtualCell.CellType.EmptyPassage;
						if (l.y + 2 <= r.leftCorner.y + 2 * (r.Height - 1)) {
							map.ConnectCells (l, new CellLocation (l.x, l.y + 2));
                            //Debug.Log("EMPTY: " + l + " TO TOP");
						}
					}

					// Set right passage to empty
					passageCell = map.GetCell(l.x+1,l.y);
					if (!passageCell.IsRoomWall()){	// May belong to another room already
						passageCell.Type = VirtualCell.CellType.EmptyPassage;
						if (l.x + 2 <= r.leftCorner.x + 2 * (r.Width - 1)) {
							map.ConnectCells (l, new CellLocation (l.x + 2, l.y));
                            //Debug.Log("EMPTY: " + l + " TO RIGHT");
						}
					}

					// Other passages are potentially walls
					if (i == r.Width - 1) {
                        map.GetCell(l.x + 1, l.y).Type = VirtualCell.CellType.RoomWall;
                        CellLocation floor_l = new CellLocation(l.x + 2, l.y);
                        if (!map.LocationIsOutsideBounds(floor_l)) map.DisconnectCells(l, floor_l);
                        //Debug.Log("WALL RIGHT: " + map.GetCell(l.x + 1, l.y).starting_location);
					}
					if (i == 0) {
                        map.GetCell(l.x - 1, l.y).Type = VirtualCell.CellType.RoomWall;
                        CellLocation floor_l = new CellLocation(l.x - 2, l.y);
                        if (!map.LocationIsOutsideBounds(floor_l)) map.DisconnectCells(l, floor_l);
                        //Debug.Log("WALL LEFT: " + map.GetCell(l.x - 1, l.y).starting_location);
					}
					if (j == r.Height - 1) {
                        map.GetCell(l.x, l.y + 1).Type = VirtualCell.CellType.RoomWall;
                        CellLocation floor_l = new CellLocation(l.x, l.y + 2);
                        if (!map.LocationIsOutsideBounds(floor_l)) map.DisconnectCells(l, floor_l);
                        //Debug.Log("WALL TOP: " + map.GetCell(l.x, l.y + 1).starting_location);
					}
					if (j == 0) {
                        map.GetCell(l.x, l.y - 1).Type = VirtualCell.CellType.RoomWall;
                        CellLocation floor_l = new CellLocation(l.x, l.y - 2);
                        if (!map.LocationIsOutsideBounds(floor_l)) map.DisconnectCells(l, floor_l);
                        //Debug.Log("WALL BOT: " + map.GetCell(l.x, l.y - 1).starting_location);
					}

					map.AddRoomCell (l);
						
					r.cells.Add(l);
					usedLocations.Add(l);
				}
			}
		}
        return r;
	}
	
	/********************
     * Door creation
     ********************/
	// Create doors for a given room
	public void CreateDoors_Post (VirtualMap map, VirtualRoom r){
		List<CellLocation> borderFloors = new List<CellLocation> ();
		
		// Examine borderFloors, create a list of border floors
		for (int i = 0; i < r.Width; i++) {
			for (int j = 0; j < r.Height; j++) {
				if (i == 0 || j == 0 || i == r.Width - 1 || j == r.Height - 1) {
					CellLocation l = new CellLocation (r.leftCorner.x + 2 * i, r.leftCorner.y + 2 * j);
					borderFloors.Add (l);
				}
			}
		}

        // Create doors close to the borders, where wall passages are
		CellLocation target_passage;
		foreach(CellLocation l in borderFloors){
			foreach(VirtualMap.DirectionType dir in map.directions){
				target_passage = map.GetNeighbourCellLocation (l, dir);
				if (map.GetCell(target_passage).IsWall())
					CheckDoorCreation(map, r, l, dir);
			}
		}
	}
	
	// Control if we can open a door in the given direction. Open it if possible
	private bool CheckDoorCreation (VirtualMap map, VirtualRoom r, CellLocation start_floor_loc, VirtualMap.DirectionType direction)
	{
		bool result = false;
		CellLocation end_floor_loc = map.GetTargetLocation (start_floor_loc, direction);
        CellLocation passage = map.GetNeighbourCellLocation(start_floor_loc, direction);
        // We get the ending floor and check its validity
		if (end_floor_loc.isValid() && !map.GetCell(end_floor_loc).IsRock()){

            // We check whether we are connecting to another room
			if (map.roomCells.Contains (end_floor_loc)) {

				// Check if we skip creating the door
                if (DungeonGenerator.Random.Instance.Next(0, 100) > doorsDensityModifier // We do not skip if we request more doors than needed
                        && r.IsAlreadyConnectedToARoomAt(end_floor_loc,map)) // We skip if we are already connected
                    return result;

                OpenRoomDoor(map, r, start_floor_loc, end_floor_loc, passage, direction);
				
			} else {
				// We need one door for each corridor segment that has been separated out by this room
				//		To do that, we also create a door if the ending floor is a dead end, effectively getting rid of the dead end
				//		Also, we create if the ending floor is a rock (i.e. 4 walls), which can happen if this room blocks out single floor cells!

                // We check if we need to skip
                if (CheckSkipDoorToCorridor(map,r,end_floor_loc)) return result;

               // Debug.Log("Room " + r + " CREATING TO " + passage);

                OpenCorridorDoor(map, r, start_floor_loc, end_floor_loc, passage, direction);
				
			}

			result = true;

		}
		return result;
	}

    private bool CheckSkipDoorToCorridor(VirtualMap map, VirtualRoom r, CellLocation end_floor_loc)
    {
        // Note that 'dont skip' takes precedence!
        //Debug.Log(r + " to " + end_floor_loc);

        // At default, we do not skip
        bool skip = false; 

        // Already connected: we should skip it
        skip = r.IsConnectedToCorridor();
        //Debug.Log("Connected already? " + skip);

        // If 3 walls, we instead do not skip it
        skip = skip && !map.IsDeadEnd(end_floor_loc, alsoConsiderDoors:true);
        /*if (r.leftCorner.x == 9 && r.leftCorner.y == 3)
        {
            Debug.Log(r + " to " + end_floor_loc);
            Debug.Log("3 walls? " + map.IsDeadEnd(end_floor_loc, alsoConsiderDoors: true));
        }*/
                  
        // If 4 walls, we instead do not skip it
        skip = skip && !map.IsSurroundedByWalls(end_floor_loc);
        //Debug.Log("4 walls? " + map.IsSurroundedByWalls(end_floor_loc));

        // We do not skip if we request more doors
        skip = skip && !(DungeonGenerator.Random.Instance.Next(0, 100) < doorsDensityModifier);
        //Debug.Log("More doors? " + moreDoors);

        return skip;
    }


    /********************
     * Door Creation NEW
     ********************/

    public void OpenRoomDoor(VirtualMap map, VirtualRoom room, CellLocation start_floor_loc, CellLocation end_floor_loc, CellLocation passage_loc, VirtualMap.DirectionType direction)
    {
        //Debug.Log(room);
        // We update both this and the connected room
        foreach (VirtualRoom tr in map.rooms)
        {
            if (tr.containsLocation(end_floor_loc))
            {
                tr.AddDoorToAnotherRoom(passage_loc);
                room.AddDoorToAnotherRoom(passage_loc);
                tr.ConnectRoom(room);
                room.ConnectRoom(tr);
                break;
            } 
        }
        OpenDoor(map, start_floor_loc, end_floor_loc, passage_loc, direction);
    }

    public void OpenCorridorDoor(VirtualMap map, VirtualRoom room, CellLocation start_floor_loc, CellLocation end_floor_loc, CellLocation passage_loc, VirtualMap.DirectionType direction)
    {
        room.corridorExit++;
        room.corridorDoors.Add(passage_loc);
        OpenDoor(map, start_floor_loc, end_floor_loc, passage_loc, direction);
    }

    public void OpenDoor(VirtualMap map, CellLocation start_floor_loc, CellLocation end_floor_loc, CellLocation passage_loc, VirtualMap.DirectionType direction)
    {
        map.GetCell(passage_loc).Type = VirtualCell.CellType.Door;
        map.GetCell(passage_loc).Orientation = direction;

        // We also connect the cells
        map.ConnectCells(start_floor_loc, end_floor_loc);
    }
		
}
