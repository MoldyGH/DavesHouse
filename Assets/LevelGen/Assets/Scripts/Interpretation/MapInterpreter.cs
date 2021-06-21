using UnityEngine;
using System.Collections;

public abstract class MapInterpreter<T> : MapInterpreter where T:MapInterpreterBehaviour {
	public T behaviour;
	
	public void SetBehaviour(T behaviour){
		this.behaviour = behaviour;	
	}


	/*****************
	 * Building
	 *****************/
	override public void BuildMaps (VirtualMap[] maps)
	{
		// We build what we have defined in the virtual maps
		for(int storey=0; storey<maps.Length; storey++){
			VirtualMap map = maps[storey];
			for (int i = 0; i < map.Width; i++) {
				for (int j = 0; j <  map.Height; j++) {
					CellLocation loc = new CellLocation(i,j);
					VirtualCell cell = map.GetCell(loc);
					BuildObjectsOfCell(map,cell,storey);
				}
			}
		}
	}
	
	public void BuildObjectsOfCell(VirtualMap map, VirtualCell cell, int storey){
//		BuildObject(map,cell.starting_location,storey,cell.Type,cell.Orientation);
		//		foreach(VirtualCell.CellType t in cell.GetSubTypes()) BuildObject(map,cell.starting_location,storey,t,cell.Orientation);
		foreach(CellInstance ci in cell.GetCellInstances()) BuildObject(map,cell.location,storey,ci.type,ci.dir);
	}
	
	protected void BuildObject(VirtualMap map, CellLocation loc, int storey, VirtualCell.CellType type, VirtualMap.DirectionType dir){
		if (type == VirtualCell.CellType.None || type == VirtualCell.CellType.EmptyPassage) return;	// TODO: Maybe this check should be in the physical map
//				Debug.Log (loc + "  " + type);
		MetricLocation metricLocation = GetWorldLocation(loc,storey);
		physical_map.CreateObject(map,metricLocation,type,dir);
	}
	

	/*****************
	 * Conversions
	 *****************/

	// Standard
	protected void ConvertRock(VirtualMap map, VirtualCell conversion_cell){
		if (!behaviour.drawRocks) conversion_cell.Type = VirtualCell.CellType.None;
		else conversion_cell.Type = VirtualCell.CellType.Rock;
	}

	
	protected void ConvertFloor(VirtualMap map, VirtualCell conversion_cell, bool mayBeDirectional = true){
		CellLocation loc = conversion_cell.location;
		if(generator.mapDimensionsType == MapDimensionsType.THREE_DEE){
			// 3D case
			if (GeneratorValues.multiStorey){
				// Add ladders and avoid placing floors or ceilings for multi-storey
				if (map.start == loc && map.storey_number > 0){
					// No floor, just a ceiling
					ConvertCeiling(map,conversion_cell);
					conversion_cell.Type = VirtualCell.CellType.None;
				} else  if (map.end == loc && map.storey_number < GeneratorValues.numberOfStoreys-1){
					// Ladder up on top of a floor, no ceiling
					ConvertDirectionalFloor(map,conversion_cell, mayBeDirectional);
					conversion_cell.AddCellInstance(VirtualCell.CellType.Ladder,conversion_cell.Orientation);
				} else {
					ConvertDirectionalFloor(map,conversion_cell, mayBeDirectional);
					ConvertCeiling(map,conversion_cell);
				}
			} else {
				ConvertDirectionalFloor(map,conversion_cell, mayBeDirectional);
				ConvertCeiling(map,conversion_cell);
			}
		} else {
			// 2D case
			
			// We just check for directionality, if needed
			ConvertDirectionalFloor(map,conversion_cell, mayBeDirectional);
		}
	}
	
