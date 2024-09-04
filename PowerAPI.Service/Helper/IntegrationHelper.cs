using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PowerAPI.Service.Helper
{
    public class IntegrationHelper
    {
        private readonly HttpClient _httpClient;
        private Uri BaseEndpoint { get; set; }

        private static JsonSerializerSettings MicrosoftDateFormatSettings
        {
            get
            {
                return new JsonSerializerSettings
                {
                    DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
                };
            }
        }


        public IntegrationHelper(Uri baseEndpoint)
        {
            if (baseEndpoint == null)
            {
                throw new ArgumentNullException("baseEndpoint");
            }
            BaseEndpoint = baseEndpoint;
            _httpClient = new HttpClient();
        }

        public IntegrationHelper(IHttpClientFactory client)
        {
            _httpClient = client.CreateClient("PowerAPI");
            BaseEndpoint = _httpClient.BaseAddress;
        }
        public HttpClient Initial()
        {
            var client = new HttpClient();
            try
            {
                client.BaseAddress = new Uri("http://localhost/API");
            }
            catch (HttpRequestException ex)
            {
            }
            catch (Exception ex)
            {
            }

            return client;
        }

        /// <summary>  
        /// Common method for making GET calls  
        /// </summary>  
        public async Task<T> GetAsync<T>(Uri requestUrl)
        {

            try
            {
                addHeaders();
                var response = await _httpClient.GetAsync(requestUrl, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();
                var data = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(data);

            }
            catch (Exception ex)
            {
                return JsonConvert.DeserializeObject<T>(null);
            }


        }


        /// <summary>  
        /// Common method for making POST calls  
        /// </summary>  
        //public async Task<T> PostAsync<T>(Uri requestUrl, T content)
        //{
        //    addHeaders();
        //    var response = await _httpClient.PostAsync(requestUrl, CreateHttpContent<T>(content));
        //    response.EnsureSuccessStatusCode();
        //    var data = await response.Content.ReadAsStringAsync();
        //    return JsonConvert.DeserializeObject<T>(data);
        //}
        public async Task<T1> PostAsync<T1, T2>(Uri requestUrl, T2 content)
        {
            addHeaders();
            var response = await _httpClient.PostAsync(requestUrl.ToString(), CreateHttpContent<T2>(content));
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T1>(data);
        }

        public async Task<T1> PutAsync<T1, T2>(Uri requestUrl, T2 content)
        {
            addHeaders();
            var response = await _httpClient.PutAsync(requestUrl.ToString(), CreateHttpContent<T2>(content));
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T1>(data);
        }

        public Uri CreateRequestUri(string relativePath, string queryString = "")
        {
            var endpoint = new Uri(BaseEndpoint, relativePath);
            var uriBuilder = new UriBuilder(endpoint);
            uriBuilder.Query = queryString;
            return uriBuilder.Uri;
        }

        private HttpContent CreateHttpContent<T>(T content)
        {
            var json = JsonConvert.SerializeObject(content, MicrosoftDateFormatSettings);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }


        private void addHeaders()
        {
            //_httpClient.DefaultRequestHeaders.Remove("userIP");
            //_httpClient.DefaultRequestHeaders.Add("userIP", "192.168.1.1");
        }

        public async Task<T1> PostAsync<T1, T2>(Uri requestUrl, T2 content, string value)
        {
            addHeaders(value);
            var response = await _httpClient.PostAsync(requestUrl.ToString(), CreateHttpContent<T2>(content));
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T1>(data);
        }


        private void addHeaders(string value)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", value);
        }

        public async Task<T> GetAsync<T>(Uri requestUrl, string value)
        {

            try
            {
                addHeaders(value);
                var response = await _httpClient.GetAsync(requestUrl, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();
                var data = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(data);

            }
            catch (Exception ex)
            {
                return JsonConvert.DeserializeObject<T>(null);
            }


        }

    }
}
