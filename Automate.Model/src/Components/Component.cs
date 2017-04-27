using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

namespace Automate.Model.Components
{
    public class Component
    {
        public float Weight { get; internal set; }
        public float Size { get; internal set; }
        public string Type { get; internal set; }
        private static Dictionary<string, Component>  _componentMap = new Dictionary<string, Component>()
        {
            {ComponentType.IronOre.ToString(),     new Component { Weight = 10, Size = 10, Type = ComponentType.IronOre.ToString() } },
            {ComponentType.IronIngot.ToString(),   new Component { Weight = 10, Size = 10, Type = ComponentType.IronIngot.ToString() } },
            {ComponentType.Wood.ToString(),        new Component { Weight = 10, Size = 10, Type = ComponentType.Wood.ToString() } },
            {ComponentType.Stone.ToString(),       new Component { Weight = 10, Size = 10, Type = ComponentType.Stone.ToString() } },
            {ComponentType.CopperOre.ToString(),   new Component { Weight = 10, Size = 10, Type = ComponentType.CopperOre.ToString() } },
            {ComponentType.CopperIngot.ToString(), new Component { Weight = 10, Size = 10, Type = ComponentType.CopperIngot.ToString() } },
            {ComponentType.Steel.ToString(),       new Component { Weight = 10, Size = 10, Type = ComponentType.Steel.ToString() } },
        };

        public static Component GetComponent(ComponentType componentType)
        {
            return GetComponent(componentType.ToString());
        }

        public static Component GetComponent(string componentType)
        {
            if (!_componentMap.ContainsKey(componentType))
                throw new ArgumentException(componentType + " component type does not exist");
            return _componentMap[componentType];
        }

        public static void NewComponent(string componentName, float weight, float size)
        {
            if (_componentMap.ContainsKey(componentName))
                throw new ArgumentException(componentName + " component type already exists");
            _componentMap[componentName] = new Component() {Weight = weight, Size = size};
        }

        internal Component()
        {
        }

        public override bool Equals(Object obj) {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            Component component = (Component)obj;
            return (Type.Equals(component.Type)) && (Size == component.Size) && (Weight == component.Weight);
        }

        public override int GetHashCode() {
            return 1;
        }

    }


    public enum ComponentType
    {
        IronOre,
        IronIngot,
        Wood,
        Stone,
        CopperOre,
        CopperIngot,
        Steel
    }
}