	public void ConvertDoor(VirtualMap map, VirtualCell conversion_cell){
		if(conversion_cell.IsDoor()) {
			if(!behaviour.drawDoors) conversion_cell.Type = VirtualCell.CellType.EmptyPassage;
			else {
				CellLocation prev_loc = map.GetNeighbourCellLocation(conversion_cell.location,conversion_cell.Orientation);
				CellLocation next_loc = map.GetNeighbourCellLocation(conversion_cell.location,map.GetDirectionOpposite(conversion_cell.Orientation));
//				Debug.Log (prev_loc);
//				Debug.Log (next_loc);
//				Debug.Log (map.GetCell(prev_loc).IsRoomFloor());
//				Debug.Log (map.GetCell(next_loc).IsRoomFloor());
				if (map.GetCell(prev_loc).IsRoomFloor() && map.GetCell(next_loc).IsRoomFloor()) conversion_cell.Type = VirtualCell.CellType.RoomDoor;
			}
		}
	}

	// This will convert 'None' cells to columns where necessary.
	protected bool ConvertColumn(VirtualMap map, VirtualCell conversion_cell, bool mayBeDirectional = true){
		CellLocation l = conversion_cell.location;
        //Debug.Log(l);
        bool isRoomColumn = false;
        bool isCorridorColumn = false;
        bool isPassageColumn = false;
		bool createColumn = true;

        if (map.IsColumnRemovable(l, behaviour.drawWallCorners, behaviour.createColumnsInRooms))
        {
            //Debug.Log(conversion_cell.location + " Is removable!");
			createColumn = false;
        }
        else
        {
            //Debug.Log(conversion_cell.location + " Is not removable!");

            // We check all neighs to determine what type of column this is
			foreach(VirtualMap.DirectionType dir in map.directions){
				CellLocation neigh_loc = map.GetNeighbourCellLocation(l,dir);
				if (!map.LocationIsOutsideBounds(neigh_loc)){
					VirtualCell neigh_cell = map.GetCell(neigh_loc);

                    //Debug.Log("CHECK " + neigh_cell.location + " TYPE " + neigh_cell.Type);
					
					if (neigh_cell.IsDoor()){
                        conversion_cell.Type = VirtualCell.CellType.PassageColumn;
                        isPassageColumn = true;
						break;
					} else if (!isRoomColumn && neigh_cell.IsCorridorWall()){
						conversion_cell.Type = VirtualCell.CellType.CorridorColumn;
                        isCorridorColumn = true;
                        // Do not break, as we need to check all the other walls to be sure
					}  else if (neigh_cell.IsRoomWall()){
						conversion_cell.Type = VirtualCell.CellType.RoomColumn;
						isRoomColumn = true;
						// Do not break, as we need to check all the other walls to be sure
                    }

				}
			}

        }

        // This may be surrounded by floors!
        if (createColumn &&
            (!isRoomColumn && !isCorridorColumn && !isPassageColumn))
        {
            if (map.IsInRoom(l))
            {
                if (behaviour.createColumnsInRooms)
                {
                    conversion_cell.Type = VirtualCell.CellType.InsideRoomColumn;
                }
                else
                {
                    conversion_cell.Type = VirtualCell.CellType.RoomFloorInside;
                }
            }
            else
            {
                // NOT IN ROOM: THIS IS EITHER SURROUNDED BY ROCKS OR BY CORRIDORS!!
                // We check all neighbours to make sure
			    /*foreach(VirtualMap.DirectionType dir in map.directions){
				    CellLocation neigh_loc = map.GetNeighbourCellLocationOfSameType(l,dir);
				    if (!map.LocationIsOutsideBounds(neigh_loc)){
					    VirtualCell neigh_cell = map.GetCell(neigh_loc);
                    }*/
                conversion_cell.Type =  VirtualCell.CellType.CorridorColumn; 
            }
            //Debug.Log("ROOM COL? " + conversion_cell.Type);
        }


		// Directional column
		if (createColumn) ConvertDirectionalColumn(map,conversion_cell, mayBeDirectional);
		
		// If the column is not created, we disable it
		if (!createColumn) conversion_cell.Type = VirtualCell.CellType.None;
		
		return createColumn;
	}


