using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Assets.src.PathFinding.MapModelComponents
{
    [Serializable]
    public class MapInfo
    {
        //        public MapInfo()
        //        {
        //            throw new NotImplementedException();
        //        }
        //        private Coordinate _topLeftBoundry;
        //        private Coordinate _bottomRightBoundry;
        private Boundary _boundary;
        private Dictionary<Coordinate,CellInfo> _cellInfoDictionary;

        public MapInfo(int x, int y, int z) :
            this(new Coordinate(0, 0, 0), new Coordinate(x, y, z)) {
        }

        public MapInfo(Coordinate topLeft, Coordinate bottomRight) :
            this(new Boundary(topLeft, bottomRight)) {
        }

        public MapInfo(Boundary boundary) {
            _boundary = boundary;
            _cellInfoDictionary = new Dictionary<Coordinate, CellInfo>();
        }

        public void SetCell(Coordinate coordinate, CellInfo cellInfo) {
            if (!IsCoordinateIsWithinBounds(coordinate))
                throw new ArgumentOutOfRangeException();
            if (_cellInfoDictionary.ContainsKey(coordinate))
                _cellInfoDictionary.Remove(coordinate);
            _cellInfoDictionary.Add(coordinate,cellInfo);
        }

        public CellInfo GetCell(Coordinate coordinate)
        {

            if (!IsCoordinateIsWithinBounds(coordinate))
                throw new ArgumentOutOfRangeException();
            if (_cellInfoDictionary.ContainsKey(coordinate))
                return _cellInfoDictionary[coordinate];

            return null;
        }

        public object GetBoundary() {
            return _boundary;
        }

        public bool IsCoordinateIsWithinBounds(Coordinate c) {
            return _boundary.IsCoordinateInBoundary(c);
        }

        public void FillMapWithCells(CellInfo cellInfo)
        {
            for (int x = _boundary.topLeft.x; x <= _boundary.bottomRight.x; x++) {
                for (int y = _boundary.topLeft.y; y <= _boundary.bottomRight.y; y++) {
                    for (int z = _boundary.topLeft.z; z <= _boundary.bottomRight.z; z++) {
                        //SetCell(new Coordinate(x,y,z), cellInfo.Clone());
                        CellInfo newCell = new CellInfo(cellInfo.IsPassable(), cellInfo.GetWeight(), null);
                        SetCell(new Coordinate(x,y,z), newCell);
                    }
                }
            }
        }

        public void SaveMap(string testmapJson)
        {
            var settings = new JsonSerializerSettings() { ContractResolver = new MyContractResolver() };
            string jsonText = JsonConvert.SerializeObject(this, settings);
            File.WriteAllText(testmapJson, jsonText);
        }

        public static MapInfo LoadMap(string testmapJson)
        {
            throw new NotImplementedException();
        }
    }
}