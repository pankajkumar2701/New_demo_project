using Microsoft.AspNetCore.Mvc;
using Newdemoproject.Models;
using Newdemoproject.Data;
using Newdemoproject.Filter;
using Newdemoproject.Entities;
using Newdemoproject.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace Newdemoproject.Controllers
{
    /// <summary>
    /// Controller responsible for managing user-related operations in the API.
    /// </summary>
    /// <remarks>
    /// This controller provides endpoints for adding, retrieving, updating, and deleting user information.
    /// </remarks>
    [Route("api/user")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly NewdemoprojectContext _context;

        public UserController(NewdemoprojectContext context)
        {
            _context = context;
        }

        /// <summary>Adds a new user to the database</summary>
        /// <param name="model">The user data to be added</param>
        /// <returns>The result of the operation</returns>
        [HttpPost]
        [UserAuthorize("User",Entitlements.Create)]
        public IActionResult Post([FromBody] User model)
        {
            _context.User.Add(model);
            var returnData = this._context.SaveChanges();
            return Ok(returnData);
        }

        /// <summary>Retrieves a list of users based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"Property": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <returns>The filtered list of users</returns>
        [HttpGet]
        [UserAuthorize("User",Entitlements.Read)]
        public IActionResult Get([FromQuery] string filters)
        {
            List<FilterCriteria> filterCriteria = null;
            if (!string.IsNullOrEmpty(filters))
            {
                filterCriteria = JsonHelper.Deserialize<List<FilterCriteria>>(filters);
            }

            var query = _context.User.AsQueryable();
            var result = FilterService<User>.ApplyFilter(query, filterCriteria);
            return Ok(result);
        }

        /// <summary>Retrieves a specific user by its primary key</summary>
        /// <param name="entityId">The primary key of the user</param>
        /// <returns>The user data</returns>
        [HttpGet]
        [Route("{entityId:Guid}")]
        [UserAuthorize("User",Entitlements.Read)]
        public IActionResult GetById([FromRoute] Guid entityId)
        {
            var entityData = _context.User.FirstOrDefault(entity => entity.Id == entityId);
            return Ok(entityData);
        }

        /// <summary>Deletes a specific user by its primary key</summary>
        /// <param name="entityId">The primary key of the user</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete]
        [UserAuthorize("User",Entitlements.Delete)]
        [Route("{entityId:Guid}")]
        public IActionResult DeleteById([FromRoute] Guid entityId)
        {
            var entityData = _context.User.FirstOrDefault(entity => entity.Id == entityId);
            if (entityData == null)
            {
                return NotFound();
            }

            _context.User.Remove(entityData);
            var returnData = this._context.SaveChanges();
            return Ok(returnData);
        }

        /// <summary>Updates a specific user by its primary key</summary>
        /// <param name="entityId">The primary key of the user</param>
        /// <param name="updatedEntity">The user data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPut]
        [UserAuthorize("User",Entitlements.Update)]
        [Route("{entityId:Guid}")]
        public IActionResult UpdateById(Guid entityId, [FromBody] User updatedEntity)
        {
            if (entityId != updatedEntity.Id)
            {
                return BadRequest("Mismatched Id");
            }

            var entityData = _context.User.FirstOrDefault(entity => entity.Id == entityId);
            if (entityData == null)
            {
                return NotFound();
            }

            var propertiesToUpdate = typeof(User).GetProperties().Where(property => property.Name != "Id").ToList();
            foreach (var property in propertiesToUpdate)
            {
                property.SetValue(entityData, property.GetValue(updatedEntity));
            }

            var returnData = this._context.SaveChanges();
            return Ok(returnData);
        }
    }
}