	// 3D
	protected void ConvertCeiling(VirtualMap map, VirtualCell conversion_cell){
		// Checking if we need to add a ceiling
		VirtualCell.CellType cell_type = conversion_cell.Type;
        //Debug.Log(conversion_cell);
		if (conversion_cell.IsFloor()){
			VirtualCell.CellType ceiling_type = VirtualCell.CellType.None;
			
			if (VirtualCell.IsRoomFloor(cell_type)){
				if (behaviour.addCeilingToRooms){
					ceiling_type = VirtualCell.CellType.RoomCeiling;
				}
			} else {
				if (behaviour.addCeilingToCorridors){
					ceiling_type = VirtualCell.CellType.CorridorCeiling;	
				}	
			}
			
			if (ceiling_type != VirtualCell.CellType.None){
				conversion_cell.AddCellInstance(ceiling_type,conversion_cell.Orientation);
			}
		}
	}
	

	
	// Directional floors
	public void ConvertDirectionalFloor(VirtualMap map, VirtualCell conversion_cell, bool mayBeDirectional){
		if (!mayBeDirectional) return;
		if (conversion_cell.IsCorridorFloor()) ConvertDirectionalCorridorFloor(map,conversion_cell);
		else if (conversion_cell.IsRoomFloor()) ConvertDirectionalRoomFloor(map,conversion_cell);
	}
	
