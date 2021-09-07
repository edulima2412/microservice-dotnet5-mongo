using Catalog.Api.Entities;
using Catalog.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.Api.Controllers
{
    [ApiController]
    [Route("api/items")]
    public class ItemsController : ControllerBase
    {
        private readonly InMemItemsRepository _repository;

        public ItemsController(InMemItemsRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Item>> GetItems()
        {
            var items = _repository.GetItems();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public ActionResult<IEnumerable<Item>> GetItem(Guid id)
        {
            var item = _repository.GetItem(id);
            return Ok(item);
        }
    }
}
