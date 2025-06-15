using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;
using semsary_backend.EntityConfigurations;
using semsary_backend.Models;
using System;

namespace semsary_backend.Service
{
    public class NotificationService
    {
        private static bool _isInitialized = false;
        private static readonly object _lock = new();
        private readonly ApiContext context;

        public NotificationService(ApiContext apiContext)
        {
            InitializeFirebase();
            context = apiContext;
        }

        private void InitializeFirebase()
        {
            if (_isInitialized) 
                return;

            lock (_lock)
            {
                if (!_isInitialized)
                {
                    FirebaseApp.Create(new AppOptions
                    {
                        Credential = GoogleCredential.FromFile("private_key2.json")
                    });
                    _isInitialized = true;
                }
            }
        }

        public async Task SendNotificationAsync(string title, string message, SermsaryUser user)
        {
            List<string> deviceTokens ;
            if (user is Tenant tenant)
            {
                deviceTokens = tenant.DeviceTokens;
            }
            else if (user is Landlord landlord)
            {
                deviceTokens = landlord.DeviceTokens;
            }
            else if (user is CustomerService customerService)
            {
                deviceTokens = customerService.DeviceTokens;
            }
            else
                return;

            if (deviceTokens == null || deviceTokens.Count == 0)
                return;

            var multicastMessage = new MulticastMessage
            {
                Data = new Dictionary<string, string>
                {
                    { "title", title },
                    { "body", message }
                },
                Tokens = deviceTokens
            };

            var response = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(multicastMessage);
            await RemoveInvalidDeviceTokens(user, response, deviceTokens);
        }
        public async Task RemoveInvalidDeviceTokens(SermsaryUser user, BatchResponse response , List<string> deviceTokens) 
        {
            var tokensToRemove = new List<string>();

            for (int i = 0; i < response.Responses.Count; i++)
            {
                var result = response.Responses[i];
                if (!result.IsSuccess)
                {
                    var failedToken = deviceTokens[i];
                    var errorCode = result.Exception?.Message ?? "";

                    if (errorCode.Contains("registration-token-not-registered") || errorCode.Contains("invalid-argument"))
                    {
                        tokensToRemove.Add(failedToken);
                    }
                }
            }

            foreach (var token in tokensToRemove)
            {
                deviceTokens.Remove(token);
            }

            if (user is Tenant tenant)
            {
                tenant.DeviceTokens = deviceTokens;
                context.Tenant.Update(tenant); 
            }
            else if (user is Landlord landlord)
            {
                landlord.DeviceTokens = deviceTokens;
                context.Landlords.Update(landlord);
            }
            await context.SaveChangesAsync();
        }

    }
}