	public void ConvertDirectionalCorridorFloor(VirtualMap map, VirtualCell conversion_cell){
		CellLocation l = conversion_cell.location;
		
		if (behaviour.useDirectionalFloors){
			if (conversion_cell.IsCorridorFloor()){
				
				// Count how many border neighbours are non-walls
				int countFloorNeighs = 0;
				bool[] validIndices = new bool[4];
				
				if (conversion_cell.IsTile()){
					// This was a tile, check neigh walls
					CellLocation[] border_neighs = map.GetAllNeighbours(l);
					for(int i=0; i<border_neighs.Length; i++){
						CellLocation other_l = border_neighs[i];
						if (!map.LocationIsOutsideBounds(other_l)  && other_l.isValid() && !(map.GetCell(other_l).IsWall())){	// TODO: Maybe isValid is not needed!
							countFloorNeighs++; 
							validIndices[i] = true;
						}
					}
				} else {
					// This was a border, check neigh floors instead
					CellLocation[] floor_neighs = map.GetAllNeighbours(l);
					for(int i=0; i<floor_neighs.Length; i++){
						CellLocation other_l = floor_neighs[i];
						if (!map.LocationIsOutsideBounds(other_l)  && other_l.isValid() && map.GetCell(other_l).IsFloor()){		// TODO: Maybe isValid is not needed!
							countFloorNeighs++; 
							validIndices[i] = true;
						}
					}
				}
				
				// Define the adbvanced floors
				if (countFloorNeighs == 1){
					conversion_cell.Type = VirtualCell.CellType.CorridorFloorU;
					for(int i= 0; i<4; i++) {
						if (validIndices[i]){
							conversion_cell.Orientation = map.directions[(int)Mathf.Repeat(i+3,4)];
							break;
						}
					}
				}
				else if (countFloorNeighs == 2){
					
					// Corridor I
					conversion_cell.Type = VirtualCell.CellType.CorridorFloorI;
					for(int i= 0; i<4; i++) {
						if (validIndices[i]){
							conversion_cell.Orientation = map.directions[(int)Mathf.Repeat(i+1,4)];
							break;
						}
					}
					
					// Corridor L
					for(int i= 0; i<4; i++) {
						if (validIndices[i] && validIndices[(int)Mathf.Repeat(i+1,4)] ){
							// This and the next are valid: left turn (we consider all of them to be left turns(
							conversion_cell.Orientation = map.directions[(int)Mathf.Repeat(i+3,4)];
							conversion_cell.Type = VirtualCell.CellType.CorridorFloorL;
							break;
						}
					}
				}
				else if (countFloorNeighs == 3) {
					conversion_cell.Type = VirtualCell.CellType.CorridorFloorT;
					for(int i= 0; i<4; i++) {
						if (validIndices[(int)Mathf.Repeat(i-1,4)] && validIndices[i] && validIndices[(int)Mathf.Repeat(i+1,4)]) {
							// This, the one before and the next are valid: T cross (with this being the middle road)
							conversion_cell.Orientation = map.directions[(int)Mathf.Repeat(i+1,4)];
							break;
						}
					}
				}
				else if (countFloorNeighs == 4) {
					conversion_cell.Type = VirtualCell.CellType.CorridorFloorX;
				}
			}
		}
	}
	
	
	public void ConvertDirectionalRoomFloor(VirtualMap map, VirtualCell conversion_cell){
		CellLocation l = conversion_cell.location;
		
		if (behaviour.useDirectionalFloors){
			if (conversion_cell.IsRoomFloor()){
				CellLocation[] border_neighs;
				CellLocation[] floor_neighs;
				
				bool considerDoorsAsWalls = true;
				
				// Count how many border neighbours are non-walls 
				int countFloorNeighs = 0;
				bool[] validIndices = new bool[4];
				
				if (conversion_cell.IsTile()){
					// This was a tile, check neigh walls
					border_neighs = map.GetAllNeighbours(l);
					for(int i=0; i<border_neighs.Length; i++){
						CellLocation other_l = border_neighs[i];
						if (!map.LocationIsOutsideBounds(other_l)  && other_l.isValid() 
						    && !(map.GetCell(other_l).IsWall())
						    && !(considerDoorsAsWalls && map.GetCell(other_l).IsDoor())
						    ){
							countFloorNeighs++; 
							validIndices[i] = true;
						}
					}
				} else {
					// This was a border, check neigh floors instead
					floor_neighs = map.GetAllNeighbours(l);
					//					Debug.Log ("From " + l);None

					for(int i=0; i<floor_neighs.Length; i++){
						CellLocation other_l = floor_neighs[i];
						//						Debug.Log ("At " + other_l + " is " + map.GetCell(other_l).Type);
						bool insideRoomTile = CheckInsideRoomTile(map,other_l);	// We need this to be checked now, or we cannot know if a tile is inside a room reliably
						if (!map.LocationIsOutsideBounds(other_l)  && other_l.isValid() && 
						    (map.GetCell(other_l).IsFloor()  //|| map.GetCell(other_l).IsNone()
						 || map.GetCell(other_l).IsInsideRoomColumn()	// Treat inside room columns as floors here 
						 || insideRoomTile
//						 || map.GetCell(other_l).IsNone()	
						    )){
							countFloorNeighs++; 
							validIndices[i] = true;
						}
					}
				}
				
				
				// Define the adbvanced floors
				//	Debug.Log (countFloorNeighs);
				if (countFloorNeighs == 2){
                    bool adjacentFloors = false;

					// This is a room corner
					for(int i= 0; i<4; i++) {
						if (validIndices[i] && validIndices[(int)Mathf.Repeat(i+1,4)] ){
							conversion_cell.Orientation = map.directions[(int)Mathf.Repeat(i+3,4)];
                            adjacentFloors = true;
							break;
						}
					}

                    if (adjacentFloors) conversion_cell.Type = VirtualCell.CellType.RoomFloorCorner;
                    else conversion_cell.Type = VirtualCell.CellType.RoomFloorInside;
				}
				else if (countFloorNeighs == 3) {
					conversion_cell.Type = VirtualCell.CellType.RoomFloorBorder;
					for(int i= 0; i<4; i++) {
						if (validIndices[(int)Mathf.Repeat(i-1,4)] && validIndices[i] && validIndices[(int)Mathf.Repeat(i+1,4)]) {
							conversion_cell.Orientation = map.directions[(int)Mathf.Repeat(i+2,4)];
							break;
						}
					}
				}
				else if (countFloorNeighs == 4) {
					conversion_cell.Type = VirtualCell.CellType.RoomFloorInside;
				} else {
					// Wrong number of floor neighs, may happen if we have too small rooms. We always use the INSIDE one, then.
					conversion_cell.Type = VirtualCell.CellType.RoomFloorInside; 
				}
				
			}
		}
	}
	
