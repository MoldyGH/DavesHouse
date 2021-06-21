using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;

public class DiggingVirtualMapGenerator : VirtualMapGenerator
{
    // Parameters
    public DiggingMapGeneratorAlgorithmChoice algorithmChoice;

    public int directionChangeModifier = 0;
    public int sparsenessModifier = 0;
    public int openDeadEndModifier = 0;

    public bool createRooms = false;
    public int minRooms = 1;
    public int maxRooms = 1;
    public int minRoomWidth = 2;
    public int maxRoomWidth = 2;
    public int minRoomHeight = 2;
    public int maxRoomHeight = 2;
    public bool forceRoomTransversal;
    public int doorsDensityModifier;

    override protected void GenerateWithAlgorithm(VirtualMap map, VirtualMap vmapBelow)
    {
        // Make sure the map is full of walls
        map.ResetToRocks();
        //return;

        // Pick the cell to start from
        CellLocation starting_location = default(CellLocation);
        if (vmapBelow != null) starting_location = vmapBelow.end;
        else starting_location = map.PickRandomUnvisitedLocation();
        //Debug.Log(starting_location);

        DiggingMapGeneratorAlgorithm alg = null;
        switch (algorithmChoice)
        {
            case DiggingMapGeneratorAlgorithmChoice.RecursiveBacktracker:       alg = new RecursiveBacktrackerDiggingMapGeneratorAlgorithm(); break;
            case DiggingMapGeneratorAlgorithmChoice.HuntAndKill:                alg = new HuntAndKillDiggingMapGeneratorAlgorithm(); break;
        }
        alg.StartDigging(map, starting_location, directionChangeModifier);

		Sparsify(map,vmapBelow);
		if (verbose) Console.WriteLine("Sparsified!");
		
		OpenDeadEnds(map);
        if (verbose) Console.WriteLine("Opened dead ends!");

        if (createRooms)
        {
            CreateRooms(map);
            if (verbose) Console.WriteLine("Added rooms!");
        }
    }

    // Sparsify the map by removing dead-end cells.
    private void Sparsify(VirtualMap map, VirtualMap vmapBelow)
    {
        // Compute the number of cells to remove as a percentage of the total number of cells in the map
        int noOfDeadEndCellsToRemove = (int)Math.Floor((decimal)sparsenessModifier / 100 * (map.ActualWidth * map.ActualHeight));
        if (verbose) Console.WriteLine("Sparsify: removing  " + sparsenessModifier + "% i.e. " + noOfDeadEndCellsToRemove + " out of " + map.ActualWidth * map.ActualHeight + " cells");

        int noOfRemovedCells = 0;
        IEnumerable<CellLocation> deads;
        while (noOfRemovedCells < noOfDeadEndCellsToRemove)
        {
            // We sweep and remove all current dead ends
            deads = map.DeadEndCellLocations;

            int currentlyRemovedCells = 0;
            foreach (CellLocation location in deads)
            {
                if (vmapBelow != null && location == vmapBelow.end) continue; // For multi-storey to work correctly, we do not remove the cell above the below's end
                //				Console.WriteLine("Dead at " + starting_location);
                //				Console.WriteLine(map.CalculateDeadEndCorridorDirection(starting_location));
                map.CreateWall(location, map.CalculateDeadEndCorridorDirection(location));
                currentlyRemovedCells++;
                if (++noOfRemovedCells == noOfDeadEndCellsToRemove) break;
            }
            if (currentlyRemovedCells == 0)
            {
                //				Console.WriteLine("We have no more dead ends!");
                break;	// No more dead endss
            }
            //			Console.WriteLine("We removed a total of " + noOfRemovedCells + " cells"); 
        }
    }

    // Open dead ends by linking them to rooms
    private void OpenDeadEnds(VirtualMap map)
    {
        //		Console.WriteLine("DEAD END MOD: " + openDeadEndModifier);
        if (openDeadEndModifier == 0) return;

        IEnumerable<CellLocation> deads = map.DeadEndCellLocations;
        foreach (CellLocation deadEnd in deads)
        {
            if (DungeonGenerator.Random.Instance.Next(1, 99) < openDeadEndModifier)
            {
                CellLocation currentLocation = deadEnd;
                //				int count=0;
                do
                {
                    // Initialize the direction picker not to select the dead-end corridor direction
                    DirectionPicker directionPicker = new DirectionPicker(map, currentLocation, map.CalculateDeadEndCorridorDirection(currentLocation));
                    //                    Debug.Log("We have a dead and " + directionPicker);
                    VirtualMap.DirectionType direction = directionPicker.GetNextDirection(map, currentLocation);
                    //					Debug.Log("We choose dir " + direction);
                    if (direction == VirtualMap.DirectionType.None)
                        throw new InvalidOperationException("Could not remove the dead end!");
                    //						Debug.Log("Cannot go that way!");
                    else
                        // Create a corridor in the selected direction
                        currentLocation = map.CreateCorridor(currentLocation, direction);
                    //					count++;
                } while (map.IsDeadEnd(currentLocation) && currentLocation != deadEnd); // Stop when you intersect an existing corridor, or when you end back to the starting cell (that means we could not remove the dead end, happens with really small maps
                //				Debug.Log("Dead end removed"); 
            }
        }
    }

