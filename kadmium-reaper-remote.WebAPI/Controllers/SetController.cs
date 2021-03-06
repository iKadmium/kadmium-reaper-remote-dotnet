﻿using kadmium_reaper_remote_dotnet.Models;
using kadmium_reaper_remote_dotnet.Util;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace kadmium_reaper_remote_dotnet.Controllers
{
    [Route("api/[controller]")]
    public class SetController : Controller
    {
        private DatabaseContext _context;

        public SetController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/values
        [HttpGet]
        public async Task<IEnumerable<SetWithSongs>> Get()
        {
            var returnVal = new List<SetWithSongs>();

            var sets = from set in _context.Sets
                       orderby set.Date descending
                       select set;

            foreach (var set in sets)
            {
                var songs = await _context.LoadSongsForSet(set.Id);
                returnVal.Add(new SetWithSongs()
                {
                    Date = set.Date,
                    Id = set.Id,
                    Songs = songs,
                    Venue = set.Venue
                });
            }
            return returnVal;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<SetWithSongs> Get(int id)
        {
            var set = await _context.LoadSet(id);
            var setWithSongs = new SetWithSongs
            {
                Id = id,
                Date = set.Date,
                Venue = set.Venue,
                Songs = await _context.LoadSongsForSet(id)
            };
            return setWithSongs;
        }

        // POST api/values
        [HttpPost]
        public async Task<int> Post([FromBody]SetWithSongs value)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                Set set = new Set()
                {
                    Venue = value.Venue,
                    Date = value.Date
                };
                await _context.Sets.AddAsync(set);
                await _context.SaveChangesAsync();
                var relationships = from song in value.Songs
                                    select new SetSongRelationship()
                                    {
                                        Order = value.Songs.IndexOf(song),
                                        SetId = set.Id,
                                        SongId = song.Id
                                    };
                await _context.SetSongRelationships.AddRangeAsync(relationships);
                await _context.SaveChangesAsync();
                transaction.Commit();
                return set.Id;
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task Put(int id, [FromBody]SetWithSongs value)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                Set set = new Set()
                {
                    Id = id,
                    Venue = value.Venue,
                    Date = value.Date
                };
                var dbSongs = _context.SetSongRelationships.Where(x => x.SetId == id);
                _context.SetSongRelationships.RemoveRange(dbSongs);
                var relationships = from song in value.Songs
                                    select new SetSongRelationship()
                                    {
                                        SetId = id,
                                        SongId = song.Id,
                                        Order = value.Songs.IndexOf(song)
                                    };
                await _context.SaveChangesAsync();
                _context.Sets.Update(set);
                await _context.SetSongRelationships.AddRangeAsync(relationships);
                await _context.SaveChangesAsync();
                transaction.Commit();
            }
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            var set = await _context.LoadSet(id);
            _context.Sets.Remove(set);
            var relationships = _context.SetSongRelationships.Where(x => x.SetId == id);
            _context.SetSongRelationships.RemoveRange(relationships);
            await _context.SaveChangesAsync();
        }

        [HttpPost("[action]/{name}")]
        public async Task Activate(string name)
        {
            var client = new HttpClient();
            try
            {
                var response = await client.GetAsync(Settings.Instance.LightingVenueURI + "/" + name);
            }
            catch(HttpRequestException e)
            {
                throw e.InnerException;
            }
            
        }
    }

    public class SetSkeleton
    {
        public int Id { get; set; }
        public string Venue { get; set; }
        public DateTime Date { get; set; }
    };

    public class SetWithSongs : Set
    {
        public List<Song> Songs;
    }
}