	private bool CheckInsideRoomTile(VirtualMap map, CellLocation loc){
		CellLocation[] neigh_locs = map.GetAllNeighbours(loc);
		int countRoomFloors = 0;
		foreach(CellLocation nl in neigh_locs){
			if(!map.LocationIsOutsideBounds(nl) 
			   && map.GetCell(nl).IsRoomFloor()) countRoomFloors++;
		}
		return countRoomFloors == 4;
	}


	// Directional walls/columns
	public void ConvertDirectionalColumn(VirtualMap map, VirtualCell conversion_cell, bool mayBeDirectional){
		if(!mayBeDirectional) return;
		if (conversion_cell.IsCorridorColumn()) ConvertDirectionalCorridorColumn(map,conversion_cell);
		else if (conversion_cell.IsRoomColumn()) ConvertDirectionalRoomColumn(map,conversion_cell);
	}
	
	public void ConvertDirectionalCorridorColumn(VirtualMap map, VirtualCell conversion_cell){
		CellLocation l = conversion_cell.location;
		if (behaviour.useDirectionalFloors){
			// Count how many border neighbours are walls
			int countWallNeighs = 0;
			bool[] validIndices = new bool[4];
			
			// This was a 'tile', check neigh walls 
			CellLocation[] border_neighs = map.GetAllNeighbours(l);
			for(int i=0; i<border_neighs.Length; i++){
				CellLocation other_l = border_neighs[i];
				if (!map.LocationIsOutsideBounds(other_l) // && other_l.isValid()
				    ){ 	// TODO: Maybe isValid is not needed!
					VirtualCell other_cell = map.GetCell(other_l);
					if (other_cell.IsWall() && !map.IsPassageRemovable(other_cell.location)){
						countWallNeighs++; 
						validIndices[i] = true;
					}
				}
			}
			
			// Define the advanced tile to use
			// TODO: merge this with the one for directional floors somehow!
			if (countWallNeighs == 0){
				conversion_cell.Type = VirtualCell.CellType.CorridorWallO;
			} 
			else if (countWallNeighs == 1){
				conversion_cell.Type = VirtualCell.CellType.CorridorWallU;
				for(int i= 0; i<4; i++) {
					if (validIndices[i]){
						conversion_cell.Orientation = map.directions[(int)Mathf.Repeat(i+3,4)];
						break;
					}
				}
			}
			else if (countWallNeighs == 2){
				// Corridor I
				conversion_cell.Type = VirtualCell.CellType.CorridorWallI;
				for(int i= 0; i<4; i++) {
					if (validIndices[i]){
						conversion_cell.Orientation = map.directions[(int)Mathf.Repeat(i+1,4)];
						break;
					}
				}
				
				// Corridor L
				for(int i= 0; i<4; i++) {
					if (validIndices[i] && validIndices[(int)Mathf.Repeat(i+1,4)] ){
						// This and the next are valid: left turn (we consider all of them to be left turns(
						conversion_cell.Orientation = map.directions[(int)Mathf.Repeat(i+3,4)];
						conversion_cell.Type = VirtualCell.CellType.CorridorWallL;
						break;
					}
				}
			}
			else if (countWallNeighs == 3) {
				conversion_cell.Type = VirtualCell.CellType.CorridorWallT;
				for(int i= 0; i<4; i++) {
					if (validIndices[(int)Mathf.Repeat(i-1,4)] && validIndices[i] && validIndices[(int)Mathf.Repeat(i+1,4)]) {
						// This, the one before and the next are valid: T cross (with this being the middle road)
						conversion_cell.Orientation = map.directions[(int)Mathf.Repeat(i+1,4)];
						break;
					}
				}
			}
			else if (countWallNeighs == 4) {
				conversion_cell.Type = VirtualCell.CellType.CorridorWallX;
			}
		}
	}

	
	public void ConvertDirectionalRoomColumn(VirtualMap map, VirtualCell conversion_cell){
		CellLocation l = conversion_cell.location;
		if (behaviour.useDirectionalFloors){
			CellLocation[] border_neighs;

			bool considerDoorsAsWalls = true;
			
			// Count how many border neighbours are room walls 
			int countWallNeighs = 0;
			bool[] validIndices = new bool[4];
			
			// This was a 'tile', check neigh walls
			border_neighs = map.GetAllNeighbours(l);
			for(int i=0; i<border_neighs.Length; i++){
				CellLocation other_l = border_neighs[i];
				if (!map.LocationIsOutsideBounds(other_l) 
				    && 
				    (((behaviour.isolateDirectionalWallsForRooms && map.GetCell(other_l).IsRoomWall())
				     || (!behaviour.isolateDirectionalWallsForRooms && map.GetCell(other_l).IsWall()))
				    || (considerDoorsAsWalls && map.GetCell(other_l).IsDoor()))
                    && !map.IsPassageRemovable(other_l)   // Make sure the wall is not being removed!
				    ){
                    //Debug.Log(l + " -  " + other_l + " " + map.GetCell(other_l).Type);
                    countWallNeighs++; 
					validIndices[i] = true;
				}
			}
			
			
			// Define the adbvanced tiles
			//Debug.Log ("Cell " + l + " has neigh walls " + countWallNeighs);
			if (countWallNeighs == 0){
				conversion_cell.Type = VirtualCell.CellType.RoomWallO;
			} else if (countWallNeighs == 1){
				conversion_cell.Type = VirtualCell.CellType.RoomWallU;
			} else if (countWallNeighs == 2){
				// Wall I
				conversion_cell.Type = VirtualCell.CellType.RoomWallI;
                //Debug.Log("SETTING " + l + " TO I");
				for(int i= 0; i<4; i++) {
					if (validIndices[i]){
						conversion_cell.Orientation = map.directions[(int)Mathf.Repeat(i+1,4)];
						break;
					}
				}
				
				// Wall L
				for(int i= 0; i<4; i++) {
					if (validIndices[i] && validIndices[(int)Mathf.Repeat(i+1,4)] ){
						// This and the next are valid: left turn (we consider all of them to be left turns(
						conversion_cell.Orientation = map.directions[(int)Mathf.Repeat(i+3,4)];
						conversion_cell.Type = VirtualCell.CellType.RoomWallL;
						break;
					}
				}
			}
			else if (countWallNeighs == 3) {
				conversion_cell.Type = VirtualCell.CellType.RoomWallT;
				for(int i= 0; i<4; i++) {
					if (validIndices[(int)Mathf.Repeat(i-1,4)] && validIndices[i] && validIndices[(int)Mathf.Repeat(i+1,4)]) {
						conversion_cell.Orientation = map.directions[(int)Mathf.Repeat(i+1,4)];
						break;
					}
				}
			}
			else if (countWallNeighs == 4) {
				conversion_cell.Type = VirtualCell.CellType.RoomWallX;
			} 
		}
	}