    /*********************
     * Room generation
     *********************/

    private void CreateRooms(VirtualMap map)
    {
        DiggingRoomGenerator roomGenerator = new DiggingRoomGenerator();
        roomGenerator.SetParameters(minRooms, maxRooms, minRoomWidth, maxRoomWidth, minRoomHeight, maxRoomHeight, forceRoomTransversal, doorsDensityModifier);
        roomGenerator.CreateRooms(map);
    }

}


public class DiggingRoomGenerator {

    private int minRooms;
    private int maxRooms;
    private int minRoomWidth;
    private int maxRoomWidth;
    private int minRoomHeight;
    private int maxRoomHeight;
    private bool forceRoomTransversal;
    private int doorsDensityModifier;

    public void SetParameters(int minRooms, int maxRooms, int minRoomWidth, int maxRoomWidth, int minRoomHeight, int maxRoomHeight, bool forceRoomTransversal,
        int doorsDensityModifier)
    {
        this.minRooms = minRooms;
        this.maxRooms = maxRooms;
        this.minRoomWidth = minRoomWidth;
        this.maxRoomWidth = maxRoomWidth;
        this.minRoomHeight = minRoomHeight;
        this.maxRoomHeight = maxRoomHeight;
        this.forceRoomTransversal = forceRoomTransversal;
        this.doorsDensityModifier = doorsDensityModifier;
    }
    private void CheckDimensions(VirtualMap map)
    {
        int minW = minRoomWidth;
        int maxW = maxRoomWidth;

        int minH = minRoomHeight;
        int maxH = maxRoomHeight;

        int i = 0;
        while ((maxH > minH || maxW > minW) && maxH * maxW * maxRooms > ((map.Width - 1) / 2) * ((map.Height - 1) / 2))
        {
            if (i % 2 == 0 && maxH > minH)
            {
                maxH--;
                Debug.LogWarning("Map is too small. Decreasing max room height to " + maxH);
            }
            else
            {
                maxW--;
                Debug.LogWarning("Map is too small. Decreasing max room width to " + maxW);
            }
            i++;

            if (maxH == 2 && minH == 2) break;
        }
    }

    public void CreateRooms(VirtualMap map)
    {
        RoomGenerator roomGenerator = new RoomGenerator();
        int roomNumber = DungeonGenerator.Random.Instance.Next(minRooms, maxRooms);

        // Ensure that the total area is compatible
        CheckDimensions(map);
        map.rooms = new List<VirtualRoom>();
        for (int n = 0; n < roomNumber; n++)
        {
            VirtualRoom room = CreateRoom(map, roomGenerator);
            if (room != null)
            {
                room.sequentialId = map.rooms.Count;
                map.rooms.Add(room);
            }
        }
    }

    private VirtualRoom CreateRoom(VirtualMap map, RoomGenerator roomGenerator)
    {
        int width = DungeonGenerator.Random.Instance.Next(minRoomWidth, maxRoomWidth);
        int height = DungeonGenerator.Random.Instance.Next(minRoomHeight, maxRoomHeight);

        VirtualRoom r = new VirtualRoom(width, height, new CellLocation(0, 0));
        PickBestRoomLocation(map, r);
        //		PickRandomLocation(map,r);

        VirtualRoom room = roomGenerator.CreateRoom(map, width, height, r);

        if (room != null) CreateDoors(map, room, roomGenerator);

        return room;
    }

    private void PickBestRoomLocation(VirtualMap map, VirtualRoom r)
    {//, int roomNumber){
        // Traverse all floor cells checking for the best position for a room
        int best_score = 1000000;
        int current_score;
        List<CellLocation> best_locations = new List<CellLocation>();
        List<CellLocation> locations = new List<CellLocation>(map.floorCells);
        foreach (CellLocation map_l in locations)
        {
            r.leftCorner = map_l;
            if (IsRoomLocationValid(map, r))
            {
                current_score = 0;	 // Lower is better
                for (int i = 0; i < r.Width; i++)
                {
                    for (int j = 0; j < r.Height; j++)
                    {
                        CellLocation possible_room_l = new CellLocation(r.leftCorner.x + 2 * i, r.leftCorner.y + 2 * j);
                        //						Debug.Log("Possible room l: " + possible_room_l);

                        // Corridor vicinity: good
                        if (map.IsSurroundedByWalls(possible_room_l) && map.HasAdjacentFloor(possible_room_l)) current_score += 1;

                        bool corridorOverlap = map.IsFloor(possible_room_l);

                        // Corridor overlap: bad (at default)
                        if (!forceRoomTransversal && corridorOverlap) current_score += 3;

                        // or good if we want the room in the middle!
                        else if (forceRoomTransversal && !corridorOverlap) current_score += 3;

                        // Room overlap: very very bad
                        if (map.IsRoomFloor(possible_room_l)) current_score += 100;

                        // If multi-storey, the first room should be placed above another room!
                        //						if (roomNumber == 0 && !belowMap.isRoomFloor(possible_room_l)) current_score += 5;

                        // TODO: may be more efficient to exit now if the score is already low enough!
                    }
                }

                if (current_score == 0) continue; // Zero is not a valid score, as it means the room is isolated

                if (current_score == best_score)
                {
                    best_locations.Add(map_l);
                }
                else if (current_score < best_score)
                {
                    best_score = current_score;
                    best_locations.Clear();
                    best_locations.Add(map_l);
                }
            }
        }

        if (best_locations.Count == 0) r.leftCorner = new CellLocation(-1, -1);
        else r.leftCorner = best_locations[DungeonGenerator.Random.Instance.Next(0, best_locations.Count - 1)];

    }

