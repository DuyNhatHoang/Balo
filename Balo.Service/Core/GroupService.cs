using Balo.Data.DataAccess;
using Balo.Data.MongoCollections;
using Balo.Data.ViewModels;
using Data.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;

namespace Balo.Service.Core
{   
    public interface IGroupService
    {
        Task<ResultModel> AddAsync(string userName, CreateGroupModel model);
        Task<PagingModel<Group>> GetAsync(string userName, GetGroupModel model, int? pageIndex = 0, int? pageSize = 10);
    }
    public class GroupService : IGroupService
    {
        private readonly AppDbContext _dbContext;

        public GroupService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ResultModel> AddAsync(string userName, CreateGroupModel model)
        {
            var resultModel = new ResultModel();
            var user = model.Adapt<User>();
            user.UserName = userName;
            var session = _dbContext.StartSession(); session.StartTransaction();
            try {
                var existedUser = await _dbContext.Users.Find(x => x.UserName == userName).FirstOrDefaultAsync();
                if (existedUser != null)
                {
                    // check board existed or not   
                    var existedBoard = await _dbContext.Boards.Find(x => x.Id == model.BoardId).FirstOrDefaultAsync();
                    if (existedBoard == null)
                    {
                        throw new Exception("The board is not existed");
                    } else
                    {
                        var group = model.Adapt<Group>();
                        group.Creater = existedUser;
                        group.BoardId = model.BoardId;
                        //insert group
                        await _dbContext.Groups.InsertOneAsync(group);
                        ////insert group into board
                        //if(existedBoard.Groups == null)
                        //{
                        //    existedBoard.Groups = new List<Group>();    
                        //}
                        //existedBoard.Groups.Add(group);

                        await _dbContext.Boards.ReplaceOneAsync(x => x.Id == model.BoardId, existedBoard);
                        resultModel.Succeed = true;
                        resultModel.Data = user.Id;
                        await session.CommitTransactionAsync();
                    }

                } else
                {
                    throw new Exception("The user not existed");
                }


            }
            catch (Exception ex)
            {
                resultModel.Succeed = false;
                resultModel.ErrorMessage = ex.Message;
                await session.AbortTransactionAsync();
            }

            return resultModel;
        }

        public async Task<PagingModel<Group>> GetAsync(string userName, GetGroupModel model, int? pageIndex = 0, int? pageSize = 10)
        {
            var filters = Builders<Group>.Filter.Empty;

            if ( !(model.BoardId == null))
            {
                filters &= Builders<Group>.Filter.Eq(x => x.BoardId, model.BoardId);
            }

            if (!(model.GroupId == null))
            {
                filters &= Builders<Group>.Filter.Eq(x => x.Id, model.GroupId);
            }

            if (!(model.CreateId == null))
            {
                filters &= Builders<Group>.Filter.Eq(x => x.Creater.Id, model.CreateId);
            }

            if (!string.IsNullOrEmpty(model.Label))
            {
                filters &= Builders<Group>.Filter.Eq(x => x.Label, model.Label);
            }
            var query = _dbContext.Groups.Find(filters);
            var count = query.CountDocuments();
            return new PagingModel<Group>
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
