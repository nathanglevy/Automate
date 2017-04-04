using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Model.MapModelComponents
{
    /// <summary>
    /// Class which holds information on a game map.<para />
    /// Contains: CellInfo grid which can be accessed via coordinates.
    /// </summary>
    [Serializable]
    public class MapInfo
    {
        private Boundary _boundary;
        private Dictionary<string,CellInfo> _cellInfoDictionary;

        /// <summary>
        /// Accepts 3 parameters defining map size from (0,0,0) to (x,y,z). <para />
        /// This is inclusive, so a size of 10,10,10 gives a 11x11x11 map
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public MapInfo(int x, int y, int z) :
            this(new Coordinate(0, 0, 0), new Coordinate(x, y, z)) {
        }

        /// <summary>
        /// Accepts 2 coordinates defining the boundary of the map
        /// </summary>
        /// <param name="topLeft">Coordinate describing the top left of the map</param>
        /// <param name="bottomRight">Coordinate describing the bottom right of the map</param>
        public MapInfo(Coordinate topLeft, Coordinate bottomRight) :
            this(new Boundary(topLeft, bottomRight)) {
        }

        /// <summary>
        /// Accepts a boundary defining the size and coordinates of the map.
        /// </summary>
        /// <param name="boundary">Boundary of the map coordinates</param>
        [JsonConstructor]
        public MapInfo(Boundary boundary) {
            _boundary = boundary;
            _cellInfoDictionary = new Dictionary<string, CellInfo>();
        }

        /// <summary>
        /// Sets the map's cell in the given coordinates. <para />
        /// If coordinate given is out of bounds, an exception is thrown.
        /// </summary>
        /// <param name="coordinate">The location on the map in which to set the cell</param>
        /// <param name="cellInfo">Replace the current cell in that location with this cell</param>
        public void SetCell(Coordinate coordinate, CellInfo cellInfo) {
            if (!IsCoordinateIsWithinBounds(coordinate))
                throw new ArgumentOutOfRangeException();
            if (_cellInfoDictionary.ContainsKey(coordinate.ToString()))
                _cellInfoDictionary.Remove(coordinate.ToString());
            _cellInfoDictionary.Add(coordinate.ToString(), cellInfo);
        }

        /// <summary>
        /// Gets the cell at the given (x,y,z) coordinates
        /// </summary>
        /// <param name="x">Defines x value</param>
        /// <param name="y">Defines y value</param>
        /// <param name="z">Defines z value</param>
        /// <returns>The cell at the given coordinates</returns>
        public CellInfo GetCell(int x, int y, int z)
        {
            return GetCell(new Coordinate(x, y, z));
        }

        /// <summary>
        /// Gets the cell at the given (x,y,z) coordinates
        /// </summary>
        /// <param name="coordinate">Coordinate location of the cell to get</param>
        /// <returns>The cell at the given coordinate</returns>
        public CellInfo GetCell(Coordinate coordinate)
        {
            if (!IsCoordinateIsWithinBounds(coordinate))
                throw new ArgumentOutOfRangeException();
            if (_cellInfoDictionary.ContainsKey(coordinate.ToString()))
                return _cellInfoDictionary[coordinate.ToString()];

            return null;
        }

        public Boundary GetBoundary() {
            return _boundary;
        }

        /// <summary>
        /// Checks if the coordinate is within the map's bounds
        /// </summary>
        /// <param name="c">Coordinate to be checked if it is within borders</param>
        /// <returns>True if the coordinate is within bounds, false otherwise</returns>
        public bool IsCoordinateIsWithinBounds(Coordinate c) {
            return _boundary.IsCoordinateInBoundary(c);
        }

        /// <summary>
        /// An initilization helper method to help fill the cells with copies of the same cell
        /// </summary>
        /// <param name="cellInfo">Single cell template with which to fill the map</param>
        public void FillMapWithCells(CellInfo cellInfo)
        {
            for (int x = _boundary.topLeft.x; x <= _boundary.bottomRight.x; x++) {
                for (int y = _boundary.topLeft.y; y <= _boundary.bottomRight.y; y++) {
                    for (int z = _boundary.topLeft.z; z <= _boundary.bottomRight.z; z++) {
                        //SetCell(new Coordinate(x,y,z), cellInfo.Clone());
                        CellInfo newCell = new CellInfo(cellInfo.IsPassable(), cellInfo.GetWeight());
                        SetCell(new Coordinate(x,y,z), newCell);
                    }
                }
            }
        }

        /// <summary>
        /// Serialize the MapInfo class to a file in JSON format
        /// </summary>
        /// <param name="testmapJson">Path to the output JSON file</param>
        public void SaveMap(string testmapJson)
        {
            var settings = new JsonSerializerSettings() { ContractResolver = new MyContractResolver() };
            string jsonText = JsonConvert.SerializeObject(this, settings);
            File.WriteAllText(testmapJson, jsonText);
        }

        /// <summary>
        /// Deserialize the MapInfo class from a file in JSON format
        /// </summary>
        /// <param name="testmapJson">Path to the input JSON file</param>
        public static MapInfo LoadMap(string testmapJson)
        {
            var settings = new JsonSerializerSettings() { ContractResolver = new MyContractResolver() };
            return JsonConvert.DeserializeObject<MapInfo>(File.ReadAllText(testmapJson),settings);
        }
    }
}