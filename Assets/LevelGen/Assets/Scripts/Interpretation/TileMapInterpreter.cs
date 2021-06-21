using UnityEngine;
using System.Collections;

public class TileMapInterpreter : MapInterpreter<TileMapInterpreterBehaviour> {

	override public void ReadMaps (VirtualMap[] maps)
	{
		for(int storey=0; storey<maps.Length; storey++){
			VirtualMap map = maps[storey];
			PerformTilemapConversion(map,maps,storey);
			PerformSpecificConversions(map,maps,storey);
        }

		// Additional modifications, multi-storey
		for(int storey=0; storey<maps.Length; storey++){
			VirtualMap map = maps[storey];
			AddStairsToRooms(map,maps,storey);
		}

		// Fake 3D 
		if (behaviour.useFake3DWalls){
			for(int storey=0; storey<maps.Length; storey++){
				VirtualMap map = maps[storey];
				Fake3DEffect(map,maps,storey);
			}
		}
	}

	
	// Change all tiles that have a wall below them to a new 'highwall' tile
	public void Fake3DEffect(VirtualMap map, VirtualMap[] maps, int storey){		
		VirtualCell conversion_cell;
		VirtualCell[,] output_cells = new VirtualCell[map.Width,map.Height];
		
		for (int i = 0; i < map.Width; i++){
			for (int j = 0; j < map.Height; j++){
				CellLocation loc = new CellLocation(i,j);
				conversion_cell = VirtualCell.Copy(map.GetCell(loc));
				ConvertFake3DEffect(map,conversion_cell);
				output_cells[i,j] = conversion_cell;
			}
		}
		
		// We can now override the initial map!
		for (int i = 0; i < map.Width; i++){
			for (int j = 0; j <  map.Height; j++){
				map.SetCell(i,j,output_cells[i,j]);
			}
		}
	}
	protected void ConvertFake3DEffect(VirtualMap map, VirtualCell conversion_cell){
		CellLocation below_loc = map.GetNeighbourCellLocation (conversion_cell.location, VirtualMap.DirectionType.South);
		if ((conversion_cell.IsColumn () || conversion_cell.IsWall ())) {
			// If we have a wall below and this is a wall, we transform this to a special wall 
			bool isAbove = false;
			VirtualCell below_cell = null;
			if (!map.LocationIsOutsideBounds (below_loc)){
				 below_cell = map.GetCell(below_loc);
				if (below_cell.IsColumn() || below_cell.IsWall() || below_cell.IsRock()){
					isAbove = true;
				} else {
					isAbove = false;
				}
			} else {
				isAbove = true;
			}
			
			if (isAbove){
				if (conversion_cell.IsRoom()){
					conversion_cell.Type = VirtualCell.CellType.Fake3D_Room_WallAbove;
				} else {
					conversion_cell.Type = VirtualCell.CellType.Fake3D_Corridor_WallAbove;
				}
			} else {
				if (conversion_cell.IsRoom()){
					conversion_cell.Type = VirtualCell.CellType.Fake3D_Room_WallFront;
					
//					// Also, we add this to make sure the doors work correctly
//					if (below_cell.IsDoor()){
//						conversion_cell.AddCellInstance(VirtualCell.CellType.DoorHorizontalTop,below_cell.Orientation);
//					}

				} else {
					conversion_cell.Type = VirtualCell.CellType.Fake3D_Corridor_WallFront;
				}
			}

			conversion_cell.Orientation = VirtualMap.DirectionType.West; // Force orientation

		} else if (conversion_cell.IsDoor()){
			if (conversion_cell.IsHorizontal()){
				conversion_cell.Type = VirtualCell.CellType.DoorHorizontalBottom;
			} else {
				conversion_cell.Type = VirtualCell.CellType.DoorVertical;
			}
		}

	}



