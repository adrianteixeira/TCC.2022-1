namespace WebApi.Controllers;

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WebApi.Entities;
using WebApi.Models;
using WebApi.Models.Users;
using WebApi.Services;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private IUserService _userService;
    private IMapper _mapper;
    private readonly IUrlHelper _urlHelper;
    public UsersController(
        IUserService userService,
        IMapper mapper,
        IUrlHelper urlHelper)
    {
        _userService = userService;
        _mapper = mapper;
        _urlHelper = urlHelper;

    }

    [HttpGet(Name = nameof(GetAll))]
    public IActionResult GetAll()
    {
        var users = _userService.GetAll();
        users.ToList().ForEach(c => GerarLinks(c));

        return Ok(users);
    }

    [HttpGet, Route("{id:int}", Name = nameof(GetById))]
    public IActionResult GetById(int id)
    {
        var user = _userService.GetById(id);
        GerarLinks(user);
        return Ok(user);
    }

    [HttpPost(Name = nameof(Create))]
    public IActionResult Create(CreateRequest model)
    {
        
        var createdModel = _userService.Create(model);
        GerarLinks(createdModel);
        return Ok(new { message = "User created", createdModel.Links });
    }

    [HttpPut, Route("{id:int}", Name = nameof(Update))]
    public IActionResult Update(int id, UpdateRequest model)
    {
        var updatedModel = _userService.Update(id, model);
        GerarLinks(updatedModel);

        return Ok(new { message = "User updated", updatedModel.Links });
    }

    [HttpDelete, Route("{id:int}", Name = nameof(Delete))]
    public IActionResult Delete(int id)
    {
        _userService.Delete(id);
        return Ok(new { message = "User deleted" });
    }

    private void GerarLinks(User user)
    {
        user.Links.Add(new LinkDTO(_urlHelper.Link(nameof(GetById), new { id = user.Id }), rel: "self", method: "GET"));
        user.Links.Add(new LinkDTO(_urlHelper.Link(nameof(Update), new { id = user.Id }), rel: "update-user", method: "PUT"));
        user.Links.Add(new LinkDTO(_urlHelper.Link(nameof(Delete), new { id = user.Id }), rel: "delete-user", method: "DELETE"));
        // _context.SaveChanges();
    }

}