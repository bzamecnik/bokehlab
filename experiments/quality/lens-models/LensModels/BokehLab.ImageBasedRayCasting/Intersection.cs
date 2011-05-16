namespace BokehLab.ImageBasedRayCasting
{
    using OpenTK;

    public class Intersection
    {
        public Vector3d position;
        public float[] color;

        public Intersection(Vector3d position, float[] color)
        {
            this.position = position;
            this.color = color;
        }
    }
}
