using UnityEngine;
using System.Collections;

public enum DiggingMapGeneratorAlgorithmChoice
{
    RecursiveBacktracker,
    HuntAndKill,
}

public abstract class DiggingMapGeneratorAlgorithm
{
    abstract public void StartDigging(VirtualMap map, CellLocation starting_location, int directionChangeModifier);
}
