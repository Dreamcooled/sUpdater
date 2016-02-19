using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SUpdater.Model;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.TvShows;
using ValueType = SUpdater.Model.ValueType;

namespace SUpdater.Provider
{
    public class TmdbProvider : IProvider
    {

        private readonly TMDbClient client;
        private const string API_KEY = "32b427eaba430fb1c86e9d15049e7799";
  

        private readonly List<ValueDefinition> _definitions = new List<ValueDefinition>(); 
        public TmdbProvider()
        {

            try
            {
                client = new TMDbClient(API_KEY);
                client.GetConfig();
                //client.MaxRetryCount = 15;
            }
            catch (Exception) { }



            _definitions.Add(new ValueDefinition(this, "Id", ValueType.Integer,EntityType.Show, ValueFetchStrategy.OnEntityCreate, ValueUpdateStrategy.Never ));  
            _definitions.Add(new ValueDefinition(this, "Title",ValueType.String, EntityType.Show, ValueFetchStrategy.OnEntityCreate, ValueUpdateStrategy.Never));
            _definitions.Add(new ValueDefinition(this, "Status", ValueType.String, EntityType.Show, ValueFetchStrategy.OnValueCreate, ValueUpdateStrategy.Never));
            _definitions.Add(new ValueDefinition(this, "Poster", ValueType.Image, EntityType.Show, ValueFetchStrategy.OnValueFetch, ValueUpdateStrategy.Never));
            _definitions.Add(new ValueDefinition(this, "Backdrop", ValueType.Image, EntityType.Show, ValueFetchStrategy.OnValueFetch, ValueUpdateStrategy.Never));
            _definitions.Add(new ValueDefinition(this, "ProviderHomepage", ValueType.Link, EntityType.Show, ValueFetchStrategy.OnValueCreate, ValueUpdateStrategy.Never));
            _definitions.Add(new ValueDefinition(this, "PublisherHomepage", ValueType.Link, EntityType.Show, ValueFetchStrategy.OnValueCreate, ValueUpdateStrategy.Never));

            _definitions.Add(new ValueDefinition(this, "Poster", ValueType.Image, EntityType.Season, ValueFetchStrategy.Never, ValueUpdateStrategy.Never));

            _definitions.Add(new ValueDefinition(this, "Name", ValueType.String, EntityType.Episode, ValueFetchStrategy.Never, ValueUpdateStrategy.Never));
            _definitions.Add(new ValueDefinition(this, "Overview", ValueType.String, EntityType.Episode, ValueFetchStrategy.Never, ValueUpdateStrategy.Never));
            _definitions.Add(new ValueDefinition(this, "Poster", ValueType.Image, EntityType.Episode, ValueFetchStrategy.Never, ValueUpdateStrategy.Never));



        }


        public string Name => "TMDB";
        public List<ValueDefinition> Values => _definitions;


        public int? FindShow(String name)
        {
            if (client?.Config == null) return null;
            Regex r = new Regex(@"\(([12]\d{3})\)"); //name contains year?
            Match m = r.Match(name);
            if (!m.Success)
            {
                var x = client.SearchTvShow(name);
                if (x.TotalResults > 0)
                {
                    return x.Results.First().Id;
                }
                return null;
            }
            else
            {
                int year = int.Parse(m.Groups[1].Value);
                name = name.Replace(m.Value, "").Trim();//remove year
                var x = client.SearchTvShow(name);
                foreach (var result in x.Results)
                {
                    if (result.FirstAirDate.HasValue && result.FirstAirDate.Value.Year == year)
                    {
                        return result.Id;
                    }
                }

                return null;
            }
        }


        private Value getVal(String name, Entity e)
        {
            bool dump;
            var val = e.AddGetValue(_definitions.First(d => d.Name == name), out dump);
            val.Requested = true;
            return val;
        }

