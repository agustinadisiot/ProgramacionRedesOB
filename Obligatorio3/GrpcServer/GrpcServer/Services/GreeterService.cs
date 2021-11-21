using Common.Domain;
using Common.Protocol;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Server.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }

        public override Task<MessageReply> PostGame(GameDTO request, ServerCallContext context)
        {
            BusinessLogicGameCUD cud = BusinessLogicGameCUD.GetInstance();
            BusinessLogicUtils utils = BusinessLogicUtils.GetInstance();
            try
            {
                Game game = new Game()
                {
                    Title = request.Title,
                    ESRBRating = (Common.ESRBRating)request.EsrbRating,
                    CoverFilePath = request.CoverFilePath,
                    Genre = request.Genre,
                    Publisher = utils.GetUser(request.PublisherId),
                    Synopsis = request.Synopsis,
                    Reviews = new List<Review>()
                };
                string message = cud.PublishGame(game);
                return Task.FromResult(new MessageReply { Message = message });
            }
            catch (ServerError e)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, e.Message));
            }
            catch (TitleAlreadyExistsException e)
            {
                throw new RpcException(new Status(StatusCode.AlreadyExists, e.Message));
            }
        }

        public override Task<MessageReply> UpdateGame(GameDTO request, ServerCallContext context)
        {
            BusinessLogicGameCUD cud = BusinessLogicGameCUD.GetInstance();
            try
            {
                Game game = new Game()
                {
                    Id = request.Id,
                    Title = request.Title,
                    ESRBRating = (Common.ESRBRating)request.EsrbRating,
                    CoverFilePath = request.CoverFilePath,
                    Genre = request.Genre,
                    Synopsis = request.Synopsis
                };
                string message = cud.ModifyGame(request.Id, game);
                return Task.FromResult(new MessageReply { Message = message });
            }
            catch (ServerError e)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, e.Message));
            }
            catch (TitleAlreadyExistsException e)
            {
                throw new RpcException(new Status(StatusCode.AlreadyExists, e.Message));
            }
        }

        public override Task<MessageReply> DeleteGame(Id request, ServerCallContext context)
        {
            BusinessLogicGameCUD cud = BusinessLogicGameCUD.GetInstance();
            bool couldDelete = cud.DeleteGame(request.Id_);
            string message = couldDelete ? "Juego eliminado corretamente" : "No se pudo eliminar el juego";
            return Task.FromResult(new MessageReply{Message = message});
        }

        public override Task<MessageReply> PostUser(UserDTO request, ServerCallContext context)
        {
            BusinessLogicSession session = BusinessLogicSession.GetInstance();
            string message = session.CreateUser(request.Name);
            return Task.FromResult(new MessageReply { Message = message });
        }

        public override Task<MessageReply> UpdateUser(UserDTO request, ServerCallContext context)
        {
            BusinessLogicSession session = BusinessLogicSession.GetInstance();
            try { 
            User modifiedUser = new User()
            {
                Id = request.Id,
                Name = request.Name
            };
            string message = session.ModifyUser(request.Id, modifiedUser);
            return Task.FromResult(new MessageReply { Message = message }); }
            catch (ServerError e)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, e.Message));
            }
            catch (NameAlreadyExistsException e)
            {
                throw new RpcException(new Status(StatusCode.AlreadyExists, e.Message));
            }
        }

        public override Task<MessageReply> DeleteUser(Id request, ServerCallContext context)
        {
            BusinessLogicSession session = BusinessLogicSession.GetInstance();
            bool couldDelete = session.DeleteUser(request.Id_);
            string message = couldDelete ? "Usuario eliminado correctamente" : "No se pudo eliminar usuario";
            return Task.FromResult(new MessageReply { Message = message });
        }

        public override Task<MessageReply> AssociateGameWithUser(Purchase request, ServerCallContext context)
        {
            BusinessLogicGameInfo info = BusinessLogicGameInfo.GetInstance();
            try {
                bool couldBuy = info.BuyGame(request.IdGame, request.IdUser);
                string message = couldBuy ? "Juego comprado correctamente" : "No se pudo comprar juego";
                return Task.FromResult(new MessageReply { Message = message });
            }
            catch (ServerError e)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, e.Message));
            }
        }

        public override Task<MessageReply> DisassociateGameWithUser(Purchase request, ServerCallContext context)
        {
            BusinessLogicGameInfo info = BusinessLogicGameInfo.GetInstance();
            try { 
            bool couldReturn = info.ReturnGame(request.IdGame, request.IdUser);
            string message = couldReturn ? "Se retorno el juego correctamente" : "No se pudo retornar el juego";
            return Task.FromResult(new MessageReply { Message = message }); }
            catch (ServerError e)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, e.Message));
            }
        }
    }
}
