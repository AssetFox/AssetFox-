using System.CommandLine;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Analysis.Input.DataTransfer;

var inputArgument = new Argument<FileInfo>("input", "Existing JSON file containing a complete iAM scenario to run.").ExistingOnly();
var rootCommand = new RootCommand { inputArgument };

rootCommand.SetHandler(static inputArgumentValue =>
{
    var timer = Stopwatch.StartNew();

    var input = readInput(inputArgumentValue);
    var inputToRun = input.ConvertOut();

    var elapsedBeforeRun = timer.Elapsed;
    Console.WriteLine($"{elapsedBeforeRun} - Input complete.");

    var runner = new SimulationRunner(inputToRun);
    runner.Progress += (sender, eventArgs) => Console.WriteLine(eventArgs);
    runner.SimulationLog += (sender, eventArgs) => Console.WriteLine(eventArgs.MessageBuilder.ToString());
    runner.Run();

    var elapsedThroughRun = timer.Elapsed;
    var elapsedDuringRun = elapsedThroughRun - elapsedBeforeRun;
    Console.WriteLine($"{elapsedThroughRun} - Analysis complete. Duration: {elapsedDuringRun}");

    var output = inputToRun.Results;
    var outputPath = Path.ChangeExtension(inputArgumentValue.FullName, $"output.{DateTime.Now:yyyy-MM-dd-HHmmssfff}.json");
    using var outputStream = File.Create(outputPath);
    JsonSerializer.Serialize(outputStream, output, new JsonSerializerOptions
    {
        Converters = { new JsonStringEnumConverter() },
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = true,
    });

    Console.WriteLine($"{timer.Elapsed} - Output complete.");
}, inputArgument);

return rootCommand.Invoke(args);

static Scenario readInput(FileInfo inputFile)
{
    var jsonOptions = new JsonSerializerOptions
    {
        AllowTrailingCommas = true,
        Converters = { new JsonStringEnumConverter() },
        ReadCommentHandling = JsonCommentHandling.Skip,
    };

    using var inputStream = inputFile.OpenRead();
    var input =
        JsonSerializer.Deserialize<Scenario>(inputStream, jsonOptions)
        ?? throw new Exception("Input JSON is null.");

    return input;
}