	// Wall
	protected bool ConvertWall(VirtualMap map, VirtualCell conversion_cell){
		if (behaviour.useDirectionalFloors){
			if (conversion_cell.IsCorridorWall()) conversion_cell.Type = VirtualCell.CellType.CorridorWallI;
			else conversion_cell.Type = VirtualCell.CellType.RoomWallI;
			ConvertWallOrientation(map,conversion_cell);
		}
		return ConvertPerimeterWall(map,conversion_cell);	// Returns whether it is a perimeter or not
	}
	
	// Perimeter
	protected bool ConvertPerimeterWall(VirtualMap map, VirtualCell conversion_cell){
		if (!behaviour.usePerimeter) return false;
		if (map.LocationIsInPerimeter(conversion_cell.location)){
			conversion_cell.Type = VirtualCell.CellType.PerimeterWall;

			if (conversion_cell.location.x == 0) 					conversion_cell.Orientation = VirtualMap.DirectionType.East;
			else if (conversion_cell.location.x == map.Width-1)  	conversion_cell.Orientation = VirtualMap.DirectionType.West;
			else if (conversion_cell.location.y == 0)  				conversion_cell.Orientation = VirtualMap.DirectionType.North;
			else if (conversion_cell.location.y == map.Height-1) 	conversion_cell.Orientation = VirtualMap.DirectionType.South;
			return true;
		}
		return false;
	}
	
