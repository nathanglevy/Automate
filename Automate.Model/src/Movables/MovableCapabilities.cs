namespace Automate.Model.Movables
{
    public class MovableCapabilities
    {
        public int MaxCarryWeight { get; set; } = 10;
        public float MovementSpeed { get; set; } = 1;
        public float WorkSpeed { get; set; } = 1;
        public int Intelligence { get; set; } = 0;
    }
}