using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
            var cObject = from o in _context.CelestialObjects
                where o.Id.Equals(id)
                select o;
            if (!cObject.Any()) return NotFound();
            var satellites = from s in _context.CelestialObjects
                where s.OrbitedObjectId.Equals(id)
                select s;
            if (satellites.Any())
            {
                cObject.First().Satellites = satellites.ToList();
            }

            return Ok(cObject.First());
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestial = _context.CelestialObjects.FirstOrDefault(c => c.Name.Equals(name));
            if (celestial == null) return NotFound();
            var satellites = _context.CelestialObjects
                .Where(o => o.OrbitedObjectId.Equals(celestial.Id));
            if (satellites.Any())
            {
                celestial.Satellites = satellites.ToList();
            }
            return Ok(celestial);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestials = _context.CelestialObjects;
            if (celestials == null) return NotFound();
            foreach (var celestialObject in celestials)
            {
                var satellites = _context.CelestialObjects
                    .Where(c => c.OrbitedObjectId.Equals(celestialObject.Id));
                if (satellites.Any())
                {
                    celestialObject.Satellites = satellites.ToList();
                }
            }

            return Ok(celestials.ToList());
        }

        [HttpPost]
        public IActionResult Create(CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new {id = celestialObject.Id}, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject newObj)
        {
            var celestial = _context.CelestialObjects.FirstOrDefault(o => o.Id.Equals(id));
            if (celestial == null) return NotFound();
            celestial.Name = newObj.Name;
            celestial.OrbitedObjectId = newObj.OrbitedObjectId;
            celestial.OrbitalPeriod = newObj.OrbitalPeriod;
            _context.CelestialObjects.Update(celestial);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var celestial = _context.CelestialObjects.FirstOrDefault(o => o.Id.Equals(id));
            if (celestial == null) return NotFound();
            celestial.Name = name;
            _context.CelestialObjects.Update(celestial);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestials = _context.CelestialObjects
                .Where(o => o.Id.Equals(id) || o.OrbitedObjectId.Equals(id));
            if (!celestials.Any()) return NotFound();
            _context.CelestialObjects.RemoveRange(celestials);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
