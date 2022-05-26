namespace AppliedResearchAssociates.iAM.Data.Networking
{
    public class SpatialWeighting
    {
        public SpatialWeighting(double area, string areaUnit = "ft^2")
        {
            Area = area;
            AreaUnit = areaUnit;
        }

        public double Area { get; }

        public string AreaUnit { get; }
    }
}
