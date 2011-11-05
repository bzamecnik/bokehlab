namespace BokehLab.RayTracing
{
    using OpenTK;

    public class Intersection
    {
        public Vector3d Position;
        public float[] Color;

        public Intersection(Vector3d position)
            : this(position, null)
        {
        }

        public Intersection(Vector3d position, float[] color)
        {
            this.Position = position;
            this.Color = color;
        }
    }
}