    private void PickRandomRoomLocation(VirtualMap map, VirtualRoom r)
    {
        // Pick a random valid Location
        List<CellLocation> locations = new List<CellLocation>(map.floorCells);
        do
        {
            int index = DungeonGenerator.Random.Instance.Next(0, locations.Count - 1);
            CellLocation l = locations[index];
            r.leftCorner = l;
            locations.Remove(l);
        } while (locations.Count > 0 && !IsRoomLocationValid(map, r));
    }

    public bool IsRoomLocationValid(VirtualMap map, VirtualRoom r)
    {
        // A room starting_location is valid if it is in bounds 
        for (int i = 0; i < r.Width; i++)
        {
            for (int j = 0; j < r.Height; j++)
            {
                CellLocation l = new CellLocation(r.leftCorner.x + 2 * i, r.leftCorner.y + 2 * j);
                if (!map.floorCells.Contains(l))// || map.visitedCells.Contains (l))
                    return false;
            }
        }
        return true;
    }


    /********************
     * Door creation
     ********************/
    public void CreateDoors(VirtualMap map, VirtualRoom r, RoomGenerator roomGenerator)
    {
        // Create a list of border floors (close to the borders of the room)
        List<CellLocation> borderFloors = new List<CellLocation>();
        for (int i = 0; i < r.Width; i++)
        {
            for (int j = 0; j < r.Height; j++)
            {
                if (i == 0 || j == 0 || i == r.Width - 1 || j == r.Height - 1)
                {
                    CellLocation l = new CellLocation(r.leftCorner.x + 2 * i, r.leftCorner.y + 2 * j);
                    borderFloors.Add(l);
                }
            }
        }

        // For each border floor, check if we are connecting to something on the other side
        List<CellLocation> outsideBorderFloors = new List<CellLocation>();
        List<CellLocation> insideBorderFloors = new List<CellLocation>();
        List<VirtualMap.DirectionType> borderDirections = new List<VirtualMap.DirectionType>();

        CellLocation target_passage;
        CellLocation target_floor;
        foreach (CellLocation l in borderFloors)
        {
            foreach (VirtualMap.DirectionType dir in map.directions)
            {
                target_passage = map.GetNeighbourCellLocation(l, dir);
                target_floor = map.GetTargetLocation(l, dir);
                if (!map.LocationIsOutsideBounds(target_floor) 
                    && map.GetCell(target_passage).IsWall()
                    && !map.IsSurroundedByWalls(target_floor))
                {
                    outsideBorderFloors.Add(target_floor);
                    insideBorderFloors.Add(l);
                    borderDirections.Add(dir);
                }
            }
        }

        // We now create a door for each outside border floor, making sure to avoid re-creating doors if the floors are already connected
        List<CellLocation> unremovedFloors = new List<CellLocation>(outsideBorderFloors);
        for (int i = 0; i < outsideBorderFloors.Count; i++)
        {
            CellLocation l = outsideBorderFloors[i];
            // If not already removed (but we may not skip if we request more doors than needed)
            if (unremovedFloors.Contains(l) ||DungeonGenerator.Random.Instance.Next(0, 100) < doorsDensityModifier){
                CreateDoor(map, r, roomGenerator, insideBorderFloors[i], l, borderDirections[i]);
                unremovedFloors.Remove(l); 

                // We also remove the other connected cells
                for(int j=unremovedFloors.Count-1; j>= 0; j--){
                    CellLocation other_l = unremovedFloors[j];
                    bool existsPath = map.ExistsPathBetweenLocations(l, other_l);
                    if (existsPath) unremovedFloors.Remove(other_l); 
                }
            }
        }

    }

    // Control if we can open a door in the given direction. Open it if possible
    private void CreateDoor(VirtualMap map, VirtualRoom r, RoomGenerator roomGenerator, CellLocation start_floor_loc, CellLocation end_floor_loc, VirtualMap.DirectionType direction)
    {
        CellLocation passage = map.GetNeighbourCellLocation(start_floor_loc, direction);

        // We check whether we are connecting to another room
        if (map.IsRoomFloor(end_floor_loc))
        {
            // Room-to-room door 
            roomGenerator.OpenRoomDoor(map, r, start_floor_loc, end_floor_loc, passage, direction);
        }
        else
        {
            // Room-to-corridor door
            roomGenerator.OpenCorridorDoor(map, r, start_floor_loc, end_floor_loc, passage, direction);
        }
    }



}