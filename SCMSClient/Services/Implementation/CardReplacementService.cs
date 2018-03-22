﻿using SCMSClient.Models;
using SCMSClient.Services.Interfaces;
using SCMSClient.Utilities;
using System.Collections.Generic;

namespace SCMSClient.Services.Implementation
{
    public class CardReplacementService : AbstractService<SOAReplaceCardRequest>, ICardReplacementService
    {
        public CardReplacementService(IHTTPService _httpService) : base(_httpService)
        {
            getUrl = ApiEndpoints.AllCardReplacementRequests;
            getAllUrl = ApiEndpoints.FindCardReplacementRequestById;
            updateUrl = ApiEndpoints.UpdateCardReplacementRequest;
            createUrl = ApiEndpoints.CreateCardReplacementRequest;
            deleteUrl = ApiEndpoints.DeleteCardReplacementRequest;
        }

        public override List<SOAReplaceCardRequest> GetAll()
        {
            return allObjects ?? (allObjects = RandomDataGenerator.ReplaceCardRequests(50));
        }

        public override SOAReplaceCardRequest Get(string parameter)
        {
            return allObjects.Find(c => c.ID == parameter);
        }
    }
}