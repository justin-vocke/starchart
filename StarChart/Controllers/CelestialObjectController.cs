using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("{id:int}")]

        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.Where(x => x.Id == id).FirstOrDefault();
            if (!(celestialObject is null))
            {
                celestialObject.Satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == celestialObject.Id).ToList();
                return Ok(celestialObject);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(x => x.Name == name).ToList();
            if (celestialObjects.Count > 0)
            {
                foreach (var item in celestialObjects)
                {
                    item.Satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == item.Id).ToList();
                }
            }
            else
            {
                return NotFound();
            }
            return Ok(celestialObjects);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects.ToList();
            foreach (var item in celestialObjects)
            {
                item.Satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == item.Id).ToList();
            }
            return Ok(celestialObjects);
        }
        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject co)
        {
            
            _context.Add(co);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new {id = co.Id }, co);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject co)
        {
            var celestialObjectToUpdate = _context.CelestialObjects.Where(x => x.Id == id).FirstOrDefault();
            if(!(celestialObjectToUpdate is null))
            {
                celestialObjectToUpdate.Name = co.Name;
                celestialObjectToUpdate.OrbitalPeriod = co.OrbitalPeriod;
                celestialObjectToUpdate.OrbitedObjectId = co.OrbitedObjectId;

                _context.Update(celestialObjectToUpdate);
                _context.SaveChanges();
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var coToPatch = _context.CelestialObjects.Where(x => x.Id == id).FirstOrDefault();
            if(!(coToPatch is null))
            {
                coToPatch.Name = name;
                _context.Update(coToPatch);
                _context.SaveChanges();
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var cosToDelete = _context.CelestialObjects.Where(x => x.Id == id || x.OrbitedObjectId == id).ToList();
            if(cosToDelete.Count > 0)
            {
                _context.RemoveRange(cosToDelete);
                _context.SaveChanges();
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

    }
    public class CreatedCelestialObject
    {
        public int Id { get; set; }
        public CelestialObject CreatedCO { get; set; }
    }
}