	protected bool ConvertPerimeterColumn(VirtualMap map, VirtualCell conversion_cell){
		if (!behaviour.usePerimeter) return false;
		if (behaviour.internalPerimeter){
			// Internal perimeter: we check that we have nothing towards the outside
			// TODO:
		} else {
			// External perimeter: we place a perimeter around the whole map
			if (map.LocationIsInPerimeter(conversion_cell.location)){
				conversion_cell.Type = VirtualCell.CellType.PerimeterColumn;
				return true;
			}
		}
		return false;
	}
	

	
	// Various
	protected void ConvertWallOrientation(VirtualMap map, VirtualCell conversion_cell){
		// Fix orientation of walls so that they follow the floors orientation
		conversion_cell.Orientation = VirtualMap.DirectionType.North;
		CellLocation neigh_loc = map.GetNeighbourCellLocation(conversion_cell.location,VirtualMap.DirectionType.North);
		if (!map.LocationIsOutsideBounds(neigh_loc)){
			VirtualCell neigh_cell = map.GetCell(neigh_loc);
			if (neigh_cell.IsNone()){
				conversion_cell.Orientation = VirtualMap.DirectionType.East;
			}
		}
	}
	
	/************
	 * Checks
	 ************/

	protected void CheckOrientationRandomization(VirtualMap map, VirtualCell conversion_cell){
		if (conversion_cell.Orientation == VirtualMap.DirectionType.None &&
		    behaviour.randomOrientations)	{
			conversion_cell.Orientation = (VirtualMap.DirectionType) DungeonGenerator.Random.Instance.Next(map.nDirections);
		}
//		Debug.Log (conversion_cell.Orientation);
	}
	
	protected bool CheckWallRendering(VirtualMap map, VirtualCell conversion_cell){
		// Check wheter we need to show a wall or not for this type of map
		if (conversion_cell.Type == VirtualCell.CellType.EmptyPassage) return false;
		if (behaviour.usePerimeter && conversion_cell.Type == VirtualCell.CellType.PerimeterWall) return true;

        //Debug.Log("Check rendering of wall " + conversion_cell.location);
        if (map.IsPassageRemovable(conversion_cell.location))
        {
            //Debug.Log("NO RENDER");
            // Do not render this wall!
            conversion_cell.Type = VirtualCell.CellType.EmptyPassage;
            return false;
        }
        else
        {
            // This wall must be rendered!
            return true;
        }
		
        /*
		// We check the neighbours. A rendered wall needs at least one floor nearby.
		CellLocation[] floor_neighs = map.GetAllNeighbours(conversion_cell.starting_location);
		int countFloorNeighs = 0;
		for(int n_i=0; n_i<floor_neighs.Length; n_i++){
			CellLocation other_l = floor_neighs[n_i];
			if (!map.LocationIsOutsideBounds(other_l) && map.GetCell(other_l).IsFloor()){
				countFloorNeighs++; 
			}
		}
		if (countFloorNeighs > 0){
			// This wall must be rendered!
			return true;
		} else {
			// Do not render this wall!
			conversion_cell.Type = VirtualCell.CellType.EmptyPassage;
			return false;
		}*/
	}
	

	/************
	 * Additions
	 ************/

