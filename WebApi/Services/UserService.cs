namespace WebApi.Services;

using System.Text.Json;
using AutoMapper;
using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models;
using WebApi.Models.Users;

public interface IUserService
{
    IEnumerable<User> GetAll();
    User GetById(int id);
    void Create(CreateRequest model);
    void Update(int id, UpdateRequest model);
    void Delete(int id);
}

public class UserService : IUserService
{
    private DataContext _context;
    private readonly IMapper _mapper;

    private readonly IUrlHelper _urlHelper;
    public UserService(
        DataContext context,
        IMapper mapper,
        IUrlHelper urlHelper)
    {
        _context = context;
        _mapper = mapper;
        
        _urlHelper = urlHelper;
        _context.Users.AddRange(JsonConvert.DeserializeObject<List<User>>(
            File.ReadAllText(@"MassaInicial.json"
        )));
        _context.SaveChanges();
    }

    public IEnumerable<User> GetAll()
    {
        foreach (var user in _context.Users)
        {
            GerarLinks(user);
        }
        return _context.Users;
    }

    public User GetById(int id)
    {
        return getUser(id);
    }

    public void Create(CreateRequest model)
    {
        // validate
        if (_context.Users.Any(x => x.Email == model.Email))
            throw new AppException("User with the email '" + model.Email + "' already exists");

        // map model to new user object
        var user = _mapper.Map<User>(model);

        // hash password
        user.PasswordHash = BCrypt.HashPassword(model.Password);

        // save user
        _context.Users.Add(user);
        _context.SaveChanges();
    }

    public void Update(int id, UpdateRequest model)
    {
        var user = getUser(id);

        // validate
        if (model.Email != user.Email && _context.Users.Any(x => x.Email == model.Email))
            throw new AppException("User with the email '" + model.Email + "' already exists");

        // hash password if it was entered
        if (!string.IsNullOrEmpty(model.Password))
            user.PasswordHash = BCrypt.HashPassword(model.Password);

        // copy model to user and save
        _mapper.Map(model, user);
        _context.Users.Update(user);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var user = getUser(id);
        _context.Users.Remove(user);
        _context.SaveChanges();
    }

    // helper methods

    private User getUser(int id)
    {
        var user = _context.Users.Find(id);
        if (user == null) throw new KeyNotFoundException("User not found");
        return user;
    }

    private void GerarLinks(User user)
        {
            // Console.WriteLine(_urlHelper.Link(nameof(GetById), new { id = user.Id }));
            user.Links.Add(new LinkDTO("teste12", rel: "self", metodo: "GET"));
            user.Links.Add(new LinkDTO(_urlHelper.Link(nameof(Update), new { id = user.Id }), rel: "update-user", metodo: "PUT"));
            user.Links.Add(new LinkDTO(_urlHelper.Link(nameof(Delete), new { id = user.Id }), rel: "delete-user", metodo: "DELETE"));
            // _context.SaveChanges();
        }

}