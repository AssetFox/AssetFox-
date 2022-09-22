namespace AppliedResearchAssociates.iAM.Common
{
    public static class ListExtensions
    {
        public static void Sort<T, U>(this List<T> list, Func<T, U> sortFunction)
            where U: IComparable<U>
        {
            list.Sort((x, y) => sortFunction(x).CompareTo(sortFunction(y)));
        }
    }
}
