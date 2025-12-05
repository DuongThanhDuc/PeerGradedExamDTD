using Microsoft.AspNetCore.Mvc;
using Services;
using Models;

namespace Controllers ;

[ApiController]
[Route("api/[controller]")]
public class UserControllers : ControllerBase
{
    private readonly UserServices _userServices;

    public UserControllers(UserServices userServices)
    {
        _userServices = userServices;
    }

    [HttpGet]
    public IActionResult GetAllUsers()
    {
        var users = _userServices.GetAllUsers();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public IActionResult GetUserById(int id)
    {
        var user = _userServices.GetUserById(id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [HttpPost]
    public IActionResult CreateUser([FromBody] User user)
    {
        var createdUser = _userServices.CreateUser(user.Name, user.Age);
        return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateUser(int id, [FromBody] User user)
    {
        var updated = _userServices.UpdateUser(id, user.Name, user.Age);
        if (!updated)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteUser(int id)
    {
        var deleted = _userServices.DeleteUser(id);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }
}