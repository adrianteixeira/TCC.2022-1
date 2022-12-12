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
using WebApi.Services.Interface;

public interface IUserService
{
    IEnumerable<User> GetAll();
    User GetById(int id);
    User Create(CreateRequest model);
    User Update(int id, UpdateRequest model);
    void Delete(int id);
}

public class UserService : IUserService
{
    private DataContext _context;
    private readonly IMapper _mapper;
    private readonly IRedisService _redisService;
    public UserService(
        DataContext context,
        IMapper mapper,
        IRedisService redisService)
    {
        _context = context;
        _mapper = mapper;
        _redisService = redisService;

        //_redisService.Clear();
        JsonSerializerSettings ser = new JsonSerializerSettings
        {
            DefaultValueHandling = DefaultValueHandling.Ignore
        };

        if (!_context.Users.Any())
        {
            _context.Users.AddRange(JsonConvert.DeserializeObject<List<User>>(
                File.ReadAllText(@"MassaInicial.json"
            ), ser));
            _context.SaveChanges();
        }
    }

    public IEnumerable<User> GetAll()
    {
        return _context.Users;
    }

    public User GetById(int id)
    {
        var user = _redisService.ReadData(id);

        if (user != null)
        {
            return user;
        }
        else
        {
            user = getUser(id);
            if (user != null)
            {
                _redisService.SaveData(user);
            }
            return user;
        }
    }

    public User Create(CreateRequest model)
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
        _redisService.SaveData(user);
        return user;
    }

    public User Update(int id, UpdateRequest model)
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
        _redisService.UpdateData(user);
        return user;
    }

    public void Delete(int id)
    {
        var user = getUser(id);
        _context.Users.Remove(user);
        _context.SaveChanges();
        _redisService.RemoveData(id);
    }



    #region HelperMethods

    private User getUser(int id)
    {
        var user = _context.Users.Find(id);
        //if (user == null) throw new KeyNotFoundException("User not found");
        return user;
    }

    #endregion
}