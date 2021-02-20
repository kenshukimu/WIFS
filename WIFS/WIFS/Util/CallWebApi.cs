using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WIFS.Util
{
    public class CallWebApi
    {
        public CallWebApi() { }


        static async Task<String> CallGetApi(String methodName)
        {
            String rtn = String.Empty;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://"+ InitSetting.CConf.serverIP + ":3000");
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = new HttpResponseMessage();

                    response = await client.GetAsync(methodName).ConfigureAwait(true);

                    response.EnsureSuccessStatusCode(); // 오류 코드를 던집니다.

                    // Verification  
                    if (response.IsSuccessStatusCode)
                    {
                        rtn = response.Content.ReadAsStringAsync().Result;
                    }
                }
            }
            catch (Newtonsoft.Json.JsonException jEx)
            {
                // 이 예외는 요청 본문을 역직렬화 할 때, 문제가 발생했음을 나타냅니다.
                rtn = jEx.ToString();
            }
            catch (HttpRequestException ex)
            {
                rtn = ex.Message.ToString();
            }
            return rtn;
        }

        public async Task<String> CallPostApiUsers(String methodName, UserEntity ue)
        {
            String rtn = String.Empty;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://" + InitSetting.CConf.serverIP + ":3000");
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = new HttpResponseMessage();
                    
                    response = await client.PostAsJsonAsync(methodName, ue).ConfigureAwait(false);

                    response.EnsureSuccessStatusCode(); // 오류 코드를 던집니다.

                    // Verification  
                    if (response.IsSuccessStatusCode)
                    {
                        rtn = response.Content.ReadAsStringAsync().Result;
                    }
                }
            }
            catch (Newtonsoft.Json.JsonException jEx)
            {
                // 이 예외는 요청 본문을 역직렬화 할 때, 문제가 발생했음을 나타냅니다.
                rtn = jEx.ToString();
            }
            catch (HttpRequestException ex)
            {
                rtn = ex.Message.ToString();
            }
            return rtn;
        }

        public async Task<String> CallPostApiWorks(String methodName, WorkEntity we)
        {
            String rtn = String.Empty;
            try
            {   
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://" + InitSetting.CConf.serverIP + ":3000");
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = new HttpResponseMessage();

                    response = await client.PostAsJsonAsync(methodName, we).ConfigureAwait(false);

                    response.EnsureSuccessStatusCode(); // 오류 코드를 던집니다.

                    // Verification  
                    if (response.IsSuccessStatusCode)
                    {
                        rtn = response.Content.ReadAsStringAsync().Result;
                    }
                }
            }
            catch (Newtonsoft.Json.JsonException jEx)
            {
                // 이 예외는 요청 본문을 역직렬화 할 때, 문제가 발생했음을 나타냅니다.
                rtn = jEx.ToString();
            }
            catch (HttpRequestException ex)
            {
                rtn = ex.Message.ToString();
            }
            return rtn;
        }

        public async Task<String> CallPostApiWeeks(String methodName, WeekEntity we)
        {
            String rtn = String.Empty;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://" + InitSetting.CConf.serverIP + ":3000");
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = new HttpResponseMessage();

                    response = await client.PostAsJsonAsync(methodName, we).ConfigureAwait(false);

                    response.EnsureSuccessStatusCode(); // 오류 코드를 던집니다.

                    // Verification  
                    if (response.IsSuccessStatusCode)
                    {
                        rtn = response.Content.ReadAsStringAsync().Result;
                    }
                }
            }
            catch (Newtonsoft.Json.JsonException jEx)
            {
                // 이 예외는 요청 본문을 역직렬화 할 때, 문제가 발생했음을 나타냅니다.
                rtn = jEx.ToString();
            }
            catch (HttpRequestException ex)
            {
                rtn = ex.Message.ToString();
            }
            return rtn;
        }
    }
}
