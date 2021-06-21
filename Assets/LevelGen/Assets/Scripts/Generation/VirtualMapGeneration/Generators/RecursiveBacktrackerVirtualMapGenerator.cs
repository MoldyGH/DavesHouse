using System.Collections;
using System.Collections.Generic;

public class RecursiveBacktrackerDiggingMapGeneratorAlgorithm : DiggingMapGeneratorAlgorithm
{
    override public void StartDigging(VirtualMap map, CellLocation starting_location, int directionChangeModifier)
    {
        CellLocation currentLocation = starting_location;
        map.MarkAsVisited(currentLocation);

        // Pick a starting previous direction
        VirtualMap.DirectionType previousDirection = VirtualMap.DirectionType.North;

        List<CellLocation> previousLocations = new List<CellLocation>();

        // Repeat until all cells have been visited
        while (!map.AllCellsVisited)
        {
            // Get a starting direction
            DirectionPicker directionPicker = new DirectionPicker(map, currentLocation, directionChangeModifier, previousDirection);
            VirtualMap.DirectionType direction = directionPicker.GetNextDirection(map, currentLocation);

            if (direction != VirtualMap.DirectionType.None)
            {
                // Create a corridor in the current cell and flag it as visited
                previousLocations.Add(currentLocation);
                previousDirection = direction;
                currentLocation = map.CreateCorridor(currentLocation, direction);
                map.FlagCellAsVisited(currentLocation);
            }
            else
            {
                // Backtrack
                currentLocation = previousLocations[previousLocations.Count - 1];
                previousLocations.RemoveAt(previousLocations.Count - 1);
            }
        }
    }

}
