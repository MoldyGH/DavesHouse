using UnityEngine;
using System.Collections;

public class SectionMapInterpreter : MapInterpreter<SectionMapInterpreterBehaviour> {
	
	override public void ReadMaps (VirtualMap[] maps)
	{
        behaviour.useDirectionalFloors = true;  // We need to force this!

		for(int storey=0; storey<maps.Length; storey++){
			VirtualMap map = maps[storey];
			PerformTilemapConversion(map,maps,storey);
            PerformSpecificConversions(map, maps, storey);
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
				
				if (conversion_cell.IsFloor()){
					ConvertFloor(map,conversion_cell);
				}
				else if (conversion_cell.IsDoor()){
					ConvertDoor(map,conversion_cell);

				} 
				else if (conversion_cell.IsWall()){
                    // All walls are rocks here
					ConvertRock(map,conversion_cell);
				}
				else if (conversion_cell.IsNone()){
                    // All nones are rocks here
					ConvertRock(map,conversion_cell);
				} else if (conversion_cell.IsRock()){
					// All is rock!
					ConvertRock(map,conversion_cell);
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

    protected void ConvertTileIntersections(VirtualMap map, VirtualCell conversion_cell)
    {
        if (conversion_cell.IsEmpty())
        {
            // Empty passages should be filled with floors
            conversion_cell.Type = VirtualCell.CellType.CorridorFloor;
        }
        else
        {
            // 'None' cells will remain as such, since they will be converted to rocks later
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