        public bool RequestValue(Value value)
        {
            Console.WriteLine("[TMDB] Requested value " + value.Definition.EntityType + "(" + value.Entity.Name + ")." + value.Definition.Name);
            String name = value.Definition.Name;
            switch (value.Definition.EntityType)
            {
                case EntityType.Show:
                    switch (name)
                    {
                        case "Id":
                            int? id = FindShow(value.Entity.Name);
                            if (id.HasValue) value.SetValue(id.Value.ToString());
                            else return false;
                        
                            break;
                        case "Title":
                        case "Status":
                        case "Poster":
                        case "Backdrop":
                        case "ProviderHomepage":
                        case "PublisherHomepage":

                         
                            var titleVal = getVal("Title", value.Entity);
                            var statusVal = getVal("Status", value.Entity);
                            var posterVal = getVal("Poster", value.Entity);
                            var backdropVal = getVal("Backdrop", value.Entity);
                            var providerHomepageVal = getVal("ProviderHomepage", value.Entity);
                            var publisherHomepageVal = getVal("PublisherHomepage", value.Entity);


                            Task.Run(delegate
                            {
                                int id2 = int.Parse(value.Entity.Values["Id"].Data);
                                var showinfo = client.GetTvShow(id2, TvShowMethods.Images);
                                titleVal.SetValue(showinfo.OriginalName);
                                statusVal.SetValue(showinfo.Status);
                                posterVal.SetValue( String.IsNullOrWhiteSpace(showinfo.PosterPath)? null: client.GetImageUrl("original", showinfo.PosterPath).AbsoluteUri);
                                backdropVal.SetValue(String.IsNullOrWhiteSpace(showinfo.BackdropPath) ? null : client.GetImageUrl("original", showinfo.BackdropPath).AbsoluteUri);
                                providerHomepageVal.SetValue("https://www.themoviedb.org/tv/" + id2);
                                publisherHomepageVal.SetValue((showinfo.Homepage == null ||
                                                               showinfo.Homepage.Trim().Length == 0)
                                    ? null
                                    : showinfo.Homepage);

                            });
                            break;
                        default:
                            return false;
                    }
              
                    break;
                default:
                    return false;
            }
            return true;


        }

        public bool RequestEntities(Entity parent)
        {
            switch (parent.Type)
            {
                case EntityType.Show:
                    Task.Run(delegate
                    {
                        int id2 = int.Parse(parent.Values["Id"].Data);
                        var showinfo = client.GetTvShow(id2);
                        foreach (var tvSeason in showinfo.Seasons)
                        {
                            bool created;
                            var seasonEntity = parent.AddGetEntity(tvSeason.SeasonNumber.ToString(), EntityType.Season,
                                out created);
                            if (created)
                            {
                                seasonEntity.Init();
                            }
                            var posterVal = getVal("Poster",seasonEntity);
                            posterVal.SetValue(String.IsNullOrWhiteSpace(tvSeason.PosterPath)
                                ? null
                                : client.GetImageUrl("original", tvSeason.PosterPath).AbsoluteUri);
                            
                        }
                    });

                    break;

                case EntityType.Season:
                    Task.Run(delegate
                    {
                        int showId = int.Parse(parent.Parent.Values["Id"].Data);
                        int seasonNr = int.Parse(parent.Name);

                        var seasonInfo = client.GetTvSeason(showId, seasonNr,TvSeasonMethods.Images);
                        foreach (var tvEpisode in seasonInfo.Episodes)
                        {
                            bool created;
                            var episodeEntity = parent.AddGetEntity(tvEpisode.EpisodeNumber.ToString(), EntityType.Episode,
                                out created);
                            if (created)
                            {
                                episodeEntity.Init();
                            }
                            var nameVal = getVal("Name", episodeEntity);
                            nameVal.SetValue(tvEpisode.Name);
                            var overviewVal = getVal("Overview", episodeEntity);
                            overviewVal.SetValue(tvEpisode.Overview);
                            var posterVal = getVal("Poster", episodeEntity);
                            posterVal.SetValue(String.IsNullOrWhiteSpace(tvEpisode.StillPath)
                                ? null
                                : client.GetImageUrl("original", tvEpisode.StillPath).AbsoluteUri);
                        }
                      
                    });

                    break;
                default:
                    return false;
            }
            return true;
        }
    }
}
