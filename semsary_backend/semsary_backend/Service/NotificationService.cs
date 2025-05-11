using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;

namespace semsary_backend.Service
{
    public class NotificationService
    {
        private static bool _isInitialized = false;
        private static readonly object _lock = new();


        public NotificationService()
        {
            InitializeFirebase();
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
                        Credential = GoogleCredential.FromFile("firebase_key.json")
                    });
                    _isInitialized = true;
                }
            }
        }
        public void AddDeviceToken(string deviceToken , List<string> deviceTokens)
        {
            if (!deviceTokens.Contains(deviceToken))
                deviceTokens.Add(deviceToken);
        }

        public async Task SendNotificationAsync(string title , string message, List<string> deviceTokens)
        {
            if (deviceTokens == null || deviceTokens.Count == 0)
                return;

            var multicastMessage = new MulticastMessage
            {
                Notification = new Notification
                {
                    Title = title,
                    Body = message
                },
                Tokens = deviceTokens
            };

            var response = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(multicastMessage);

            for (int i = 0; i < response.Responses.Count; i++)
            {
                var result = response.Responses[i];
                if (!result.IsSuccess)
                {
                    var failedToken = deviceTokens[i];
                    var errorCode = result.Exception?.Message ?? "";

                    if (errorCode.Contains("registration-token-not-registered") || errorCode.Contains("invalid-argument"))
                    {
                         deviceTokens.Remove(failedToken); 
                    }
                }
            }
        }


    }
}
