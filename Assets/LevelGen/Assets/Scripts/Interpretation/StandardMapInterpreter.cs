using UnityEngine;
using System.Collections;

public class StandardMapInterpreter : MapInterpreter<StandardMapInterpreterBehaviour> {
	
	// TODO: This may be made more performant by checking 'map.getAllRoomTiles' and so on instead of iterating over everything
	override public void ReadMaps (VirtualMap[] maps)
	{
		for(int storey=0; storey<maps.Length; storey++){
			VirtualMap map = maps[storey];
			PerformSpecificConversions(map, maps, storey);
		}

		// Additional modifications, multi-storey
		for(int storey=0; storey<maps.Length; storey++){
			VirtualMap map = maps[storey];
			AddStairsToRooms(map,maps,storey);
		}
	}

	private void PerformSpecificConversions(VirtualMap map, VirtualMap[] maps, int storey){
		VirtualCell conversion_cell;
		VirtualCell[,] output_cells = new VirtualCell[map.Width,map.Height];

		// Modify the virtual map as needed
		for (int i = 0; i < map.Width; i++){
			for (int j = 0; j < map.Height; j++){
				CellLocation loc = new CellLocation(i,j);
				conversion_cell = VirtualCell.Copy(map.GetCell(loc));
				
				if (conversion_cell.IsFloor()){
					ConvertFloor(map,conversion_cell);
				}
				else if (conversion_cell.IsDoor()){
					ConvertDoor(map,conversion_cell);
					
					bool renderWall  = CheckWallRendering(map,conversion_cell);
					if (renderWall) ConvertWallOrientation(map,conversion_cell);
				}
				else if (conversion_cell.IsWall() || conversion_cell.IsEmpty()){
					bool isPerimeter = ConvertPerimeterWall(map,conversion_cell);
					if (!isPerimeter){
						bool renderWall  = CheckWallRendering(map,conversion_cell);
						if (renderWall) ConvertWallOrientation(map,conversion_cell);
					}
				}
				else if (conversion_cell.IsNone()){
					// 'None' cells may be converted to columns
					ConvertColumn(map,conversion_cell,true);
					ConvertPerimeterColumn(map,conversion_cell);
					
				} else if (conversion_cell.IsRock()){
					// We may draw rocks
					ConvertRock(map,conversion_cell);
				}
				
				// Final pass on orientation randomization
				CheckOrientationRandomization(map,conversion_cell);
				
				// We save the cell in the initial map
				output_cells[i,j] = conversion_cell;
			}
			
		}
		
		// We can now override the initial map!
		for (int i = 0; i < map.Width; i++){
			for (int j = 0; j <  map.Height; j++){
				map.SetCell(i,j,output_cells[i,j]);
			}
		}
		
		// Additional modifications, per-storey
		// WE DO NOT HAVE ANYTHING HERE FOR NOW

	}




	override public MetricLocation GetWorldLocation(CellLocation l, int storey){
		return this.virtual_maps[storey].GetActualLocation(l, storey);
	}

}