	private int nStairs = 0;
	protected void AddStairsToRooms(VirtualMap currentMap, VirtualMap[] maps, int storey){
		if (storey == 0) return; // Do not add at the first one! We'll add stairs from top to bottom.

		bool allowStairsCloseToDoors = false;
		
//		Debug.Log ("CHECKING STAIRS FOR STOREY " + storey);
		// Stairs are added if we have two consecutive floors both at storey i and i+1
		foreach(CellLocation l in currentMap.roomCells){
			VirtualCell cell_here = currentMap.GetCell(l);

			if (cell_here.location == currentMap.end || cell_here.location == currentMap.start) continue;	// Not on the start/end cells

			foreach(VirtualMap.DirectionType direction in currentMap.directions){
				CellLocation next_l = currentMap.GetNeighbourCellLocationAtStep(l,direction,this.TileSeparationSteps);
				if (currentMap.LocationIsOutsideBounds(next_l)) continue;
				if (next_l == currentMap.end || next_l == currentMap.start) continue;	// Not on the start/end cells

				VirtualCell cell_next = currentMap.GetCell (next_l);
//				Debug.Log ("Cell here: " + cell_here.starting_location + " is " + cell_here.Type + " and next: " + cell_next.starting_location + " is " + cell_next.Type);
				
				if (VirtualCell.IsRoomFloor(cell_here.Type) && VirtualCell.IsRoomFloor(cell_next.Type)){
					if (!currentMap.CellsAreInTheSameRoom(cell_here.location,cell_next.location))  continue;
					// Two consecutive floors! Check the below map as well
//				    Debug.Log ("DOUBLE FLOORS! " + storey);
					if (!allowStairsCloseToDoors && (currentMap.HasAdjacentDoor(cell_here.location) || currentMap.HasAdjacentDoor(cell_next.location))) continue;

					VirtualMap belowMap = maps[storey-1];
					if (belowMap.GetCell(l).IsRoomFloor() && belowMap.GetCell(next_l).IsRoomFloor()){
						if (l == belowMap.end || l == belowMap.start) continue;	// Not on the start/end cells
						if (next_l == belowMap.end || next_l == belowMap.start) continue;	// Not on the start/end cells
						if (!belowMap.CellsAreInTheSameRoom(cell_here.location,cell_next.location)) continue;
						// Also below! This is a two-tile stair! Update the map!

						if (!allowStairsCloseToDoors && (currentMap.HasAdjacentDoor(cell_here.location) || currentMap.HasAdjacentDoor(cell_next.location))) continue;

						// We place the stair below
						belowMap.GetCell(l).AddCellInstance(VirtualCell.CellType.Ladder2,direction);

						// We remove any ceiling below
						belowMap.GetCell(l).RemoveCellInstancesOfTypesInSelection(SelectionObjectType.Ceilings);
						belowMap.GetCell(next_l).RemoveCellInstancesOfTypesInSelection(SelectionObjectType.Ceilings);
						
						// We override the current map by removing its floors
						currentMap.GetCell(l).RemoveCellInstancesOfTypesInSelection(SelectionObjectType.Floors);
						currentMap.GetCell(next_l).RemoveCellInstancesOfTypesInSelection(SelectionObjectType.Floors);
						
						nStairs++;
						if (nStairs > 0) return; // At most one stair
					}
				}
			}
		}
	}

}

public abstract class MapInterpreter {

	// Number of steps between two same-type tiles. Standard: 2 (a wall in-between)
	virtual protected int TileSeparationSteps{
		get{return 2;}
	}
	
	protected VirtualMap[] virtual_maps;
	protected PhysicalMap physical_map;
	protected GeneratorBehaviour generator;

	virtual public void Initialise(VirtualMap[] virtual_maps, PhysicalMap physical_map, GeneratorBehaviour generator){
		this.virtual_maps = virtual_maps;
		this.physical_map = physical_map;
		this.generator = generator;
	}
	abstract public void ReadMaps(VirtualMap[] maps);		// Read the maps and interpret them
	abstract public void BuildMaps(VirtualMap[] maps);		// Build the interpreted maps
	
	abstract public MetricLocation GetWorldLocation(CellLocation l, int storey);
	
	
	// This defaults to an orientation plane of XZ
	public Vector3 GetWorldPosition(CellLocation l, int storey){	
		MetricLocation world_location = GetWorldLocation(l,storey);
		return new Vector3(world_location.x,storey,world_location.y);
	}
	
}