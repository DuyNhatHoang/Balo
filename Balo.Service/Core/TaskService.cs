using Balo.Data.DataAccess;
using Balo.Data.MongoCollections;
using Balo.Data.ViewModels;
using Data.ViewModels;
using Mapster;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balo.Service.Core
{
    public interface ITaskService
    {
        Task<ResultModel> AddAsync(string userName, CreateTaskModel model);
        Task<PagingModel<PlannedTask>> GetPagingData(GetTaskModel model, int? pageIndex = 0, int? pageSize = 10);
    }

    public class TaskService : ITaskService
    {

        private readonly AppDbContext _dbContext;

        public TaskService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ResultModel> AddAsync(string userName, CreateTaskModel model)
        {
            var resultModel = new ResultModel();
            var user = model.Adapt<User>();
            user.UserName = userName;
            try {
                var existedUser = await _dbContext.Users.Find(x => x.UserName == userName).FirstOrDefaultAsync();
                if (existedUser != null)
                {
                    var task = model.Adapt<PlannedTask>();
                    task.Members.Clear();
                    task.Groups.Clear();
                    if(!model.ColumnId.Equals(Guid.Empty))
                    {
                        var column = await _dbContext.Columns.Find(x => x.Id == model.ColumnId).FirstOrDefaultAsync();
                        task.Column = column;
                    } else
                    {
                        throw new Exception("Column id cannot be empty");
                    }
                    if (model.Groups.Count > 0)
                    {
                        for (var i = 0; i < model.Groups.Count; i++)
                        {
                            var group = await _dbContext.Groups.Find(x => x.Id == model.Groups.ElementAt(i)).FirstOrDefaultAsync();
                            task.Groups.Add(group);
                        }
                    }
                   
                    if(model.Members.Count > 0)
                    {   
                        var fuser = await _dbContext.Users.Find(x => x.UserName == userName).FirstOrDefaultAsync();
                        task.Members.Add(fuser);
                    }

                    await _dbContext.Tasks.InsertOneAsync(task);
                    resultModel.Succeed = true;
                    resultModel.Data = task.Id;
                } else
                {
                    throw new Exception("The user not existed");
                }
                   

            }
            catch (Exception ex)
            {
                resultModel.Succeed = false;
                resultModel.ErrorMessage = ex.Message;
            }

            return resultModel;
        }

        public async Task<PagingModel<PlannedTask>> GetPagingData(GetTaskModel model, int? pageIndex = 0, int? pageSize = 10)
        {
            var filters = Builders<PlannedTask>.Filter.Empty;
            if (!model.GroupId.Equals(Guid.Empty))
            {
                filters &= Builders<PlannedTask>.Filter.ElemMatch(x => x.Groups, Builders<Group>.Filter.Eq(y => y.Id, model.GroupId));
            }

            if (!model.MemberId.Equals(Guid.Empty))
            {
                filters &= Builders<PlannedTask>.Filter.ElemMatch(x => x.Members, Builders<User>.Filter.Eq(y => y.Id, model.MemberId));
            }

            if (!model.ColumnId.Equals(Guid.Empty))
            {
                filters &= Builders<PlannedTask>.Filter.Eq(x => x.Column.Id, model.ColumnId);
            }

            if (model.Priority != null)
            {
                Builders<User>.Filter.Eq(y => y.Id, model.MemberId);
            }

            var query = _dbContext.Tasks.Find(filters);
            var count = query.CountDocuments();
            return new PagingModel<PlannedTask>
            {
                TotalPages = (int)Math.Ceiling((double)count / pageSize.Value),
                TotalRows = count,
                Data = await query
                        .Skip(pageIndex * pageSize)
                        .Limit(pageSize)
                        .ToListAsync(),
            };
        }

    }
}