	// The first pass will fill the tilemap intersection cells, so that it can be used similarly to the standard map interpreter
	private void PerformTilemapConversion(VirtualMap map, VirtualMap[] maps, int storey){
		VirtualCell conversion_cell;
		VirtualCell[,] output_cells = new VirtualCell[map.Width,map.Height];

		for (int i = 0; i < map.Width; i++){
			for (int j = 0; j < map.Height; j++){
				CellLocation loc = new CellLocation(i,j);
				conversion_cell = VirtualCell.Copy(map.GetCell(loc));
				ConvertTileIntersections(map,conversion_cell);
				output_cells[i,j] = conversion_cell;
			}
		}
		
		// We can now override the initial map!
		for (int i = 0; i < map.Width; i++){
			for (int j = 0; j <  map.Height; j++){
				map.SetCell(i,j,output_cells[i,j]);
//				Debug.Log (map.cells[i,j].starting_location + ": " + map.cells[i,j].Type);
			}
		}
	}


	
	// The second pass will update the different tiles as needed
	private void PerformSpecificConversions(VirtualMap map, VirtualMap[] maps, int storey){
		VirtualCell conversion_cell;
		VirtualCell[,] output_cells = new VirtualCell[map.Width,map.Height];
	                                        
		for (int i = 0; i <  map.Width; i++){
			for (int j = 0; j <  map.Height; j++)	{
				CellLocation loc = new CellLocation(i,j);
				conversion_cell = VirtualCell.Copy(map.GetCell(loc));
                //Debug.Log("CHECKING " + conversion_cell);
				
				if (conversion_cell.IsFloor()){
					ConvertFloor(map,conversion_cell);
				}
				else if (conversion_cell.IsDoor()){
					ConvertDoor(map,conversion_cell);

					CheckWallRendering(map,conversion_cell);
//					if (renderWall) ConvertFixedWallOrientation(map,conversion_cell);
				
					AddFillingFloors(map,conversion_cell);
				} 
				else if (conversion_cell.IsWall()){
					bool isPerimeter = ConvertWall(map,conversion_cell);

					bool renderWall = CheckWallRendering(map,conversion_cell);
					if (renderWall) {
						if (!isPerimeter || (isPerimeter && !behaviour.orientPerimeter)) ConvertFixedWallOrientation(map,conversion_cell);
					} 	
					else ConvertRock(map,conversion_cell);	// May be a rock

					AddFillingFloors(map,conversion_cell);
				}
                else if (conversion_cell.IsNone())
                {
                    //Debug.Log("NONE CELL " + conversion_cell);
					// 'None' cells are usually converted to columns
					bool isColumn = ConvertColumn(map,conversion_cell);
					bool isPerimeter = ConvertPerimeterColumn(map,conversion_cell);
					ConvertFixedWallOrientation(map,conversion_cell);	// Also columns need to be orientated correctly
					
					// If not a column, check what this is
					if (!isColumn && !isPerimeter){ 
                        // Usually a rock
                        ConvertRock(map,conversion_cell);
                    }

                    bool isActuallyFloor = false;
                    // Also, an inside-room tile may be transformed into a floor
                    if (!isPerimeter){
                        if (conversion_cell.IsFloor()){//CheckRoom(map,conversion_cell.location)){
                            //Debug.Log("INSIDE!");
							AddToCorrectRoom(map,conversion_cell.location);
							conversion_cell.Type = VirtualCell.CellType.RoomFloor;
							ConvertFloor(map,conversion_cell);
                            isActuallyFloor = true;
						}
                    }

                    if (!isActuallyFloor) AddFillingFloors(map, conversion_cell);


				} else if (conversion_cell.IsRock()){
					// We may draw rocks
					ConvertRock(map,conversion_cell);
					
					AddFillingFloors(map,conversion_cell);
				}

				
				// Final pass on orientation randomization
				CheckOrientationRandomization(map,conversion_cell);

				// We save it in the initial map
				output_cells[i,j] = conversion_cell;
				
			}
		}
		
		// We can now override the initial map!
		for (int i = 0; i < map.Width; i++){
			for (int j = 0; j <  map.Height; j++){
				map.SetCell(i,j,output_cells[i,j]);
			}
		}
	}
	

	protected void ConvertFixedWallOrientation(VirtualMap map, VirtualCell conversion_cell){
		if (!behaviour.randomOrientations && !behaviour.useDirectionalFloors) conversion_cell.Orientation = VirtualMap.DirectionType.North;
	}


