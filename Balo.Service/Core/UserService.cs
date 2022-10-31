using Balo.Data.DataAccess;
using Balo.Data.MongoCollections;
using Balo.Data.ViewModels;
using Data.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;

namespace Balo.Service.Core
{   
    public interface IUserService
    {
        Task<ResultModel> Get(Guid id);
        Task<ResultModel> AddAsync(string userName, CreateUserModel model);
        Task Update(Guid id, UpdateUserModel model);
        Task Delete(Guid id);

        Task<PagingModel<User>> GetPagingData(string? name,Guid? code, int? pageIndex = 0, int? pageSize = 10);
    }
    public class UserService : IUserService
    {
        private readonly AppDbContext _dbContext;

        public UserService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ResultModel> AddAsync(string userName, CreateUserModel model)
        {
            var resultModel = new ResultModel();
            var user = model.Adapt<User>();
            user.UserName = userName;
            try {
                var existedUser = await _dbContext.Users.Find(x => x.UserName == userName).FirstOrDefaultAsync();
                if (existedUser == null)
                {
                    await _dbContext.Users.InsertOneAsync(user);
                    resultModel.Succeed = true;
                    resultModel.Data = user.Id;
                } else
                {
                    throw new Exception("The user already existed");
                }
                   

            }
            catch (Exception ex)
            {
                resultModel.Succeed = false;
                resultModel.ErrorMessage = ex.Message;
            }

            return resultModel;
        }

        public async Task Delete(Guid id)
        {
            var deleteFilter = Builders<User>.Filter.Eq(x => x.Id, id);
            await _dbContext.Users.DeleteOneAsync(deleteFilter);
        }

        public async Task<ResultModel> Get(Guid id)
        {
            var result = new ResultModel();
            try
            {
                var user = await _dbContext.Users.Find(x => x.Id == id).FirstOrDefaultAsync();
                if (user == null)
                {
                    throw new Exception("Cant find any user!");
                }
                result.Data = user;
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }

        public async Task<PagingModel<User>> GetPagingData(string? name, Guid? id, int? pageIndex = 0, int? pageSize = 10)
        {
            var filters = Builders<User>.Filter.Empty;
            if (!string.IsNullOrEmpty(name))
            {
                filters &= Builders<User>.Filter.Eq(x => x.UserName, name);
                filters &= Builders<User>.Filter.Eq(x => x.FullName, name);
            }
            if (id != null)
            {
                filters &= Builders<User>.Filter.Eq(x => x.Id, id);
            }
            var query = _dbContext.Users.Find(filters);
            var count = query.CountDocuments();

            return new PagingModel<User>
            {
                TotalPages = (int)Math.Ceiling((double)count / pageSize.Value),
                TotalRows = count,
                Data = await query
                        .Skip(pageIndex * pageSize)
                        .Limit(pageSize)
                        .ToListAsync(),
            };
        }

        public async Task Update(Guid id, UpdateUserModel model)
        {
            var user = model.Adapt<User>();
            await _dbContext.Users.ReplaceOneAsync(x => x.Id == id, user);
        }
    }
}
