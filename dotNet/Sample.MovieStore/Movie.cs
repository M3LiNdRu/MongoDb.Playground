using System;
using MongoDb.Playground.Repository;

namespace Sample.MovieStore;

public record MovieReview(string Text);

public class MovieDocument : ICollectionDocument
{
    public string Id { get; set; }
    public string Title { get; private set; }
    public string Genre { get; private set; }
    public int Year { get; private set; }
    public string Duration { get; private set; }
    public string Country { get; private set; }
    public string Director { get; private set; }
    public string Writer { get; private set; } 
    public string Music { get; private set; }
    public string Photography { get; private set; }
    public string Producer { get; private set; } 
    public string Cast { get; private set; }
    public string Synopsis { get; private set; }
    public IEnumerable<MovieReview> Reviews { get; private set; }

    public MovieDocument(  
                            string id, 
                            string title, 
                            string genre, 
                            int year, 
                            string duration, 
                            string country, 
                            string director, 
                            string writer, 
                            string music, 
                            string photography, 
                            string producer, 
                            string cast, 
                            string synopsis,
                            IEnumerable<MovieReview> reviews)
    {
        Id = id;
        Title = title;
        Genre = genre;
        Year = year;
        Duration = duration;
        Country = country;
        Director = director;
        Writer = writer;
        Music = music;
        Photography = photography;
        Producer = producer;
        Cast = cast;
        Synopsis = synopsis;
        Reviews = reviews;
    }
}