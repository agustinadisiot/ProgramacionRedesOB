using Common.Domain;
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
            return Task.FromResult(new MessageReply
            {
                Message = message
            });
        }

        public override Task<MessageReply> UpdateGame(GameDTO request, ServerCallContext context)
        {
            BusinessLogicGameCUD cud = BusinessLogicGameCUD.GetInstance();
            BusinessLogicUtils utils = BusinessLogicUtils.GetInstance();
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
            return Task.FromResult(new MessageReply
            {
                Message = message
            });
        }
    }
}
