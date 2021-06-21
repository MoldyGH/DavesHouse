using System.Collections.Generic;
using System;
using UnityEngine;

// For GUI purposes
public enum MazeGenerationAlgorithmChoice {
    HuntAndKill, 
    RecursiveBacktracker,
    BSPTree
}

// Handles the generation of a VirtualMap using a chosen algorithm
public class VirtualMapGenerator 
{
	protected bool verbose = false;
    
    // Properties: start and end
    public bool createStartAndEnd = false;
    public bool forceStartAndEndInRooms = false;
    public float minimumDistanceBetweenStartAndEnd = 0;
    public float maximumDistanceBetweenStartAndEnd = 100;


    public void Initialise(bool forceStartAndEndInRooms)
    {
        this.forceStartAndEndInRooms = forceStartAndEndInRooms;
    }

	public uint InitialiseSeed(bool useSeed, int seed){
	    uint lastUsedSeed = 0U;
		if (useSeed)	lastUsedSeed = DungeonGenerator.Random.Instance.Initialize(seed);
		else 			lastUsedSeed = DungeonGenerator.Random.Instance.Initialize();
        return lastUsedSeed;
	}

    public VirtualMap[] GenerateAllMaps(int width, int height, int nStoreys)
    {
        VirtualMap[] virtualMaps = new VirtualMap[nStoreys];
        for (int i = 0; i < nStoreys; i++)
        {
            if (i == 0) virtualMaps[i] = Generate(width, height, i);
            else virtualMaps[i] = Generate(width, height, i, virtualMaps[i - 1]);
        }

        return virtualMaps;
    }

	private VirtualMap Generate (int width, int height, int storey_number, VirtualMap vmapBelow = null)
	{
		// Create a new map  
		VirtualMap map = new VirtualMap(width, height, storey_number);

        // Start generating, choosing a starting position
        GenerateWithAlgorithm(map, vmapBelow);

        // Addendums (common)
        CreateRocks(map);
        if (verbose) Debug.Log("Added rocks!");

        // DEPRECATED: now done inside the derived generator
        //CreateDoorPassages_Post(map);
        //if (verbose) Debug.Log("Added doors!");

        // We always compute start and end! It is needed by other algorithms!
        ComputeStartAndEnd(map, vmapBelow);

        return map;
    }

    virtual protected void GenerateWithAlgorithm(VirtualMap map, VirtualMap vmapBelow){}

    /* DEPRECATED
     * public void CreateDoorPassages_Post(VirtualMap map)
    {
        if (map.HasRooms())
        {
            RoomGenerator roomG = new RoomGenerator();
            roomG.doorsDensityModifier = doorsDensityModifier;
            foreach (VirtualRoom r in map.rooms) roomG.CreateDoors_Post(map, r);
        }
	}*/
	
	public void CreateRocks(VirtualMap map){
		foreach (CellLocation c in map.visitedCells) {
			if(map.IsSurroundedByWalls(c)) map.GetCell((int)c.x,(int)c.y).Type=VirtualCell.CellType.Rock;
		}
	}

    public void ComputeStartAndEnd(VirtualMap map, VirtualMap vmapBelow)
    {
		List<CellLocation> iterable_locations;
		bool foundStart = false;

		if (forceStartAndEndInRooms) iterable_locations = new List<CellLocation>(map.RoomWalkableLocations);
		else iterable_locations = new List<CellLocation>(map.WalkableLocations);
		
		// Makes sure it is in some dead end, or in a room, if possible
		List<CellLocation> possible_start_locations = new List<CellLocation>();
		foreach(CellLocation l in iterable_locations){
			if (map.IsDeadEnd(l) || map.IsInRoom(l)) possible_start_locations.Add(l);
		}

		// If not possible, consider all locations equally
		if (possible_start_locations.Count == 0) possible_start_locations = iterable_locations;
		//		Debug.Log ("Possible start locations: " + possible_start_locations.Count);

		// TODO: Make an option for the start to be on the perimeter
//		foreach (CellLocation l in possible_start_locations) {
//			if (map.IsOnPerimeter(l) possible_start_locations
//		}


        // NOTE: we here find a start, but the algorithm already could have one! maybe use that?
		if (vmapBelow == null){
			// Choose a random walkable cell as the starting point
			int index;
			index = DungeonGenerator.Random.Instance.Next(0,possible_start_locations.Count-1);
			if(index !=-1 && possible_start_locations.Count != 0){
				map.start = new CellLocation(possible_start_locations[index].x,possible_start_locations[index].y);
				foundStart = true;
			}
		} else {
			// Choose the cell above the below map's end as the starting point
			map.start = vmapBelow.end;
			foundStart = true;
		}

		if (foundStart){
			//Debug.Log ("CHOSEN START: " + map.start);
			// For this to work, we must compute the distance of all cells from the starting cell
			map.ComputeCellDistances (startCellLocation:map.start);

			// Choose a cell at a certain distance from the starting point as the ending point
			map.end = map.GetCellLocationInLimits(iterable_locations, minimumDistanceBetweenStartAndEnd/100.0f * map.GetMaximumDistance(), maximumDistanceBetweenStartAndEnd/100.0f * map.GetMaximumDistance());
		}

        //Debug.Log("START : " + map.start);
        //Debug.Log("END : " + map.end);
        //Debug.Log(map.GetWalkDistance(map.start,map.end));

        if (!foundStart) Debug.LogError("Cannot find a suitable entrance!");
		//DebugUtils.Assert(foundStart, "Cannot find a suitable entrance!");
	}	

}