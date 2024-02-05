using System.Text;

namespace AppliedResearchAssociates.iAM.Analysis.Logic.Testing;

public static class DebuggingExtensions
{
    public static string ToCode<T>(this T[,] values)
    {
        var grid = new StringBuilder();

        grid.Append('{');

        for (var i = 0; i < values.GetLength(0); ++i)
        {
            grid.AppendLine();

            grid.Append('{');

            for (var j = 0; j < values.GetLength(1); ++j)
            {
                if (j > 0)
                {
                    grid.Append(',');
                }

                grid.Append(values[i, j]);
            }

            grid.Append('}').Append(',');
        }

        grid.AppendLine().Append('}');

        return grid.ToString();
    }
}