	protected void ConvertTileIntersections(VirtualMap map, VirtualCell conversion_cell){

		if(conversion_cell.IsEmpty()){			
			// Empty passages should be filled with floors, unless removable (and thus become rock)
            if (map.IsPassageRemovable(conversion_cell.location)) conversion_cell.Type = VirtualCell.CellType.Rock;
            else
            {
                if (CheckRoom(map, conversion_cell.location))
                {
                    AddToCorrectRoom(map, conversion_cell.location);
                    conversion_cell.Type = VirtualCell.CellType.RoomFloor;
                }
                else
                    conversion_cell.Type = VirtualCell.CellType.CorridorFloor;
            }
		} else {
			// 'None' cells will remain as such, since they will be converted to columns later (or to floors), if needed.
		}
	}


	protected void AddFillingFloors(VirtualMap map, VirtualCell conversion_cell){
		if (conversion_cell.Type == VirtualCell.CellType.None) return;

	    // Fill with floors
		VirtualCell.CellType initial_type = conversion_cell.Type;
		VirtualMap.DirectionType initial_dir = conversion_cell.Orientation;
		bool addedFloor = false;
	    if (behaviour.fillWithFloors){
			bool mayBeDirectional = false;
			if (CheckRoom(map,conversion_cell.location)){
				if (conversion_cell.IsDoor() || conversion_cell.IsEmpty()) {
					AddToCorrectRoom(map,conversion_cell.location);
					conversion_cell.Type = VirtualCell.CellType.RoomFloor;
					ConvertDirectionalRoomFloor(map,conversion_cell);
					mayBeDirectional = true;
				} else {
					conversion_cell.Type = VirtualCell.CellType.RoomFloor;
				}
			} else {
				if (conversion_cell.IsDoor() || conversion_cell.IsEmpty()){
					conversion_cell.Type = VirtualCell.CellType.CorridorFloor;
					ConvertDirectionalCorridorFloor(map,conversion_cell);  
					mayBeDirectional = true;
				} else {
					conversion_cell.Type = VirtualCell.CellType.CorridorFloor;
				}
			}
			ConvertFloor(map,conversion_cell,mayBeDirectional);
			addedFloor = true;
		} else {
			// Special case: when not filling with floors AND we do not draw doors, we still need to place a floor underneath the empty passage representing the doors!
			if(conversion_cell.IsEmpty()){		// Decomment this if you want floors underneath doors ALWAYS: // || input_cell.IsDoor()){
				conversion_cell.Type = VirtualCell.CellType.CorridorFloor;
				ConvertDirectionalCorridorFloor(map,conversion_cell);
				ConvertFloor(map,conversion_cell);
				addedFloor = true;
			}
		}

		if (addedFloor){
			// The initial type is switched in, the floor becomes a subtype
			VirtualCell.CellType new_subtype = conversion_cell.Type;
			VirtualMap.DirectionType new_dir = conversion_cell.Orientation;
			conversion_cell.Type = initial_type;
			conversion_cell.Orientation = initial_dir;
			conversion_cell.AddCellInstance(new_subtype,new_dir);
		}
	}

	private bool ShouldBeFilled(VirtualCell.CellType type){
		return !(type == VirtualCell.CellType.CorridorFloor || type==VirtualCell.CellType.RoomFloor);
	}
	
	// True if this starting_location belongs to a room's borderFloors
	private bool CheckRoomBorder(VirtualMap map, CellLocation l)
	{
		if (map.rooms == null) return false;
		foreach (VirtualRoom r in map.rooms)
		{
			if(r.IsInBorder(l))
				return true;
		}
		return false;
	}
	
	// True if this starting_location is inside a room
	private bool CheckRoom(VirtualMap map, CellLocation l)
	{
		if (map.rooms == null) return false;
		foreach (VirtualRoom r in map.rooms)
		{
			if(r.IsInRoom(l))
				return true;
		}
		return false;
	}


	private void AddToCorrectRoom(VirtualMap map, CellLocation l){
		if (map.rooms == null) return;
		foreach (VirtualRoom r in map.rooms){
			if(r.IsInRoom(l)) { r.cells.Add(l); break;}
		}
	}
	
	override public MetricLocation GetWorldLocation(CellLocation l, int storey){
		MetricLocation actual_location = this.virtual_maps[0].GetActualLocation(l,storey);
		actual_location.x *= 2;	// Double, since a tilemap is two times as big
		actual_location.y *= 2;
		return actual_location;
	}


	// Only 1 step between two tiles
	override protected int TileSeparationSteps{
		get{return 1;}
	}
}
