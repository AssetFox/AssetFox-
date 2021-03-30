namespace AppliedResearchAssociates.iAM.DataAssignment.Networking
{
    public class SpatialWeighting
    {
        public SpatialWeighting(double area, string areaUnit)
        {
            Area = area;
            AreaUnit = areaUnit;
        }

        public double Area { get; }

        public string AreaUnit { get; }
    }
}
