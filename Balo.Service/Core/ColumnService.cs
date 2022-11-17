using Balo.Data.DataAccess;
using Balo.Data.MongoCollections;
using Balo.Data.ViewModels;
using Data.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;

namespace Balo.Service.Core
{   
    public interface IColumnService
    {
        Task<ResultModel> AddAsync(string userName, CreateColumnModel model);
        Task<PagingModel<Column>> GetAsync(string userName, GetColumnModel model, int? pageIndex = 0, int? pageSize = 10);
    }
    public class ColumnService : IColumnService
    {
        private readonly AppDbContext _dbContext;

        public ColumnService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ResultModel> AddAsync(string userName, CreateColumnModel model)
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
                        var column = model.Adapt<Column>();
                        column.Creater = existedUser;
                        column.BoardId = model.BoardId;
                        //insert column
                        await _dbContext.Columns.InsertOneAsync(column);
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

        public async Task<PagingModel<Column>> GetAsync(string userName, GetColumnModel model, int? pageIndex = 0, int? pageSize = 10)
        {
            var filters = Builders<Column>.Filter.Empty;

            if ( !(model.BoardId == null))
            {
                filters &= Builders<Column>.Filter.Eq(x => x.BoardId, model.BoardId);
            }

            if (!(model.GroupId == null))
            {
                filters &= Builders<Column>.Filter.Eq(x => x.Id, model.GroupId);
            }

            if (!(model.CreateId == null))
            {
                filters &= Builders<Column>.Filter.Eq(x => x.Creater.Id, model.CreateId);
            }

            if (!string.IsNullOrEmpty(model.Label))
            {
                filters &= Builders<Column>.Filter.Eq(x => x.Label, model.Label);
            }

            var query = _dbContext.Columns.Find(filters);
            var count = query.CountDocuments();
            return new PagingModel<Column>
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
