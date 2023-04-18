using System.CommandLine;
using System.Text.Json;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Analysis.Input.DataTransfer;

var inputArgument = new Argument<FileInfo>("input", "Existing JSON file containing a complete iAM scenario to run.").ExistingOnly();
var rootCommand = new RootCommand { inputArgument };

rootCommand.SetHandler(static inputArgumentValue =>
{
    var input = readInput(inputArgumentValue);
    var inputToRun = input.ConvertOut();
    var runner = new SimulationRunner(inputToRun);
    runner.Run();
    var output = inputToRun.Results;
    var outputPath = Path.ChangeExtension(inputArgumentValue.FullName, $"output.{DateTime.Now:yyyy-MM-dd-HHmmssfff}.json");
    using var outputStream = File.Create(outputPath);
    JsonSerializer.Serialize(outputStream, output, new JsonSerializerOptions { WriteIndented = true });
}, inputArgument);

return rootCommand.Invoke(args);

static Scenario readInput(FileInfo inputFile)
{
    var jsonOptions = new JsonSerializerOptions
    {
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    using var inputStream = inputFile.OpenRead();
    var input =
        JsonSerializer.Deserialize<Scenario>(inputStream, jsonOptions)
        ?? throw new Exception("Input JSON is null.");

    return input;
}
