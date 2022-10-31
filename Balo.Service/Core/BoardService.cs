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
    public interface IBoardService
    {
        Task<ResultModel> AddAsync(string username, BoardCreateModel model);
        Task<ResultModel> Get(Guid id);
        Task<ResultModel> GetBoardMembers(Guid id);
        Task Update(Guid id, Board model);
        Task Delete(Guid id);

        Task<PagingModel<Board>> GetPagingData(Guid userId, int? pageIndex = 0, int? pageSize = 10);
    }
    public class BoardService : IBoardService
    {
        private readonly AppDbContext _dbContext;

        public BoardService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ResultModel> AddAsync(string username, BoardCreateModel model)
        {
            var resultModel = new ResultModel();
            var session = _dbContext.StartSession(); session.StartTransaction();
            try
            {
                var user = await _dbContext.Users.Find(x => x.UserName == username).FirstOrDefaultAsync();
                if (user != null)
                {
                    var board = model.Adapt<Board>();
                    var boardUser = user.Adapt<User>();
                    boardUser.Role = Data.Enums.Role.Owner;
                    board.Members.Add(boardUser);
                    await _dbContext.Boards.InsertOneAsync(board);
                    resultModel.Succeed = true;
                    resultModel.Data = board.Id;
                    await session.CommitTransactionAsync();
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

        public async Task Delete(Guid id)
        {
            var deleteFilter = Builders<Board>.Filter.Eq(x => x.Id, id);
            await _dbContext.Boards.DeleteOneAsync(deleteFilter);
        }

        public async Task<ResultModel> Get(Guid id)
        {
            var result = new ResultModel();
            try
            {
                var board = await _dbContext.Boards.Find(x => x.Id == id).FirstOrDefaultAsync();
                if (board == null)
                {
                    throw new Exception("Cant find any board!");
                }
                result.Data = board;
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }

        public async Task Update(Guid id, Board model)
        {
            await _dbContext.Boards.ReplaceOneAsync(x => x.Id == id, model);
        }

        public async Task<PagingModel<Board>> GetPagingData(Guid userId, int? pageIndex = 0, int? pageSize = 10)
        {
            var filters = Builders<Board>.Filter.Empty;
            if(!userId.Equals(Guid.Empty))
            {
                filters &= Builders<Board>.Filter.ElemMatch(x => x.Members, Builders<User>.Filter.Eq(y => y.Id, userId));

            }

            var query = _dbContext.Boards.Find(filters);
            var count = query.CountDocuments();
            return new PagingModel<Board>
            {
                TotalPages = (int)Math.Ceiling((double)count / pageSize.Value),
                TotalRows = count,
                Data = await query
                        .Skip(pageIndex * pageSize)
                        .Limit(pageSize)
                        .ToListAsync(),
            };
        }

        public async Task<ResultModel> GetBoardMembers(Guid id)
        {
            var result = new ResultModel();
            try
            {
                var board = await _dbContext.Boards.Find(x => x.Id == id).FirstOrDefaultAsync();
                if (board == null)
                {
                    throw new Exception("Cant find any board!");
                }
                result.Data = board.Members;
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }
    }
}
