using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDb.Playground.Repository;
using MongoDb.Playground.Repository.Configuration;
using MongoDB.Driver;
using Sample.MovieStore;

var folder = args.Length > 0 ? args[0] : Path.Combine(Environment.CurrentDirectory, "data");
var batchSize = args.Length > 1 && int.TryParse(args[1], out var parsedBatchSize) && parsedBatchSize > 0
	? parsedBatchSize
	: 100;

if (!Directory.Exists(folder))
{
	Console.Error.WriteLine($"Folder not found: {folder}");
	Console.Error.WriteLine("Usage: dotnet run -- <folderPath>");
	return;
}

var jsonFiles = Directory.EnumerateFiles(folder, "*.json", SearchOption.TopDirectoryOnly).ToList();
Console.WriteLine($"Found {jsonFiles.Count} json files in: {folder}");
Console.WriteLine($"Batch size: {batchSize}");

var serializerOptions = new JsonSerializerOptions
{
	PropertyNameCaseInsensitive = true,
	ReadCommentHandling = JsonCommentHandling.Skip,
	AllowTrailingCommas = true,
};

var cancellationToken = CancellationToken.None;
var failures = new List<(string File, string Error)>();

var connectionString = "mongodb://localhost:27017/?replicaSet=rs0";
var database = "MovieStore";
var dbConfig = new OptionsMonitorDbConfiguration(connectionString, database);
var store = new DataStore<MovieDocument>(dbConfig, "Movies");

var processed = 0;
var batchNumber = 0;

foreach (var batch in jsonFiles.Chunk(batchSize))
{
	batchNumber++;
	var batchLoaded = new List<MovieDocument>(capacity: batch.Length);
	foreach (var filePath in batch)
	{
		try
		{
			await using var stream = File.OpenRead(filePath);
			var item = await JsonSerializer.DeserializeAsync<MovieFile>(stream, serializerOptions, cancellationToken);

			if (item is null)
			{
				failures.Add((filePath, "Deserialized to null"));
				continue;
			}

			if (item.Movie is null)
			{
				failures.Add((filePath, "Missing 'movie' object"));
				continue;
			}

            var movie = Mapper.ToMovie(item);

			batchLoaded.Add(movie);
		}
		catch (JsonException ex)
		{
			failures.Add((filePath, $"Invalid JSON: {ex.Message}"));
		}
		catch (Exception ex)
		{
			failures.Add((filePath, ex.Message));
		}
	}

	if (batchLoaded.Count > 0)
	{
		try
		{
			await store.InsertAsync(batchLoaded, cancellationToken);
		}
		catch (MongoBulkWriteException<MovieDocument> ex)
		{
			var nonDuplicateErrors = ex.WriteErrors.Where(e => e.Code != 11000).ToList();
			if (nonDuplicateErrors.Count > 0)
				throw;

			Console.WriteLine($"Batch {batchNumber}: skipped {ex.WriteErrors.Count} duplicate documents");
		}
	}
	processed += batch.Length;
	Console.WriteLine($"Batch {batchNumber}: processed {processed}/{jsonFiles.Count} (loaded {batchLoaded.Count}, failed {failures.Count})");
}


Console.WriteLine($"Failed: {failures.Count}");

if (failures.Count > 0)
{
	Console.WriteLine();
	Console.WriteLine("Failures:");
	foreach (var (file, error) in failures.Take(10))
		Console.WriteLine($"  - {Path.GetFileName(file)}: {error}");

	if (failures.Count > 10)
		Console.WriteLine($"  ... and {failures.Count - 10} more");
}

internal class Mapper
{
    internal static MovieDocument ToMovie(MovieFile item)
    {
        return new MovieDocument(Guid.NewGuid().ToString(), 
                                    item.Movie?.Title ?? string.Empty, 
                                    item.Movie?.Genre ?? string.Empty, 
                                    int.TryParse(item.Movie?.Year, out var year) ? year : 0, 
                                    item.Movie?.Duration ?? string.Empty, 
                                    item.Movie?.Country ?? string.Empty, 
                                    item.Movie?.Director ?? string.Empty, 
                                    item.Movie?.Writer ?? string.Empty, 
                                    item.Movie?.Music ?? string.Empty, 
                                    item.Movie?.Photography ?? string.Empty, 
                                    item.Movie?.Producer ?? string.Empty, 
                                    item.Movie?.Cast ?? string.Empty, 
                                    item.Movie?.Synopsis ?? string.Empty,
                                    !string.IsNullOrWhiteSpace(item.Reviews?.Text) ? new[] { new MovieReview(item.Reviews.Text) } : Array.Empty<MovieReview>());    
    }
}

public sealed class MovieFile
{
	[JsonPropertyName("movie")]
	public Movie? Movie { get; init; }

	[JsonPropertyName("reviews")]
	public Reviews? Reviews { get; init; }
}

public sealed class Reviews
{
	[JsonPropertyName("Críticas")]
	public string? Text { get; init; }

	[JsonExtensionData]
	public Dictionary<string, JsonElement>? Extra { get; init; }
}

public sealed record Movie
{
	[JsonPropertyName("Título original")]
	public string? Title { get; init; }

	[JsonPropertyName("Año")]
	public string? Year { get; init; }

	[JsonPropertyName("Duración")]
	public string? Duration { get; init; }

	[JsonPropertyName("País")]
	public string? Country { get; init; }

	[JsonPropertyName("Director")]
	public string? Director { get; init; }

	[JsonPropertyName("Guión")]
	public string? Writer { get; init; }

	[JsonPropertyName("Música")]
	public string? Music { get; init; }

	[JsonPropertyName("Fotografía")]
	public string? Photography { get; init; }

	[JsonPropertyName("Reparto")]
	public string? Cast { get; init; }

	[JsonPropertyName("Productora")]
	public string? Producer { get; init; }

	[JsonPropertyName("Género")]
	public string? Genre { get; init; }

	[JsonPropertyName("Sinopsis")]
	public string? Synopsis { get; init; }
}


internal class OptionsMonitorDbConfiguration : IOptionsMonitor<DbConfigurationSettings>
{
    private readonly DbConfigurationSettings _config;

    public OptionsMonitorDbConfiguration(string connectionString, string database)
    {
        _config = new DbConfigurationSettings
        {
            ConnectionString = connectionString,
            Database = database
        };
    }

    public DbConfigurationSettings CurrentValue => _config;

    public DbConfigurationSettings Get(string name)
    {
        return _config;
    }

    public IDisposable OnChange(Action<DbConfigurationSettings, string> listener)
    {
        throw new NotImplementedException();
    }
}
