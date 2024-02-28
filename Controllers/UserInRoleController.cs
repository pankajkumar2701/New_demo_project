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
    /// Controller responsible for managing userinrole-related operations in the API.
    /// </summary>
    /// <remarks>
    /// This controller provides endpoints for adding, retrieving, updating, and deleting userinrole information.
    /// </remarks>
    [Route("api/userinrole")]
    [Authorize]
    public class UserInRoleController : ControllerBase
    {
        private readonly NewdemoprojectContext _context;

        public UserInRoleController(NewdemoprojectContext context)
        {
            _context = context;
        }

        /// <summary>Adds a new userinrole to the database</summary>
        /// <param name="model">The userinrole data to be added</param>
        /// <returns>The result of the operation</returns>
        [HttpPost]
        [UserAuthorize("UserInRole",Entitlements.Create)]
        public IActionResult Post([FromBody] UserInRole model)
        {
            _context.UserInRole.Add(model);
            var returnData = this._context.SaveChanges();
            return Ok(returnData);
        }

        /// <summary>Retrieves a list of userinroles based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"Property": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <returns>The filtered list of userinroles</returns>
        [HttpGet]
        [UserAuthorize("UserInRole",Entitlements.Read)]
        public IActionResult Get([FromQuery] string filters)
        {
            List<FilterCriteria> filterCriteria = null;
            if (!string.IsNullOrEmpty(filters))
            {
                filterCriteria = JsonHelper.Deserialize<List<FilterCriteria>>(filters);
            }

            var query = _context.UserInRole.AsQueryable();
            var result = FilterService<UserInRole>.ApplyFilter(query, filterCriteria);
            return Ok(result);
        }

        /// <summary>Retrieves a specific userinrole by its primary key</summary>
        /// <param name="entityId">The primary key of the userinrole</param>
        /// <returns>The userinrole data</returns>
        [HttpGet]
        [Route("{entityId:Guid}")]
        [UserAuthorize("UserInRole",Entitlements.Read)]
        public IActionResult GetById([FromRoute] Guid entityId)
        {
            var entityData = _context.UserInRole.FirstOrDefault(entity => entity.Id == entityId);
            return Ok(entityData);
        }

        /// <summary>Deletes a specific userinrole by its primary key</summary>
        /// <param name="entityId">The primary key of the userinrole</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete]
        [UserAuthorize("UserInRole",Entitlements.Delete)]
        [Route("{entityId:Guid}")]
        public IActionResult DeleteById([FromRoute] Guid entityId)
        {
            var entityData = _context.UserInRole.FirstOrDefault(entity => entity.Id == entityId);
            if (entityData == null)
            {
                return NotFound();
            }

            _context.UserInRole.Remove(entityData);
            var returnData = this._context.SaveChanges();
            return Ok(returnData);
        }

        /// <summary>Updates a specific userinrole by its primary key</summary>
        /// <param name="entityId">The primary key of the userinrole</param>
        /// <param name="updatedEntity">The userinrole data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPut]
        [UserAuthorize("UserInRole",Entitlements.Update)]
        [Route("{entityId:Guid}")]
        public IActionResult UpdateById(Guid entityId, [FromBody] UserInRole updatedEntity)
        {
            if (entityId != updatedEntity.Id)
            {
                return BadRequest("Mismatched Id");
            }

            var entityData = _context.UserInRole.FirstOrDefault(entity => entity.Id == entityId);
            if (entityData == null)
            {
                return NotFound();
            }

            var propertiesToUpdate = typeof(UserInRole).GetProperties().Where(property => property.Name != "Id").ToList();
            foreach (var property in propertiesToUpdate)
            {
                property.SetValue(entityData, property.GetValue(updatedEntity));
            }

            var returnData = this._context.SaveChanges();
            return Ok(returnData);
        }
    }
}