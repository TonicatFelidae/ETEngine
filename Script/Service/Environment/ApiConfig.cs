using System;
using UnityEngine;

namespace ETEngine.Service
{
    public enum ApiEnvironment
    {
        Local,
        Staging,
        Production
    }

    [CreateAssetMenu(fileName = "ApiConfig", menuName = "Environment/Api Config")]
    public class ApiConfig : ScriptableObject
    {
        public ApiEnvironment environment = ApiEnvironment.Staging;
        public string localUrl = "http://localhost:3000";
        public string stagingUrl = "https://example.xxx.com";
        public string productionUrl = "https://example.com";

        static ApiConfig _instance;
        public static ApiConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<ApiConfig>("ApiConfig");
                }
                return _instance;
            }
        }

        public string BaseUrl
        {
            get
            {
                var envUrl = Environment.GetEnvironmentVariable("PROJECT_API_URL");
                if (!string.IsNullOrEmpty(envUrl))
                {
                    return envUrl.TrimEnd('/');
                }

                return environment switch
                {
                    ApiEnvironment.Local => localUrl.TrimEnd('/'),
                    ApiEnvironment.Staging => stagingUrl.TrimEnd('/'),
                    ApiEnvironment.Production => productionUrl.TrimEnd('/'),
                    _ => localUrl.TrimEnd('/')
                };
            }
        }
    }
}
